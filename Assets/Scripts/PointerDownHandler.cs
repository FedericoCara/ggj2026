using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerDownHandler : MonoBehaviour, IPointerDownHandler
{
    public Action onPointerDown;

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke();
    }
}
