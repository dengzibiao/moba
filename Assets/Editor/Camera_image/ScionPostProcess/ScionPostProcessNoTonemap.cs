using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

namespace ScionEngine
{
	[ExecuteInEditMode, AddComponentMenu("Image Effects/Scion Post Process No Tonemap")]
	[RequireComponent(typeof(Camera))]
	public class ScionPostProcessNoTonemap : ScionPostProcessBase 
	{	
		protected override bool ShowTonemapping() { return false; }

		protected override void SetShaderKeyWords(PostProcessParameters postProcessParams)
		{
			base.SetShaderKeyWords(postProcessParams);
		}
		
		protected override void InitializePostProcessParams()
		{			
			base.InitializePostProcessParams();

			postProcessParams.tonemapping = false;
		}

		protected override void OnRenderImage(RenderTexture source, RenderTexture dest)
		{
			base.OnRenderImage(source, dest);
		}
	}
}