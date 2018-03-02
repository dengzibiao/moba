Shader "Hidden/ScionLensFlare" 
{	    	
 	Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    
	CGINCLUDE
	#include "UnityCG.cginc" 
	#include "../ShaderIncludes/ScionCommon.cginc" 
	#include "../ShaderIncludes/VirtualCameraCommon.cginc" 
	
	uniform sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;
	
	uniform float4 _TextureParams;
	#define InvTexWidth			_TextureParams.x
	#define InvTexHeight		_TextureParams.y
	#define TexelSize			float2(InvTexWidth, InvTexHeight)	
	
	uniform float4 _LensFlareParams1;
	#define GhostIntensity 		_LensFlareParams1.x //ALSO INCLUDES NUM_SAMPLES_RCP
	#define GhostDispersal 		_LensFlareParams1.y
	#define GhostChromatic 		_LensFlareParams1.z
	#define GhostEdgeFade 		_LensFlareParams1.w
	
	uniform float4 _LensFlareParams2;
	#define HaloIntensity			_LensFlareParams2.x
	#define HaloWidth	 			_LensFlareParams2.y
	#define HaloChromatic 			_LensFlareParams2.z
	#define DownsamplingNormalizer	_LensFlareParams2.w
	
	uniform float4 _LensFlareParams3;
	#define FlareBlurStrength 	_LensFlareParams3.x
	
	uniform float4x4 _LensStarMatrix;
	uniform sampler2D _LensColorTexture;		
	
	struct BlurOutput
	{
		float4 output0 : SV_Target0;
		float4 output1 : SV_Target1;
	};
	
	#define BLUR_SAMPLE_COUNT 4
	#define BLUR_SAMPLE_COUNT_RCP (1.0f/BLUR_SAMPLE_COUNT)
	
	float GetBlurOffset(float2 uv)
	{
		float centerDist = length(0.5f - uv) * 0.75f + 0.25f;
		//float centerDist = DistanceSquared(0.5f, uv);
		//return centerDist * _ScionLensFlareOffsetScale;
		return FlareBlurStrength;
	}	
    	
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
	
	// First blur pass.
	// texture0 - output of CalculateCoCSize
	BlurOutput BlurPass1(v2f i, int numSamples)
	{
		const float numSamplesRCP = 1.0f / (float)numSamples;
		const float2 texel = TexelSize;		
		BlurOutput output;

		// Final colour and CoC size will be accumulated in the output.
		output.output0 = float4(0.0f, 0.0f, 0.0f, 0.0f);
		output.output1 = float4(0.0f, 0.0f, 0.0f, 0.0f);

		// Diagonal blur step, corrected for aspect ratio.
		float xStep = 0.866f * InvAspectRatio;

		for (int k = 0; k < numSamples; k++)
		{
			float stepDistance = (k + 0.5f) * texel.y * GetBlurOffset(i.uv);
			//float stepDistance = k * texel.y;

			//Vertical blur.
			float2 step0 = float2(0.0f, 1.0f) * stepDistance;
			output.output0 += tex2Dlod(_MainTex, float4(i.uv + step0, 0.0f, 0.0f));
			
			//Diagonal blur.
			float2 step1 = float2(xStep, -0.5f) * stepDistance;
			output.output1 += tex2Dlod(_MainTex, float4(i.uv + step1, 0.0f, 0.0f));
		}
		
		output.output0 *= numSamplesRCP;
		output.output1 *= numSamplesRCP;

		// The second render target contains both of these added together. Don't divide
		// by two here, as it'll be combined again and divided by three in the next pass.
		output.output1.xyz += output.output0.xyz;
		return output;
	}
	
	BlurOutput BlurPass1_4(v2f i) { return BlurPass1(i, 4); }
	BlurOutput BlurPass1_8(v2f i) { return BlurPass1(i, 8); }	
	
	uniform sampler2D _BlurTexture1;

	// Second blur pass.
	// texture0 - SV_Target0 from BlurPass1
	// texture1 - SV_Target1 from BlurPass1
	float4 BlurPass2(v2f i, int numSamples)
	{
		const float numSamplesRCP = 1.0f / (float)numSamples;
		const float2 texel = TexelSize;	
		float4 finalCol = 0.0f;
		
		// Two sets of colour to accumulate this time.
		float4 col0 = float4(0.0f, 0.0f, 0.0f, 0.0f);
		float4 col1 = float4(0.0f, 0.0f, 0.0f, 0.0f);

		// Diagonal passes in different directions for each input texture.
		float xStep = 0.866f * InvAspectRatio;
		float2 step0 = float2( xStep, -0.5f);
		float2 step1 = float2(-xStep, -0.5f);

		SCION_UNROLL for (int k = 0; k < numSamples; k++)
		{
			float stepDistance = (k + 0.5f) * texel.y * GetBlurOffset(i.uv);
			//float stepDistance = k * texel.y;

			col0 += tex2Dlod(_MainTex, float4(i.uv + step0*stepDistance, 0.0f, 0.0f));
			col1 += tex2Dlod(_BlurTexture1, float4(i.uv + step1*stepDistance, 0.0f, 0.0f));
		}
		
		col0 *= numSamplesRCP;
		col1 *= numSamplesRCP;

		// Combine and divide by three (col1 is double brightness).
		finalCol.xyz = (col0.xyz + col1.xyz) / 3.0f;
		return finalCol;
	}	
	
	float4 BlurPass2_4(v2f i) : SV_Target0 { return BlurPass2(i, 4); }
	float4 BlurPass2_8(v2f i) : SV_Target0 { return BlurPass2(i, 8); }	
	
	float3 DistoredSample(sampler2D samp, float2 uv, float2 direction, float3 distortion)
	{
      return float3(
         tex2Dlod(samp, float4(uv + direction * distortion.r, 0.0f, 0.0f)).r,
         tex2Dlod(samp, float4(uv + direction * distortion.g, 0.0f, 0.0f)).g,
         tex2Dlod(samp, float4(uv + direction * distortion.b, 0.0f, 0.0f)).b
      );
   }
   
	float GetDistortion(float2 uv, float cromaticAbberationIntensity)
	{
		//float centerDist = length(0.5f - uv) * 2.0f;
//		float centerDist = DistanceSquared(0.5f, uv) * 2.0f + 0.5f;
//		float mult = lerp(0.4f, 1.0f, centerDist);

		float centerDistSqrd = DistanceSquared(0.5f, frac(uv));
		float weight = 1.0f - centerDistSqrd * centerDistSqrd;		
		weight = weight * 0.5f + 0.5f;
		
		return cromaticAbberationIntensity * weight * DownsamplingNormalizer;
	}
	
	float3 GetRadialLensColor(float2 uv)
	{
      	float colorU = length(float2(0.5f, 0.5f) - uv) * sqrt(2.0f);
      	return tex2Dlod(_LensColorTexture, float4(colorU, 0.5f, 0.0f, 0.0f)).xyz;
	}
	
	float2 GetHaloOffset(float2 normalizedDirection, float2 uv)
	{
		return normalizedDirection * HaloWidth;
	}

	float4 FlarePass(v2f i, int numGhostSamples)
	{
		const float2 texel = _MainTex_TexelSize.xy;	
		
		//return GetRadialLensColor(i.uv).xyzz;
		
		float2 uv = 1.0f - i.uv;		
		float2 sampleDirection = (float2(0.5f, 0.5f) - uv) * GhostDispersal; 
		float2 normDirection = normalize(sampleDirection);
		float3 radialColor = GetRadialLensColor(uv);
		
		//Halo
		float2 haloUV = uv + GetHaloOffset(normDirection, uv);
		float3 haloDistortion = float3(-texel.x, 0.0f, texel.x) * GetDistortion(haloUV, HaloChromatic);
		float3 radialHaloColor = radialColor;
		float haloWeight = length(float2(0.5f, 0.5f) - frac(haloUV)) * sqrt(2.0f);
		haloWeight = pow(1.0 - haloWeight, 4.0) * HaloIntensity;
		float3 halo = DistoredSample(_MainTex, haloUV, normDirection, haloDistortion) * radialHaloColor * haloWeight;
		
		//Ghosts
		float3 ghosts = 0.0f;
		SCION_UNROLL for (int k = 0; k < numGhostSamples; ++k) 
		{
			float2 offsetUV = uv + sampleDirection * (float)k;
			float3 ghostDistortion = float3(-texel.x, 0.0, texel.x) * GetDistortion(offsetUV, GhostChromatic);			
      		float3 texSample = DistoredSample(_MainTex, offsetUV, normDirection, ghostDistortion);      
      		
			float weight = Square(VignetteMask(offsetUV, GhostEdgeFade, 1.0f));
			float3 radialGhostColor = radialColor; 	
								
			ghosts += texSample * radialGhostColor * weight;
		}
		
		//MAD optimized, HaloIntensity is further up
		float3 result = halo + ghosts * GhostIntensity;
		return float4(result, 0.0f);
	}

	float4 FlarePass2(v2f i) : SV_Target0 { return FlarePass(i, 2); }
	float4 FlarePass3(v2f i) : SV_Target0 { return FlarePass(i, 3); }	
	float4 FlarePass5(v2f i) : SV_Target0 { return FlarePass(i, 5); }	
	float4 FlarePass7(v2f i) : SV_Target0 { return FlarePass(i, 7); }	
	float4 FlarePass9(v2f i) : SV_Target0 { return FlarePass(i, 9); }	
	
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
				Name "BlurPass1_4" //Pass 0
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment BlurPass1_4
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
			Pass 
			{
				Name "BlurPass1_8" //Pass 1
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment BlurPass1_8
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
			Pass 
			{
				Name "BlurPass2_4" //Pass 2
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment BlurPass2_4
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
			Pass 
			{
				Name "BlurPass2_8" //Pass 3
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment BlurPass2_8
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
			Pass 
			{
				Name "FlarePass2" //Pass 4
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment FlarePass2
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
			Pass 
			{
				Name "FlarePass3" //Pass 5
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment FlarePass3
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
			Pass 
			{
				Name "FlarePass5" //Pass 6
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment FlarePass5
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
			Pass 
			{
				Name "FlarePass7" //Pass 7
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment FlarePass7
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
			Pass 
			{
				Name "FlarePass9" //Pass 8
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment FlarePass9
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
		}
	}	
	Fallback Off	
}


