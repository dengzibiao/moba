Shader "Hidden/ScionCombinationPass" 
{	
 	Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    
	CGINCLUDE
	#include "UnityCG.cginc" 
	#include "../ShaderIncludes/ScionCommon.cginc" 
	#include "../ShaderIncludes/VirtualCameraCommon.cginc" 
	#include "../ShaderIncludes/ColorGradingCommon.cginc" 
    
	struct v2f
	{
	    float4 pos : SV_POSITION;
	    float2 uv : TEXCOORD0;	    
	};	
	
	#define CHROMATIC_ABERRATION_SAMPLES 5
	
	uniform sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;
	
	uniform float _ManualExposure;
	
	uniform sampler2D _BloomTexture;
	uniform sampler2D _LensDirtTexture;
	uniform sampler2D _BlurTexture;
	uniform sampler2D _LensFlareTexture;

	uniform float4 _GlareParameters;
	#define BloomIntensity 	_GlareParameters.x
	#define BloomBrightness _GlareParameters.y
	#define BloomEnergyNormalization _GlareParameters.z
	
	uniform float4 _LensDirtParameters;
	#define LensDirtBloomEffect 		_LensDirtParameters.x	
	#define LensDirtBloomBrightness 	_LensDirtParameters.y
	#define LensDirtLensFlareEffect		_LensDirtParameters.z	
	#define LensDirtLensFlareBrightness	_LensDirtParameters.w

	uniform float4 _PostProcessParams1;
	#define GrainIntensity 				_PostProcessParams1.x
	#define VignetteIntensity 			_PostProcessParams1.y
	#define VignetteScale				_PostProcessParams1.z
	#define ChromaticDistortionScale 	_PostProcessParams1.w

	uniform float4 _PostProcessParams2;
	#define VignetteColor 				_PostProcessParams2.xyz
	#define ChromaticIntensity 			_PostProcessParams2.w

	uniform float4 _PostProcessParams3;
	#define GrainSeed 					_PostProcessParams3.x
	#define WhitePointMult				_PostProcessParams3.y
	#define InvWhitePoint				_PostProcessParams3.z	
	
	uniform sampler2D _LensFlareStarTexture;
	uniform float4 _LensStarParams1;
	#define StarCosTheta 	_LensStarParams1.x
	#define StarSinTheta 	_LensStarParams1.y
	#define StarUVScale 	_LensStarParams1.z
	#define StarUVBias 		_LensStarParams1.w
	
	//Removes grain from the shader completely, making it cheaper
	//#define FORCE_GRAIN_OFF
		
	v2f vert (appdata_img v)
	{
		v2f o;
		
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		#if UNITY_UV_STARTS_AT_TOP
        if (_MainTex_TexelSize.y < 0.0f) o.uv.y = 1.0f - o.uv.y; 
		#endif

		return o;
	}
	
	float3 ColorGrading(float3 clr)
	{		
		//Saturate to clamp to [0,1]
		clr = saturate(clr);	
		
		#ifdef SC_COLOR_CORRECTION_1_TEX
		clr = AccurateLinearToSRGB(clr); //Convert from linear to sRGB
		clr = ApplyColorGrading(clr, _ColorGradingLUT1);
		#endif
		
		#ifdef SC_COLOR_CORRECTION_2_TEX
		clr = AccurateLinearToSRGB(clr); //Convert from linear to sRGB
		clr = ApplyColorGrading(clr, _ColorGradingLUT1, _ColorGradingLUT2, _ColorGradingBlendFactor);
		#endif
		
		return clr;
	}
	
	float3 SampleInputTexture(float2 uv)
	{	
		#ifdef SC_CHROMATIC_ABERRATION_ON
		float3 samplesSum = 0.0f;
		float3 weightsSum = 0.0f;
		
		SCION_UNROLL for (int k = 0; k < CHROMATIC_ABERRATION_SAMPLES; k++)
		{
			float iterStep = k / (CHROMATIC_ABERRATION_SAMPLES - 1.0f);
			float3 spectrumWeights = SpectrumOffset(iterStep);			
			
			samplesSum += spectrumWeights * tex2Dlod(_MainTex, float4(VignettedDistortion(uv, iterStep * ChromaticIntensity, ChromaticDistortionScale), 0.0f, 0.0f)).xyz;
			weightsSum += spectrumWeights;
		}
			
		return samplesSum / weightsSum;
		#else
		return tex2Dlod(_MainTex, float4(uv, 0.0f, 0.0f)).xyz;
		#endif
	}
	
	float3 LensFlare(float2 uv)
	{
		float2 centerVec = (uv * StarUVScale + StarUVBias) - 0.5f;
		float2 lensStarUV = 0.0f;
		lensStarUV.x = StarCosTheta * centerVec.x - StarSinTheta * centerVec.y + 0.5f;
		lensStarUV.y = StarSinTheta * centerVec.x + StarCosTheta * centerVec.y + 0.5f;
		
		float fadeFactor = 1.0f - saturate(length(centerVec) * 2.0f);
		float3 starColor = tex2Dlod(_LensFlareStarTexture, float4(lensStarUV, 0.0f, 0.0f)).xyz;
		starColor = starColor;
		
		float3 lensFlare = tex2Dlod(_LensFlareTexture, float4(uv, 0.0f, 0.0f)).xyz;		
		return lensFlare * starColor;
	}
	
	float3 VariousPostProcessing(float3 clr, float2 uv)
	{			
		clr = Vignette(clr, uv, VignetteScale, VignetteIntensity, VignetteColor);	
		
		#ifndef FORCE_GRAIN_OFF			
		clr = Grain(clr, uv, GrainIntensity, GrainSeed);
		#endif
		
		return clr;
	}
	
	float3 FilmicTonemapping(float3 clr)
	{
		clr = (clr * (0.22 * clr + 0.03) + 0.002) / (clr * (0.22 * clr + 0.3) + 0.06f) - 0.033334;		
		return clr;
	}
	
	float3 LumaFilmicTonemapping(float3 clr)
	{
		float luma = Luma(clr);
		float tonemappedLuma = (luma * (0.22 * luma + 0.03) + 0.002) / (luma * (0.22 * luma + 0.3) + 0.06f) - 0.033334;	
		clr = clr * tonemappedLuma / luma;
		return clr;
	}
	
	float3 PhotographicTonemapping(float3 clr)
	{
		return 1.0f - exp2(-clr);
	}

	float3 ReinhardTonemapping(float3 clr)
	{
		clr = clr / (clr + float3(1.0f, 1.0f, 1.0f));
		return clr;
	}

	float3 LumaReinhardTonemapping(float3 clr)
	{
		float luma = Luma(clr);
		float toneMappedLuma = luma * (1.0f + luma * InvWhitePoint) / (1.0f + luma);
		clr *= toneMappedLuma / luma;
		return clr;
	}
	
	float3 Tonemapping(float3 clr)
	{		
		//Clamp to avoid artifacts
		clr = min(clr, 1000.0f);	
							
		#ifdef SC_TONEMAPPING_REINHARD
		clr = ReinhardTonemapping(clr) * WhitePointMult;
		#endif		
			
		#ifdef SC_TONEMAPPING_LUMAREINHARD
		clr = LumaReinhardTonemapping(clr);
		#endif	
				
		#ifdef SC_TONEMAPPING_FILMIC
		clr = FilmicTonemapping(clr) * WhitePointMult;
		#endif
		
		#ifdef SC_TONEMAPPING_PHOTOGRAPHIC
		clr = PhotographicTonemapping(clr) * WhitePointMult;
		#endif
	
		return clr;
	}
	
	float3 GetBloomSamples(float2 screenUV)
	{	
		const float2 texel = InvHalfResSize;			
		const float weights[9] = { 1.0f/16.0f, 2.0f/16.0f, 1.0f/16.0f, 2.0f/16.0f, 4.0f/16.0f, 2.0f/16.0f, 1.0f/16.0f, 2.0f/16.0f, 1.0f/16.0f };
		
		float4 outColor = float4(0.0f, 0.0f, 0.0f, 0.0f);		
		int weightIndex = 0;
		
		SCION_UNROLL for (int x = -1; x < 2; x++)
		{		
			SCION_UNROLL for (int y = -1; y < 2; y++)
			{
				float2 uvOffset = texel * float2(x, y);
				float2 uv = screenUV + uvOffset;
				outColor = outColor + tex2Dlod(_BloomTexture, float4(uv, 0.0f, 0.0f)) * weights[weightIndex];
				weightIndex++;
			}
		}
		
		return outColor * BloomEnergyNormalization;	
	}
	
	float3 SampleOnlyBloom(float3 clr, float2 screenUV)
	{
		float3 bloomTexture = GetBloomSamples(screenUV);		
		clr = lerp(clr, bloomTexture * BloomBrightness, BloomIntensity);
		
		#ifdef SC_LENS_FLARE_ON
		clr = clr + LensFlare(screenUV);
		#endif
				
		return clr;
	}
	
	float3 SampleOnlyDirt(float3 clr, float2 screenUV)
	{
		float3 dirtMask = tex2D(_LensDirtTexture, screenUV).xyz;
	
		#ifdef SC_LENS_FLARE_ON
		float3 lensFlare = LensFlare(screenUV);
		lensFlare = lerp(lensFlare, lensFlare*LensDirtLensFlareBrightness*dirtMask, LensDirtLensFlareEffect);
		clr = clr + lensFlare;
		#endif
		
		return clr;
	}
	
	float3 SampleOnlyLensFlare(float3 clr, float2 screenUV)
	{		
		#ifdef SC_LENS_FLARE_ON
		clr = clr + LensFlare(screenUV);
		#endif
				
		return clr;
	}
	
	float3 SampleBloomDirt(float3 clr, float2 screenUV)
	{
		float3 bloomTexture = GetBloomSamples(screenUV);
		float3 dirtMask = tex2D(_LensDirtTexture, screenUV).xyz;
		
		//clr = lerp(clr, bloomTexture * BloomBrightness, BloomIntensity);					
		//clr = lerp(clr, bloomTexture * LensDirtBloomBrightness, saturate(dirtMask*LensDirtBloomEffect));	
		
		float3 bloomClr = lerp(clr, bloomTexture * BloomBrightness, BloomIntensity);					
		float3 dirtClr = lerp(clr, bloomTexture * LensDirtBloomBrightness, dirtMask);	
		clr = lerp(bloomClr, dirtClr, LensDirtBloomEffect);
	
		#ifdef SC_LENS_FLARE_ON
		float3 lensFlare = LensFlare(screenUV);
		lensFlare = lerp(lensFlare, lensFlare*LensDirtLensFlareBrightness*dirtMask, LensDirtLensFlareEffect);
		clr = clr + lensFlare;
		#endif
			
		return clr; 
	}
	
	float3 ApplyExposure(float3 clr)
	{			
		#ifdef SC_EXPOSURE_AUTO			
		float4 virtualCameraResult = tex2Dlod(_VirtualCameraTexture2, float4(0.5f, 0.5f, 0.0f, 0.0f));
		float exposure = virtualCameraResult.x;	
		return clr * exposure;
		#endif
		
		#ifdef SC_EXPOSURE_MANUAL
		return clr * _ManualExposure;	
		#endif
	}
	
	float4 NoTonemappingBloomNoDirt(v2f i) : SV_Target0
	{
		float3 texSample = SampleInputTexture(i.uv);
		texSample = SampleOnlyBloom(texSample, i.uv);	
		texSample = VariousPostProcessing(texSample, i.uv);		
		texSample = ApplyExposure(texSample);	
		texSample = ColorGrading(texSample);
				
		return float4(texSample, 1.0f);
	}
	
	float4 NoTonemappingBloomDirt(v2f i) : SV_Target0
	{
		float3 texSample = SampleInputTexture(i.uv);
		texSample = SampleBloomDirt(texSample, i.uv);	
		texSample = VariousPostProcessing(texSample, i.uv);	
		texSample = ApplyExposure(texSample);	
		texSample = ColorGrading(texSample);
				
		return float4(texSample, 1.0f);
	}
	
	float4 TonemappingNoBloomDirt(v2f i) : SV_Target0
	{
		float3 texSample = SampleInputTexture(i.uv);		
		texSample = SampleOnlyDirt(texSample, i.uv);
		texSample = VariousPostProcessing(texSample, i.uv);	
		texSample = ApplyExposure(texSample);			
		texSample = Tonemapping(texSample);
		texSample = ColorGrading(texSample);
		
		return float4(texSample, 1.0f);
	}
	
	float4 TonemappingNoBloomNoDirt(v2f i) : SV_Target0
	{
		float3 texSample = SampleInputTexture(i.uv);	
		texSample = SampleOnlyLensFlare(texSample, i.uv);	
		texSample = VariousPostProcessing(texSample, i.uv);	
		texSample = ApplyExposure(texSample);			
		texSample = Tonemapping(texSample);
		texSample = ColorGrading(texSample);
		
		return float4(texSample, 1.0f);
	}
	
	float4 TonemappingBloomNoDirt(v2f i) : SV_Target0
	{
		float3 texSample = SampleInputTexture(i.uv);
		texSample = SampleOnlyBloom(texSample, i.uv);	
		texSample = VariousPostProcessing(texSample, i.uv);	
		texSample = ApplyExposure(texSample);	
		texSample = Tonemapping(texSample);
		texSample = ColorGrading(texSample);
		
		return float4(texSample, 1.0f);
	}
	
	float4 TonemappingBloomDirt(v2f i) : SV_Target0
	{
		float3 texSample = SampleInputTexture(i.uv);
		texSample = SampleBloomDirt(texSample, i.uv);	
		texSample = VariousPostProcessing(texSample, i.uv);	
		texSample = ApplyExposure(texSample);		
		texSample = Tonemapping(texSample);	
		texSample = ColorGrading(texSample);	
		
		return float4(texSample, 1.0f);
	}
	
	float4 NoTonemappingNoBloomDirt(v2f i) : SV_Target0
	{
		float3 texSample = SampleInputTexture(i.uv);		
		texSample = SampleOnlyDirt(texSample, i.uv);
		texSample = VariousPostProcessing(texSample, i.uv);	
		texSample = ApplyExposure(texSample);	
		texSample = ColorGrading(texSample);
		
		return float4(texSample, 1.0f);
	}
	
	float4 NoTonemappingNoBloomNoDirt(v2f i) : SV_Target0
	{
		float3 texSample = SampleInputTexture(i.uv);
		texSample = SampleOnlyLensFlare(texSample, i.uv);
		texSample = VariousPostProcessing(texSample, i.uv);	
		texSample = ApplyExposure(texSample);	
		texSample = ColorGrading(texSample);
		
		return float4(texSample, 1.0f);
	}
	
	ENDCG
	
	Subshader 
	{
	    ZTest Always
	    Cull Off
	    ZWrite Off
	    Blend Off
	    Fog { Mode off }
		Pass 
		{
			Name "TonemappingBloomDirt" //Pass 0

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment TonemappingBloomDirt
			#pragma target 3.0
			#pragma multi_compile SC_TONEMAPPING_REINHARD SC_TONEMAPPING_LUMAREINHARD SC_TONEMAPPING_FILMIC SC_TONEMAPPING_PHOTOGRAPHIC
			#pragma multi_compile SC_EXPOSURE_MANUAL SC_EXPOSURE_AUTO
			#pragma multi_compile __ SC_CHROMATIC_ABERRATION_ON
			#pragma multi_compile __ SC_COLOR_CORRECTION_1_TEX SC_COLOR_CORRECTION_2_TEX
			#pragma multi_compile __ SC_LENS_FLARE_ON
			
			ENDCG
		}
		Pass 
		{
			Name "TonemappingBloomNoDirt" //Pass 1

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment TonemappingBloomNoDirt
			#pragma target 3.0
			#pragma multi_compile SC_TONEMAPPING_REINHARD SC_TONEMAPPING_LUMAREINHARD SC_TONEMAPPING_FILMIC SC_TONEMAPPING_PHOTOGRAPHIC
			#pragma multi_compile SC_EXPOSURE_MANUAL SC_EXPOSURE_AUTO
			#pragma multi_compile __ SC_CHROMATIC_ABERRATION_ON
			#pragma multi_compile __ SC_COLOR_CORRECTION_1_TEX SC_COLOR_CORRECTION_2_TEX
			#pragma multi_compile __ SC_LENS_FLARE_ON
			
			ENDCG
		}	
		Pass 
		{
			Name "TonemappingNoBloomDirt" //Pass 2

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment TonemappingNoBloomDirt
			#pragma target 3.0
			#pragma multi_compile SC_TONEMAPPING_REINHARD SC_TONEMAPPING_LUMAREINHARD SC_TONEMAPPING_FILMIC SC_TONEMAPPING_PHOTOGRAPHIC
			#pragma multi_compile SC_EXPOSURE_MANUAL SC_EXPOSURE_AUTO
			#pragma multi_compile __ SC_CHROMATIC_ABERRATION_ON
			#pragma multi_compile __ SC_COLOR_CORRECTION_1_TEX SC_COLOR_CORRECTION_2_TEX
			#pragma multi_compile __ SC_LENS_FLARE_ON
			
			ENDCG
		}
		Pass 
		{
			Name "TonemappingNoBloomNoDirt" //Pass 3

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment TonemappingNoBloomNoDirt
			#pragma target 3.0
			#pragma multi_compile SC_TONEMAPPING_REINHARD SC_TONEMAPPING_LUMAREINHARD SC_TONEMAPPING_FILMIC SC_TONEMAPPING_PHOTOGRAPHIC
			#pragma multi_compile SC_EXPOSURE_MANUAL SC_EXPOSURE_AUTO
			#pragma multi_compile __ SC_CHROMATIC_ABERRATION_ON
			#pragma multi_compile __ SC_COLOR_CORRECTION_1_TEX SC_COLOR_CORRECTION_2_TEX
			#pragma multi_compile __ SC_LENS_FLARE_ON
			
			ENDCG
		}
		Pass 
		{
			Name "NoTonemappingBloomDirt" //Pass 4

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment NoTonemappingBloomDirt
			#pragma target 3.0
			#pragma multi_compile SC_TONEMAPPING_REINHARD SC_TONEMAPPING_LUMAREINHARD SC_TONEMAPPING_FILMIC SC_TONEMAPPING_PHOTOGRAPHIC
			#pragma multi_compile SC_EXPOSURE_MANUAL SC_EXPOSURE_AUTO
			#pragma multi_compile __ SC_CHROMATIC_ABERRATION_ON
			#pragma multi_compile __ SC_COLOR_CORRECTION_1_TEX SC_COLOR_CORRECTION_2_TEX
			#pragma multi_compile __ SC_LENS_FLARE_ON
			
			ENDCG
		}
		Pass 
		{
			Name "NoTonemappingBloomNoDirt" //Pass 5

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment NoTonemappingBloomNoDirt
			#pragma target 3.0
			#pragma multi_compile SC_TONEMAPPING_REINHARD SC_TONEMAPPING_LUMAREINHARD SC_TONEMAPPING_FILMIC SC_TONEMAPPING_PHOTOGRAPHIC
			#pragma multi_compile SC_EXPOSURE_MANUAL SC_EXPOSURE_AUTO
			#pragma multi_compile __ SC_CHROMATIC_ABERRATION_ON
			#pragma multi_compile __ SC_COLOR_CORRECTION_1_TEX SC_COLOR_CORRECTION_2_TEX
			#pragma multi_compile __ SC_LENS_FLARE_ON
			
			ENDCG
		}
		Pass 
		{
			Name "NoTonemappingNoBloomDirt" //Pass 6

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment NoTonemappingNoBloomDirt
			#pragma target 3.0
			#pragma multi_compile SC_TONEMAPPING_REINHARD SC_TONEMAPPING_LUMAREINHARD SC_TONEMAPPING_FILMIC SC_TONEMAPPING_PHOTOGRAPHIC
			#pragma multi_compile SC_EXPOSURE_MANUAL SC_EXPOSURE_AUTO
			#pragma multi_compile __ SC_CHROMATIC_ABERRATION_ON
			#pragma multi_compile __ SC_COLOR_CORRECTION_1_TEX SC_COLOR_CORRECTION_2_TEX
			#pragma multi_compile __ SC_LENS_FLARE_ON
			
			ENDCG
		}
		Pass 
		{
			Name "NoTonemappingNoBloomNoDirt" //Pass 7

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment NoTonemappingNoBloomNoDirt
			#pragma target 3.0
			#pragma multi_compile SC_TONEMAPPING_REINHARD SC_TONEMAPPING_LUMAREINHARD SC_TONEMAPPING_FILMIC SC_TONEMAPPING_PHOTOGRAPHIC
			#pragma multi_compile SC_EXPOSURE_MANUAL SC_EXPOSURE_AUTO
			#pragma multi_compile __ SC_CHROMATIC_ABERRATION_ON
			#pragma multi_compile __ SC_COLOR_CORRECTION_1_TEX SC_COLOR_CORRECTION_2_TEX
			#pragma multi_compile __ SC_LENS_FLARE_ON
			
			ENDCG
		}
	}	
	Fallback Off	
}
