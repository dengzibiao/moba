using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

namespace ScionEngine
{
	public abstract class ScionPostProcessBase : MonoBehaviour 
	{					
		[Inspector.Decorations.Header(0, "Grain")]
		[Inspector.Toggle("Active", useProperty = "grain", tooltip = "Determines if grain is used")]
		[SerializeField] protected bool m_grain = true;
		protected bool ShowGrain() { return m_grain; }
		[Inspector.Slider("Intensity", useProperty = "grainIntensity", visibleCheck = "ShowGrain", minValue = 0.0f, maxValue = 1.0f, tooltip = "How strong the grain effect is")]
		[SerializeField] protected float m_grainIntensity = 0.1f;	
		
		[Inspector.Decorations.Header(0, "Vignette")]	
		[Inspector.Toggle("Active", useProperty = "vignette", tooltip = "Determines if vignette is used")]
		[SerializeField] protected bool m_vignette = true;
		protected bool ShowVignette() { return m_vignette; }
		[Inspector.Slider("Intensity", useProperty = "vignetteIntensity", visibleCheck = "ShowVignette", minValue = 0.0f, maxValue = 1.0f, tooltip = "How strong the vignette effect is")]
		[SerializeField] protected float m_vignetteIntensity = 0.7f;
		[Inspector.Slider("Scale", useProperty = "vignetteScale", visibleCheck = "ShowVignette", minValue = 0.0f, maxValue = 1.0f, tooltip = "How much of the screen is affected")]
		[SerializeField] protected float m_vignetteScale = 0.7f;
		[Inspector.Field("Color", useProperty = "vignetteColor", visibleCheck = "ShowVignette", tooltip = "What color the vignette effect has")]
		[SerializeField] protected Color m_vignetteColor = Color.black;
		
		[Inspector.Decorations.Header(0, "Chromatic Aberration")]
		[Inspector.Toggle("Active", useProperty = "chromaticAberration", tooltip = "Determines if chromatic aberration is used")]
		[SerializeField] protected bool m_chromaticAberration = true;
		protected bool ShowChromaticAberration() { return m_chromaticAberration; }
		[Inspector.Slider("Distortion Scale", useProperty = "chromaticAberrationDistortion", visibleCheck = "ShowChromaticAberration", minValue = 0.0f, maxValue = 1.0f, tooltip = "How much of the screen is affected")]
		[SerializeField] protected float m_chromaticAberrationDistortion = 0.5f;
		[Inspector.Slider("Intensity", useProperty = "chromaticAberrationIntensity", visibleCheck = "ShowChromaticAberration", minValue = -30.0f, maxValue = 30.0f, tooltip = "How strong the distortion effect is")]
		[SerializeField] protected float m_chromaticAberrationIntensity = 10.0f;
		
		[Inspector.Decorations.Header(0, "Bloom")]
		[Inspector.Toggle("Active", useProperty = "bloom", tooltip = "Determines if bloom is used")]
		[SerializeField] protected bool m_bloom = true;
		protected bool ShowBloom() { return bloom; }
		[Inspector.Slider("Intensity", useProperty = "bloomIntensity", visibleCheck = "ShowBloom", minValue = 0.0f, maxValue = 1.0f, tooltip = "How strong the bloom effect is")]
		[SerializeField] protected float m_bloomIntensity = 0.35f;
		[Inspector.Slider("Brightness", useProperty = "bloomBrightness", visibleCheck = "ShowBloom", minValue = 0.25f, maxValue = 4.0f, tooltip = "How bright the bloom effect is")]
		[SerializeField] protected float m_bloomBrightness = 1.2f;
		[Inspector.Slider("Range", useProperty = "bloomDistanceMultiplier", visibleCheck = "ShowBloom", minValue = 0.25f, maxValue = 1.25f, tooltip = "Modifies the range of the bloom")]
		[SerializeField] protected float m_bloomDistanceMultiplier = 1.0f;
		
		[Inspector.Slider("Downsamples", useProperty = "bloomDownsamples", visibleCheck = "ShowBloom", minValue = 3.0f, maxValue = 9.0f, tooltip = "Number of downsamples")]
		[SerializeField] protected int m_bloomDownsamples = 7;


		[Inspector.Decorations.Header(0, "Lens Flare")]
		[Inspector.Toggle("Active", useProperty = "lensFlare", tooltip = "Determines if lens flares are used")]
		[SerializeField] protected bool m_lensFlare = true;
		protected bool ShowLensFlare() { return m_lensFlare; }

		[Inspector.Field("Ghost Samples", useProperty = "lensFlareGhostSamples", visibleCheck = "ShowLensFlare", tooltip = "The number of samples used for ghosting")]
		[SerializeField] protected LensFlareGhostSamples m_lensFlareGhostSamples = LensFlareGhostSamples.x3;
		[Inspector.Slider("Ghost Intensity", useProperty = "lensFlareGhostIntensity", visibleCheck = "ShowLensFlare", minValue = 0.0f, maxValue = 2.0f, tooltip = "The intensity of the lens ghosts")]
		[SerializeField] protected float m_lensFlareGhostIntensity = 0.1f;
		[Inspector.Slider("Ghost Dispersal", useProperty = "lensFlareGhostDispersal", visibleCheck = "ShowLensFlare", minValue = 0.01f, maxValue = 1.0f, tooltip = "How spread out the ghost samples are")]
		[SerializeField] protected float m_lensFlareGhostDispersal = 0.2f;
		[Inspector.Slider("Ghost Distortion", useProperty = "lensFlareGhostDistortion", visibleCheck = "ShowLensFlare", minValue = 0.01f, maxValue = 1.0f, tooltip = "How much chromatic abberation is applied to the ghosts")]
		[SerializeField] protected float m_lensFlareGhostDistortion = 0.1f;
		[Inspector.Slider("Ghost Edge Fade", useProperty = "lensFlareGhostEdgeFade", visibleCheck = "ShowLensFlare", minValue = 0.0f, maxValue = 1.5f, tooltip = "High values mean bright pixels on the border of the screen will not cause ghosting")]
		[SerializeField] protected float m_lensFlareGhostEdgeFade = 1.0f;

		[Inspector.Field("Lens Color", useProperty = "lensFlareLensColorTexture", visibleCheck = "ShowLensFlare", tooltip = "A radial color texture for the lens flare effects")]
		[SerializeField] protected Texture2D m_lensFlareLensColorTexture = null;

