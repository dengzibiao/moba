// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Level4/Outline/SimpleRimOutlineOnly" {
	Properties {
		_OutlineColor("Outline Color",color) = (0,0,0,0)
		_OutlineWidth("Outline Width",float) = 0.01
		_RimPower("Rim Power",float) = 1
		// _AlphaPower("Alpha Power",float) = 0.4
	}
	
	CGINCLUDE
		#include "UnityCG.cginc"
		struct appdata_custom{
			float4 vertex:POSITION;
			float3 normal:NORMAL;
		};
		
		struct v2f{
			float4 pos:POSITION;
			// float4 color:COLOR;
			float3 viewDir:TEXCOORD;
			float3 normal:TEXCOORD1;
		};
		float4 _OutlineColor;
		float _OutlineWidth;
		float _RimPower;
		// float _AlphaPower;
		
		/**
			1顶点法线转换到mvp空间. 
			2用mvp的法线,偏移消化过的顶点.
		*/
		v2f vert(appdata_custom i){
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP,i.vertex);
			o.viewDir = _WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld,i.vertex).xyz;
			// o.viewDir = WorldSpaceViewDir(i.vertex);
			float3 offset = mul((float3x3)UNITY_MATRIX_MVP,i.normal);
			o.pos.xy += offset.xy * _OutlineWidth;
			// o.color = _OutlineColor;
			
			o.normal = mul((float3x3)(unity_ObjectToWorld), i.normal.xyz);
			return o;
		}
		
		float4 frag(v2f i):COLOR{
			float3 viewDir = normalize(i.viewDir);
			float3 normal = normalize(i.normal);
			float nDotE = saturate(dot(viewDir,normal));
			float rim = 1 - nDotE;
			// clip(rim - _AlphaPower);
			float4 color = _OutlineColor * pow(rim,_RimPower);
			return color;
		}
	ENDCG
	
	SubShader{
		Tags{"Queue"="Transparent"}
		
		Pass{
			Name "RimOutline"
			Tags{"LightMode"="Always"}
			// Cull Front
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
	// Fallback "Diffuse"
}
