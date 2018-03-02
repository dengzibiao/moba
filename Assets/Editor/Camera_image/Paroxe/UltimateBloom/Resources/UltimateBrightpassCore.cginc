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

sampler2D _MainTex;
sampler2D _MaskTex;
half4 _Threshhold;

fixed4 frag(v2f i):COLOR
{
	half4 color = tex2D(_MainTex, i.uv);

	half3 tColor = max(half3(0,0,0), color.rgb-_Threshhold.rgb);
	half intensity = dot(tColor, half3(0.3,0.3,0.3));

	return clamp(color * intensity * tex2D(_MaskTex, i.uv).r,0,65000);
}

fixed4 fragNOI(v2f i):COLOR
{
	half4 color = tex2D(_MainTex, i.uv);
				
	half3 tColor = max(half3(0,0,0), color.rgb-_Threshhold.rgb);

	return half4(tColor, 1.0) * tex2D(_MaskTex, i.uv).r;

}





