using UnityEngine;

public class BuildingPlacementSystem : MonoBehaviour
{
    private Grid targetGrid;
    private GameObject currentHoverVisual;
    public GameObject placementHoverPrefab; // Префаб для подсветки размещения
    public GameObject buildingToPlace; // Префаб здания, которое будем ставить (пока куб)

    void Start()
    {
        targetGrid = GetComponent<Grid>();
        // Создаем визуал для подсветки места размещения
        if (placementHoverPrefab != null)
        {
            currentHoverVisual = Instantiate(placementHoverPrefab);
            currentHoverVisual.SetActive(false);
        }
    }

    void Update()
    {
        // Получаем позицию мыши в мире
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // Важно для 2D!

        // Конвертируем мировые координаты в координаты сетки
        Vector3Int gridPosition = targetGrid.WorldToCell(mouseWorldPos);
        Vector3 cellCenterWorld = targetGrid.GetCellCenterWorld(gridPosition);

        // Обновляем позицию и видимость визуала подсветки
        if (currentHoverVisual != null)
        {
            currentHoverVisual.transform.position = cellCenterWorld;
            currentHoverVisual.SetActive(true);
            // Здесь можно менять цвет подсветки в зависимости от возможности постройки
        }

        // Если нажата ЛКМ, размещаем здание
        if (Input.GetMouseButtonDown(0))
        {
            // Простая проверка: если луч ни во что не уперся (не в UI и не в другое здание)
            if (hit.collider == null)
            {
                PlaceBuilding(cellCenterWorld);
            }
            else
            {
                Debug.Log("Can't build here! " + hit.collider.name);
            }
        }
    }

    void PlaceBuilding(Vector3 position)
    {
        if (buildingToPlace != null)
        {
            Instantiate(buildingToPlace, position, Quaternion.identity);
            Debug.Log("Building placed at: " + position);
        }
    }
}