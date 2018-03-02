// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/HeroShow" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MaskTex ("Mask (R,G,B)", 2D) = "white" {}
		_SpecColor ("Spec Color", Color) = (0,0,0,0)
		_SpecPower ("Spec Power", Range(1,128)) = 15
		_SpecMultiplier ("Spec Multiplier", Float) = 1
		_RampMap ("Ramp Map", 2D) = "white" {}
		_AmbientColor ("Ambient", Color) = (0.2,0.2,0.2,0)
		_LightTex ("Light (RGB)", 2D) = "white" {}
//		_NormalTex ("Normal", 2D) = "bump" {}
		_NoiseTex ("Noise(RGB)", 2D) = "white" {}
		_Scroll2X ("Noise speed X", Float) = 1
		_Scroll2Y ("Noise speed Y", Float) = 0
		_NoiseColor ("Noise Color", Color) = (1,1,1,1)
		_MMultiplier ("Layer Multiplier", Float) = 2
		_ReflectTex ("Reflect(RGB)", 2D) = "white" {}
		_ReflectColor ("Reflect Color", Color) = (1,1,1,1)
		_ReflectPower ("Reflect Power", Range(0.1,5)) = 1
		_ReflectionMultiplier ("Reflection Multiplier", Float) = 2
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" "HeroShader"="1" }
		LOD 200
 		Pass 
 		{
	  		Tags { "RenderType"="Opaque" "HeroShader"="1" }
	  		Fog { Mode Off }		
			
			CGPROGRAM
			#define ALLOW_ALL_EFFECT 1
			#pragma vertex vert
			#pragma fragment frag
			
#ifdef ALLOW_ALL_EFFECT
#else
			#pragma multi_compile _NORMALMAP_OFF _NORMALMAP_ON
			#pragma multi_compile _NOISETEX_OFF _NOISETEX_ON
			#pragma multi_compile _REFLECTTEX_OFF _REFLECTTEX_ON
			#pragma multi_compile _SGAME_HEROSHOW_SHADOW_OFF _SGAME_HEROSHOW_SHADOW_ON
#endif
			#include "UnityCG.cginc"
			
			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord0 : TEXCOORD0;
				float4 tangent : TANGENT;
			};
			
			struct voutput
			{
				float4 pos : SV_POSITION;
				float4 texcoord0 : TEXCOORD0;
				float3 texcoord1 : TEXCOORD1;
				float3 texcoord2 : TEXCOORD2;
			};
			
			float _Scroll2X;
			float _Scroll2Y;
			float4 _MainTex_ST;
			float4 _NoiseTex_ST;
			
			voutput vert (appdata v) 
			{
				voutput o;
				UNITY_INITIALIZE_OUTPUT(voutput, o);
						  	
			  	float4 mainUV_NoiseUV;
			  	mainUV_NoiseUV.xy = TRANSFORM_TEX(v.texcoord0, _MainTex);
			  	
#ifdef ALLOW_ALL_EFFECT
				mainUV_NoiseUV.zw = (TRANSFORM_TEX(v.texcoord0, _NoiseTex) + frac((float2(_Scroll2X, _Scroll2Y) * _Time.x)));
#else
			  	#if _NOISETEX_ON
			  	mainUV_NoiseUV.zw = (TRANSFORM_TEX(v.texcoord0, _NoiseTex) + frac((float2(_Scroll2X, _Scroll2Y) * _Time.x)));
			  	#elif _NOISETEX_OFF
			  	mainUV_NoiseUV.zw = mainUV_NoiseUV.xy;
			  	#endif
#endif  				  	
			  	float3 vertNormal = normalize(v.normal);			  		
			  	float3 vertTangent = normalize(v.tangent.xyz);			  		  	
			  	float3 binormal = (((vertNormal.yzx * vertTangent.zxy) - (vertNormal.zxy * vertTangent.yzx)) * v.tangent. w);
			  	float3x3 rotation = float3x3(vertTangent, binormal, vertNormal);				
			    float4 viewDir = float4((_WorldSpaceCameraPos - mul(unity_ObjectToWorld,  v.vertex).xyz), 0);

			    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			    o.texcoord0 = mainUV_NoiseUV;
			    o.texcoord1 = mul(UNITY_MATRIX_MV, float4(vertNormal, 0.0)).xyz;
			    o.texcoord2 = normalize(mul(rotation, mul(unity_WorldToObject, viewDir).xyz));
			    return o;
		    }
	    
			float _MMultiplier;
			float _SpecPower;
			float _SpecMultiplier;
			float _ReflectPower;
			float _ReflectionMultiplier;
			float3 _AmbientColor;
			float3 _NoiseColor;
			float3 _SpecColor;
			float3 _ReflectColor;
			sampler2D _MainTex;
		 	sampler2D _MaskTex;
		 	sampler2D _LightTex;
