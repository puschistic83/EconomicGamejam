using UnityEngine;

// ������� ���� � Assets/Create ��� �������� �������� ����� ������ ������
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
    public int goldProduction; // ������ ������ ������������ ����� ������, �� ����� � ���
    public int foodProduction;

    [Header("Consumption (per second)")]
    public int woodConsumption;
    public int oreConsumption;
    public int foodConsumption;

    [Header("Other Properties")]
    public int maxWorkers; // ������������ ���������� �������
    public float buildTime; // ����� ��������� � ��������
    public Sprite buildingSprite; // ������ ��� ����� ������
    public GameObject buildingPrefab; // ������ ��� ���������������

    // ����� �������� ������ ������, ������� ������ ���� ��������� ��� ������������� ����� (����������)
    // public BuildingData[] prerequisites;
}