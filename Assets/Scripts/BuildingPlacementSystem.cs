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
        // ��������� ������� �� ����� �������������
        if (!isInConstructionMode || (selectedBuildingData == null && selectedRoadData == null))
        {
            if (currentHoverVisual != null) currentHoverVisual.SetActive(false);
            return;
        }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector3Int gridPosition = targetGrid.WorldToCell(mouseWorldPos);
        Vector3 cellCenterWorld = targetGrid.GetCellCenterWorld(gridPosition);

        // ��������� ������ ���������
        if (currentHoverVisual != null && hoverRenderer != null)
        {
            currentHoverVisual.transform.position = cellCenterWorld;
            currentHoverVisual.SetActive(true);

            bool canBuild = CanBuildAtPosition(gridPosition);
            hoverRenderer.color = canBuild ? canBuildColor : cannotBuildColor;
        }

        // ��� - �������������
        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceConstruction(gridPosition, cellCenterWorld);
        }
        // ��� - ������ ������
        else if (Input.GetMouseButtonDown(1))
        {
            CancelConstruction();
        }

        // ������� ������� ��� ����� - R
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (selectedRoadData != null)
            {
                SelectRoad(selectedRoadData);
            }
            else
            {
                Debug.Log("No road data selected! Assign road data in inspector.");
            }
        }
    }

    private bool CanBuildAtPosition(Vector3Int gridPosition)
    {
        Vector3 worldPos = targetGrid.GetCellCenterWorld(gridPosition);

        // ��������� ���������� - ��� �� ������ ��������
        Collider2D overlap = Physics2D.OverlapBox(worldPos, new Vector2(0.8f, 0.8f), 0);
        if (overlap != null)
        {
            Debug.Log("Cannot build here: object in the way!");
            return false;
        }

        // ��������� ������� ��� ������
        if (selectedBuildingData != null)
        {
            if (!resourceManager.CanAfford(selectedBuildingData.woodCost,
                                          selectedBuildingData.oreCost,
                                          selectedBuildingData.goldCost,
                                          selectedBuildingData.foodCost))
            {
                Debug.Log("Cannot build: not enough resources for building!");
                return false;
            }
        }
        // ��������� ������� ��� �����
        else if (selectedRoadData != null)
        {
            if (!resourceManager.CanAfford(selectedRoadData.woodCost,
                                         selectedRoadData.oreCost, 0, 0))
            {
                Debug.Log("Cannot build: not enough resources for road!");
                return false;
            }
        }

        // ������� ������ ����� ������� ��� ������
        if (selectedBuildingData != null && IsMainBuilding(selectedBuildingData))
        {
            return true;
        }

        // ��� ����� ��������� ����������� �������������
        if (selectedRoadData != null)
        {
            // ������ ����� ������� �����, ���� ���� ������� ������
            if (!resourceManager.HasMainBuilding)
            {
                Debug.Log("Cannot build roads: no main building!");
                return false;
            }
            return true;
        }

        // ��� ������� ������ ��������� ������
        if (!resourceManager.CanBuildHere(worldPos))
        {
            return false;
        }

        return true;
    }

    private bool IsMainBuilding(BuildingData data)
    {
        return data.buildingName.Contains("Town Hall") ||
               data.buildingName.Contains("Main") ||
               data.buildingName.Contains("������");
    }

    private void TryPlaceConstruction(Vector3Int gridPosition, Vector3 worldPosition)
    {
        if (CanBuildAtPosition(gridPosition))
        {
            if (selectedBuildingData != null)
            {
                PlaceBuilding(selectedBuildingData, worldPosition);
            }
            else if (selectedRoadData != null)
            {
                PlaceRoad(selectedRoadData, worldPosition);
            }
        }
        else
        {
            Debug.Log("Cannot build here!");
        }
    }

    private void PlaceBuilding(BuildingData data, Vector3 position)
    {
        if (resourceManager.SpendResources(data.woodCost, data.oreCost, data.goldCost, data.foodCost))
        {
            GameObject newBuilding = Instantiate(data.buildingPrefab, position, Quaternion.identity);
            Building buildingComponent = newBuilding.GetComponent<Building>();

            if (buildingComponent != null)
            {
                buildingComponent.Initialize(data);

                if (IsMainBuilding(data))
                {
                    resourceManager.SetMainBuilding(buildingComponent);
                    Debug.Log("Main building placed! Now build roads and other structures.");
                }
            }

            CancelConstruction();
        }
    }

    private void PlaceRoad(RoadData data, Vector3 position)
    {
        if (resourceManager.SpendResources(data.woodCost, data.oreCost, 0, 0))
        {
            GameObject newRoad = Instantiate(data.roadPrefab, position, Quaternion.identity);
            Road roadComponent = newRoad.GetComponent<Road>();
            if (roadComponent != null)
            {
                roadComponent.SetRoadData(data);
                Debug.Log($"Road built at {position}! Cost: {data.woodCost} wood, {data.oreCost} ore");
            }
            else
            {
                Debug.LogError("Road prefab doesn't have Road component!");
            }

            // �� ������� �� ������ ������������� ����� ��� ������� ���������
            // CancelConstruction(); // ���������������� ��� �����
        }
        else
        {
            Debug.Log("Not enough resources to build road!");
        }
    }

    public void SelectBuilding(BuildingData data)
    {
        selectedBuildingData = data;
        selectedRoadData = null;
        isInConstructionMode = true;

        Debug.Log($"Selected building: {data.buildingName}");

        if (!IsMainBuilding(data) && !resourceManager.HasMainBuilding)
        {
            Debug.Log("Hint: Build Town Hall first! Then connect buildings with roads.");
        }
    }

    public void SelectRoad(RoadData data)
    {
        selectedRoadData = data;
        selectedBuildingData = null;
        isInConstructionMode = true;

        Debug.Log($"Selected road: {data.roadName}. Cost: {data.woodCost} wood, {data.oreCost} ore");

        if (!resourceManager.HasMainBuilding)
        {
            Debug.Log("Hint: Build Town Hall first before placing roads!");
        }
    }

    public void CancelConstruction()
    {
        selectedBuildingData = null;
        selectedRoadData = null;
        isInConstructionMode = false;

        if (currentHoverVisual != null)
        {
            currentHoverVisual.SetActive(false);
        }

        Debug.Log("Construction mode cancelled");
    }
}