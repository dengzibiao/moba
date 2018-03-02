// SSAO Pro - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com

#if (UNITY_4_5 || UNITY_4_6)
#define UNITY_4_X
#else
#define UNITY_5_X
#endif

using UnityEngine;
using SSAOProUtils;

[HelpURL("http://www.thomashourdel.com/ssaopro/doc/")]
[ExecuteInEditMode, AddComponentMenu("Image Effects/SSAO Pro")]
[RequireComponent(typeof(Camera))]
public class SSAOPro : MonoBehaviour
{
	public enum BlurMode
	{
		None,
		Gaussian,
		Bilateral,
		HighQualityBilateral
	}

	public enum SampleCount
	{
		VeryLow,
		Low,
		Medium,
		High,
		Ultra
	}

	public Texture2D NoiseTexture;

	public bool UseHighPrecisionDepthMap = false;

	public SampleCount Samples = SampleCount.Medium;

	[Range(1, 4)]
	public int Downsampling = 1;

	[Range(0.01f, 1.25f)]
	public float Radius = 0.125f;

	[Range(0f, 16f)]
	public float Intensity = 2f;

	[Range(0f, 10f)]
	public float Distance = 1f;

	[Range(0f, 1f)]
	public float Bias = 0.1f;

	[Range(0f, 1f)]
	public float LumContribution = 0.5f;

	[ColorUsage(false)]
	public Color OcclusionColor = Color.black;

	public float CutoffDistance = 150f;
	public float CutoffFalloff = 50f;

	public BlurMode Blur = BlurMode.None;
	public bool BlurDownsampling = false;

	[Range(1, 4)]
	public int BlurPasses = 1;

	[Range(0.05f, 1f)]
	public float BlurBilateralThreshold = 0.1f;

	public bool DebugAO = false;

	protected Shader m_ShaderSSAO_v2;
	protected Shader m_ShaderHighPrecisionDepth;
	protected Material m_Material_v2;
	protected Camera m_Camera;
	protected Camera m_RWSCamera;
	protected RenderTextureFormat m_RTFormat = RenderTextureFormat.RFloat;

	public Material Material
	{
		get
		{
			if (m_Material_v2 == null)
			{
				m_Material_v2 = new Material(ShaderSSAO);
				m_Material_v2.hideFlags = HideFlags.HideAndDontSave;
			}

			return m_Material_v2;
		}
	}

	public Shader ShaderSSAO
	{
		get
		{
			if (m_ShaderSSAO_v2 == null)
				m_ShaderSSAO_v2 = Shader.Find("Hidden/SSAO Pro V2");

			return m_ShaderSSAO_v2;
		}
	}

#if UNITY_4_X
	public Shader ShaderHighPrecisionDepth
	{
		get
		{
			if (m_ShaderHighPrecisionDepth == null)
				m_ShaderHighPrecisionDepth = Shader.Find("Hidden/SSAO Pro - High Precision Depth Map");

			return m_ShaderHighPrecisionDepth;
		}
	}
#endif

	void Start()
	{
		// Disable if we don't support image effects
		if (!SystemInfo.supportsImageEffects)
		{
			Debug.LogWarning("Image Effects are not supported on this platform.");
			enabled = false;
			return;
		}

		// Disable if we don't support render textures
		if (SystemInfo.supportsRenderTextures)
		{
#if UNITY_4_X
			if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RFloat))
			{
				if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
				{
					Debug.LogWarning("RFloat && Depth RenderTextures are not supported on this platform.");
					enabled = false;
					return;
				}

				m_RTFormat = RenderTextureFormat.Depth;
			}
#endif
		}
		else
		{
			Debug.LogWarning("RenderTextures are not supported on this platform.");
			enabled = false;
			return;
		}

		// Disable the image effect if the shaders can't run on the users graphics card
		if (ShaderSSAO != null && !ShaderSSAO.isSupported)
		{
			Debug.LogWarning("Unsupported shader (SSAO).");
			enabled = false;
			return;
		}

#if UNITY_4_X
		if (ShaderHighPrecisionDepth != null && !ShaderHighPrecisionDepth.isSupported)
		{
			Debug.LogWarning("Unsupported shader (High Precision Depth Map).");
			enabled = false;
			return;
		}
#endif
	}

	void OnEnable()
	{
		m_Camera = GetComponent<Camera>();
	}

	void OnDestroy()
	{
		if (m_Material_v2 != null)
			DestroyImmediate(m_Material_v2);

		if (m_RWSCamera != null)
			DestroyImmediate(m_RWSCamera.gameObject);
	}

#if UNITY_4_X
	void OnPreRender()
	{
		if (!UseHighPrecisionDepthMap)
			return;
		
		// Create the camera used to generate the alternate depth map
		if (m_RWSCamera == null)
		{
			GameObject go = new GameObject("Depth Normal Camera", typeof(Camera));
			go.hideFlags = HideFlags.HideAndDontSave;
			m_RWSCamera = go.GetComponent<Camera>();
			m_RWSCamera.CopyFrom(m_Camera);
			m_RWSCamera.renderingPath = RenderingPath.Forward;
			m_RWSCamera.clearFlags = CameraClearFlags.Color;
			m_RWSCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
			m_RWSCamera.enabled = false;
		}

		// Render depth & normals to a custom Float RenderTexture
		m_RWSCamera.CopyFrom(m_Camera);
		m_RWSCamera.rect = new Rect(0f, 0f, 1f, 1f);
		m_RWSCamera.renderingPath = RenderingPath.Forward;
		m_RWSCamera.clearFlags = CameraClearFlags.Color;
		m_RWSCamera.backgroundColor = new Color(1f, 1f, 1f, 1f);
		m_RWSCamera.farClipPlane = CutoffDistance;

		RenderTexture rt = RenderTexture.GetTemporary((int)m_Camera.pixelWidth, (int)m_Camera.pixelHeight, 24, m_RTFormat);
		rt.filterMode = FilterMode.Bilinear;
		m_RWSCamera.targetTexture = rt;
		m_RWSCamera.RenderWithShader(m_ShaderHighPrecisionDepth, "RenderType");
		rt.SetGlobalShaderProperty("_DepthNormalMapF32");
		m_RWSCamera.targetTexture = null;
		RenderTexture.ReleaseTemporary(rt);
	}
