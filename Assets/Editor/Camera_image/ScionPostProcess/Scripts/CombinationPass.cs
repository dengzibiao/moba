using UnityEngine;
using System.Collections;

namespace ScionEngine
{
	public class CombinationPass
	{
		private Material m_combinationMat;
		private const float MinValue = 1e-4f;
		
		public CombinationPass()
		{
			m_combinationMat = new Material(Shader.Find("Hidden/ScionCombinationPass"));
			m_combinationMat.hideFlags = HideFlags.HideAndDontSave;
		}
		
		public void ReleaseResources()
		{
			if (m_combinationMat != null)
			{
				#if UNITY_EDITOR
				Object.DestroyImmediate(m_combinationMat);
				#else
				Object.Destroy(m_combinationMat);
				#endif
				m_combinationMat = null;
			}
		}	
		
		public bool PlatformCompatibility()
		{
			if (Shader.Find("Hidden/ScionCombinationPass").isSupported == false) return false;
			return true;
		}
		
		private void PrepareBloomSampling(RenderTexture bloomTexture, GlareParameters glareParams)
		{ 		
			m_combinationMat.SetTexture("_BloomTexture", bloomTexture);			

			Vector4 shaderParams = new Vector4();
			shaderParams.x = glareParams.intensity;	
			shaderParams.y = glareParams.brightness;	
			shaderParams.z = glareParams.bloomNormalizationTerm;
			m_combinationMat.SetVector("_GlareParameters", shaderParams);
		}		
		
		private void PrepareLensDirtSampling(Texture lensDirtTexture, LensDirtParameters lensDirtParams, GlareParameters glareParams)
		{ 				
			m_combinationMat.SetTexture("_LensDirtTexture", lensDirtTexture);

			Vector4 shaderParams = new Vector4();
			shaderParams.x = lensDirtParams.bloomEffect;	
			shaderParams.y = lensDirtParams.bloomBrightness * glareParams.brightness;	
			shaderParams.z = lensDirtParams.lensFlareEffect;	
			shaderParams.w = lensDirtParams.lensFlareBrightness;		
			m_combinationMat.SetVector("_LensDirtParameters", shaderParams);
		}

		private void PrepareLensFlareSampling(LensFlareParameters lensFlareParams, RenderTexture lensFlareTexture, Transform cameraTransform)
		{			
			Vector3 camLeft = -cameraTransform.right;
			Vector3 camForward = cameraTransform.forward;
			float camRotation = Vector3.Dot(camLeft, Vector3.forward) + Vector3.Dot(camForward, Vector3.up);
			
			Vector4 lensStarParams1 = new Vector4();
			lensStarParams1.x = Mathf.Cos(camRotation * 2.0f * Mathf.PI);
			lensStarParams1.y = Mathf.Sin(camRotation * 2.0f * Mathf.PI);
			lensStarParams1.z = lensFlareParams.starUVScale;
			lensStarParams1.w = (1.0f - lensFlareParams.starUVScale) * 0.5f;
			m_combinationMat.SetVector("_LensStarParams1", lensStarParams1);
			
			if (lensFlareParams.starTexture != null) m_combinationMat.SetTexture("_LensFlareStarTexture", lensFlareParams.starTexture);
			else m_combinationMat.SetTexture("_LensFlareStarTexture", ScionUtility.WhiteTexture);

			m_combinationMat.SetTexture("_LensFlareTexture", lensFlareTexture);
		}

		private void PrepareExposure(CameraParameters cameraParams, VirtualCamera virtualCamera)
		{
			if (cameraParams.cameraMode == CameraMode.Off)
			{
				m_combinationMat.SetFloat("_ManualExposure", 1.0f);
			}
			else if (cameraParams.cameraMode != CameraMode.Manual) 
			{
				virtualCamera.BindVirtualCameraTextures(m_combinationMat);
			}
			else 
			{
				m_combinationMat.SetFloat("_ManualExposure", virtualCamera.CalculateManualExposure(cameraParams));
			}
		}
		
