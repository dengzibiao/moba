using UnityEngine;
using System.Collections;

namespace ScionEngine
{
	public class ScionLensFlare 
	{
		public Material m_lensFlareMat;

		public ScionLensFlare()
		{
			m_lensFlareMat = new Material(Shader.Find("Hidden/ScionLensFlare"));
			m_lensFlareMat.hideFlags = HideFlags.HideAndDontSave;
		}

		public bool PlatformCompatibility()
		{
			if (Shader.Find("Hidden/ScionLensFlare").isSupported == false) return false;
			return true;
		}
		
		private float GetDownsamplingNormalizer(int origSourceWidth, int downsampledWidth)
		{
			return downsampledWidth / (float)origSourceWidth;
		}

		private float GetBlurNormalizer(LensFlareBlurSamples blurSamples)
		{
			float normMult = 0.0f;
			if (blurSamples == LensFlareBlurSamples.Off) normMult = 1.0f;
			else if (blurSamples == LensFlareBlurSamples.x4) normMult = 1.0f / 4.0f;
			else if (blurSamples == LensFlareBlurSamples.x8) normMult = 1.0f / 8.0f;
			return normMult;
		}

		private void SetShaderParameters(LensFlareParameters lensFlareParams, RenderTexture downsampledScene, float downsamplingNormalizer)
		{
			if (lensFlareParams.lensColorTexture != null) m_lensFlareMat.SetTexture("_LensColorTexture", lensFlareParams.lensColorTexture);
			else m_lensFlareMat.SetTexture("_LensColorTexture", ScionUtility.WhiteTexture);

			Vector4 lensFlareParams1 = new Vector4();
			lensFlareParams1.x = lensFlareParams.ghostIntensity / (float)lensFlareParams.ghostSamples; //Normalize energy!
			lensFlareParams1.y = lensFlareParams.ghostDispersal * 10.0f / (float)lensFlareParams.ghostSamples; //Normalize reach!
			lensFlareParams1.z = lensFlareParams.ghostDistortion * 60.0f;
			lensFlareParams1.w = lensFlareParams.ghostEdgeFade;
			m_lensFlareMat.SetVector("_LensFlareParams1", lensFlareParams1);

			Vector4 lensFlareParams2 = new Vector4();
			lensFlareParams2.x = lensFlareParams.haloIntensity;
			lensFlareParams2.y = lensFlareParams.haloWidth;
			lensFlareParams2.z = lensFlareParams.haloDistortion * 25.0f;
			lensFlareParams2.w = downsamplingNormalizer;
			m_lensFlareMat.SetVector("_LensFlareParams2", lensFlareParams2);

			Vector4 lensFlareParams3 = new Vector4();
			//Normalize the value so all sample counts "reach" as far under all circumstances
			lensFlareParams3.x = lensFlareParams.blurStrength * 50.0f * GetBlurNormalizer(lensFlareParams.blurSamples) * downsamplingNormalizer; 
			m_lensFlareMat.SetVector("_LensFlareParams3", lensFlareParams3);
			
			Vector4 textureParams = new Vector4();
			textureParams.x = 1.0f / downsampledScene.width;
			textureParams.y = 1.0f / downsampledScene.height;
			m_lensFlareMat.SetVector("_TextureParams", textureParams);
		}

		public RenderTexture RenderLensFlare(RenderTexture downsampledScene, LensFlareParameters lensFlareParams, int origSourceWidth)
		{
			float downsamplingNormalizer = GetDownsamplingNormalizer(origSourceWidth, downsampledScene.width);
			SetShaderParameters(lensFlareParams, downsampledScene, downsamplingNormalizer);

			downsampledScene.wrapMode = TextureWrapMode.Clamp;

			RenderTexture lensFlareTex = RenderTexture.GetTemporary(downsampledScene.width, downsampledScene.height, 0, downsampledScene.format, RenderTextureReadWrite.Linear);
			lensFlareTex.filterMode = FilterMode.Bilinear;
			lensFlareTex.wrapMode = TextureWrapMode.Clamp;

			Graphics.Blit(downsampledScene, lensFlareTex, m_lensFlareMat, (int)lensFlareParams.ghostSamples);
			HexagonalBlur(lensFlareTex, lensFlareParams.blurSamples);

			return lensFlareTex;
		}

		private RenderBuffer[] targetBuffers = new RenderBuffer[2];
		public void HexagonalBlur(RenderTexture lensFlareTex, LensFlareBlurSamples blurSamples)
		{
			if (blurSamples == LensFlareBlurSamples.Off) return;

			int blurPass1ID = blurSamples == LensFlareBlurSamples.x4 ? 0 : 1;
			int blurPass2ID = blurSamples == LensFlareBlurSamples.x4 ? 2 : 3;

			RenderTexture blurTarget0 = RenderTexture.GetTemporary(lensFlareTex.width, lensFlareTex.height, 0, lensFlareTex.format, RenderTextureReadWrite.Linear);
			RenderTexture blurTarget1 = RenderTexture.GetTemporary(lensFlareTex.width, lensFlareTex.height, 0, lensFlareTex.format, RenderTextureReadWrite.Linear);

			lensFlareTex.filterMode = FilterMode.Bilinear;
			blurTarget0.filterMode = FilterMode.Bilinear;
			blurTarget1.filterMode = FilterMode.Bilinear;

			targetBuffers[0] = blurTarget0.colorBuffer;
			targetBuffers[1] = blurTarget1.colorBuffer;

			Graphics.SetRenderTarget(targetBuffers, blurTarget0.depthBuffer);
			m_lensFlareMat.SetTexture("_MainTex", lensFlareTex);
			ScionGraphics.Blit(m_lensFlareMat, blurPass1ID);

			m_lensFlareMat.SetTexture("_BlurTexture1", blurTarget1);
			Graphics.Blit(blurTarget0, lensFlareTex, m_lensFlareMat, blurPass2ID);


			//Graphics.Blit(blurTarget0, lensFlareTex);

			//ScionPostProcessBase.ActiveDebug.RegisterTextureForVisualization(blurTarget0, false);

			RenderTexture.ReleaseTemporary(blurTarget0);
			RenderTexture.ReleaseTemporary(blurTarget1);
			
//			uniform sampler2D _BlurTexture1;
//			
//			// First blur pass.
//			// texture0 - output of CalculateCoCSize
//			BlurOutput BlurPass1(v2f i)
//				// texture0 - SV_Target0 from BlurPass1
//				// texture1 - SV_Target1 from BlurPass1
//				float4 BlurPass2(v2f i) : SV_Target0
		}
	}
}