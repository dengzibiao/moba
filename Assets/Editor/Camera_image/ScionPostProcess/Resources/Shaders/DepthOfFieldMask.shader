
Shader "Scion/ScionDepthOfFieldMask"
{
	Properties 
	{
      _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    
	SubShader
	{
		Tags
		{      
			"RenderType"="Opaque"
		}
		Pass
		{	
			
			cull Back
			ZTest Off
			
			CGPROGRAM
			#include "UnityCG.cginc" 
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
									
			struct v2f 
			{
			    float4 pos : SV_POSITION;
			};
			
			v2f vert (appdata_base v) 
			{
			    v2f o;
			    
			    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			    
			    return o;
			}
			
			float4 frag (v2f i) : SV_Target0
			{
				return float4(0.0f, 0.0f, 0.0f, 0.0f);
			}
			ENDCG
		}
	}
	SubShader
	{
		Pass 
		{
			Tags
			{      
				"RenderType" = "TransparentCutout"
			}
			
			cull Back
			ZTest Off
			
			CGPROGRAM
			#include "UnityCG.cginc" 
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
									
			struct v2f 
			{
			    float4 pos : SV_POSITION;
			    float2 uv : TEXCOORD1;
			};
			
			uniform float _Cutoff;
			uniform float4 _MainTex_ST;				
			uniform sampler2D _MainTex;
			
			v2f vert (appdata_base v) 
			{
			    v2f o;
			    
			    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			    
			    return o;
			}
			
			float4 frag (v2f i) : SV_Target0
			{
				float4 mainTex = tex2D(_MainTex, i.uv);	
				clip(mainTex.a - _Cutoff);
				return 0.0f;
			}
			ENDCG
		}
	}
}
    



