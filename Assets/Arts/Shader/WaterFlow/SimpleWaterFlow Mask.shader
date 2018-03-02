Shader "Custom/SimpleWaterFlow Mask" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap("BumpMap",2d) = ""{}
		_MaskMap("MaskMap",2d) = ""{}
		
		_UVTiling("UVTiling",vector) = (1,1,1,1) //缩放
		_UVDirection("UVDirection",vector) = (1,1,1,1) //速度
		_Color("Color",color) = (1,1,1,1)
	}
	CGINCLUDE
	#include "UnityCG.cginc"
	
	sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D _MaskMap;
	
	half4 _UVTiling;
	half4 _UVDirection;
	half4 _Color;
	
	struct v2f{
		half4 pos:POSITION;
		half2 uv:TEXCOORD;
		half4 normalUV:TEXCOORD1;
	};
	
	v2f vert(appdata_base i){
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP,i.vertex);
		o.uv = i.texcoord.xy;
		o.normalUV = o.uv.xyxy * _UVTiling + _Time.xxxx * _UVDirection;
		return o;
	}
	
	half4 frag(v2f i):COLOR{
		half4 maskMap = tex2D(_MaskMap,i.uv);
		
		half4 n = tex2D(_BumpMap,i.normalUV.xy);
		n += tex2D(_BumpMap,i.normalUV.zw);
		n *= 0.025;
		
		half4 tex= tex2D(_MainTex,i.uv + n.xy *0.25 * maskMap.a);
		
		return tex * _Color;
	}
	
	ENDCG
	
	SubShader {
		Tags { "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	} 
	// FallBack "Diffuse"
}
