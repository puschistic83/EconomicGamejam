using UnityEngine;

public class BuildingPlacementSystem : MonoBehaviour
{
    private Grid targetGrid;
    private GameObject currentHoverVisual;
    public GameObject placementHoverPrefab; // ������ ��� ��������� ����������
    public GameObject buildingToPlace; // ������ ������, ������� ����� ������� (���� ���)

    void Start()
    {
        targetGrid = GetComponent<Grid>();
        // ������� ������ ��� ��������� ����� ����������
        if (placementHoverPrefab != null)
        {
            currentHoverVisual = Instantiate(placementHoverPrefab);
            currentHoverVisual.SetActive(false);
        }
    }

    void Update()
    {
        // �������� ������� ���� � ����
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // ����� ��� 2D!

        // ������������ ������� ���������� � ���������� �����
        Vector3Int gridPosition = targetGrid.WorldToCell(mouseWorldPos);
        Vector3 cellCenterWorld = targetGrid.GetCellCenterWorld(gridPosition);

        // ��������� ������� � ��������� ������� ���������
        if (currentHoverVisual != null)
        {
            currentHoverVisual.transform.position = cellCenterWorld;
            currentHoverVisual.SetActive(true);
            // ����� ����� ������ ���� ��������� � ����������� �� ����������� ���������
        }

        // ���� ������ ���, ��������� ������
        if (Input.GetMouseButtonDown(0))
        {
            // ������� ��������: ���� ��� �� �� ��� �� ������ (�� � UI � �� � ������ ������)
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