using UnityEngine;
using TMPro;

public class WorkerUI : MonoBehaviour
{
    public TextMeshProUGUI workerInfoText;
    private WorkerManager workerManager;

    void Start()
    {
        workerManager = WorkerManager.Instance;

        if (workerManager != null)
        {
            workerManager.OnWorkersUpdated += UpdateWorkerInfo;
        }

        UpdateWorkerInfo();
    }

    void OnDestroy()
    {
        if (workerManager != null)
        {
            workerManager.OnWorkersUpdated -= UpdateWorkerInfo;
        }
    }

    void UpdateWorkerInfo()
    {
        if (workerInfoText != null && workerManager != null)
        {
            workerInfoText.text = $"Workers: {workerManager.availableWorkers}/{workerManager.maxWorkers}\n" +
                                 $"Houses: {workerManager.GetConnectedHousesCount()}/{workerManager.GetHousesCount()} connected";
        }
    }
}