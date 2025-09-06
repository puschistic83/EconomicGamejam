using UnityEngine;

public class Road : MonoBehaviour
{
    public RoadData data;
    public float durability = 100f;
    public float maxDurability = 100f;
    public float decayRate = 0.1f;

    private RoadManager roadManager;

    void Start()
    {
        roadManager = FindObjectOfType<RoadManager>();

        // ������������ ������ ��� ��������
        if (roadManager != null)
        {
            roadManager.RegisterRoad(transform.position);
        }

        // �������������� ������ ����� ��������
        InitializeRoad();
    }

    void OnDestroy()
    {
        // ������� ������ ��� �����������
        if (roadManager != null)
        {
            roadManager.UnregisterRoad(transform.position);
        }
    }

    void Update()
    {
        // ������ �������� ������������
        durability -= decayRate * Time.deltaTime;
        durability = Mathf.Clamp(durability, 0, maxDurability);
    }

    public void Repair(float amount)
    {
        durability += amount;
        durability = Mathf.Clamp(durability, 0, maxDurability);
    }

    // ��������������� ����� ����� �������� ���������
    public void InitializeRoad()
    {
        if (data != null)
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (renderer != null && data.roadSprite != null)
            {
                renderer.sprite = data.roadSprite;
            }

            // ��������� ��������� ���� ��� ���
            if (GetComponent<Collider2D>() == null)
            {
                gameObject.AddComponent<BoxCollider2D>();
            }
        }
    }

    // �������������� ����� ��� �������� ������
    public void SetRoadData(RoadData roadData)
    {
        data = roadData;
        InitializeRoad();
    }
}