using UnityEngine;
using UnityEngine.UI;

public class BuildingPlacer : MonoBehaviour
{
    [Header("Настройки")]
    public LayerMask buildableLayer;
    public LayerMask obstacleLayer;
    public Color validPlacementColor = new Color(0, 1, 0, 0.5f);
    public Color invalidPlacementColor = new Color(1, 0, 0, 0.5f);
    public Color noMoneyColor = new Color(1, 0.5f, 0, 0.5f); // Оранжевый для "нет денег"

    [Header("Префабы зданий")]
    public GameObject factoryPrefab;
    public GameObject housePrefab;

    [Header("Ссылки")]
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

    // Начать процесс размещения здания с проверкой денег
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
            Debug.LogError("У префаба нет компонента Building!");
            return;
        }

        // ПРОВЕРКА ДЕНЕГ ПРИ НАЧАЛЕ РАЗМЕЩЕНИЯ
        if (gameManager.Money < buildingScript.buildCost)
        {
            Debug.Log($"Недостаточно денег! Нужно: {buildingScript.buildCost}, есть: {gameManager.Money}");
            // Можно добавить звук или UI сообщение
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

        Debug.Log("Режим размещения активирован");
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
        bool stillHasMoney = CheckMoney(); // Проверяем деньги каждый кадр

        // Меняем цвет в зависимости от условий
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
            Debug.Log("Недостаточно денег для постройки!");
            return;
        }

        if (!IsPlacementValid())
        {
            Debug.Log("Нельзя построить здесь!");
            return;
        }

        Building buildingScript = currentBuildingPrefab.GetComponent<Building>();
        if (buildingScript == null) return;

        // ФИНАЛЬНАЯ ПРОВЕРКА ПЕРЕСТРОЙКОЙ (на случай если деньги потратили в другом месте)
        if (gameManager.Money < buildingScript.buildCost)
        {
            Debug.Log("Деньги закончились во время размещения!");
            hasEnoughMoney = false;
            return;
        }

        Vector3 buildPosition = placementGhost.transform.position;
        GameObject newBuilding = Instantiate(currentBuildingPrefab, buildPosition, Quaternion.identity);

        Building newBuildingScript = newBuilding.GetComponent<Building>();
        if (newBuildingScript != null)
        {
            // ВЫЧИТАЕМ ДЕНЬГИ ЧЕРЕЗ МЕНЕДЖЕР
            gameManager.AddMoney(-buildingScript.buildCost);
            gameManager.AddBuildingToManager(newBuildingScript);
            gameManager.UpdateUI();

            Debug.Log($"Построено: {newBuilding.name} за {buildingScript.buildCost}");
        }

        // После постройки проверяем можем ли построить еще такое же здание
        CheckMoney();
    }

    private void CancelPlacement()
    {
        isPlacing = false;
        currentBuildingPrefab = null;
        hasEnoughMoney = false;
        if (placementGhost != null)
            placementGhost.SetActive(false);

        Debug.Log("Режим размещения отменен");
    }

    // Методы для кнопок UI
    public void StartFactoryPlacement()
    {
        StartBuildingPlacement(factoryPrefab);
    }

    public void StartHousePlacement()
    {
        StartBuildingPlacement(housePrefab);
    }
}