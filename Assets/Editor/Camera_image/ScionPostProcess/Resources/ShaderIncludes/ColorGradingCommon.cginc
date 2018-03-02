#ifndef SCION_COLOR_GRADING_COMMON
#define SCION_COLOR_GRADING_COMMON

uniform sampler2D _ColorGradingLUT1;	
uniform sampler2D _ColorGradingLUT2;	
uniform float _ColorGradingBlendFactor;

uniform float4 _ColorGradingParams1;
#define UVScale 		float3(_ColorGradingParams1.xyz)
#define NumSlices 		_ColorGradingParams1.w

uniform float4 _ColorGradingParams2;
#define UVOffset 		float3(_ColorGradingParams2.xyz)
#define InvNumSlices 	_ColorGradingParams2.w

float3 ApplyColorGrading(float3 clr, sampler2D LUTSampler)
{
	float3 uvw = clr * UVScale + UVOffset;
	float3 uvwFrac = frac(uvw);
	float3 uvwFloor = uvw - uvwFrac;

	float2 uvFloor = uvw.xy + float2(uvwFloor.z * InvNumSlices, 0.0f);
	float2 uvCeil = uvFloor + float2(InvNumSlices, 0.0f);
	float3 sampleFloor = tex2Dlod(LUTSampler, float4(uvFloor, 0.0f, 0.0f)).xyz;
	float3 sampleCeil = tex2Dlod(LUTSampler, float4(uvCeil, 0.0f, 0.0f)).xyz;

	return lerp(sampleFloor, sampleCeil, uvwFrac.z);
}

float3 ApplyColorGrading(float3 clr, sampler2D LUTSampler1, sampler2D LUTSampler2, float blendFactor)
{
	float3 uvw = clr * UVScale + UVOffset;
	float3 uvwFrac = frac(uvw);
	float3 uvwFloor = uvw - uvwFrac;

	float2 uvFloor = uvw.xy + float2(uvwFloor.z * InvNumSlices, 0.0f);
	float2 uvCeil = uvFloor + float2(InvNumSlices, 0.0f);
	float3 sampleFloor = lerp(tex2Dlod(LUTSampler1, float4(uvFloor, 0.0f, 0.0f)).xyz, tex2Dlod(LUTSampler2, float4(uvFloor, 0.0f, 0.0f)).xyz, blendFactor);
	float3 sampleCeil = lerp(tex2Dlod(LUTSampler1, float4(uvCeil, 0.0f, 0.0f)).xyz, tex2Dlod(LUTSampler2, float4(uvCeil, 0.0f, 0.0f)).xyz, blendFactor);

	return lerp(sampleFloor, sampleCeil, uvwFrac.z);
}

#endif // #ifndef SCION_COLOR_GRADING_COMMON