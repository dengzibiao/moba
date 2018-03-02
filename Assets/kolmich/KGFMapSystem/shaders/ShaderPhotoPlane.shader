Shader "ColorTexture" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		
		Pass 
		{
			Lighting Off
			SetTexture [_MainTex] 
			{ 
				constantColor [_Color]
				combine constant*texture 
			} 						
		}
	}
}