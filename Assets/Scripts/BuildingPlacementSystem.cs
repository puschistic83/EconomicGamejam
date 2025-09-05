using UnityEngine;

public class BuildingPlacementSystem : MonoBehaviour
{
    private Grid targetGrid;
    private GameObject currentHoverVisual;
    public GameObject placementHoverPrefab;

    public BuildingData selectedBuildingData;
    public RoadData selectedRoadData;

    private ResourceManager resourceManager;
    private bool isInConstructionMode = false;

    // ÷вета дл€ разных состо€ний
    public Color canBuildColor = Color.green;
    public Color cannotBuildColor = Color.red;
    private SpriteRenderer hoverRenderer;

    void Start()
    {
        targetGrid = GetComponent<Grid>();
        resourceManager = FindObjectOfType<ResourceManager>();

        if (placementHoverPrefab != null)
        {
            currentHoverVisual = Instantiate(placementHoverPrefab);
            hoverRenderer = currentHoverVisual.GetComponent<SpriteRenderer>();

            if (hoverRenderer == null)
            {
                Debug.LogError("Hover prefab doesn't have SpriteRenderer!");
                return;
            }

            currentHoverVisual.SetActive(false);
        }
    }

    void Update()
    {
        if (!isInConstructionMode || (selectedBuildingData == null && selectedRoadData == null))
        {
            if (currentHoverVisual != null) currentHoverVisual.SetActive(false);
            return;
        }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector3Int gridPosition = targetGrid.WorldToCell(mouseWorldPos);
        Vector3 cellCenterWorld = targetGrid.GetCellCenterWorld(gridPosition);

        // ќбновл€ем позицию и видимость
        if (currentHoverVisual != null && hoverRenderer != null)
        {
            currentHoverVisual.transform.position = cellCenterWorld;
            currentHoverVisual.SetActive(true);

            // ћен€ем цвет в зависимости от возможности строительства
            bool canBuild = CanBuildAtPosition(gridPosition);
            hoverRenderer.color = canBuild ? canBuildColor : cannotBuildColor;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceConstruction(gridPosition, cellCenterWorld);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            CancelConstruction();
        }
    }


    private bool CanBuildAtPosition(Vector3Int gridPosition)
    {
        // ѕровер€ем, что клетка свободна
        Collider2D overlap = Physics2D.OverlapBox(targetGrid.GetCellCenterWorld(gridPosition),
                                                 new Vector2(0.8f, 0.8f), 0);
        if (overlap != null) return false;

        // ѕровер€ем ресурсы в зависимости от типа строительства
        if (selectedBuildingData != null)
        {
            return resourceManager.CanAfford(selectedBuildingData.woodCost,
                                            selectedBuildingData.oreCost,
                                            selectedBuildingData.goldCost,
                                            selectedBuildingData.foodCost);
        }
        else if (selectedRoadData != null)
        {
            return resourceManager.CanAfford(selectedRoadData.woodCost,
                                            selectedRoadData.oreCost, 0, 0);
        }

        return false;
    }

    private void TryPlaceConstruction(Vector3Int gridPosition, Vector3 worldPosition)
    {
        if (!CanBuildAtPosition(gridPosition)) return;

        if (selectedBuildingData != null)
        {
            PlaceBuilding(selectedBuildingData, worldPosition);
        }
        else if (selectedRoadData != null)
        {
            PlaceRoad(selectedRoadData, worldPosition);
        }
    }

    private void PlaceBuilding(BuildingData data, Vector3 position)
    {
        if (resourceManager.SpendResources(data.woodCost, data.oreCost, data.goldCost, data.foodCost))
        {
            GameObject newBuilding = Instantiate(data.buildingPrefab, position, Quaternion.identity);
            Building building = newBuilding.GetComponent<Building>();
            building.Initialize(data);

            // јвтоматически выходим из режима строительства после постройки
            CancelConstruction();
        }
    }

    private void PlaceRoad(RoadData data, Vector3 position)
    {
        if (resourceManager.SpendResources(data.woodCost, data.oreCost, 0, 0))
        {
            GameObject newRoad = Instantiate(data.roadPrefab, position, Quaternion.identity);
            Road road = newRoad.GetComponent<Road>();
            road.Initialize(data);

            // Ќе выходим из режима строительства дорог дл€ быстрой прокладки
        }
    }

    public void SelectBuilding(BuildingData data)
    {
        selectedBuildingData = data;
        selectedRoadData = null;
        isInConstructionMode = true;
        Debug.Log($"Building selected: {data.buildingName}");
    }

    public void SelectRoad(RoadData data)
    {
        selectedRoadData = data;
        selectedBuildingData = null;
        isInConstructionMode = true;
        Debug.Log($"Road selected: {data.roadName}");
    }

    public void CancelConstruction()
    {
        selectedBuildingData = null;
        selectedRoadData = null;
        isInConstructionMode = false;
        if (currentHoverVisual != null) currentHoverVisual.SetActive(false);
        Debug.Log("Construction cancelled");
    }
}