		[Inspector.Slider("Halo Intensity", useProperty = "lensFlareHaloIntensity", visibleCheck = "ShowLensFlare", minValue = 0.0f, maxValue = 2.0f, tooltip = "The intensity of the lens flare halo")]
		[SerializeField] protected float m_lensFlareHaloIntensity = 0.1f;
		[Inspector.Slider("Halo Width", useProperty = "lensFlareHaloWidth", visibleCheck = "ShowLensFlare", minValue = 0.0f, maxValue = 0.8f, tooltip = "How far from the center of the screen the halo will appear")]
		[SerializeField] protected float m_lensFlareHaloWidth = 0.3f;
		[Inspector.Slider("Halo Distortion", useProperty = "lensFlareHaloDistortion", visibleCheck = "ShowLensFlare", minValue = 0.0f, maxValue = 1.0f, tooltip = "How much chromatic abberation is applied to the halo")]
		[SerializeField] protected float m_lensFlareHaloDistortion = 0.5f;

		[Inspector.Field("Diffraction Texture", useProperty = "lensFlareDiffractionTexture", visibleCheck = "ShowLensFlare", tooltip = "A rotating texture that the lens flare is multiplied by")]
		[SerializeField] protected Texture2D m_lensFlareDiffractionTexture = null;
		private bool ShowDiffractionUVScale() { return ShowLensFlare() == true && m_lensFlareDiffractionTexture != null; }
		[Inspector.Slider("Diffraction UV Scale", useProperty = "lensFlareDiffractionUVScale", visibleCheck = "ShowDiffractionUVScale", minValue = 0.5f, maxValue = 0.9f, tooltip = "Scales the diffraction texture so that it can rotate without the corners ending up outside the texture")]
		[SerializeField] protected float m_lensFlareDiffractionUVScale = 0.8f;
		
		[Inspector.Field("Blur Samples", useProperty = "lensFlareBlurSamples", visibleCheck = "ShowLensFlare", tooltip = "How many samples are used to blur the resulting lens flare texture")]
		[SerializeField] protected LensFlareBlurSamples m_lensFlareBlurSamples = LensFlareBlurSamples.x4;
		[Inspector.Slider("Blur Strength", useProperty = "lensFlareBlurStrength", visibleCheck = "ShowLensFlare", minValue = 0.0f, maxValue = 1.0f, tooltip = "How spread out the blur samples are")]
		[SerializeField] protected float m_lensFlareBlurStrength = 0.5f;

		[Inspector.Slider("Downsamples", useProperty = "lensFlareDownsamples", visibleCheck = "ShowLensFlare", minValue = 1.0f, maxValue = 3.0f, tooltip = "How many times the lens flares are downsampled. Higher values have better performance")]
		[SerializeField] protected int m_lensFlareDownsamples = 2;

		private bool ShowLensDirtActive() { return ShowBloom() || ShowLensFlare(); }		
		[Inspector.Decorations.Header(0, "Lens Dirt", visibleCheck = "ShowLensDirtActive")]
		[Inspector.Toggle("Active", useProperty = "lensDirt", visibleCheck = "ShowLensDirtActive", tooltip = "Determines if lens dirt is used")]
		[SerializeField] protected bool m_lensDirt = false;
		protected bool ShowLensDirt() { return ShowLensDirtActive() && lensDirt; }
		[Inspector.Field("Dirt Texture", useProperty = "lensDirtTexture", visibleCheck = "ShowLensDirt", tooltip = "The texture used as lens dirt")]
		[SerializeField] protected Texture2D m_lensDirtTexture = null;
		protected bool ShowLensDirtSettings() { return lensDirtTexture != null && ShowLensDirt(); }
		[Inspector.Slider("Bloom Effect", useProperty = "lensDirtBloomEffect", visibleCheck = "ShowLensDirtSettings", minValue = 0.0f, maxValue = 1.0f, tooltip = "How strong the lens dirt effect is")]
		[SerializeField] protected float m_lensDirtBloomEffect = 0.3f;
		[Inspector.Slider("Bloom Brightness", useProperty = "lensDirtBloomBrightness", visibleCheck = "ShowLensDirtSettings", minValue = 0.5f, maxValue = 2.0f, tooltip = "How bright the lens dirt effect is")]
		[SerializeField] protected float m_lensDirtBloomBrightness = 1.2f;
		[Inspector.Slider("Lens Flare Effect", useProperty = "lensDirtLensFlareEffect", visibleCheck = "ShowLensDirtSettings", minValue = 0.0f, maxValue = 1.0f, tooltip = "")]
		[SerializeField] protected float m_lensDirtLensFlareEffect = 1.0f;
		[Inspector.Slider("Lens Flare Brightness", useProperty = "lensDirtLensFlareBrightness", visibleCheck = "ShowLensDirtSettings", minValue = 0.5f, maxValue = 2.0f, tooltip = "How bright the lens dirt effect is")]
		[SerializeField] protected float m_lensDirtLensFlareBrightness = 1.0f;

		protected virtual bool ShowTonemapping() { return false; }

		[Inspector.Decorations.Header(0, "Tonemapping", visibleCheck = "ShowTonemapping")]
		[Inspector.Field("Mode", useProperty = "tonemappingMode", visibleCheck = "ShowTonemapping", tooltip = "What type of tonemapping algorithm is used")]
		[SerializeField] protected TonemappingMode m_tonemappingMode = TonemappingMode.Filmic;
		[Inspector.Slider("White Point", useProperty = "whitePoint", visibleCheck = "ShowTonemapping", minValue = 0.5f, maxValue = 20.0f, tooltip = "At what intensity pixels will become white")]
		[SerializeField] protected float m_whitePoint = 7.0f;
		
		//protected bool ShowCameraMode() { return m_camera.hdr == true; }
		protected bool ShowCameraMode() { return true; }
		protected bool ShowExposureComp() { return cameraMode != CameraMode.Off; }
		protected bool ShowExposureAdaption() { return cameraMode != CameraMode.Off && cameraMode != CameraMode.Manual; }
		protected bool ShowDownsampleBloomExposure() { return ShowExposureAdaption() && ShowBloom(); }
		
