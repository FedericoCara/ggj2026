using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip winClip;
    public AudioClip failClip;
    public GameObject winPopup;
    public int requiredSequenceLength = 5;

    readonly List<Color> targetOrder = new List<Color>();
    readonly List<Color> inputSequence = new List<Color>();

    public void SetTargetOrder(List<Color> order)
    {
        targetOrder.Clear();
        if (order != null)
        {
            targetOrder.AddRange(order);
        }

        if (targetOrder.Count > 0)
        {
            requiredSequenceLength = targetOrder.Count;
        }

        inputSequence.Clear();
    }

    public void RegisterColorInput(Color color)
    {
        inputSequence.Add(color);

        if (inputSequence.Count < requiredSequenceLength)
        {
            return;
        }

        bool matches = CheckSequence();
        if (matches)
        {
            HandleWin();
        }
        else
        {
            HandleFail();
        }

        inputSequence.Clear();
    }

    bool CheckSequence()
    {
        if (targetOrder.Count == 0)
        {
            Debug.LogWarning("[GameManager] No target order set.");
            return false;
        }

        int length = Mathf.Min(requiredSequenceLength, targetOrder.Count);
        for (int i = 0; i < length; i++)
        {
            if (inputSequence[i] != targetOrder[i])
            {
                return false;
            }
        }

        return true;
    }

    void HandleWin()
    {
        if (audioSource != null && winClip != null)
        {
            audioSource.PlayOneShot(winClip);
        }

        if (winPopup != null)
        {
            winPopup.SetActive(true);
        }
    }

    void HandleFail()
    {
        if (audioSource != null && failClip != null)
        {
            audioSource.PlayOneShot(failClip);
        }
    }
}
