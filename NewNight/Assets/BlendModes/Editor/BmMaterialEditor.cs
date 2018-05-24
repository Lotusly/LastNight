// PackageExporter: SkipWrap

using UnityEngine;
using UnityEditor;
using System;

public class BmMaterialEditor : MaterialEditor
{
    private BlendModes.BlendMode selectedBlendMode;

    public override void OnEnable ()
    {
        base.OnEnable();

        var currentBlendMode = BlendModes.BlendMode.Normal;

        foreach (var keyword in ((Material)target).shaderKeywords)
        {
            if (keyword.StartsWith("Bm"))
            {
                currentBlendMode = (BlendModes.BlendMode)Enum.Parse(typeof(BlendModes.BlendMode), keyword.Replace("Bm", string.Empty), true);
                break;
            }
        }
        selectedBlendMode = currentBlendMode;
    }

    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI();
        if (!isVisible) return;

        var targetMaterial = target as Material;

        EditorGUI.BeginChangeCheck();
        selectedBlendMode = (BlendModes.BlendMode)EditorGUILayout.EnumPopup("Blend Mode", selectedBlendMode);
        if (EditorGUI.EndChangeCheck())
        {
            for (int i = 0; i < targetMaterial.shaderKeywords.Length; i++)
                if (targetMaterial.shaderKeywords[i].StartsWith("Bm"))
                    targetMaterial.DisableKeyword(targetMaterial.shaderKeywords[i]);
            targetMaterial.EnableKeyword("Bm" + selectedBlendMode);

            EditorUtility.SetDirty(targetMaterial);
        }
    }
}
