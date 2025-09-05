using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Ресурсы
    public int Money { get; set; } = 300;
    public int Goods { get; set; } = 30;
    public float Happiness { get; set; } = 70f;
    public float InflationRate { get; set; } = 0.05f;

    [Header("Настройки игры")]
    public float turnDuration = 10f;
    public float baseInflationEffect = 0.01f;
    private float turnTimer = 0f;
    private int turnCount = 0;

    [Header("Налоговая система")]
    public float taxRate = 0.15f;
    public int taxCollectionTurnInterval = 3;
    private int turnsSinceLastTaxCollection = 0;

    [Header("Торговая система")]
    public int exportPricePerGood = 3;
    public int importPricePerGood = 5;
    public int autoExportThreshold = 50;

    [Header("Экономические инструменты")]
    public float interestRate = 0.05f;
    public int subsidyAmount = 30;
    public float printMoneyInflationEffect = 0.07f;
    public float taxChangeHappinessEffect = 5f;
    public float interestRateInvestmentEffect = 0.1f;

    [Header("UI Elements")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI goodsText;
    public TextMeshProUGUI happinessText;
    public TextMeshProUGUI inflationText;
    public TextMeshProUGUI turnText;
    public Slider turnProgressSlider;
    public TextMeshProUGUI taxRateText;
    public TextMeshProUGUI interestRateText;
    public TextMeshProUGUI nextTaxText;
    public TextMeshProUGUI maintenanceText;

    [Header("Building Prefabs")]
    public GameObject factoryPrefab;
    public GameObject housePrefab;

    // Список всех построенных зданий
    private List<Building> allBuildings = new List<Building>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Инициализируем кнопки UI
        InitializeUIButtons();

        UpdateUI();
        Debug.Log("Игра началась! Следите за балансом доходов и расходов.");
    }

    void Update()
    {
        UpdateTurnTimer();
    }

    void UpdateTurnTimer()
    {
        turnTimer += Time.deltaTime;
        if (turnProgressSlider != null)
        {
            turnProgressSlider.value = turnTimer / turnDuration;
        }

        if (turnTimer >= turnDuration)
        {
            CompleteTurn();
            turnTimer = 0f;
        }
    }

    void CompleteTurn()
    {
        turnCount++;

        ApplyTaxes();
        ProduceResources();
        HandleConsumption(); // ← ДОБАВИЛИ ЗДЕСЬ
        HandleTrade();
        ApplyInflation();
        PayMaintenanceCosts();

        UpdateUI();

        Debug.Log($"Ход #{turnCount} завершен! Товары: {Goods}, Деньги: {Money}, Инфляция: {InflationRate:P0}");
    }

    void ApplyTaxes()
    {
        turnsSinceLastTaxCollection++;

        if (turnsSinceLastTaxCollection >= taxCollectionTurnInterval)
        {
            CollectTaxes();
            turnsSinceLastTaxCollection = 0;
        }
    }

    void CollectTaxes()
    {
        int populationTax = CalculatePopulationTax();
        int businessTax = CalculateBusinessTax();
        int totalTax = populationTax + businessTax;

        Money += totalTax;

        Debug.Log($"Собрано налогов: {totalTax} (Население: {populationTax}, Бизнес: {businessTax})");
        ShowFloatingText($"Налоги: +{totalTax}", Color.green);
    }

    int CalculatePopulationTax()
    {
        int totalPopulation = 0;
        foreach (Building building in allBuildings)
        {
            if (building is HouseBuilding)
            {
                totalPopulation += 5;
            }
        }
        return Mathf.RoundToInt(totalPopulation * taxRate * 10);
    }

    int CalculateBusinessTax()
    {
        int totalBusinessIncome = 0;
        foreach (Building building in allBuildings)
        {
            if (building is FactoryBuilding)
            {
                totalBusinessIncome += Mathf.RoundToInt(building.goodsEffect * 2 * taxRate);
            }
        }
        return totalBusinessIncome;
    }

    int CalculateNextMaintenance()
    {
        int totalMaintenance = 0;
        foreach (Building building in allBuildings)
        {
            if (building is FactoryBuilding)
                totalMaintenance += 2;
            else if (building is HouseBuilding)
                totalMaintenance += 1;
        }
        return totalMaintenance;
    }

    void ProduceResources()
    {
        if (allBuildings == null) return;

        foreach (Building building in allBuildings)
        {
            building.ApplyProductionEffects();
        }
    }

    void HandleTrade()
    {
        if (Goods > autoExportThreshold)
        {
            int excessGoods = Goods - autoExportThreshold;
            int exportAmount = Mathf.Min(excessGoods, 10);
            int exportIncome = exportAmount * exportPricePerGood;

            Goods -= exportAmount;
            Money += exportIncome;

            Debug.Log($"Авто-экспорт: {exportAmount} товаров → +{exportIncome} денег");
        }
    }

    void ApplyInflation()
    {
        CalculateNewInflation();
        ApplyInflationEffects();
    }

    void CalculateNewInflation()
    {
        if (Goods == 0) return;

        float moneySupplyRatio = (float)Money / Goods;
        float newInflation = moneySupplyRatio * baseInflationEffect;

        InflationRate = Mathf.Lerp(InflationRate, newInflation, 0.1f);
        InflationRate = Mathf.Clamp(InflationRate, 0f, 1f);
    }

    void ApplyInflationEffects()
    {
        int inflationLoss = Mathf.RoundToInt(Goods * InflationRate * 0.3f);
        Goods = Mathf.Max(0, Goods - inflationLoss);

        float happinessLoss = InflationRate * 8f;
        Happiness = Mathf.Clamp(Happiness - happinessLoss, 0f, 100f);
    }

    void PayMaintenanceCosts()
    {
        int totalMaintenance = CalculateNextMaintenance();

        Money -= totalMaintenance;

        if (totalMaintenance > 0)
        {
            Debug.Log($"Расходы на содержание: -{totalMaintenance} денег");
        }
    }

    // Экономические инструменты
    public void PrintMoney(int amount = 50)
    {
        Money += amount;
        InflationRate += printMoneyInflationEffect;

        Debug.Log($"Напечатано {amount} денег. Инфляция +{printMoneyInflationEffect:P0}");
        ShowFloatingText($"+{amount} денег\nИнфляция +{printMoneyInflationEffect:P0}", Color.yellow);

        UpdateUI();
    }

    public void AdjustTaxes(bool increase)
    {
        float oldTaxRate = taxRate;

        if (increase)
        {
            taxRate = Mathf.Clamp(taxRate + 0.05f, 0.1f, 0.5f);
            Money += Mathf.RoundToInt(Money * 0.1f);
            Happiness -= taxChangeHappinessEffect;

            Debug.Log($"Налоги повышены до {taxRate:P0}");
            ShowFloatingText($"Налоги ↑\n+{Mathf.RoundToInt(Money * 0.1f)} денег\nНастроение ↓", Color.red);
        }
        else
        {
            taxRate = Mathf.Clamp(taxRate - 0.05f, 0.1f, 0.5f);
            Happiness += taxChangeHappinessEffect;

            Debug.Log($"Налоги понижены до {taxRate:P0}");
            ShowFloatingText($"Налоги ↓\nНастроение ↑", Color.green);
        }

        UpdateUI();
    }

    public void GiveSubsidy()
    {
        if (Money < subsidyAmount)
        {
            Debug.Log("Недостаточно денег для субсидий!");
            return;
        }

        Money -= subsidyAmount;

        foreach (Building building in allBuildings)
        {
            if (building.goodsEffect > 0)
            {
                building.goodsEffect += 2;
            }
        }

        StartCoroutine(RemoveSubsidyEffect(3));

        Debug.Log($"Выданы субсидии производства! +2 к производству на 3 хода");
        ShowFloatingText($"Субсидии!\nПроизводство ↑", Color.blue);

        UpdateUI();
    }

    private IEnumerator RemoveSubsidyEffect(int turns)
    {
        for (int i = 0; i < turns; i++)
        {
            yield return new WaitForSeconds(turnDuration);
        }

        foreach (Building building in allBuildings)
        {
            if (building.goodsEffect > 2)
            {
                building.goodsEffect -= 2;
            }
        }

        Debug.Log("Действие субсидий закончилось");
    }

    public void AdjustInterestRates(bool increase)
    {
        float oldRate = interestRate;

        if (increase)
        {
            interestRate = Mathf.Clamp(interestRate + 0.01f, 0.01f, 0.1f);
            InflationRate -= 0.03f;
            foreach (Building building in allBuildings)
            {
                building.goodsEffect = Mathf.RoundToInt(building.goodsEffect * 0.95f);
            }

            Debug.Log($"Учетная ставка повышена до {interestRate:P0}");
            ShowFloatingText($"Ставка ↑\nИнфляция ↓\nПроизводство ↓", Color.cyan);
        }
        else
        {
            interestRate = Mathf.Clamp(interestRate - 0.01f, 0.01f, 0.1f);
            InflationRate += 0.02f;
            foreach (Building building in allBuildings)
            {
                building.goodsEffect = Mathf.RoundToInt(building.goodsEffect * 1.05f);
            }

            Debug.Log($"Учетная ставка понижена до {interestRate:P0}");
            ShowFloatingText($"Ставка ↓\nПроизводство ↑\nИнфляция ↑", Color.magenta);
        }

        UpdateUI();
    }

    public void ExportGoods(int amount = 10)
    {
        if (Goods < amount)
        {
            Debug.Log("Недостаточно товаров для экспорта!");
            return;
        }

        Goods -= amount;
        Money += amount * exportPricePerGood;

        Debug.Log($"Экспорт: {amount} товаров → +{amount * exportPricePerGood} денег");
        ShowFloatingText($"Экспорт: +{amount * exportPricePerGood}", Color.blue);

        UpdateUI();
    }

    public void ImportGoods(int amount = 10)
    {
        int cost = amount * importPricePerGood;

        if (Money < cost)
        {
            Debug.Log("Недостаточно денег для импорта!");
            return;
        }

        Money -= cost;
        Goods += amount;

        Debug.Log($"Импорт: {amount} товаров → -{cost} денег");
        ShowFloatingText($"Импорт: +{amount} товаров", Color.cyan);

        UpdateUI();
    }

    // Управление зданиями
    public void AddBuildingToManager(Building building)
    {
        if (allBuildings == null)
            allBuildings = new List<Building>();

        allBuildings.Add(building);
        Debug.Log($"Здание добавлено! Всего зданий: {allBuildings.Count}");
        UpdateUI();
    }

    // Управление ресурсами
    public void AddMoney(int amount)
    {
        int newMoney = Money + amount;
        if (newMoney < 0)
        {
            Debug.LogWarning("Попытка уйти в отрицательный баланс!");
            return;
        }
        Money = newMoney;
    }

    public void AddGoods(int amount)
    {
        Goods = Mathf.Max(0, Goods + amount);
    }

    public void AddHappiness(float amount)
    {
        Happiness = Mathf.Clamp(Happiness + amount, 0f, 100f);
    }

    // UI
    public void UpdateUI()
    {
        if (this == null) return;

        try
        {
            // Основные ресурсы
            if (moneyText != null) moneyText.text = $"Деньги: {Money}";
            if (goodsText != null) goodsText.text = $"Товары: {Goods}";
            if (happinessText != null) happinessText.text = $"Настроение: {Mathf.RoundToInt(Happiness)}%";
            if (inflationText != null) inflationText.text = $"Инфляция: {InflationRate:P0}";
            if (turnText != null) turnText.text = $"Ход: {turnCount}";

            // Экономические показатели
            if (taxRateText != null) taxRateText.text = $"Налоги: {taxRate:P0}";
            if (interestRateText != null) interestRateText.text = $"Ставка: {interestRate:P0}";
            if (nextTaxText != null) nextTaxText.text = $"След. налоги: +{CalculatePopulationTax() + CalculateBusinessTax()}";
            if (maintenanceText != null) maintenanceText.text = $"Расходы: -{CalculateNextMaintenance()}";

            // Обновляем состояние кнопок
            UpdateButtonsInteractable();

            // Цветовая индикация
            UpdateTextColors();
        }
        catch (System.NullReferenceException e)
        {
            Debug.LogWarning($"UI update error: {e.Message}");
        }

        // Показываем потребление товаров
        if (GameObject.Find("ConsumptionText") != null)
        {
            GameObject.Find("ConsumptionText").GetComponent<Text>().text =
                $"Потребление: -{CalculateTotalConsumption()}";
        }

        // Цветовая индикация дефицита
        if (Goods < CalculateTotalConsumption())
        {
            if (goodsText != null) goodsText.color = Color.red;
        }
        else
        {
            if (goodsText != null) goodsText.color = Color.white;
        }
    }

    void UpdateTextColors()
    {
        if (inflationText != null)
        {
            if (InflationRate > 0.3f)
                inflationText.color = Color.red;
            else if (InflationRate > 0.15f)
                inflationText.color = Color.yellow;
            else
                inflationText.color = Color.green;
        }

        if (happinessText != null)
        {
            if (Happiness < 30f)
                happinessText.color = Color.red;
            else if (Happiness < 60f)
                happinessText.color = Color.yellow;
            else
                happinessText.color = Color.green;
        }
    }

    void ShowFloatingText(string text, Color color)
    {
        Debug.Log(text);
        // Здесь можно добавить визуальные эффекты
    }

    // Методы для кнопок UI
    public void OnPrintMoneyButton() => PrintMoney(50);
    public void OnTaxesUpButton() => AdjustTaxes(true);
    public void OnTaxesDownButton() => AdjustTaxes(false);
    public void OnSubsidiesButton() => GiveSubsidy();
    public void OnInterestUpButton() => AdjustInterestRates(true);
    public void OnInterestDownButton() => AdjustInterestRates(false);
    public void OnExportButton() => ExportGoods(10);
    public void OnImportButton() => ImportGoods(10);

    // В конец класса GameManager добавим:

    [Header("UI Кнопки инструментов")]
    public Button printMoneyButton;
    public Button taxesUpButton;
    public Button taxesDownButton;
    public Button subsidiesButton;
    public Button interestUpButton;
    public Button interestDownButton;
    public Button exportButton;
    public Button importButton;

    // Метод для инициализации кнопок
    public void InitializeUIButtons()
    {
        if (printMoneyButton != null)
            printMoneyButton.onClick.AddListener(OnPrintMoneyButton);

        if (taxesUpButton != null)
            taxesUpButton.onClick.AddListener(OnTaxesUpButton);

        if (taxesDownButton != null)
            taxesDownButton.onClick.AddListener(OnTaxesDownButton);

        if (subsidiesButton != null)
            subsidiesButton.onClick.AddListener(OnSubsidiesButton);

        if (interestUpButton != null)
            interestUpButton.onClick.AddListener(OnInterestUpButton);

        if (interestDownButton != null)
            interestDownButton.onClick.AddListener(OnInterestDownButton);

        if (exportButton != null)
            exportButton.onClick.AddListener(OnExportButton);

        if (importButton != null)
            importButton.onClick.AddListener(OnImportButton);
    }

    // Метод для обновления состояния кнопок (доступность)
    void UpdateButtonsInteractable()
    {
        // Печать денег всегда доступна
        if (printMoneyButton != null)
            printMoneyButton.interactable = true;

        // Субсидии требуют денег
        if (subsidiesButton != null)
            subsidiesButton.interactable = Money >= subsidyAmount;

        // Экспорт требует товаров
        if (exportButton != null)
            exportButton.interactable = Goods >= 10;

        // Импорт требует денег
        if (importButton != null)
            importButton.interactable = Money >= importPricePerGood * 10;

        // Налоги можно только повышать/понижать в пределах
        if (taxesUpButton != null)
            taxesUpButton.interactable = taxRate < 0.5f;

        if (taxesDownButton != null)
            taxesDownButton.interactable = taxRate > 0.1f;

        // Учетные ставки тоже ограничены
        if (interestUpButton != null)
            interestUpButton.interactable = interestRate < 0.1f;

        if (interestDownButton != null)
            interestDownButton.interactable = interestRate > 0.01f;
    }

    // В GameManager добавим метод для обработки потребления
    void HandleConsumption()
    {
        int totalConsumption = CalculateTotalConsumption();

        if (Goods >= totalConsumption)
        {
            Goods -= totalConsumption;
            if (totalConsumption > 0)
            {
                ShowFloatingText($"Потребление: -{totalConsumption}\nНаселение довольно", Color.green);
            }
        }
        else
        {
            int deficit = totalConsumption - Goods;
            Goods = 0;

            float happinessPenalty = deficit * 0.3f;
            AddHappiness(-happinessPenalty);

            ShowFloatingText($"ДЕФИЦИТ!\nНе хватает {deficit} товаров\nНастроение -{happinessPenalty:F1}", Color.red);

            // Образовательное сообщение
            Debug.Log("💡 Образовательный момент: При дефиците товаров цены растут, " +
                     "а население недовольно. Это приводит к инфляции и социальным проблемам.");
        }
    }

    int CalculateTotalConsumption()
    {
        int totalConsumption = 0;
        foreach (Building building in allBuildings)
        {
            if (building is HouseBuilding && building.goodsEffect < 0)
            {
                totalConsumption += Mathf.Abs(building.goodsEffect);
            }
        }
        return totalConsumption;
    }
}