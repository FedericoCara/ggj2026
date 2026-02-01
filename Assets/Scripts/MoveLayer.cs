using UnityEngine;

public class MoveLayer : MonoBehaviour
{
    public SpriteRenderer background;
    public SpriteRenderer layer;
    public Camera worldCamera;

    bool isDragging;
    Vector3 dragOffset;
    float layerZ;

    void Awake()
    {
        if (layer == null)
        {
            layer = GetComponent<SpriteRenderer>();
        }

        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }

    }

    void Update()
    {
        if (background == null || layer == null || worldCamera == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = GetMouseWorldPosition();
            if (IsMouseOverBackground2D(mouseWorld))
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
            Vector3 clamped = ClampLayerToBackground(desired);
            clamped.z = layerZ;
            layer.transform.position = clamped;
        }
    }

    public void BeginDragFromMouse(Vector3 mouseWorld)
    {
        if (background == null || layer == null || worldCamera == null)
        {
            return;
        }

        if (!IsMouseOverBackground2D(mouseWorld))
        {
            return;
        }

        isDragging = true;
        layerZ = layer.transform.position.z;
        dragOffset = layer.transform.position - mouseWorld;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 screen = Input.mousePosition;
        Vector3 camToLayer = layer.transform.position - worldCamera.transform.position;
        float depth = Vector3.Dot(camToLayer, worldCamera.transform.forward);
        screen.z = depth;
        Vector3 world = worldCamera.ScreenToWorldPoint(screen);
        return world;
    }

    bool IsMouseOverBackground2D(Vector3 mouseWorld)
    {
        Bounds bg = background.bounds;
        bool insideX = mouseWorld.x >= bg.min.x && mouseWorld.x <= bg.max.x;
        bool insideY = mouseWorld.y >= bg.min.y && mouseWorld.y <= bg.max.y;
        return insideX && insideY;
    }

    Vector3 ClampLayerToBackground(Vector3 desiredCenter)
    {
        Bounds bg = background.bounds;
        Vector3 layerExtents = layer.bounds.extents;

        float minX = bg.max.x - layerExtents.x;
        float maxX = bg.min.x + layerExtents.x;
        float minY = bg.max.y - layerExtents.y;
        float maxY = bg.min.y + layerExtents.y;

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
