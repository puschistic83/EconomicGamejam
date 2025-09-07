using UnityEngine;

[CreateAssetMenu(fileName = "HouseData", menuName = "City Builder/House Data")]
public class HouseData : BuildingData
{
    [Header("House Specific")]
    public int residentsCapacity = 5;
    public float happinessBonus = 0.1f;
}