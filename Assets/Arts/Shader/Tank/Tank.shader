Shader "Custom/Tank" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		// _Specular ("_Specular", Range(0,1)) = 0.5
		
		_DetailMaskMap("MaskMap,(r:detail,g:specular)",2d)=""{}
		_DetailMap("DetailMap",2d)=""{}
		
		_Intensity("Intensity",float)=2.5
		_DetailIntensity("DetailIntensity",float) = 3
		_Saturate("Saturate",float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert noforwardadd nodirlightmap exclude_path:prepass 

		// Use shader model 3.0 target, to get nicer looking lighting
		// #pragma target 3.0

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};

		sampler2D _MainTex;
		fixed4 _Color;
		
		sampler2D _DetailMaskMap;
		sampler2D _DetailMap;
		half _Specular;
		
		fixed _Saturate,_Intensity,_DetailIntensity;
		const fixed3 w = fixed3(0.2,0.7,0.07);

		fixed4 Luminance1(sampler2D tex,fixed2 uv,fixed intensity){
			fixed4 c = tex2D(tex,uv);
			c = lerp(fixed4(0,0,0,0),c,intensity);
			return c;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = Luminance1(_MainTex,IN.uv_MainTex,_Intensity);
			o.Alpha = c.a;
			
			// detail
			fixed detailMask = tex2D(_DetailMaskMap,IN.uv_MainTex).r;
			fixed4 detailColor = Luminance1(_DetailMap,IN.uv_MainTex,_DetailIntensity);
			c.rgb = lerp(c.rgb,c.rgb*detailColor.rgb,detailMask) * _Color;
			fixed gray = dot(w,c.rgb);
			o.Albedo = lerp(fixed3(gray,gray,gray),c.rgb,_Saturate);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
