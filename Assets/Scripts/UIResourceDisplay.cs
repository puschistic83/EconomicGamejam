using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIResourceDisplay : MonoBehaviour
{
    [Header("Text References")]
    public TextMeshProUGUI textWood;
    public TextMeshProUGUI textOre;
    public TextMeshProUGUI textGold;
    public TextMeshProUGUI textFood;

    private ResourceManager resourceManager;

    void Start()
    {
        resourceManager = FindObjectOfType<ResourceManager>();

        if (resourceManager == null)
        {
            Debug.LogError("ResourceManager not found!");
            return;
        }

        // Подписываемся на событие обновления ресурсов
        resourceManager.OnResourcesUpdated += UpdateResourceDisplay;

        // Обновляем отображение сразу при старте
        UpdateResourceDisplay();
    }

    void OnDestroy()
    {
        // Отписываемся от события при уничтожении объекта
        if (resourceManager != null)
        {
            resourceManager.OnResourcesUpdated -= UpdateResourceDisplay;
        }
    }

    public void UpdateResourceDisplay()
    {
        if (resourceManager == null) return;

        // Обновляем текст для каждого ресурса
        if (textWood != null) textWood.text = $"Дерево: {resourceManager.wood}";
        if (textOre != null) textOre.text = $"Руда: {resourceManager.ore}";
        if (textGold != null) textGold.text = $"Золото: {resourceManager.gold}";
        if (textFood != null) textFood.text = $"Еда: {resourceManager.food}";
    }
}