using UnityEngine;

public class HouseBuilding : Building
{
    void Start()
    {
        buildCost = 40;
        goodsEffect = -3; // ���������� ������
        happinessEffect = 1.5f; // ���������� ����������
    }

    public override void ApplyProductionEffects()
    {
        GameManager.Instance.AddGoods(goodsEffect);
        GameManager.Instance.AddHappiness(happinessEffect);
    }

    public override string GetInfo()
    {
        return "����� ���\n" + base.GetInfo() + $"\n(�������� ����������, �� ������� �������)";
    }
}