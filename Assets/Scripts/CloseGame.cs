using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CloseGame : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
