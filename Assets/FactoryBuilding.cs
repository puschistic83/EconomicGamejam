using UnityEngine;

public class FactoryBuilding : Building
{
    void Start()
    {
        buildCost = 70;
        goodsEffect = 10; // Производит товары
        moneyEffect = -3; // Требует содержания
        happinessEffect = -0.7f; // Загрязнение
    }

    public override void ApplyProductionEffects()
    {
        GameManager.Instance.AddGoods(goodsEffect);
        GameManager.Instance.AddMoney(moneyEffect);
        GameManager.Instance.AddHappiness(happinessEffect);
    }

    public override string GetInfo()
    {
        return "ФАБРИКА\n" + base.GetInfo() + $"\n(Производит товары, но снижает настроение)";
    }
}