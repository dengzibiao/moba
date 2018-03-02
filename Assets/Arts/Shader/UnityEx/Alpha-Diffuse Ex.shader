Shader "Transparent/Diffuse Ex" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Wrap("Wrap",float) = -0.25
	_Intensity("Intensity",float) = 2
}

SubShader {
	Tags {"Queue"="Transparent-1" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200
	Blend SrcAlpha OneMinusSrcAlpha

CGPROGRAM
#pragma surface surf Lambert
#include "ExCG.cginc"
#define Wrap

sampler2D _MainTex;
fixed4 _Color;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	#ifdef Wrap
		c *= WrapTexture(_MainTex, IN.uv_MainTex);
	#endif
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "Transparent/VertexLit"
}
