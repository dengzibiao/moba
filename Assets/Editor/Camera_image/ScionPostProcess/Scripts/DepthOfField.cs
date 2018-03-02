using UnityEngine;
using System.Collections;

namespace ScionEngine
{
	public class DepthOfField
	{
		private Material m_DoFMat;
		private Material m_DoFMatDX11;
		private RenderTexture previousPointAverage;

		private Camera m_maskCamera;
		private Camera maskCamera
		{
			get
			{ 
				if (m_maskCamera == null) 
				{
					GameObject camGO = new GameObject();
					camGO.SetActive(false);
					camGO.hideFlags = HideFlags.HideAndDontSave;
					camGO.name = "ScionDoFMaskCamera";					
					m_maskCamera = camGO.AddComponent<Camera>();
					m_maskCamera.enabled = false;
					m_maskCamera.hideFlags = HideFlags.HideAndDontSave;
				}
				return m_maskCamera;
			}
		}
		private static Shader m_maskShader;
		private static Shader maskShader
		{
			get
			{
				if (m_maskShader == null) m_maskShader = Shader.Find("Scion/ScionDepthOfFieldMask");
				return m_maskShader;
			}
		}
		private Transform m_maskCameraTransform;
		private Transform maskCameraTransform
		{
			get
			{
				if (m_maskCameraTransform == null) m_maskCameraTransform = maskCamera.transform;
				return m_maskCameraTransform;
			}
		}
		
		public DepthOfField()
		{
			m_DoFMat = new Material(Shader.Find("Hidden/ScionDepthOfField"));
			m_DoFMat.hideFlags = HideFlags.HideAndDontSave;
			CreateDX11Mat();
		}

		private void CreateDX11Mat()
		{		
			if (SystemInfo.graphicsShaderLevel >= 40 && m_DoFMatDX11 == null)
			{
				m_DoFMatDX11 = new Material(Shader.Find("Hidden/ScionDepthOfFieldDX11"));
				m_DoFMatDX11.hideFlags = HideFlags.HideAndDontSave;
			}
		}
		
