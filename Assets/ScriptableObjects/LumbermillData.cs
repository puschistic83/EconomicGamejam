using UnityEngine;

[CreateAssetMenu(fileName = "LumbermillData", menuName = "City Builder/Lumbermill Data")]
public class LumbermillData : BuildingData
{
    [Header("Lumbermill Specific")]
    public float woodProductionMultiplier = 1.5f;
    public int toolConsumption = 1;
}