		[Inspector.Decorations.Header(0, "Camera Mode")]
		[Inspector.Field("Camera Mode", useProperty = "cameraMode", visibleCheck = "ShowCameraMode", tooltip = "What camera mode is used")]
		[SerializeField] protected CameraMode m_cameraMode = CameraMode.AutoPriority;
		
		protected bool ShowFocalLength() { return m_userControlledFocalLength; }
		protected bool ShowFNumber() { return cameraMode == CameraMode.AperturePriority || cameraMode == CameraMode.Manual || (cameraMode == CameraMode.Off && depthOfField == true); }
		protected bool ShowISO() { return cameraMode == CameraMode.Manual; }
		//protected bool ShowShutterSpeed() { return cameraMode == CameraMode.ShutterPriority || cameraMode == CameraMode.Manual; }
		protected bool ShowShutterSpeed() { return cameraMode == CameraMode.Manual; }
		
		[Inspector.Slider("F Number", useProperty = "fNumber", visibleCheck = "ShowFNumber", minValue = 1.0f, maxValue = 22.0f, tooltip = "The F number of the camera")]
		[SerializeField] protected float m_fNumber = 4.0f;
		[Inspector.Slider("ISO", useProperty = "ISO", visibleCheck = "ShowISO", minValue = 100.0f, maxValue = 6400.0f, tooltip = "The ISO setting of the camera")]
		[SerializeField] protected float m_ISO = 100.0f;
		[Inspector.Slider("Shutter Speed", useProperty = "shutterSpeed", visibleCheck = "ShowShutterSpeed", minValue = 1.0f/4000.0f, maxValue = 1.0f/30.0f, tooltip = "The shutted speed of the camera")]
		[SerializeField] protected float m_shutterSpeed = 0.01f;
		[Inspector.Toggle("Custom Focal Length", useProperty = "userControlledFocalLength", tooltip = "If false the focal length will instead be derived from the camera's field of view")]
		[SerializeField] protected bool m_userControlledFocalLength = false;
		[Inspector.Slider("Focal Length", useProperty = "focalLength", visibleCheck = "ShowFocalLength", minValue = 10.0f, maxValue = 250.0f, tooltip = "The focal length of the camera in millimeters")]
		[SerializeField] protected float m_focalLength = 15.0f;
		
		[Inspector.Decorations.Header(0, "Exposure Settings")]
		[Inspector.Slider("Exposure Compensation", useProperty = "exposureCompensation", visibleCheck = "ShowExposureComp", minValue = -8.0f, maxValue = 8.0f, 
		                  tooltip = "Allows you to manually compensate towards the desired exposure")]
		[SerializeField] protected float m_exposureCompensation = 0.0f;
		[Inspector.MinMaxSlider("Min Max Exposure", -16.0f, 16.0f, useProperty = "minMaxExposure")]
		[SerializeField] protected Vector2 m_minMaxExposure = new Vector2(-16.0f, 16.0f);
		[Inspector.Slider("Adaption Speed", useProperty = "adaptionSpeed", visibleCheck = "ShowExposureAdaption", minValue = 0.1f, maxValue = 8.0f, tooltip = "How fast the exposure is allowed to change")]
		[SerializeField] protected float m_adaptionSpeed = 1.0f;
		
		protected bool ShowDepthOfField() { return depthOfField; }
		protected bool ShowPointAverage() { return m_depthFocusMode == DepthFocusMode.PointAverage && ShowDepthOfField(); }
		protected bool ShowFocalDistance() { return (m_depthFocusMode == DepthFocusMode.ManualDistance || m_depthFocusMode == DepthFocusMode.ManualRange) && ShowDepthOfField(); }
		protected bool ShowFocalRange() { return m_depthFocusMode == DepthFocusMode.ManualRange && ShowDepthOfField(); }
		
		[Inspector.Decorations.Header(0, "Depth of Field")]
		[Inspector.Toggle("Active", useProperty = "depthOfField", tooltip = "Determines if depth of field is used")]
		[SerializeField] protected bool m_depthOfField = true;	
		[Tooltip("Excludes layers from the depth of field")]
		[SerializeField] protected LayerMask m_exclusionMask;
		[Inspector.Slider("Max Radius", useProperty = "maxCoCRadius", visibleCheck = "ShowDepthOfField", minValue = 8.0f, maxValue = 20.0f, tooltip = "The maximum radius the blur can be. Lower values will have less artifacts. Set this as high as you can without seeing artifacts")]
		[SerializeField] protected float m_maxCoCRadius = 14.0f;
		[Inspector.Field("Quality Level", useProperty = "depthOfFieldQuality", visibleCheck = "ShowDepthOfField", tooltip = "Dictates how many samples the algorithm does")]
		[SerializeField] protected DepthOfFieldQuality m_depthOfFieldQuality = DepthOfFieldQuality.Normal;
		[Inspector.Field("Depth Focus Mode", useProperty = "depthFocusMode", visibleCheck = "ShowDepthOfField", tooltip = "How the depth focus point is chosen")]
		[SerializeField] protected DepthFocusMode m_depthFocusMode = DepthFocusMode.PointAverage;
		[Inspector.Field("Point Center", useProperty = "pointAveragePosition", visibleCheck = "ShowPointAverage", tooltip = "Where the center of focus is on the screen." +
		                 " [0,0] is the bottom left corner and [1,1] is the top right")]
		[SerializeField] protected Vector2 m_pointAveragePosition = new Vector2(0.5f, 0.5f); 	
		[Inspector.Decorations.Space(0, 1)]
		[Inspector.Slider("Point Range", useProperty = "pointAverageRange", visibleCheck = "ShowPointAverage", minValue = 0.01f, maxValue = 1.0f, tooltip = "How far the point average calculation reaches")]
		[SerializeField] protected float m_pointAverageRange = 0.2f;
		[Inspector.Toggle("Visualize", useProperty = "visualizePointFocus", visibleCheck = "ShowPointAverage", tooltip = "Show the area of influence on the main screen for visualizaiton")]
		[SerializeField] protected bool m_visualizePointFocus = false;
		[Inspector.Slider("Adaption Speed", useProperty = "depthAdaptionSpeed", visibleCheck = "ShowPointAverage", minValue = 1.0f, maxValue = 30.0f, tooltip = "Dictates how fast the focal distance changes")]
		[SerializeField] protected float m_depthAdaptionSpeed = 15.0f;
		[Inspector.Field("Focal Distance", useProperty = "focalDistance", visibleCheck = "ShowFocalDistance", tooltip = "The focal distance in meters")]
		[SerializeField] protected float m_focalDistance = 10.0f;
		[Inspector.Slider("Depth Range", useProperty = "focalRange", visibleCheck = "ShowFocalRange", minValue = 0.0f, maxValue = 50.0f, tooltip = "The length of the range that is 100% in focus")]
		[SerializeField] protected float m_focalRange = 10.0f;

