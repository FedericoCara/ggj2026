using System.Collections.Generic;
using UnityEngine;

public class MoveLayerClickSwitch : MonoBehaviour
{
    public MoveLayerToggleGroupSpawner toggleGroup;
    public int layerIndex;
    public Renderer previewMesh;
    public Color previewColor = new(1f, 0.5f, 0f, 1f);
    public Color selectedColor = new(0.5f, 0f, 1f, 1f);

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
    int lastActiveIndex = int.MinValue;
    MaterialPropertyBlock previewBlock;
    static readonly int colorPropertyId = Shader.PropertyToID("_Color");
    static readonly int baseColorPropertyId = Shader.PropertyToID("_BaseColor");

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
        previewBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (toggleGroup == null || worldCamera == null)
        {
            return;
        }

        SyncPreviewColor();

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
                // Notify the toggle group so it can activate the corresponding layer.
                toggleGroup.ActivateLayer(layerIndex);
            }
        }
    }

    void SyncPreviewColor()
    {
        int activeIndex = toggleGroup.GetActiveLayerIndex();
        if (activeIndex == lastActiveIndex)
        {
            return;
        }

        lastActiveIndex = activeIndex;
        if (previewMesh == null)
        {
            return;
        }

        Color targetColor = activeIndex == layerIndex ? selectedColor : previewColor;
        if (previewMesh is SpriteRenderer spriteRenderer)
        {
            spriteRenderer.color = targetColor;
            return;
        }

        previewMesh.GetPropertyBlock(previewBlock);
        if (previewMesh.sharedMaterial != null &&
            previewMesh.sharedMaterial.HasProperty(baseColorPropertyId))
        {
            previewBlock.SetColor(baseColorPropertyId, targetColor);
        }
        else
        {
            previewBlock.SetColor(colorPropertyId, targetColor);
        }
        previewMesh.SetPropertyBlock(previewBlock);
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
