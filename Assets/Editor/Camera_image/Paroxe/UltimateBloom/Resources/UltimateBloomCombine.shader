Shader "Hidden/Ultimate/BloomCombine" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "black" {}
		_FlareTexture ("Flare (RGB)", 2D) = "black" {}
	}


	Subshader 
	{
		Pass 
 		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }      

			CGPROGRAM
			  
			

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag
			
			
#include "UnityCG.cginc"

struct v2f 
{
	half4 pos : SV_POSITION;
	half2 uv : TEXCOORD0;
};

v2f vert( appdata_img v ) 
{
	v2f o;
	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.uv =  v.texcoord.xy;
	return o;
} 

// Curve mapping
half4 _Toe;
half4 _Shoulder;
half _K;
half _Crossover;
float Map(half x)
{
	float4 data;
	float endAdd;

	if (x > _Crossover)
	{
		data = _Shoulder;
		endAdd = _K;
	}
	else
	{
		data = _Toe;
		endAdd = 0;
	}


	float2 numDenum = data.xy * x + data.zw;
	return numDenum.x / numDenum.y + endAdd;
}


sampler2D _MainTex;
sampler2D _FlareTexture;
sampler2D _ColorBuffer;
sampler2D _AdditiveTexture;
sampler2D _brightTexture;
half _Intensity;
half _FlareIntensity;
half _DirtIntensity;
half _DirtLightIntensity;
half _ScreenMaxIntensity;


fixed4 frag(v2f i):COLOR
{
	half4 addedbloom = tex2D(_MainTex, i.uv);

	half4 screencolor = tex2D(_ColorBuffer, float2(i.uv.x,i.uv.y));

	half4 bloom =  addedbloom;

#ifdef ULTIMATE_BLOOM_CURVE
	half intensity = dot(screencolor, half3(0.3,0.3,0.3));
	half bloomIntensity = Map(intensity);
	bloom *=  screencolor * bloomIntensity/intensity;
#endif
	
#ifdef ULTIMATE_USE_FLARE
	bloom += tex2D(_FlareTexture, i.uv); /* _FlareIntensity;*/
#endif

	bloom *= _Intensity;

#ifdef ULTIMATE_USE_DIRT
	half3 dirt = tex2D(_AdditiveTexture, i.uv).rgb;

	float dirtIntensity = dot(dirt.xyz, half3(0.3,0.3,0.3));
	float bloomIntensity = dot(bloom.xyz, half3(0.3,0.3,0.3));
	float factor = saturate(bloomIntensity * dirtIntensity * _DirtIntensity);

	bloom.xyz *= (dirt*_DirtIntensity + _DirtLightIntensity*0.2);

#endif




	return bloom + screencolor;


}

fixed4 fragINV(v2f i):COLOR
{
	half4 addedbloom = tex2D(_MainTex, i.uv);

	half4 screencolor = tex2D(_ColorBuffer, float2(i.uv.x,1- i.uv.y));

	half4 bloom = _Intensity * addedbloom;

#ifdef ULTIMATE_BLOOM_CURVE
	bloom.x = Map(bloom.x);
	bloom.y = Map(bloom.y);
	bloom.z = Map(bloom.z);
#endif

#ifdef ULTIMATE_USE_FLARE

	bloom += tex2D(_FlareTexture, i.uv); 

	/*half3 flare = tex2D(_FlareTexture, i.uv).rgb * 40;
	half3 flareIntensity =  dot(half3(0.3,0.3,0.3),flare);
	half flareFactor = saturate(flareIntensity );
	bloom.xyz = lerp(bloom.xyz, flare, flareFactor);*/

	//bloom += tex2D(_FlareTexture, i.uv) * 5;
#endif

#ifdef ULTIMATE_USE_DIRT
	half3 dirt = tex2D(_AdditiveTexture, i.uv).rgb;

	float dirtIntensity = dot(dirt.xyz, half3(0.3,0.3,0.3));
	float bloomIntensity = dot(bloom.xyz, half3(0.3,0.3,0.3));
	float factor = saturate(bloomIntensity * dirtIntensity * _DirtIntensity);

	bloom.xyz *= (dirt*_DirtIntensity + _DirtLightIntensity*0.2);
#endif

	return bloom + screencolor;

}



			ENDCG
		}

		Pass  
 		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }      

			CGPROGRAM

			

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment fragINV
			
			
#include "UnityCG.cginc"

