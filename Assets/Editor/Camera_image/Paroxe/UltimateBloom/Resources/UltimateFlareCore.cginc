
#include "UnityCG.cginc"

struct v2f 
{
	half4 pos : SV_POSITION;
	half2 uv : TEXCOORD0;
	half4 uv0 : TEXCOORD1;
	half4 uv1 : TEXCOORD2;
	half4 uv2 : TEXCOORD3;
	half4 uv3 : TEXCOORD4;
};



sampler2D _MainTex;
sampler2D _FlareTexture;
half _Intensity;

half4 _FlareScales;
half4 _FlareScalesNear;
half4 _FlareTint0;
half4 _FlareTint1;
half4 _FlareTint2;
half4 _FlareTint3;
half4 _FlareTint4;
half4 _FlareTint5;
half4 _FlareTint6;
half4 _FlareTint7;

half2 cUV(half2 uv)
{
	return 2.0 * uv - float2(1.0,1.0);
}

half2 tUV(half2 uv)
{
	return (uv + float2(1.0,1.0))*0.5;
}

v2f vert( appdata_img v ) 
{
	v2f o;
	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.uv =  v.texcoord.xy;

	half scale0 = _FlareScales.x;
	half scale1 =  _FlareScales.y;
	half scale2 =  _FlareScales.z;
	half scale3 =  _FlareScales.w;

	half2 flareUv = cUV(half2(1.0,1.0) - o.uv);
	o.uv0.xy = tUV(flareUv*scale0);
	o.uv1.xy = tUV(flareUv*scale1); 
	o.uv2.xy = tUV(flareUv*scale2);
	o.uv3.xy = tUV(flareUv*scale3);

	half scale4 = _FlareScalesNear.x;
	half scale5 =  _FlareScalesNear.y;
	half scale6 =  _FlareScalesNear.z;
	half scale7 =  _FlareScalesNear.w;

	flareUv = cUV(o.uv);
	o.uv0.zw = tUV(flareUv*scale4);
	o.uv1.zw = tUV(flareUv*scale5);
	o.uv2.zw = tUV(flareUv*scale6);
	o.uv3.zw = tUV(flareUv*scale7);

	return o;
} 

fixed4 frag(v2f i):COLOR
{
	half2 flareUv = cUV(float2(1.0,1.0) - i.uv);

	float4 acc = float4(0,0,0,0);

	acc += tex2D(_MainTex, i.uv0.xy ) * _FlareTint0;
	acc += tex2D(_MainTex, i.uv1.xy ) * _FlareTint1;
	acc += tex2D(_MainTex, i.uv2.xy ) * _FlareTint2;
	acc += tex2D(_MainTex, i.uv3.xy ) * _FlareTint3;

#ifdef FLARE_DOUBLE
	flareUv = cUV(i.uv);

	acc += tex2D(_MainTex, i.uv0.zw ) * _FlareTint4;
	acc += tex2D(_MainTex, i.uv1.zw  ) * _FlareTint5;
	acc += tex2D(_MainTex, i.uv2.zw ) * _FlareTint6; 
	acc += tex2D(_MainTex, i.uv3.zw ) * _FlareTint7;
#endif

	return clamp(acc *_Intensity,0, 65000);
}


