Shader "Custom/EdgesGlow_Z" {
    Properties {
        _MainColor("Main Color", Color) = (1,1,1,1)
        _MainTex ("Main Tex(RGB)", 2D) = "white" {}
        _Alpha ("Alpha", Range(0, 1)) = 1
        _Blink ("Blink", Range(0, 500)) = 0
        
        _Shinness("Shinness", float) = 1

		_BumpMap("Normalmap", 2D) = "bump" {}
		_OccColor("Occlusion Color", Color) = (0, 0.58, 0.99, 1)
		_OccPower("Occlusion Power", Range(0.0, 2.0)) = 0.44
		_Alpha("Alpha", Range(0, 1)) = 1
    }


    SubShader {
    	
		Tags
		{ 
			"Queue" = "Transparent"
			"RenderType" = "Opaque"
			"IgnoreProjector" = "True" 
		}

		Pass
		{
			Name "BASE"
			Blend SrcAlpha OneMinusSrcAlpha
			Fog{ Mode Off }
			Lighting Off
			ZWrite Off
			ZTest Greater

			CGPROGRAM
			#include "UnityCG.cginc"  
			#pragma vertex vert  
			#pragma fragment frag  
			#pragma fragmentoption ARB_precision_hint_fastest  

			sampler2D _BumpMap;
			fixed4 _OccColor;
			fixed _OccPower;

			struct a2v
			{
				fixed4 vertex : POSITION;
				fixed3 normal : NORMAL;
				fixed4 tangent : TANGENT;
				fixed4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				fixed4 pos : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				fixed3 viewDir : TEXCOORD1;
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy;

				TANGENT_SPACE_ROTATION;
				o.viewDir = normalize(mul(rotation, ObjSpaceViewDir(v.vertex)));

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 n = UnpackNormal(tex2D(_BumpMap, i.uv));
				fixed o = 1 - saturate(dot(n, i.viewDir));
				fixed4 c = _OccColor * pow(o, _OccPower);

				return c;
			}

			ENDCG
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
                float2 uv0 : TEXCOORD0;
            };
            
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            
            fixed4 frag(VertexOutput i) : COLOR {
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                _MainTex_var.a = min(_Alpha, _MainTex_var.a);
                if(_Blink > 0)
                {
                	fixed tOffest = sin(_Time.x * _Blink);
                	_MainTex_var.rgb += abs(tOffest) * .4;
                } 
                return fixed4(_MainTex_var.rgb * _MainColor.rgb, _MainTex_var.a) * _Shinness;
            }
            ENDCG
        }
    }
			
    FallBack "Diffuse"
}

