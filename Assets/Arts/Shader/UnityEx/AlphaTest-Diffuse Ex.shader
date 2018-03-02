Shader "Transparent/Cutout/Diffuse Ex" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		
	_Wrap("Wrap",float) = -0.25
	_Intensity("Intensity",float) = 2
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 200

CGPROGRAM
#pragma surface surf Lambert alphatest:_Cutoff
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

Fallback "Transparent/Cutout/VertexLit"
}
