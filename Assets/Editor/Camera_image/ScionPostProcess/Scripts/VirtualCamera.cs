using UnityEngine;
using System.Collections;

namespace ScionEngine
{
	public class VirtualCamera
	{
		public const float FilmWidth = 35.0f; 

		//This exists to push the brightness of a scene towards similar levels as a scene would have looked like in regular Unity
		//Tonemapping often darkens the image more than regular Unity does, this compensates
		private const float BuiltinExposureCompensation = 1.0f;

		private Material m_virtualCameraMat;
		private RenderTexture m_previousExposureTexture;
		private const RenderTextureFormat VCTextureFormat = RenderTextureFormat.ARGBFloat;

		private RenderTexture m_currentResult1;
		private RenderTexture m_currentResult2;
		private RenderBuffer[] renderBuffers = new RenderBuffer[2];
		
		public VirtualCamera()
		{
			m_virtualCameraMat = new Material(Shader.Find("Hidden/ScionVirtualCamera"));
			m_virtualCameraMat.hideFlags = HideFlags.HideAndDontSave;
		}

		public bool PlatformCompatibility()
		{			
			if (Shader.Find("Hidden/ScionVirtualCamera").isSupported == false) return false;
			return true;
		}	

		private RenderTexture DownsampleTexture(RenderTexture renderTex)
		{
			int width = renderTex.width;
			int height = renderTex.height;
			int size = width > height ? width : height;

			renderTex.filterMode = FilterMode.Bilinear;
			RenderTexture input = renderTex;
			bool firstIteration = true;

			while (size > 1)
			{
				size 	= size / 2 + size % 2;
				width 	= Mathf.Max(width / 2 + width % 2, 1);
				height 	= Mathf.Max(height / 2 + height % 2, 1);

				RenderTexture downsampled = RenderTexture.GetTemporary(width, height, 0, renderTex.format);
				downsampled.filterMode = FilterMode.Bilinear;
				downsampled.wrapMode = TextureWrapMode.Clamp;

				if (firstIteration == true) 
				{
					Graphics.Blit(input, downsampled, m_virtualCameraMat, 3);
					firstIteration = false;
				}
				else
				{
					Graphics.Blit(input, downsampled);
					RenderTexture.ReleaseTemporary(input);
				}

				input = downsampled;
			}

			return input;
		}
		
		public void BindVirtualCameraTextures(Material mat)
		{
			mat.SetTexture("_VirtualCameraTexture1", m_currentResult1);
			mat.SetTexture("_VirtualCameraTexture2", m_currentResult2);
		}

		public const float LIGHT_INTENSITY_MULT = 3000.0f;

		//Standard Output Based Exposure
		public float CalculateManualExposure(CameraParameters cameraParams, float middleGrey = 0.18f)
		{			
			float lAvg = (1000.0f / 65.0f) * cameraParams.fNumber*cameraParams.fNumber / (cameraParams.ISO * cameraParams.shutterSpeed);
			return Mathf.Pow(2.0f, cameraParams.exposureCompensation) * LIGHT_INTENSITY_MULT * middleGrey / lAvg;
		}

		private ComputeBuffer readBfr;
		private Vector4[] readVec;    

