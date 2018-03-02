Shader "Custom/TankSpec" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalMap("NormalMap",2d) = ""{}
		
		_SpecColor("Spec Color",color)=(1,1,1,1)
		_Specular ("Specular", Range(0.04,0.9)) = 0.5
		_Gloss("Gloss",float) = 1
		
		_DetailMaskMap("DetailMaskMap",2d)="black"{} //r 4 detailMask,g 4 specular
		_DetailMap("DetailMap",2d)=""{}
		
		_Intensity("Intensity",float)=2.5
		_DetailIntensity("DetailIntensity",float) = 3
		_Saturate("Saturate",float) = 1
		
		_LightDir("LightDir",Vector) = (1,1,0,0)
		_LightColor("LightColor",Color)=(1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf BlinnPhong1 noforwardadd nodirlightmap exclude_path:prepass 

		// Use shader model 3.0 target, to get nicer looking lighting
		// #pragma target 3.0

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};
		struct SurfaceOutput1{
			fixed3 Albedo;  // diffuse color
			fixed3 Normal;  // tangent space normal, if written
			fixed3 Emission;
			half Specular;  // specular power in 0..1 range
			fixed Gloss;    // specular intensity
			fixed Alpha;    // alpha for transparencies
			fixed SpecularMask;
		};

		sampler2D _MainTex;
		
		fixed4 _Color;
		
		sampler2D _DetailMaskMap;
		sampler2D _DetailMap;
		
		sampler2D _NormalMap;
		half _Specular;
		fixed _Gloss;
		fixed4 _LightDir;
		fixed4 _LightColor;
		
		fixed _Saturate,_Intensity,_DetailIntensity;
		const fixed3 w = fixed3(0.2,0.7,0.2);
		

		inline fixed4 UnityBlinnPhongLight1 (SurfaceOutput1 s, half3 viewDir, UnityLight light)
		{
			half3 dir = normalize(_LightDir.xyz);
			half3 h = normalize (dir + viewDir);
			
			fixed diff = max (0, dot (s.Normal, dir));
			
			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, s.Specular*128.0) * s.Gloss * s.SpecularMask;
			if(diff == 0.0)
				spec = 0;
			fixed4 c;
			c.rgb = s.Albedo * _LightColor.rgb * diff + _LightColor.rgb * _SpecColor.rgb * spec;
			c.a = s.Alpha;

			return c;
		}

		inline fixed4 LightingBlinnPhong1 (SurfaceOutput1 s, half3 viewDir, UnityGI gi)
		{
			fixed4 c;
			c = UnityBlinnPhongLight1 (s, viewDir, gi.light);

			#if defined(DIRLIGHTMAP_SEPARATE)
				#ifdef LIGHTMAP_ON
					c += UnityBlinnPhongLight1 (s, viewDir, gi.light2);
				#endif
				#ifdef DYNAMICLIGHTMAP_ON
					c += UnityBlinnPhongLight1 (s, viewDir, gi.light3);
				#endif
			#endif

			#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
				c.rgb += s.Albedo * gi.indirect.diffuse;
			#endif

			return c;
		}
		inline void LightingBlinnPhong1_GI (
			SurfaceOutput1 s,
			UnityGIInput data,
			inout UnityGI gi)
		{
			gi = UnityGlobalIllumination (data, 1.0, s.Gloss, s.Normal, false);
		}
		inline half4 LightingBlinnPhong1_Deferred (SurfaceOutput1 s, half3 viewDir, UnityGI gi, out half4 outDiffuseOcclusion, out half4 outSpecSmoothness, out half4 outNormal)
		{
			outDiffuseOcclusion = half4(s.Albedo, 1);
			outSpecSmoothness = half4(_SpecColor.rgb, s.Specular) * s.SpecularMask;
			outNormal = half4(s.Normal * 0.5 + 0.5, 1);
			half4 emission = half4(s.Emission, 1);

			#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
				emission.rgb += s.Albedo * gi.indirect.diffuse;
			#endif
			
			return emission;
		}
		inline fixed4 LightingBlinnPhong1_PrePass (SurfaceOutput1 s, half4 light)
		{
		fixed spec = light.a * s.Gloss;

		fixed4 c;
		c.rgb = (s.Albedo * light.rgb + light.rgb * _SpecColor.rgb * spec);
		c.a = s.Alpha;
		return c;
		}
		
		fixed4 Luminance1(sampler2D tex,fixed2 uv,fixed intensity){
			fixed4 c = tex2D(tex,uv);
			c = lerp(fixed4(0,0,0,0),c,intensity);
			return c;
		}
		
		void surf (Input IN, inout SurfaceOutput1 o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = Luminance1(_MainTex,IN.uv_MainTex,_Intensity);
			// detail
			fixed4 detailMask = tex2D(_DetailMaskMap,IN.uv_MainTex);
			
			// Metallic and smoothness come from slider variables
			o.Specular = _Specular;
			o.SpecularMask = detailMask.g;
			// o.Normal = UnpackNormal(tex2D(_NormalMap,IN.uv_MainTex));
			//Detail 
			fixed4 detailColor = Luminance1(_DetailMap,IN.uv_MainTex,_DetailIntensity);
			
			c.rgb = lerp(c.rgb,(c.rgb + detailColor.rgb * detailColor.rgb) * 0.5,detailMask.r) * _Color;

			fixed gray = dot(w,c.rgb);
			o.Albedo = lerp(fixed3(gray,gray,gray),c.rgb,_Saturate);
			
			o.Gloss = _Gloss;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
