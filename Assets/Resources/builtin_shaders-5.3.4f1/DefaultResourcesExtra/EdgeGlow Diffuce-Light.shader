Shader "Custom/EdgeGlow Diffuce-Light" {
    Properties {
        _MainColor("Main Color", Color) = (1,1,1,1)
        _MainTex ("Main Tex(RGB)", 2D) = "white" {}
		_RampMap("Ramp Map", 2D) = "white" {}
		_AmbientColor("Ambient", Color) = (0.2,0.2,0.2,0)
		_LightTex("Light (RGB)", 2D) = "white" {}
        _Alpha ("Alpha", Range(0, 1)) = 1
        _Blink ("Blink", Range(0, 500)) = 0
        _Shinness("Shinness", float) = 1
    }

    SubShader {
    	
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Opaque"
			"IgnoreProjector" = "True" 
		}
		
        Pass {
        	Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            uniform sampler2D _MainTex; 
            uniform float4 _MainTex_ST;
            uniform float _Alpha;
            uniform int _Blink;
            uniform float _Shinness;
            uniform float4 _MainColor;

			sampler2D _MaskTex;
			float3 _SpecColor;
			float _SpecPower;
			float _SpecMultiplier;
			sampler2D _RampMap;
			float3 _AmbientColor;
			sampler2D _LightTex;

            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
				float3 texcoord1 : TEXCOORD1;
            };
            
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
				o.uv = TRANSFORM_TEX(v.texcoord0, _MainTex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				float3 vertNormal = normalize(v.normal);
				o.texcoord1 = mul(UNITY_MATRIX_MV, float4(vertNormal, 0.0)).rgb;
                return o;
            }

            fixed4 frag(VertexOutput i) : COLOR {
				float4 mainColor = tex2D(_MainTex, i.uv);
				float2 fNormal = normalize(i.texcoord1);
				float3 albedo = ((mainColor.rgb + 0.15) * (tex2D(_LightTex, ((fNormal.xy * 0.5) + 0.5)) * 2.0).rgb) + mainColor.rgb;
				float4 finalColor;
				float3 ramp = tex2D(_RampMap, float2(0, 0.5)).rgb;
				finalColor.rgb = (ramp + _AmbientColor) * albedo;
				finalColor.a = min(_Alpha, mainColor.a);

				if (_Blink > 0)
				{
					fixed tOffest = sin(_Time.x * _Blink);
					finalColor.rgb += abs(tOffest) * .4;
				}
				finalColor = fixed4(finalColor.rgb * _MainColor.rgb, finalColor.a) * _Shinness;
                return finalColor;
            }
            ENDCG
        }
	}
    FallBack "Diffuse"
}
