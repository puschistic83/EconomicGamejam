using UnityEngine;
using System.Collections.Generic;

public class RoadManager : MonoBehaviour
{
    public static RoadManager Instance { get; private set; }

    private List<Vector3> roadPositions = new List<Vector3>();
    private ResourceManager resourceManager;

    public event System.Action OnRoadsUpdated;

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
        resourceManager = FindObjectOfType<ResourceManager>();
    }

    public void RegisterRoad(Vector3 position)
    {
        if (!roadPositions.Contains(position))
        {
            roadPositions.Add(position);
            OnRoadsUpdated?.Invoke();
            Debug.Log($"Road registered at {position}. Total roads: {roadPositions.Count}");
        }
    }

    public void UnregisterRoad(Vector3 position)
    {
        if (roadPositions.Contains(position))
        {
            roadPositions.Remove(position);
            OnRoadsUpdated?.Invoke();
        }
    }

    public bool IsConnectedToMainBuilding(Vector3 buildingPosition)
    {
        if (!resourceManager.HasMainBuilding) return false;

        Vector3 mainBuildingPos = resourceManager.mainBuilding.transform.position;

        // ѕроста€ проверка: есть ли дорога р€дом с зданием и главным зданием
        bool hasRoadNearBuilding = HasRoadNearby(buildingPosition, 1.5f);
        bool hasRoadNearMain = HasRoadNearby(mainBuildingPos, 1.5f);

        // ≈сли оба имеют дороги р€дом - считаем соединенными
        return hasRoadNearBuilding && hasRoadNearMain;
    }

    private bool HasRoadNearby(Vector3 position, float maxDistance)
    {
        foreach (Vector3 roadPos in roadPositions)
        {
            if (Vector3.Distance(position, roadPos) <= maxDistance)
            {
                return true;
            }
        }
        return false;
    }

    public List<Vector3> GetAllRoadPositions()
    {
        return new List<Vector3>(roadPositions);
    }
}