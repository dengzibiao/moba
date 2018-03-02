// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Hidden/Ultimate/FlareMesh" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "black" {}
	_BrightTexture ("Base (RGB) Trans (A)", 2D) = "black" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	Cull Off 
    ZWrite Off 
	Blend SrcAlpha One
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma glsl
			
			#include "UnityCG.cginc"

			float4x4 _FlareProj; 
			half _Intensity;


			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				half3 color : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BrightTexture;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				//o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertex = mul(_FlareProj, v.vertex);
				o.texcoord = v.texcoord;

				half3 bloom = tex2Dlod(_BrightTexture, float4(v.texcoord1.xy,0,0) ).xyz;
				o.color = bloom * _Intensity;

				half intensity = dot(half3(0.3,0.3,0.3), o.color.xyz);
				if (intensity < 0.001)
					o.vertex = half4(-10000,-10000,0.0,1.0);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				col.xyz *= i.color.xyz;  

				return col;

			}
		ENDCG
	}
}

}
