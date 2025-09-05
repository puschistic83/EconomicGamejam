using UnityEngine;

public class HouseBuilding : Building
{
    void Start()
    {
        buildCost = 40;
        goodsEffect = -2; // ���������� ������
        happinessEffect = 0.8f; // ���������� ����������
    }

    public override void ApplyProductionEffects()
    {
        GameManager gm = GameManager.Instance;

        // ���������, ������� �� ������� ��� �����������
        if (gm.Goods >= Mathf.Abs(goodsEffect))
        {
            // ������� ������� - ���������� � ��������� ����������
            gm.AddGoods(goodsEffect);
            gm.AddHappiness(happinessEffect);
        }
        else
        {
            // �� ������� ������� - ���������� ������
            gm.AddHappiness(-2f); // ����� �� �������� �������
            Debug.Log("�� ������� ������� ��� ���������! ���������� ������.");
        }
    }

    public override string GetInfo()
    {
        return "����� ���\n" + base.GetInfo() + $"\n(�������� ����������, �� ������� �������)";
    }
}