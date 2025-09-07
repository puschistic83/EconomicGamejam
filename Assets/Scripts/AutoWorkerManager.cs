using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoWorkerManager : MonoBehaviour
{
    [Header("Settings")]
    public float autoAssignInterval = 10f;
    public bool showDebugLogs = true;

    private WorkerManager workerManager;
    private BuildingManager buildingManager;

    void Start()
    {
        workerManager = WorkerManager.Instance;
        buildingManager = BuildingManager.Instance;

        if (workerManager == null) Debug.LogError("WorkerManager not found!");
        if (buildingManager == null) Debug.LogError("BuildingManager not found!");

        StartCoroutine(AutoAssignCoroutine());
        Debug.Log("AutoWorkerManager started");
    }

    private IEnumerator AutoAssignCoroutine()
    {
        // Ждем перед первым назначением
        yield return new WaitForSeconds(5f);

        while (true)
        {
            yield return new WaitForSeconds(autoAssignInterval);
            AutoAssignWorkers();
        }
    }

    public void AutoAssignWorkers()
    {
        if (workerManager == null || buildingManager == null) return;
        if (workerManager.availableWorkers == 0)
        {
            if (showDebugLogs) Debug.Log("No available workers for auto assignment");
            return;
        }

        // Ищем все подходящие здания
        List<Building> allBuildings = buildingManager.GetAllBuildings();
        List<Building> eligibleBuildings = new List<Building>();

        if (showDebugLogs) Debug.Log($"Checking {allBuildings.Count} buildings for worker assignment");

        foreach (Building building in allBuildings)
        {
            if (CanAssignWorkersToBuilding(building))
            {
                eligibleBuildings.Add(building);
                if (showDebugLogs) Debug.Log($"Eligible: {building.data.buildingName} ({building.currentWorkers}/{building.data.maxWorkers})");
            }
        }

        if (eligibleBuildings.Count == 0)
        {
            if (showDebugLogs) Debug.Log("No eligible buildings found");
            return;
        }

        // Назначаем рабочих на подходящие здания
        foreach (Building building in eligibleBuildings)
        {
            if (workerManager.availableWorkers == 0) break;

            if (workerManager.AssignWorkerToBuilding(building))
            {
                if (showDebugLogs) Debug.Log($"Assigned worker to {building.data.buildingName}");
            }
        }
    }

    private bool CanAssignWorkersToBuilding(Building building)
    {
        // Базовые проверки
        if (building == null || building.data == null)
        {
            if (showDebugLogs) Debug.Log("Building or data is null");
            return false;
        }
        if (!building.isConnected)
        {
            if (showDebugLogs) Debug.Log($"{building.data.buildingName} not connected");
            return false;
        }

        // Проверяем производственные здания (требуют рабочих)
        bool isProductionBuilding = building.data.requiredWorkers > 0;
        bool hasFreeSlots = building.currentWorkers < building.data.maxWorkers;

        // Для производственных зданий: назначаем рабочих если есть свободные места
        // Не проверяем needsWorkers, чтобы можно было назначать сверх минимального требования
        if (isProductionBuilding && hasFreeSlots)
        {
            return true;
        }

        return false;
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}