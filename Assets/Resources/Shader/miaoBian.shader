Shader "miaoBian" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
	_MainTex ("Base (RGB) RefStrGloss (A)", 2D) = "white" {}
	_Cube ("Reflection Cubemap", Cube) = "" { TexGen CubeReflect }
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_OutlineColor ("Outline Color", Color) = (0,1,0,1) 
	_Outline ("Outline width", Range (0, 0.03)) = 0.01 
}

SubShader {
	Tags { "RenderType"="Opaque" "Queue"="Transparent"}
	LOD 400
CGPROGRAM
#pragma surface surf BlinnPhong
#pragma target 3.0

sampler2D _MainTex;
sampler2D _BumpMap;
samplerCUBE _Cube;

float4 _Color;
float4 _ReflectColor;
float _Shininess;

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float3 worldRefl;
	INTERNAL_DATA
};

void surf (Input IN, inout SurfaceOutput o) {
	half4 tex = tex2D(_MainTex, IN.uv_MainTex);
	half4 c = tex * _Color;
	o.Albedo = c.rgb;
	
	o.Gloss = tex.a;
	o.Specular = _Shininess;
	
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	
	float3 worldRefl = WorldReflectionVector (IN, o.Normal);
	half4 reflcol = texCUBE (_Cube, worldRefl);
	reflcol *= tex.a;
	o.Emission = reflcol.rgb * _ReflectColor.rgb;
	o.Alpha = reflcol.a * _ReflectColor.a;
}
ENDCG

      Pass 
      { 
         Name "OUTLINE" 
         Tags { "LightMode" = "Always" } 
		 
		 Blend SrcAlpha OneMinusSrcAlpha
          
         CGPROGRAM 
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct appdata members vertex,normal)
#pragma exclude_renderers d3d11 xbox360
#pragma exclude_renderers gles
#pragma exclude_renderers xbox360
         #pragma vertex vert 

         struct appdata { 
             float4 vertex; 
             float3 normal; 
         }; 

         struct v2f { 
            float4 pos : POSITION; 
            float4 color : COLOR; 
            float fog : FOGC; 
         }; 
         uniform float _Outline; 
         uniform float4 _OutlineColor; 

         v2f vert(appdata v) { 
            v2f o; 
            o.pos = mul(UNITY_MATRIX_MVP, v.vertex); 
            float3 norm = mul ((float3x3)UNITY_MATRIX_MV, v.normal); 
            norm.x *= UNITY_MATRIX_P[0][0]; 
            norm.y *= UNITY_MATRIX_P[1][1]; 
            o.pos.xy += norm.xy * o.pos.z * _Outline; 
    
            o.fog = o.pos.z; 
            o.color = _OutlineColor; 
            return o; 
         } 
		 
         ENDCG 
          
         Cull Front
         ZWrite On
         ColorMask RGBA
      } 
}

FallBack "Reflective/Bumped Diffuse"
}