		protected bool ShowCCTex1() { return colorGradingMode == ColorGradingMode.On || colorGradingMode == ColorGradingMode.Blend; }
		protected bool ShowCCTex2() { return colorGradingMode == ColorGradingMode.Blend; }
		
		[Inspector.Decorations.Header(0, "Color Correction")]
		[Inspector.Field("Mode", useProperty = "colorGradingMode", tooltip = "Which color correction mode is currently active")]
		[SerializeField] protected ColorGradingMode m_colorGradingMode = ColorGradingMode.Off;
		[Inspector.Field("Lookup Texture", useProperty = "colorGradingTex1", visibleCheck = "ShowCCTex1", tooltip = "The lookup texture used for color correction")]
		[SerializeField] protected Texture2D m_colorGradingTex1 = null;
		[Inspector.Field("Blend Lookup Texture", useProperty = "colorGradingTex2", visibleCheck = "ShowCCTex2", tooltip = "The lookup texture blended in as the blend factor increases")]
		[SerializeField] protected Texture2D m_colorGradingTex2 = null;
		[Inspector.Slider("Blend Factor", useProperty = "colorGradingBlendFactor", visibleCheck = "ShowCCTex2", minValue = 0.0f, maxValue = 1.0f, tooltip = "Interpolates between the original color correction texture and the blend target color correction texture")]
		[SerializeField] protected float m_colorGradingBlendFactor = 0.0f;

		//Whenever this is reset by a reload, force a new fill!
		[HideInInspector][SerializeField] private bool forceFillParams = true;

		protected bool m_isFirstRender = true;
		protected float prevCamFoV;
		
		protected Camera m_camera;
		protected Transform m_cameraTransform;
		protected Bloom m_bloomClass;
		protected ScionLensFlare m_lensFlareClass; //Prefixed with Scion since it conflicts w Unitys LensFlare class
		protected VirtualCamera m_virtualCamera;
		protected CombinationPass m_combinationPass;
		protected Downsampling m_downsampling;
		protected DepthOfField m_depthOfFieldClass;
		
		protected PostProcessParameters postProcessParams = new PostProcessParameters();
		
		protected ScionDebug m_scionDebug;
		public static ScionDebug ActiveDebug;	
		
		 
		public CameraMode cameraMode 
		{
			get { return m_cameraMode; }
			set 
			{ 
				m_cameraMode = value; 
				postProcessParams.cameraParams.cameraMode = value;
				postProcessParams.exposure = value != CameraMode.Off ? true : false;
			}
		}
		public bool bloom 
		{
			get { return m_bloom; }
			set { m_bloom = value; postProcessParams.bloom = value; }
		}
		public bool lensFlare
		{
			get { return m_lensFlare; }
			set { m_lensFlare = value; postProcessParams.lensFlare = value; }
		}
		public bool lensDirt 
		{
			get { return m_lensDirt; }
			set { m_lensDirt = value; postProcessParams.lensDirt = value; }
		}
		public Texture2D lensDirtTexture 
		{
			get { return m_lensDirtTexture; }
			set { m_lensDirtTexture = value; postProcessParams.lensDirtTexture = value; }
		}
		public bool depthOfField 
		{
			get { return m_depthOfField; }
			set 
			{ 
				m_depthOfField = value; 
				postProcessParams.depthOfField = value; 
				PlatformCompatibility();
			}
		}
		
		public float bloomIntensity 
		{
			get { return m_bloomIntensity; }
			set { m_bloomIntensity = value; postProcessParams.glareParams.intensity = ScionUtility.Square(value); }
		}
		public float bloomBrightness 
		{
			get { return m_bloomBrightness; }
			set { m_bloomBrightness = value; postProcessParams.glareParams.brightness = value; }
		}
		public float bloomDistanceMultiplier
		{
			get { return m_bloomDistanceMultiplier; }
			set { m_bloomDistanceMultiplier = value; postProcessParams.glareParams.distanceMultiplier = value; }
		}
		public int bloomDownsamples 
		{
			get { return m_bloomDownsamples; }
			set { m_bloomDownsamples = value; postProcessParams.glareParams.downsamples = value; }
		}

		public LensFlareGhostSamples lensFlareGhostSamples
		{
			get { return m_lensFlareGhostSamples; }
			set { m_lensFlareGhostSamples = value; postProcessParams.lensFlareParams.ghostSamples = value; }
		}
		public float lensFlareGhostIntensity
		{
			get { return m_lensFlareGhostIntensity; }
			set { m_lensFlareGhostIntensity = value; postProcessParams.lensFlareParams.ghostIntensity = value; }
		}
		public float lensFlareGhostDispersal
		{
			get { return m_lensFlareGhostDispersal; }
			set { m_lensFlareGhostDispersal = value; postProcessParams.lensFlareParams.ghostDispersal = value; }
		}
		public float lensFlareGhostDistortion
		{
			get { return m_lensFlareGhostDistortion; }
			set { m_lensFlareGhostDistortion = value; postProcessParams.lensFlareParams.ghostDistortion = value; }
		}
		public float lensFlareGhostEdgeFade
		{
			get { return m_lensFlareGhostEdgeFade; }
			set { m_lensFlareGhostEdgeFade = value; postProcessParams.lensFlareParams.ghostEdgeFade = value; }
		}

		public float lensFlareHaloIntensity 
		{
			get { return m_lensFlareHaloIntensity; }
			set { m_lensFlareHaloIntensity = value; postProcessParams.lensFlareParams.haloIntensity = value; }
		}
		public float lensFlareHaloWidth 
		{
			get { return m_lensFlareHaloWidth; }
			set { m_lensFlareHaloWidth = value; postProcessParams.lensFlareParams.haloWidth = value; }
		}
		public float lensFlareHaloDistortion 
		{
			get { return m_lensFlareHaloDistortion; }
			set { m_lensFlareHaloDistortion = value; postProcessParams.lensFlareParams.haloDistortion = value; }
		}	

