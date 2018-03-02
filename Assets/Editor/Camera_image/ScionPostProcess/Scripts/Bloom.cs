using UnityEngine;
using System.Collections;

namespace ScionEngine
{
	public class Bloom
	{
		private Material m_bloomMat;
		private RenderTexture[] m_glareTextures;	
		private int numDownsamples = -1;
		private float distanceMultiplier;
		
		public Bloom()
		{
			//m_glareTextures = new RenderTexture[numDownsamples];
			m_bloomMat = new Material(Shader.Find("Hidden/ScionBloom"));
			m_bloomMat.hideFlags = HideFlags.HideAndDontSave;
		}
		
		public void ReleaseResources()
		{
			if (m_bloomMat != null)
			{
				#if UNITY_EDITOR
				Object.DestroyImmediate(m_bloomMat);
				#else
				Object.Destroy(m_bloomMat);
				#endif
				m_bloomMat = null;
			}
		}	

		public bool PlatformCompatibility()
		{
			if (Shader.Find("Hidden/ScionBloom").isSupported == false) return false;
			return true;
		}

		public RenderTexture TryGetSmallGlareTexture(int minimumReqPixels, out int numSearches)
		{
			numSearches = 1;
			for (int i = numDownsamples-1; i >= 0; i--)
			{
				int size = m_glareTextures[i].width > m_glareTextures[i].height ? m_glareTextures[i].height : m_glareTextures[i].width;
				if (size >= minimumReqPixels) return m_glareTextures[i];
				numSearches++;
			}

			return null;
		}

		public float GetEnergyNormalizer(int forNumDownsamples)
		{
			float energyTerm = 1.0f;			

			for (int i = forNumDownsamples-1; i > 0; i--) energyTerm = energyTerm * distanceMultiplier + 1.0f;

			return 1.0f / energyTerm;
		}	

		public void EndOfFrameCleanup()
		{
			if (m_glareTextures == null) return;

			for (int i = 0; i < numDownsamples; i++) 
			{
				RenderTexture.ReleaseTemporary(m_glareTextures[i]);
				m_glareTextures[i] = null;
			}
		}

		public RenderTexture GetGlareTexture(int downsampleIndex)
		{
			return m_glareTextures[downsampleIndex];
		}

		public void RunUpsamplingChain(RenderTexture halfResSource)
		{			
			//Upsample
			for (int i = numDownsamples-1; i > 1; i--)
			{
				Graphics.Blit(m_glareTextures[i], m_glareTextures[i-1], m_bloomMat, 1);	
			}

			//Final upsample pass
			m_bloomMat.SetTexture("_HalfResSource", halfResSource);
			Graphics.Blit(m_glareTextures[1], m_glareTextures[0], m_bloomMat, 2);	
		}
		
		public void RunDownsamplingChain(RenderTexture halfResSource, int numDownsamples, float distanceMultiplier)
		{			
			this.distanceMultiplier = distanceMultiplier;
			if (numDownsamples != this.numDownsamples)
			{
				this.numDownsamples = numDownsamples;
				m_glareTextures = new RenderTexture[numDownsamples];
			}
			
			halfResSource.filterMode = FilterMode.Bilinear;
			RenderTextureFormat format = halfResSource.format;
			
			int width = halfResSource.width;
			int height = halfResSource.height;	

			//Allocate textures
			for (int i = 0; i < numDownsamples; i++)
			{		
				m_glareTextures[i] = RenderTexture.GetTemporary(width, height, 0, format);
				m_glareTextures[i].filterMode = FilterMode.Bilinear;		
				m_glareTextures[i].wrapMode = TextureWrapMode.Clamp;	
				
				width /= 2;
				height /= 2;	
			}
			
			halfResSource.filterMode = FilterMode.Bilinear;
			RenderTexture input = halfResSource;
			m_bloomMat.SetFloat("_GlareDistanceMultiplier", distanceMultiplier);

			//Downsample
			for (int i = 1; i < numDownsamples; i++)
			{
				Graphics.Blit(input, m_glareTextures[i], m_bloomMat, 0);		
				input = m_glareTextures[i];
			}
			
			//ScionPostProcess.ActiveDebug.RegisterTextureForVisualization(halfResSource, false);		
		}
	}
}