using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    private List<Building> allBuildings = new List<Building>();
    private ResourceManager resourceManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
    }

    public void RegisterBuilding(Building building)
    {
        if (!allBuildings.Contains(building))
        {
            allBuildings.Add(building);
            Debug.Log($"Building registered: {building.data.buildingName}. Total: {allBuildings.Count}");
        }
    }

    public void UnregisterBuilding(Building building)
    {
        if (allBuildings.Contains(building))
        {
            allBuildings.Remove(building);
            Debug.Log($"Building removed: {building.data.buildingName}. Total: {allBuildings.Count}");
        }
    }

    public int GetTotalProduction(ResourceType resourceType)
    {
        int total = 0;
        foreach (Building building in allBuildings)
        {
            // ����������: ���������� isOperational ������ isActive
            if (building.isOperational)
            {
                switch (resourceType)
                {
                    case ResourceType.Wood: total += building.CalculateEffectiveProduction(building.data.woodProduction); break;
                    case ResourceType.Ore: total += building.CalculateEffectiveProduction(building.data.oreProduction); break;
                    case ResourceType.Gold: total += building.CalculateEffectiveProduction(building.data.goldProduction); break;
                    case ResourceType.Food: total += building.CalculateEffectiveProduction(building.data.foodProduction); break;
                }
            }
        }
        return total;
    }

    public int GetTotalConsumption(ResourceType resourceType)
    {
        int total = 0;
        foreach (Building building in allBuildings)
        {
            // ����������: ���������� isOperational ������ isActive
            if (building.isOperational)
            {
                switch (resourceType)
                {
                    case ResourceType.Wood: total += building.CalculateEffectiveConsumption(building.data.woodConsumption); break;
                    case ResourceType.Ore: total += building.CalculateEffectiveConsumption(building.data.oreConsumption); break;
                    case ResourceType.Food: total += building.CalculateEffectiveConsumption(building.data.foodConsumption); break;
                }
            }
        }
        return total;
    }

    public List<Building> GetAllBuildings()
    {
        return new List<Building>(allBuildings);
    }

    // ��������� ������������� ������ � Building
    public int CalculateEffectiveProduction(int baseProduction)
    {
        // ���������� ������ - ������ ���������� ������� ������������
        // � �������� ���������� ��� ������ ��������� ������� � �������������
        return baseProduction;
    }

    public int CalculateEffectiveConsumption(int baseConsumption)
    {
        // ���������� ������ - ������ ���������� ������� �����������
        return baseConsumption;
    }
}

public enum ResourceType
{
    Wood,
    Ore,
    Gold,
    Food
}