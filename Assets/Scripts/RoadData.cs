using UnityEngine;

[CreateAssetMenu(fileName = "New Road Data", menuName = "City Builder/Road Data")]
public class RoadData : ScriptableObject
{
    public string roadName = "Road";
    public int woodCost = 5;
    public int oreCost = 2;
    public Sprite roadSprite;
    public GameObject roadPrefab;
}