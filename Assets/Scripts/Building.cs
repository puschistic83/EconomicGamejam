using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Static Data")]
    public BuildingData data; // ������ �� ScriptableObject � �������

    [Header("Dynamic Data")]
    public int currentWorkers; // ������� ���������� �������
    public bool isActive = true; // �������� �� ������ (���� �� �������/�������)
    public float efficiency = 1f; // ��������� ������������� (�� 0 �� 1)

    // �������, ������� ����� �������� ��� ��������� ��������� ������
    // (��������, ��� ���������� UI)
    public System.Action OnBuildingUpdated;

    // ������������� ������ ������� (���������� ��� ����������)
    public void Initialize(BuildingData buildingData)
    {
        data = buildingData;
        // ���������� ������ �� ������
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null && data.buildingSprite != null)
        {
            renderer.sprite = data.buildingSprite;
        }
        // �������� ���������, ���� ��� ���
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }

    // ����� ��� ��������� ���������� �������
    public bool AssignWorkers(int workers)
    {
        if (workers <= data.maxWorkers)
        {
            currentWorkers = workers;
            OnBuildingUpdated?.Invoke();
            return true;
        }
        return false;
    }

    // ���������� ������� ������������/����������� � ������ ������� � �������������
    public int GetEffectiveProduction(int baseProduction)
    {
        if (!isActive) return 0;
        float workerFactor = (float)currentWorkers / data.maxWorkers;
        return Mathf.RoundToInt(baseProduction * workerFactor * efficiency);
    }

    // ���������� ��� ������� �� ������ (��� UI)
    void OnMouseDown()
    {
        Debug.Log($"Selected: {data.buildingName}. Workers: {currentWorkers}/{data.maxWorkers}");
        // ����� ����� ������� ������ ���������� � ������
    }
}