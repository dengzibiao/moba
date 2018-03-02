Shader "Hidden/ScionDepthOfField" 
{	   	
 	Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
       	
	CGINCLUDE
	#include "UnityCG.cginc" 
	#include "../ShaderIncludes/ScionCommon.cginc" 
	#include "../ShaderIncludes/MedianFilter.cginc" 
	#include "../ShaderIncludes/VirtualCameraCommon.cginc" 
	#include "../ShaderIncludes/DepthOfFieldCommon.cginc"
    	
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

	float4 TiledDataHorizontal(v2f i) : SV_Target0
	{		
		//return tex2Dlod(_HalfResDepthTexture, float4(i.uv, 0.0f, 0.0f)).xx;
		float maxCoC;
		float focalDistance = GetFocalDistance();
					
		SCION_UNROLL for (int k = -5; k < 5; k++)
		{
			float kWithHalfPixOffset = k + 0.5f;
			float2 uv = float2(i.uv.x + kWithHalfPixOffset * _CoCUVOffset, i.uv.y);
			
			float depth = DecodeDepth01(tex2Dlod(_HalfResDepthTexture, float4(uv, 0.0f, 0.0f)).x);
			float CoC = CoCFromDepth(depth, focalDistance);
			
			//Removed by compiler, no runtime branch
			if (k == -5) 
			{
				maxCoC = CoC;
			}
			else
			{
				maxCoC = max(maxCoC, CoC);
			}
		}
				
		return maxCoC;
	}	
	
	uniform sampler2D _HorizontalTileResult;

	float4 TiledDataVertical(v2f i) : SV_Target0
	{		
		float maxCoC;
					
		SCION_UNROLL for (int k = -5; k < 5; k++)
		{
			float kWithHalfPixOffset = k + 0.5f;
			float2 uv = float2(i.uv.x , i.uv.y + kWithHalfPixOffset * _CoCUVOffset);
			
			float CoC = tex2Dlod(_HorizontalTileResult, float4(uv, 0.0f, 0.0f)).x;
			
			//Removed by compiler, no runtime branch
			if (k == -5) 
			{
				maxCoC = CoC;
			}
			else
			{
				maxCoC = max(maxCoC, CoC);
			}
		}
				
		return maxCoC;
	}	
	
	uniform float4 _NeighbourhoodParams;
	#define TileDataTexelWidth _NeighbourhoodParams.x
	#define TileDataTexelHeight _NeighbourhoodParams.y
	
	uniform sampler2D _TiledData;

	float4 NeighbourhoodDataGather(v2f i) : SV_Target0
	{		
		float maxCoC;
					
		SCION_UNROLL for (int v = -1; v < 2; v++)
		{
			SCION_UNROLL for (int u = -1; u < 2; u++)
			{
				float2 uv = float2(i.uv.x + u * TileDataTexelWidth, i.uv.y + v * TileDataTexelHeight);				
				float CoC = tex2Dlod(_TiledData, float4(uv, 0.0f, 0.0f)).x;
				
				//Removed by compiler, no runtime branch
				if (u == -1 && v == -1) 
				{
					maxCoC = CoC;
				}
				else
				{
					maxCoC = max(maxCoC, CoC);
				}
			}
		}
		
		maxCoC = min(maxCoC, MAX_COC_RADIUS);		
		return maxCoC;
	}	
							
	float4 MedianFilter(v2f i) : SV_Target0
	{
		return MedianFilter4(i.uv, _MainTex, InvHalfResSize);
	}	
	
	float4 DebugPass(float2 iuv)
	{		
		float4 tiledNeighbourhood = tex2Dlod(_TiledNeighbourhoodData, float4(iuv, 0.0f, 0.0f));
		float searchRadius = tiledNeighbourhood.x; //Neighbourhood max CoC
		
		for (int k = 0; k < 33; k++)
		{
			float2 sampleOffset = Poisson33[k] * searchRadius;
			float2 uv = iuv + sampleOffset * InvHalfResSize;
			
			float4 tiled = tex2Dlod(_TiledData, float4(uv, 0.0f, 0.0f));
			if (tiled.x > tiledNeighbourhood.x) return float4(1,0,0,0);
		}
		return float4(0,1,0,0);
	}

	static const float2 PoissonTaps8[8] = { float2( -0.7456541f, 0.1131393f), 
		float2( 0.08293837f, -0.8036098f), float2( 0.2584362f, 0.1864142f), float2( -0.7107184f, -0.6010008f), 
		float2( 0.08933985f, 0.9051569f), float2( -0.6178224f, 0.7624108f), float2( 0.7340344f, -0.4169394f), 
		float2( 0.922537f, 0.2814612f) };		

	static const float2 PoissonTaps16[16] = { float2( 0.1213936f, -0.6422687f), 
		float2( -0.0200316f, -0.1517201f), float2( -0.595705f, -0.7857988f), float2( 0.488985f, -0.392335f), 
		float2( -0.4554596f, -0.1692587f), float2( -0.1923808f, -0.9592055f), float2( 0.5479098f, 0.07066441f), 
		float2( 0.06035915f, 0.3289492f), float2( 0.9886969f, 0.008187056f), float2( -0.8500692f, -0.4246502f), 
		float2( 0.6955848f, 0.6919048f), float2( 0.3017962f, 0.8908848f), float2( -0.2407158f, 0.6789985f), 
		float2( -0.9108973f, 0.1838938f), float2( -0.4678481f, 0.3011438f), float2( -0.7653541f, 0.6193144f) };

	float4 PrefilterSource(v2f i) : SV_Target0
	{			
		float centerDepth = DecodeDepth01(tex2Dlod(_HalfResDepthTexture, float4(i.uv, 0.0f, 0.0f)).x);
		float focalDistance = GetFocalDistance();
		float sampleRadius = CoCFromDepthClamped(centerDepth, focalDistance) * 0.34f;	
		float4 totalLighting = float4(tex2Dlod(_HalfResSourceTexture, float4(i.uv, 0.0f, 0.0f)).xyz, 1.0f);
		
		#ifdef USE_SINGLE_TAP_EARLY_OUT
		SCION_BRANCH if (sampleRadius < SINGLE_TAP_EARLY_OUT_PIXEL_RADIUS)
		{
			return totalLighting;
		}
		#endif 
		
		i.uv += RandomOffset(i.uv) * InvHalfResSize;
		float centerIsBackground = saturate(centerDepth - focalDistance);
		
		#define PRE_ITER 8		
		SCION_UNROLL for (int k = 0; k < PRE_ITER; k++)
		{
			#if PRE_ITER == 8
			float2 uv = i.uv + PoissonTaps8[k] * InvHalfResSize * sampleRadius;
			#endif 
			
			#if PRE_ITER == 16
			float2 uv = i.uv + PoissonTaps16[k] * InvHalfResSize * sampleRadius;
			#endif			
			
			float4 sampleLighting = float4(tex2Dlod(_HalfResSourceTexture, float4(uv, 0.0f, 0.0f)).xyz, 1.0f);

			float2 halfResUV = (floor(uv * HalfResSize) + 0.5f) * InvHalfResSize;
			float2 UVDiffDirection = sign(uv - halfResUV) * InvHalfResSize;
			
			float2 sampleUV[4];	
			sampleUV[0] = halfResUV;
			sampleUV[1] = halfResUV + UVDiffDirection;
			sampleUV[2] = float2(halfResUV.x + UVDiffDirection.x, halfResUV.y);
			sampleUV[3] = float2(halfResUV.x, halfResUV.y + UVDiffDirection.y);	

			float minimumWeight = 0.0f;

			#if 1
			SCION_UNROLL for (int j = 0; j < 4; j++)
			{
				float depth = DecodeDepth01(tex2Dlod(_HalfResDepthTexture, float4(sampleUV[j], 0.0f, 0.0f)).x);
				float weight = 1.0f / (abs(centerDepth - depth) + 1e-5f);				
				weight = max(weight, saturate(depth - centerDepth) * centerIsBackground);
				
				if (j == 0) minimumWeight = weight;
				else minimumWeight = min(minimumWeight, weight);
			}
			#else
			SCION_UNROLL for (int j = 0; j < 4; j++)
			{
				float depth = DecodeDepth01(tex2Dlod(_HalfResDepthTexture, float4(sampleUV[j], 0.0f, 0.0f)).x);
				float sampleCoC = CoCFromDepth(depth, focalDistance);
					
				float2 sampleOffset = (i.uv - uv) * HalfResSize;
				
				float distSqrd 				= dot(sampleOffset, sampleOffset);
				float intersectionLerpVal 	= (sampleCoC*sampleCoC - distSqrd);
				float intersection 			= smoothstep(0.0f, 1.0f, intersectionLerpVal);
				float weight 				= intersection;
				
				if (j == 0) minimumWeight = weight;
				else minimumWeight = min(minimumWeight, weight);
			}
			#endif
						
			totalLighting += minimumWeight * sampleLighting;
		}
		
		return totalLighting / totalLighting.w;
	}
	
	uniform sampler2D _DepthOfFieldTexture;
	uniform sampler2D _CameraDepthTexture;
	uniform sampler2D _FullResolutionSource;
	
	float4 UpsamplePass(v2f i) : SV_Target0
	{		
		//return tex2Dlod(_DepthOfFieldTexture, float4(i.uv, 0.0f, 0.0f)).wwww;
		#ifdef DEBUG_OPTIMIZATIONS
		return tex2Dlod(_DepthOfFieldTexture, float4(i.uv, 0.0f, 0.0f));
		#endif
		
		float centerDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv));

		float2 halfResUV = (floor(i.uv * HalfResSize) + 0.5f) * InvHalfResSize;
		float2 UVDiffDirection = sign(i.uv - halfResUV) * InvHalfResSize;
		
		float2 sampleUV[4];	
		sampleUV[0] = halfResUV;
		sampleUV[1] = halfResUV + UVDiffDirection;
		sampleUV[2] = float2(halfResUV.x + UVDiffDirection.x, halfResUV.y);
		sampleUV[3] = float2(halfResUV.x, halfResUV.y + UVDiffDirection.y);	
		
		float bilinearWeight[4];
		bilinearWeight[0] = 9.0f/16.0f;
		bilinearWeight[1] = 1.0f/16.0f;
		bilinearWeight[2] = 3.0f/16.0f;
		bilinearWeight[3] = 3.0f/16.0f;
		
		#ifdef SC_DOF_MASK_ON	
		float exclusionMaskAlpha = 0.0f;
		#endif
		
		float4 upsampledDoF = 0.0f;
		float totalWeight = 0.0f;
		SCION_UNROLL for (int j = 0; j < 4; j++)
		{
			float depth = DecodeDepth01(tex2Dlod(_HalfResDepthTexture, float4(sampleUV[j], 0.0f, 0.0f)).x);
			float weight = bilinearWeight[j] / (abs(centerDepth - depth) + 1e-5f);
			float4 texSample = tex2Dlod(_DepthOfFieldTexture, float4(sampleUV[j], 0.0f, 0.0f));
			
			upsampledDoF = upsampledDoF + texSample * weight; 
			totalWeight = totalWeight + weight;
			
			#ifdef SC_DOF_MASK_ON
			float sampleValidity = tex2Dlod(_ExclusionMask, float4(sampleUV[j], 0.0f, 0.0f)).x;
			exclusionMaskAlpha = exclusionMaskAlpha + sampleValidity * weight;
			#endif
		}		
		
		#ifdef SC_DOF_MASK_ON
		float invWeight = 1.0f / totalWeight;
		exclusionMaskAlpha = exclusionMaskAlpha * invWeight;
		upsampledDoF = upsampledDoF * invWeight;	
		#else
		upsampledDoF = upsampledDoF / totalWeight;	
		#endif
		
		float tiledNeighbourhood = tex2Dlod(_TiledNeighbourhoodData, float4(i.uv, 0.0f, 0.0f)).x;
		float3 fullResSource = tex2Dlod(_FullResolutionSource, float4(i.uv, 0.0f, 0.0f)).xyz;	
		
		float searchRadiusFactor = saturate(tiledNeighbourhood * 0.5f);
		float lerpFactor = sqrt(saturate(searchRadiusFactor * upsampledDoF.w));	
		 
		#ifdef SC_DOF_MASK_ON
		lerpFactor *= exclusionMaskAlpha;
		#endif
		 
		float3 finalResult = lerp(fullResSource, upsampledDoF.xyz, lerpFactor);
		
		//return searchRadiusFactor.xxxx;
		//return upsampledDoF.wwww;
		//return lerpFactor.xxxx;
		 
		return float4(finalResult, 1.0f);
	} 
	  
	uniform float4 _DownsampleWeightedParams; 
	#define DownsampleWeightedCenter float2(_DownsampleWeightedParams.xy)
	#define DownsampleWeightedRangeSqrd _DownsampleWeightedParams.z
	#define InvDownsampleWeightedRangeSqrd _DownsampleWeightedParams.w
	
	float4 DownsampleWeighted(v2f i) : SV_Target0
	{
		float depth = tex2Dlod(_MainTex, float4(i.uv, 0.0f, 0.0f));
		
		float2 direction = i.uv - DownsampleWeightedCenter;
		direction.x *= AspectRatio;
		float distSqrd = direction.x*direction.x + direction.y*direction.y;
		float difference = DownsampleWeightedRangeSqrd - distSqrd;
		float weight = saturate(difference * InvDownsampleWeightedRangeSqrd);
				
		return float4(depth * weight, weight, 0.0f, 0.0f);
	}
	
	uniform sampler2D _PreviousWeightedResult;	
	uniform float _DownsampleWeightedAdaptionSpeed;
	
	float4 DownsampleWeightedFinalPass(v2f i) : SV_Target0
	{
		float2 depthAndWeight = tex2Dlod(_MainTex, float4(i.uv, 0.0f, 0.0f)).xy;		
		float depth = depthAndWeight.x / (depthAndWeight.y+1e-5f);
		float previousDepth = tex2Dlod(_PreviousWeightedResult, float4(0.5f, 0.5f, 0.0f, 0.0f));
		
		depth = previousDepth + (depth - previousDepth) * _DownsampleWeightedAdaptionSpeed;	
		depth = -min(-depth, 0.0f);
		
		return float4(depth, 0.0f, 0.0f, 0.0f);
	}
	
	float4 Downsample(v2f i) : SV_Target0
	{
		float2 depthAndWeight = tex2Dlod(_MainTex, float4(i.uv, 0.0f, 0.0f)).xy;
		depthAndWeight *= 4.0f;		
		return float4(depthAndWeight, 0.0f, 0.0f);
	}
	
	float4 VisualizationPass(v2f i) : SV_Target0
	{
		float4 mainTex = tex2Dlod(_MainTex, float4(i.uv, 0.0f, 0.0f));		
		return float4(mainTex.xy, 0.0f, 0.0f);
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
				Name "TiledDataHorizontal" //Pass 0
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment TiledDataHorizontal
				#pragma fragmentoption ARB_precision_hint_fastest 	
				#pragma multi_compile SC_EXPOSURE_AUTO SC_EXPOSURE_MANUAL	
				#pragma multi_compile SC_DOF_FOCUS_MANUAL SC_DOF_FOCUS_RANGE SC_DOF_FOCUS_CENTER		
				ENDCG	
			}
			Pass 
			{
				Name "TiledDataVertical" //Pass 1
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment TiledDataVertical
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
			Pass 
			{
				Name "NeighbourhoodDataGather" //Pass 2
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment NeighbourhoodDataGather
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
			Pass 
			{
				Name "MedianFilter" //Pass 3
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment MedianFilter
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
			Pass 
			{
				Name "PrefilterSource" //Pass 4
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment PrefilterSource
				#pragma fragmentoption ARB_precision_hint_fastest 
				#pragma multi_compile SC_EXPOSURE_AUTO SC_EXPOSURE_MANUAL	
				#pragma multi_compile SC_DOF_FOCUS_MANUAL SC_DOF_FOCUS_RANGE SC_DOF_FOCUS_CENTER		
				#pragma multi_compile __ SC_DOF_MASK_ON		
				ENDCG	
			}
			Pass 
			{
				Name "BlurTapPassNormalQuality" //Pass 5
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment BlurTapPass
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma multi_compile SC_EXPOSURE_AUTO SC_EXPOSURE_MANUAL	
				#pragma multi_compile SC_DOF_FOCUS_MANUAL SC_DOF_FOCUS_RANGE SC_DOF_FOCUS_CENTER	
				#pragma multi_compile __ SC_DOF_MASK_ON
		
				#define SAMPLING_OFFSET_MULT_1 (3.0f)
				#define SAMPLING_OFFSET_MULT_2 (3.0f/2.0f)
				#define SAMPLING_OFFSET_MULT_3 (3.0f/3.0f)
				#define TAPS_AMOUNT 49
				#define TAPS_ARRAY ConcentricTaps49
				//#define TAPS_ARRAY ConcentricTapsPentagon49
				
				#include "DepthOfFieldImpl.cginc"
						
				ENDCG	
			}
			Pass 
			{
				Name "UpsamplePass" //Pass 6
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment UpsamplePass
				#pragma fragmentoption ARB_precision_hint_fastest 	
				#pragma multi_compile __ SC_DOF_MASK_ON		
				ENDCG	
			}	
			Pass 
			{
				Name "DownsampleWeighted" //Pass 7 
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment DownsampleWeighted
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
			Pass 
			{
				Name "DownsampleWeightedFinalPass" //Pass 8
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment DownsampleWeightedFinalPass
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
			Pass 
			{
				Name "VisualizationPass" //Pass 9
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment VisualizationPass
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}	
			Pass 
			{
				Name "Downsample" //Pass 10
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment Downsample
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
		}
	}	
	Fallback Off	
}





