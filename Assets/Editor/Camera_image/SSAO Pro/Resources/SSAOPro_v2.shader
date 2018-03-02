// SSAO Pro - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com

Shader "Hidden/SSAO Pro V2"
{
	Properties
	{
		_MainTex ("", 2D) = "white" {}
	}

	CGINCLUDE
	
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma exclude_renderers flash
		#pragma target 3.0
		#pragma glsl
		#include "UnityCG.cginc"

	ENDCG

	SubShader
	{
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }


		// --------------------------------------------------------------------------------
		// Clear (white)

		Pass // (0)
		{
			CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				
				struct v_data 
				{
					float4 pos : SV_POSITION; 
					float2 uv : TEXCOORD0;
				};

				v_data vert(appdata_img v)
				{
					v_data o;
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = v.texcoord;        	        	
					return o; 
				}

				float4 frag(v_data i) : COLOR
				{
					return float4(1.0, 1.0, 1.0, 1.0);
				}

			ENDCG
		}


		// --------------------------------------------------------------------------------
		// SSAO
		
		Pass // (1) 
		{
			CGPROGRAM

				#pragma vertex vert_ssao
				#pragma fragment frag_ssao

				#pragma multi_compile SAMPLES_VERY_LOW  SAMPLES_LOW  SAMPLES_MEDIUM  SAMPLES_HIGH  SAMPLES_ULTRA
				#pragma multi_compile HIGH_PRECISION_DEPTHMAP_ON  HIGH_PRECISION_DEPTHMAP_OFF
				
				//#define SAMPLE_NOISE
				//#define LIGHTING_CONTRIBUTION

				#include "SSAOPro.cginc"

			ENDCG
		}

		Pass // (2) 
		{
			CGPROGRAM

				#pragma vertex vert_ssao
				#pragma fragment frag_ssao

				#pragma multi_compile SAMPLES_VERY_LOW  SAMPLES_LOW  SAMPLES_MEDIUM  SAMPLES_HIGH  SAMPLES_ULTRA
				#pragma multi_compile HIGH_PRECISION_DEPTHMAP_ON  HIGH_PRECISION_DEPTHMAP_OFF
				
				#define SAMPLE_NOISE
				//#define LIGHTING_CONTRIBUTION

				#include "SSAOPro.cginc"

			ENDCG
		}

		Pass // (3) 
		{
			CGPROGRAM

				#pragma vertex vert_ssao
				#pragma fragment frag_ssao

				#pragma multi_compile SAMPLES_VERY_LOW  SAMPLES_LOW  SAMPLES_MEDIUM  SAMPLES_HIGH  SAMPLES_ULTRA
				#pragma multi_compile HIGH_PRECISION_DEPTHMAP_ON  HIGH_PRECISION_DEPTHMAP_OFF
				
				//#define SAMPLE_NOISE
				#define LIGHTING_CONTRIBUTION

				#include "SSAOPro.cginc"

			ENDCG
		}

		Pass // (4) 
		{
			CGPROGRAM

				#pragma vertex vert_ssao
				#pragma fragment frag_ssao

				#pragma multi_compile SAMPLES_VERY_LOW  SAMPLES_LOW  SAMPLES_MEDIUM  SAMPLES_HIGH  SAMPLES_ULTRA
				#pragma multi_compile HIGH_PRECISION_DEPTHMAP_ON  HIGH_PRECISION_DEPTHMAP_OFF

				#define SAMPLE_NOISE
				#define LIGHTING_CONTRIBUTION

				#include "SSAOPro.cginc"

			ENDCG
		}


		// --------------------------------------------------------------------------------
		// Gaussian Blur

		Pass // (5)
		{
			CGPROGRAM

				#pragma vertex vert_gaussian
				#pragma fragment frag_gaussian

				#include "SSAOPro.cginc"

			ENDCG
		}


		// --------------------------------------------------------------------------------
		// Bilateral Blur

		Pass // (6)
		{
			CGPROGRAM

				#pragma vertex vert_bilateral
				#pragma fragment frag_bilateral
				
				#pragma multi_compile HIGH_PRECISION_DEPTHMAP_ON  HIGH_PRECISION_DEPTHMAP_OFF

				#include "SSAOPro.cginc"

			ENDCG
		}


		// --------------------------------------------------------------------------------
		// High Quality Bilateral Blur

		Pass // (7)
		{
			CGPROGRAM

				#pragma vertex vert_hqbilateral
				#pragma fragment frag_hqbilateral
				
				#pragma multi_compile HIGH_PRECISION_DEPTHMAP_ON  HIGH_PRECISION_DEPTHMAP_OFF

				#include "SSAOPro.cginc"

			ENDCG
		}


		// --------------------------------------------------------------------------------
		// Composite

		Pass // (8)
		{
			CGPROGRAM

				#pragma vertex vert_composite
				#pragma fragment frag_composite

				#include "SSAOPro.cginc"

			ENDCG
		}
	}

	FallBack off
}