		public void BindVirtualCameraParams(Material mat, CameraParameters cameraParams, float focalDistance, float halfResWidth, bool isFirstRender)
		{			
			Vector4 shaderParams1 = new Vector4();
			shaderParams1.x = 1.0f / (cameraParams.focalLength * 1000.0f); //From meter to millimeter
			shaderParams1.y = cameraParams.fNumber;
			shaderParams1.z = cameraParams.shutterSpeed;
			shaderParams1.w = isFirstRender == true ? 1.0f : 1.0f - Mathf.Exp(-Time.deltaTime * cameraParams.adaptionSpeed);
			#if UNITY_EDITOR
			if (UnityEditor.EditorApplication.isPlaying == false) shaderParams1.w = 1.0f;
			#endif
			mat.SetVector("_VirtualCameraParams1", shaderParams1);
			 
			Vector4 shaderParams2 = new Vector4();
			shaderParams2.x = cameraParams.exposureCompensation + BuiltinExposureCompensation;
			shaderParams2.y = cameraParams.focalLength;
			shaderParams2.z = focalDistance;
			shaderParams2.w = ScionUtility.CoCToPixels(halfResWidth);
			mat.SetVector("_VirtualCameraParams2", shaderParams2);
			
			Vector4 shaderParams3 = new Vector4();
			shaderParams3.x = Mathf.Pow(2.0f, cameraParams.minMaxExposure.x);
			shaderParams3.y = Mathf.Pow(2.0f, cameraParams.minMaxExposure.y);
			mat.SetVector("_VirtualCameraParams3", shaderParams3);
		}

		public void CalculateVirtualCamera(CameraParameters cameraParams, RenderTexture textureToDownsample, float halfResWidth, 
		                                   float tanHalfFoV, float focalDistance, bool isFirstRender)
		{			
			if (cameraParams.cameraMode == CameraMode.Manual || cameraParams.cameraMode == CameraMode.Off) return;
			if (m_currentResult2 != null)
			{
				RenderTexture.ReleaseTemporary(m_currentResult2);
				m_currentResult2 = null; 
			}    

			BindVirtualCameraParams(m_virtualCameraMat, cameraParams, focalDistance, halfResWidth, isFirstRender);

			RenderTexture downsampledScene = DownsampleTexture(textureToDownsample);
			m_virtualCameraMat.SetTexture("_DownsampledScene", downsampledScene);
			if (m_previousExposureTexture != null) m_virtualCameraMat.SetTexture("_PreviousExposureTexture", m_previousExposureTexture);

			m_currentResult1 = RenderTexture.GetTemporary(1, 1, 0, VCTextureFormat, RenderTextureReadWrite.Linear);
			m_currentResult2 = RenderTexture.GetTemporary(1, 1, 0, VCTextureFormat, RenderTextureReadWrite.Linear);
			renderBuffers[0] = m_currentResult1.colorBuffer;
			renderBuffers[1] = m_currentResult2.colorBuffer;
			
			int passIndex = (int)cameraParams.cameraMode - 2;
			Graphics.SetRenderTarget(renderBuffers, m_currentResult1.depthBuffer);
			ScionGraphics.Blit(m_virtualCameraMat, passIndex);

			
//			struct CameraOutput
//			{
//				float sceneLuminance;
//				float shutterSpeed;
//				float ISO;
//				float fNumber;
//				float exposure;
//				float2 CoCScaleAndBias;
//				float notUsed;
//			};


//			if (readBfr == null) readBfr = new ComputeBuffer(2, 16);
//			if (readVec == null) readVec = new Vector4[2];
//			ComputeShader shdr = HelpScript.HelpShader;
//			shdr.SetTexture(0, "ReadTexture1", m_currentResult1);
//			shdr.SetTexture(0, "ReadTexture2", m_currentResult2);
//			shdr.SetBuffer(0, "ReadBackBuffer", readBfr);
//			shdr.Dispatch(0,1,1,1);
//			readBfr.GetData(readVec);
//			Debug.Log("sceneLuminance: " + readVec[0].x +
//			          "\nshutterSpeed: " + readVec[0].y +
//			          "\nISO: " + readVec[0].z +
//			          "\nfNumber: " + readVec[0].w +
//			          "\nexposure: " + readVec[1].x + 
//						"\nCoC scale: " + readVec[1].y + 
//			          "\nCoC bias: " + readVec[1].z + 
//						"\ntargetEV: " + readVec[1].w);



			RenderTexture.ReleaseTemporary(downsampledScene);
			if (m_previousExposureTexture != null) RenderTexture.ReleaseTemporary(m_previousExposureTexture);
			m_previousExposureTexture = m_currentResult1;
		}

		public void EndOfFrameCleanup()
		{
		}
	}
}