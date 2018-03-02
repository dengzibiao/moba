Shader "ColorTextureAlpha" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 100 Cull Off ZWrite Off Fog { Mode Off } 
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass 
		{
			Lighting Off
			SetTexture [_MainTex] 
			{
				ConstantColor [_Color]
				combine texture * constant
			}
		}
	}
}
