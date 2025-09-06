using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour
{
    [Header("Static Data")]
    public BuildingData data;

    [Header("Dynamic Data")]
    public int currentWorkers = 0;
    public bool isActive = true;
    public float efficiency = 1f;

    private ResourceManager resourceManager;
    private RoadManager roadManager;
    private const float productionInterval = 1f;
    private bool canDeliverResources = false;
    private bool isConnectedToMain = false;

    public System.Action OnBuildingUpdated;

    void Start()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        roadManager = FindObjectOfType<RoadManager>();

        if (resourceManager == null) Debug.LogError("ResourceManager not found!");
        if (roadManager == null) Debug.LogError("RoadManager not found!");

        StartCoroutine(ProductionCycle());
        CheckConnectionStatus();

        if (resourceManager != null)
        {
            resourceManager.OnMainBuildingChanged += CheckConnectionStatus;
        }
        if (roadManager != null)
        {
            roadManager.OnRoadsUpdated += CheckConnectionStatus;
        }
    }

    // ДОБАВЛЯЕМ ОТСУТСТВУЮЩИЙ МЕТОД Initialize
    public void Initialize(BuildingData buildingData)
    {
        data = buildingData;

        // Устанавливаем спрайт
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null && data.buildingSprite != null)
        {
            renderer.sprite = data.buildingSprite;
        }

        // Добавляем коллайдер если его нет
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }

        // Автоматически нанимаем рабочих
        currentWorkers = data.maxWorkers;

        Debug.Log($"Building initialized: {data.buildingName} with {currentWorkers} workers");
    }

    void OnDestroy()
    {
        if (resourceManager != null)
        {
            resourceManager.OnMainBuildingChanged -= CheckConnectionStatus;
        }
        if (roadManager != null)
        {
            roadManager.OnRoadsUpdated -= CheckConnectionStatus;
        }
    }

    private void CheckConnectionStatus()
    {
        if (resourceManager == null || roadManager == null) return;

        // Для главного здания всегда разрешена доставка
        if (IsMainBuilding())
        {
            canDeliverResources = true;
            isConnectedToMain = true;
        }
        else
        {
            // Проверяем соединение дорогами с главным зданием
            isConnectedToMain = roadManager.IsConnectedToMainBuilding(transform.position);

            // Может доставлять если соединен и в радиусе
            canDeliverResources = isConnectedToMain &&
                                resourceManager.HasMainBuilding &&
                                Vector3.Distance(transform.position, resourceManager.mainBuilding.transform.position) <= resourceManager.deliveryRange;
        }

        isActive = canDeliverResources;

        Debug.Log($"{data.buildingName} - Connected: {isConnectedToMain}, Can deliver: {canDeliverResources}");
    }

    private bool IsMainBuilding()
    {
        return resourceManager != null &&
               resourceManager.HasMainBuilding &&
               resourceManager.mainBuilding.gameObject == gameObject;
    }

    IEnumerator ProductionCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(productionInterval);

            if (isActive && resourceManager != null && currentWorkers > 0 && canDeliverResources)
            {
                ProduceResources();
                ConsumeResources();
            }
        }
    }

    private void ProduceResources()
    {
        int effectiveWood = CalculateEffectiveProduction(data.woodProduction);
        int effectiveOre = CalculateEffectiveProduction(data.oreProduction);
        int effectiveGold = CalculateEffectiveProduction(data.goldProduction);
        int effectiveFood = CalculateEffectiveProduction(data.foodProduction);

        if (effectiveWood > 0) resourceManager.AddResources(effectiveWood, 0, 0, 0);
        if (effectiveOre > 0) resourceManager.AddResources(0, effectiveOre, 0, 0);
        if (effectiveGold > 0) resourceManager.AddResources(0, 0, effectiveGold, 0);
        if (effectiveFood > 0) resourceManager.AddResources(0, 0, 0, effectiveFood);

        if (effectiveWood > 0 || effectiveOre > 0 || effectiveGold > 0 || effectiveFood > 0)
        {
            Debug.Log($"{data.buildingName} produced: W{effectiveWood}/O{effectiveOre}/G{effectiveGold}/F{effectiveFood}");
        }
    }

    private void ConsumeResources()
    {
        if (data.woodConsumption > 0 || data.oreConsumption > 0 || data.foodConsumption > 0)
        {
            int effectiveWoodConsumption = CalculateEffectiveConsumption(data.woodConsumption);
            int effectiveOreConsumption = CalculateEffectiveConsumption(data.oreConsumption);
            int effectiveFoodConsumption = CalculateEffectiveConsumption(data.foodConsumption);

            bool canConsume = resourceManager.CanAfford(effectiveWoodConsumption,
                                                       effectiveOreConsumption,
                                                       0,
                                                       effectiveFoodConsumption);

            if (canConsume)
            {
                resourceManager.SpendResources(effectiveWoodConsumption,
                                              effectiveOreConsumption,
                                              0,
                                              effectiveFoodConsumption);
            }
            else
            {
                efficiency = Mathf.Max(0.1f, efficiency - 0.1f);
                Debug.Log($"{data.buildingName} efficiency decreased to {efficiency:P0} due to lack of resources");
            }
        }
    }

    public int CalculateEffectiveProduction(int baseProduction)
    {
        if (baseProduction == 0) return 0;

        float workerFactor = (float)currentWorkers / data.maxWorkers;
        return Mathf.RoundToInt(baseProduction * workerFactor * efficiency);
    }

    public int CalculateEffectiveConsumption(int baseConsumption)
    {
        if (baseConsumption == 0) return 0;

        float workerFactor = (float)currentWorkers / data.maxWorkers;
        return Mathf.RoundToInt(baseConsumption * workerFactor);
    }

    void OnMouseDown()
    {
        string statusMessage = $"Selected: {data.buildingName}\n" +
                              $"Workers: {currentWorkers}/{data.maxWorkers}\n" +
                              $"Efficiency: {efficiency:P0}\n" +
                              $"Connected to main: {isConnectedToMain}\n" +
                              $"Can deliver: {canDeliverResources}";

        if (resourceManager != null && resourceManager.HasMainBuilding)
        {
            float distance = Vector3.Distance(transform.position, resourceManager.mainBuilding.transform.position);
            statusMessage += $"\nDistance to main: {distance:F1}m (Range: {resourceManager.deliveryRange}m)";
        }

        Debug.Log(statusMessage);
    }

    public int GetCurrentProduction(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Wood: return CalculateEffectiveProduction(data.woodProduction);
            case ResourceType.Ore: return CalculateEffectiveProduction(data.oreProduction);
            case ResourceType.Gold: return CalculateEffectiveProduction(data.goldProduction);
            case ResourceType.Food: return CalculateEffectiveProduction(data.foodProduction);
            default: return 0;
        }
    }

    public int GetCurrentConsumption(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Wood: return CalculateEffectiveConsumption(data.woodConsumption);
            case ResourceType.Ore: return CalculateEffectiveConsumption(data.oreConsumption);
            case ResourceType.Food: return CalculateEffectiveConsumption(data.foodConsumption);
            default: return 0;
        }
    }

    public bool IsConnectedToMain()
    {
        return isConnectedToMain;
    }

    public bool IsInDeliveryRange()
    {
        return canDeliverResources;
    }
}