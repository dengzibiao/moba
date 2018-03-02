// Shader created with Shader Forge v1.25 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.25;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:33507,y:32698,varname:node_3138,prsc:2|emission-6751-OUT,alpha-444-R;n:type:ShaderForge.SFN_Rotator,id:851,x:32763,y:32666,varname:node_851,prsc:2|UVIN-2392-UVOUT,ANG-5987-OUT;n:type:ShaderForge.SFN_TexCoord,id:2392,x:32533,y:32618,varname:node_2392,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:7091,x:33010,y:32680,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_7091,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-851-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:444,x:33030,y:33036,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_444,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2d20b50b3c5328548b7b85309e54e1dd,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:504,x:32422,y:33060,ptovrint:False,ptlb:RotatorSp,ptin:_RotatorSp,varname:node_504,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:1,max:2;n:type:ShaderForge.SFN_Time,id:681,x:32543,y:32853,varname:node_681,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5987,x:32817,y:32947,varname:node_5987,prsc:2|A-681-T,B-504-OUT;n:type:ShaderForge.SFN_Color,id:4861,x:33095,y:32443,ptovrint:False,ptlb:Coloer,ptin:_Coloer,varname:node_4861,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:6751,x:33280,y:32520,varname:node_6751,prsc:2|A-4861-RGB,B-7091-RGB;proporder:4861-7091-504-444;pass:END;sub:END;*/

Shader "Custom/Icon_halo" {
    Properties {
        _Coloer ("Coloer", Color) = (0.5,0.5,0.5,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _RotatorSp ("RotatorSp", Range(-2, 2)) = 1
        _Mask ("Mask", 2D) = "white" {}
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
            Blend SrcAlpha One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase            
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _RotatorSp;
            uniform float4 _Coloer;
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
                float4 node_681 = _Time + _TimeEditor;
                float node_851_ang = (node_681.g*_RotatorSp);
                float node_851_spd = 1.0;
                float node_851_cos = cos(node_851_spd*node_851_ang);
                float node_851_sin = sin(node_851_spd*node_851_ang);
                float2 node_851_piv = float2(0.5,0.5);
                float2 node_851 = (mul(i.uv0-node_851_piv,float2x2( node_851_cos, -node_851_sin, node_851_sin, node_851_cos))+node_851_piv);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_851, _MainTex));
                float3 emissive = (_Coloer.rgb*_MainTex_var.rgb);
                float3 finalColor = emissive;
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                return fixed4(finalColor,_Mask_var.r);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
