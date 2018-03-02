// SSAO Pro - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com

#ifndef SSAO_PRO_INCLUDED
#define SSAO_PRO_INCLUDED

	// --------------------------------------------------------------------------------
	// Uniforms

	sampler2D _MainTex;
	half4 _MainTex_TexelSize;
	sampler2D _SSAOTex;

	sampler2D_float _DepthNormalMapF32; // High precision depth map (Unity 4 only)
	sampler2D_float _CameraDepthTexture;
	sampler2D_float _CameraDepthNormalsTexture;
		
	half4x4 _InverseViewProject;
	half4x4 _CameraModelView;

	sampler2D _NoiseTex;
	half4 _Params1; // Noise Size / Sample Radius / Intensity / Distance
	half4 _Params2; // Bias / Luminosity Contribution / Distance Cutoff / Cutoff Falloff
	half4 _OcclusionColor;

	half2 _Direction;
	half _BilateralThreshold;


	// --------------------------------------------------------------------------------
	// Functions

	inline half invlerp(half from, half to, half value)
	{
		return (value - from) / (to - from);
	}

	inline half getDepth(half2 uv)
	{
		#if HIGH_PRECISION_DEPTHMAP_OFF
		return tex2D(_CameraDepthTexture, uv).x;
		#elif HIGH_PRECISION_DEPTHMAP_ON
		return tex2D(_DepthNormalMapF32, uv).x;
		#endif

		return 0;
	}

	inline half3 getWSPosition(half2 uv, half depth)
	{
		// Compute world space position from the view depth
		half4 pos = half4(uv.xy * 2.0 - 1.0, depth, 1.0);
		half4 ray = mul(_InverseViewProject, pos);
		return ray.xyz / ray.w;
	}

	inline half3 getWSNormal(half2 uv)
	{
		// Get the view space normal and convert it to world space
		half3 nn = tex2D(_CameraDepthNormalsTexture, uv).xyz * half3(3.5554, 3.5554, 0) + half3(-1.7777, -1.7777, 1.0);
		half g = 2.0 / dot(nn.xyz, nn.xyz);
		half3 vsnormal = half3(g * nn.xy, g - 1.0); // View space
		half3 wsnormal = mul((half3x3)_CameraModelView, vsnormal); // World space
		return wsnormal;
	}

	inline half calcAO(half2 tcoord, half2 uv, half3 p, half3 cnorm)
	{
		half2 t = tcoord + uv;
		half depth = getDepth(t);
		half3 diff = getWSPosition(t, depth) - p; // World space
		half3 v = normalize(diff);
		half d = length(diff) * _Params1.w;
		return max(0.0, dot(cnorm, v) - _Params2.x) * (1.0 / (1.0 + d)) * _Params1.z;
	}

	half ssao(half2 uv)
	{
		const half2 CROSS[4] = { half2(1.0, 0.0), half2(-1.0, 0.0), half2(0.0, 1.0), half2(0.0, -1.0) };
			
		half depth = getDepth(uv);
		half eyeDepth = LinearEyeDepth(depth);

		half3 position = getWSPosition(uv, depth); // World space
		half3 normal = getWSNormal(uv); // World space

		#if defined(SAMPLE_NOISE)
		half2 random = normalize(tex2D(_NoiseTex, _ScreenParams.xy * uv / _Params1.x).rg * 2.0 - 1.0);
		#endif

		half radius = max(_Params1.y / eyeDepth, 0.005);
		clip(_Params2.z - eyeDepth); // Skip out of range pixels
		half ao = 0.0;

		// Sampling
		for (int j = 0; j < 4; j++)
		{
			half2 coord1;

			#if defined(SAMPLE_NOISE)
			coord1 = reflect(CROSS[j], random) * radius;
			#else
			coord1 = CROSS[j] * radius;
			#endif

			#if !SAMPLES_VERY_LOW
			half2 coord2 = coord1 * 0.707;
			coord2 = half2(coord2.x - coord2.y, coord2.x + coord2.y);
			#endif
  
			#if SAMPLES_ULTRA			// 20
			ao += calcAO(uv, coord1 * 0.20, position, normal);
			ao += calcAO(uv, coord2 * 0.40, position, normal);
			ao += calcAO(uv, coord1 * 0.60, position, normal);
			ao += calcAO(uv, coord2 * 0.80, position, normal);
			ao += calcAO(uv, coord1, position, normal);
			#elif SAMPLES_HIGH			// 16
			ao += calcAO(uv, coord1 * 0.25, position, normal);
			ao += calcAO(uv, coord2 * 0.50, position, normal);
			ao += calcAO(uv, coord1 * 0.75, position, normal);
			ao += calcAO(uv, coord2, position, normal);
			#elif SAMPLES_MEDIUM		// 12
			ao += calcAO(uv, coord1 * 0.30, position, normal);
			ao += calcAO(uv, coord2 * 0.60, position, normal);
			ao += calcAO(uv, coord1 * 0.90, position, normal);
			#elif SAMPLES_LOW			// 8
			ao += calcAO(uv, coord1 * 0.30, position, normal);
			ao += calcAO(uv, coord2 * 0.80, position, normal);
			#elif SAMPLES_VERY_LOW		// 4
			ao += calcAO(uv, coord1 * 0.50, position, normal);
			#endif
		}
		
		#if SAMPLES_ULTRA
		ao /= 20.0;
		#elif SAMPLES_HIGH
		ao /= 16.0;
		#elif SAMPLES_MEDIUM
		ao /= 12.0;
		#elif SAMPLES_LOW
		ao /= 8.0;
		#elif SAMPLES_VERY_LOW
		ao /= 4.0;
		#endif

		// Distance cutoff
		ao = lerp(1.0 - ao, 1.0, saturate(invlerp(_Params2.z - _Params2.w, _Params2.z, eyeDepth)));

		return ao;
	}


	// --------------------------------------------------------------------------------
	// SSAO

	struct v_data_simple
	{
		float4 pos : SV_POSITION; 
		float2 uv : TEXCOORD0;
					
		#if UNITY_UV_STARTS_AT_TOP
		float2 uv2 : TEXCOORD1;
		#endif
	};

	v_data_simple vert_ssao(appdata_img v)
	{
		v_data_simple o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord;
        	
		#if UNITY_UV_STARTS_AT_TOP
		o.uv2 = v.texcoord;
		if (_MainTex_TexelSize.y < 0.0)
			o.uv.y = 1.0 - o.uv.y;
		#endif
        	        	
		return o; 
	}

	half4 getAOColor(half ao, half2 uv)
	{
		#if defined(LIGHTING_CONTRIBUTION)

		// Luminance for the current pixel, used to reduce the AO amount in bright areas
		// Could potentially be replaced by the lighting pass in Deferred...
		half3 color = tex2D(_MainTex, uv).rgb;
		half luminance = dot(color, half3(0.299, 0.587, 0.114));
		half aofinal = lerp(ao, 1.0, luminance * _Params2.y);
		return half4(aofinal, aofinal, aofinal, 1.0);

		#else

		return half4(ao, ao, ao, 1.0);

		#endif
	}

	half4 frag_ssao(v_data_simple i) : SV_Target
	{
		#if UNITY_UV_STARTS_AT_TOP
		return saturate(getAOColor(ssao(i.uv), i.uv2) + _OcclusionColor);
		#else
		return saturate(getAOColor(ssao(i.uv), i.uv) + _OcclusionColor);
		#endif
	}


	// --------------------------------------------------------------------------------
	// Gaussian Blur

	struct v_data_blur
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float4 uv1 : TEXCOORD1;
		float4 uv2 : TEXCOORD2;
	};

	v_data_blur vert_gaussian(appdata_img v)
	{
		v_data_blur o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
		float2 d1 = 1.3846153846 * _Direction;
		float2 d2 = 3.2307692308 * _Direction;
		o.uv1 = float4(o.uv + d1, o.uv - d1);
		o.uv2 = float4(o.uv + d2, o.uv - d2);
		return o;
	}

	float4 frag_gaussian(v_data_blur i) : SV_Target
	{
		half3 c = tex2D(_MainTex, i.uv).rgb * 0.2270270270;
		c += tex2D(_MainTex, i.uv1.xy).rgb * 0.3162162162;
		c += tex2D(_MainTex, i.uv1.zw).rgb * 0.3162162162;
		c += tex2D(_MainTex, i.uv2.xy).rgb * 0.0702702703;
		c += tex2D(_MainTex, i.uv2.zw).rgb * 0.0702702703;
		return half4(c, 1.0);
	}


	// --------------------------------------------------------------------------------
	// Bilateral Blur

	v_data_blur vert_bilateral(appdata_img v)
	{
		v_data_blur o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
		float2 d2 = 2.0 * _Direction;
		o.uv1 = float4(o.uv + _Direction, o.uv - _Direction);
		o.uv2 = float4(o.uv + d2, o.uv - d2);
		return o;
	}

	float4 frag_bilateral(v_data_blur i) : SV_Target
	{
		half4 depthTmp, coeff;
		half depth = Linear01Depth(getDepth(i.uv));
		half3 c = tex2D(_MainTex, i.uv).rgb;
		
		depthTmp.x = Linear01Depth(getDepth(i.uv1.xy));
		depthTmp.y = Linear01Depth(getDepth(i.uv1.zw));
		depthTmp.z = Linear01Depth(getDepth(i.uv2.xy));
		depthTmp.w = Linear01Depth(getDepth(i.uv2.zw));
		coeff = 1.0 / (1e-06 + abs(depth - depthTmp));
		c += tex2D(_MainTex, i.uv1.xy).rgb * coeff.x;
		c += tex2D(_MainTex, i.uv1.zw).rgb * coeff.y;
		c += tex2D(_MainTex, i.uv2.xy).rgb * coeff.z;
		c += tex2D(_MainTex, i.uv2.zw).rgb * coeff.w;

		c /= (coeff.x + coeff.y + coeff.z + coeff.w);
		return half4(c, 1.0);
	}


	// --------------------------------------------------------------------------------
	// High Quality Bilateral Blur

	v_data_blur vert_hqbilateral(appdata_img v)
	{
		v_data_blur o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
		float2 d1 = 1.3846153846 * _Direction;
		float2 d2 = 3.2307692308 * _Direction;
		o.uv1 = float4(o.uv + d1, o.uv - d1);
		o.uv2 = float4(o.uv + d2, o.uv - d2);
		return o;
	}

	float4 frag_hqbilateral(v_data_blur i) : SV_Target
	{	
		const half2 uvs[4] = { i.uv1.xy, i.uv1.zw, i.uv2.xy, i.uv2.zw };

		half depth = Linear01Depth(getDepth(i.uv));
		half3 accum = tex2D(_MainTex, i.uv).rgb * 0.2270270270;
		half accumWeight = 0.2270270270;

		half4 depthTmp;
		depthTmp.x = Linear01Depth(getDepth(uvs[0]));
		depthTmp.y = Linear01Depth(getDepth(uvs[1]));
		depthTmp.z = Linear01Depth(getDepth(uvs[2]));
		depthTmp.w = Linear01Depth(getDepth(uvs[3]));
		half4 diff = abs(depth - depthTmp);
		half4 weight = (1.0 - step(_BilateralThreshold, diff)) * half4(0.3162162162, 0.3162162162, 0.0702702703, 0.0702702703);

		for (int i = 0; i < 4; i++)
		{
			accum += weight[i] * tex2D(_MainTex, uvs[i]).rgb;
			accumWeight += weight[i];
		}

		return half4(accum / accumWeight, 1.0);
	}


	// --------------------------------------------------------------------------------
	// Composite

	v_data_simple vert_composite(appdata_img v)
	{
		v_data_simple o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord;
        	
		#if UNITY_UV_STARTS_AT_TOP
		o.uv2 = v.texcoord;
		if (_MainTex_TexelSize.y < 0.0)
			o.uv.y = 1.0 - o.uv.y;
		#endif
        	        	
		return o; 
	}

	half4 frag_composite(v_data_simple i) : SV_Target
	{
		#if UNITY_UV_STARTS_AT_TOP
		half4 color = tex2D(_MainTex, i.uv2).rgba;
		#else
		half4 color = tex2D(_MainTex, i.uv).rgba;
		#endif
		
		return half4(color.rgb * tex2D(_SSAOTex, i.uv).rgb, color.a);
	}

#endif // SSAO_PRO_INCLUDED
