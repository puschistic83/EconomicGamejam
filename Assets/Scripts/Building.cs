using System.Collections;
using TMPro;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Building Data")]
    public BuildingData data;

    [Header("Current State")]
    public int currentWorkers = 0;
    public bool isOperational = false;
    public bool isConnected = false;

    private ResourceManager resourceManager;
    private RoadManager roadManager;
    private WorkerManager workerManager;
    private const float productionInterval = 1f;

    [Header("Visual Feedback")]
    public TextMeshPro statusText;
    public Renderer buildingRenderer;
    public Material operationalMaterial;
    public Material nonOperationalMaterial;

    void Start()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        roadManager = FindObjectOfType<RoadManager>();
        workerManager = FindObjectOfType<WorkerManager>();

        // Регистрируем дом в менеджере рабочих ЕСЛИ это дом
        if (IsHouse() && workerManager != null)
        {
            workerManager.RegisterHouse(this);
        }

        // Настраиваем визуал
        SetupVisuals();

        // Запускаем проверки
        StartCoroutine(ConnectionCheckCoroutine());
        StartCoroutine(ProductionCoroutine());
    }

    public void Initialize(BuildingData buildingData)
    {
        data = buildingData;

        // Устанавливаем спрайт
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && data.buildingSprite != null)
        {
            spriteRenderer.sprite = data.buildingSprite;
        }

        // Добавляем коллайдер если нужно
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }

        SetupVisuals();
    }

    // Проверяем, является ли здание домом
    public bool IsHouse()
    {
        if (data == null) return false;

        return data.buildingName.ToLower().Contains("house") ||
               data.buildingName.ToLower().Contains("дом") ||
               data.buildingName.ToLower().Contains("home") ||
               data.buildingName.ToLower().Contains("жилой");
    }

    private void SetupVisuals()
    {
        // Создаем текст статуса если его нет
        if (statusText == null)
        {
            GameObject textObj = new GameObject("StatusText");
            textObj.transform.SetParent(transform);
            textObj.transform.localPosition = new Vector3(0, 1.5f, 0);

            statusText = textObj.AddComponent<TextMeshPro>();
            statusText.alignment = TextAlignmentOptions.Center;
            statusText.fontSize = 1.5f;
            statusText.sortingOrder = 10;
        }

        UpdateStatusDisplay();
    }

    private IEnumerator ConnectionCheckCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            CheckConnection();
        }
    }

    private void CheckConnection()
    {
        if (roadManager == null || resourceManager == null) return;

        bool newConnectionStatus = roadManager.IsConnectedToMainBuilding(transform.position);

        if (newConnectionStatus != isConnected)
        {
            isConnected = newConnectionStatus;
            Debug.Log($"{data.buildingName} connection: {isConnected}");
            UpdateStatusDisplay();

            if (IsHouse() && workerManager != null)
            {
                Debug.Log($"Updating house connection for {data.buildingName}");
                workerManager.UpdateHouseConnection(this);
            }
        }
    }

    private IEnumerator ProductionCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(productionInterval);

            if (CanOperate())
            {
                ProduceResources();
            }
        }
    }

    private bool CanOperate()
    {
        bool hasConnection = isConnected;
        bool hasWorkers = data.requiredWorkers <= 0 || currentWorkers >= data.requiredWorkers;
        bool isOperational = hasConnection && hasWorkers;

        if (this.isOperational != isOperational)
        {
            this.isOperational = isOperational;
            UpdateStatusDisplay();
        }

        return isOperational;
    }

    private void ProduceResources()
    {
        if (!isOperational || resourceManager == null) return;

        // Производство
        if (data.woodProduction > 0)
            resourceManager.AddResources(data.woodProduction, 0, 0, 0);

        if (data.oreProduction > 0)
            resourceManager.AddResources(0, data.oreProduction, 0, 0);

        if (data.goldProduction > 0)
            resourceManager.AddResources(0, 0, data.goldProduction, 0);

        if (data.foodProduction > 0)
            resourceManager.AddResources(0, 0, 0, data.foodProduction);

        // Потребление
        ConsumeResources();
    }

    private void ConsumeResources()
    {
        if (data.woodConsumption > 0 || data.oreConsumption > 0 || data.foodConsumption > 0)
        {
            bool canAfford = resourceManager.CanAfford(
                data.woodConsumption,
                data.oreConsumption,
                0,
                data.foodConsumption
            );

            if (canAfford)
            {
                resourceManager.SpendResources(
                    data.woodConsumption,
                    data.oreConsumption,
                    0,
                    data.foodConsumption
                );
            }
        }
    }

    public void UpdateStatusDisplay()
    {
        if (statusText == null) return;

        string status = "";

        if (!isConnected)
        {
            status = "No Road Connection";
            statusText.color = Color.red;
        }
        else if (IsHouse() && currentWorkers < GetHouseCapacity())
        {
            status = $"Workers: {currentWorkers}/{GetHouseCapacity()}";
            statusText.color = Color.yellow;
        }
        else if (data.requiredWorkers > 0 && currentWorkers < data.requiredWorkers)
        {
            status = $"Need Workers: {currentWorkers}/{data.requiredWorkers}";
            statusText.color = Color.yellow;
        }
        else
        {
            status = "Operational";
            statusText.color = Color.green;
        }

        statusText.text = $"{data.buildingName}\n{status}";

        if (buildingRenderer != null)
        {
            buildingRenderer.material = isOperational ? operationalMaterial : nonOperationalMaterial;
        }
    }

    // Метод для получения вместимости дома из WorkerManager
    private int GetHouseCapacity()
    {
        if (workerManager != null && IsHouse())
        {
            return workerManager.workersPerHouse;
        }
        return data.maxWorkers;
    }

    public void AssignWorker()
    {
        if (currentWorkers < data.maxWorkers)
        {
            currentWorkers++;
            UpdateStatusDisplay();
            Debug.Log($"Worker assigned to {data.buildingName}. Total: {currentWorkers}");
        }
    }

    public void RemoveWorker()
    {
        if (currentWorkers > 0)
        {
            currentWorkers--;
            UpdateStatusDisplay();
            Debug.Log($"Worker removed from {data.buildingName}. Total: {currentWorkers}");
        }
    }

    public int CalculateEffectiveProduction(int baseProduction)
    {
        if (baseProduction == 0) return 0;

        // Простой расчет: если здание работает, производим ресурсы
        return isOperational ? baseProduction : 0;
    }

    public int CalculateEffectiveConsumption(int baseConsumption)
    {
        if (baseConsumption == 0) return 0;

        // Простой расчет: если здание работает, потребляем ресурсы
        return isOperational ? baseConsumption : 0;
    }

    void OnMouseDown()
    {
        Debug.Log($"Building: {data.buildingName}\n" +
                 $"Workers: {currentWorkers}/{data.maxWorkers}\n" +
                 $"Connected: {isConnected}\n" +
                 $"Operational: {isOperational}");
    }

    void OnDestroy()
    {
        // Убираем дом из менеджера при уничтожении
        if (IsHouse() && workerManager != null)
        {
            workerManager.UnregisterHouse(this);
        }
    }
    
}