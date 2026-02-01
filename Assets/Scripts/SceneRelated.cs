using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRelated : MonoBehaviour
{

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit()
    {
        SceneManager.LoadScene("Home");
    }
}
