using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Targets/Color Palette", fileName = "TargetColorPalette")]
public class TargetColorPalette : ScriptableObject
{
    public List<Color> colors = new List<Color>();
}
