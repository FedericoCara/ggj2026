using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonRandomSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();

    Button cachedButton;

    void Awake()
    {
        cachedButton = GetComponent<Button>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
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
        if (audioSource == null || clips == null || clips.Count == 0)
        {
            return;
        }

        int index = Random.Range(0, clips.Count);
        AudioClip clip = clips[index];
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
