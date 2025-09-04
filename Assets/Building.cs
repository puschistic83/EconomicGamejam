using UnityEngine;

// ������� ����� ��� ���� ������ � ����
public class Building : MonoBehaviour
{
    [Header("������� ���������")]
    public int buildCost = 50; // ��������� ���������

    [Header("������� ������������ (� ���)")]
    public int moneyEffect = 0;
    public int goodsEffect = 0;
    public float happinessEffect = 0f;

    // ����������� �����, ������� ����� ���������� ������ ���
    public virtual void ApplyProductionEffects()
    {
        // ������� ����������, ������� ����� ���������������� � �������� �������
        GameManager.Instance.Money += moneyEffect;
        GameManager.Instance.Goods += goodsEffect;
        GameManager.Instance.Happiness += happinessEffect;
    }

    // ����������� ����� ��� ��������� ���������� � ������ (��� UI)
    public virtual string GetInfo()
    {
        return $"���������: {buildCost}\n" +
               $"������/���: {moneyEffect}\n" +
               $"������/���: {goodsEffect}\n" +
               $"����������/���: {happinessEffect}";
    }
}