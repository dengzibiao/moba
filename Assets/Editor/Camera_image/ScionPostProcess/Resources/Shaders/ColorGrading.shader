Shader "Hidden/ScionColorGrading" 
{	
 	Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    
	CGINCLUDE
	#include "UnityCG.cginc" 
	#include "../ShaderIncludes/ScionCommon.cginc" 
	#include "../ShaderIncludes/ColorGradingCommon.cginc" 
    
	struct v2f
	{
	    float4 pos : SV_POSITION;
	    float2 uv : TEXCOORD0;	    
	};	
	
	uniform sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;
	uniform int _LinearInput;

	//With permission this function is copied straight from Chromatica
	float4 ChromaticaLUTUV(float3 clr, float blue)
	{
		float2 quad1 = float2(0.0, 0.0);
		quad1.y = floor(floor(blue) * 0.125);
		quad1.x = floor(blue) - quad1.y * 8.0;

		float2 quad2 = float2(0.0, 0.0);
		quad2.y = floor(ceil(blue) * 0.125);
		quad2.x = ceil(blue) - quad2.y * 8.0;

		float c1 = 0.0009765625 + (0.123046875 * clr.r);
		float c2 = 0.0009765625 + (0.123046875 * clr.g);

		float4 texPos = float4(0.0, 0.0, 0.0, 0.0);
		texPos.x = quad1.x * 0.125 + c1;
		texPos.y = -(quad1.y * 0.125 + c2);
		texPos.z = quad2.x * 0.125 + c1;
		texPos.w = -(quad2.y * 0.125 + c2);

		return texPos;
	}
		
	v2f vert (appdata_img v)
	{
		v2f o;
		
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;

		return o;
	}
	
	float3 FixColorSpace(float3 clr)
	{
		SCION_BRANCH if (_LinearInput == 0) clr = AccurateLinearToSRGB(clr); 
		return clr;
	}
	
	float4 ConvertAmplify(v2f i) : SV_Target0
	{
		float3 clr = tex2Dlod(_MainTex, float4(i.uv, 0.0f, 0.0f)).xyz;
		float3 finalColor = FixColorSpace(clr);
		return float4(finalColor, 1.0f);
	}
	
	uniform sampler2D _NeutralLUT;
	float3 ColorFromUV(float2 uv)
	{
		return tex2Dlod(_NeutralLUT, float4(uv, 0.0f, 0.0f)).xyz;
	}
	
	float4 ConvertChromatica(v2f i) : SV_Target0
	{
		float3 neutralColor = ColorFromUV(i.uv);
		
		float blue = neutralColor.b * 63.0;
		float4 lutUV = ChromaticaLUTUV(neutralColor, blue);

		float3 sample0 = tex2Dlod(_MainTex, float4(lutUV.xy, 0.0f, 0.0f)).xyz;
		float3 sample1 = tex2Dlod(_MainTex, float4(lutUV.zw, 0.0f, 0.0f)).xyz;
		
		float3 clr = lerp(sample0, sample1, frac(blue));
		float3 finalColor = FixColorSpace(clr);
				
		return float4(finalColor, 1.0f);
	}
	
	float4 ConvertUnity(v2f i) : SV_Target0
	{
		float2 uv = i.uv;
		uv.y = 1.0f - uv.y;
		
		float3 clr = tex2Dlod(_MainTex, float4(uv, 0.0f, 0.0f)).xyz;
		float3 finalColor = FixColorSpace(clr);
		//finalColor = AccurateSRGBToLinear(clr);
		//finalColor = AccurateLinearToSRGB(clr);
		
		return float4(finalColor, 1.0f);
	}
	
	ENDCG
	
	Subshader 
	{
	    ZTest Off
	    Cull Off
	    ZWrite Off
	    Blend Off
	    Fog { Mode off }
		Pass 
		{
			Name "ConvertAmplify" //Pass 0

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment ConvertAmplify
			#pragma target 3.0
			
			ENDCG
		}
		Pass 
		{
			Name "ConvertChromatica" //Pass 1

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment ConvertChromatica
			#pragma target 3.0
			
			ENDCG
		}
		Pass 
		{
			Name "ConvertUnity" //Pass 2

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment ConvertUnity
			#pragma target 3.0
			
			ENDCG
		}
	}	
	Fallback Off	
}
