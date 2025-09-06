using UnityEngine;
using TMPro;

public class EconomyUI : MonoBehaviour
{
    public TextMeshProUGUI textProduction;
    public TextMeshProUGUI textConsumption;
    public TextMeshProUGUI textBalance;

    private BuildingManager buildingManager;
    private ResourceManager resourceManager;

    void Start()
    {
        buildingManager = BuildingManager.Instance;
        resourceManager = FindObjectOfType<ResourceManager>();

        if (resourceManager != null)
        {
            resourceManager.OnResourcesUpdated += UpdateEconomyInfo;
        }

        UpdateEconomyInfo();
    }

    void OnDestroy()
    {
        if (resourceManager != null)
        {
            resourceManager.OnResourcesUpdated -= UpdateEconomyInfo;
        }
    }

    public void UpdateEconomyInfo()
    {
        if (buildingManager == null || resourceManager == null) return;

        int woodProduction = buildingManager.GetTotalProduction(ResourceType.Wood);
        int oreProduction = buildingManager.GetTotalProduction(ResourceType.Ore);
        int foodProduction = buildingManager.GetTotalProduction(ResourceType.Food);

        int woodConsumption = buildingManager.GetTotalConsumption(ResourceType.Wood);
        int oreConsumption = buildingManager.GetTotalConsumption(ResourceType.Ore);
        int foodConsumption = buildingManager.GetTotalConsumption(ResourceType.Food);

        string productionText = $"Production:\n";
        productionText += $"Wood: {woodProduction}/s\n";
        productionText += $"Ore: {oreProduction}/s\n";
        productionText += $"Food: {foodProduction}/s";

        string consumptionText = $"Consumption:\n";
        consumptionText += $"Wood: {woodConsumption}/s\n";
        consumptionText += $"Ore: {oreConsumption}/s\n";
        consumptionText += $"Food: {foodConsumption}/s";

        string balanceText = $"Balance:\n";
        balanceText += $"Wood: {woodProduction - woodConsumption}/s\n";
        balanceText += $"Ore: {oreProduction - oreConsumption}/s\n";
        balanceText += $"Food: {foodProduction - foodConsumption}/s";

        if (textProduction != null) textProduction.text = productionText;
        if (textConsumption != null) textConsumption.text = consumptionText;
        if (textBalance != null) textBalance.text = balanceText;
    }
}