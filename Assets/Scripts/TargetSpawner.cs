using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class TargetSpawner : MonoBehaviour
{
    public Target targetPrefab;
    public List<Transform> spawnPoints = new List<Transform>();
    public int spawnCount = 5;
    public TargetColorPalette colorPalette;
    public GameManager gameManager;
    public IndicatorPostController indicatorPostController;
    public List<Color> spawnedColors = new List<Color>();
    public List<Target> spawnedTargets = new List<Target>();
    public bool hasSpawned;
    public GameObject maskObject;

    private void Awake()
    {
        maskObject.SetActive(true);
    }

    void Start()
    {
        hasSpawned = false;
        spawnedColors.Clear();
        spawnedTargets.Clear();

        if (targetPrefab == null)
        {
            Debug.LogWarning("[TargetSpawner] Missing targetPrefab.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("[TargetSpawner] No spawn points assigned.");
            return;
        }

        int count = Mathf.Clamp(spawnCount, 0, spawnPoints.Count);
        if (count < spawnCount)
        {
            Debug.LogWarning("[TargetSpawner] spawnCount exceeds points. Using " + count + ".");
        }

        int availableColors = (colorPalette != null && colorPalette.colors != null) ? colorPalette.colors.Count : 0;
        if (availableColors < count)
        {
            Debug.LogWarning("[TargetSpawner] Not enough colors for unique assignment. Using " + availableColors + ".");
            count = availableColors;
        }

        List<int> indices = new List<int>(spawnPoints.Count);
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            indices.Add(i);
        }

        Shuffle(indices);

        List<int> numbers = new List<int>(count);
        for (int i = 1; i <= count; i++)
        {
            numbers.Add(i);
        }

        Shuffle(numbers);

        List<Color> colors = new List<Color>();
        if (colorPalette != null && colorPalette.colors != null)
        {
            colors.AddRange(colorPalette.colors);
        }
        Shuffle(colors);

        Color[] orderedColors = new Color[count];
        for (int i = 0; i < count; i++)
        {
            int pointIndex = indices[i];
            Transform point = spawnPoints[pointIndex];
            Target instance = Instantiate(targetPrefab, point.position, point.rotation, transform);
            string order = numbers[i].ToString();
            Color color = colors[i];
            instance.SetTarget(color, order);
            int orderIndex = Mathf.Clamp(numbers[i] - 1, 0, count - 1);
            orderedColors[orderIndex] = color;
            spawnedTargets.Add(instance);
        }

        spawnedColors.Clear();
        spawnedColors.AddRange(orderedColors);

        hasSpawned = true;

        if (gameManager != null)
        {
            gameManager.SetTargetOrder(new List<Color>(orderedColors));
        }
        else
        {
            Debug.LogWarning("[TargetSpawner] Missing gameManager reference.");
        }

        if (indicatorPostController != null)
        {
            indicatorPostController.SetTargets(new List<Target>(spawnedTargets));
        }
        else
        {
            Debug.LogWarning("[TargetSpawner] Missing indicatorPostController reference.");
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
