#ifndef SCION_COMMON
#define SCION_COMMON

#define PI 3.14159265359f

//This is the same as Unitys HLSL support, but added manually to support Unity 5.0 as well
#ifdef UNITY_COMPILER_HLSL
	#define SCION_BRANCH	[branch]
	#define SCION_FLATTEN	[flatten]
	#define SCION_UNROLL	[unroll]
	#define SCION_LOOP		[loop]
	#define SCION_FASTOPT	[fastopt]
#else
	#define SCION_BRANCH
	#define SCION_FLATTEN
	#define SCION_UNROLL
	#define SCION_LOOP
	#define SCION_FASTOPT
#endif

float  Square(float value) 	{ return value * value; }
float2 Square(float2 value) { return value * value; }
float3 Square(float3 value) { return value * value; }
float4 Square(float4 value) { return value * value; }

float DistanceSquared(float value1, float value2) 		{ value1 = value1 - value2; return dot(value1, value1); }
float DistanceSquared(float2 value1, float2 value2) 	{ value1 = value1 - value2; return dot(value1, value1); }
float DistanceSquared(float3 value1, float3 value2) 	{ value1 = value1 - value2; return dot(value1, value1); }
float DistanceSquared(float4 value1, float4 value2) 	{ value1 = value1 - value2; return dot(value1, value1); }

float VectorMax(float2 value) { return max(value.x, value.y); }
float VectorMax(float3 value) { return max(value.x, max(value.y, value.z)); }
float VectorMax(float4 value) { return max(VectorMax(value.xy), VectorMax(value.zw)); }

float VectorMin(float2 value) { return min(value.x, value.y); }
float VectorMin(float3 value) { return min(value.x, min(value.y, value.z)); }
float VectorMin(float4 value) { return min(VectorMin(value.xy), VectorMin(value.zw)); }

//Had issues with clamp in DX9, workaround
float FixedClamp(float value, float minValue, float maxValue)
{
	#ifdef SHADER_API_D3D9 
	value = max(value, minValue);
	value = min(value, maxValue);
	#else
	value = clamp(value, minValue, maxValue);
	#endif
	return value;
}
	
uniform sampler2D _HalfResDepthTexture;

uniform float4 _ScionResolutionParameters1;
#define HalfResWidth _ScionResolutionParameters1.x
#define HalfResHeight _ScionResolutionParameters1.y
#define FullResWidth _ScionResolutionParameters1.z
#define FullResHeight _ScionResolutionParameters1.w

#define HalfResSize float2(HalfResWidth, HalfResHeight)
#define FullResSize float2(FullResWidth, FullResHeight)

uniform float4 _ScionResolutionParameters2;
#define InvHalfResWidth _ScionResolutionParameters2.x
#define InvHalfResHeight _ScionResolutionParameters2.y
#define InvFullResWidth _ScionResolutionParameters2.z
#define InvFullResHeight _ScionResolutionParameters2.w

#define InvHalfResSize float2(InvHalfResWidth, InvHalfResHeight)
#define InvFullResSize float2(InvFullResWidth, InvFullResHeight)

uniform float4 _ScionNearFarParams;
#define NearPlane _ScionNearFarParams.x
#define FarPlane _ScionNearFarParams.y
#define InvFarPlane _ScionNearFarParams.z
#define NearPlaneTimesInvFarPlane _ScionNearFarParams.w

static const float2 Poisson33[33] = { float2(0.0f, 0.0f), float2( -0.02068222f, -0.04876602f), 
			float2( -0.3897605f, 0.2394096f), float2( -0.2249043f, -0.4151815f), float2( 0.468218f, 0.2127425f), 
			float2( -0.06608687f, 0.3672915f), float2( 0.2839191f, -0.4402199f), float2( 0.5071067f, -0.2257781f), 
			float2( -0.5964095f, 0.01200622f), float2( -0.0197941f, -0.6359698f), float2( -0.528392f, -0.3399327f), 
			float2( 0.2421739f, 0.4146926f), float2( -0.2779354f, -0.8280222f), float2( -0.818099f, -0.4398771f), 
			float2( -0.6348993f, -0.7099844f), float2( 0.2701441f, -0.752901f), float2( 0.03076914f, -0.9479648f), 
			float2( -0.3152059f, -0.1135688f), float2( 0.7479827f, 0.005718112f), float2( 0.8372025f, -0.4992182f), 
			float2( 0.5679177f, -0.6375975f), float2( 0.9663672f, -0.210884f), float2( 0.7753006f, 0.5263627f), 
			float2( 0.9595293f, 0.2307652f), float2( -0.9109734f, -0.09067982f), float2( -0.8470589f, 0.2442961f), 
			float2( -0.4651257f, 0.5571797f), float2( -0.7671899f, 0.5414186f), float2( -0.001862288f, 0.6835418f), 
			float2( 0.5226822f, 0.7215486f), float2( 0.2336524f, 0.942329f), float2( -0.1887437f, 0.9296713f), 
			float2( -0.5202904f, 0.8526009f) };

inline float DecodeDepth01(float depth01)
{
	//return NearPlane + depth01 * FarPlane;
	return depth01 * FarPlane;
}

inline float EncodeDepth01(float depth)
{
	//return depth * InvFarPlane - NearPlaneTimesInvFarPlane;
	return depth * InvFarPlane;
}

