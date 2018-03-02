Shader "Custom/TransparentScrollUV" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DetailTex("Detail Tex",2d) = ""{}
		_Speed("Speed",Vector) = (1,1,0,0)
		_Alpha("Alpha",float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf Lambert 

		sampler2D _MainTex;
		sampler2D _DetailTex;
		float4 _Speed;
		float _Alpha;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex + frac(_Time.xx * _Speed.xy));
			half4 c2 = tex2D (_DetailTex, IN.uv_MainTex + frac(_Time.xx * _Speed.zw));
			o.Albedo = c.rgb * c2;
			o.Alpha = _Alpha;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
