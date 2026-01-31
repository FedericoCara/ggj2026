using UnityEngine;

public class MoveLayerClickSwitch : MonoBehaviour
{
    public MoveLayerToggleGroupSpawner toggleGroup;
    public int layerIndex;

    SpriteRenderer targetRenderer;
    SpriteMask targetMask;
    Camera worldCamera;

    void Awake()
    {
        targetRenderer = GetComponent<SpriteRenderer>();
        targetMask = GetComponent<SpriteMask>();
        worldCamera = Camera.main;
    }

    void Update()
    {
        if (toggleGroup == null || worldCamera == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Convert mouse position to world space to test the target bounds.
            Vector3 mouseWorld = GetMouseWorldPosition();
            if (IsMouseOverTarget(mouseWorld))
            {
                Debug.Log("[MoveLayerClickSwitch] Click detected. Switching to layer index " + layerIndex + ".");
                // Notify the toggle group so it can activate the corresponding layer.
                toggleGroup.ActivateLayer(layerIndex);
            }
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        // Project the mouse position onto the target's depth plane.
        Vector3 screen = Input.mousePosition;
        Vector3 camToTarget = transform.position - worldCamera.transform.position;
        float depth = Vector3.Dot(camToTarget, worldCamera.transform.forward);
        screen.z = depth;
        return worldCamera.ScreenToWorldPoint(screen);
    }

    bool IsMouseOverTarget(Vector3 mouseWorld)
    {
        // Check if the mouse world position is inside the target bounds.
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
            // SpriteMask bounds are in local sprite space; scale to world size.
            Vector3 size = Vector3.Scale(targetMask.sprite.bounds.size, transform.lossyScale);
            return new Bounds(transform.position, size);
        }

        return new Bounds(transform.position, Vector3.zero);
    }
}