		public float lensFlareDiffractionUVScale 
		{
			get { return m_lensFlareDiffractionUVScale; }
			set { m_lensFlareDiffractionUVScale = value; postProcessParams.lensFlareParams.starUVScale = value; }
		}		
		public Texture2D lensFlareDiffractionTexture 
		{
			get { return m_lensFlareDiffractionTexture; }
			set { m_lensFlareDiffractionTexture = value; postProcessParams.lensFlareParams.starTexture = value; }
		}	

		public LensFlareBlurSamples lensFlareBlurSamples
		{
			get { return m_lensFlareBlurSamples; }
			set { m_lensFlareBlurSamples = value; postProcessParams.lensFlareParams.blurSamples = value; }
		}
		public float lensFlareBlurStrength 
		{
			get { return m_lensFlareBlurStrength; }
			set { m_lensFlareBlurStrength = value; postProcessParams.lensFlareParams.blurStrength = value; }
		}

		public int lensFlareDownsamples
		{
			get { return m_lensFlareDownsamples; }
			set { m_lensFlareDownsamples = value; postProcessParams.lensFlareParams.downsamples = value; }
		}
		public Texture2D lensFlareLensColorTexture
		{
			get { return m_lensFlareLensColorTexture; }
			set { m_lensFlareLensColorTexture = value; postProcessParams.lensFlareParams.lensColorTexture = value; }
		}

		public float lensDirtBloomEffect 
		{
			get { return m_lensDirtBloomEffect; }
			set { m_lensDirtBloomEffect = value; postProcessParams.lensDirtParams.bloomEffect = ScionUtility.Square(value); }
		}
		public float lensDirtBloomBrightness 
		{
			get { return m_lensDirtBloomBrightness; }
			set { m_lensDirtBloomBrightness = value; postProcessParams.lensDirtParams.bloomBrightness = value; }
		}
		public float lensDirtLensFlareEffect 
		{
			get { return m_lensDirtLensFlareEffect; }
			set { m_lensDirtLensFlareEffect = value; postProcessParams.lensDirtParams.lensFlareEffect = value; }
		}
		public float lensDirtLensFlareBrightness 
		{
			get { return m_lensDirtLensFlareBrightness; }
			set { m_lensDirtLensFlareBrightness = value; postProcessParams.lensDirtParams.lensFlareBrightness = value; }
		}

		public TonemappingMode tonemappingMode
		{
			get { return m_tonemappingMode; }
			set { m_tonemappingMode = value; }
		}
		public float whitePoint 
		{
			get { return m_whitePoint; }
			set { m_whitePoint = value; postProcessParams.commonPostProcess.whitePoint = value; }
		}
		
		public LayerMask exclusionMask
		{
			get { return m_exclusionMask; }
			set { m_exclusionMask = value; postProcessParams.DoFParams.depthOfFieldMask = value; }
		}
		public DepthFocusMode depthFocusMode 
		{
			get { return m_depthFocusMode; }
			set { m_depthFocusMode = value; postProcessParams.DoFParams.depthFocusMode = value; }
		}
		public float maxCoCRadius 
		{
			get { return m_maxCoCRadius; }
			set { m_maxCoCRadius = value; postProcessParams.DoFParams.maxCoCRadius = value; }
		}
		public DepthOfFieldQuality depthOfFieldQuality 
		{
			get { return m_depthOfFieldQuality; }
			set { m_depthOfFieldQuality = value; postProcessParams.DoFParams.quality = SystemInfo.graphicsShaderLevel < 40 ? DepthOfFieldQuality.Normal : value; }
		}
		public Vector2 pointAveragePosition 
		{
			get { return m_pointAveragePosition; }
			set { m_pointAveragePosition = value; postProcessParams.DoFParams.pointAveragePosition = value; }
		}
		public float pointAverageRange 
		{
			get { return m_pointAverageRange; }
			set { m_pointAverageRange = value; postProcessParams.DoFParams.pointAverageRange = value; }
		}
		public bool visualizePointFocus 
		{
			get { return m_visualizePointFocus; }
			set { m_visualizePointFocus = value; postProcessParams.DoFParams.visualizePointFocus = value; }
		}
		public float depthAdaptionSpeed 
		{
			get { return m_depthAdaptionSpeed; }
			set { m_depthAdaptionSpeed = value; postProcessParams.DoFParams.depthAdaptionSpeed = value; }
		}
		public float focalDistance 
		{
			get { return m_focalDistance; }
			set { m_focalDistance = value; postProcessParams.DoFParams.focalDistance = value; }
		}
		public float focalRange 
		{
			get { return m_focalRange; }
			set { m_focalRange = value; postProcessParams.DoFParams.focalRange = value; }
		}
		
		public ColorGradingMode colorGradingMode 
		{
			get { return m_colorGradingMode; }
			set 
			{ 
				m_colorGradingMode = value; 
				postProcessParams.colorGradingParams.colorGradingMode = colorGradingTex1 == null ? ColorGradingMode.Off : value; 
			}
		}
		public Texture2D colorGradingTex1 
		{
			get { return m_colorGradingTex1; }
			set 
			{ 
				m_colorGradingTex1 = value; 
				postProcessParams.colorGradingParams.colorGradingTex1 = value; 
				colorGradingMode = colorGradingMode; //Rerun this property to update state based on texture
			}
		}
		public Texture2D colorGradingTex2 
		{
			get { return m_colorGradingTex2; }
			set 
			{ 
				m_colorGradingTex2 = value; 
				postProcessParams.colorGradingParams.colorGradingTex2 = value; 
			}
		}
		public float colorGradingBlendFactor 
		{
			get { return m_colorGradingBlendFactor; }
			set 
			{ 
				float clampedValue = Mathf.Clamp01(value);
				m_colorGradingBlendFactor = clampedValue; 
				postProcessParams.colorGradingParams.colorGradingBlendFactor = clampedValue; 
			}
		}
		
