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

        // ������������� �� ������� ���������� ��������
        resourceManager.OnResourcesUpdated += UpdateResourceDisplay;

        // ��������� ����������� ����� ��� ������
        UpdateResourceDisplay();
    }

    void OnDestroy()
    {
        // ������������ �� ������� ��� ����������� �������
        if (resourceManager != null)
        {
            resourceManager.OnResourcesUpdated -= UpdateResourceDisplay;
        }
    }

    public void UpdateResourceDisplay()
    {
        if (resourceManager == null) return;

        // ��������� ����� ��� ������� �������
        if (textWood != null) textWood.text = $"������: {resourceManager.wood}";
        if (textOre != null) textOre.text = $"����: {resourceManager.ore}";
        if (textGold != null) textGold.text = $"������: {resourceManager.gold}";
        if (textFood != null) textFood.text = $"���: {resourceManager.food}";
    }
}