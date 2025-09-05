using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    private Grid targetGrid;
    public Vector2Int gridSize = new Vector2Int(20, 20);
    public GameObject gridCellVisualPrefab; // ������ ������� ��� ������

    void Start()
    {
        targetGrid = GetComponent<Grid>();
        DrawGrid();
    }

    void DrawGrid()
    {
        if (gridCellVisualPrefab == null)
        {
            Debug.LogWarning("Grid Cell Visual Prefab is not assigned!");
            return;
        }

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                // �������� ������� ���������� ������ ������
                Vector3 worldPosition = targetGrid.GetCellCenterWorld(new Vector3Int(x, y, 0));
                // ������� ���������� ������ ��� �������� ������
                GameObject visualCell = Instantiate(gridCellVisualPrefab, worldPosition, Quaternion.identity, this.transform);
                visualCell.name = $"GridCell_{x}_{y}";
            }
        }
    }
}