		private bool ShouldInvertVAxis(PostProcessParameters postProcessParams)
		{
			if (postProcessParams.camera.actualRenderingPath == RenderingPath.Forward && QualitySettings.antiAliasing > 0) return true;
			else return false;
		}

		private void UploadVariables(PostProcessParameters postProcessParams)
		{
			Vector4 postProcessParams1 = new Vector4();
			postProcessParams1.x = postProcessParams.commonPostProcess.grainIntensity;
			postProcessParams1.y = postProcessParams.commonPostProcess.vignetteIntensity;
			postProcessParams1.z = postProcessParams.commonPostProcess.vignetteScale;
			postProcessParams1.w = postProcessParams.commonPostProcess.chromaticAberrationDistortion;
			m_combinationMat.SetVector("_PostProcessParams1", postProcessParams1);
			
			Vector4 postProcessParams2 = new Vector4();
			postProcessParams2.x = postProcessParams.commonPostProcess.vignetteColor.r;
			postProcessParams2.y = postProcessParams.commonPostProcess.vignetteColor.g;
			postProcessParams2.z = postProcessParams.commonPostProcess.vignetteColor.b;
			postProcessParams2.w = postProcessParams.commonPostProcess.chromaticAberrationIntensity;
			m_combinationMat.SetVector("_PostProcessParams2", postProcessParams2);
			
			Vector4 postProcessParams3 = new Vector4();
			postProcessParams3.x = Random.value;
			postProcessParams3.y = ScionUtility.GetWhitePointMultiplier(postProcessParams.commonPostProcess.whitePoint);
			postProcessParams3.z = 1.0f / postProcessParams.commonPostProcess.whitePoint;
			m_combinationMat.SetVector("_PostProcessParams3", postProcessParams3);
		} 

		private void PrepareColorGrading(ColorGradingParameters colorGradingParams)
		{
			if (colorGradingParams.colorGradingMode == ColorGradingMode.Off) return;

			m_combinationMat.SetTexture("_ColorGradingLUT1", colorGradingParams.colorGradingTex1);
			ColorGrading.UploadColorGradingParams(m_combinationMat, colorGradingParams.colorGradingTex1.height);

			if (colorGradingParams.colorGradingMode == ColorGradingMode.On) return;

			m_combinationMat.SetTexture("_ColorGradingLUT2", colorGradingParams.colorGradingTex2);
			m_combinationMat.SetFloat("_ColorGradingBlendFactor", colorGradingParams. colorGradingBlendFactor);
		}
		
		public void Combine(RenderTexture source, RenderTexture dest, PostProcessParameters postProcessParams, VirtualCamera virtualCamera)
		{			
			if (postProcessParams.bloom == true) PrepareBloomSampling(postProcessParams.bloomTexture, postProcessParams.glareParams);
			if (postProcessParams.lensDirt == true) PrepareLensDirtSampling(postProcessParams.lensDirtTexture, postProcessParams.lensDirtParams, postProcessParams.glareParams);
			if (postProcessParams.lensFlare == true) PrepareLensFlareSampling(postProcessParams.lensFlareParams, postProcessParams.lensFlareTexture, postProcessParams.cameraTransform);
			PrepareExposure(postProcessParams.cameraParams, virtualCamera);
			PrepareColorGrading(postProcessParams.colorGradingParams);
			UploadVariables(postProcessParams);

			//The reasoning behind the passIndex stuff is keyword avoidance
			int passIndex = 0;
			if (postProcessParams.tonemapping == false) 	passIndex += 4;
			if (postProcessParams.bloom == false)  			passIndex += 2;
			if (postProcessParams.lensDirt == false || postProcessParams.lensDirtTexture == null) passIndex += 1;

			source.filterMode = FilterMode.Bilinear;
			source.wrapMode = TextureWrapMode.Clamp;
			Graphics.Blit(source, dest, m_combinationMat, passIndex);

			//Graphics.Blit(postProcessParams.blurTexture, dest);
			//Graphics.Blit(postProcessParams.halfResSource, dest);
			//Graphics.Blit(source, dest);
		}
	}
}