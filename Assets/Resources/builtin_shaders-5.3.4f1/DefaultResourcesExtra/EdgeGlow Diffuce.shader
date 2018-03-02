Shader "Custom/EdgesGlow" {
    Properties {
        _MainColor("Main Color", Color) = (1,1,1,1)
        _MainTex ("Main Tex(RGB)", 2D) = "white" {}
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

            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
				o.uv = TRANSFORM_TEX(v.texcoord0, _MainTex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }

            fixed4 frag(VertexOutput i) : COLOR {
				float4 _MainTex_var = tex2D(_MainTex, i.uv);
				_MainTex_var.a = min(_Alpha, _MainTex_var.a);
				if (_Blink > 0)
				{
					fixed tOffest = sin(_Time.x * _Blink);
					_MainTex_var.rgb += abs(tOffest) * .4;
				}
				_MainTex_var = fixed4(_MainTex_var.rgb * _MainColor.rgb, _MainTex_var.a) * _Shinness;
                return _MainTex_var;
            }
            ENDCG
        }
	}
    FallBack "Diffuse"
}
