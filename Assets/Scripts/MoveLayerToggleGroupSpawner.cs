using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveLayerToggleGroupSpawner : MonoBehaviour
{
    [Header("UI")]
    public Toggle togglePrefab;
    public Transform toggleParent;
    public ToggleGroup toggleGroup;

    [Header("Layers (assign in editor)")]
    public List<MonoBehaviour> moveLayers = new();

    void Start()
    {
        if (toggleParent == null)
        {
            toggleParent = transform;
        }

        if (togglePrefab == null)
        {
            Debug.LogWarning("[MoveLayerToggleGroupSpawner] Missing togglePrefab.");
            return;
        }

        if (toggleGroup == null)
        {
            toggleGroup = toggleParent.GetComponent<ToggleGroup>();
        }

        if (moveLayers == null || moveLayers.Count == 0)
        {
            Debug.LogWarning("[MoveLayerToggleGroupSpawner] No MoveLayers assigned.");
            return;
        }

        ClearChildren();

        for (int i = 0; i < moveLayers.Count; i++)
        {
            int index = i;
            Toggle instance = Instantiate(togglePrefab, toggleParent);
            string label = "Capa " + (index + 1);
            instance.name = label;
            instance.group = toggleGroup;
            SetToggleLabel(instance, label);
            instance.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    SetActiveLayer(index);
                }
            });
            instance.isOn = index == 0;
        }

        SetActiveLayer(0);
    }

    void SetActiveLayer(int activeIndex)
    {
        for (int i = 0; i < moveLayers.Count; i++)
        {
            var layer = moveLayers[i];
            if (layer != null)
            {
                layer.enabled = i == activeIndex;
            }
        }
    }

    void SetToggleLabel(Toggle toggle, string text)
    {
        if (toggle == null)
        {
            return;
        }

        Text legacyText = toggle.GetComponentInChildren<Text>();
        if (legacyText != null)
        {
            legacyText.text = text;
            return;
        }

        #if TMP_PRESENT
        TMPro.TMP_Text tmpText = toggle.GetComponentInChildren<TMPro.TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = text;
        }
        #endif
    }

    void ClearChildren()
    {
        for (int i = toggleParent.childCount - 1; i >= 0; i--)
        {
            Destroy(toggleParent.GetChild(i).gameObject);
        }
    }
}
