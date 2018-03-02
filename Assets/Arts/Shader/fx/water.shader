// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge

Shader "Custom/water" {
    Properties {
        _wavetex ("wavetex", 2D) = "white" {}
        _wavespeed ("wavespeed", Range(-5, 5)) = -3.034188
        _noise ("noise", 2D) = "white" {}
        _wavesp ("wavesp", Range(-5, 5)) = 0
        _wave ("wave", Float ) = 0.5
        _foam ("foam", 2D) = "white" {}
        _btmspeed ("btmspeed", Range(-5, 5)) = -0.6237352
        _foampow ("foampow", Range(0, 1)) = 0.9059829
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        Pass {
            //Name "FORWARD"
            Tags {
                //"LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            //ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
           //#pragma multi_compile_fwdbase
            //#pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            //#pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _foam; uniform float4 _foam_ST;
            uniform float _wave;
            uniform float _btmspeed;
            uniform float _wavespeed;
            uniform float _wavesp;
            uniform sampler2D _noise; uniform float4 _noise_ST;
            uniform sampler2D _wavetex; uniform float4 _wavetex_ST;
            uniform float _foampow;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
               float4 node_6441 = _Time + _TimeEditor;
                float4 node_5503 = _Time + _TimeEditor;
                float2 node_1608 = (i.uv0+(_wavesp*node_5503.r)*float2(0,1));
                float4 _noise_var = tex2D(_noise,TRANSFORM_TEX(node_1608, _noise));
                float2 node_6519 = ((i.uv0+(_noise_var.r*_wave)*float2(0,-1))+(_wavespeed*node_6441.r)*float2(1,1));
                float4 _wavetex_var = tex2D(_wavetex,TRANSFORM_TEX(node_6519, _wavetex));
                float4 node_3192 = _Time + _TimeEditor;
                float2 node_2665 = (i.uv0+(_btmspeed*node_3192.r)*float2(0,1));
                float4 _foam_var = tex2D(_foam,TRANSFORM_TEX(node_2665, _foam));
                float node_8239 = (_wavetex_var.r*step(_foampow,_foam_var.r));
                float3 emissive = float3(node_8239,node_8239,node_8239);
                float3 finalColor = emissive;
                return fixed4(finalColor,dot(node_8239,float3(0.3,0.59,0.11)));
            }
            ENDCG
        }
    }
    //FallBack "Unlit/Color"
    //CustomEditor "ShaderForgeMaterialInspector"
}
