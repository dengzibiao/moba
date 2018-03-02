using UnityEngine;
using System.Collections;

namespace ScionEngine
{
	public class Downsampling
	{
		private Material m_downsampleMat;

		public Downsampling()
		{
			m_downsampleMat = new Material(Shader.Find("Hidden/ScionDownsampling"));
			m_downsampleMat.hideFlags = HideFlags.HideAndDontSave;
		}

		private const int FireflyRemovingPass = 0;
		private const int FireflyRemovingBilateralPass = 1;
		private const int DepthPass = 2;
		private const int MinFilterPass = 3;

		//TODO: Implement a stronger filter for DoF
		//Does a weighted downsample to get rid of fireflies (bright pixels, which tend to flicker)
		public RenderTexture DownsampleFireflyRemoving(RenderTexture source)
		{			
			int halfWidth = source.width / 2;
			int halfHeight = source.height / 2;

			RenderTexture downsampled = RenderTexture.GetTemporary(halfWidth, halfHeight, 0, source.format);			
			downsampled.filterMode = FilterMode.Bilinear;
			downsampled.wrapMode = TextureWrapMode.Clamp;
			
			source.filterMode = FilterMode.Bilinear;
			source.wrapMode = TextureWrapMode.Clamp;

			Graphics.Blit(source, downsampled, m_downsampleMat, FireflyRemovingPass);

			return downsampled;
		}
		
		//Does a weighted downsample to get rid of fireflies (bright pixels, which tend to flicker) 
		//while taking depth into account
		public RenderTexture DownsampleFireflyRemovingBilateral(RenderTexture source, RenderTexture halfResDepth)
		{
			int halfWidth = source.width / 2;
			int halfHeight = source.height / 2;
			
			RenderTexture downsampled = RenderTexture.GetTemporary(halfWidth, halfHeight, 0, source.format);			
			downsampled.filterMode = FilterMode.Bilinear;
			downsampled.wrapMode = TextureWrapMode.Clamp;
			
			source.filterMode = FilterMode.Point;
			source.wrapMode = TextureWrapMode.Clamp;
			halfResDepth.filterMode = FilterMode.Point;
			halfResDepth.wrapMode = TextureWrapMode.Clamp;

			m_downsampleMat.SetTexture("_HalfResDepth", halfResDepth);
			
			Graphics.Blit(source, downsampled, m_downsampleMat, FireflyRemovingBilateralPass);
			return downsampled;
		}

		public RenderTexture DownsampleDepthTexture(int width, int height)
		{
			int halfWidth = width / 2;
			int halfHeight = height / 2;
			
			RenderTexture downsampled = RenderTexture.GetTemporary(halfWidth, halfHeight, 0, RenderTextureFormat.RHalf);			
			downsampled.filterMode = FilterMode.Point;
			downsampled.wrapMode = TextureWrapMode.Clamp;
			
			ScionGraphics.Blit(downsampled, m_downsampleMat, DepthPass);
			return downsampled;
		}
		
		public RenderTexture DownsampleMinFilter(int width, int height, RenderTexture source)
		{
			int halfWidth = width / 2;
			int halfHeight = height / 2;
			
			RenderTexture downsampled = RenderTexture.GetTemporary(halfWidth, halfHeight, 0, source.format);			
			downsampled.filterMode = source.filterMode;
			downsampled.wrapMode = source.wrapMode;
									
			Graphics.Blit(source, downsampled, m_downsampleMat, MinFilterPass);		
			return downsampled;
		}

		public RenderTexture Downsample(RenderTexture source)
		{
			int halfWidth = source.width / 2;
			int halfHeight = source.height / 2;
			FilterMode origFilterMode = source.filterMode;
			
			RenderTexture downsampled = RenderTexture.GetTemporary(halfWidth, halfHeight, 0, source.format);			
			downsampled.filterMode = source.filterMode;
			downsampled.wrapMode = source.wrapMode;

			source.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, downsampled);
			source.filterMode = origFilterMode;

			return downsampled;
		}
	}
}