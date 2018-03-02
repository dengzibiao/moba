using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class UltimateBloom : MonoBehaviour
{

    public enum BloomQualityPreset
    {
        Optimized,
        Standard,
        HighVisuals,
        Custom
    }

    public enum BloomSamplingQuality
    {
        VerySmallKernel, // 9
        SmallKernel, //  13
        MediumKernel, // 17
        LargeKernel, // 23
        LargerKernel, // 27
        VeryLargeKernel // 31
    }

    public enum BloomScreenBlendMode
    {
        Screen = 0,
        Add = 1,
    }

    public enum HDRBloomMode
    {
        Auto = 0,
        On = 1,
        Off = 2,
    }

    public enum BlurSampleCount
    {
        Nine,
        Seventeen,
        Thirteen,
        TwentyThree,
        TwentySeven,
        ThrirtyOne,
        NineCurve,
        FourSimple
    }

    public enum FlareRendering
    {
        Sharp,
        Blurred,
        MoreBlurred
    }

    public enum SimpleSampleCount
    {
        Four,
        Nine,
        FourCurve,
        ThirteenTemporal,
        ThirteenTemporalCurve
    }

    public enum FlareType
    {
        Single,
        Double
    }

    public enum BloomIntensityManagement
    {
        FilmicCurve,
        Threshold
    }

    private enum FlareStripeType
    {
        Anamorphic,
        Star,
        DiagonalUpright,
        DiagonalUpleft
    }

    public enum AnamorphicDirection
    {
        Horizontal,
        Vertical
    }

    public enum BokehFlareQuality
    {
        Low,
        Medium,
        High,
        VeryHigh
    }

    public enum BlendMode
    {
        ADD,
        SCREEN
    }

    public enum SamplingMode
    {
        Fixed,
        HeightRelative
    }

    public enum FlareBlurQuality
    {
        Fast,
        Normal,
        High
    }

    public enum FlarePresets
    {
        ChoosePreset,
        GhostFast,
        Ghost1,
        Ghost2,
        Bokeh1,
        Bokeh2
    }
 


    public float m_SamplingMinHeight = 400.0f;
    public float[] m_ResSamplingPixelCount = new float[6];

    public SamplingMode m_SamplingMode = SamplingMode.HeightRelative;

    public BlendMode m_BlendMode = BlendMode.ADD;
    public float m_ScreenMaxIntensity;

    public BloomQualityPreset m_QualityPreset;

    public HDRBloomMode m_HDR = HDRBloomMode.Auto;

    public BloomScreenBlendMode m_ScreenBlendMode = BloomScreenBlendMode.Add;

    public float m_BloomIntensity = 1.0f;

    public float m_BloomThreshhold = 0.5f;
    public Color m_BloomThreshholdColor = Color.white;
    public int m_DownscaleCount = 5;
    public BloomIntensityManagement m_IntensityManagement = BloomIntensityManagement.FilmicCurve;
    public float[] m_BloomIntensities;
    public Color[] m_BloomColors;
    public bool[] m_BloomUsages;

    [SerializeField]
    public DeluxeFilmicCurve m_BloomCurve = new DeluxeFilmicCurve();

    private int m_LastDownscaleCount = 5;

    public bool m_UseLensFlare = false;
    public float m_FlareTreshold = 0.8f;
    public float m_FlareIntensity = 0.25f;

    public Color m_FlareTint0 = new Color(137 / 255.0f, 82 / 255.0f, 0 / 255.0f);
    public Color m_FlareTint1 = new Color(0 / 255.0f, 63 / 255.0f, 126 / 255.0f);
    public Color m_FlareTint2 = new Color(72 / 255.0f, 151 / 255.0f, 0 / 255.0f);
    public Color m_FlareTint3 = new Color(114 / 255.0f, 35 / 255.0f, 0 / 255.0f);
    public Color m_FlareTint4 = new Color(122 / 255.0f, 88 / 255.0f, 0 / 255.0f);
    public Color m_FlareTint5 = new Color(137 / 255.0f, 71 / 255.0f, 0 / 255.0f);
    public Color m_FlareTint6 = new Color(97 / 255.0f, 139 / 255.0f, 0 / 255.0f);
    public Color m_FlareTint7 = new Color(40 / 255.0f, 142 / 255.0f, 0 / 255.0f);

    public float m_FlareGlobalScale = 1.0f;
    public Vector4 m_FlareScales = new Vector4(1.0f, 0.6f, 0.5f, 0.4f);
    public Vector4 m_FlareScalesNear = new Vector4(1.0f, 0.8f, 0.6f, 0.5f);
    public Texture2D m_FlareMask;
    public FlareRendering m_FlareRendering = FlareRendering.Blurred;
    public FlareType m_FlareType = FlareType.Double;
    public Texture2D m_FlareShape;
    public FlareBlurQuality m_FlareBlurQuality = FlareBlurQuality.High;
    BokehRenderer m_FlareSpriteRenderer;
    Mesh[] m_BokehMeshes;
    public bool m_UseBokehFlare = false;
    public float m_BokehScale = 0.4f;
    //public bool m_HighQualityBokehFlare = true;
    public BokehFlareQuality m_BokehFlareQuality = BokehFlareQuality.Medium;

    public bool m_UseAnamorphicFlare = false;
    public float m_AnamorphicFlareTreshold = 0.8f;
    public float m_AnamorphicFlareIntensity = 1.0f;
    public int m_AnamorphicDownscaleCount = 3;
    public int m_AnamorphicBlurPass = 2;
    private int m_LastAnamorphicDownscaleCount;
    private RenderTexture[] m_AnamorphicUpscales;
    public float[] m_AnamorphicBloomIntensities;
    public Color[] m_AnamorphicBloomColors;
    public bool[] m_AnamorphicBloomUsages;
    public bool m_AnamorphicSmallVerticalBlur;
    public AnamorphicDirection m_AnamorphicDirection = AnamorphicDirection.Horizontal;
    public float m_AnamorphicScale = 3.0f;


    public bool m_UseStarFlare = false;
    public float m_StarFlareTreshol = 0.8f;
    public float m_StarFlareIntensity = 1.0f;
    public float m_StarScale = 2.0f;
    public int m_StarDownscaleCount = 3;
    public int m_StarBlurPass = 1;
    private int m_LastStarDownscaleCount;
    private RenderTexture[] m_StarUpscales;
    public float[] m_StarBloomIntensities;
    public Color[] m_StarBloomColors;
    public bool[] m_StarBloomUsages;

    public bool m_UseLensDust = false;
    public float m_DustIntensity = 1.0f;
    public Texture2D m_DustTexture;
    public float m_DirtLightIntensity = 5.0f;

    public BloomSamplingQuality m_DownsamplingQuality;
    public BloomSamplingQuality m_UpsamplingQuality;
    public bool m_TemporalStableDownsampling = false;
    

    // Misc
    public bool m_InvertImage = false;


    // Materials/shaders
    private Material m_FlareMaterial;
    private Shader m_FlareShader;
    private Material m_SamplingMaterial;
    private Shader m_SamplingShader;
    private Material m_CombineMaterial;
    private Shader m_CombineShader;
    private Material m_BrightpassMaterial;
    private Shader m_BrightpassShader;
    private Material m_FlareMaskMaterial;
    private Shader m_FlareMaskShader;
    private Material m_MixerMaterial;
    private Shader m_MixerShader;

    private Material m_FlareBokehMaterial;
    private Shader m_FlareBokehShader;

    // Optimization
    public bool m_DirectDownSample = false;
    public bool m_DirectUpsample = false;

    // UI
    public bool m_UiShowBloomScales = false;
    public bool m_UiShowAnamorphicBloomScales = false;
    public bool m_UiShowStarBloomScales = false;
    public bool m_UiShowHeightSampling = false;

    private void DestroyMaterial(Material mat)
    {
        if (mat)
        {
            DestroyImmediate(mat);
            mat = null;
        }
    }

    private void LoadShader(ref Material material, ref Shader shader, string shaderPath)
    {
        if (shader != null)
            return;
        shader = Shader.Find(shaderPath);
        if (shader == null)
        {
            Debug.LogError("Shader not found: " + shaderPath);
            return;
        }


        if (!shader.isSupported)
        {
            Debug.LogError("Shader contains error: " + shaderPath + "\n Maybe include path? Try rebuilding the shader.");
            return;
        }

        material = CreateMaterial(shader);
    }

    public void CreateMaterials()
    { 
        //m_FlareType = FlareType.Double;
		int maxScaleCount = 8;
		if (m_BloomIntensities == null || m_BloomIntensities.Length < maxScaleCount)
		{
			m_BloomIntensities = new float[maxScaleCount];
			for (int i = 0; i < 8; ++i)
				m_BloomIntensities[i] = 1.0f;
		}
		if (m_BloomColors == null || m_BloomColors.Length < maxScaleCount)
		{
			m_BloomColors = new Color[maxScaleCount];
			for (int i = 0; i < 8; ++i)
				m_BloomColors[i] = Color.white;
		}
		if (m_BloomUsages == null || m_BloomUsages.Length < maxScaleCount )
		{
			m_BloomUsages = new bool[maxScaleCount];
			for (int i = 0; i < 8; ++i)
				m_BloomUsages[i] = true;
		}
		
		if (m_AnamorphicBloomIntensities == null || m_AnamorphicBloomIntensities.Length < maxScaleCount)
		{
			m_AnamorphicBloomIntensities = new float[maxScaleCount];
			for (int i = 0; i < 8; ++i)
				m_AnamorphicBloomIntensities[i] = 1.0f;
		}
		if (m_AnamorphicBloomColors == null || m_AnamorphicBloomColors.Length < maxScaleCount)
		{
			m_AnamorphicBloomColors = new Color[maxScaleCount];
			for (int i = 0; i < 8; ++i)
				m_AnamorphicBloomColors[i] = Color.white;
		}
		if (m_AnamorphicBloomUsages == null || m_AnamorphicBloomUsages.Length < maxScaleCount )
		{
			m_AnamorphicBloomUsages = new bool[maxScaleCount];
			for (int i = 0; i < 8; ++i)
				m_AnamorphicBloomUsages[i] = true;
		}
		
		if (m_StarBloomIntensities == null || m_StarBloomIntensities.Length < maxScaleCount)
		{
			m_StarBloomIntensities = new float[maxScaleCount];
			for (int i = 0; i < 8; ++i)
				m_StarBloomIntensities[i] = 1.0f;
		}
		if (m_StarBloomColors == null || m_StarBloomColors.Length < maxScaleCount)
		{
			m_StarBloomColors = new Color[maxScaleCount];
			for (int i = 0; i < 8; ++i)
				m_StarBloomColors[i] = Color.white;
		}
		if (m_StarBloomUsages == null || m_StarBloomUsages.Length < maxScaleCount)
		{
			m_StarBloomUsages = new bool[maxScaleCount];
			for (int i = 0; i < 8; ++i)
				m_StarBloomUsages[i] = true;
		}

        if (m_FlareSpriteRenderer == null && m_FlareShape != null && m_UseBokehFlare)
        {
            if (m_FlareSpriteRenderer != null)
                m_FlareSpriteRenderer.Clear(ref m_BokehMeshes);

            m_FlareSpriteRenderer = new BokehRenderer();
        }

        if (m_SamplingMaterial == null)
        {
            m_DownSamples = new RenderTexture[GetNeededDownsamples()];
            m_UpSamples = new RenderTexture[m_DownscaleCount];
            m_AnamorphicUpscales = new RenderTexture[m_AnamorphicDownscaleCount];
            m_StarUpscales = new RenderTexture[m_StarDownscaleCount];
        }

        string flareShaderPath = m_FlareType == FlareType.Single ? "Hidden/Ultimate/FlareSingle" : "Hidden/Ultimate/FlareDouble";
        LoadShader(ref m_FlareMaterial, ref m_FlareShader, flareShaderPath);

        LoadShader(ref m_SamplingMaterial, ref m_SamplingShader, "Hidden/Ultimate/Sampling");
        LoadShader(ref m_BrightpassMaterial, ref m_BrightpassShader, "Hidden/Ultimate/BrightpassMask");
        LoadShader(ref m_FlareMaskMaterial, ref m_FlareMaskShader, "Hidden/Ultimate/FlareMask");
        LoadShader(ref m_MixerMaterial, ref m_MixerShader, "Hidden/Ultimate/BloomMixer");
        LoadShader(ref m_FlareBokehMaterial, ref m_FlareBokehShader, "Hidden/Ultimate/FlareMesh");

        bool useDustOrFlare = m_UseLensDust || m_UseLensFlare || m_UseAnamorphicFlare || m_UseStarFlare;

        string combineShaderPath = "Hidden/Ultimate/BloomCombine";
        if (useDustOrFlare)
            combineShaderPath += "FlareDirt";
        if (m_BlendMode == BlendMode.SCREEN)
            combineShaderPath += "Screen";



        LoadShader(ref m_CombineMaterial, ref m_CombineShader, combineShaderPath);
    }

    private Material CreateMaterial(Shader shader)
    {
        if (!shader)
            return null;
        Material m = new Material(shader);
        m.hideFlags = HideFlags.HideAndDontSave;
        return m;
    }

    void OnDisable()
    {
        ForceShadersReload();
        if (m_FlareSpriteRenderer != null)
        {
            m_FlareSpriteRenderer.Clear(ref m_BokehMeshes);
            m_FlareSpriteRenderer = null;
        }
    }

    public void ForceShadersReload()
    {
        DestroyMaterial(m_FlareMaterial); m_FlareMaterial = null; m_FlareShader = null;
        DestroyMaterial(m_SamplingMaterial); m_SamplingMaterial = null; m_SamplingShader = null;
        DestroyMaterial(m_CombineMaterial); m_CombineMaterial = null; m_CombineShader = null;
        DestroyMaterial(m_BrightpassMaterial); m_BrightpassMaterial = null; m_BrightpassShader = null;
        DestroyMaterial(m_FlareBokehMaterial); m_FlareBokehMaterial = null; m_FlareBokehShader = null;
        DestroyMaterial(m_FlareMaskMaterial); m_FlareMaskMaterial = null; m_FlareMaskShader = null;
        DestroyMaterial(m_MixerMaterial); m_MixerMaterial = null; m_MixerShader = null;
    }


    private RenderTexture[] m_DownSamples;
    private RenderTexture[] m_UpSamples;

    int GetNeededDownsamples()
    {
        return Mathf.Max(   m_DownscaleCount,
                            m_UseAnamorphicFlare ? m_AnamorphicDownscaleCount : 0,
                            (m_UseLensFlare ) ? GetGhostBokehLayer()+1 : 0,
                            m_UseStarFlare ? m_StarDownscaleCount : 0);
    }

    private RenderTextureFormat m_Format;


    bool[] m_BufferUsage;
    void ComputeBufferOptimization()
    {
        if (m_BufferUsage == null)
            m_BufferUsage = new bool[m_DownSamples.Length];
        if (m_BufferUsage.Length != m_DownSamples.Length)
            m_BufferUsage = new bool[m_DownSamples.Length];

        for (int i = 0; i < m_BufferUsage.Length; ++i)
            m_BufferUsage[i] = false;

        for (int i = 0; i < m_BufferUsage.Length; ++i)
            m_BufferUsage[i] = m_BloomUsages[i] ? true : m_BufferUsage[i];

        if (m_UseAnamorphicFlare)
            for (int i = 0; i < m_BufferUsage.Length; ++i)
                m_BufferUsage[i] = m_AnamorphicBloomUsages[i] ? true : m_BufferUsage[i];

        if (m_UseStarFlare)
            for (int i = 0; i < m_BufferUsage.Length; ++i)
                m_BufferUsage[i] = m_StarBloomUsages[i] ? true : m_BufferUsage[i];
    }

    int GetGhostBokehLayer()
    {
        if (m_UseBokehFlare && m_FlareShape != null)
        {
            if (m_BokehFlareQuality == BokehFlareQuality.VeryHigh)
                return 1;
            if (m_BokehFlareQuality == BokehFlareQuality.High)
                return 2;
            if (m_BokehFlareQuality == BokehFlareQuality.Medium)
                return 3;
            if (m_BokehFlareQuality == BokehFlareQuality.Low)
                return 4;
        }
        return 0;// Ghost
    }

    BlurSampleCount GetUpsamplingSize()
    {
        if (m_SamplingMode == SamplingMode.Fixed)
        {
            BlurSampleCount upsamplingCount = BlurSampleCount.ThrirtyOne;
            if (m_UpsamplingQuality == BloomSamplingQuality.VerySmallKernel)
                upsamplingCount = BlurSampleCount.Nine;
            else if (m_UpsamplingQuality == BloomSamplingQuality.SmallKernel)
                upsamplingCount = BlurSampleCount.Thirteen;
            else if (m_UpsamplingQuality == BloomSamplingQuality.MediumKernel)
                upsamplingCount = BlurSampleCount.Seventeen;
            else if (m_UpsamplingQuality == BloomSamplingQuality.LargeKernel)
                upsamplingCount = BlurSampleCount.TwentyThree;
            else if (m_UpsamplingQuality == BloomSamplingQuality.LargerKernel)
                upsamplingCount = BlurSampleCount.TwentySeven;
            return upsamplingCount;
        }

        float pixelCount = Screen.height;//Screen.width * Screen.height;
        int nearestIdx = 0;
        float nearestDist = float.MaxValue;
        for (int i = 0; i < m_ResSamplingPixelCount.Length; ++i)
        {
            float dist = Math.Abs(pixelCount - m_ResSamplingPixelCount[i]);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearestIdx = i;
            }
        }

        if (nearestIdx == 0)
            return BlurSampleCount.Nine;
        if (nearestIdx == 1)
            return BlurSampleCount.Thirteen;
        if (nearestIdx == 2)
            return BlurSampleCount.Seventeen;
        if (nearestIdx == 3)
            return BlurSampleCount.TwentyThree;
        if (nearestIdx == 4)
            return BlurSampleCount.TwentySeven;

        return BlurSampleCount.ThrirtyOne;
    }


    public void ComputeResolutionRelativeData()
    {
        float currentRes = m_SamplingMinHeight;
        float currentSampling = 9.0f;
        for (int i = 0; i < m_ResSamplingPixelCount.Length; ++i)
        {
            m_ResSamplingPixelCount[i] = currentRes;

            float nextSampling = currentSampling + 4.0f;
            float ratio = nextSampling / currentSampling;
            currentRes *= ratio;
            currentSampling = nextSampling;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Determine Texture Format
        bool doHdr = false;
        if (m_HDR == HDRBloomMode.Auto)
            doHdr = source.format == RenderTextureFormat.ARGBHalf && GetComponent<Camera>().hdr;
        else
            doHdr = m_HDR == HDRBloomMode.On;

        m_Format = (doHdr) ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.Default;

        if (m_DownSamples != null)
        {
            if (m_DownSamples.Length != GetNeededDownsamples())
            {
                OnDisable();
            }
        }


        if (m_LastDownscaleCount != m_DownscaleCount 
            || m_LastAnamorphicDownscaleCount != m_AnamorphicDownscaleCount
            || m_LastStarDownscaleCount != m_StarDownscaleCount)
        {
            OnDisable();
        }
        m_LastDownscaleCount = m_DownscaleCount;
        m_LastAnamorphicDownscaleCount = m_AnamorphicDownscaleCount;
        m_LastStarDownscaleCount = m_StarDownscaleCount;

        CreateMaterials();

        if (m_DirectDownSample || m_DirectUpsample)
            ComputeBufferOptimization();

        bool debugFlareShape = false;

        if (m_SamplingMode == SamplingMode.HeightRelative)
            ComputeResolutionRelativeData();

        //////////////////////////////////
        // 1. Bright pass
        //////////////////////////////////
        RenderTexture brightTexture = RenderTexture.GetTemporary(source.width, source.height, 0, m_Format);
        brightTexture.filterMode = FilterMode.Bilinear;

        if (m_IntensityManagement == BloomIntensityManagement.Threshold)
            BrightPass(source, brightTexture, m_BloomThreshhold * m_BloomThreshholdColor);
        else
            Graphics.Blit(source, brightTexture);

        //////////////////////////////////
        // 2. Downscale source texture
        //////////////////////////////////
        if (m_IntensityManagement == BloomIntensityManagement.Threshold)
            CachedDownsample(brightTexture, m_DownSamples, null);
        else
            CachedDownsample(brightTexture, m_DownSamples, m_BloomCurve);

        //////////////////////////////////
        // 3. Upsample
        //////////////////////////////////
        // Upsampling quality
        BlurSampleCount upsamplingCount = GetUpsamplingSize();
        // Upsample
        CachedUpsample(m_DownSamples, m_UpSamples, source.width, source.height, upsamplingCount);
        // Release unused upsamples
       /* if (m_DirectUpsample)
        {
            for (int i = 1; i < m_UpSamples.Length; ++i)
                if (m_BufferUsage[i])
                    RenderTexture.ReleaseTemporary(m_UpSamples[i]);
        }
        else*/


            //for (int i = 1; i < m_UpSamples.Length; ++i)
            //    RenderTexture.ReleaseTemporary(m_UpSamples[i]);

        //////////////////////////////////
        // Optional: Ghost lens flare
        //////////////////////////////////
        Texture flareRT = Texture2D.blackTexture;
        RenderTexture flareShapeBuffer = null;
        if (m_UseLensFlare)
        {
            int bokehQuality = GetGhostBokehLayer();

            int flareWidth = source.width / (int)Mathf.Pow(2.0f, bokehQuality);
            int flareHeigth = source.height / (int)Mathf.Pow(2.0f, bokehQuality);

            if (m_FlareShape != null && m_UseBokehFlare)
            {
                float size = 15.0f;
                if (m_BokehFlareQuality == BokehFlareQuality.Medium)
                    size *= 2;
                if (m_BokehFlareQuality == BokehFlareQuality.High)
                    size *= 4;
                if (m_BokehFlareQuality == BokehFlareQuality.VeryHigh)
                    size *= 8;

                size *= m_BokehScale;
                m_FlareSpriteRenderer.SetMaterial(m_FlareBokehMaterial);
                m_FlareSpriteRenderer.RebuildMeshIfNeeded(flareWidth, flareHeigth, 1.0f / flareWidth * size, 1.0f / flareHeigth * size, ref m_BokehMeshes);
                m_FlareSpriteRenderer.SetTexture(m_FlareShape);

                flareShapeBuffer = RenderTexture.GetTemporary(source.width / 4 , source.height  / 4 , 0, m_Format);

                int bokehTargetSize = bokehQuality;
                RenderTexture flareBrightTexture = RenderTexture.GetTemporary(source.width / (int)Mathf.Pow(2.0f, (bokehTargetSize + 1)), source.height / (int)Mathf.Pow(2.0f, (bokehTargetSize + 1)), 0, m_Format);
                BrightPass(m_DownSamples[bokehQuality], flareBrightTexture, m_FlareTreshold * Vector4.one);
                m_FlareSpriteRenderer.RenderFlare(flareBrightTexture, flareShapeBuffer, m_UseBokehFlare ? 1.0f : m_FlareIntensity, ref m_BokehMeshes);
                RenderTexture.ReleaseTemporary(flareBrightTexture);

                RenderTexture maskFlare = RenderTexture.GetTemporary(flareShapeBuffer.width, flareShapeBuffer.height, 0, m_Format);
                m_FlareMaskMaterial.SetTexture("_MaskTex", m_FlareMask);
                Graphics.Blit(flareShapeBuffer, maskFlare, m_FlareMaskMaterial, 0);

                RenderTexture.ReleaseTemporary(flareShapeBuffer); flareShapeBuffer = null;
                RenderFlares(maskFlare, source, ref flareRT);
                RenderTexture.ReleaseTemporary(maskFlare);
            }
            else
            {
                //BrightPassWithMask(source, brightTexture, m_FlareTreshold * Vector4.one, m_FlareMask);
                //RenderFlares( brightTexture, source, ref flareRT);
                int ghostLayer = GetGhostBokehLayer();
                RenderTexture flareSource = m_DownSamples[ghostLayer];
                RenderTexture flareBrightTexture = RenderTexture.GetTemporary(flareSource.width, flareSource.height, 0, m_Format);

                BrightPassWithMask(m_DownSamples[ghostLayer], flareBrightTexture, m_FlareTreshold * Vector4.one, m_FlareMask);
                RenderFlares(flareBrightTexture, source, ref flareRT);

                RenderTexture.ReleaseTemporary(flareBrightTexture);
            }

            
        }

        if (!m_UseLensFlare && m_FlareSpriteRenderer != null)
            m_FlareSpriteRenderer.Clear(ref m_BokehMeshes);

        //////////////////////////////////
        // Optional: Anamorphic lens flare
        //////////////////////////////////
        if (m_UseAnamorphicFlare)
        {
            RenderTexture anamorphicResult = RenderStripe(m_DownSamples, upsamplingCount, source.width, source.height, FlareStripeType.Anamorphic);

            if (anamorphicResult != null)
            {
                if (m_UseLensFlare)
                {
                    RenderTextureAdditive(anamorphicResult, (RenderTexture)flareRT, 1.0f);
                    RenderTexture.ReleaseTemporary(anamorphicResult);
                }
                else
                {
                    flareRT = anamorphicResult;
                }
            }
        }

        //////////////////////////////////
        // Optional: Star lens flare
        //////////////////////////////////
        if (m_UseStarFlare)
        {
            //RenderTexture starResult = RenderStar(m_DownSamples, upsamplingCount, source.width, source.height);
            RenderTexture starResult = null;

            if (m_StarBlurPass == 1)
            {
                starResult = RenderStripe(m_DownSamples, upsamplingCount, source.width, source.height, FlareStripeType.Star);

                if (starResult != null)
                {
                    if (m_UseLensFlare || m_UseAnamorphicFlare)
                    {
                        RenderTextureAdditive(starResult, (RenderTexture)flareRT, m_StarFlareIntensity);
                    }
                    else
                    {
                        flareRT = RenderTexture.GetTemporary(source.width, source.height, 0, m_Format);
                        BlitIntensity(starResult, (RenderTexture)flareRT, m_StarFlareIntensity);
                    }

                    RenderTexture.ReleaseTemporary(starResult);
                }
            }
            else
            {
                if (m_UseLensFlare || m_UseAnamorphicFlare)
                {
                    starResult = RenderStripe(m_DownSamples, upsamplingCount, source.width, source.height, FlareStripeType.DiagonalUpright);
                    if (starResult != null)
                    {
                        RenderTextureAdditive(starResult, (RenderTexture)flareRT, m_StarFlareIntensity);
                        RenderTexture.ReleaseTemporary(starResult);
                        starResult = RenderStripe(m_DownSamples, upsamplingCount, source.width, source.height, FlareStripeType.DiagonalUpleft);

                        RenderTextureAdditive(starResult, (RenderTexture)flareRT, m_StarFlareIntensity);
                        RenderTexture.ReleaseTemporary(starResult);
                    }
                }
                else
                {
                    starResult = RenderStripe(m_DownSamples, upsamplingCount, source.width, source.height, FlareStripeType.DiagonalUpleft);
                    if (starResult != null)
                    {
                        RenderTexture tmpStarResult = RenderStripe(m_DownSamples, upsamplingCount, source.width, source.height, FlareStripeType.DiagonalUpright);
                        CombineAdditive(tmpStarResult, starResult, m_StarFlareIntensity, m_StarFlareIntensity);
                        RenderTexture.ReleaseTemporary(tmpStarResult);

                        flareRT = starResult;
                    }
                }
            }

        
        }
        
        // Release downsamples
        if (m_DirectDownSample)
            for (int i = 0; i < m_DownSamples.Length; ++i)
            {
                if (m_BufferUsage[i])
                    RenderTexture.ReleaseTemporary(m_DownSamples[i]);
            }
        else
            for (int i = 0; i < m_DownSamples.Length; ++i)
                RenderTexture.ReleaseTemporary(m_DownSamples[i]);

        //////////////////////////////////
        // Combine pass
        //////////////////////////////////
        m_CombineMaterial.SetFloat("_Intensity", m_BloomIntensity);

        m_CombineMaterial.SetFloat("_FlareIntensity", m_FlareIntensity);
        m_CombineMaterial.SetTexture("_ColorBuffer", source);
        m_CombineMaterial.SetTexture("_FlareTexture", flareRT);
        m_CombineMaterial.SetTexture("_AdditiveTexture", m_UseLensDust ? m_DustTexture : Texture2D.whiteTexture);
        m_CombineMaterial.SetTexture("_brightTexture", brightTexture);
        if (m_UseLensDust)
        {
            m_CombineMaterial.SetFloat("_DirtIntensity", m_DustIntensity);
            m_CombineMaterial.SetFloat("_DirtLightIntensity", m_DirtLightIntensity);
        }
        else
        {
            m_CombineMaterial.SetFloat("_DirtIntensity", 1.0f);
            m_CombineMaterial.SetFloat("_DirtLightIntensity", 0.0f);
        }


        if (m_BlendMode == BlendMode.SCREEN)
        {
            m_CombineMaterial.SetFloat("_ScreenMaxIntensity", m_ScreenMaxIntensity);
        }

        if (m_InvertImage /*QualitySettings.antiAliasing > 0 && camera.actualRenderingPath == RenderingPath.Forward*/)
            Graphics.Blit(m_LastBloomUpsample, destination, m_CombineMaterial, 1);
        else
            Graphics.Blit(m_LastBloomUpsample, destination, m_CombineMaterial, 0);


        for (int i = 0; i < m_UpSamples.Length; ++i)
            if (m_UpSamples[i] != null)
                RenderTexture.ReleaseTemporary(m_UpSamples[i]);

        //Graphics.Blit(m_UpSamples[0], destination);

        //////////////////////////////////
        // Cleaning
        //////////////////////////////////

        if (debugFlareShape)
            Graphics.Blit(flareShapeBuffer, destination);


        if (m_UseLensFlare || m_UseAnamorphicFlare || m_UseStarFlare)
            if (flareRT != null && flareRT is RenderTexture)
                RenderTexture.ReleaseTemporary((RenderTexture)flareRT);

        RenderTexture.ReleaseTemporary(brightTexture);

        if (m_FlareShape != null && m_UseBokehFlare && flareShapeBuffer != null)
        {
            RenderTexture.ReleaseTemporary(flareShapeBuffer);
        }
    }

    RenderTexture RenderStar(RenderTexture[] sources, BlurSampleCount upsamplingCount, int sourceWidth, int sourceHeight)
    {
        for (int i = m_StarUpscales.Length - 1; i >= 0; --i)
        {
            m_StarUpscales[i] = RenderTexture.GetTemporary(sourceWidth / (int)Mathf.Pow(2.0f, i), sourceHeight / (int)Mathf.Pow(2.0f, i), 0, m_Format);
            m_StarUpscales[i].filterMode = FilterMode.Bilinear;

            float horizontalBlur = 1.0f / sources[i].width;
            float verticalBlur = 1.0f / sources[i].height;

            if (i < m_StarDownscaleCount - 1)
                GaussianBlur2(sources[i], m_StarUpscales[i], horizontalBlur * m_StarScale, verticalBlur * m_StarScale, m_StarUpscales[i + 1], upsamplingCount, Color.white, 1.0f);
            else
                GaussianBlur2(sources[i], m_StarUpscales[i], horizontalBlur * m_StarScale, verticalBlur * m_StarScale, null, upsamplingCount, Color.white, 1.0f);
        }

        for (int i = 1; i < m_StarUpscales.Length; ++i)
            if (m_StarUpscales[i] != null)
                RenderTexture.ReleaseTemporary(m_StarUpscales[i]);

        return m_StarUpscales[0];

    }

    delegate void BlurFunction(RenderTexture source, RenderTexture destination, float horizontalBlur, float verticalBlur, RenderTexture additiveTexture, BlurSampleCount sampleCount, Color tint, float intensity);

    RenderTexture RenderStripe(RenderTexture[] sources, BlurSampleCount upsamplingCount, int sourceWidth, int sourceHeight, FlareStripeType type)
    {
        BlurFunction blur = GaussianBlur1;
        //int nbUpscales = m_AnamorphicUpscales.Length;
        RenderTexture[] upscales = m_AnamorphicUpscales;
        bool[] usages = m_AnamorphicBloomUsages;
        float[] intensities = m_AnamorphicBloomIntensities;
        Color[] tints = m_AnamorphicBloomColors;
        bool antiJitter = m_AnamorphicSmallVerticalBlur;
        float blurPass = m_AnamorphicBlurPass;
        float scale = m_AnamorphicScale;
        float globalIntensity = m_AnamorphicFlareIntensity;

        float horiMul = 1.0f;
        float vertiMul = 0.0f;

        if (m_AnamorphicDirection == AnamorphicDirection.Vertical)
        {
            horiMul = 0.0f;
            vertiMul = 1.0f;
        }

        if (type != FlareStripeType.Anamorphic)
        {
            if (type == FlareStripeType.Star)
                blur = GaussianBlur2;

            //nbUpscales = m_StarUpscales.Length;
            upscales = m_StarUpscales;
            usages = m_StarBloomUsages;
            intensities = m_StarBloomIntensities;
            tints = m_StarBloomColors;
            antiJitter = false;
            blurPass = m_StarBlurPass;
            scale = m_StarScale;
            globalIntensity = m_StarFlareIntensity;

            if (type == FlareStripeType.DiagonalUpleft)
                vertiMul = -1.0f;
            else
                vertiMul = 1.0f;
        }

        for (int i = 0; i < upscales.Length; ++i)
            upscales[i] = null;

        RenderTexture additiveTexture = null;

        for (int i = upscales.Length - 1; i >= 0; --i)
        {
            if (sources[i] == null && m_DirectUpsample)
                continue;
            if (!usages[i] && m_DirectUpsample)
                continue;


            upscales[i] = RenderTexture.GetTemporary(sourceWidth / (int)Mathf.Pow(2.0f, i), sourceHeight / (int)Mathf.Pow(2.0f, i), 0, m_Format);
            upscales[i].filterMode = FilterMode.Bilinear;

            float horizontalBlur = 1.0f / upscales[i].width;
            float verticalBlur = 1.0f / upscales[i].height;

            RenderTexture source = sources[i];
            RenderTexture dest = upscales[i];

            if (!usages[i])
            {
                if (additiveTexture != null)
                {
                    if (antiJitter)
                        blur(additiveTexture, dest, m_AnamorphicDirection == AnamorphicDirection.Vertical ? horizontalBlur : 0.0f, m_AnamorphicDirection == AnamorphicDirection.Horizontal ? verticalBlur : 0.0f, null, BlurSampleCount.FourSimple, Color.white, 1.0f);
                    else
                        Graphics.Blit(additiveTexture, dest);
                }
                else
                    Graphics.Blit(Texture2D.blackTexture, dest);

                additiveTexture = upscales[i];
                continue;
            }

            RenderTexture antiJitterBuffer = null;
            if (antiJitter && additiveTexture != null)
            {
                antiJitterBuffer = RenderTexture.GetTemporary(dest.width, dest.height, 0, m_Format);
                blur(additiveTexture, antiJitterBuffer, m_AnamorphicDirection == AnamorphicDirection.Vertical ? horizontalBlur : 0.0f, m_AnamorphicDirection == AnamorphicDirection.Horizontal ? verticalBlur : 0.0f, null, BlurSampleCount.FourSimple, Color.white, 1.0f);
                additiveTexture = antiJitterBuffer;
            }

            if (blurPass == 1)
            {
                blur(source, dest, horizontalBlur * scale * horiMul, verticalBlur * scale * vertiMul, additiveTexture, upsamplingCount, tints[i], intensities[i] * globalIntensity);
            }
            else
            {
                RenderTexture tmp = RenderTexture.GetTemporary(dest.width, dest.height, 0, m_Format);
                bool lastTargetIsTmp = false;
                for (int j = 0; j < blurPass; ++j)
                {
                    RenderTexture finalAdditiveTexture = (j == blurPass - 1) ? additiveTexture : null;

                    if (j == 0)
                        blur(source, tmp, horizontalBlur * scale * horiMul, verticalBlur * scale * vertiMul, finalAdditiveTexture, upsamplingCount, tints[i], intensities[i] * globalIntensity);
                    else
                    {
                        horizontalBlur = 1.0f / dest.width;
                        verticalBlur = 1.0f / dest.height;
                        if (j % 2 == 1)
                        {
                            blur(tmp, dest, horizontalBlur * scale * horiMul * 1.5f, verticalBlur * scale * vertiMul * 1.5f, finalAdditiveTexture, upsamplingCount, tints[i], intensities[i] * globalIntensity);
                            lastTargetIsTmp = false;
                        }
                        else
                        {
                            blur(dest, tmp, horizontalBlur * scale * horiMul * 1.5f, verticalBlur * scale * vertiMul * 1.5f, finalAdditiveTexture, upsamplingCount, tints[i], intensities[i] * globalIntensity);
                            lastTargetIsTmp = true;
                        }
                    }
                }
                if (lastTargetIsTmp)
                    Graphics.Blit(tmp, dest);

                if (antiJitterBuffer != null)
                    RenderTexture.ReleaseTemporary(antiJitterBuffer);


                RenderTexture.ReleaseTemporary(tmp);
                
            }

            additiveTexture = upscales[i];
        }

        RenderTexture firstFound = null;
        for (int i = 0; i < upscales.Length; ++i)
            if (upscales[i] != null)
                if (firstFound == null) firstFound = upscales[i]; 
                else RenderTexture.ReleaseTemporary(upscales[i]);

        return firstFound;
    }

    void RenderFlares(RenderTexture brightTexture, RenderTexture source,ref Texture flareRT)
    {

        flareRT = RenderTexture.GetTemporary(source.width, source.height, 0, m_Format);
        flareRT.filterMode = FilterMode.Bilinear;
        m_FlareMaterial.SetVector("_FlareScales", m_FlareScales * m_FlareGlobalScale);
        m_FlareMaterial.SetVector("_FlareScalesNear", m_FlareScalesNear * m_FlareGlobalScale);
        m_FlareMaterial.SetVector("_FlareTint0", m_FlareTint0);
        m_FlareMaterial.SetVector("_FlareTint1", m_FlareTint1);
        m_FlareMaterial.SetVector("_FlareTint2", m_FlareTint2);
        m_FlareMaterial.SetVector("_FlareTint3", m_FlareTint3);
        m_FlareMaterial.SetVector("_FlareTint4", m_FlareTint4);
        m_FlareMaterial.SetVector("_FlareTint5", m_FlareTint5);
        m_FlareMaterial.SetVector("_FlareTint6", m_FlareTint6);
        m_FlareMaterial.SetVector("_FlareTint7", m_FlareTint7);
        m_FlareMaterial.SetFloat("_Intensity", m_FlareIntensity);
        //Graphics.Blit(brightTexture, (RenderTexture)flareRT, m_BloomMaterial, 8);
        if (m_FlareRendering == FlareRendering.Sharp)
        {
            RenderTexture HalfTmp = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, m_Format);
            HalfTmp.filterMode = FilterMode.Bilinear;

            RenderSimple(brightTexture, HalfTmp, 1.0f / brightTexture.width, 1.0f / brightTexture.height, SimpleSampleCount.Four);

            Graphics.Blit(HalfTmp, (RenderTexture)flareRT, m_FlareMaterial, 0);

            RenderTexture.ReleaseTemporary(HalfTmp);

            return;
        }

        // Blur flare

        if (m_FlareBlurQuality == FlareBlurQuality.Fast)
        {
            RenderTexture HalfFlareRT = RenderTexture.GetTemporary(brightTexture.width / 2, brightTexture.height / 2, 0, m_Format);
            HalfFlareRT.filterMode = FilterMode.Bilinear;

            RenderTexture QuarterFlareRT = RenderTexture.GetTemporary(brightTexture.width / 4, brightTexture.height / 4, 0, m_Format);
            QuarterFlareRT.filterMode = FilterMode.Bilinear;

            Graphics.Blit(brightTexture, HalfFlareRT, m_FlareMaterial, 0);

            if (m_FlareRendering == FlareRendering.Blurred)
            {
                GaussianBlurSeparate(HalfFlareRT, (RenderTexture)QuarterFlareRT, 1.0f / HalfFlareRT.width, 1.0f / HalfFlareRT.height, null, BlurSampleCount.Thirteen, Color.white, 1.0f);
                RenderSimple(QuarterFlareRT, (RenderTexture)flareRT, 1.0f / QuarterFlareRT.width, 1.0f / QuarterFlareRT.height, SimpleSampleCount.Four);
            }
            else if (m_FlareRendering == FlareRendering.MoreBlurred)
            {
                GaussianBlurSeparate(HalfFlareRT, (RenderTexture)QuarterFlareRT, 1.0f / HalfFlareRT.width, 1.0f / HalfFlareRT.height, null, BlurSampleCount.ThrirtyOne, Color.white, 1.0f);
                RenderSimple(QuarterFlareRT, (RenderTexture)flareRT, 1.0f / QuarterFlareRT.width, 1.0f / QuarterFlareRT.height, SimpleSampleCount.Four);
            }
            RenderTexture.ReleaseTemporary(HalfFlareRT);
            RenderTexture.ReleaseTemporary(QuarterFlareRT);
            return;
        }
        else if (m_FlareBlurQuality == FlareBlurQuality.Normal)
        {
            RenderTexture HalfFlareRT = RenderTexture.GetTemporary(brightTexture.width / 2, brightTexture.height / 2, 0, m_Format);
            HalfFlareRT.filterMode = FilterMode.Bilinear;

            RenderTexture QuarterFlareRT = RenderTexture.GetTemporary(brightTexture.width / 4, brightTexture.height / 4, 0, m_Format);
            QuarterFlareRT.filterMode = FilterMode.Bilinear;

            RenderTexture QuarterFlareRT2 = RenderTexture.GetTemporary(brightTexture.width / 4, brightTexture.height / 4, 0, m_Format);
            QuarterFlareRT2.filterMode = FilterMode.Bilinear;


            RenderSimple(brightTexture, HalfFlareRT, 1.0f / brightTexture.width, 1.0f / brightTexture.height, SimpleSampleCount.Four);
            RenderSimple(HalfFlareRT, QuarterFlareRT, 1.0f / HalfFlareRT.width, 1.0f / HalfFlareRT.height, SimpleSampleCount.Four);

            Graphics.Blit(QuarterFlareRT, QuarterFlareRT2, m_FlareMaterial, 0);


            if (m_FlareRendering == FlareRendering.Blurred)
            {
                GaussianBlurSeparate(QuarterFlareRT2, (RenderTexture)QuarterFlareRT, 1.0f / QuarterFlareRT.width, 1.0f / QuarterFlareRT.height, null, BlurSampleCount.Thirteen, Color.white, 1.0f);
                RenderSimple(QuarterFlareRT, (RenderTexture)flareRT, 1.0f / QuarterFlareRT.width, 1.0f / QuarterFlareRT.height, SimpleSampleCount.Four);
            }
            else if (m_FlareRendering == FlareRendering.MoreBlurred)
            {
                GaussianBlurSeparate(QuarterFlareRT2, (RenderTexture)QuarterFlareRT, 1.0f / QuarterFlareRT.width, 1.0f / QuarterFlareRT.height, null, BlurSampleCount.ThrirtyOne, Color.white, 1.0f);
                RenderSimple(QuarterFlareRT, (RenderTexture)flareRT, 1.0f / QuarterFlareRT.width, 1.0f / QuarterFlareRT.height, SimpleSampleCount.Four);
            }


            RenderTexture.ReleaseTemporary(HalfFlareRT);
            RenderTexture.ReleaseTemporary(QuarterFlareRT);
            RenderTexture.ReleaseTemporary(QuarterFlareRT2);
        }
        else if (m_FlareBlurQuality == FlareBlurQuality.High)
        {
            RenderTexture HalfFlareRT = RenderTexture.GetTemporary(brightTexture.width / 2, brightTexture.height / 2, 0, m_Format);
            HalfFlareRT.filterMode = FilterMode.Bilinear;

            RenderTexture QuarterFlareRT = RenderTexture.GetTemporary(HalfFlareRT.width / 2, HalfFlareRT.height / 2, 0, m_Format);
            QuarterFlareRT.filterMode = FilterMode.Bilinear;

            RenderTexture HFlareRT1 = RenderTexture.GetTemporary(QuarterFlareRT.width / 2, QuarterFlareRT.height / 2, 0, m_Format);
            HFlareRT1.filterMode = FilterMode.Bilinear;

            RenderTexture HFlareRT2 = RenderTexture.GetTemporary(QuarterFlareRT.width / 2, QuarterFlareRT.height / 2, 0, m_Format);
            HFlareRT2.filterMode = FilterMode.Bilinear;


            RenderSimple(brightTexture, HalfFlareRT, 1.0f / brightTexture.width, 1.0f / brightTexture.height, SimpleSampleCount.Four);
            RenderSimple(HalfFlareRT, QuarterFlareRT, 1.0f / HalfFlareRT.width, 1.0f / HalfFlareRT.height, SimpleSampleCount.Four);
            RenderSimple(QuarterFlareRT, HFlareRT1, 1.0f / QuarterFlareRT.width, 1.0f / QuarterFlareRT.height, SimpleSampleCount.Four);

            Graphics.Blit(HFlareRT1, HFlareRT2, m_FlareMaterial, 0);


            if (m_FlareRendering == FlareRendering.Blurred)
            {
                GaussianBlurSeparate(HFlareRT2, (RenderTexture)HFlareRT1, 1.0f / HFlareRT1.width, 1.0f / HFlareRT1.height, null, BlurSampleCount.Thirteen, Color.white, 1.0f);
                RenderSimple(HFlareRT1, (RenderTexture)flareRT, 1.0f / HFlareRT1.width, 1.0f / HFlareRT1.height, SimpleSampleCount.Four);
            }
            else if (m_FlareRendering == FlareRendering.MoreBlurred)
            {
                GaussianBlurSeparate(HFlareRT2, (RenderTexture)HFlareRT1, 1.0f / HFlareRT1.width, 1.0f / HFlareRT1.height, null, BlurSampleCount.ThrirtyOne, Color.white, 1.0f);
                RenderSimple(HFlareRT1, (RenderTexture)flareRT, 1.0f / HFlareRT1.width, 1.0f / HFlareRT1.height, SimpleSampleCount.Four);
            }


            RenderTexture.ReleaseTemporary(HalfFlareRT);
            RenderTexture.ReleaseTemporary(QuarterFlareRT);
            RenderTexture.ReleaseTemporary(HFlareRT1);
            RenderTexture.ReleaseTemporary(HFlareRT2);
        }

        
        
    }

    RenderTexture m_LastBloomUpsample;

    void CachedUpsample(RenderTexture[] sources, RenderTexture[] destinations, int originalWidth, int originalHeight, BlurSampleCount upsamplingCount)
    {
        RenderTexture lastUpsample = null;

        for (int i = 0; i < m_UpSamples.Length; ++i)
            m_UpSamples[i] = null;

        for (int i = destinations.Length - 1; i >= 0; --i)
        {
            if (m_BloomUsages[i] || !m_DirectUpsample)
            {
                m_UpSamples[i] = RenderTexture.GetTemporary(originalWidth / (int)Mathf.Pow(2.0f, i), originalHeight / (int)Mathf.Pow(2.0f, i), 0, m_Format);
                m_UpSamples[i].filterMode = FilterMode.Bilinear;
            }

            float mul = 1.0f;

            if (m_BloomUsages[i])
            {
                float horizontalBlur = 1.0f / sources[i].width;
                float verticalBlur = 1.0f / sources[i].height;

                GaussianBlurSeparate(m_DownSamples[i], m_UpSamples[i], horizontalBlur * mul, verticalBlur, lastUpsample, upsamplingCount, m_BloomColors[i], m_BloomIntensities[i]);
            }
            else
            {
                if (i < m_DownscaleCount - 1)
                {
                    if (!m_DirectUpsample)
                        RenderSimple(lastUpsample, m_UpSamples[i], 1.0f / m_UpSamples[i].width, 1.0f / m_UpSamples[i].height, SimpleSampleCount.Four);
                    //Graphics.Blit(m_UpSamples[i + 1], m_UpSamples[i]);
                }
                else
                    Graphics.Blit(Texture2D.blackTexture, m_UpSamples[i]);
            }

            if (m_BloomUsages[i] || !m_DirectUpsample)
                lastUpsample = m_UpSamples[i];
        }

        m_LastBloomUpsample = lastUpsample;
    }

    void CachedDownsample(RenderTexture source, RenderTexture[] destinations, DeluxeFilmicCurve intensityCurve)
    {
        int downscaleCount = destinations.Length;

        RenderTexture currentSource = source;

        bool filmicCurveDone = false;

        for (int i = 0; i < downscaleCount; ++i)
        {
            if (m_DirectDownSample)
            {
                if (!m_BufferUsage[i])
                    continue;
            }

            destinations[i] = RenderTexture.GetTemporary(source.width / (int)Mathf.Pow(2.0f, (i + 1)), source.height / (int)Mathf.Pow(2.0f, (i + 1)), 0, m_Format);
            destinations[i].filterMode = FilterMode.Bilinear;

            RenderTexture dest = destinations[i];
            float dist = 1.0f;

            float horizontalBlur = 1.0f / currentSource.width;
            float verticalBlur = 1.0f / currentSource.height;

            // Render previous into next
            {
                if (intensityCurve != null && !filmicCurveDone)
                {
                    intensityCurve.StoreK();
                    m_SamplingMaterial.SetFloat("_K", intensityCurve.m_k);
                    m_SamplingMaterial.SetFloat("_Crossover", intensityCurve.m_CrossOverPoint);
                    m_SamplingMaterial.SetVector("_Toe", intensityCurve.m_ToeCoef);
                    m_SamplingMaterial.SetVector("_Shoulder", intensityCurve.m_ShoulderCoef);
                    m_SamplingMaterial.SetFloat("_MaxValue", intensityCurve.m_WhitePoint);
                    horizontalBlur = 1.0f / currentSource.width;
                    verticalBlur = 1.0f / currentSource.height;

                    if (m_TemporalStableDownsampling)
                        RenderSimple(currentSource, dest, horizontalBlur * dist, verticalBlur * dist, SimpleSampleCount.ThirteenTemporalCurve);
                    else
                        RenderSimple(currentSource, dest, horizontalBlur * dist, verticalBlur * dist, SimpleSampleCount.FourCurve);
                    filmicCurveDone = true;
                }
                else
                {
                    if (m_TemporalStableDownsampling)
                        RenderSimple(currentSource, dest, horizontalBlur * dist, verticalBlur * dist, SimpleSampleCount.ThirteenTemporal);
                    else
                        RenderSimple(currentSource, dest, horizontalBlur * dist, verticalBlur * dist, SimpleSampleCount.Four);
                }
            }


            currentSource = destinations[i];
            
        }
    }


    void BrightPass(RenderTexture source, RenderTexture destination, Vector4 treshold)
    {
        m_BrightpassMaterial.SetTexture("_MaskTex", Texture2D.whiteTexture);
        m_BrightpassMaterial.SetVector("_Threshhold", treshold);
        Graphics.Blit(source, destination, m_BrightpassMaterial, 0);
    }

    void BrightPassWithMask(RenderTexture source, RenderTexture destination, Vector4 treshold, Texture mask)
    {
        m_BrightpassMaterial.SetTexture("_MaskTex", mask);
        m_BrightpassMaterial.SetVector("_Threshhold", treshold);
        Graphics.Blit(source, destination, m_BrightpassMaterial, 0);
    }

    void RenderSimple(RenderTexture source, RenderTexture destination, float horizontalBlur, float verticalBlur, SimpleSampleCount sampleCount)
    {
        m_SamplingMaterial.SetVector("_OffsetInfos", new Vector4(horizontalBlur, verticalBlur, 0, 0));

        if (sampleCount == SimpleSampleCount.Four)
            Graphics.Blit(source, destination, m_SamplingMaterial, 0);
        else if (sampleCount == SimpleSampleCount.Nine)
            Graphics.Blit(source, destination, m_SamplingMaterial, 1);
        else if (sampleCount == SimpleSampleCount.FourCurve)
            Graphics.Blit(source, destination, m_SamplingMaterial, 5);
        else if (sampleCount == SimpleSampleCount.ThirteenTemporal)
            Graphics.Blit(source, destination, m_SamplingMaterial, 11);
        else if (sampleCount == SimpleSampleCount.ThirteenTemporalCurve)
            Graphics.Blit(source, destination, m_SamplingMaterial, 12);
    }

    void GaussianBlur1(RenderTexture source, RenderTexture destination, float horizontalBlur, float verticalBlur, RenderTexture additiveTexture, BlurSampleCount sampleCount, Color tint, float intensity)
    {
        int passFromSamples = 2;
        if (sampleCount == BlurSampleCount.Seventeen)
            passFromSamples = 3;
        if (sampleCount == BlurSampleCount.Nine)
            passFromSamples = 4;
        if (sampleCount == BlurSampleCount.NineCurve)
            passFromSamples = 6;
        if (sampleCount == BlurSampleCount.FourSimple)
            passFromSamples = 7;
        if (sampleCount == BlurSampleCount.Thirteen)
            passFromSamples = 8;
        if (sampleCount == BlurSampleCount.TwentyThree)
            passFromSamples = 9;
        if (sampleCount == BlurSampleCount.TwentySeven)
            passFromSamples = 10;

        Texture additiveColor = null;
        if (additiveTexture == null)
            additiveColor = Texture2D.blackTexture;
        else
            additiveColor = additiveTexture;

        m_SamplingMaterial.SetTexture("_AdditiveTexture", additiveColor);
        m_SamplingMaterial.SetVector("_OffsetInfos", new Vector4(horizontalBlur, verticalBlur, 0, 0));
        m_SamplingMaterial.SetVector("_Tint", tint);
        m_SamplingMaterial.SetFloat("_Intensity", intensity);
        Graphics.Blit(source, destination, m_SamplingMaterial, passFromSamples);

    }

    void GaussianBlur2(RenderTexture source, RenderTexture destination, float horizontalBlur, float verticalBlur, RenderTexture additiveTexture, BlurSampleCount sampleCount, Color tint, float intensity)
    {
        RenderTexture tmpTexture = RenderTexture.GetTemporary(destination.width, destination.height, destination.depth, destination.format);
        tmpTexture.filterMode = FilterMode.Bilinear;

        int passFromSamples = 2;
        if (sampleCount == BlurSampleCount.Seventeen)
            passFromSamples = 3;
        if (sampleCount == BlurSampleCount.Nine)
            passFromSamples = 4;
        if (sampleCount == BlurSampleCount.NineCurve)
            passFromSamples = 6;
        if (sampleCount == BlurSampleCount.FourSimple)
            passFromSamples = 7;
        if (sampleCount == BlurSampleCount.Thirteen)
            passFromSamples = 8;
        if (sampleCount == BlurSampleCount.TwentyThree)
            passFromSamples = 9;
        if (sampleCount == BlurSampleCount.TwentySeven)
            passFromSamples = 10;

        Texture additiveColor = null;

        if (additiveTexture == null)
            additiveColor = Texture2D.blackTexture;
        else
            additiveColor = additiveTexture;

        // First pass
        m_SamplingMaterial.SetTexture("_AdditiveTexture", additiveColor);
        m_SamplingMaterial.SetVector("_OffsetInfos", new Vector4(horizontalBlur, verticalBlur, 0, 0));
        m_SamplingMaterial.SetVector("_Tint", tint);
        m_SamplingMaterial.SetFloat("_Intensity", intensity);
        Graphics.Blit(source, tmpTexture, m_SamplingMaterial, passFromSamples);

        additiveColor = tmpTexture;

        // Second pass
        m_SamplingMaterial.SetTexture("_AdditiveTexture", additiveColor);
        m_SamplingMaterial.SetVector("_OffsetInfos", new Vector4(-horizontalBlur, verticalBlur, 0, 0));
        m_SamplingMaterial.SetVector("_Tint", tint);
        m_SamplingMaterial.SetFloat("_Intensity", intensity);
        Graphics.Blit(source, destination, m_SamplingMaterial, passFromSamples);

        RenderTexture.ReleaseTemporary(tmpTexture);
    }

    void GaussianBlurSeparate(RenderTexture source, RenderTexture destination, float horizontalBlur, float verticalBlur, RenderTexture additiveTexture, BlurSampleCount sampleCount, Color tint, float intensity)
    {
        RenderTexture tmpTexture = RenderTexture.GetTemporary(destination.width, destination.height, destination.depth, destination.format);
        tmpTexture.filterMode = FilterMode.Bilinear;

        int passFromSamples = 2;
        if (sampleCount == BlurSampleCount.Seventeen)
            passFromSamples = 3;
        if (sampleCount == BlurSampleCount.Nine)
            passFromSamples = 4;
        if (sampleCount == BlurSampleCount.NineCurve)
            passFromSamples = 6;
        if (sampleCount == BlurSampleCount.FourSimple)
            passFromSamples = 7;
        if (sampleCount == BlurSampleCount.Thirteen)
            passFromSamples = 8;
        if (sampleCount == BlurSampleCount.TwentyThree)
            passFromSamples = 9;
        if (sampleCount == BlurSampleCount.TwentySeven)
            passFromSamples = 10;

        // Vertical
        m_SamplingMaterial.SetTexture("_AdditiveTexture", Texture2D.blackTexture);
        m_SamplingMaterial.SetVector("_OffsetInfos", new Vector4(0.0f, verticalBlur, 0, 0));
        m_SamplingMaterial.SetVector("_Tint", tint);
        m_SamplingMaterial.SetFloat("_Intensity", intensity);
        Graphics.Blit(source, tmpTexture, m_SamplingMaterial, passFromSamples);

        Texture additiveColor = null;
        if (additiveTexture == null)
            additiveColor = Texture2D.blackTexture;
        else
            additiveColor = additiveTexture;

        // Horizontal
        m_SamplingMaterial.SetTexture("_AdditiveTexture", additiveColor);
        m_SamplingMaterial.SetVector("_OffsetInfos", new Vector4(horizontalBlur, 0.0f, 1.0f / destination.width, 1.0f / destination.height));
        m_SamplingMaterial.SetVector("_Tint", Color.white);
        m_SamplingMaterial.SetFloat("_Intensity", 1.0f);
        Graphics.Blit(tmpTexture, destination, m_SamplingMaterial, passFromSamples);

        RenderTexture.ReleaseTemporary(tmpTexture);
    }

    void RenderTextureAdditive(RenderTexture source, RenderTexture destination, float intensity)
    {

        RenderTexture tmpTexture = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format);
        Graphics.Blit(destination, tmpTexture);

        m_MixerMaterial.SetTexture("_ColorBuffer", tmpTexture);
        m_MixerMaterial.SetFloat("_Intensity", intensity);

        Graphics.Blit(source, destination, m_MixerMaterial, 0);

        RenderTexture.ReleaseTemporary(tmpTexture);
    }

    void BlitIntensity(RenderTexture source, RenderTexture destination, float intensity)
    {
        m_MixerMaterial.SetFloat("_Intensity", intensity);
        Graphics.Blit(source, destination, m_MixerMaterial, 2);
    }

    void CombineAdditive(RenderTexture source, RenderTexture destination, float intensitySource, float intensityDestination)
    {
        RenderTexture tmpTexture = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format);
        Graphics.Blit(destination, tmpTexture);

        m_MixerMaterial.SetTexture("_ColorBuffer", tmpTexture);
        m_MixerMaterial.SetFloat("_Intensity0", intensitySource);
        m_MixerMaterial.SetFloat("_Intensity1", intensityDestination);
        Graphics.Blit(source, destination, m_MixerMaterial, 1);

        RenderTexture.ReleaseTemporary(tmpTexture);
    }



   /* float Gaussian(float Scale, int iSamplePoint)
    {
        float sigma = (Scale - 1.0f) / 6.5f;
        float g = 1.0f / Mathf.Sqrt(2.0f * 3.14159f * sigma * sigma);
        return (g * Mathf.Exp(-(iSamplePoint * iSamplePoint) / (2 * sigma * sigma)));
    }

    void Start()
    {
        int count = 16;
        float scale = 31;
        float sum = 0.0f;
        float[] weights = new float[count];
        for (int i = 0; i < count; ++i)
        {
            weights[i] = Gaussian(scale, i);
            sum += weights[i];
        }

        string str = "";
        for (int i = 0; i < count; ++i)
        {
            float nWeight = weights[i] / sum;

            if (i == 0)
            {
                 str += "color += " + nWeight + " * tex2D (_MainTex, gUV);\n";
            }
            else
            {
                str += "color += " + nWeight + " * tex2D (_MainTex, gUV + _OffsetInfos.xy * " + i + ");\n";
                str += "color += " + nWeight + " * tex2D (_MainTex, gUV - _OffsetInfos.xy * " + i + ");\n";
            }
            //Debug.Log("" + i + ": " + nWeight);
        }

        Debug.Log(str);
    }
    * */
}


