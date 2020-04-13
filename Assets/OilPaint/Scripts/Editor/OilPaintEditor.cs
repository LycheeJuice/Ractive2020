///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Oil paint - Image Effect.
// Copyright (c) Ibuprogames. All rights reserved.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Do not activate. Only for promotional videos.
//#define ENABLE_DEMO

using UnityEngine;
using UnityEditor;

namespace OilPaint
{
  /// <summary>
  /// Oil Paint Editor Base.
  /// </summary>
  [CustomEditor(typeof(OilPaint))]
  public sealed class OilPaintEditor : Editor
  {
    /// <summary>
    /// Desc.
    /// </summary>
    private readonly string oilPaintDesc = "A painterly effect based on Anisotropic Kuwahara filter.";

    /// <summary>
    /// Help text.
    /// </summary>
    public string Help { get; set; }

    /// <summary>
    /// Warnings.
    /// </summary>
    public string Warnings { get; set; }

    /// <summary>
    /// Errors.
    /// </summary>
    public string Errors { get; set; }

    /// <summary>
    /// OnInspectorGUI.
    /// </summary>
    public override void OnInspectorGUI()
    {
      EditorGUIUtility.LookLikeControls();
      EditorGUI.indentLevel = 0;

      EditorGUIUtility.labelWidth = 100.0f;

      EditorGUILayout.BeginVertical();
      {
        EditorGUILayout.Separator();

#if (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6)
        if (EditorGUIUtility.isProSkin == true)
#endif
        {
          OilPaint thisTarget = (OilPaint)target;

          EditorGUILayout.BeginVertical();
          {
            thisTarget.amount = EditorGUILayout.IntSlider(new GUIContent(@"Amount", "The strength of the effect.\nFrom 0 (no effect) to 100 (full effect)."), Mathf.RoundToInt(thisTarget.amount * 100.0f), 0, 100) * 0.01f;

            thisTarget.Mode = (OilPaintMode)EditorGUILayout.EnumPopup("Mode", thisTarget.Mode);

            if (thisTarget.Mode == OilPaintMode.Custom)
            {
              EditorGUI.indentLevel++;

              thisTarget.customStrength = EditorGUILayout.IntSlider(@"Strength", thisTarget.customStrength, 1, 10);

              EditorGUI.indentLevel--;
            }
#if ENABLE_DEMO
            thisTarget.showGUI = EditorGUILayout.Toggle("Show GUI", thisTarget.showGUI);

            thisTarget.musicClip = EditorGUILayout.ObjectField("Music", thisTarget.musicClip, typeof(AudioClip)) as AudioClip;
#endif
          }
          EditorGUILayout.EndVertical();

          Help += oilPaintDesc;

          if (thisTarget.Mode == OilPaintMode.Custom && thisTarget.customStrength > 6)
            Warnings += "Notice that values greater than 6 are very resource intensive.";
        }
#if (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6)
        else
        {
          this.Help = string.Empty;
          this.Errors += "'Oil Paint - Image Effect' require Unity Pro version!";
        }
#endif

        if (string.IsNullOrEmpty(Warnings) == false)
        {
          EditorGUILayout.HelpBox(Warnings, MessageType.Warning);

          EditorGUILayout.Separator();
        }

        if (string.IsNullOrEmpty(Errors) == false)
        {
          EditorGUILayout.HelpBox(Errors, MessageType.Error);

          EditorGUILayout.Separator();
        }

        if (string.IsNullOrEmpty(Help) == false)
          EditorGUILayout.HelpBox(Help, MessageType.Info);
      }
      EditorGUILayout.EndVertical();

      if (GUI.changed == true)
        EditorUtility.SetDirty(target);

      EditorGUIUtility.LookLikeControls();

      Help = Warnings = Errors = string.Empty;
    }
  }
}