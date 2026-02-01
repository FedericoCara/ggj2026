using UnityEngine;

public class DragWithinSpriteBounds : MonoBehaviour
{
    public SpriteRenderer background;
    public Vector2 clampOffset;

    bool isDragging;
    Vector3 dragOffset;
    float targetZ;
    SpriteRenderer targetRenderer;
    SpriteMask targetMask;
    Collider2D targetCollider;
    Camera worldCamera;

    void Awake()
    {
        targetRenderer = GetComponent<SpriteRenderer>();
        targetMask = GetComponent<SpriteMask>();
        targetCollider = GetComponent<Collider2D>();
        worldCamera = Camera.main;
    }

    void Update()
    {
        if (background == null || worldCamera == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = GetMouseWorldPosition();
            if (IsMouseOverTarget(mouseWorld))
            {
                BeginDragFromMouse(mouseWorld);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 mouseWorld = GetMouseWorldPosition();
            Vector3 desired = mouseWorld + dragOffset;
            Vector3 clamped = ClampToBackground(desired);
            clamped.z = targetZ;
            transform.position = clamped;
        }
    }

    public void BeginDragFromMouse(Vector3 mouseWorld)
    {
        if (background == null || worldCamera == null)
        {
            return;
        }

        if (!IsMouseOverTarget(mouseWorld))
        {
            return;
        }

        isDragging = true;
        targetZ = transform.position.z;
        dragOffset = transform.position - mouseWorld;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 screen = Input.mousePosition;
        Vector3 camToTarget = transform.position - worldCamera.transform.position;
        float depth = Vector3.Dot(camToTarget, worldCamera.transform.forward);
        screen.z = depth;
        return worldCamera.ScreenToWorldPoint(screen);
    }

    bool IsMouseOverTarget(Vector3 mouseWorld)
    {
        if (targetCollider != null)
        {
            return targetCollider.OverlapPoint(mouseWorld);
        }

        Bounds bounds = GetTargetBounds();
        bool insideX = mouseWorld.x >= bounds.min.x && mouseWorld.x <= bounds.max.x;
        bool insideY = mouseWorld.y >= bounds.min.y && mouseWorld.y <= bounds.max.y;
        return insideX && insideY;
    }

    Bounds GetTargetBounds()
    {
        if (targetRenderer != null)
        {
            return targetRenderer.bounds;
        }

        if (targetMask != null && targetMask.sprite != null)
        {
            Vector3 size = Vector3.Scale(targetMask.sprite.bounds.size, transform.lossyScale);
            return new Bounds(transform.position, size);
        }

        return new Bounds(transform.position, Vector3.zero);
    }

    Vector3 ClampToBackground(Vector3 desiredCenter)
    {
        Bounds bg = background.bounds;
        Vector3 extents = GetTargetBounds().extents;

        float minX = bg.min.x + extents.x - clampOffset.x;
        float maxX = bg.max.x - extents.x + clampOffset.x;
        float minY = bg.min.y + extents.y - clampOffset.y;
        float maxY = bg.max.y - extents.y + clampOffset.y;

        if (minX > maxX)
        {
            desiredCenter.x = bg.center.x;
        }
        else
        {
            desiredCenter.x = Mathf.Clamp(desiredCenter.x, minX, maxX);
        }

        if (minY > maxY)
        {
            desiredCenter.y = bg.center.y;
        }
        else
        {
            desiredCenter.y = Mathf.Clamp(desiredCenter.y, minY, maxY);
        }

        return desiredCenter;
    }
}
