Shader "Hidden/Ultimate/FlareMask" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MaskTex ("Base (RGB)", 2D) = "white" {}
	}


	Subshader 
	{
		Pass 
 		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }      

			CGPROGRAM

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag

			
			#include "UnityCG.cginc"

			struct v2f 
			{
				half4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};

			v2f vert( appdata_img v ) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv =  v.texcoord.xy;
				return o;
			} 

			sampler2D _MainTex;
			sampler2D _MaskTex;


			fixed4 frag(v2f i):COLOR
			{
				return  tex2D(_MainTex, i.uv) * tex2D(_MaskTex, i.uv).r;
			}

			ENDCG
		}

	} 
	FallBack "Diffuse"
}
