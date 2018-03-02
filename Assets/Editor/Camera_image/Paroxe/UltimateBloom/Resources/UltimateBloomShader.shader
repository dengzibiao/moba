Shader "Hidden/Ultimate/Bloom"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_AdditiveTexture ("Base (RGB)", 2D) = "black" {}
		_OffsetInfos ("HorizontalOffset", Vector) = (0.0,0.0,0.0,0.0)
	}
	 
	CGINCLUDE
	 
	#pragma target 3.0

	#include "UnityCG.cginc"

	struct v2f 
	{
		half4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
	};

	struct v2f_opts 
	{
		half4 pos : SV_POSITION;
		half2 uv[7] : TEXCOORD0;
	};

	uniform half4 _MainTex_TexelSize;

	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv =  v.texcoord.xy;
		return o;
	} 



	float4 _OffsetInfos;
	sampler2D _MainTex;
	half _Intensity;
    sampler2D _ColorBuffer;
	sampler2D _AdditiveTexture;
	sampler2D _FlareTexture;
	half4 _Threshhold;

	float Gaussian(float Scale, int iSamplePoint)
	{
		float sigma = (Scale-1.0)/5;
		float g = 1.0f / sqrt(2.0f * 3.14159 * sigma * sigma);  
		return (g * exp(-(iSamplePoint * iSamplePoint) / (2 * sigma * sigma)));
	}


	half4 fragGaussBlurHigh (v2f i) : SV_Target 
	{
		half4 color = half4 (0,0,0,0);

		float Scale = 31;

		float2 gUV = i.uv;
		float Offset = 0;

		color += Gaussian(Scale, 0.0 + Offset) * tex2D (_MainTex, gUV);
		color += Gaussian(Scale, 1.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 1.0);
		color += Gaussian(Scale, 1.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 1.0);
		color += Gaussian(Scale, 2.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 2.0);
		color += Gaussian(Scale, 2.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 2.0);
		color += Gaussian(Scale, 3.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 3.0);
		color += Gaussian(Scale, 3.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 3.0);	
		color += Gaussian(Scale, 4.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 4.0);
		color += Gaussian(Scale, 4.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 4.0);	
		color += Gaussian(Scale, 5.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 5.0);
		color += Gaussian(Scale, 5.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 5.0);	
		color += Gaussian(Scale, 6.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 6.0);
		color += Gaussian(Scale, 6.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 6.0);	
		color += Gaussian(Scale, 7.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 7.0);
		color += Gaussian(Scale, 7.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 7.0);	
		color += Gaussian(Scale, 8.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 8.0);
		color += Gaussian(Scale, 8.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 8.0);	
		color += Gaussian(Scale, 9.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 9.0);
		color += Gaussian(Scale, 9.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 9.0);	
		color += Gaussian(Scale, 10.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 10.0);
		color += Gaussian(Scale, 10.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 10.0);	
		color += Gaussian(Scale, 11.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 11.0);
		color += Gaussian(Scale, 11.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 11.0);	
		color += Gaussian(Scale, 12.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 12.0);
		color += Gaussian(Scale, 12.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 12.0);
		color += Gaussian(Scale, 13.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 13.0);
		color += Gaussian(Scale, 13.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 13.0);	
		color += Gaussian(Scale, 14.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 14.0);
		color += Gaussian(Scale, 14.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 14.0);	
		color += Gaussian(Scale, 15.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 15.0);
		color += Gaussian(Scale, 15.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 15.0);		

		return color + tex2D(_AdditiveTexture, i.uv);
	}

	half4 fragGaussBlurMedium (v2f i) : SV_Target 
	{
		half4 color = half4 (0,0,0,0);

		float Scale = 17;

		float2 gUV = i.uv;
		float Offset = 0;

		color += Gaussian(Scale, 0.0 + Offset) * tex2D (_MainTex, gUV);
		color += Gaussian(Scale, 1.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 1.0);
		color += Gaussian(Scale, 1.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 1.0);
		color += Gaussian(Scale, 2.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 2.0);
		color += Gaussian(Scale, 2.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 2.0);
		color += Gaussian(Scale, 3.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 3.0);
		color += Gaussian(Scale, 3.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 3.0);	
		color += Gaussian(Scale, 4.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 4.0);
		color += Gaussian(Scale, 4.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 4.0);	
		color += Gaussian(Scale, 5.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 5.0);
		color += Gaussian(Scale, 5.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 5.0);	
		color += Gaussian(Scale, 6.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 6.0);
		color += Gaussian(Scale, 6.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 6.0);	
		color += Gaussian(Scale, 7.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 7.0);
		color += Gaussian(Scale, 7.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 7.0);	
		color += Gaussian(Scale, 8.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 8.0);
		color += Gaussian(Scale, 8.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 8.0);	

		return color + tex2D(_AdditiveTexture, i.uv);
	}

	half4 fragGaussBlurLow (v2f i) : SV_Target 
	{
		half4 color = half4 (0,0,0,0);

		float Scale = 9;

		float2 gUV = i.uv;
		float Offset = 0;

		color += Gaussian(Scale, 0.0 + Offset) * tex2D (_MainTex, gUV);
		color += Gaussian(Scale, 1.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 1.0);
		color += Gaussian(Scale, 1.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 1.0);
		color += Gaussian(Scale, 2.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 2.0);
		color += Gaussian(Scale, 2.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 2.0);
		color += Gaussian(Scale, 3.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 3.0);
		color += Gaussian(Scale, 3.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 3.0);	
		color += Gaussian(Scale, 4.0 + Offset) * tex2D (_MainTex, gUV + _OffsetInfos.xy * 4.0);
		color += Gaussian(Scale, 4.0 + Offset) * tex2D (_MainTex, gUV - _OffsetInfos.xy * 4.0);	

		return color + tex2D(_AdditiveTexture, i.uv);
	}

	ENDCG

	SubShader
	{
		Pass // #0 Simple Downscaling
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"


			fixed4 frag(v2f i):COLOR
			{
			 
			 
				float2 UV[4];

				UV[0] = i.uv + float2(-1.0 * _OffsetInfos.x, -1.0 * _OffsetInfos.y);
				UV[1] = i.uv + float2( 1.0 * _OffsetInfos.x, -1.0 * _OffsetInfos.y);
				UV[2] = i.uv + float2(-1.0 * _OffsetInfos.x,  1.0 * _OffsetInfos.y);
				UV[3] = i.uv + float2( 1.0 * _OffsetInfos.x,  1.0 * _OffsetInfos.y);

				
				fixed4 Sample[4];

				for(int j = 0; j < 4; ++j)
				{
					Sample[j] = tex2D(_MainTex, UV[j]);
				}

				return (Sample[0] + Sample[1] + Sample[2] + Sample[3]) * 1.0/4;
			}

			ENDCG
		}

		Pass // #1 Gaussian Sampling High
		{
			CGPROGRAM
      
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
			#pragma vertex vert_img
			#pragma fragment fragGaussBlurHigh
      
			ENDCG
		}

		Pass // #2 Gaussian Sampling Medium
		{
			CGPROGRAM
      
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
			#pragma vertex vert_img
			#pragma fragment fragGaussBlurMedium
      
			ENDCG
		}

		Pass // #3 Gaussian Sampling Low
		{
			CGPROGRAM
      
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
			#pragma vertex vert_img
			#pragma fragment fragGaussBlurLow
      
			ENDCG
		}

		Pass // #4 Color Brightpass
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"

			

			fixed4 frag(v2f i):COLOR
			{
				half4 color = tex2D(_MainTex, i.uv);
				
				half3 tColor = max(half3(0,0,0), color.rgb-_Threshhold.rgb);
				//half intensity = dot(tColor, float3(0.212671, 0.71516, 0.072169));
				half intensity = dot(tColor, half3(0.3,0.3,0.3));

				return color * intensity;
			}

			ENDCG
		}

		Pass // #5 Blend Add
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"

			

			fixed4 frag(v2f i):COLOR
			{
				half4 addedbloom = tex2D(_MainTex, i.uv);
				half4 screencolor = tex2D(_ColorBuffer, i.uv);
				return _Intensity * addedbloom + screencolor;
			}

			ENDCG
		}

		Pass // #6 Blend Screen
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"


			fixed4 frag(v2f i):COLOR
			{
				half4 addedbloom = tex2D(_MainTex, i.uv);
				half4 screencolor = tex2D(_ColorBuffer, i.uv);
				return _Intensity * addedbloom + screencolor;
			}

			ENDCG
		}

		Pass // #7 Add One One
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }

			Blend One One
			
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"


			fixed4 frag(v2f i):COLOR
			{
				half4 addedColors = tex2D(_MainTex, i.uv.xy);
				return addedColors * _Intensity;
			}

			ENDCG
		}

		Pass // #8 Render Flare
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }

			
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"

			half4 _FlareScales;
			half4 _FlareTint0;
			half4 _FlareTint1;
			half4 _FlareTint2;
			half4 _FlareTint3;

			half2 cUV(half2 uv)
			{
				return 2.0 * uv - float2(1.0,1.0);
			}

			half2 tUV(half2 uv)
			{
				return (uv + float2(1.0,1.0))*0.5;
			}

			fixed4 frag(v2f i):COLOR
			{
				half scale0 = _FlareScales.x;//1.1f;
				half scale1 =  _FlareScales.y;//0.95f;
				half scale2 =  _FlareScales.z;//0.75f;
				half scale3 =  _FlareScales.w;//0.55f;

				half2 flareUv = cUV(float2(1.0,1.0) - i.uv);

				float4 col0 = tex2D(_MainTex, tUV(flareUv*scale0) ) * _FlareTint0;
				float4 col1 = tex2D(_MainTex, tUV(flareUv*scale1) ) * _FlareTint1;
				float4 col2 = tex2D(_MainTex, tUV(flareUv*scale2) ) * _FlareTint2;
				float4 col3 = tex2D(_MainTex, tUV(flareUv*scale3) ) * _FlareTint3;

				// Optional..­.
				flareUv = cUV(i.uv);
				float4 col4 = tex2D(_MainTex, tUV(flareUv*scale0) ) * _FlareTint0;
				float4 col5 = tex2D(_MainTex, tUV(flareUv*scale1) ) * _FlareTint1;
				float4 col6 = tex2D(_MainTex, tUV(flareUv*scale2) ) * _FlareTint2; 
				float4 col7 = tex2D(_MainTex, tUV(flareUv*scale3) ) * _FlareTint3;

				return (col0 + col1 + col2 + col3 + col4 + col5 + col6 + col7) * tex2D(_FlareTexture,i.uv);
			}

			ENDCG
		}

		Pass // #9 Blend Add with flares
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"

			half _FlareIntensity;

			fixed4 frag(v2f i):COLOR
			{
				half4 addedbloom = tex2D(_MainTex, i.uv);
				//half4 screencolor = tex2D(_ColorBuffer, float2(i.uv.x,1- i.uv.y));
				half4 screencolor = tex2D(_ColorBuffer, float2(i.uv.x,i.uv.y));
				half4 bloom = _Intensity * addedbloom + tex2D(_FlareTexture, i.uv) * _FlareIntensity;

				half3 dirt = tex2D(_AdditiveTexture, i.uv).rgb * bloom * 1000;
				//bloom.rgb -= dirt;
				return bloom + screencolor + float4(dirt,1.0) ; 
			}

			ENDCG
		}

		Pass // #10 Complex Downscaling
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"


			fixed4 frag(v2f i):COLOR
			{
			 
			 
				float2 UV[9];

				UV[0] = i.uv;

				UV[1] = i.uv + float2( -2.0 * _OffsetInfos.x, -2.0 * _OffsetInfos.y);
				UV[2] = i.uv + float2( 0.0 * _OffsetInfos.x,  -2.0 * _OffsetInfos.y);
				UV[3] = i.uv + float2( 2.0 * _OffsetInfos.x,  -2.0 * _OffsetInfos.y);
				UV[4] = i.uv + float2( -2.0 * _OffsetInfos.x, 2.0 * _OffsetInfos.y);
				UV[5] = i.uv + float2( 0.0 * _OffsetInfos.x,  2.0 * _OffsetInfos.y);
				UV[6] = i.uv + float2( 2.0 * _OffsetInfos.x,  2.0 * _OffsetInfos.y);
				UV[7] = i.uv + float2( -2.0 * _OffsetInfos.x,  0.0 * _OffsetInfos.y);
				UV[8] = i.uv + float2( 2.0 * _OffsetInfos.x,  0.0 * _OffsetInfos.y);

				
				fixed4 Sample[9];

				for(int j = 0; j < 9; ++j)
				{
					Sample[j] = tex2D(_MainTex, UV[j]);
				}

				return (Sample[0] + Sample[1] + Sample[2] + Sample[3] + Sample[4] + Sample[5] + Sample[6] + Sample[7] + Sample[8]) * 1.0/9;
			}

			ENDCG
		}

		Pass // #11 Blend Add with flares Inverted Source (for forward MSAA)
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"

			half _FlareIntensity;

			fixed4 frag(v2f i):COLOR
			{
				half4 addedbloom = tex2D(_MainTex, i.uv);
				half4 screencolor = tex2D(_ColorBuffer, float2(i.uv.x,1- i.uv.y));
				half4 bloom = _Intensity * addedbloom + tex2D(_FlareTexture, i.uv) * _FlareIntensity;

				half3 dirt = tex2D(_AdditiveTexture, i.uv).rgb * bloom * 1000;
				return bloom + screencolor + float4(dirt,1.0) ; 
			}

			ENDCG
		}

	}

	FallBack off
}
