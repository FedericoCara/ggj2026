using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DualMusicPlayer : MonoBehaviour
{
    [Header("Clips")]
    public AudioClip introClip;
    public AudioClip loopClip;

    [Header("Sources (optional)")]
    public AudioSource introSource;
    public AudioSource loopSource;

    [Header("Playback")]
    public bool playOnStart = true;
    [Min(0f)] public float scheduleLeadTime = 0.1f;

    [Header("Persistence")]
    public bool persistAcrossScenes;
    public bool stopOnGameScene = true;
    public string gameSceneName = "Game";

    static DualMusicPlayer instance;
    Coroutine playRoutine;
    bool sceneHooked;

    void Awake()
    {
        EnsureSources();
        if (persistAcrossScenes)
        {
            SetupPersistence();
        }
    }

    void Start()
    {
        if (playOnStart)
        {
            Play();
        }
    }

    public void Play()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
        }

        playRoutine = StartCoroutine(PlayRoutine());
    }

    public void PlayPersistent()
    {
        persistAcrossScenes = true;
        if (!SetupPersistence())
        {
            return;
        }
        Play();
    }

    public void PlayPersistentUntilScene(string sceneName)
    {
        gameSceneName = sceneName;
        stopOnGameScene = true;
        PlayPersistent();
    }

    public void Stop()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }

        if (introSource != null)
        {
            introSource.Stop();
        }

        if (loopSource != null)
        {
            loopSource.Stop();
        }
    }

    public void StopAndDestroy()
    {
        Stop();
        Destroy(gameObject);
    }

    IEnumerator PlayRoutine()
    {
        if (introClip == null || loopClip == null)
        {
            yield break;
        }

        yield return EnsureLoaded(introClip);
        yield return EnsureLoaded(loopClip);

        EnsureSources();

        introSource.clip = introClip;
        introSource.loop = false;
        loopSource.clip = loopClip;
        loopSource.loop = true;

        double startTime = AudioSettings.dspTime + Mathf.Max(0.0f, scheduleLeadTime);
        introSource.PlayScheduled(startTime);
        loopSource.PlayScheduled(startTime + introClip.length);
    }

    IEnumerator EnsureLoaded(AudioClip clip)
    {
        if (clip.loadState == AudioDataLoadState.Loaded)
        {
            yield break;
        }

        clip.LoadAudioData();
        while (clip.loadState == AudioDataLoadState.Loading)
        {
            yield return null;
        }
    }

    void EnsureSources()
    {
        if (introSource == null)
        {
            introSource = GetComponent<AudioSource>();
        }

        if (introSource == null)
        {
            introSource = gameObject.AddComponent<AudioSource>();
        }

        if (loopSource == null)
        {
            GameObject child = new GameObject("Loop Music Source");
            child.transform.SetParent(transform, false);
            loopSource = child.AddComponent<AudioSource>();
        }

        introSource.playOnAwake = false;
        loopSource.playOnAwake = false;
        introSource.loop = false;
        loopSource.loop = true;
    }

    bool SetupPersistence()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return false;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (!sceneHooked && stopOnGameScene)
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
            sceneHooked = true;
        }

        return true;
    }

    void OnDestroy()
    {
        if (sceneHooked)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            sceneHooked = false;
        }

        if (instance == this)
        {
            instance = null;
        }
    }

    void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!stopOnGameScene)
        {
            return;
        }

        if (!string.IsNullOrEmpty(gameSceneName) && scene.name == gameSceneName)
        {
            StopAndDestroy();
        }
    }
}
