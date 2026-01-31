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

        Debug.Log("[MoveLayer] Awake. background=" + background + " layer=" + layer + " camera=" + worldCamera);
    }

    void Update()
    {
        if (background == null || layer == null || worldCamera == null)
        {
            Debug.LogWarning("[MoveLayer] Missing refs. background=" + background + " layer=" + layer + " camera=" + worldCamera);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = GetMouseWorldPosition();
            Debug.Log("[MoveLayer] MouseDown at " + mouseWorld);
            if (IsMouseOverBackground2D(mouseWorld))
            {
                isDragging = true;
                layerZ = layer.transform.position.z;
                dragOffset = layer.transform.position - mouseWorld;
                Debug.Log("[MoveLayer] Drag start. offset=" + dragOffset + " layerZ=" + layerZ +
                    " bgBounds=" + background.bounds + " layerBounds=" + layer.bounds);
            }
            else
            {
                Debug.Log("[MoveLayer] MouseDown outside background bounds. bg=" + background.bounds);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            Debug.Log("[MoveLayer] Drag end.");
        }

        if (isDragging)
        {
            Vector3 mouseWorld = GetMouseWorldPosition();
            Vector3 desired = mouseWorld + dragOffset;
            Vector3 clamped = ClampLayerToBackground(desired);
            clamped.z = layerZ;
            layer.transform.position = clamped;
            Debug.Log("[MoveLayer] Dragging. mouse=" + mouseWorld + " desired=" + desired +
                " clamped=" + clamped + " layerPos=" + layer.transform.position);
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 screen = Input.mousePosition;
        Vector3 camToLayer = layer.transform.position - worldCamera.transform.position;
        float depth = Vector3.Dot(camToLayer, worldCamera.transform.forward);
        screen.z = depth;
        Vector3 world = worldCamera.ScreenToWorldPoint(screen);
        Debug.Log("[MoveLayer] MouseWorld calc. screen=" + screen + " world=" + world +
            " camPos=" + worldCamera.transform.position + " camForward=" + worldCamera.transform.forward);
        return world;
    }

    bool IsMouseOverBackground2D(Vector3 mouseWorld)
    {
        Bounds bg = background.bounds;
        bool insideX = mouseWorld.x >= bg.min.x && mouseWorld.x <= bg.max.x;
        bool insideY = mouseWorld.y >= bg.min.y && mouseWorld.y <= bg.max.y;
        Debug.Log("[MoveLayer] MouseOver2D check. insideX=" + insideX + " insideY=" + insideY +
            " mouse=" + mouseWorld + " bgMin=" + bg.min + " bgMax=" + bg.max);
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
        Debug.Log("[MoveLayer] Clamp ranges. minX=" + minX + " maxX=" + maxX + " minY=" + minY + " maxY=" + maxY);

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
