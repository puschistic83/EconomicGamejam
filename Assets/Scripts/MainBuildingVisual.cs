using UnityEngine;

public class MainBuildingVisual : MonoBehaviour
{
    public GameObject rangeVisualization;
    private ResourceManager resourceManager;

    void Start()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        CreateRangeVisualization();
        UpdateRangeVisualization();
    }

    void Update()
    {
        UpdateRangeVisualization();
    }

    private void CreateRangeVisualization()
    {
        if (rangeVisualization == null && resourceManager != null)
        {
            rangeVisualization = new GameObject("RangeVisualization");
            rangeVisualization.transform.SetParent(transform);
            rangeVisualization.transform.localPosition = Vector3.zero;

            SpriteRenderer renderer = rangeVisualization.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateCircleSprite();
            renderer.color = new Color(0, 0.5f, 1f, 0.15f); // Легкий голубой
            renderer.sortingOrder = -1;
        }
    }

    private void UpdateRangeVisualization()
    {
        if (rangeVisualization != null && resourceManager != null)
        {
            float diameter = resourceManager.deliveryRange * 2f;
            rangeVisualization.transform.localScale = new Vector3(diameter, diameter, 1f);
        }
    }

    private Sprite CreateCircleSprite()
    {
        int textureSize = 128;
        Texture2D texture = new Texture2D(textureSize, textureSize);

        Vector2 center = new Vector2(textureSize / 2, textureSize / 2);
        float radius = textureSize / 2;

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Clamp01(1 - (distance / radius));
                texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f));
    }
}