		public bool PlatformCompatibility()
		{
			if (Shader.Find("Hidden/ScionDepthOfField").isSupported == false)  
			{ 
				Debug.LogWarning("Depth of Field shader not supported");
				return false;
			}
			if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8) == false)
			{ 
				Debug.LogWarning("R8 texture format not supported");
				return false;
			}
			return true;
		}
		
		public void EndOfFrameCleanup()
		{
			
		}

		public RenderTexture RenderDepthOfField(PostProcessParameters postProcessParams, RenderTexture source, VirtualCamera virtualCamera, RenderTexture exclusionMask)
		{
			CreateDX11Mat();

			if (ShaderSettings.ExposureSettings.IsActive("SC_EXPOSURE_AUTO") == true)
			{
				virtualCamera.BindVirtualCameraTextures(m_DoFMat);
				if (postProcessParams.DoFParams.quality == DepthOfFieldQuality.High_DX11) 
				{
					virtualCamera.BindVirtualCameraTextures(m_DoFMatDX11);
				}
			}

			virtualCamera.BindVirtualCameraParams(m_DoFMat, postProcessParams.cameraParams, postProcessParams.DoFParams.focalDistance, postProcessParams.halfWidth, postProcessParams.isFirstRender);	
			if (postProcessParams.DoFParams.quality == DepthOfFieldQuality.High_DX11) 
			{
				virtualCamera.BindVirtualCameraParams(m_DoFMatDX11, postProcessParams.cameraParams, postProcessParams.DoFParams.focalDistance, postProcessParams.halfWidth, postProcessParams.isFirstRender);	
			}

			RenderTexture depthCenterAverage = null;
			if (postProcessParams.DoFParams.depthFocusMode == DepthFocusMode.PointAverage)
			{
				depthCenterAverage = PrepatePointAverage(postProcessParams);
				//ScionPostProcess.ActiveDebug.RegisterTextureForVisualization(depthCenterAverage, false, false, false);
			}	

			RenderTexture tiledData = CreateTiledData(postProcessParams.halfResDepth, 
			                                          postProcessParams.preCalcValues.tanHalfFoV,
			                                          postProcessParams.cameraParams.fNumber,
			                                          postProcessParams.DoFParams.focalDistance,
			                                          postProcessParams.DoFParams.focalRange,
			                                          postProcessParams.cameraParams.apertureDiameter,
			                                          postProcessParams.cameraParams.focalLength,
			                                          postProcessParams.DoFParams.maxCoCRadius,
			                                          postProcessParams.cameraParams.nearPlane,
			                                          postProcessParams.cameraParams.farPlane);
			
			//ScionPostProcess.ActiveDebug.RegisterTextureForVisualization(exclusionMask, false, false, false);
			RenderTexture neighbourhoodData = TileNeighbourhoodDataGathering(tiledData);
			RenderTexture prefilteredSource = PrefilterSource(postProcessParams.halfResSource);			
			RenderTexture depthOfFieldTexture = BlurTapPass(prefilteredSource, tiledData, neighbourhoodData, exclusionMask, depthCenterAverage, postProcessParams.DoFParams.quality);

			if (postProcessParams.DoFParams.useMedianFilter == true) 
			{
				depthOfFieldTexture = MedianFilterPass(depthOfFieldTexture);
			}

			RenderTexture compositedDoF = UpsampleDepthOfField(source, depthOfFieldTexture, neighbourhoodData, exclusionMask);

			RenderTexture.ReleaseTemporary(tiledData);
			RenderTexture.ReleaseTemporary(neighbourhoodData);
			RenderTexture.ReleaseTemporary(prefilteredSource);
			RenderTexture.ReleaseTemporary(depthOfFieldTexture);

			return compositedDoF;
		}
		
		private float Min(float val1, float val2) { return val1 > val2 ? val2 : val1; } 
		private float Max(float val1, float val2) { return val1 < val2 ? val2 : val1; } 
		private int Min(int val1, int val2) { return val1 > val2 ? val2 : val1; } 
		private int Max(int val1, int val2) { return val1 < val2 ? val2 : val1; } 
		
		private RenderTexture PrepatePointAverage(PostProcessParameters postProcessParams)
		{				
			const int weightedDownsamplePassID = 7;
			const int finalPassID = 8;
			const int visualizationPassID = 9;
			const int downsamplePassID = 10;
			const RenderTextureFormat format = RenderTextureFormat.RGHalf;
			const RenderTextureFormat format2 = RenderTextureFormat.RHalf;
			
			//Force at least 10x10 pixels to always effect the depth
			//Because stuff goes south if the circle is too small
			float range = Max(10.0f/postProcessParams.halfResDepth.width, postProcessParams.DoFParams.pointAverageRange);
			
			Vector4 weightedDownsampleParams = new Vector4();
			weightedDownsampleParams.x = Mathf.Clamp01(postProcessParams.DoFParams.pointAveragePosition.x);
			weightedDownsampleParams.y = Mathf.Clamp01(postProcessParams.DoFParams.pointAveragePosition.y);
			weightedDownsampleParams.z = range*range;
			weightedDownsampleParams.w = 1.0f / (range*range);
			m_DoFMat.SetVector("_DownsampleWeightedParams", weightedDownsampleParams);
			
			if (previousPointAverage != null && postProcessParams.isFirstRender == false)
			{
#if UNITY_EDITOR
				if (UnityEditor.EditorApplication.isPlaying == false) m_DoFMat.SetFloat("_DownsampleWeightedAdaptionSpeed", 1.0f);
				else
#endif
				{
					m_DoFMat.SetFloat("_DownsampleWeightedAdaptionSpeed", 1.0f - Mathf.Exp(-Time.deltaTime * postProcessParams.DoFParams.depthAdaptionSpeed));
				}
				m_DoFMat.SetTexture("_PreviousWeightedResult", previousPointAverage);
			}
			else
			{
				m_DoFMat.SetFloat("_DownsampleWeightedAdaptionSpeed", 1.0f);
				m_DoFMat.SetTexture("_PreviousWeightedResult", null);
			}
			
			postProcessParams.halfResDepth.filterMode = FilterMode.Bilinear;
			
			const int maxSize = 1;
			int texWidth = Max(postProcessParams.halfWidth / 2, maxSize);
			int texHeight = Max(postProcessParams.halfHeight / 2, maxSize);
			
			RenderTexture weightedDownsample = RenderTexture.GetTemporary(texWidth, texHeight, 0, format);
			weightedDownsample.filterMode = FilterMode.Bilinear;
			weightedDownsample.wrapMode = TextureWrapMode.Clamp;
			
			Graphics.Blit(postProcessParams.halfResDepth, weightedDownsample, m_DoFMat, weightedDownsamplePassID);
			
			if (postProcessParams.DoFParams.visualizePointFocus == true)
			{
				RenderTexture visTexture = RenderTexture.GetTemporary(texWidth, texHeight, 0, RenderTextureFormat.ARGB32);
				Graphics.Blit(weightedDownsample, visTexture, m_DoFMat, visualizationPassID);
				//Graphics.Blit(visTexture, dest);
				//RenderTexture.ReleaseTemporary(visTexture);
				ScionPostProcess.ActiveDebug.RegisterTextureForVisualization(visTexture, true, true, false);
			}
			
			RenderTexture input = weightedDownsample;
			int largestSide = Max(texWidth, texHeight);
			
			while (largestSide > maxSize)
			{
				texWidth = Max(maxSize, texWidth/2 + texWidth%2);
				texHeight = Max(maxSize, texHeight/2 + texHeight%2);
				largestSide = largestSide / 2 + largestSide%2;
				
				RenderTexture downsample;
				if (largestSide > maxSize)
				{
					downsample = RenderTexture.GetTemporary(texWidth, texHeight, 0, format);
					downsample.filterMode = FilterMode.Bilinear;
					downsample.wrapMode = TextureWrapMode.Clamp;
					Graphics.Blit(input, downsample, m_DoFMat, downsamplePassID);
				}
				else //Final pass
				{
					downsample = RenderTexture.GetTemporary(texWidth, texHeight, 0, format2);
					downsample.filterMode = FilterMode.Bilinear;
					downsample.wrapMode = TextureWrapMode.Clamp;
					Graphics.Blit(input, downsample, m_DoFMat, finalPassID);
				}
				
				RenderTexture.ReleaseTemporary(input);
				input = downsample;
			}
			
			RenderTexture pointAverage = input;
			if (previousPointAverage != null) RenderTexture.ReleaseTemporary(previousPointAverage);
			previousPointAverage = pointAverage;
			
			return pointAverage;
		}
		
		public RenderTexture RenderExclusionMask(int width, int height, Camera camera, Transform cameraTransform, LayerMask mask)
		{
			RenderTexture exclusionMask = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.R8);
			exclusionMask.filterMode = FilterMode.Point;
			exclusionMask.wrapMode = TextureWrapMode.Clamp;

			maskCameraTransform.position = cameraTransform.position;
			maskCameraTransform.rotation = cameraTransform.rotation;

			maskCamera.CopyFrom(camera);
			maskCamera.cullingMask = mask;
			maskCamera.SetTargetBuffers(exclusionMask.colorBuffer, exclusionMask.depthBuffer);
			maskCamera.clearFlags = CameraClearFlags.SolidColor;
			maskCamera.backgroundColor = Color.white;
			maskCamera.renderingPath = RenderingPath.Forward;
			maskCamera.hdr = false;
			maskCamera.RenderWithShader(maskShader, "RenderType");

			return exclusionMask;
		}

		private RenderTexture CreateTiledData(RenderTexture halfResDepth, float tanHalfFoV, float fNumber, float focalDistance, float focalRange,
		                                      float apertureDiameter, float focalLength, float maxCoCRadius, float nearPlane, float farPlane)
		{
			int tileWidth = halfResDepth.width / 10		+ (halfResDepth.width % 10 == 0 ? 0 : 1);
			int tileHeight = halfResDepth.height / 10	+ (halfResDepth.height % 10 == 0 ? 0 : 1);		

			float CoCScale = apertureDiameter * focalLength * focalDistance / (focalDistance - focalLength);
			float CoCBias = -apertureDiameter * focalLength / (focalDistance - focalLength);

			float toPixels = ScionUtility.CoCToPixels(halfResDepth.width);
			CoCScale *= toPixels;
			CoCBias *= toPixels;

			Vector4 CoCParams1 = new Vector4();
			CoCParams1.x = CoCScale;
			CoCParams1.y = CoCBias;
			CoCParams1.z = focalDistance;
			CoCParams1.w = focalRange * 0.5f;
			m_DoFMat.SetVector("_CoCParams1", CoCParams1);
			
			Vector4 CoCParams2 = new Vector4();
			CoCParams2.x = maxCoCRadius * 0.5f; //We're in half res, so halve it
			CoCParams2.y = 1.0f / maxCoCRadius;
			m_DoFMat.SetVector("_CoCParams2", CoCParams2);

			if (m_DoFMatDX11 != null)
			{ 
				m_DoFMatDX11.SetVector("_CoCParams1", CoCParams1);
				m_DoFMatDX11.SetVector("_CoCParams2", CoCParams2);
			}

			m_DoFMat.SetFloat("_CoCUVOffset", 1.0f / halfResDepth.width); //Width for horizontal

			RenderTexture tiledDataHorizontal = RenderTexture.GetTemporary(tileWidth, halfResDepth.height, 0, RenderTextureFormat.RHalf);
			tiledDataHorizontal.filterMode = FilterMode.Point;
			tiledDataHorizontal.wrapMode = TextureWrapMode.Clamp;

			RenderTexture tiledData = RenderTexture.GetTemporary(tileWidth, tileHeight, 0, RenderTextureFormat.RHalf);
			tiledData.filterMode = FilterMode.Point;
			tiledData.wrapMode = TextureWrapMode.Clamp;

			halfResDepth.filterMode = FilterMode.Point;

			ScionGraphics.Blit(tiledDataHorizontal, m_DoFMat, 0);

			m_DoFMat.SetTexture("_HorizontalTileResult", tiledDataHorizontal);
			m_DoFMat.SetFloat("_CoCUVOffset", 1.0f / halfResDepth.height); //Height for vertical

			ScionGraphics.Blit(tiledData, m_DoFMat, 1);
			RenderTexture.ReleaseTemporary(tiledDataHorizontal);

			return tiledData;
		}

		private RenderTexture TileNeighbourhoodDataGathering(RenderTexture tiledData)
		{
			Vector4 neighbourhoodParams = new Vector4();
			neighbourhoodParams.x = 1.0f / tiledData.width;
			neighbourhoodParams.y = 1.0f / tiledData.height;
			m_DoFMat.SetVector("_NeighbourhoodParams", neighbourhoodParams);

			RenderTexture neighbourhoodData = RenderTexture.GetTemporary(tiledData.width, tiledData.height, 0, RenderTextureFormat.RHalf);
			neighbourhoodData.filterMode = FilterMode.Point;
			neighbourhoodData.wrapMode = TextureWrapMode.Clamp;

			m_DoFMat.SetTexture("_TiledData", tiledData);
			ScionGraphics.Blit(neighbourhoodData, m_DoFMat, 2);

			return neighbourhoodData;
		}

		private RenderTexture PrefilterSource(RenderTexture halfResSource)
		{
			m_DoFMat.SetTexture("_HalfResSourceTexture", halfResSource);
			halfResSource.filterMode = FilterMode.Bilinear;
			
			RenderTexture prefilteredSource = RenderTexture.GetTemporary(halfResSource.width, halfResSource.height, 0, halfResSource.format);
			prefilteredSource.filterMode = FilterMode.Point;
			prefilteredSource.wrapMode = TextureWrapMode.Clamp;
			
			ScionGraphics.Blit(prefilteredSource, m_DoFMat, 4);
			return prefilteredSource;
		}

		private RenderTexture BlurTapPass(RenderTexture halfResSource, RenderTexture tiledData, RenderTexture neighbourhoodData, 
		                                  RenderTexture exclusionMask, RenderTexture depthCenterAverage, DepthOfFieldQuality qualityLevel)
		{
			Material dofMat = qualityLevel == DepthOfFieldQuality.Normal ? m_DoFMat : m_DoFMatDX11;

			dofMat.SetTexture("_TiledData", tiledData);
			dofMat.SetTexture("_TiledNeighbourhoodData", neighbourhoodData);
			dofMat.SetTexture("_HalfResSourceTexture", halfResSource); //Actually the prefiltered half res
			if (exclusionMask != null) dofMat.SetTexture("_ExclusionMask", exclusionMask); 
			if (depthCenterAverage != null) dofMat.SetTexture("_AvgCenterDepth", depthCenterAverage); 
			halfResSource.filterMode = FilterMode.Point;

			RenderTexture blurTexture = RenderTexture.GetTemporary(halfResSource.width, halfResSource.height, 0, halfResSource.format);
			blurTexture.filterMode = FilterMode.Point;
			blurTexture.wrapMode = TextureWrapMode.Clamp;

			if (qualityLevel == DepthOfFieldQuality.Normal) ScionGraphics.Blit(blurTexture, m_DoFMat, 5);
			else if (qualityLevel == DepthOfFieldQuality.High_DX11) ScionGraphics.Blit(blurTexture, m_DoFMatDX11, 0);

			return blurTexture;
		}

		private RenderTexture MedianFilterPass(RenderTexture inputTexture)
		{
			RenderTexture medianFiltered = RenderTexture.GetTemporary(inputTexture.width, inputTexture.height, 0, inputTexture.format);
			medianFiltered.filterMode = FilterMode.Point;
			medianFiltered.wrapMode = TextureWrapMode.Clamp;

			Graphics.Blit(inputTexture, medianFiltered, m_DoFMat, 3);
			RenderTexture.ReleaseTemporary(inputTexture);
			return medianFiltered;
		}

		private RenderTexture UpsampleDepthOfField(RenderTexture source, RenderTexture depthOfFieldTexture, RenderTexture neighbourhoodData, RenderTexture exclusionMask)
		{
			m_DoFMat.SetTexture("_DepthOfFieldTexture", depthOfFieldTexture);
			m_DoFMat.SetTexture("_FullResolutionSource", source);
			m_DoFMat.SetTexture("_TiledNeighbourhoodData", neighbourhoodData);
			if (exclusionMask != null) m_DoFMat.SetTexture("_ExclusionMask", exclusionMask); 
			neighbourhoodData.filterMode = FilterMode.Bilinear;

			RenderTexture compositedDoF = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
			source.filterMode = FilterMode.Point;
			source.wrapMode = TextureWrapMode.Clamp;

			ScionGraphics.Blit(compositedDoF, m_DoFMat, 6);
			return compositedDoF;
		}
	}
}