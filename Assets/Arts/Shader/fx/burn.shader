// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:34186,y:32993,varname:node_3138,prsc:2|emission-3653-OUT,alpha-5325-A;n:type:ShaderForge.SFN_Tex2d,id:5325,x:33777,y:33259,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_5325,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6300bc6586dda0048ba764556db64952,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9096,x:33522,y:33295,ptovrint:False,ptlb:BurnTex,ptin:_BurnTex,varname:node_9096,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1036aea220e05214d970c49c37286a07,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Color,id:7844,x:33237,y:33165,ptovrint:False,ptlb:color,ptin:_color,varname:node_7844,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.4264706,c3:0.4264706,c4:1;n:type:ShaderForge.SFN_Multiply,id:947,x:33472,y:33133,varname:node_947,prsc:2|A-7844-RGB,B-9096-RGB;n:type:ShaderForge.SFN_ChannelBlend,id:3653,x:33960,y:33109,varname:node_3653,prsc:2,chbt:1|M-9096-R,R-2532-OUT,BTM-5325-RGB;n:type:ShaderForge.SFN_Time,id:6296,x:32652,y:32784,varname:node_6296,prsc:2;n:type:ShaderForge.SFN_Sin,id:489,x:33047,y:32849,varname:node_489,prsc:2|IN-830-OUT;n:type:ShaderForge.SFN_Vector1,id:6940,x:33237,y:33059,varname:node_6940,prsc:2,v1:3.5;n:type:ShaderForge.SFN_Multiply,id:2031,x:33251,y:32912,varname:node_2031,prsc:2|A-489-OUT,B-1722-OUT;n:type:ShaderForge.SFN_Vector1,id:1722,x:32982,y:33032,varname:node_1722,prsc:2,v1:3;n:type:ShaderForge.SFN_Add,id:7256,x:33472,y:32988,varname:node_7256,prsc:2|A-2031-OUT,B-6940-OUT;n:type:ShaderForge.SFN_Multiply,id:2532,x:33695,y:33039,varname:node_2532,prsc:2|A-7256-OUT,B-947-OUT;n:type:ShaderForge.SFN_Slider,id:2771,x:32495,y:32935,ptovrint:False,ptlb:timesp,ptin:_timesp,varname:node_2771,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Multiply,id:830,x:32886,y:32870,varname:node_830,prsc:2|A-6296-TTR,B-2771-OUT;proporder:5325-9096-7844-2771;pass:END;sub:END;*/

Shader "Custom/burn" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _BurnTex ("BurnTex", 2D) = "black" {}
        _color ("color", Color) = (1,0.4264706,0.4264706,1)
        _timesp ("timesp", Range(0, 2)) = 0
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
                        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BurnTex; uniform float4 _BurnTex_ST;
            uniform float4 _color;
            uniform float _timesp;
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
                float4 _BurnTex_var = tex2D(_BurnTex,TRANSFORM_TEX(i.uv0, _BurnTex));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 node_6296 = _Time + _TimeEditor;
                float3 node_947 = (_color.rgb*_BurnTex_var.rgb);
                float3 emissive = (lerp( _MainTex_var.rgb, (((sin((node_6296.a*_timesp))*3.0)+3.5)*node_947), _BurnTex_var.r.r ));
                float3 finalColor = emissive;
                return fixed4(finalColor,_MainTex_var.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
