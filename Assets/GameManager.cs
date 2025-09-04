using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Ресурсы
    public int Money { get; set; } = 200;
    public int Goods { get; set; } = 50;
    public float Happiness { get; set; } = 70f;
    public float InflationRate { get; set; } = 0.0f;

    [Header("Настройки игры")]
    public float turnDuration = 10f; // Длительность одного хода в секундах
    public float baseInflationEffect = 0.01f; // Базовое влияние на инфляцию
    private float turnTimer = 0f;
    private int turnCount = 0;

    [Header("UI Elements")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI goodsText;
    public TextMeshProUGUI happinessText;
    public TextMeshProUGUI inflationText;
    public TextMeshProUGUI turnText;
    public Slider turnProgressSlider;

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
        UpdateUI();
        Debug.Log("Игра началась! Строите фабрики для производства товаров.");
    }

    void Update()
    {
        // Обновляем таймер хода
        UpdateTurnTimer();
    }

    void UpdateTurnTimer()
    {
        turnTimer += Time.deltaTime;
        float progress = turnTimer / turnDuration;
        turnProgressSlider.value = progress;

        if (turnTimer >= turnDuration)
        {
            CompleteTurn();
            turnTimer = 0f;
        }
    }

    void CompleteTurn()
    {
        turnCount++;

        // 1. Производим ресурсы со всех зданий
        ProduceResources();

        // 2. Применяем инфляцию
        ApplyInflation();

        // 3. Обновляем UI
        UpdateUI();

        // 4. Логируем информацию о ходе
        Debug.Log($"Ход #{turnCount} завершен! " +
                 $"Товары: {Goods} (+{CalculateTotalProduction()}), " +
                 $"Инфляция: {InflationRate:P0}");
    }

    int CalculateTotalProduction()
    {
        int total = 0;
        if (allBuildings != null)
        {
            foreach (Building building in allBuildings)
            {
                total += building.goodsEffect;
            }
        }
        return total;
    }

    void ProduceResources()
    {
        if (allBuildings == null) return;

        foreach (Building building in allBuildings)
        {
            building.ApplyProductionEffects();
        }
    }

    void ApplyInflation()
    {
        // Рассчитываем новую инфляцию на основе баланса денег и товаров
        CalculateNewInflation();

        // Применяем эффекты инфляции к экономике
        ApplyInflationEffects();
    }

    void CalculateNewInflation()
    {
        // Базовый расчет: соотношение денежной массы к товарам
        if (Goods == 0) return; // Защита от деления на ноль

        float moneySupplyRatio = (float)Money / Goods;

        // Формула инфляции: чем больше денег относительно товаров - тем выше инфляция
        float newInflation = moneySupplyRatio * baseInflationEffect;

        // Плавное изменение инфляции
        InflationRate = Mathf.Lerp(InflationRate, newInflation, 0.1f);

        // Ограничиваем диапазон инфляции (0% - 100%)
        InflationRate = Mathf.Clamp(InflationRate, 0f, 1f);
    }

    void ApplyInflationEffects()
    {
        // 1. Инфляция "съедает" часть товаров (обесценивание)
        int inflationLoss = Mathf.RoundToInt(Goods * InflationRate * 0.3f);
        Goods = Mathf.Max(0, Goods - inflationLoss);

        // 2. Инфляция снижает настроение населения
        float happinessLoss = InflationRate * 8f;
        Happiness = Mathf.Clamp(Happiness - happinessLoss, 0f, 100f);

        // 3. Высокая инфляция снижает эффективность производства
        if (InflationRate > 0.3f)
        {
            float productionPenalty = (InflationRate - 0.3f) * 0.5f;
            // Можно применить штраф к зданиям в следующем ходу
        }
    }

    public void AddBuildingToManager(Building building)
    {
        if (allBuildings == null)
            allBuildings = new List<Building>();

        allBuildings.Add(building);
        Debug.Log($"Здание добавлено! Всего зданий: {allBuildings.Count}");
    }

    public void PrintMoney(int amount = 50)
    {
        Money += amount;
        // Печать денег напрямую увеличивает инфляцию
        InflationRate += 0.07f;
        Debug.Log($"Напечатано {amount} денег. Инфляция +7%");
        UpdateUI();
    }

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

    public void UpdateUI()
    {
        moneyText.text = $"Деньги: {Money}";
        goodsText.text = $"Товары: {Goods}";
        happinessText.text = $"Настроение: {Mathf.RoundToInt(Happiness)}%";
        inflationText.text = $"Инфляция: {InflationRate:P0}";
        turnText.text = $"Ход: {turnCount}";

        // Цветовая индикация инфляции
        if (InflationRate > 0.3f)
            inflationText.color = Color.red;
        else if (InflationRate > 0.15f)
            inflationText.color = Color.yellow;
        else
            inflationText.color = Color.green;

        // Цветовая индикация настроения
        if (Happiness < 30f)
            happinessText.color = Color.red;
        else if (Happiness < 60f)
            happinessText.color = Color.yellow;
        else
            happinessText.color = Color.green;
    }

    // Метод для кнопки печати денег
    public void OnPrintMoneyButton()
    {
        PrintMoney(50);
    }
}