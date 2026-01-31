using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ColorButtonsSpawner : MonoBehaviour
{
    public TargetSpawner targetSpawner;
    public Button buttonPrefab;
    public Transform buttonParent;

    void Start()
    {
        if (buttonParent == null)
        {
            buttonParent = transform;
        }

        StartCoroutine(SpawnWhenReady());
    }

    IEnumerator SpawnWhenReady()
    {
        if (targetSpawner == null)
        {
            Debug.LogWarning("[ColorButtonsSpawner] Missing targetSpawner.");
            yield break;
        }

        if (buttonPrefab == null)
        {
            Debug.LogWarning("[ColorButtonsSpawner] Missing buttonPrefab.");
            yield break;
        }

        while (!targetSpawner.hasSpawned)
        {
            yield return null;
        }

        if (targetSpawner.spawnedColors == null || targetSpawner.spawnedColors.Count == 0)
        {
            Debug.LogWarning("[ColorButtonsSpawner] No colors to spawn.");
            yield break;
        }

        ClearChildren();

        for (int i = 0; i < targetSpawner.spawnedColors.Count; i++)
        {
            Button instance = Instantiate(buttonPrefab, buttonParent);
            SetButtonColor(instance, targetSpawner.spawnedColors[i]);
        }
    }

    void SetButtonColor(Button button, Color color)
    {
        if (button == null)
        {
            return;
        }

        Graphic graphic = button.targetGraphic;
        if (graphic == null)
        {
            graphic = button.GetComponent<Graphic>();
        }

        if (graphic == null)
        {
            Debug.LogWarning("[ColorButtonsSpawner] Button has no Graphic to color.");
            return;
        }

        graphic.color = color;
    }

    void ClearChildren()
    {
        for (int i = buttonParent.childCount - 1; i >= 0; i--)
        {
            Destroy(buttonParent.GetChild(i).gameObject);
        }
    }
}
