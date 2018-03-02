Shader "Hidden/ScionBloom" 
{	    	
 	Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    
	CGINCLUDE
	#include "UnityCG.cginc" 
	#include "../ShaderIncludes/ScionCommon.cginc" 
	
	uniform sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;
	uniform float _BloomWeight;
    	
	struct v2f
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};	
		
	v2f vert(appdata_img v)
	{
		v2f o;
		
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		#if UNITY_UV_STARTS_AT_TOP
        if (_MainTex_TexelSize.y < 0.0f) o.uv.y = 1.0f - o.uv.y; 
		#endif
		
		return o; 
	}

	float4 Downsample(v2f i) : SV_Target0
	{		
		const float2 texel = _MainTex_TexelSize.xy;		
			
		float3 center00 = tex2D(_MainTex, i.uv - texel).xyz;
		float3 center01 = tex2D(_MainTex, i.uv + float2(-texel.x, texel.y)).xyz;
		float3 center10 = tex2D(_MainTex, i.uv + float2(texel.x, -texel.y)).xyz;
		float3 center11 = tex2D(_MainTex, i.uv + texel).xyz;
		
		float3 box00_00 = tex2D(_MainTex, i.uv - 2.0f * texel).xyz;
		float3 box00_01 = tex2D(_MainTex, i.uv - 2.0f * float2(texel.x, 0.0f)).xyz;
		float3 box00_10 = tex2D(_MainTex, i.uv - 2.0f * float2(0.0f, texel.y)).xyz;
		float3 box00_11 = tex2D(_MainTex, i.uv).xyz;
		
		float3 box01_00 = box00_01;
		float3 box01_01 = tex2D(_MainTex, i.uv + 2.0f * float2(-texel.x, texel.y)).xyz;
		float3 box01_10 = box00_11;
		float3 box01_11 = tex2D(_MainTex, i.uv + 2.0f * float2(0.0f, texel.y)).xyz;
		
		float3 box10_00 = box00_10;
		float3 box10_01 = box00_11;
		float3 box10_10 = tex2D(_MainTex, i.uv + 2.0f * float2(texel.x, -texel.y)).xyz;
		float3 box10_11 = tex2D(_MainTex, i.uv + 2.0f * float2(texel.x, 0.0f)).xyz;
		
		float3 box11_00 = box00_11;
		float3 box11_01 = box01_11;
		float3 box11_10 = box10_11;
		float3 box11_11 = tex2D(_MainTex, i.uv + 2.0f * texel).xyz;
		
		float3 center = (center00 + center01 + center10 + center11) * 0.25f;
		float3 box00 = (box00_00 + box00_01 + box00_10 + box00_11) * 0.25f;
		float3 box01 = (box01_00 + box01_01 + box01_10 + box01_11) * 0.25f;
		float3 box10 = (box10_00 + box10_01 + box10_10 + box10_11) * 0.25f;
		float3 box11 = (box11_00 + box11_01 + box11_10 + box11_11) * 0.25f;
		
		float3 final = center * 0.5f + (box00 + box01 + box10 + box11) * 0.125f;
		
		return float4(final, 1.0f);
	}	
		
	static const float weights[9] = { 1.0f/16.0f, 2.0f/16.0f, 1.0f/16.0f, 2.0f/16.0f, 4.0f/16.0f, 2.0f/16.0f, 1.0f/16.0f, 2.0f/16.0f, 1.0f/16.0f };
	uniform float _GlareDistanceMultiplier;

	float4 Upsample(v2f i) : SV_Target0
	{
		//012
		//345
		//678	
		const float2 texel = _MainTex_TexelSize.xy;				
		float3 outColor = float3(0.0f, 0.0f, 0.0f);		
		int weightIndex = 0;
		
		SCION_UNROLL for (int x = -1; x < 2; x++)
		{		
			SCION_UNROLL for (int y = -1; y < 2; y++)
			{
				float2 uvOffset = texel * float2(x, y);
				float2 uv = i.uv + uvOffset;
				outColor = outColor + tex2Dlod(_MainTex, float4(uv, 0.0f, 0.0f)).xyz * weights[weightIndex];
				weightIndex++;
			}
		}
		
		outColor = outColor * _GlareDistanceMultiplier;
		
		return float4(outColor, 1.0f);		
	}	
	
	uniform sampler2D _HalfResSource;	

	float4 UpsampleFinal(v2f i) : SV_Target0
	{
		//012
		//345
		//678	
		const float2 texel = _MainTex_TexelSize.xy;				
		float3 outColor = float3(0.0f, 0.0f, 0.0f);		
		int weightIndex = 0;
		
		SCION_UNROLL for (int x = -1; x < 2; x++)
		{		
			SCION_UNROLL for (int y = -1; y < 2; y++)
			{
				float2 uvOffset = texel * float2(x, y);
				float2 uv = i.uv + uvOffset;
				outColor = outColor + tex2Dlod(_MainTex, float4(uv, 0.0f, 0.0f)).xyz * weights[weightIndex];
				weightIndex++;
			}
		}
		
		//Mimic the other upsampling pass but do a "manual" One One blend instead with the distance weight
		//The target rendertexture can have anything, so replace the contents
		float3 source = tex2Dlod(_HalfResSource, float4(i.uv, 0.0f, 0.0f)).xyz;
		outColor = source + outColor * _GlareDistanceMultiplier;
		
		return float4(outColor, 1.0f);		
	}	
	
	ENDCG
	

	Category 
	{
		Subshader 
		{
		    ZTest Off
		    Cull Off
		    ZWrite Off
		    Blend Off
		    Fog { Mode off }
			Pass 
			{
				Name "Downsample" //Pass 0
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment Downsample
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
			Pass 	
			{ 	
	         	Blend One One
				Name "Upsample" //Pass 1
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment Upsample
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG		 
			}
			Pass 	
			{ 	
				Name "UpsampleFinal" //Pass 2
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment UpsampleFinal
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG		 
			}
		}
	}	
	Fallback Off	
}





