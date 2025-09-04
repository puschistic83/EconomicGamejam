using UnityEngine;

public class FactoryBuilding : Building
{
    void Start()
    {
        // ������������� ��������� ��� �������
        buildCost = 70;
        goodsEffect = 8; // ������� ���������� ������
        moneyEffect = -5; // �� ������� ���������� (������ ������)
        happinessEffect = -0.5f; // � ������� ������� ���������� (�����������)
    }

    public override string GetInfo()
    {
        return "�������\n" + base.GetInfo();
    }
}