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

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] List<AudioClip> switchClips = new();
    readonly List<int> remainingClipIndices = new();

    readonly List<Toggle> toggles = new();
    bool suppressToggleEvent;
    int currentIndex = -1;
    bool hasInitialized;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

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
        toggles.Clear();

        for (int i = 0; i < moveLayers.Count; i++)
        {
            int index = i;
            Toggle instance = Instantiate(togglePrefab, toggleParent);
            string label = "Capa " + (index + 1);
            instance.name = label;
            instance.group = toggleGroup;
            SetToggleLabel(instance, label);
            toggles.Add(instance);
            instance.onValueChanged.AddListener(isOn =>
            {
                if (suppressToggleEvent)
                {
                    return;
                }
                if (isOn)
                {
                    ActivateLayer(index);
                }
            });
            instance.isOn = index == 0;
        }

        RefreshRemainingClips();
        ActivateLayer(0);
        hasInitialized = true;
    }

    public void ActivateLayer(int activeIndex)
    {
        if (activeIndex < 0 || activeIndex >= moveLayers.Count)
        {
            return;
        }

        if (currentIndex == activeIndex)
        {
            return;
        }

        currentIndex = activeIndex;

        if (toggles.Count == moveLayers.Count)
        {
            suppressToggleEvent = true;
            for (int i = 0; i < toggles.Count; i++)
            {
                Toggle toggle = toggles[i];
                if (toggle != null)
                {
                    toggle.isOn = i == activeIndex;
                }
            }
            suppressToggleEvent = false;
        }

        SetActiveLayer(activeIndex);

        if (hasInitialized)
        {
            PlayRandomClip();
        }
    }

    public int GetActiveLayerIndex()
    {
        return currentIndex;
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

    void PlayRandomClip()
    {
        if (audioSource == null || switchClips == null || switchClips.Count == 0)
        {
            return;
        }

        if (remainingClipIndices.Count == 0 || remainingClipIndices.Count > switchClips.Count)
        {
            RefreshRemainingClips();
        }

        int pickIndex = Random.Range(0, remainingClipIndices.Count);
        int clipIndex = remainingClipIndices[pickIndex];
        remainingClipIndices.RemoveAt(pickIndex);

        AudioClip clip = switchClips[clipIndex];
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void RefreshRemainingClips()
    {
        remainingClipIndices.Clear();
        for (int i = 0; i < switchClips.Count; i++)
        {
            if (switchClips[i] != null)
            {
                remainingClipIndices.Add(i);
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

        TMPro.TMP_Text tmpText = toggle.GetComponentInChildren<TMPro.TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = text;
        }
    }

    void ClearChildren()
    {
        for (int i = toggleParent.childCount - 1; i >= 0; i--)
        {
            Destroy(toggleParent.GetChild(i).gameObject);
        }
    }
}
