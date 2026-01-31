using System.Collections.Generic;
using UnityEngine;

public class MoveLayerClickSwitch : MonoBehaviour
{
    public MoveLayerToggleGroupSpawner toggleGroup;
    public int layerIndex;

    static readonly List<MoveLayerClickSwitch> instances = new();

    struct GroupFrameCache
    {
        public int frame;
        public int activeIndex;
        public bool activeUnderCursor;
        public Vector3 mouseWorld;
    }

    static readonly Dictionary<MoveLayerToggleGroupSpawner, GroupFrameCache> groupCache = new();

    SpriteRenderer targetRenderer;
    SpriteMask targetMask;
    Camera worldCamera;

    void OnEnable()
    {
        instances.Add(this);
    }

    void OnDisable()
    {
        instances.Remove(this);
    }

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
            GroupFrameCache cache = GetOrBuildCache();
            if (cache.activeUnderCursor && cache.activeIndex != layerIndex)
            {
                return;
            }

            if (IsMouseOverTarget(cache.mouseWorld))
            {
                if (cache.activeIndex == layerIndex)
                {
                    return;
                }
                Debug.Log("[MoveLayerClickSwitch] Click detected. Switching to layer index " + layerIndex + ".");
                // Notify the toggle group so it can activate the corresponding layer.
                toggleGroup.ActivateLayer(layerIndex);
            }
        }
    }

    GroupFrameCache GetOrBuildCache()
    {
        if (toggleGroup == null)
        {
            return default;
        }

        if (groupCache.TryGetValue(toggleGroup, out GroupFrameCache cache) &&
            cache.frame == Time.frameCount)
        {
            return cache;
        }

        cache = new GroupFrameCache
        {
            frame = Time.frameCount,
            activeIndex = toggleGroup.GetActiveLayerIndex(),
            mouseWorld = GetMouseWorldPosition(),
            activeUnderCursor = false
        };

        for (int i = 0; i < instances.Count; i++)
        {
            MoveLayerClickSwitch instance = instances[i];
            if (instance == null || instance.toggleGroup != toggleGroup)
            {
                continue;
            }

            if (instance.layerIndex == cache.activeIndex &&
                instance.IsMouseOverTarget(cache.mouseWorld))
            {
                cache.activeUnderCursor = true;
                break;
            }
        }

        groupCache[toggleGroup] = cache;
        return cache;
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
