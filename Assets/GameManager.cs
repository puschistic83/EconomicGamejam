using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Singleton паттерн для легкого доступа из других скриптов
    public static GameManager Instance;

    // Ресурсы (сделаем свойствами с уведомлением об изменении)
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

    // Список всех построенных зданий
    private List<Building> allBuildings = new List<Building>();

    void Awake()
    {
        // Инициализируем Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
        // Запускаем игровые циклы
        InvokeRepeating("NextTurn", 5.0f, 5.0f);
    }
    public void AddMoney(int amount)
    {
        int newMoney = Money + amount;

        // Запрещаем уходить в минус
        if (newMoney < 0)
        {
            Debug.LogWarning("Попытка уйти в отрицательный баланс! Операция отклонена.");
            return;
        }

        Money = newMoney;
        UpdateUI();
    }
    // Метод для попытки постройки здания
    public bool TryBuildBuilding(GameObject buildingPrefab)
    {
        Building buildingScript = buildingPrefab.GetComponent<Building>();

        if (buildingScript == null) return false;
        if (Money < buildingScript.buildCost) return false;

        // Вычитаем стоимость постройки
        Money -= buildingScript.buildCost;

        // Здесь будет логика размещения на карте
        // Пока просто добавляем в список "виртуально"
        GameObject newBuilding = Instantiate(buildingPrefab);
        Building newBuildingScript = newBuilding.GetComponent<Building>();
        allBuildings.Add(newBuildingScript);

        // Сразу применяем эффекты первой постройки
        newBuildingScript.ApplyProductionEffects();

        UpdateUI();
        return true;
    }

    // Методы для кнопок UI
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
        InflationRate += 0.05f; // +5% инфляции
        UpdateUI();
    }

    void NextTurn()
    {
        // Применяем эффекты всех зданий
        foreach (Building building in allBuildings)
        {
            building.ApplyProductionEffects();
        }

        // Применяем инфляцию (упрощенная модель)
        ApplyInflation();

        UpdateUI();
    }

    void ApplyInflation()
    {
        // Инфляция уменьшает эффективность производства
        float inflationMultiplier = 1 - (InflationRate / 2f);
        Goods = Mathf.RoundToInt(Goods * inflationMultiplier);

        // Инфляция также уменьшает настроение
        Happiness -= InflationRate * 10f;
    }

   public void UpdateUI()
    {
        moneyText.text = $"Деньги: {Money}";
        goodsText.text = $"Товары: {Goods}";
        happinessText.text = $"Настроение: {Mathf.RoundToInt(Happiness)}%";
        inflationText.text = $"Инфляция: {InflationRate:P0}";
    }

    // Методы для изменения ресурсов (лучше делать через свойства)
    public void AddGoods(int amount) => Goods += amount;
    public void AddHappiness(float amount) => Happiness = Mathf.Clamp(Happiness + amount, 0, 100);

    // Метод для добавления зданий в список (вызывается из BuildingPlacer)
    public void AddBuildingToManager(Building building)
    {
        if (allBuildings == null)
            allBuildings = new List<Building>();

        allBuildings.Add(building);
        Debug.Log($"Здание добавлено! Всего зданий: {allBuildings.Count}");
    }
}