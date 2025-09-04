using UnityEngine;

public class FactoryBuilding : Building
{
    void Start()
    {
        buildCost = 70;
        goodsEffect = 10; // ���������� ������
        moneyEffect = -3; // ������� ����������
        happinessEffect = -0.7f; // �����������
    }

    public override void ApplyProductionEffects()
    {
        GameManager.Instance.AddGoods(goodsEffect);
        GameManager.Instance.AddMoney(moneyEffect);
        GameManager.Instance.AddHappiness(happinessEffect);
    }

    public override string GetInfo()
    {
        return "�������\n" + base.GetInfo() + $"\n(���������� ������, �� ������� ����������)";
    }
}