using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(UltimateBloom))]
public class UltimateBloomEditor : Editor 
{

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.hideFlags = HideFlags.HideAndDontSave;
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    GUIStyle m_Background1;
    GUIStyle m_Background2;
    GUIStyle m_Background3;

    Texture2D m_Logo;

    void OnEnable()
    {
        m_Background1 = new GUIStyle();
        m_Background1.normal.background = MakeTex(600, 1, new Color(1.0f, 1.0f, 1.0f, 0.1f));
        m_Background2 = new GUIStyle();
        m_Background2.normal.background = MakeTex(600, 1, new Color(1.0f, 1.0f, 1.0f, 0.0f));
        m_Background3 = new GUIStyle();
        m_Background3.normal.background = MakeTex(600, 1, new Color(1.0f, 1.0f, 1.0f, 0.05f));


        MonoScript script = MonoScript.FromScriptableObject(this);
        string path = AssetDatabase.GetAssetPath(script);
        string logoPath = Path.GetDirectoryName(path) + "/ub_logo.png";
        m_Logo = (Texture2D)AssetDatabase.LoadAssetAtPath(logoPath, typeof(Texture2D));

        if (m_Logo != null)
            m_Logo.hideFlags = HideFlags.HideAndDontSave;

        /*m_Logo = (Texture2D)Resources.LoadAssetAtPath("Assets/Paroxe/UltimateBloom/Editor/ub_logo.png", typeof(Texture2D));
        if (m_Logo != null)
            m_Logo.hideFlags = HideFlags.HideAndDontSave;*/
    }

    void OnDisable()
    {
        GameObject.DestroyImmediate(m_Background1.normal.background);
        GameObject.DestroyImmediate(m_Background2.normal.background);
        GameObject.DestroyImmediate(m_Background3.normal.background);
    }


