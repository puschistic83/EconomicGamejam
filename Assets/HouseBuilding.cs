using UnityEngine;

public class HouseBuilding : Building
{
    void Start()
    {
        // Устанавливаем параметры для дома
        buildCost = 40;
        goodsEffect = -2; // Дом потребляет товары
        happinessEffect = 1.2f; // И производит настроение
    }

    public override string GetInfo()
    {
        return "ЖИЛОЙ ДОМ\n" + base.GetInfo();
    }
}