using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WorkerManager : MonoBehaviour
{
    public static WorkerManager Instance { get; private set; }

    [Header("Settings")]
    public float workerSpawnInterval = 5f;
    public float workerMoveSpeed = 2f;
    public int workersPerHouse = 5;
    public GameObject workerPrefab;

    [Header("Current Stats")]
    public int availableWorkers = 0;
    public int totalWorkers = 0;
    public int maxWorkers = 0;

    private List<Building> houses = new List<Building>();
    private List<Worker> allWorkers = new List<Worker>();

    public System.Action OnWorkersUpdated;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(WorkerSpawningCoroutine());
    }

    public void RegisterHouse(Building house)
    {
        if (!houses.Contains(house))
        {
            houses.Add(house);
            UpdateMaxWorkers();
        }
    }

    public void UnregisterHouse(Building house)
    {
        if (houses.Contains(house))
        {
            houses.Remove(house);
            UpdateMaxWorkers();
        }
    }

    private void UpdateMaxWorkers()
    {
        int oldMax = maxWorkers;
        maxWorkers = 0;

        Debug.Log($"Updating max workers. Houses count: {houses.Count}");

        foreach (Building house in houses)
        {
            if (house.isConnected)
            {
                maxWorkers += workersPerHouse;
                Debug.Log($"Connected house: {house.data.buildingName}, adds {workersPerHouse} workers. Current workers: {house.currentWorkers}");
            }
        }

        Debug.Log($"Max workers updated: {oldMax} -> {maxWorkers}");

        // ОБРАТИТЕ ВНИМАНИЕ: если availableWorkers > maxWorkers, нужно скорректировать
        if (availableWorkers > maxWorkers)
        {
            Debug.Log($"Adjusting available workers: {availableWorkers} -> {maxWorkers}");
            availableWorkers = maxWorkers;
            totalWorkers = availableWorkers;
        }

        OnWorkersUpdated?.Invoke();
    }

    public bool CanAddWorkers(int amount = 1)
    {
        return availableWorkers + amount <= maxWorkers;
    }
        
    private IEnumerator WorkerSpawningCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(workerSpawnInterval);

            // Проверяем, есть ли подключенные дома с свободными местами
            bool hasValidHouses = false;
            foreach (Building house in houses)
            {
                if (house.isConnected && house.currentWorkers < workersPerHouse)
                {
                    hasValidHouses = true;
                    break;
                }
            }

            Debug.Log($"Worker spawn check: CanAddWorkers: {CanAddWorkers()}, HasValidHouses: {hasValidHouses}, Available: {availableWorkers}, Max: {maxWorkers}");

            if (CanAddWorkers() && hasValidHouses)
            {
                SpawnWorker();
            }
            else
            {
                Debug.Log($"Cannot spawn worker: CanAddWorkers: {CanAddWorkers()}, HasValidHouses: {hasValidHouses}");
            }
        }
    }

    private void SpawnWorker()
    {
        if (workerPrefab == null)
        {
            Debug.LogError("Worker prefab is not assigned!");
            return;
        }

        Vector3 spawnPosition = Vector3.zero;
        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        if (resourceManager != null && resourceManager.HasMainBuilding)
        {
            spawnPosition = resourceManager.mainBuilding.transform.position;
        }

        GameObject workerObj = Instantiate(workerPrefab, spawnPosition, Quaternion.identity);
        Worker worker = workerObj.GetComponent<Worker>();

        if (worker != null)
        {
            worker.Initialize(this);
            allWorkers.Add(worker);
            AddWorkers(1);
            Debug.Log($"Worker spawned! Available: {availableWorkers}/{maxWorkers}");
        }
    }

    public void AddWorkers(int amount)
    {
        int oldAvailable = availableWorkers;
        availableWorkers = Mathf.Min(availableWorkers + amount, maxWorkers);
        totalWorkers = availableWorkers;

        Debug.Log($"AddWorkers: {oldAvailable} + {amount} = {availableWorkers}, Max: {maxWorkers}");
        OnWorkersUpdated?.Invoke();
    }


    public Building FindAvailableHouse()
    {
        foreach (Building house in houses)
        {
            if (house.isConnected && house.currentWorkers < workersPerHouse)
            {
                return house;
            }
        }
        return null;
    }

    public void OnWorkerArrivedAtHouse(Building house)
    {
        Debug.Log($"Worker arrived at {house.data.buildingName}. Available before: {availableWorkers}, House workers before: {house.currentWorkers}");

        // Дополнительная проверка на случай, если дом отключился пока рабочий шел
        if (house.isConnected)
        {
            house.currentWorkers++;
            availableWorkers--;
            OnWorkersUpdated?.Invoke();
            house.UpdateStatusDisplay();
            Debug.Log($"Worker entered house. Available after: {availableWorkers}, House workers after: {house.currentWorkers}");
        }
        else
        {
            // Если дом отключился, возвращаем рабочего в доступные
            availableWorkers++;
            Debug.Log("Worker couldn't enter house - no road connection. Available workers returned");
        }
    }

    public int GetHousesCount()
    {
        return houses.Count;
    }

    public int GetConnectedHousesCount()
    {
        int count = 0;
        foreach (Building house in houses)
        {
            if (house.isConnected)
            {
                count++;
            }
        }
        return count;
    }

    // Метод для обновления максимального количества рабочих при изменении соединения
    public void UpdateHouseConnection(Building house)
    {
        UpdateMaxWorkers();
    }
    public bool AssignWorkerToBuilding(Building building)
    {
        if (availableWorkers > 0 &&
            building != null &&
            building.data != null &&
            building.currentWorkers < building.data.maxWorkers &&
            building.isConnected)
        {
            availableWorkers--;
            building.AssignWorker();
            OnWorkersUpdated?.Invoke();

            Debug.Log($"Worker assigned to {building.data.buildingName}. " +
                     $"Available: {availableWorkers}, Building: {building.currentWorkers}/{building.data.maxWorkers}");
            return true;
        }

        Debug.Log($"Cannot assign worker to {building?.data?.buildingName}. " +
                 $"Available: {availableWorkers}, Connected: {building?.isConnected}, " +
                 $"Slots: {building?.currentWorkers}/{building?.data?.maxWorkers}");
        return false;
    }

    public bool RemoveWorkerFromBuilding(Building building)
    {
        if (building.currentWorkers > 0)
        {
            building.RemoveWorker();
            availableWorkers++;
            OnWorkersUpdated?.Invoke();
            return true;
        }
        return false;
    }

}