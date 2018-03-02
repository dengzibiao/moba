Shader "Hidden/ScionDepthOfFieldDX11" 
{	   	
 	Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
       	
	CGINCLUDE
	#include "UnityCG.cginc" 
	#include "../ShaderIncludes/ScionCommon.cginc" 
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
				Name "BlurTapPassHighQuality" //Pass 0
			
				CGPROGRAM	
				#pragma target 4.0		
				#pragma vertex vert
				#pragma fragment BlurTapPass
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma multi_compile SC_EXPOSURE_AUTO SC_EXPOSURE_MANUAL	
				#pragma multi_compile SC_DOF_FOCUS_MANUAL SC_DOF_FOCUS_RANGE SC_DOF_FOCUS_CENTER	
				#pragma multi_compile __ SC_DOF_MASK_ON		
				
				#define SC_DOF_QUALITY_MAX						
				#define SAMPLING_OFFSET_MULT_1 (4.0f)
				#define SAMPLING_OFFSET_MULT_2 (4.0f/2.0f)
				#define SAMPLING_OFFSET_MULT_3 (4.0f/3.0f)
				#define SAMPLING_OFFSET_MULT_4 (4.0f/4.0f)
				#define TAPS_ARRAY ConcentricTaps81
				#define TAPS_AMOUNT 81
				
//				#define SAMPLING_OFFSET_MULT_1 (3.0f)
//				#define SAMPLING_OFFSET_MULT_2 (3.0f/2.0f)
//				#define SAMPLING_OFFSET_MULT_3 (3.0f/3.0f)
//				#define TAPS_AMOUNT 49
//				#define TAPS_ARRAY ConcentricTaps49
				
				#include "DepthOfFieldImpl.cginc"
					
				ENDCG	
			}
		}
	}	
	Fallback Off	
}





