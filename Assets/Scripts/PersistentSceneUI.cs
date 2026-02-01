using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentSceneUI : MonoBehaviour
{
    [SerializeField] private string[] visibleScenes = new[] { "Game" };
    [SerializeField] private bool keepOnlyOne = true;
    [SerializeField] private bool hideWhenNotVisible = true;

    private static PersistentSceneUI instance;

    void Awake()
    {
        if (keepOnlyOne && instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += HandleSceneLoaded;

        ApplyVisibility(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;

        if (instance == this)
        {
            instance = null;
        }
    }

    void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyVisibility(scene.name);
    }

    void ApplyVisibility(string sceneName)
    {
        bool visible = IsVisibleInScene(sceneName);
        if (hideWhenNotVisible)
        {
            gameObject.SetActive(visible);
        }
    }

    bool IsVisibleInScene(string sceneName)
    {
        if (visibleScenes == null || visibleScenes.Length == 0)
        {
            return true;
        }

        for (int i = 0; i < visibleScenes.Length; i++)
        {
            if (visibleScenes[i] == sceneName)
            {
                return true;
            }
        }

        return false;
    }
}
