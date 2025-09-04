using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // �������
    public int Money { get; set; } = 200;
    public int Goods { get; set; } = 50;
    public float Happiness { get; set; } = 70f;
    public float InflationRate { get; set; } = 0.0f;

    [Header("��������� ����")]
    public float turnDuration = 10f; // ������������ ������ ���� � ��������
    public float baseInflationEffect = 0.01f; // ������� ������� �� ��������
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

    // ������ ���� ����������� ������
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
        Debug.Log("���� ��������! ������� ������� ��� ������������ �������.");
    }

    void Update()
    {
        // ��������� ������ ����
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

        // 1. ���������� ������� �� ���� ������
        ProduceResources();

        // 2. ��������� ��������
        ApplyInflation();

        // 3. ��������� UI
        UpdateUI();

        // 4. �������� ���������� � ����
        Debug.Log($"��� #{turnCount} ��������! " +
                 $"������: {Goods} (+{CalculateTotalProduction()}), " +
                 $"��������: {InflationRate:P0}");
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
        // ������������ ����� �������� �� ������ ������� ����� � �������
        CalculateNewInflation();

        // ��������� ������� �������� � ���������
        ApplyInflationEffects();
    }

    void CalculateNewInflation()
    {
        // ������� ������: ����������� �������� ����� � �������
        if (Goods == 0) return; // ������ �� ������� �� ����

        float moneySupplyRatio = (float)Money / Goods;

        // ������� ��������: ��� ������ ����� ������������ ������� - ��� ���� ��������
        float newInflation = moneySupplyRatio * baseInflationEffect;

        // ������� ��������� ��������
        InflationRate = Mathf.Lerp(InflationRate, newInflation, 0.1f);

        // ������������ �������� �������� (0% - 100%)
        InflationRate = Mathf.Clamp(InflationRate, 0f, 1f);
    }

    void ApplyInflationEffects()
    {
        // 1. �������� "�������" ����� ������� (�������������)
        int inflationLoss = Mathf.RoundToInt(Goods * InflationRate * 0.3f);
        Goods = Mathf.Max(0, Goods - inflationLoss);

        // 2. �������� ������� ���������� ���������
        float happinessLoss = InflationRate * 8f;
        Happiness = Mathf.Clamp(Happiness - happinessLoss, 0f, 100f);

        // 3. ������� �������� ������� ������������� ������������
        if (InflationRate > 0.3f)
        {
            float productionPenalty = (InflationRate - 0.3f) * 0.5f;
            // ����� ��������� ����� � ������� � ��������� ����
        }
    }

    public void AddBuildingToManager(Building building)
    {
        if (allBuildings == null)
            allBuildings = new List<Building>();

        allBuildings.Add(building);
        Debug.Log($"������ ���������! ����� ������: {allBuildings.Count}");
    }

    public void PrintMoney(int amount = 50)
    {
        Money += amount;
        // ������ ����� �������� ����������� ��������
        InflationRate += 0.07f;
        Debug.Log($"���������� {amount} �����. �������� +7%");
        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        int newMoney = Money + amount;
        if (newMoney < 0)
        {
            Debug.LogWarning("������� ���� � ������������� ������!");
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
        moneyText.text = $"������: {Money}";
        goodsText.text = $"������: {Goods}";
        happinessText.text = $"����������: {Mathf.RoundToInt(Happiness)}%";
        inflationText.text = $"��������: {InflationRate:P0}";
        turnText.text = $"���: {turnCount}";

        // �������� ��������� ��������
        if (InflationRate > 0.3f)
            inflationText.color = Color.red;
        else if (InflationRate > 0.15f)
            inflationText.color = Color.yellow;
        else
            inflationText.color = Color.green;

        // �������� ��������� ����������
        if (Happiness < 30f)
            happinessText.color = Color.red;
        else if (Happiness < 60f)
            happinessText.color = Color.yellow;
        else
            happinessText.color = Color.green;
    }

    // ����� ��� ������ ������ �����
    public void OnPrintMoneyButton()
    {
        PrintMoney(50);
    }
}