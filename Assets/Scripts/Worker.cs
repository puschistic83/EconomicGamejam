using UnityEngine;
using System.Collections;

public class Worker : MonoBehaviour
{
    private WorkerManager workerManager;
    private Building targetHouse;
    private float moveSpeed = 2f;

    public void Initialize(WorkerManager manager)
    {
        workerManager = manager;
        Debug.Log($"Worker initialized. Available workers before: {workerManager.availableWorkers}");
        FindHouseAndMove();
    }

    private void FindHouseAndMove()
    {
        targetHouse = workerManager.FindAvailableHouse();

        if (targetHouse != null)
        {
            Debug.Log($"Worker found house: {targetHouse.data.buildingName}");
            StartCoroutine(MoveToHouse());
        }
        else
        {
            Debug.Log("No available houses found for worker");
            Destroy(gameObject, 10f);
        }
    }

    private IEnumerator MoveToHouse()
    {
        Debug.Log($"Worker moving to {targetHouse.data.buildingName}");

        while (Vector3.Distance(transform.position, targetHouse.transform.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetHouse.transform.position,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        Debug.Log($"Worker arrived at {targetHouse.data.buildingName}. Available workers before: {workerManager.availableWorkers}");
        workerManager.OnWorkerArrivedAtHouse(targetHouse);
        Debug.Log($"Worker destroyed. Available workers after: {workerManager.availableWorkers}");
        Destroy(gameObject);
    }
}