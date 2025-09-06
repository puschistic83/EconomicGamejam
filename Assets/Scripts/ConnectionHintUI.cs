using UnityEngine;
using TMPro;

public class ConnectionHintUI : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    private ResourceManager resourceManager;
    private RoadManager roadManager;

    void Start()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        roadManager = FindObjectOfType<RoadManager>();

        if (roadManager != null)
        {
            roadManager.OnRoadsUpdated += UpdateHint;
        }
        UpdateHint();
    }

    void OnDestroy()
    {
        if (roadManager != null)
        {
            roadManager.OnRoadsUpdated -= UpdateHint;
        }
    }

    void UpdateHint()
    {
        if (hintText == null) return;

        if (!resourceManager.HasMainBuilding)
        {
            hintText.text = "🏗️ First build a Town Hall!";
            hintText.color = Color.yellow;
        }
        else
        {
            hintText.text = "🛣️ Connect buildings with roads to make them work!\nPress R to build roads";
            hintText.color = Color.cyan;
        }
    }
}