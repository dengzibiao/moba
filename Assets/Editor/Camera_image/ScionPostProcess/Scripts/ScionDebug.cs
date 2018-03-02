using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScionDebug
{
	private List<RenderTexture> registeredTextures = new List<RenderTexture>();
	private List<bool> forceBilinear = new List<bool>();
	private List<bool> forcePoint = new List<bool>();
	private List<bool> shouldRelease = new List<bool>();

	public void RegisterTextureForVisualization(RenderTexture texture, bool shouldRelease, bool forceBilinear = false, bool forcePoint = false)
	{
		this.registeredTextures.Add(texture);
		this.forceBilinear.Add(forceBilinear);
		this.forcePoint.Add(forcePoint);
		this.shouldRelease.Add(shouldRelease);
	}

	public void VisualizeDebug(RenderTexture dest)
	{
		if (registeredTextures.Count != 0)
		{
			int index = registeredTextures.Count - 1;

			if (forceBilinear[index] == true) registeredTextures[index].filterMode = FilterMode.Bilinear;
			if (forcePoint[index] == true) registeredTextures[index].filterMode = FilterMode.Point;
			Graphics.Blit(registeredTextures[index], dest);

			for (int i = 0; i < shouldRelease.Count; i++) 
			{
				if (shouldRelease[i] == true) RenderTexture.ReleaseTemporary(registeredTextures[i]);
			}

			registeredTextures.Clear();
			forceBilinear.Clear();
			forcePoint.Clear();
			shouldRelease.Clear();
		}
	}
}
