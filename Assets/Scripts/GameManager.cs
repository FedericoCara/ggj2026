using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip winClip;
    public AudioClip failClip;
    public GameObject winPopup;
    public int requiredSequenceLength = 5;
    public List<Image> semaphoreImages = new List<Image>();

    readonly List<Color> targetOrder = new List<Color>();
    readonly List<Color> inputSequence = new List<Color>();
    Coroutine pendingReset;
    bool inputLocked;

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
        inputLocked = false;
        SetAllSemaphoreAlpha(0f);
    }

    public void RegisterColorInput(Color color)
    {
        if (inputLocked)
        {
            return;
        }

        inputSequence.Add(color);
        SetSemaphoreColor(inputSequence.Count - 1, color);

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

        inputLocked = true;
        SetAllSemaphoreColors(Color.white);
        StartDelayedPopup();
    }

    void HandleFail()
    {
        if (audioSource != null && failClip != null)
        {
            audioSource.PlayOneShot(failClip);
        }

        inputLocked = true;
        StartResetAfterDelay();
    }

    void SetSemaphoreColor(int index, Color color)
    {
        if (index < 0 || index >= semaphoreImages.Count)
        {
            return;
        }

        Image image = semaphoreImages[index];
        if (image != null)
        {
            image.color = color;
        }
    }

    void SetAllSemaphoreColors(Color color)
    {
        for (int i = 0; i < semaphoreImages.Count; i++)
        {
            Image image = semaphoreImages[i];
            if (image != null)
            {
                image.color = color;
            }
        }
    }

    void SetAllSemaphoreAlpha(float alpha)
    {
        for (int i = 0; i < semaphoreImages.Count; i++)
        {
            Image image = semaphoreImages[i];
            if (image != null)
            {
                Color current = image.color;
                current.a = alpha;
                image.color = current;
            }
        }
    }

    void StartResetAfterDelay()
    {
        if (pendingReset != null)
        {
            StopCoroutine(pendingReset);
        }

        pendingReset = StartCoroutine(ResetAfterDelay());
    }

    void StartDelayedPopup()
    {
        if (pendingReset != null)
        {
            StopCoroutine(pendingReset);
        }

        pendingReset = StartCoroutine(ShowPopupAfterDelay());
    }

    IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        inputSequence.Clear();
        inputLocked = false;
        SetAllSemaphoreAlpha(0f);
    }

    IEnumerator ShowPopupAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        if (winPopup != null)
        {
            winPopup.SetActive(true);
        }
    }
}
