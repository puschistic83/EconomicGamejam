using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    [Header("Starting Resources")]
    public int startWood = 100;
    public int startOre = 50;
    public int startGold = 200;
    public int startFood = 100;

    [Header("Current Resources (Read Only)")]
    [SerializeField] private int _wood;
    [SerializeField] private int _ore;
    [SerializeField] private int _gold;
    [SerializeField] private int _food;

    // Событие для обновления UI
    public event Action OnResourcesUpdated;

    // Публичные свойства с вызовом события при изменении
    public int wood
    {
        get => _wood;
        private set
        {
            _wood = value;
            OnResourcesUpdated?.Invoke();
        }
    }

    public int ore
    {
        get => _ore;
        private set
        {
            _ore = value;
            OnResourcesUpdated?.Invoke();
        }
    }

    public int gold
    {
        get => _gold;
        private set
        {
            _gold = value;
            OnResourcesUpdated?.Invoke();
        }
    }

    public int food
    {
        get => _food;
        private set
        {
            _food = value;
            OnResourcesUpdated?.Invoke();
        }
    }

    void Start()
    {
        // Инициализируем ресурсы из настроек инспектора
        InitializeResources();
    }

    public void InitializeResources()
    {
        wood = startWood;
        ore = startOre;
        gold = startGold;
        food = startFood;

        Debug.Log($"Resources initialized: Wood={wood}, Ore={ore}, Gold={gold}, Food={food}");
    }

    public void ResetToStartValues()
    {
        InitializeResources();
    }

    public bool CanAfford(int woodCost, int oreCost, int goldCost, int foodCost)
    {
        return wood >= woodCost && ore >= oreCost && gold >= goldCost && food >= foodCost;
    }

    public bool SpendResources(int woodCost, int oreCost, int goldCost, int foodCost)
    {
        if (CanAfford(woodCost, oreCost, goldCost, foodCost))
        {
            wood -= woodCost;
            ore -= oreCost;
            gold -= goldCost;
            food -= foodCost;
            return true;
        }
        return false;
    }

    public void AddResources(int woodAmount, int oreAmount, int goldAmount, int foodAmount)
    {
        wood += woodAmount;
        ore += oreAmount;
        gold += goldAmount;
        food += foodAmount;
    }

    // Метод для установки конкретных значений (например, для cheat-кодов или тестов)
    public void SetResources(int newWood, int newOre, int newGold, int newFood)
    {
        wood = newWood;
        ore = newOre;
        gold = newGold;
        food = newFood;
    }

    // Метод для проверки, достаточно ли ресурсов
    public bool HasEnoughWood(int amount) => wood >= amount;
    public bool HasEnoughOre(int amount) => ore >= amount;
    public bool HasEnoughGold(int amount) => gold >= amount;
    public bool HasEnoughFood(int amount) => food >= amount;
}