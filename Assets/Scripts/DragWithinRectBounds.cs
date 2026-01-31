using UnityEngine;
using UnityEngine.EventSystems;

public class DragWithinRectBounds : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public RectTransform backgroundRect;
    public RectTransform targetRect;
    public Canvas canvas;
    public Camera uiCamera;

    Vector2 dragOffset;

    void Awake()
    {
        if (targetRect == null)
        {
            targetRect = GetComponent<RectTransform>();
        }

        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        if (uiCamera == null && canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = canvas.worldCamera;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (backgroundRect == null || targetRect == null)
        {
            return;
        }

        Vector2 localPointer;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                backgroundRect, eventData.position, uiCamera, out localPointer))
        {
            dragOffset = targetRect.anchoredPosition - localPointer;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (backgroundRect == null || targetRect == null)
        {
            return;
        }

        Vector2 localPointer;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                backgroundRect, eventData.position, uiCamera, out localPointer))
        {
            Vector2 desired = localPointer + dragOffset;
            targetRect.anchoredPosition = ClampToBackground(desired);
        }
    }

    Vector2 ClampToBackground(Vector2 desiredAnchoredPos)
    {
        Rect bg = backgroundRect.rect;
        Rect trg = targetRect.rect;

        float halfW = trg.width * 0.5f;
        float halfH = trg.height * 0.5f;

        float minX = bg.xMin + halfW;
        float maxX = bg.xMax - halfW;
        float minY = bg.yMin + halfH;
        float maxY = bg.yMax - halfH;

        if (minX > maxX)
        {
            desiredAnchoredPos.x = bg.center.x;
        }
        else
        {
            desiredAnchoredPos.x = Mathf.Clamp(desiredAnchoredPos.x, minX, maxX);
        }

        if (minY > maxY)
        {
            desiredAnchoredPos.y = bg.center.y;
        }
        else
        {
            desiredAnchoredPos.y = Mathf.Clamp(desiredAnchoredPos.y, minY, maxY);
        }

        return desiredAnchoredPos;
    }
}
