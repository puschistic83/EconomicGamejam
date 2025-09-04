using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Singleton ������� ��� ������� ������� �� ������ ��������
    public static GameManager Instance;

    // ������� (������� ���������� � ������������ �� ���������)
    public int Money { get; set; } = 100;
    public int Goods { get; set; } = 20;
    public float Happiness { get; set; } = 50f;
    public float InflationRate { get; set; } = 0.0f;

    [Header("UI Elements")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI goodsText;
    public TextMeshProUGUI happinessText;
    public TextMeshProUGUI inflationText;

    [Header("Building Prefabs")]
    public GameObject factoryPrefab;
    public GameObject housePrefab;

    // ������ ���� ����������� ������
    private List<Building> allBuildings = new List<Building>();

    void Awake()
    {
        // �������������� Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
        // ��������� ������� �����
        InvokeRepeating("NextTurn", 5.0f, 5.0f);
    }
    public void AddMoney(int amount)
    {
        int newMoney = Money + amount;

        // ��������� ������� � �����
        if (newMoney < 0)
        {
            Debug.LogWarning("������� ���� � ������������� ������! �������� ���������.");
            return;
        }

        Money = newMoney;
        UpdateUI();
    }
    // ����� ��� ������� ��������� ������
    public bool TryBuildBuilding(GameObject buildingPrefab)
    {
        Building buildingScript = buildingPrefab.GetComponent<Building>();

        if (buildingScript == null) return false;
        if (Money < buildingScript.buildCost) return false;

        // �������� ��������� ���������
        Money -= buildingScript.buildCost;

        // ����� ����� ������ ���������� �� �����
        // ���� ������ ��������� � ������ "����������"
        GameObject newBuilding = Instantiate(buildingPrefab);
        Building newBuildingScript = newBuilding.GetComponent<Building>();
        allBuildings.Add(newBuildingScript);

        // ����� ��������� ������� ������ ���������
        newBuildingScript.ApplyProductionEffects();

        UpdateUI();
        return true;
    }

    // ������ ��� ������ UI
    //public void BuildFactory()
    //{
    //    TryBuildBuilding(factoryPrefab);
    //}

    //public void BuildHouse()
    //{
    //    TryBuildBuilding(housePrefab);
    //}

    public void PrintMoney()
    {
        Money += 50;
        InflationRate += 0.05f; // +5% ��������
        UpdateUI();
    }

    void NextTurn()
    {
        // ��������� ������� ���� ������
        foreach (Building building in allBuildings)
        {
            building.ApplyProductionEffects();
        }

        // ��������� �������� (���������� ������)
        ApplyInflation();

        UpdateUI();
    }

    void ApplyInflation()
    {
        // �������� ��������� ������������� ������������
        float inflationMultiplier = 1 - (InflationRate / 2f);
        Goods = Mathf.RoundToInt(Goods * inflationMultiplier);

        // �������� ����� ��������� ����������
        Happiness -= InflationRate * 10f;
    }

   public void UpdateUI()
    {
        moneyText.text = $"������: {Money}";
        goodsText.text = $"������: {Goods}";
        happinessText.text = $"����������: {Mathf.RoundToInt(Happiness)}%";
        inflationText.text = $"��������: {InflationRate:P0}";
    }

    // ������ ��� ��������� �������� (����� ������ ����� ��������)
    public void AddGoods(int amount) => Goods += amount;
    public void AddHappiness(float amount) => Happiness = Mathf.Clamp(Happiness + amount, 0, 100);

    // ����� ��� ���������� ������ � ������ (���������� �� BuildingPlacer)
    public void AddBuildingToManager(Building building)
    {
        if (allBuildings == null)
            allBuildings = new List<Building>();

        allBuildings.Add(building);
        Debug.Log($"������ ���������! ����� ������: {allBuildings.Count}");
    }
}