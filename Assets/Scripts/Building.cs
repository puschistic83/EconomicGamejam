using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Static Data")]
    public BuildingData data; // Ссылка на ScriptableObject с данными

    [Header("Dynamic Data")]
    public int currentWorkers; // Текущее количество рабочих
    public bool isActive = true; // Работает ли здание (есть ли ресурсы/рабочие)
    public float efficiency = 1f; // Множитель эффективности (от 0 до 1)

    // Событие, которое можно вызывать при изменении состояния здания
    // (например, для обновления UI)
    public System.Action OnBuildingUpdated;

    // Инициализация здания данными (вызывается при размещении)
    public void Initialize(BuildingData buildingData)
    {
        data = buildingData;
        // Установите спрайт из данных
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null && data.buildingSprite != null)
        {
            renderer.sprite = data.buildingSprite;
        }
        // Добавьте коллайдер, если его нет
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }

    // Метод для изменения количества рабочих
    public bool AssignWorkers(int workers)
    {
        if (workers <= data.maxWorkers)
        {
            currentWorkers = workers;
            OnBuildingUpdated?.Invoke();
            return true;
        }
        return false;
    }

    // Рассчитать текущее производство/потребление с учетом рабочих и эффективности
    public int GetEffectiveProduction(int baseProduction)
    {
        if (!isActive) return 0;
        float workerFactor = (float)currentWorkers / data.maxWorkers;
        return Mathf.RoundToInt(baseProduction * workerFactor * efficiency);
    }

    // Вызывается при нажатии на здание (для UI)
    void OnMouseDown()
    {
        Debug.Log($"Selected: {data.buildingName}. Workers: {currentWorkers}/{data.maxWorkers}");
        // Здесь позже вызовем панель информации о здании
    }
}