    public override void OnInspectorGUI()
    {
        UltimateBloom bloomDeluxe = (UltimateBloom)target;
        Undo.RecordObject(bloomDeluxe, "Bloom DELUXE");

        if (m_Logo != null)
        {
            Rect rect = GUILayoutUtility.GetRect(m_Logo.width, m_Logo.height);
            GUI.DrawTexture(rect, m_Logo, ScaleMode.ScaleToFit);
        }

        //GUILayout.BeginVertical(m_Background1);
        GUILayout.BeginVertical("Box");
        GUILayout.Label("Bloom Settings", EditorStyles.boldLabel);
        bloomDeluxe.m_HDR = (UltimateBloom.HDRBloomMode)EditorGUILayout.EnumPopup("HDR", bloomDeluxe.m_HDR);
        bloomDeluxe.m_InvertImage = EditorGUILayout.Toggle("Flip Image", bloomDeluxe.m_InvertImage);

        // TODO: Screen blending
        /*
        UltimateBloom.BlendMode lastBlendMode = bloomDeluxe.m_BlendMode;
        bloomDeluxe.m_BlendMode = (UltimateBloom.BlendMode)EditorGUILayout.EnumPopup("Blend Mode", bloomDeluxe.m_BlendMode);
        if (lastBlendMode != bloomDeluxe.m_BlendMode)
            bloomDeluxe.ForceShadersReload();
        if (bloomDeluxe.m_BlendMode == UltimateBloom.BlendMode.SCREEN)
            bloomDeluxe.m_ScreenMaxIntensity = Mathf.Clamp(EditorGUILayout.FloatField("  Max Screen Intensity (>1 if HDR)", bloomDeluxe.m_ScreenMaxIntensity), 1, 100.0f);*/

        //bloomDeluxe.m_DownsamplingQuality = (BloomDeluxe.BloomSamplingQuality)EditorGUILayout.EnumPopup("Downsampling Quality", bloomDeluxe.m_DownsamplingQuality);
        

        bloomDeluxe.m_BloomIntensity = DoSlider("Bloom Master Intensity", bloomDeluxe.m_BloomIntensity, 0.0f, 5.0f);
        //bloomDeluxe.m_BloomIntensity = Mathf.Clamp(EditorGUILayout.FloatField("Bloom Master Intensity", bloomDeluxe.m_BloomIntensity), 0.0f, 100.0f);
        bloomDeluxe.m_DownscaleCount = Mathf.Clamp(EditorGUILayout.IntField("Layers (Downscale Count)", bloomDeluxe.m_DownscaleCount), 1, 6);
        if (GUILayout.Button((bloomDeluxe.m_UiShowBloomScales ? "Hide Layers" : "Show Layers") + "[" + bloomDeluxe.m_DownscaleCount + "]"))
            bloomDeluxe.m_UiShowBloomScales = !bloomDeluxe.m_UiShowBloomScales;
        
        if (bloomDeluxe.m_UiShowBloomScales)
        {
            
            for (int i = 0; i < bloomDeluxe.m_DownscaleCount; ++i)
            {
                GUILayout.BeginVertical(i % 2 == 0 ? m_Background3 : m_Background1);
                int idx = i + 1;
                bloomDeluxe.m_BloomUsages[i] = EditorGUILayout.Toggle("  Layer " + idx + " Enabled", bloomDeluxe.m_BloomUsages[i]);
                bloomDeluxe.m_BloomIntensities[i] = DoSlider("  Layer " + idx + " Intensity", bloomDeluxe.m_BloomIntensities[i], 0.0f, 5.0f);
                bloomDeluxe.m_BloomColors[i] = EditorGUILayout.ColorField("  Layer " + idx + " Tint", bloomDeluxe.m_BloomColors[i]);
                GUILayout.EndVertical();
            }
        }
        GUILayout.Space(10.0f);
        GUILayout.EndVertical();

        // Sampling
        GUILayout.BeginVertical("Box");
        GUILayout.Label("Sampling", EditorStyles.boldLabel);
        bloomDeluxe.m_TemporalStableDownsampling = EditorGUILayout.Toggle("Temporal Stability Filter", bloomDeluxe.m_TemporalStableDownsampling);
        bloomDeluxe.m_SamplingMode = (UltimateBloom.SamplingMode)EditorGUILayout.EnumPopup("Sampling Mode", bloomDeluxe.m_SamplingMode);
        if (bloomDeluxe.m_SamplingMode == UltimateBloom.SamplingMode.Fixed)
            bloomDeluxe.m_UpsamplingQuality = (UltimateBloom.BloomSamplingQuality)EditorGUILayout.EnumPopup("Sampling Kernel Size", bloomDeluxe.m_UpsamplingQuality);
        else // Screen relative sampling
        {
            bloomDeluxe.m_SamplingMinHeight = DoSlider("Min Height", bloomDeluxe.m_SamplingMinHeight, 300.0f, 1000.0f);

            if (GUILayout.Button((bloomDeluxe.m_UiShowHeightSampling ? "Hide Sampling Heights" : "Show Sampling Heights")))
                bloomDeluxe.m_UiShowHeightSampling = !bloomDeluxe.m_UiShowHeightSampling;

            if (bloomDeluxe.m_UiShowHeightSampling)
            {
                bloomDeluxe.ComputeResolutionRelativeData();
                for (int i = 0; i < bloomDeluxe.m_ResSamplingPixelCount.Length; ++i)
                {
                    GUILayout.Label("Sampling Height[" + i + "] = " + bloomDeluxe.m_ResSamplingPixelCount[i]);
                }
            }
        }

        GUILayout.Space(10.0f);
        GUILayout.EndVertical();


        // Intensity Management
        GUILayout.BeginVertical("Box");
        GUILayout.Label("Intensity Settings", EditorStyles.boldLabel);
        UltimateBloom.BloomIntensityManagement lastIm = bloomDeluxe.m_IntensityManagement;
        bloomDeluxe.m_IntensityManagement = (UltimateBloom.BloomIntensityManagement)EditorGUILayout.EnumPopup("Intensity Function", bloomDeluxe.m_IntensityManagement);
        if (bloomDeluxe.m_IntensityManagement == UltimateBloom.BloomIntensityManagement.Threshold)
        {
            bloomDeluxe.m_BloomThreshhold = DoSlider("  Threshold", bloomDeluxe.m_BloomThreshhold, 0.0f, 5.0f);
        }
        else if (bloomDeluxe.m_IntensityManagement == UltimateBloom.BloomIntensityManagement.FilmicCurve)
        {
            bloomDeluxe.m_BloomCurve.OnGUI();
        }
        if (lastIm != bloomDeluxe.m_IntensityManagement)
            bloomDeluxe.ForceShadersReload();
        GUILayout.Space(10.0f);
        GUILayout.EndVertical();
        
        // Optimizations
        GUILayout.BeginVertical("Box");
        GUILayout.Label("Optimizations", EditorStyles.boldLabel);
        bloomDeluxe.m_DirectDownSample = EditorGUILayout.Toggle("  Direct Downsampling", bloomDeluxe.m_DirectDownSample);
        if (bloomDeluxe.m_DirectDownSample)
            EditorGUILayout.HelpBox("Enabling direct downsampling may introduce jittering. It should only be enabled on low end hardwares.", MessageType.Info);
        bloomDeluxe.m_DirectUpsample = EditorGUILayout.Toggle("  Direct Upsampling", bloomDeluxe.m_DirectUpsample);
        GUILayout.Space(10.0f);
        GUILayout.EndVertical();
       
        //bloomDeluxe.m_UseBloomTreshold = EditorGUILayout.Toggle("Use Bloom Treshold", bloomDeluxe.m_UseBloomTreshold);

       

        // LENS DUST

        GUILayout.BeginVertical("Box");
        GUILayout.Label("Lens Dirt", EditorStyles.boldLabel);
        bool lastUseLensDust = bloomDeluxe.m_UseLensDust;
        bloomDeluxe.m_UseLensDust = EditorGUILayout.Toggle("Use Lens Dirt", bloomDeluxe.m_UseLensDust);
        if (bloomDeluxe.m_UseLensDust)
        {
            bloomDeluxe.m_DustTexture = (Texture2D)EditorGUILayout.ObjectField("  Dirt Texture", bloomDeluxe.m_DustTexture, typeof(Texture2D), false);
            bloomDeluxe.m_DustIntensity = DoSlider("  Dirtiness", bloomDeluxe.m_DustIntensity, 0.0f, 10.0f);
            bloomDeluxe.m_DirtLightIntensity = DoSlider("  Dirt Light Intensity", bloomDeluxe.m_DirtLightIntensity, 0.0f, 30.0f);
        }
        if (lastUseLensDust != bloomDeluxe.m_UseLensDust)
            bloomDeluxe.ForceShadersReload();
        GUILayout.Space(10.0f);
        GUILayout.EndVertical();

        // LENS FLARE

        GUILayout.BeginVertical("Box");
        GUILayout.Label("Lens Flare (Bokeh & Ghost)", EditorStyles.boldLabel);
        bool lastUseLensFlare = bloomDeluxe.m_UseLensFlare;
        bloomDeluxe.m_UseLensFlare = EditorGUILayout.Toggle("Use Lens Flare", bloomDeluxe.m_UseLensFlare);
        if (bloomDeluxe.m_UseLensFlare)
        {
            UltimateBloom.FlarePresets preset = (UltimateBloom.FlarePresets)EditorGUILayout.EnumPopup("Flare Preset", UltimateBloom.FlarePresets.ChoosePreset);
            SetFlarePreset(preset, bloomDeluxe);

            bloomDeluxe.m_FlareRendering = (UltimateBloom.FlareRendering)EditorGUILayout.EnumPopup("  Flare Rendering", bloomDeluxe.m_FlareRendering);
            if (bloomDeluxe.m_FlareRendering != UltimateBloom.FlareRendering.Sharp)
            {
                bloomDeluxe.m_FlareBlurQuality = (UltimateBloom.FlareBlurQuality)EditorGUILayout.EnumPopup("  Blur Quality", bloomDeluxe.m_FlareBlurQuality);
            }

            UltimateBloom.FlareType lastFareType =  bloomDeluxe.m_FlareType;
            bloomDeluxe.m_FlareType = (UltimateBloom.FlareType)EditorGUILayout.EnumPopup("  Flare Duplication", bloomDeluxe.m_FlareType);
            if (lastFareType != bloomDeluxe.m_FlareType)
                bloomDeluxe.ForceShadersReload();

            bloomDeluxe.m_FlareIntensity = DoSlider("  Flare Intensity", bloomDeluxe.m_FlareIntensity, 0.0f, 30.0f);
            bloomDeluxe.m_FlareTreshold = DoSlider("  Threshold", bloomDeluxe.m_FlareTreshold, 0.0f, 5.0f);
            bloomDeluxe.m_FlareGlobalScale = DoSlider("  Global Scale", bloomDeluxe.m_FlareGlobalScale, 0.1f, 5.0f);
            bloomDeluxe.m_FlareScales = EditorGUILayout.Vector4Field("  Flare Scales Far", bloomDeluxe.m_FlareScales);
            if (bloomDeluxe.m_FlareType == UltimateBloom.FlareType.Double)
                bloomDeluxe.m_FlareScalesNear = EditorGUILayout.Vector4Field("  Flare Scales Near", bloomDeluxe.m_FlareScalesNear);
            Vector4 tmp = bloomDeluxe.m_FlareScales;
            float maxFlareScale = 12.0f;
            bloomDeluxe.m_FlareScales = new Vector4(Mathf.Clamp(tmp.x, 0, maxFlareScale), Mathf.Clamp(tmp.y, 0, maxFlareScale), Mathf.Clamp(tmp.z, 0, maxFlareScale), Mathf.Clamp(tmp.w, 0, maxFlareScale));

            tmp = bloomDeluxe.m_FlareScalesNear;
            bloomDeluxe.m_FlareScalesNear = new Vector4(Mathf.Clamp(tmp.x, 0, maxFlareScale), Mathf.Clamp(tmp.y, 0, maxFlareScale), Mathf.Clamp(tmp.z, 0, maxFlareScale), Mathf.Clamp(tmp.w, 0, maxFlareScale));

            bloomDeluxe.m_FlareTint0 = EditorGUILayout.ColorField("  Flare Tint 0", bloomDeluxe.m_FlareTint0);
            bloomDeluxe.m_FlareTint1 = EditorGUILayout.ColorField("  Flare Tint 1", bloomDeluxe.m_FlareTint1);
            bloomDeluxe.m_FlareTint2 = EditorGUILayout.ColorField("  Flare Tint 2", bloomDeluxe.m_FlareTint2);
            bloomDeluxe.m_FlareTint3 = EditorGUILayout.ColorField("  Flare Tint 3", bloomDeluxe.m_FlareTint3);

            if (bloomDeluxe.m_FlareType == UltimateBloom.FlareType.Double)
            {
                bloomDeluxe.m_FlareTint4 = EditorGUILayout.ColorField("  Flare Tint 4", bloomDeluxe.m_FlareTint4);
                bloomDeluxe.m_FlareTint5 = EditorGUILayout.ColorField("  Flare Tint 5", bloomDeluxe.m_FlareTint5);
                bloomDeluxe.m_FlareTint6 = EditorGUILayout.ColorField("  Flare Tint 6", bloomDeluxe.m_FlareTint6);
                bloomDeluxe.m_FlareTint7 = EditorGUILayout.ColorField("  Flare Tint 7", bloomDeluxe.m_FlareTint7);
            }

            bloomDeluxe.m_FlareMask = (Texture2D)EditorGUILayout.ObjectField("  Flare Mask", bloomDeluxe.m_FlareMask, typeof(Texture2D), false);

            bloomDeluxe.m_UseBokehFlare = EditorGUILayout.Toggle("Use Bokeh Texture", bloomDeluxe.m_UseBokehFlare);
            if (bloomDeluxe.m_UseBokehFlare)
            {
                bloomDeluxe.m_BokehFlareQuality = (UltimateBloom.BokehFlareQuality)EditorGUILayout.EnumPopup("  Bokeh Quality", bloomDeluxe.m_BokehFlareQuality);
  
                bloomDeluxe.m_BokehScale = Mathf.Clamp(EditorGUILayout.FloatField("  Bokeh Scale", bloomDeluxe.m_BokehScale), 0.2f, 2.5f);
                bloomDeluxe.m_FlareShape = (Texture2D)EditorGUILayout.ObjectField("  Bokeh Texture", bloomDeluxe.m_FlareShape, typeof(Texture2D), false);
            }
        }
        if (lastUseLensFlare != bloomDeluxe.m_UseLensFlare)
            bloomDeluxe.ForceShadersReload();
        GUILayout.Space(10.0f);
        GUILayout.EndVertical();

        // Anamorphic lens flare

        GUILayout.BeginVertical("Box");
        GUILayout.Label("Anamorphic Lens Flare", EditorStyles.boldLabel);
        bool lastUseAnamorphic = bloomDeluxe.m_UseAnamorphicFlare;
        bloomDeluxe.m_UseAnamorphicFlare = EditorGUILayout.Toggle("Use Anamorphic Lens Flare", bloomDeluxe.m_UseAnamorphicFlare);
        if (bloomDeluxe.m_UseAnamorphicFlare)
        {
            bloomDeluxe.m_AnamorphicDownscaleCount = Mathf.Clamp(EditorGUILayout.IntField("  Layers (Downscale Count)", bloomDeluxe.m_AnamorphicDownscaleCount), 1, 6);
            bloomDeluxe.m_AnamorphicFlareIntensity = DoSlider("  Intensity", bloomDeluxe.m_AnamorphicFlareIntensity, 0.0f, 5.0f);
            //bloomDeluxe.m_AnamorphicFlareTreshold = DoSlider("  Treshold", bloomDeluxe.m_AnamorphicFlareTreshold, 0.0f, 5.0f);
            bloomDeluxe.m_AnamorphicScale = DoSlider("  Scale", bloomDeluxe.m_AnamorphicScale, 0.0f, 6.0f);
            bloomDeluxe.m_AnamorphicBlurPass = Mathf.Clamp(EditorGUILayout.IntField("  Blur Pass", bloomDeluxe.m_AnamorphicBlurPass), 1, 6);
            bloomDeluxe.m_AnamorphicSmallVerticalBlur = EditorGUILayout.Toggle("  Anti-jitter Pass", bloomDeluxe.m_AnamorphicSmallVerticalBlur);
            bloomDeluxe.m_AnamorphicDirection = (UltimateBloom.AnamorphicDirection)EditorGUILayout.EnumPopup("  Direction", bloomDeluxe.m_AnamorphicDirection);
        }
        if (lastUseAnamorphic != bloomDeluxe.m_UseAnamorphicFlare)
            bloomDeluxe.ForceShadersReload();

        if (bloomDeluxe.m_UseAnamorphicFlare)
        {
            if (GUILayout.Button((bloomDeluxe.m_UiShowAnamorphicBloomScales ? "Hide Layers" : "Show Layers") + "[" + bloomDeluxe.m_AnamorphicDownscaleCount + "]"))
                bloomDeluxe.m_UiShowAnamorphicBloomScales = !bloomDeluxe.m_UiShowAnamorphicBloomScales;
            if (bloomDeluxe.m_UiShowAnamorphicBloomScales)
            {
                for (int i = 0; i < bloomDeluxe.m_AnamorphicDownscaleCount; ++i)
                {
                    GUILayout.BeginVertical(i % 2 == 0 ? m_Background3 : m_Background1);
                    int idx = i + 1;
                    bloomDeluxe.m_AnamorphicBloomUsages[i] = EditorGUILayout.Toggle("  Layer " + idx + " Enabled", bloomDeluxe.m_AnamorphicBloomUsages[i]);
                    bloomDeluxe.m_AnamorphicBloomIntensities[i] = DoSlider("  Layer " + idx + " Intensity", bloomDeluxe.m_AnamorphicBloomIntensities[i], 0.0f, 5.0f);
                    bloomDeluxe.m_AnamorphicBloomColors[i] = EditorGUILayout.ColorField("  Layer " + idx + " Tint", bloomDeluxe.m_AnamorphicBloomColors[i]);
                    GUILayout.EndVertical();
                }
            }
        }

        GUILayout.Space(10.0f);
        GUILayout.EndVertical();

        // Star lens flare

        GUILayout.BeginVertical("Box");
        GUILayout.Label("Star Lens Flare", EditorStyles.boldLabel);
        bool lastUseStar = bloomDeluxe.m_UseStarFlare;
        bloomDeluxe.m_UseStarFlare = EditorGUILayout.Toggle("Use Star Lens Flare", bloomDeluxe.m_UseStarFlare);
        if (bloomDeluxe.m_UseStarFlare)
        {
            bloomDeluxe.m_StarDownscaleCount = Mathf.Clamp(EditorGUILayout.IntField("  Layers (Downscale Count)", bloomDeluxe.m_StarDownscaleCount), 1, 6);
            bloomDeluxe.m_StarFlareIntensity = DoSlider("  Intensity", bloomDeluxe.m_StarFlareIntensity, 0.0f, 5.0f);
            //bloomDeluxe.m_StarFlareTreshol = DoSlider("  Treshold", bloomDeluxe.m_StarFlareTreshol, 0.0f, 5.0f);
            bloomDeluxe.m_StarScale = DoSlider("  Scale", bloomDeluxe.m_StarScale, 0.0f, 5.0f);
            bloomDeluxe.m_StarBlurPass = Mathf.Clamp(EditorGUILayout.IntField("  Blur Pass", bloomDeluxe.m_StarBlurPass), 1, 4);
        }
        if (lastUseStar != bloomDeluxe.m_UseStarFlare)
            bloomDeluxe.ForceShadersReload();

        if (bloomDeluxe.m_UseStarFlare)
        {
            if (GUILayout.Button((bloomDeluxe.m_UiShowStarBloomScales ? "Hide Layers" : "Show Layers") + "[" + bloomDeluxe.m_StarDownscaleCount + "]"))
                bloomDeluxe.m_UiShowStarBloomScales = !bloomDeluxe.m_UiShowStarBloomScales;
            if (bloomDeluxe.m_UiShowStarBloomScales)
            {
                for (int i = 0; i < bloomDeluxe.m_StarDownscaleCount; ++i)
                {
                    GUILayout.BeginVertical(i % 2 == 0 ? m_Background3 : m_Background1);
                    int idx = i + 1;
                    bloomDeluxe.m_StarBloomUsages[i] = EditorGUILayout.Toggle("  Layer " + idx + " Enabled", bloomDeluxe.m_StarBloomUsages[i]);
                    bloomDeluxe.m_StarBloomIntensities[i] = DoSlider("  Layer " + idx + " Intensity", bloomDeluxe.m_StarBloomIntensities[i], 0.0f, 5.0f);
                    bloomDeluxe.m_StarBloomColors[i] = EditorGUILayout.ColorField("  Layer " + idx + " Tint", bloomDeluxe.m_StarBloomColors[i]);
                    GUILayout.EndVertical();
                }
            }
        }

        GUILayout.EndVertical();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }


