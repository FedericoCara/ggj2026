using System.Collections.Generic;
using UnityEngine;

public class IndicatorPost : MonoBehaviour
{
    public List<SpriteRenderer> arrowSprites = new List<SpriteRenderer>();

    public void SetTargets(List<Vector3> positions, List<Color> colors)
    {
        if (positions == null || colors == null)
        {
            Debug.LogWarning("[IndicatorPost] Missing targets data.");
            return;
        }

        int count = Mathf.Min(arrowSprites.Count, positions.Count, colors.Count);
        for (int i = 0; i < arrowSprites.Count; i++)
        {
            SpriteRenderer arrow = arrowSprites[i];
            if (arrow == null)
            {
                continue;
            }

            if (i < count)
            {
                arrow.enabled = true;
                arrow.color = colors[i];
                RotateArrowToTarget(arrow.transform, positions[i]);
            }
            else
            {
                arrow.enabled = false;
            }
        }
    }

    void RotateArrowToTarget(Transform arrowTransform, Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - arrowTransform.position;
        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        direction.z = 0f;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrowTransform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}
