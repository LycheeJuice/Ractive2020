///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Oil paint - Image Effect.
// Copyright (c) Ibuprogames. All rights reserved.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Do not activate. Only for promotional videos.
//#define ENABLE_DEMO

// If you want to use another value of RADIUS, enabling this can generate the code needed for the shader.
//#define OUTPUT_SAMPLE_SHADER_CODE

using System;

using UnityEngine;

namespace OilPaint
{
  /// <summary>
  /// Modes.
  /// </summary>
  public enum OilPaintMode
  {
    Low,
    Medium,
    High,
    Custom,
  }

  /// <summary>
  /// Oil paint.
  /// </summary>
  [ExecuteInEditMode]
  [RequireComponent(typeof(Camera))]
  [AddComponentMenu("Image Effects/Oil Paint")]
  public sealed class OilPaint : MonoBehaviour
  {
    /// <summary>
    /// Amount of the effect (0 none, 1 full).
    /// </summary>
    public float amount = 1.0f;

    /// <summary>
    /// Strength of the effect in custom mode.
    /// </summary>
    public int customStrength;

    [SerializeField]
    private OilPaintMode oilPaintMode;

    private Shader[] shaders = new Shader[4];

    private Material material;

#if ENABLE_DEMO
    public AudioClip musicClip = null;

    public bool showGUI = true;

    private float timeToChange = 0.0f;
    private float timeMode = 0.0f;

    private GUIStyle effectNameStyle;
#endif

    /// <summary>
    /// Effect mode.
    /// </summary>
    public OilPaintMode Mode
    {
      get { return oilPaintMode; }
      set
      {
        if (oilPaintMode != value)
        {
          oilPaintMode = value;

          CreateMaterial();
        }
      }
    }

    private void Awake()
    {
      shaders[(int)OilPaintMode.Low] = Resources.Load<Shader>(@"Shaders/OilPaintLow");
      shaders[(int)OilPaintMode.Medium] = Resources.Load<Shader>(@"Shaders/OilPaintMedium");
      shaders[(int)OilPaintMode.High] = Resources.Load<Shader>(@"Shaders/OilPaintHigh");
      shaders[(int)OilPaintMode.Custom] = Resources.Load<Shader>(@"Shaders/OilPaintCustom");

      if (shaders[(int)OilPaintMode.Low] == null ||
          shaders[(int)OilPaintMode.Medium] == null ||
          shaders[(int)OilPaintMode.High] == null ||
          shaders[(int)OilPaintMode.Custom] == null)
      {
        Debug.LogError("Shaders not found. 'Oil Paint' disabled.");

        this.enabled = false;
      }
    }

    /// <summary>
    /// Check.
    /// </summary>
    private void OnEnable()
    {
      if (SystemInfo.supportsImageEffects == false)
      {
        Debug.LogError("Hardware not support Image Effects. 'Oil Paint' disabled.");
      
        this.enabled = false;
      }
      else
      {
        for (int i = 0; i < (int)OilPaintMode.Custom; ++i)
        {
          if (shaders[i] == null)
            Debug.LogWarning(string.Format("'{0} {1}' shader null.", this.GetType().ToString(), ((OilPaintMode)i).ToString()));
          else if (shaders[i].isSupported == false)
            Debug.LogWarning(string.Format("'{0} {1}' shader not supported.", this.GetType().ToString(), ((OilPaintMode)i).ToString()));
        }

        CreateMaterial();

#if ENABLE_DEMO
        if (musicClip != null && Application.isPlaying == true)
        {
          AudioSource audioSource = this.gameObject.GetComponent<AudioSource>();
          audioSource.clip = musicClip;
          audioSource.loop = Application.isWebPlayer;
          audioSource.Play();
          
          timeToChange = musicClip.length / 12.0f;
          timeMode = timeToChange;
        }
#endif
        if (material == null)
          this.enabled = false;
      }
    }

