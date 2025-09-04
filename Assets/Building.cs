using UnityEngine;

// Базовый класс для всех зданий в игре
public class Building : MonoBehaviour
{
    [Header("Базовая стоимость")]
    public int buildCost = 50; // Стоимость постройки

    [Header("Эффекты производства (в ход)")]
    public int moneyEffect = 0;
    public int goodsEffect = 0;
    public float happinessEffect = 0f;

    // Виртуальный метод, который будет вызываться каждый ход
    public virtual void ApplyProductionEffects()
    {
        // Базовая реализация, которая будет переопределяться в дочерних классах
        GameManager.Instance.Money += moneyEffect;
        GameManager.Instance.Goods += goodsEffect;
        GameManager.Instance.Happiness += happinessEffect;
    }

    // Виртуальный метод для получения информации о здании (для UI)
    public virtual string GetInfo()
    {
        return $"Стоимость: {buildCost}\n" +
               $"Деньги/ход: {moneyEffect}\n" +
               $"Товары/ход: {goodsEffect}\n" +
               $"Настроение/ход: {happinessEffect}";
    }
}