using UnityEngine;

public class SfxManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;

    private static SfxManager instance;

    public static SfxManager Instance
    {
        get
        {
            if (instance == null)
            {
                CreateInstance();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureAudioSource();
    }

    public static void PlayClip(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
        {
            return;
        }

        Instance.PlayOneShotInternal(clip, volume);
    }

    void PlayOneShotInternal(AudioClip clip, float volume)
    {
        EnsureAudioSource();
        audioSource.PlayOneShot(clip, Mathf.Clamp01(masterVolume) * Mathf.Clamp01(volume));
    }

    void EnsureAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    static void CreateInstance()
    {
        GameObject go = new GameObject("SfxManager");
        instance = go.AddComponent<SfxManager>();
        DontDestroyOnLoad(go);
    }
}
