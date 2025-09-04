using UnityEngine;
using UnityEngine.UI;

public class BuildingPlacer : MonoBehaviour
{
    [Header("���������")]
    public LayerMask buildableLayer;
    public LayerMask obstacleLayer;
    public Color validPlacementColor = new Color(0, 1, 0, 0.5f);
    public Color invalidPlacementColor = new Color(1, 0, 0, 0.5f);
    public Color noMoneyColor = new Color(1, 0.5f, 0, 0.5f); // ��������� ��� "��� �����"

    [Header("������� ������")]
    public GameObject factoryPrefab;
    public GameObject housePrefab;

    [Header("������")]
    public GameManager gameManager;
    public GameObject placementGhost;
    private SpriteRenderer ghostRenderer;

    private GameObject currentBuildingPrefab;
    private bool isPlacing = false;
    private bool hasEnoughMoney = false;

    void Start()
    {
        if (placementGhost == null)
        {
            CreateGhostObject();
        }
        else
        {
            ghostRenderer = placementGhost.GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (isPlacing)
        {
            UpdateGhostPosition();
            CheckPlacementValidity();

            if (Input.GetMouseButtonDown(0) && hasEnoughMoney)
            {
                TryPlaceBuilding();
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPlacement();
            }
        }
    }

    private void CreateGhostObject()
    {
        placementGhost = new GameObject("PlacementGhost");
        ghostRenderer = placementGhost.AddComponent<SpriteRenderer>();
        ghostRenderer.sortingOrder = 10;
        ghostRenderer.color = validPlacementColor;
        placementGhost.SetActive(false);
    }

    // ������ ������� ���������� ������ � ��������� �����
    public void StartBuildingPlacement(GameObject buildingPrefab)
    {
        if (buildingPrefab == null)
        {
            Debug.LogError("Prefab is null!");
            return;
        }

        Building buildingScript = buildingPrefab.GetComponent<Building>();
        if (buildingScript == null)
        {
            Debug.LogError("� ������� ��� ���������� Building!");
            return;
        }

        // �������� ����� ��� ������ ����������
        if (gameManager.Money < buildingScript.buildCost)
        {
            Debug.Log($"������������ �����! �����: {buildingScript.buildCost}, ����: {gameManager.Money}");
            // ����� �������� ���� ��� UI ���������
            return;
        }

        if (isPlacing)
        {
            CancelPlacement();
        }

        currentBuildingPrefab = buildingPrefab;
        isPlacing = true;
        hasEnoughMoney = true;

        SetupGhost(buildingPrefab);
        placementGhost.SetActive(true);

        Debug.Log("����� ���������� �����������");
    }

    private void SetupGhost(GameObject buildingPrefab)
    {
        if (ghostRenderer == null)
        {
            ghostRenderer = placementGhost.GetComponent<SpriteRenderer>();
            if (ghostRenderer == null)
                ghostRenderer = placementGhost.AddComponent<SpriteRenderer>();
        }

        SpriteRenderer buildingSprite = buildingPrefab.GetComponent<SpriteRenderer>();
        if (buildingSprite != null && buildingSprite.sprite != null)
        {
            ghostRenderer.sprite = buildingSprite.sprite;
        }

        BoxCollider2D buildingCollider = buildingPrefab.GetComponent<BoxCollider2D>();
        if (buildingCollider != null)
        {
            BoxCollider2D ghostCollider = placementGhost.GetComponent<BoxCollider2D>();
            if (ghostCollider == null)
                ghostCollider = placementGhost.AddComponent<BoxCollider2D>();

            ghostCollider.size = buildingCollider.size;
            ghostCollider.offset = buildingCollider.offset;
            ghostCollider.isTrigger = true;
        }
    }

    private void UpdateGhostPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        placementGhost.transform.position = mousePos;
    }

    private void CheckPlacementValidity()
    {
        bool isValidPosition = IsPlacementValid();
        bool stillHasMoney = CheckMoney(); // ��������� ������ ������ ����

        // ������ ���� � ����������� �� �������
        if (!stillHasMoney)
        {
            ghostRenderer.color = noMoneyColor;
        }
        else if (!isValidPosition)
        {
            ghostRenderer.color = invalidPlacementColor;
        }
        else
        {
            ghostRenderer.color = validPlacementColor;
        }

        hasEnoughMoney = stillHasMoney && isValidPosition;
    }

    private bool CheckMoney()
    {
        if (currentBuildingPrefab == null) return false;

        Building buildingScript = currentBuildingPrefab.GetComponent<Building>();
        if (buildingScript == null) return false;

        return gameManager.Money >= buildingScript.buildCost;
    }

    private bool IsPlacementValid()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            placementGhost.transform.position,
            GetGhostSize(),
            0,
            obstacleLayer
        );

        return colliders.Length == 0;
    }

    private Vector2 GetGhostSize()
    {
        if (ghostRenderer != null)
        {
            return ghostRenderer.bounds.size * 0.8f;
        }
        return Vector2.one;
    }

    private void TryPlaceBuilding()
    {
        if (!hasEnoughMoney)
        {
            Debug.Log("������������ ����� ��� ���������!");
            return;
        }

        if (!IsPlacementValid())
        {
            Debug.Log("������ ��������� �����!");
            return;
        }

        Building buildingScript = currentBuildingPrefab.GetComponent<Building>();
        if (buildingScript == null) return;

        // ��������� �������� ������������ (�� ������ ���� ������ ��������� � ������ �����)
        if (gameManager.Money < buildingScript.buildCost)
        {
            Debug.Log("������ ����������� �� ����� ����������!");
            hasEnoughMoney = false;
            return;
        }

        Vector3 buildPosition = placementGhost.transform.position;
        GameObject newBuilding = Instantiate(currentBuildingPrefab, buildPosition, Quaternion.identity);

        Building newBuildingScript = newBuilding.GetComponent<Building>();
        if (newBuildingScript != null)
        {
            // �������� ������ ����� ��������
            gameManager.AddMoney(-buildingScript.buildCost);
            gameManager.AddBuildingToManager(newBuildingScript);
            gameManager.UpdateUI();

            Debug.Log($"���������: {newBuilding.name} �� {buildingScript.buildCost}");
        }

        // ����� ��������� ��������� ����� �� ��������� ��� ����� �� ������
        CheckMoney();
    }

    private void CancelPlacement()
    {
        isPlacing = false;
        currentBuildingPrefab = null;
        hasEnoughMoney = false;
        if (placementGhost != null)
            placementGhost.SetActive(false);

        Debug.Log("����� ���������� �������");
    }

    // ������ ��� ������ UI
    public void StartFactoryPlacement()
    {
        StartBuildingPlacement(factoryPrefab);
    }

    public void StartHousePlacement()
    {
        StartBuildingPlacement(housePrefab);
    }
}