#endif

	[ImageEffectOpaque]
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		// Fail checks
		if (ShaderSSAO == null)
		{
			Graphics.Blit(source, destination);
			return;
		}

#if UNITY_4_X
		if (ShaderHighPrecisionDepth == null && UseHighPrecisionDepthMap)
		{
			Graphics.Blit(source, destination);
			return;
		}
#endif

		// Shader keywords & pass ID
		int ssaoPass = SetShaderStates();

		// Uniforms
		Material.SetMatrix("_InverseViewProject", (m_Camera.projectionMatrix * m_Camera.worldToCameraMatrix).inverse);
		Material.SetMatrix("_CameraModelView", m_Camera.cameraToWorldMatrix);
		Material.SetTexture("_NoiseTex", NoiseTexture);
		Material.SetVector("_Params1", new Vector4(NoiseTexture == null ? 0f : NoiseTexture.width, Radius, Intensity, Distance));
		Material.SetVector("_Params2", new Vector4(Bias, LumContribution, CutoffDistance, CutoffFalloff));
		Material.SetColor("_OcclusionColor", OcclusionColor);

		// Render !
		if (Blur == BlurMode.None)
		{
			RenderTexture rt = RenderTexture.GetTemporary(source.width / Downsampling, source.height / Downsampling, 0, RenderTextureFormat.ARGB32);
			Graphics.Blit(rt, rt, Material, 0); // Clear

			if (DebugAO)
			{
				Graphics.Blit(source, rt, Material, ssaoPass);
				Graphics.Blit(rt, destination);
				RenderTexture.ReleaseTemporary(rt);
				return;
			}

			Graphics.Blit(source, rt, Material, ssaoPass);
			Material.SetTexture("_SSAOTex", rt);
			Graphics.Blit(source, destination, Material, 8);
			RenderTexture.ReleaseTemporary(rt);
		}
		else
		{
			// Pass ID
			int blurPass = 5;

			if (Blur == BlurMode.Bilateral)
				blurPass = 6;
			else if (Blur == BlurMode.HighQualityBilateral)
				blurPass = 7;

			// Prep work
			int d = BlurDownsampling ? Downsampling : 1;
			RenderTexture rt1 = RenderTexture.GetTemporary(source.width / d, source.height / d, 0, RenderTextureFormat.ARGB32);
			RenderTexture rt2 = RenderTexture.GetTemporary(source.width / Downsampling, source.height / Downsampling, 0, RenderTextureFormat.ARGB32);
			Graphics.Blit(rt1, rt1, Material, 0); // Clear

			// SSAO
			Graphics.Blit(source, rt1, Material, ssaoPass);

			if (Blur == BlurMode.HighQualityBilateral)
				Material.SetFloat("_BilateralThreshold", BlurBilateralThreshold / 10000);

			for (int i = 0; i < BlurPasses; i++)
			{
				// Horizontal blur
				Material.SetVector("_Direction", new Vector2(1f / source.width, 0f));
				Graphics.Blit(rt1, rt2, Material, blurPass);

				// Vertical blur
				Material.SetVector("_Direction", new Vector2(0f, 1f / source.height));
				Graphics.Blit(rt2, rt1, Material, blurPass);
			}

			if (!DebugAO)
			{
				Material.SetTexture("_SSAOTex", rt1);
				Graphics.Blit(source, destination, Material, 8);
			}
			else
			{
				Graphics.Blit(rt1, destination);
			}

			RenderTexture.ReleaseTemporary(rt1);
			RenderTexture.ReleaseTemporary(rt2);
		}
	}

	// State switching
	private string[] keywords = new string[2];

	int SetShaderStates()
	{
		// Depth & normal maps
		m_Camera.depthTextureMode |= DepthTextureMode.Depth;
		m_Camera.depthTextureMode |= DepthTextureMode.DepthNormals;

		// Shader keywords
		keywords[0] = (Samples == SampleCount.Low) ? "SAMPLES_LOW"
					: (Samples == SampleCount.Medium) ? "SAMPLES_MEDIUM"
					: (Samples == SampleCount.High) ? "SAMPLES_HIGH"
					: (Samples == SampleCount.Ultra) ? "SAMPLES_ULTRA"
					: "SAMPLES_VERY_LOW";

#if UNITY_4_X
		keywords[1] = (UseHighPrecisionDepthMap) ? "HIGH_PRECISION_DEPTHMAP_ON" : "HIGH_PRECISION_DEPTHMAP_OFF";
#else
		keywords[1] = "HIGH_PRECISION_DEPTHMAP_OFF";
#endif

		Material.shaderKeywords = keywords;

		// SSAO pass ID
		int pass = 0;

		if (NoiseTexture != null)
			pass = 1;

		if (LumContribution >= 0.001f)
			pass += 2;

		return 1 + pass;
	}
}
