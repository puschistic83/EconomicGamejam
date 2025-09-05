using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIResourceTooltip : MonoBehaviour
{
    public GameObject tooltipPanel;
    public TextMeshProUGUI costText;
    private ResourceManager resourceManager;

    void Start()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        tooltipPanel.SetActive(false);
    }

    public void ShowTooltip(BuildingData data)
    {
        string costString = $"Wood: {data.woodCost}\n";
        costString += $"Ore: {data.oreCost}\n";
        costString += $"Gold: {data.goldCost}\n";
        costString += $"Food: {data.foodCost}";

        costText.text = costString;
        tooltipPanel.SetActive(true);
    }

    public void ShowTooltip(RoadData data)
    {
        string costString = $"Wood: {data.woodCost}\n";
        costString += $"Ore: {data.oreCost}";

        costText.text = costString;
        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}