float VignetteMask(float2 uv, float scale, float intensity)
{
	uv = uv * 2.0f - 1.0f;	
	uv = uv * scale;
	
	float vignetteMask = saturate(1.0f - dot(uv, uv));
	vignetteMask = vignetteMask * vignetteMask;
	return lerp(1.0f, vignetteMask, intensity);	
}

//This is a workaround for the shader compiler sometimes spitting out this: 'VignetteMask' : no matching overloaded function found
float VignetteMask(float2 uv, float scale)
{
	return VignetteMask(uv, scale, 1.0f);
}

float3 Vignette(float3 clr, float2 uv, float scale, float intensity, float3 vignetteColor)
{
	float vignetteMask = VignetteMask(uv, scale, intensity);
	return lerp(vignetteColor*clr, clr, vignetteMask);
}

float UVBasedNoise01(float2 uv, float seed = 6238672.3f)
{
	return frac(sin(uv.x + uv.y * 541.17f + seed) * 273351.5f - seed);	
}

float3 Grain(float3 clr, float2 uv, float grainIntensity, float grainSeed)
{
	float grain = UVBasedNoise01(uv, grainSeed);	
	grain = grain * 2.0f - 1.0f;
	grain = lerp(0.0f, grain, grainIntensity);		
	return clr + grain * clr;	
}

float2 VignettedDistortion(float2 uv, float strength, float scale) 
{
	float vignetteMask = VignetteMask(uv, scale);
	vignetteMask = InvFullResSize - InvFullResSize * vignetteMask;
	return uv + vignetteMask * strength;
}

float3 SpectrumOffset(float iterStep) 
{
	//Compiler should flatten
	float isFirstHalf = step(iterStep, 0.49f);
	float isSecondHalf = 1.0f - isFirstHalf;
		
	float3 binaryWeights = float3(isFirstHalf, 1.0f, isSecondHalf);
	
	float smoothLerp = 1.0f - abs(2.0f*iterStep - 1.0f);	
	float3 smoothWeights = float3(1.0f - smoothLerp, smoothLerp, 1.0f - smoothLerp);		
	
	return smoothWeights * binaryWeights;
}

float Luma601(float3 clr)
{
	return dot(clr, float3(0.299f, 0.587f, 0.114f));
}

float Luma709(float3 clr)
{
	return dot(clr, float3(0.2126f, 0.7152f, 0.0722f));
}

float Luma(float3 clr)
{
	#if 0
	return Luma709(clr);
	#else
	return Luma601(clr);
	#endif
}

float Luma(float4 clr) 
{
	return Luma(clr.xyz);
}

float LumaWeight(float3 clr) 
{
	return 1.0f / (1.0f + Luma(clr.xyz));
}
	
float LumaWeightInverse(float value) 
{
	return value / (1.0f - value);
}

float4 LumaWeighted(float3 clr)
{
	float weight = LumaWeight(clr);
	return float4(clr.xyz * weight, weight);
}
float4 LumaWeighted(float4 clr) { return LumaWeighted(clr.xyz); }

float4 BilateralLumaWeighted(float3 clr, float bilateralWeight)
{
	float weight = LumaWeight(clr) * bilateralWeight;
	return float4(clr.xyz * weight, weight);
}

float4 LumaWeightedLerp(float4 clr0, float4 clr1, float lerpValue) 
{
	//float lerpValue0 = LumaWeight(clr0.xyz) * (1.0f - lerpValue);
	//MAD optimized version
	float lumaWeight0 = LumaWeight(clr0.xyz);
	float lerpValue0 =  lumaWeight0 - lumaWeight0*lerpValue;	
	
	float lerpValue1 = LumaWeight(clr1.xyz) * lerpValue;
	float lerpNormalizer = 1.0f / (lerpValue0 + lerpValue1);
	
	lerpValue0 = lerpValue0 * lerpNormalizer;
	lerpValue1 = lerpValue1 * lerpNormalizer;
	
	return lerpValue0 * clr0 + lerpValue1 * clr1;
}

float3 LumaWeightedLerp(float3 clr0, float3 clr1, float lerpValue) 
{
	//float lerpValue0 = LumaWeight(clr0.xyz) * (1.0f - lerpValue);
	//MAD optimized version
	float lumaWeight0 = LumaWeight(clr0.xyz);
	float lerpValue0 =  lumaWeight0 - lumaWeight0*lerpValue;	
	
	float lerpValue1 = LumaWeight(clr1.xyz) * lerpValue;
	float lerpNormalizer = 1.0f / (lerpValue0 + lerpValue1);
	
	lerpValue0 = lerpValue0 * lerpNormalizer;
	lerpValue1 = lerpValue1 * lerpNormalizer;
	
	return lerpValue0 * clr0 + lerpValue1 * clr1;
}
		
float3 AccurateSRGBToLinear(float3 sRGBCol)
{
	float3 linearRGBLo = sRGBCol / 12.92f;
	float3 linearRGBHi = pow(( sRGBCol + 0.055f) / 1.055f, 2.4f);
	float3 linearRGB = ( sRGBCol <= 0.04045f) ? linearRGBLo : linearRGBHi;
	return linearRGB ;
}

float3 AccurateLinearToSRGB(float3 linearCol)
{
	float3 sRGBLo = linearCol * 12.92f;
	float3 sRGBHi = (pow(abs(linearCol), 1.0f/2.4f) * 1.055f) - 0.055f;
	float3 sRGB = (linearCol <= 0.0031308f) ? sRGBLo : sRGBHi;
	return sRGB;
}

#endif // #ifndef SCION_COMMON