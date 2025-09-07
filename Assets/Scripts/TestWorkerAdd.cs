using UnityEngine;

public class TestWorkerAdd : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            WorkerManager workerManager = WorkerManager.Instance;
            if (workerManager != null)
            {
                Debug.Log($"Adding 5 workers manually. Before: {workerManager.availableWorkers}");
                workerManager.AddWorkers(5);
                Debug.Log($"After: {workerManager.availableWorkers}");
            }
        }
    }
}