using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRelated : MonoBehaviour
{

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnHome()
    {
        SceneManager.LoadScene("Home");
    }

    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }
}
