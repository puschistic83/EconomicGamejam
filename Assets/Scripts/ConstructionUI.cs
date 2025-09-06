using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConstructionUI : MonoBehaviour
{
    [Header("Building Data Assets")]
    public BuildingData houseData;
    public BuildingData lumbermillData;
    public BuildingData farmData;
    public BuildingData mineData;
    public RoadData basicRoadData;
    public BuildingData mainBuildingData;

    [Header("UI Buttons")]
    public Button btnHouse;
    public Button btnLumbermill;
    public Button btnFarm;
    public Button btnMine;
    public Button btnRoad;
    public Button btnMainBuilding;

    private BuildingPlacementSystem buildingPlacementSystem;

    private ResourceManager resourceManager;


    void Start()
    {
        buildingPlacementSystem = FindObjectOfType<BuildingPlacementSystem>();
        resourceManager = FindObjectOfType<ResourceManager>();

        if (resourceManager != null)
        {
            resourceManager.OnResourcesUpdated += UpdateButtonsInteractability;
        }

        // Настраиваем кнопки
        SetupButton(btnHouse, houseData, "House");
        SetupButton(btnLumbermill, lumbermillData, "Lumbermill");
        SetupButton(btnFarm, farmData, "Farm");
        SetupButton(btnMine, mineData, "Mine");
        SetupButton(btnRoad, basicRoadData, "Road");

        // Делаем кнопку главного здания более заметной
        if (btnMainBuilding != null)
        {
            btnMainBuilding.image.color = Color.yellow;
            Text btnText = btnMainBuilding.GetComponentInChildren<Text>();
            if (btnText != null) btnText.color = Color.black;
            btnMainBuilding.onClick.AddListener(() => SelectBuilding(mainBuildingData));
        }
    }

    private void SetupButton(Button button, BuildingData data, string buttonName)
    {
        if (button != null && data != null)
        {
            button.onClick.AddListener(() => SelectBuilding(data));
            UpdateButtonText(button, $"{buttonName}\nWood: {data.woodCost} Ore: {data.oreCost}");
        }
    }

    private void SetupButton(Button button, RoadData data, string buttonName)
    {
        if (button != null && data != null)
        {
            button.onClick.AddListener(() => SelectRoad(data));
            UpdateButtonText(button, $"{buttonName}\nWood: {data.woodCost} Ore: {data.oreCost}");
        }
    }

    private void UpdateButtonText(Button button, string text)
    {
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = text;
        }

        // Для TextMeshPro
        TextMeshProUGUI tmpText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = text;
        }
    }

    public void SelectBuilding(BuildingData data)
    {
        if (buildingPlacementSystem != null && data != null)
        {
            buildingPlacementSystem.SelectBuilding(data);
            Debug.Log($"Selected: {data.buildingName}");
        }
    }

    public void SelectRoad(RoadData data)
    {
        if (buildingPlacementSystem != null && data != null)
        {
            buildingPlacementSystem.SelectRoad(data);
            Debug.Log($"Selected: {data.roadName}");
        }
    }

    void OnDestroy()
    {
        if (resourceManager != null)
        {
            resourceManager.OnResourcesUpdated -= UpdateButtonsInteractability;
        }
    }

    private void UpdateButtonsInteractability()
    {
        UpdateButtonInteractability(btnRoad, basicRoadData);
        // Добавьте для других кнопок по необходимости
    }

    private void UpdateButtonInteractability(Button button, RoadData data)
    {
        if (button != null && data != null && resourceManager != null)
        {
            button.interactable = resourceManager.CanAfford(data.woodCost, data.oreCost, 0, 0);
        }
    }
}