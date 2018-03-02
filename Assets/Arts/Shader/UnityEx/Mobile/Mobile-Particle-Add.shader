Shader "Mobile/Particles/Additive" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	struct v2f{
		fixed4 pos:POSITION;
		fixed2 uv:TEXCOORD;
		fixed4 color:TEXCOORD1;
	};
	sampler2D _MainTex;
	fixed4 _MainTex_ST;
	fixed4 _Color;

	v2f vert(appdata_full v){
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
		o.color = v.color;
		return o;
	}
	fixed4 frag(v2f v):COLOR{
		fixed4 color = tex2D(_MainTex,v.uv) * v.color * _Color;
		return color;
	}

	ENDCG

	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 200
		Blend SrcAlpha One
		Cull Off 
		Lighting Off 
		ZWrite Off 
		
		Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		ENDCG
		}
	} 
	// Fallback "Transparent/Diffuse"
}