//		 	sampler2D _NormalTex;
		 	sampler2D _NoiseTex;
		 	sampler2D _ReflectTex;
		 	sampler2D _RampMap;  
	    
		    fixed4 frag(voutput i) : COLOR
		    {
				float4 mainColor = tex2D (_MainTex, i.texcoord0.xy);
				float4 maskColor = tex2D (_MaskTex, i.texcoord0.xy);

				float2 fNormal = normalize(i.texcoord1);
				float3 albedo = ((mainColor.xyz + 0.15) * (tex2D (_LightTex, ((fNormal.xy * 0.5) + 0.5)) * 2.0).xyz) + mainColor.xyz;

#ifdef ALLOW_ALL_EFFECT
				float3 noiseColor = tex2D (_NoiseTex, i.texcoord0.zw).xyz;				
				noiseColor = ((noiseColor * (mainColor.xyz * _NoiseColor)) * (maskColor.y * _MMultiplier));
				albedo = albedo + noiseColor;
#else
				#if _NOISETEX_ON
				float3 noiseColor = tex2D (_NoiseTex, i.texcoord0.zw).xyz;				
				noiseColor = ((noiseColor * (mainColor.xyz * _NoiseColor)) * (maskColor.y * _MMultiplier));
				albedo = albedo + noiseColor;
				#endif
#endif				

#ifdef ALLOW_ALL_EFFECT
				float4 reflectColor = tex2D (_ReflectTex, ((fNormal.xy * 0.5) + 0.5));
				albedo = lerp (albedo, ((albedo * pow ((reflectColor.xyz * _ReflectColor), _ReflectPower))
								 * _ReflectionMultiplier), maskColor.z);
#else
				#if _REFLECTTEX_ON
				float4 reflectColor = tex2D (_ReflectTex, ((fNormal.xy * 0.5) + 0.5));
				albedo = lerp (albedo, ((albedo * pow ((reflectColor.xyz * _ReflectColor), _ReflectPower))
								 * _ReflectionMultiplier), maskColor.z);
				#endif
#endif

				float4 finalColor;
				float3 ramp = tex2D (_RampMap, float2(0, 0.5)).xyz;
				finalColor.xyz = (ramp + _AmbientColor) * albedo;
				finalColor.w = mainColor.w;
				
				float nh;				
//#ifdef ALLOW_ALL_EFFECT
				nh = max (0.0, normalize(i.texcoord2).z);
//#else
//				#if _NORMALMAP_ON
//				float3 normalTS = normalize(((tex2D (_NormalTex, i.texcoord0.xy).xyz * 2.0) - 1.0));
//				nh = max (0.0, dot (normalTS, normalize((i.texcoord2))));
//				#elif _NORMALMAP_OFF
//				nh = max (0.0, normalize(i.texcoord2).z);
//				#endif
//#endif
				
				float3 specColor = (_SpecColor * ((((pow (nh, _SpecPower) * maskColor.x) * _SpecMultiplier) * 1.0)
									* 2.0));
				finalColor.xyz = finalColor.xyz + specColor;								
				
				return finalColor;    
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
