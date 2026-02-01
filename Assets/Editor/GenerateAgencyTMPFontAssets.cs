using UnityEditor;
using UnityEngine;
using TMPro;

public static class GenerateAgencyTMPFontAssets
{
    const string RegularFontPath = "Assets/Fonts/Agency FB/agencyfb_reg.ttf";
    const string BoldFontPath = "Assets/Fonts/Agency FB/agencyfb_bold.ttf";
    const string OutputFolder = "Assets/Fonts/Agency FB";

    [MenuItem("Tools/TextMeshPro/Generate Agency Font Assets")]
    static void Generate()
    {
        GenerateForFont(RegularFontPath, "AgencyFB-Regular TMP.asset");
        GenerateForFont(BoldFontPath, "AgencyFB-Bold TMP.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void GenerateForFont(string fontPath, string assetName)
    {
        Font font = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
        if (font == null)
        {
            Debug.LogError($"[GenerateAgencyTMPFontAssets] Font not found at: {fontPath}");
            return;
        }

        string outputPath = $"{OutputFolder}/{assetName}";
        TMP_FontAsset existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(outputPath);
        if (existing != null)
        {
            Debug.Log($"[GenerateAgencyTMPFontAssets] Font asset already exists: {outputPath}");
            return;
        }

        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(font);
        AssetDatabase.CreateAsset(fontAsset, outputPath);
        Debug.Log($"[GenerateAgencyTMPFontAssets] Created: {outputPath}");
    }
}