    [MenuItem("Component/UltimateBloom/Add to selected camera")]
    public static void AddPreset()
    {
        UltimateBloom ub = GetUltimateBloomObject();
        if (ub == null)
            return;

        ub.CreateMaterials();

        ub.m_BloomUsages[0] = false;
        ub.m_AnamorphicBloomUsages[0] = false;
        ub.m_AnamorphicBloomUsages[1] = false;
        ub.m_StarBloomUsages[0] = false;
        ub.m_StarBloomUsages[1] = false;
        ub.m_UpsamplingQuality = UltimateBloom.BloomSamplingQuality.MediumKernel;


        string flareMaskPath = "Assets/Paroxe/UltimateBloom/Graphics/FlareMask.png";
        Texture2D flareMask = (Texture2D)AssetDatabase.LoadAssetAtPath(flareMaskPath, typeof(Texture2D));
        ub.m_FlareMask = flareMask;

        string bokehPath = "Assets/Paroxe/UltimateBloom/Graphics/Bokeh.png";
        Texture2D bokeh = (Texture2D)AssetDatabase.LoadAssetAtPath(bokehPath, typeof(Texture2D));
        ub.m_FlareShape = bokeh;

        string dustPath = "Assets/Paroxe/UltimateBloom/DirtTextureSample/CreatedWithDirtGenerator2.png";
        Texture2D dust = (Texture2D)AssetDatabase.LoadAssetAtPath(dustPath, typeof(Texture2D));
        ub.m_DustTexture = dust;
    }

