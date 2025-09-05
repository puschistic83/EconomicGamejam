using UnityEngine;

// Создает меню в Assets/Create для быстрого создания новых данных здания
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
    public int goldProduction; // Золото обычно генерируется через налоги, но может и так
    public int foodProduction;

    [Header("Consumption (per second)")]
    public int woodConsumption;
    public int oreConsumption;
    public int foodConsumption;

    [Header("Other Properties")]
    public int maxWorkers; // Максимальное количество рабочих
    public float buildTime; // Время постройки в секундах
    public Sprite buildingSprite; // Спрайт для этого здания
    public GameObject buildingPrefab; // Префаб для инстанцирования

    // Можно добавить список зданий, которые должны быть построены для разблокировки этого (технология)
    // public BuildingData[] prerequisites;
}