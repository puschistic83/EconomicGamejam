using UnityEngine;

public class Road : MonoBehaviour
{
    public RoadData data;
    public float durability = 100f;
    public float maxDurability = 100f;
    public float decayRate = 0.1f; // �������� ������ � �������

    void Update()
    {
        // ������ �������� ������������ �� ��������
        durability -= decayRate * Time.deltaTime;
        durability = Mathf.Clamp(durability, 0, maxDurability);

        // ����� ����� ������� ���������� ����������� ������
        // (��������, ������ ���� �������)
    }

    public void Repair(float amount)
    {
        durability += amount;
        durability = Mathf.Clamp(durability, 0, maxDurability);
    }

    public void Initialize(RoadData roadData)
    {
        data = roadData;
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null && data.roadSprite != null)
        {
            renderer.sprite = data.roadSprite;
        }
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }
}