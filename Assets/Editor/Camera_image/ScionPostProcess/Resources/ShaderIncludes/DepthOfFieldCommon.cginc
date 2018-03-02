#ifndef SCION_DOF_COMMON
#define SCION_DOF_COMMON

#include "../ShaderIncludes/VirtualCameraCommon.cginc" 
	
uniform float4 _CoCParams1;		
#define PrecalcCoCScale _CoCParams1.x
#define PrecalcCoCBias _CoCParams1.y
#define FocalDistance _CoCParams1.z
#define FocalRangeLength _CoCParams1.w

uniform float4 _CoCParams2;
#define MaxCoCRadius _CoCParams2.x
#define InvHalfMaxCoCRadius _CoCParams2.y

//This is done in half res, so its actually 20 pixels
//#define MAX_COC_RADIUS 10.0f
#define MAX_COC_RADIUS MaxCoCRadius
#define INV_HALF_MAX_COC_RADIUS InvHalfMaxCoCRadius

uniform sampler2D _MainTex;
uniform sampler2D _HalfResSourceTexture;	
uniform sampler2D _TiledNeighbourhoodData;
uniform sampler2D _ExclusionMask;
uniform float4 _MainTex_TexelSize;	
uniform float _CoCUVOffset;		

uniform sampler2D _AvgCenterDepth;	
float GetFocalDistance()
{
	#ifdef SC_DOF_FOCUS_MANUAL
	return FocalDistance;
	#endif
	
	#ifdef SC_DOF_FOCUS_RANGE
	return FocalDistance;
	#endif	
	
	#ifdef SC_DOF_FOCUS_CENTER
	return DecodeDepth01(tex2Dlod(_AvgCenterDepth, float4(0.5f, 0.5f, 0.0f, 0.0f)).x);
	#endif
	
	return 0.0f;
}

float2 PreCalculatedCoC()
{
	#ifdef SC_EXPOSURE_MANUAL
	return float2(PrecalcCoCScale, PrecalcCoCBias);
	#endif

	#ifdef SC_EXPOSURE_AUTO
	return tex2Dlod(_VirtualCameraTexture2, float4(0.5f, 0.5f, 0.0f, 0.0f)).yz;		
	#endif
	
	return 0.0f;
}

float2 CalculateCoC(float focalDistance)
{
	#ifdef SC_EXPOSURE_MANUAL
	return ComputeCoCScaleAndBias(ManualFNumber, VCFocalLength, focalDistance, PixelsPerMeter);	
	//CoCScaleAndBias = ComputeCoCScaleAndBiasFromAperture(ApertureDiameter, VCFocalLength, focalDistance, PixelsPerMeter);		
	#endif
	
	#ifdef SC_EXPOSURE_AUTO
	float fNumber = tex2Dlod(_VirtualCameraTexture1, float4(0.5f, 0.5f, 0.0f, 0.0f)).w;
	return ComputeCoCScaleAndBias(fNumber, VCFocalLength, focalDistance, PixelsPerMeter);
	#endif
	
	return 0.0f;
}

float CoCFromDepthSigned(float depth, float focalDistance)
{			
	float2 CoCScaleAndBias = 0.0f; 
	
	#ifdef SC_DOF_FOCUS_MANUAL
	CoCScaleAndBias = PreCalculatedCoC();
	#endif
	
	#ifdef SC_DOF_FOCUS_RANGE
	CoCScaleAndBias = PreCalculatedCoC();

	float focalDepthDiff = focalDistance - depth; 	
	float biasedDepth = depth + FixedClamp(focalDepthDiff, -FocalRangeLength, FocalRangeLength);
	depth = biasedDepth;
	#endif
	
	#ifdef SC_DOF_FOCUS_CENTER
	CoCScaleAndBias = CalculateCoC(focalDistance);
	#endif
	
	//return abs(depth - focalDistance);
	//CoCScaleAndBias = 0.0f;
	
	float invDepth = 1.0f / depth;
	float CoC = invDepth * CoCScaleAndBias.x + CoCScaleAndBias.y;	
	return CoC;
}

float CoCFromDepth(float depth, float focalDistance)
{
	return abs(CoCFromDepthSigned(depth, focalDistance));
}

float CoCFromDepthClamped(float depth, float focalDistance)
{
	return min(CoCFromDepth(depth, focalDistance), MAX_COC_RADIUS);
}

//[-0.5, 0.5]
float2 RandomOffset(float2 uv)
{
	float2 randomOffset;
	
	#if (SHADER_API_D3D11 || SHADER_API_XBOXONE)
	float scale = 0.25f;
	float2 posMod = float2(uint2(uv) & 1);
	randomOffset = (-scale + 2.0f * scale * posMod.x) * (-1.0f + 2.0f * posMod.y); 
	#else
	float scale = 0.5f;
	float magic = 3571.0f;
	float2 random = (1.0f / 4320.0f) * uv + float2(0.25f, 0.0f);
	random = frac(dot(random*random, magic));
	randomOffset = -scale + 2.0f * scale * random;
	#endif
	
	return randomOffset * 2.0f;
}
		
float InvCircleArea(float radius)
{
	return 1.0f / (PI * radius * radius + 1e-5f);	
}

float InvCircleAreaClamped(float radius)
{
	const float MinSize = 1.0f;
	const float MaxRadius = 10.0f;
	return clamp(InvCircleArea(radius), InvCircleArea(MaxRadius), 1.0f/MinSize);
}

#endif //SCION_DOF_COMMON