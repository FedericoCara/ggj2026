using TMPro;
using UnityEngine;

public class Target : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public TMP_Text text;
    public Color lightColor = Color.white;
    public Color darkColor = Color.black;
    
    public void SetTarget(Color color, string order)
    {
        spriteRenderer.color = color;
        text.text = order;
        text.color = GetContrastingTextColor(color);
    }

    Color GetContrastingTextColor(Color backgroundColor)
    {
        float luminance = 0.2126f * backgroundColor.r +
                          0.7152f * backgroundColor.g +
                          0.0722f * backgroundColor.b;
        return luminance > 0.5f ? darkColor : lightColor;
    }
}
