using UnityEngine;

[CreateAssetMenu(fileName = "New Building Data", menuName = "City Builder/Building Data")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public string description;

    [Header("Cost")]
    public int woodCost;
    public int oreCost;
    public int goldCost;
    public int foodCost;

    [Header("Production (per second)")]
    public int woodProduction;
    public int oreProduction;
    public int goldProduction;
    public int foodProduction;

    [Header("Consumption (per second)")]
    public int woodConsumption;
    public int oreConsumption;
    public int foodConsumption;

    [Header("Worker Settings")]
    public int maxWorkers = 0;
    public int requiredWorkers = 0; // 0 = не требует рабочих

    [Header("Visuals")]
    public Sprite buildingSprite;
    public GameObject buildingPrefab;
    public float buildTime = 5f;
}