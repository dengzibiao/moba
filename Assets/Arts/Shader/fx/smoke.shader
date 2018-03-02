// Shader created with Shader Forge v1.25 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge

Shader "Custom/smokerBG" {
    Properties {
        _sp ("sp", Range(0, 1)) = 1
        _tex ("tex", 2D) = "white" {}
        _DisPOWER ("DisPOWER", Range(-3, 3)) = 0.3578987
        _node_2350 ("node_2350", 2D) = "white" {}
        _coloer ("coloer", Color) = (0.5,0.5,0.5,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            // Name "FORWARD"
            // Tags {
                // "LightMode"="ForwardBase"
            // }
            Blend SrcAlpha OneMinusSrcAlpha
            //ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            // #pragma multi_compile_fwdbase
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float _sp;
            uniform sampler2D _tex; uniform float4 _tex_ST;
            uniform float _DisPOWER;
            uniform sampler2D _node_2350; uniform float4 _node_2350_ST;
            uniform float4 _coloer;
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
                float4 node_5660 = _Time + _TimeEditor;
                float node_6145 = (_sp*node_5660.r);
                float2 node_1991 = (i.uv0+node_6145*float2(-1,-1));
                float4 node_3238 = tex2D(_tex,node_1991);
                float node_8142_ang = node_6145;
                float node_8142_spd = 1.0;
                float node_8142_cos = cos(node_8142_spd*node_8142_ang);
                float node_8142_sin = sin(node_8142_spd*node_8142_ang);
                float2 node_8142_piv = float2(0.5,0.5);
                float2 node_8142 = (mul(i.uv0-node_8142_piv,float2x2( node_8142_cos, -node_8142_sin, node_8142_sin, node_8142_cos))+node_8142_piv);
                float4 node_3944 = tex2D(_tex,node_8142);
                float node_3150 = (node_3238.r*node_3944.r);
                float node_375 = (node_3150*_DisPOWER);
                float2 node_3649 = (i.uv0+node_375*float2(1,1));
                float4 node_7316 = tex2D(_tex,node_3649);
                float4 node_4139 = tex2D(_tex,i.uv0);
                float3 node_7885 = (node_7316.rgb*node_4139.rgb);
                float4 _node_2350_var = tex2D(_node_2350,TRANSFORM_TEX(i.uv0, _node_2350));
                float node_6956 = (dot(node_7885,float3(0.3,0.59,0.11))*_node_2350_var.r*1.0);
                float3 emissive = (_coloer.rgb*node_6956);
                float3 finalColor = emissive;
                return fixed4(finalColor,node_6956);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
   // CustomEditor "ShaderForgeMaterialInspector"
}
