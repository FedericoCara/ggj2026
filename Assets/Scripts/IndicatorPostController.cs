using System.Collections.Generic;
using UnityEngine;

public class IndicatorPostController : MonoBehaviour
{
    private List<IndicatorPost> indicatorPosts = new List<IndicatorPost>();

    void Awake()
    {
        indicatorPosts.Clear();
        IndicatorPost[] foundPosts = FindObjectsOfType<IndicatorPost>();
        indicatorPosts.AddRange(foundPosts);

        if (indicatorPosts.Count == 0)
        {
            Debug.LogWarning("[IndicatorPostController] No IndicatorPost found in scene.");
        }
    }

    public void SetTargets(List<Target> targets)
    {
        if (targets == null || targets.Count == 0)
        {
            Debug.LogWarning("[IndicatorPostController] No targets provided.");
            return;
        }

        List<Target> orderedTargets = new List<Target>(targets);
        orderedTargets.RemoveAll(target => target == null);
        orderedTargets.Sort((a, b) => b.transform.position.y.CompareTo(a.transform.position.y));

        List<Vector3> positions = new List<Vector3>(orderedTargets.Count);
        List<Color> colors = new List<Color>(orderedTargets.Count);
        for (int i = 0; i < orderedTargets.Count; i++)
        {
            Target target = orderedTargets[i];
            positions.Add(target.transform.position);
            colors.Add(target.assignedColor);
        }

        for (int i = 0; i < indicatorPosts.Count; i++)
        {
            IndicatorPost post = indicatorPosts[i];
            if (post != null)
            {
                post.SetTargets(positions, colors);
            }
        }
    }
}
