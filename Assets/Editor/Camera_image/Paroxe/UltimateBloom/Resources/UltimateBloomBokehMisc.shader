Shader "Hidden/Ultimate/BokehMisc" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}


	Subshader 
	{
		Pass // #0 Chromatic Aberration
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"

			struct v2f 
			{
				half4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};


			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			half _ChromaticAberration;

			half4 frag(v2f i) : COLOR 
			{
				half2 coords = i.uv;
				half2 uv = i.uv;
		
				coords = (coords - 0.5) * 2.0;		
				half coordDot = dot (coords,coords);
		
				half2 uvG = uv - _MainTex_TexelSize.xy * _ChromaticAberration * coords * coordDot;
				half4 color = tex2D (_MainTex, uv);
				#if SHADER_API_D3D9
					// Work around Cg's code generation bug for D3D9 pixel shaders :(
					color.g = color.g * 0.0001 + tex2D (_MainTex, uvG).g;
				#else
					color.g = tex2D (_MainTex, uvG).g;
				#endif
		
				return color;
			}

			ENDCG
		}



	} 
	FallBack "Diffuse"
}
