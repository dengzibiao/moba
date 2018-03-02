// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Custom/SimpleWaterFlow Alpha" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap("BumpMap",2d) = ""{}
		_UVTiling("UVTiling",vector) = (1,1,1,1)
		_UVDirection("UVDirection",vector) = (1,1,1,1)
		_Color("Color",color) = (1,1,1,1)
	}
	CGINCLUDE
	#include "UnityCG.cginc"
	
	sampler2D _MainTex;
	sampler2D _BumpMap;
	half4 _UVTiling;
	half4 _UVDirection;
	half4 _Color;
	
	// sampler2D unity_Lightmap;
	// float4 unity_LightmapST;
	
	struct v2f{
		half4 pos:POSITION;
		half2 uv:TEXCOORD;
		half4 normalUV:TEXCOORD1;
		#ifdef LIGHTMAP_ON
			half2 uvLM:TEXCOORD2;
		#endif
	};
	
	v2f vert(appdata_full i){
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP,i.vertex);
		o.uv = i.texcoord.xy;
		o.normalUV = o.uv.xyxy * _UVTiling + _Time.xxxx * _UVDirection;
		#ifdef LIGHTMAP_ON
			o.uvLM = i.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
		#endif
		return o;
	}
	
	half4 frag(v2f i):COLOR{
		half4 n = tex2D(_BumpMap,i.normalUV.xy);
		n += tex2D(_BumpMap,i.normalUV.zw);
		n *= 0.025;
		half4 tex = tex2D(_MainTex,i.uv + n.xy *0.25);
		
		#ifdef LIGHTMAP_ON
			tex.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap,i.uvLM));
		#endif
		
		return tex * _Color;
	}
	
	ENDCG
	
	SubShader {
		Tags { "Queue"="Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			ENDCG
		}
	} 
	// FallBack "Diffuse"
}