    private static UltimateBloom GetUltimateBloomObject()
    {
        GameObject obj = Selection.activeGameObject;
        if (obj == null)
            return null;
        if (obj.GetComponent<Camera>() == null)
            return null;

        return obj.AddComponent<UltimateBloom>();
    }

    float DoSlider(string label, float value, float min, float max)
    {
        float v = value;
        EditorGUILayout.BeginHorizontal();
        v = Mathf.Clamp(EditorGUILayout.FloatField(label, v, GUILayout.ExpandWidth(false)), min, max);
        v = GUILayout.HorizontalSlider(v, min, max);
        EditorGUILayout.EndHorizontal();

        return v;
    }

    void SetFlarePreset(UltimateBloom.FlarePresets preset, UltimateBloom ub)
    {
        if (preset == UltimateBloom.FlarePresets.ChoosePreset)
            return;

        if (preset == UltimateBloom.FlarePresets.Bokeh2 || preset == UltimateBloom.FlarePresets.Ghost2)
        {
            ub.m_FlareTint0 = new Color(78 / 255.0f, 69 / 255.0f, 149.0f / 255.0f);
            ub.m_FlareTint1 = new Color(36 / 255.0f, 51 / 255.0f, 141 / 255.0f);
            ub.m_FlareTint2 = new Color(29 / 255.0f, 41 / 255.0f, 105 / 255.0f);
            ub.m_FlareTint3 = new Color(17 / 255.0f, 22 / 255.0f, 107 / 255.0f);
            ub.m_FlareTint4 = new Color(78 / 255.0f, 69 / 255.0f, 149.0f / 255.0f);
            ub.m_FlareTint5 = new Color(36 / 255.0f, 51 / 255.0f, 141 / 255.0f);
            ub.m_FlareTint6 = new Color(29 / 255.0f, 41 / 255.0f, 105 / 255.0f);
            ub.m_FlareTint7 = new Color(17 / 255.0f, 22 / 255.0f, 107 / 255.0f);
        }
        else
        {
            ub.m_FlareTint0 = new Color(137 / 255.0f, 82 / 255.0f, 0 / 255.0f);
            ub.m_FlareTint1 = new Color(0 / 255.0f, 63 / 255.0f, 126 / 255.0f);
            ub.m_FlareTint2 = new Color(72 / 255.0f, 151 / 255.0f, 0 / 255.0f);
            ub.m_FlareTint3 = new Color(114 / 255.0f, 35 / 255.0f, 0 / 255.0f);
            ub.m_FlareTint4 = new Color(122 / 255.0f, 88 / 255.0f, 0 / 255.0f);
            ub.m_FlareTint5 = new Color(137 / 255.0f, 71 / 255.0f, 0 / 255.0f);
            ub.m_FlareTint6 = new Color(97 / 255.0f, 139 / 255.0f, 0 / 255.0f);
            ub.m_FlareTint7 = new Color(40 / 255.0f, 142 / 255.0f, 0 / 255.0f);
        }

        ub.m_FlareIntensity = 1.0f;
        ub.m_FlareTreshold = 0.8f;

        if (preset == UltimateBloom.FlarePresets.Bokeh1 || preset == UltimateBloom.FlarePresets.Bokeh2)
        {
            ub.m_FlareScales = new Vector4(5.77f, 2.5f, 1.32f, 1.12f);
            ub.m_FlareScalesNear = new Vector4(12, 7.37f, 5.3f, 4.14f);
            ub.m_FlareRendering = UltimateBloom.FlareRendering.Sharp;
            ub.m_UseBokehFlare = true;
            
        }
        else
        {
            ub.m_FlareScales = new Vector4(1.0f, 0.6f, 0.5f, 0.4f);
            ub.m_FlareScalesNear = new Vector4(1.0f, 0.8f, 0.6f, 0.5f);
            ub.m_FlareBlurQuality = UltimateBloom.FlareBlurQuality.High;
            ub.m_FlareRendering = UltimateBloom.FlareRendering.Blurred;
            ub.m_UseBokehFlare = false;
            

            if (preset == UltimateBloom.FlarePresets.GhostFast)
                ub.m_FlareBlurQuality = UltimateBloom.FlareBlurQuality.Fast;
        }

    }

    void ChangeQualityPreset(UltimateBloom.BloomQualityPreset newPreset)
    {

    }
}