struct v2f 
{
	half4 pos : SV_POSITION;
	half2 uv : TEXCOORD0;
};

v2f vert( appdata_img v ) 
{
	v2f o;
	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.uv =  v.texcoord.xy;
	return o;
} 

// Curve mapping
half4 _Toe;
half4 _Shoulder;
half _K;
half _Crossover;
float Map(half x)
{
	float4 data;
	float endAdd;

	if (x > _Crossover)
	{
		data = _Shoulder;
		endAdd = _K;
	}
	else
	{
		data = _Toe;
		endAdd = 0;
	}


	float2 numDenum = data.xy * x + data.zw;
	return numDenum.x / numDenum.y + endAdd;
}


sampler2D _MainTex;
sampler2D _FlareTexture;
sampler2D _ColorBuffer;
sampler2D _AdditiveTexture;
sampler2D _brightTexture;
half _Intensity;
half _FlareIntensity;
half _DirtIntensity;
half _DirtLightIntensity;
half _ScreenMaxIntensity;


fixed4 frag(v2f i):COLOR
{
	half4 addedbloom = tex2D(_MainTex, i.uv);

	half4 screencolor = tex2D(_ColorBuffer, float2(i.uv.x,i.uv.y));

	half4 bloom =  addedbloom;

#ifdef ULTIMATE_BLOOM_CURVE
	half intensity = dot(screencolor, half3(0.3,0.3,0.3));
	half bloomIntensity = Map(intensity);
	bloom *=  screencolor * bloomIntensity/intensity;
#endif
	
#ifdef ULTIMATE_USE_FLARE
	bloom += tex2D(_FlareTexture, i.uv); /* _FlareIntensity;*/
#endif

	bloom *= _Intensity;

#ifdef ULTIMATE_USE_DIRT
	half3 dirt = tex2D(_AdditiveTexture, i.uv).rgb;

	float dirtIntensity = dot(dirt.xyz, half3(0.3,0.3,0.3));
	float bloomIntensity = dot(bloom.xyz, half3(0.3,0.3,0.3));
	float factor = saturate(bloomIntensity * dirtIntensity * _DirtIntensity);

	bloom.xyz *= (dirt*_DirtIntensity + _DirtLightIntensity*0.2);

#endif




	return bloom + screencolor;


}

fixed4 fragINV(v2f i):COLOR
{
	half4 addedbloom = tex2D(_MainTex, i.uv);

	half4 screencolor = tex2D(_ColorBuffer, float2(i.uv.x,1- i.uv.y));

	half4 bloom = _Intensity * addedbloom;

#ifdef ULTIMATE_BLOOM_CURVE
	bloom.x = Map(bloom.x);
	bloom.y = Map(bloom.y);
	bloom.z = Map(bloom.z);
#endif

#ifdef ULTIMATE_USE_FLARE

	bloom += tex2D(_FlareTexture, i.uv); 

	/*half3 flare = tex2D(_FlareTexture, i.uv).rgb * 40;
	half3 flareIntensity =  dot(half3(0.3,0.3,0.3),flare);
	half flareFactor = saturate(flareIntensity );
	bloom.xyz = lerp(bloom.xyz, flare, flareFactor);*/

	//bloom += tex2D(_FlareTexture, i.uv) * 5;
#endif

#ifdef ULTIMATE_USE_DIRT
	half3 dirt = tex2D(_AdditiveTexture, i.uv).rgb;

	float dirtIntensity = dot(dirt.xyz, half3(0.3,0.3,0.3));
	float bloomIntensity = dot(bloom.xyz, half3(0.3,0.3,0.3));
	float factor = saturate(bloomIntensity * dirtIntensity * _DirtIntensity);

	bloom.xyz *= (dirt*_DirtIntensity + _DirtLightIntensity*0.2);
#endif

	return bloom + screencolor;

}



			ENDCG
		}
	} 
	FallBack "Diffuse"
}