		public bool userControlledFocalLength 
		{
			get { return m_userControlledFocalLength; }
			set { m_userControlledFocalLength = value; }
		}
		public float focalLength 
		{
			get { return m_focalLength; }
			set { m_focalLength = value; postProcessParams.cameraParams.focalLength = value; }
		}
		public float fNumber 
		{
			get { return m_fNumber; }
			set { m_fNumber = value; postProcessParams.cameraParams.fNumber = value; }
		}
		public float ISO 
		{
			get { return m_ISO; }
			set { m_ISO = value; postProcessParams.cameraParams.ISO = value; }
		}
		public float shutterSpeed 
		{
			get { return m_shutterSpeed; }
			set { m_shutterSpeed = value; postProcessParams.cameraParams.shutterSpeed = value; }
		}
		public float adaptionSpeed 
		{
			get { return m_adaptionSpeed; }
			set { m_adaptionSpeed = value; postProcessParams.cameraParams.adaptionSpeed = value; }
		}
		public Vector2 minMaxExposure 
		{
			get { return m_minMaxExposure; }
			set { m_minMaxExposure = value; postProcessParams.cameraParams.minMaxExposure = value; }
		}
		public float exposureCompensation 
		{
			get { return m_exposureCompensation; }
			set { m_exposureCompensation = value; postProcessParams.cameraParams.exposureCompensation = value; }
		}
		
		public bool grain 
		{
			get { return m_grain; }
			set { m_grain = value; postProcessParams.commonPostProcess.grainIntensity = m_grain == true ? grainIntensity : 0.0f; }
		}
		public float grainIntensity 
		{
			get { return m_grainIntensity; }
			set { m_grainIntensity = value; postProcessParams.commonPostProcess.grainIntensity = m_grain == true ? grainIntensity : 0.0f; }
		}
		public bool vignette 
		{
			get { return m_vignette; }
			set { m_vignette = value; postProcessParams.commonPostProcess.vignetteIntensity = m_vignette == true ? vignetteIntensity : 0.0f; }
		}
		public float vignetteIntensity 
		{
			get { return m_vignetteIntensity; }
			set { m_vignetteIntensity = value; postProcessParams.commonPostProcess.vignetteIntensity = m_vignette == true ? vignetteIntensity : 0.0f; }
		}
		public float vignetteScale 
		{
			get { return m_vignetteScale; }
			set { m_vignetteScale = value; postProcessParams.commonPostProcess.vignetteScale = value;}
		}
		public Color vignetteColor 
		{
			get { return m_vignetteColor; }
			set { m_vignetteColor = value; postProcessParams.commonPostProcess.vignetteColor = value; }
		}
		public bool chromaticAberration 
		{
			get { return m_chromaticAberration; }
			set { m_chromaticAberration = value; postProcessParams.commonPostProcess.chromaticAberration = value; }
		}
		public float chromaticAberrationDistortion 
		{
			get { return m_chromaticAberrationDistortion; }
			set { m_chromaticAberrationDistortion = value; postProcessParams.commonPostProcess.chromaticAberrationDistortion = value; }
		}
		public float chromaticAberrationIntensity 
		{
			get { return m_chromaticAberrationIntensity; }
			set { m_chromaticAberrationIntensity = value; postProcessParams.commonPostProcess.chromaticAberrationIntensity = value; }
		}
		
		protected void OnEnable() 
		{			
			m_camera 				= GetComponent<Camera>();
			m_cameraTransform		= m_camera.transform;
			m_bloomClass			= new Bloom();
			m_lensFlareClass		= new ScionLensFlare();
			m_combinationPass 		= new CombinationPass();
			m_downsampling			= new Downsampling();
			m_virtualCamera 		= new VirtualCamera();
			m_depthOfFieldClass		= new DepthOfField();
			m_scionDebug			= new ScionDebug();
			m_isFirstRender			= true;

			if (PlatformCompatibility() == false) this.enabled = false;
			
			#if UNITY_EDITOR
			UnityEditor.Undo.undoRedoPerformed -= OnUndoCallback;
			UnityEditor.Undo.undoRedoPerformed += OnUndoCallback;
			#endif
		}
		
		protected void OnDisable() 
		{
			if (m_bloomClass != null) m_bloomClass.ReleaseResources();
			
			#if UNITY_EDITOR
			UnityEditor.Undo.undoRedoPerformed -= OnUndoCallback;
			#endif
		}

#if UNITY_EDITOR
		private void OnUndoCallback()
		{
			//Force fill the input with variables on undo, properties are not called
			forceFillParams = true;
			InitializePostProcessParams();
		}
#endif

		protected virtual void InitializePostProcessParams()
		{	
			postProcessParams.Fill(this, forceFillParams);	
			forceFillParams = false;
		}
		
		protected void OnPreRender()
		{			
			InitializePostProcessParams();
			m_camera.depthTextureMode |= DepthTextureMode.Depth;
		}
				
		protected bool PlatformCompatibility()
		{			
			if (SystemInfo.supportsImageEffects == false)
			{
				Debug.LogWarning("Image Effects are not supported on this platform");
				return false;
			}
			
			if (SystemInfo.supportsRenderTextures == false)
			{
				Debug.LogWarning("RenderTextures are not supported on this platform");
				return false;
			}
			
			if (m_bloomClass.PlatformCompatibility() == false) 
			{
				Debug.LogWarning("Bloom shader not supported on this platform");
				return false;
			}

			if (m_lensFlareClass.PlatformCompatibility() == false)
			{
				Debug.LogWarning("Lens flare shader not supported on this platform");
				return false;
			}
			
			if (m_combinationPass.PlatformCompatibility() == false) 
			{
				Debug.LogWarning("Combination shader not supported on this platform");
				return false;
			}
			
			if (m_virtualCamera.PlatformCompatibility() == false) 
			{
				Debug.LogWarning("Virtual camera shader not supported on this platform");
				return false;
			}
			
			if (m_depthOfFieldClass.PlatformCompatibility() == false && depthOfField == true)
			{
				return false;
			}
			
			return true;
		}
		
