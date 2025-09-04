using UnityEngine;

public class HouseBuilding : Building
{
    void Start()
    {
        buildCost = 40;
        goodsEffect = -3; // Потребляет товары
        happinessEffect = 1.5f; // Производит настроение
    }

    public override void ApplyProductionEffects()
    {
        GameManager.Instance.AddGoods(goodsEffect);
        GameManager.Instance.AddHappiness(happinessEffect);
    }

    public override string GetInfo()
    {
        return "ЖИЛОЙ ДОМ\n" + base.GetInfo() + $"\n(Повышает настроение, но требует товаров)";
    }
}