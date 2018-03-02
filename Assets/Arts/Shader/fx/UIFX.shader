// Shader created with Shader Forge v1.25 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.25;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:33012,y:32733,varname:node_3138,prsc:2|emission-7306-RGB,alpha-1169-OUT;n:type:ShaderForge.SFN_Tex2d,id:7306,x:32526,y:32610,ptovrint:False,ptlb:UItex,ptin:_UItex,varname:node_7306,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1841-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:374,x:32528,y:32873,ptovrint:False,ptlb:Alpha_a,ptin:_Alpha_a,varname:node_374,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-3409-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:8936,x:32503,y:33140,ptovrint:False,ptlb:Alpha_b,ptin:_Alpha_b,varname:node_8936,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7060-UVOUT;n:type:ShaderForge.SFN_Add,id:1169,x:32815,y:33033,varname:node_1169,prsc:2|A-374-A,B-351-OUT;n:type:ShaderForge.SFN_Panner,id:7060,x:32307,y:33165,varname:node_7060,prsc:2,spu:1,spv:0|UVIN-729-UVOUT,DIST-7009-OUT;n:type:ShaderForge.SFN_Time,id:3498,x:31776,y:32958,varname:node_3498,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:2146,x:31776,y:33324,ptovrint:False,ptlb:Alpha_a_sp02,ptin:_Alpha_a_sp02,varname:node_2146,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_TexCoord,id:3409,x:32284,y:32873,varname:node_3409,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:7009,x:32048,y:33234,varname:node_7009,prsc:2|A-3498-T,B-2146-OUT;n:type:ShaderForge.SFN_Panner,id:1841,x:32295,y:32593,varname:node_1841,prsc:2,spu:1,spv:0|UVIN-2118-UVOUT,DIST-2838-OUT;n:type:ShaderForge.SFN_TexCoord,id:2118,x:32089,y:32538,varname:node_2118,prsc:2,uv:0;n:type:ShaderForge.SFN_ValueProperty,id:7981,x:31776,y:32755,ptovrint:False,ptlb:UI_sp01,ptin:_UI_sp01,varname:_Alpha_a_sp02,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:2838,x:32089,y:32688,varname:node_2838,prsc:2|A-7981-OUT,B-3498-TSL;n:type:ShaderForge.SFN_Color,id:6517,x:32503,y:33363,ptovrint:False,ptlb:node_6517,ptin:_node_6517,varname:node_6517,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Subtract,id:351,x:32706,y:33206,varname:node_351,prsc:2|A-8936-A,B-6517-R;n:type:ShaderForge.SFN_TexCoord,id:729,x:32029,y:33083,varname:node_729,prsc:2,uv:0;proporder:7306-7981-374-8936-2146-6517;pass:END;sub:END;*/

Shader "Custom/UIFX" {
    Properties {
        _UItex ("UItex", 2D) = "white" {}
        _UI_sp01 ("UI_sp01", Float ) = 0
        _Alpha_a ("Alpha_a", 2D) = "white" {}
        _Alpha_b ("Alpha_b", 2D) = "white" {}
        _Alpha_a_sp02 ("Alpha_a_sp02", Float ) = 0
        _node_6517 ("node_6517", Color) = (0,0,0,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _UItex; uniform float4 _UItex_ST;
            uniform sampler2D _Alpha_a; uniform float4 _Alpha_a_ST;
            uniform sampler2D _Alpha_b; uniform float4 _Alpha_b_ST;
            uniform float _Alpha_a_sp02;
            uniform float _UI_sp01;
            uniform float4 _node_6517;
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
                float4 node_3498 = _Time + _TimeEditor;
                float2 node_1841 = (i.uv0+(_UI_sp01*node_3498.r)*float2(1,0));
                float4 _UItex_var = tex2D(_UItex,TRANSFORM_TEX(node_1841, _UItex));
                float3 emissive = _UItex_var.rgb;
                float3 finalColor = emissive;
                float4 _Alpha_a_var = tex2D(_Alpha_a,TRANSFORM_TEX(i.uv0, _Alpha_a));
                float2 node_7060 = (i.uv0+(node_3498.g*_Alpha_a_sp02)*float2(1,0));
                float4 _Alpha_b_var = tex2D(_Alpha_b,TRANSFORM_TEX(node_7060, _Alpha_b));
                return fixed4(finalColor,(_Alpha_a_var.a+(_Alpha_b_var.a-_node_6517.r)));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
