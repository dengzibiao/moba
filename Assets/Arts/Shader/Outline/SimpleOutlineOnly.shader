Shader "Level4/Outline/SimpleOutlineOnly" {
	Properties {
		_OutlineColor("Outline Color",color) = (0,0,0,0)
		_Outline("Outline Width",float) = 0.01
	}
	
	CGINCLUDE
		#include "UnityCG.cginc"
		struct appdata_custom{
			float4 vertex:POSITION;
			float3 normal:NORMAL;
		};
		
		struct v2f{
			float4 pos:POSITION;
			float4 color:COLOR;
		};
		float4 _OutlineColor;
		float _Outline;
		
		/**
			1顶点法线转换到mvp空间. 
			2用mvp的法线,偏移消化过的顶点.
		*/
		v2f vert(appdata_custom i){
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP,i.vertex);
			float3 offset = mul((float3x3)UNITY_MATRIX_MVP,i.normal);
			
			o.pos.xy += offset.xy * _Outline;
			o.color = _OutlineColor;
			return o;
		}
		
		float4 frag(v2f i):COLOR{
			return i.color;
		}
	ENDCG
	
	SubShader{
		Tags{"Queue"="Transparent"}
		
		// Pass{
			// Name "Base"
			// cull back
			// blend zero one
			
			// SetTexture[_OutlineColor]{
				// ConstantColor(0,0,0,0)
				// Combine constant
			// }
		// }
	
		Pass{
			Name "outline"
			Tags{"LightMode"="Always"}
			Cull Front
			Blend One OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
	Fallback "Diffuse"
}
