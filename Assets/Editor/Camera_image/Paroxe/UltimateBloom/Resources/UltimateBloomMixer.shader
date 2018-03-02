Shader "Hidden/Ultimate/BloomMixer" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}


	Subshader 
	{
		Pass // #0 Blend Add
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
			sampler2D _ColorBuffer;
			half _Intensity;

			fixed4 frag(v2f i):COLOR
			{
				half4 addedbloom = tex2D(_MainTex, i.uv);
				half4 screencolor = tex2D(_ColorBuffer, i.uv);
				return _Intensity * addedbloom + screencolor;
			}

			ENDCG
		}

		Pass // #1 Blend With Intensity
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
			sampler2D _ColorBuffer;
			half _Intensity0;
			half _Intensity1;

			fixed4 frag(v2f i):COLOR
			{
				half4 tex0 = tex2D(_MainTex, i.uv);
				half4 tex1 = tex2D(_ColorBuffer, i.uv);
				return tex0 * _Intensity0 + tex1 * _Intensity1;
			}

			ENDCG
		}

		Pass // #2 Blit with intensity
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
			half _Intensity;

			fixed4 frag(v2f i):COLOR
			{
				half4 tex = tex2D(_MainTex, i.uv);
				return tex * _Intensity;
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
}
