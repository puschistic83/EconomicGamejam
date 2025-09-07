using UnityEngine;
using TMPro;

public class DebugInfo : MonoBehaviour
{
    public TextMeshProUGUI debugText;

    private WorkerManager workerManager;
    private BuildingManager buildingManager;
    private AutoWorkerManager autoWorkerManager;

    void Start()
    {
        workerManager = WorkerManager.Instance;
        buildingManager = BuildingManager.Instance;
        autoWorkerManager = FindObjectOfType<AutoWorkerManager>();
    }

    void Update()
    {
        if (debugText != null && workerManager != null)
        {
            string info = $"Workers: {workerManager.availableWorkers}/{workerManager.maxWorkers}\n";
            info += $"Houses: {workerManager.GetHousesCount()} ({workerManager.GetConnectedHousesCount()} connected)\n";

            if (buildingManager != null)
            {
                int productionBuildings = 0;
                foreach (Building building in buildingManager.GetAllBuildings())
                {
                    if (building.data.requiredWorkers > 0 && building.isConnected)
                    {
                        productionBuildings++;
                    }
                }
                info += $"Production buildings: {productionBuildings}";
            }

            debugText.text = info;
        }

        // Принудительная проверка по клавише P
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (autoWorkerManager != null)
            {
                autoWorkerManager.AutoAssignWorkers();
                Debug.Log("Manual assignment triggered");
            }
        }
    }
}