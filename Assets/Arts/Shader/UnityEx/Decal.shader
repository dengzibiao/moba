Shader "Custom/Decal" {
//////////////////////////////////////////////////////////////////////////////
//使用Decal.a控制 decal的rgb与mainTex.rgb的混合.
//////////////////////////////////////////////////////////////////////////////
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color("Color",Color) = (1,1,1,1)
		_Decal("Decal",2d) = ""{}
		_DecalColor("DecalColor",Color) = (1,1,1,1)
		_DecalIntensity("DecalIntensity",float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _Decal;
		fixed4 _Color;
		fixed4 _DecalColor;
		fixed _DecalIntensity;

		struct Input {
			float2 uv_MainTex;
			float2 uv_Decal;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			half4 decal = tex2D(_Decal,IN.uv_Decal) * _DecalColor;
			c.rgb = lerp(c.rgb,decal.rgb,decal.a - _DecalIntensity);
			
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