    /// <summary>
    /// Destroy the material.
    /// </summary>
    private void OnDisable()
    {
      if (material != null)
        DestroyImmediate(material);
    }

#if ENABLE_DEMO
    private void Update()
    {
      if (Application.isPlaying == false)
        return;

      if (Application.isWebPlayer == false && (Input.GetKeyUp(KeyCode.Escape) == true ||
        (musicClip != null && this.gameObject.GetComponent<AudioSource>().time > Time.realtimeSinceStartup)))
        Application.Quit();

      if (Input.GetKeyUp(KeyCode.Alpha1) == true || Input.GetKeyUp(KeyCode.Keypad1) == true)
      {
        Mode = OilPaintMode.Low;
        timeToChange = 0.0f;
      }
      else if (Input.GetKeyUp(KeyCode.Alpha2) == true || Input.GetKeyUp(KeyCode.Keypad2) == true)
      {
        Mode = OilPaintMode.Medium;
        timeToChange = 0.0f;
      }
      else if (Input.GetKeyUp(KeyCode.Alpha3) == true || Input.GetKeyUp(KeyCode.Keypad3) == true)
      {
        Mode = OilPaintMode.High;
        timeToChange = 0.0f;
      }
      else if (Input.GetKeyUp(KeyCode.Alpha4) == true || Input.GetKeyUp(KeyCode.Keypad4) == true)
      {
        Mode = OilPaintMode.Custom;
        timeToChange = 0.0f;
      }

      if (timeToChange > 0.0f)
      {
        timeMode -= Time.deltaTime;
        if (timeMode <= 0.0f)
        {
          Mode = (Mode == OilPaintMode.Low ? Mode = OilPaintMode.Medium : (Mode == OilPaintMode.Medium ? Mode = OilPaintMode.High : Mode = OilPaintMode.Low));

          timeMode = timeToChange;
        }
      }
    }

    private void OnGUI()
    {
      if (effectNameStyle == null)
      {
        effectNameStyle = new GUIStyle(GUI.skin.textArea);
        effectNameStyle.alignment = TextAnchor.MiddleCenter;
        effectNameStyle.fontSize = 22;
      }

      if (showGUI == true && amount > 0.1f)
      {
        GUILayout.BeginArea(new Rect(20.0f, 20.0f, 160.0f, oilPaintMode == OilPaintMode.Custom ? 90.0f : 30.0f));
        {
          //GUILayout.Label("OIL PAINT", effectNameStyle);
          GUILayout.Label(oilPaintMode.ToString().ToUpper(), effectNameStyle);

          if (oilPaintMode == OilPaintMode.Custom)
            customStrength = (int)GUILayout.HorizontalSlider(customStrength, 1.0f, 10.0f);
        }
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(Screen.width - 180.0f, 20.0f, 160.0f, 30.0f), "NORMAL", effectNameStyle);
        GUILayout.EndArea();
      }
    }
#endif

    /// <summary>
    /// Creates the material.
    /// </summary>
    private void CreateMaterial()
    {
      if (shaders[(int)oilPaintMode] != null)
      {
        if (material != null)
        {
          if (Application.isEditor == true)
            DestroyImmediate(material);
          else
            Destroy(material);
        }

        material = new Material(shaders[(int)oilPaintMode]);

        if (material == null)
          Debug.LogWarning(string.Format("'{0}' material null.", this.name));

        Debug.Log("Set 'Oil Paint' shader to '" + oilPaintMode.ToString() + "'.");

#if OUTPUT_SAMPLE_SHADER_CODE
        string output = string.Empty;
        const int RADIUS = 6;

        for (int u = -RADIUS; u <= 0; ++u)
          for (int v = -RADIUS; v <= 0; ++v)
            output += "SAMPLE(" + u + "," + v + ", 0)\n";
        output += "\n";

        for (int u = -RADIUS; u <= 0; ++u)
          for (int v = 0; v <= RADIUS; ++v)
            output += "SAMPLE(" + u + "," + v + ", 1)\n";
        output += "\n";

        for (int u = 0; u <= RADIUS; ++u)
          for (int v = 0; v <= RADIUS; ++v)
            output += "SAMPLE(" + u + "," + v + ", 2)\n";
        output += "\n";

        for (int u = 0; u <= RADIUS; ++u)
          for (int v = -RADIUS; v <= 0; ++v)
            output += "SAMPLE(" + u + "," + v + ", 3)\n";

        Debug.Log(output);
#endif
      }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (material != null)
      {
        if (oilPaintMode == OilPaintMode.Custom)
          material.SetInt("_Radius", customStrength);

        material.SetFloat(@"_Amount", amount);
        material.SetVector("_InvScreenSize", new Vector4(1.0f / Screen.width, 1.0f / Screen.height, 0.0f, 0.0f));
        
        Graphics.Blit(source, destination, material, QualitySettings.activeColorSpace == ColorSpace.Linear ? 1 : 0);
      }
    }
  }
}
