using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraAspectSize : MonoBehaviour
{
    [Header("Reference (16:9 = 1920x1080)")]
    [SerializeField] private Vector2 referenceResolution = new Vector2(1920f, 1080f);
    [SerializeField] private float referenceOrthoSize = 6f;

    [Header("Fit Mode")]
    [SerializeField] private bool fitWidth = true;

    private Camera cachedCamera;
    private float lastAspect = -1f;

    private void Awake()
    {
        cachedCamera = GetComponent<Camera>();
        ApplySize();
    }

    private void OnValidate()
    {
        if (cachedCamera == null)
        {
            cachedCamera = GetComponent<Camera>();
        }

        ApplySize();
    }

    private void Update()
    {
        if (cachedCamera == null)
        {
            cachedCamera = GetComponent<Camera>();
        }

        if (!Mathf.Approximately(lastAspect, cachedCamera.aspect))
        {
            ApplySize();
        }
    }

    private void ApplySize()
    {
        if (cachedCamera == null || !cachedCamera.orthographic)
        {
            return;
        }

        float referenceAspect = Mathf.Max(1f, referenceResolution.x) / Mathf.Max(1f, referenceResolution.y);
        float currentAspect = Mathf.Max(0.01f, cachedCamera.aspect);

        if (fitWidth)
        {
            cachedCamera.orthographicSize = referenceOrthoSize * (referenceAspect / currentAspect);
        }
        else
        {
            cachedCamera.orthographicSize = referenceOrthoSize;
        }

        lastAspect = cachedCamera.aspect;
    }
}
