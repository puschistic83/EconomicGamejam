using UnityEngine;

public class HouseBuilding : Building
{
    void Start()
    {
        // ������������� ��������� ��� ����
        buildCost = 40;
        goodsEffect = -2; // ��� ���������� ������
        happinessEffect = 1.2f; // � ���������� ����������
    }

    public override string GetInfo()
    {
        return "����� ���\n" + base.GetInfo();
    }
}