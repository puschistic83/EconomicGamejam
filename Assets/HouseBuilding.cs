using UnityEngine;

public class HouseBuilding : Building
{
    void Start()
    {
        buildCost = 40;
        goodsEffect = -2; // Потребляет товары
        happinessEffect = 0.8f; // Производит настроение
    }

    public override void ApplyProductionEffects()
    {
        GameManager gm = GameManager.Instance;

        // Проверяем, хватает ли товаров для потребления
        if (gm.Goods >= Mathf.Abs(goodsEffect))
        {
            // Хватает товаров - потребляем и добавляем настроение
            gm.AddGoods(goodsEffect);
            gm.AddHappiness(happinessEffect);
        }
        else
        {
            // Не хватает товаров - настроение падает
            gm.AddHappiness(-2f); // Штраф за нехватку товаров
            Debug.Log("Не хватает товаров для населения! Настроение падает.");
        }
    }

    public override string GetInfo()
    {
        return "ЖИЛОЙ ДОМ\n" + base.GetInfo() + $"\n(Повышает настроение, но требует товаров)";
    }
}