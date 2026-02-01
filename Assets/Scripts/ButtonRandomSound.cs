using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonRandomSound : MonoBehaviour
{
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    Button cachedButton;

    void Awake()
    {
        cachedButton = GetComponent<Button>();
    }

    void OnEnable()
    {
        if (cachedButton == null)
        {
            cachedButton = GetComponent<Button>();
        }

        cachedButton.onClick.AddListener(PlayRandomClip);
    }

    void OnDisable()
    {
        if (cachedButton != null)
        {
            cachedButton.onClick.RemoveListener(PlayRandomClip);
        }
    }

    void PlayRandomClip()
    {
        if (clips == null || clips.Count == 0)
        {
            return;
        }

        int index = Random.Range(0, clips.Count);
        AudioClip clip = clips[index];
        if (clip != null)
        {
            SfxManager.PlayClip(clip, volume);
        }
    }
}
