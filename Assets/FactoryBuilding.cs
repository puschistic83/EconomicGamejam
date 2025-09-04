using UnityEngine;

public class FactoryBuilding : Building
{
    void Start()
    {
        // Устанавливаем параметры для фабрики
        buildCost = 70;
        goodsEffect = 8; // Фабрика производит товары
        moneyEffect = -5; // Но требует содержания (деньги уходят)
        happinessEffect = -0.5f; // И немного снижает настроение (загрязнение)
    }

    public override string GetInfo()
    {
        return "ФАБРИКА\n" + base.GetInfo();
    }
}