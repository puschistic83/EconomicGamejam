using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    [Header("Starting Resources")]
    public int startWood = 50;
    public int startOre = 25;
    public int startGold = 100;
    public int startFood = 50;

    [Header("Delivery Settings")]
    public float deliveryRange = 15f; // Настраиваемый радиус в инспекторе
    public bool requireMainBuilding = true; // Можно отключить требование главного здания

    [Header("Current Resources (Read Only)")]
    [SerializeField] private int _wood;
    [SerializeField] private int _ore;
    [SerializeField] private int _gold;
    [SerializeField] private int _food;

    [Header("Main Building")]
    public Building mainBuilding;

    public event Action OnResourcesUpdated;
    public event Action OnMainBuildingChanged;

    public int wood { get => _wood; set { _wood = value; OnResourcesUpdated?.Invoke(); } }
    public int ore { get => _ore; set { _ore = value; OnResourcesUpdated?.Invoke(); } }
    public int gold { get => _gold; set { _gold = value; OnResourcesUpdated?.Invoke(); } }
    public int food { get => _food; set { _food = value; OnResourcesUpdated?.Invoke(); } }

    public bool HasMainBuilding => mainBuilding != null;
    public bool CanBuildWithoutMainBuilding => !requireMainBuilding;

    void Start()
    {
        InitializeResources();
        Debug.Log($"Delivery range set to: {deliveryRange}m");
    }

    public void InitializeResources()
    {
        wood = startWood;
        ore = startOre;
        gold = startGold;
        food = startFood;
    }

    public void SetMainBuilding(Building building)
    {
        mainBuilding = building;
        OnMainBuildingChanged?.Invoke();
        Debug.Log($"Main building set. Delivery range: {deliveryRange}m");
    }

    public bool CanBuildHere(Vector3 position)
    {
        // Если не требуется главное здание - можно строить везде
        if (!requireMainBuilding) return true;

        // Если требуется главное здание, но его нет - нельзя строить
        if (!HasMainBuilding) return false;

        // Проверяем расстояние до главного здания
        return Vector3.Distance(position, mainBuilding.transform.position) <= deliveryRange;
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
}