		protected void SetupPostProcessParameters(PostProcessParameters postProcessParams, RenderTexture source)
		{
			focalDistance = focalDistance < m_camera.nearClipPlane + 0.3f ? m_camera.nearClipPlane + 0.3f : focalDistance;
			
			postProcessParams.camera = m_camera;
			postProcessParams.cameraTransform = m_cameraTransform;
			
			//Done later
			postProcessParams.halfResSource 	= null; 
			postProcessParams.halfResDepth		= m_downsampling.DownsampleDepthTexture(source.width, source.height);
			
			postProcessParams.width 			= source.width;
			postProcessParams.height 			= source.height;
			postProcessParams.halfWidth 		= source.width / 2;
			postProcessParams.halfHeight 		= source.height / 2;
			
			if (prevCamFoV != m_camera.fieldOfView || postProcessParams.preCalcValues.tanHalfFoV == 0.0f)
			{
				postProcessParams.preCalcValues.tanHalfFoV = Mathf.Tan(m_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
				prevCamFoV = m_camera.fieldOfView;
			}
			
			postProcessParams.DoFParams.useMedianFilter = true; //This could technically be a user choice, but its a lot of quality for a low price
			
			if (userControlledFocalLength == false) postProcessParams.cameraParams.focalLength = ScionUtility.GetFocalLength(postProcessParams.preCalcValues.tanHalfFoV);
			else postProcessParams.cameraParams.focalLength = focalLength * 0.001f; //Millimeter to meter
			postProcessParams.cameraParams.apertureDiameter 		= ScionUtility.ComputeApertureDiameter(fNumber, postProcessParams.cameraParams.focalLength);
			
			postProcessParams.cameraParams.fieldOfView	= m_camera.fieldOfView;
			postProcessParams.cameraParams.aspect		= m_camera.aspect;
			postProcessParams.cameraParams.nearPlane	= m_camera.nearClipPlane;
			postProcessParams.cameraParams.farPlane		= m_camera.farClipPlane;
			
			postProcessParams.isFirstRender = m_isFirstRender;
			m_isFirstRender 				= false;
		}
		
		protected void SetGlobalParameters(PostProcessParameters postProcessParams)
		{
			Vector4 nearFarParams = new Vector4();
			nearFarParams.x = postProcessParams.cameraParams.nearPlane;
			nearFarParams.y = postProcessParams.cameraParams.farPlane;
			nearFarParams.z = 1.0f / nearFarParams.y;
			nearFarParams.w = nearFarParams.x * nearFarParams.z;
			Shader.SetGlobalVector("_ScionNearFarParams", nearFarParams);
			
			Vector4 resolutionParams1 = new Vector4();
			resolutionParams1.x = postProcessParams.halfWidth;
			resolutionParams1.y = postProcessParams.halfHeight;
			resolutionParams1.z = postProcessParams.width;
			resolutionParams1.w = postProcessParams.height;
			Shader.SetGlobalVector("_ScionResolutionParameters1", resolutionParams1);
			
			Vector4 resolutionParams2 = new Vector4();
			resolutionParams2.x = 1.0f / postProcessParams.halfWidth;
			resolutionParams2.y = 1.0f / postProcessParams.halfHeight;
			resolutionParams2.z = 1.0f / postProcessParams.width;
			resolutionParams2.w = 1.0f / postProcessParams.height;
			Shader.SetGlobalVector("_ScionResolutionParameters2", resolutionParams2);			
			
			Vector4 cameraParams1 = new Vector4();
			cameraParams1.x = postProcessParams.cameraParams.apertureDiameter;
			cameraParams1.y = postProcessParams.cameraParams.focalLength;
			cameraParams1.z = postProcessParams.cameraParams.aspect;
			cameraParams1.w = 1.0f / postProcessParams.cameraParams.aspect;
			Shader.SetGlobalVector("_ScionCameraParams1", cameraParams1);
			
			Shader.SetGlobalTexture("_HalfResDepthTexture", postProcessParams.halfResDepth);
		}
		
		protected virtual void SetShaderKeyWords(PostProcessParameters postProcessParams)
		{
			if (postProcessParams.cameraParams.cameraMode == CameraMode.Off ||
			    postProcessParams.cameraParams.cameraMode == CameraMode.Manual)
			{
				ShaderSettings.ExposureSettings.SetIndex(1);
			}
			else ShaderSettings.ExposureSettings.SetIndex(0);
			
			switch (postProcessParams.DoFParams.depthFocusMode)
			{
				case (DepthFocusMode.ManualDistance):
					ShaderSettings.DepthFocusSettings.SetIndex(0);
					break;
				case (DepthFocusMode.ManualRange):
					ShaderSettings.DepthFocusSettings.SetIndex(1);
					break;
				case (DepthFocusMode.PointAverage):
					ShaderSettings.DepthFocusSettings.SetIndex(2);
					break;
			}

			if (postProcessParams.DoFParams.depthOfFieldMask != 0) ShaderSettings.DepthOfFieldMaskSettings.SetIndex(0);
			else ShaderSettings.DepthOfFieldMaskSettings.Disable();

			if (postProcessParams.lensFlare == true) ShaderSettings.LensFlareSettings.SetIndex(0);
			else ShaderSettings.LensFlareSettings.Disable();

			if (postProcessParams.colorGradingParams.colorGradingMode == ColorGradingMode.Off || 
			    postProcessParams.colorGradingParams.colorGradingTex1 == null)
			{
				ShaderSettings.ColorGradingSettings.Disable();
			}
			else
			{
				if (postProcessParams.colorGradingParams.colorGradingMode == ColorGradingMode.On) ShaderSettings.ColorGradingSettings.SetIndex(0);
				if (postProcessParams.colorGradingParams.colorGradingMode == ColorGradingMode.Blend) ShaderSettings.ColorGradingSettings.SetIndex(1);
			}
			
			if (postProcessParams.commonPostProcess.chromaticAberration == true) ShaderSettings.ChromaticAberrationSettings.SetIndex(0);
			else ShaderSettings.ChromaticAberrationSettings.Disable();
		}

		private bool previousSRGBWriteStting;
		private void SRGBSettings(PostProcessParameters postProcessParams)
		{
			previousSRGBWriteStting = GL.sRGBWrite;

			//If color grading is on the sRGB conversion is handled manually instead
			if (postProcessParams.colorGradingParams.colorGradingMode != ColorGradingMode.Off)
			{
				GL.sRGBWrite = false;
			}
		}

		private void ResetSRGBSettings()
		{
			GL.sRGBWrite = previousSRGBWriteStting;
		}

		protected virtual void OnRenderImage (RenderTexture source, RenderTexture dest)
		{
			ActiveDebug = m_scionDebug;
			
			SetupPostProcessParameters(postProcessParams, source);
			
			//TODO: REMOVE REMOVE REMOVE when inspector attributes are updated
			postProcessParams.DoFParams.depthOfFieldMask = exclusionMask;
			
			SetGlobalParameters(postProcessParams);
			SRGBSettings(postProcessParams);
			SetShaderKeyWords(postProcessParams);
			PerformPostProcessing(source, dest, postProcessParams);
			ResetSRGBSettings();
			
			ActiveDebug = null;
		}
		
		protected void PerformPostProcessing(RenderTexture source, RenderTexture dest, PostProcessParameters postProcessParams)
		{	
			source = DepthOfFieldStep(postProcessParams, source);
			
			//Do this after DoF so DoF gets included (if active)
			postProcessParams.halfResSource = m_downsampling.DownsampleFireflyRemoving(source);
			//postProcessParams.halfResSource = m_downsampling.Downsample(source);

			//If either is true
			bool downsampledGlareChainExists = false;
			if (postProcessParams.bloom == true || postProcessParams.lensFlare == true)
			{
				int bloomDownsamples 		= postProcessParams.bloom == true 		? postProcessParams.glareParams.downsamples : 0;
				int lensFlareDownsamples 	= postProcessParams.lensFlare == true 	? postProcessParams.lensFlareParams.downsamples : 0;
				int numDownsamples 			= Mathf.Max(bloomDownsamples, lensFlareDownsamples);

				m_bloomClass.RunDownsamplingChain(postProcessParams.halfResSource, numDownsamples, postProcessParams.glareParams.distanceMultiplier);
				downsampledGlareChainExists = true;
			}

			//Run lens flare generation first so that lens flares can access the downsampled textures from the downsampling chain
			//When the upsampling chain is run these textures are overwritten with gaussian blurred values (central limit therom)
			if (postProcessParams.lensFlare == true)
			{
				m_virtualCamera.BindVirtualCameraTextures(m_lensFlareClass.m_lensFlareMat);				
				int numDownsamples = postProcessParams.lensFlareParams.downsamples; 
				
				RenderTexture downsampledGlare = null;
				if (numDownsamples > 1) downsampledGlare = m_bloomClass.GetGlareTexture(numDownsamples-1);
				else if (numDownsamples == 1) downsampledGlare = postProcessParams.halfResSource;
				else downsampledGlare = source;
				
				postProcessParams.lensFlareTexture = m_lensFlareClass.RenderLensFlare(downsampledGlare, postProcessParams.lensFlareParams, source.width);
			}			
			
			if (downsampledGlareChainExists == true && postProcessParams.exposure == true) 
			{
				//Semi high value or logarithmic average works poorly (everything down to this limit is bilinearly downsampled)
				const int minimumReqPixels = 100;
				int numSearches;
				RenderTexture textureToAverage = m_bloomClass.TryGetSmallGlareTexture(minimumReqPixels, out numSearches);

				if (textureToAverage == null) { textureToAverage = postProcessParams.halfResSource; }
				m_virtualCamera.CalculateVirtualCamera(postProcessParams.cameraParams, textureToAverage, postProcessParams.halfWidth, postProcessParams.preCalcValues.tanHalfFoV, 
				                                       postProcessParams.DoFParams.focalDistance, postProcessParams.isFirstRender);
			}
			else if (postProcessParams.exposure == true)
			{
				m_virtualCamera.CalculateVirtualCamera(postProcessParams.cameraParams, postProcessParams.halfResSource, postProcessParams.halfWidth, 
				                                       postProcessParams.preCalcValues.tanHalfFoV, postProcessParams.DoFParams.focalDistance, postProcessParams.isFirstRender);
			}
			
			if (postProcessParams.bloom == true) 
			{
				//Run upsampling chain to create a gaussian blurred half res texture
				m_bloomClass.RunUpsamplingChain(postProcessParams.halfResSource);
				postProcessParams.bloomTexture = m_bloomClass.GetGlareTexture(0);
				postProcessParams.glareParams.bloomNormalizationTerm = m_bloomClass.GetEnergyNormalizer(postProcessParams.glareParams.downsamples);
			}

			//Graphics.Blit(source, dest); 
			m_combinationPass.Combine(source, dest, postProcessParams, m_virtualCamera);
			m_scionDebug.VisualizeDebug(dest);
			
			RenderTexture.ReleaseTemporary(postProcessParams.halfResSource);
			RenderTexture.ReleaseTemporary(postProcessParams.halfResDepth);
			RenderTexture.ReleaseTemporary(postProcessParams.dofTexture);
			
			m_bloomClass.EndOfFrameCleanup();
			m_virtualCamera.EndOfFrameCleanup(); 
			m_depthOfFieldClass.EndOfFrameCleanup();

			if (postProcessParams.lensFlare == true) RenderTexture.ReleaseTemporary(postProcessParams.lensFlareTexture);			
			if (postProcessParams.depthOfField == true) RenderTexture.ReleaseTemporary(source);
		}
		
		//This function is also responsible for downsampling the depth buffer and binding it
		protected RenderTexture DepthOfFieldStep(PostProcessParameters postProcessParams, RenderTexture source)
		{		
			if (postProcessParams.depthOfField == false) return source;
			
			RenderTexture exclusionMask = null;
			if (postProcessParams.DoFParams.depthOfFieldMask != 0) //If objects are masked out
			{
				exclusionMask = m_depthOfFieldClass.RenderExclusionMask(postProcessParams.width, postProcessParams.height, postProcessParams.camera, 
				                                                        postProcessParams.cameraTransform, postProcessParams.DoFParams.depthOfFieldMask);
				//ScionPostProcess.ActiveDebug.RegisterTextureForVisualization(exclusionMask, false, false, false);
				RenderTexture downsampledExclusionMask = m_downsampling.DownsampleMinFilter(source.width, source.height, exclusionMask);
				RenderTexture.ReleaseTemporary(exclusionMask);
				exclusionMask = downsampledExclusionMask;
			}
			
			//postProcessParams.halfResSource = m_downsampling.DownsampleFireflyRemoving(source);
			postProcessParams.halfResSource = m_downsampling.DownsampleFireflyRemovingBilateral(source, postProcessParams.halfResDepth);
			//postProcessParams.halfResSource = m_downsampling.Downsample(source);
			
			source = m_depthOfFieldClass.RenderDepthOfField(postProcessParams, source, m_virtualCamera, exclusionMask);	
			
			//Downsample scene again, this time with DoF applied
			RenderTexture.ReleaseTemporary(postProcessParams.halfResSource);
			if (exclusionMask != null) RenderTexture.ReleaseTemporary(exclusionMask);
			
			return source;
		}
	}
}