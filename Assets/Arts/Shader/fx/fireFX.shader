// Shader created with Shader Forge v1.25 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.25;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:33195,y:32669,varname:node_3138,prsc:2|emission-1274-OUT,alpha-1537-A,clip-9820-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32496,y:32492,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:9363,x:32496,y:32704,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_9363,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:67e580e962bb88b4d9f5732ba3d8e8bc,ntxv:0,isnm:False|UVIN-8605-UVOUT;n:type:ShaderForge.SFN_VertexColor,id:1537,x:32496,y:32911,varname:node_1537,prsc:2;n:type:ShaderForge.SFN_TexCoord,id:7517,x:32005,y:32666,varname:node_7517,prsc:2,uv:0;n:type:ShaderForge.SFN_Panner,id:8605,x:32279,y:32711,varname:node_8605,prsc:2,spu:0,spv:1|UVIN-7517-UVOUT,DIST-9880-OUT;n:type:ShaderForge.SFN_Time,id:1218,x:31735,y:33069,varname:node_1218,prsc:2;n:type:ShaderForge.SFN_Slider,id:2688,x:31607,y:32914,ptovrint:False,ptlb:texsp,ptin:_texsp,varname:node_2688,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:0,max:2;n:type:ShaderForge.SFN_Multiply,id:9880,x:32028,y:32860,varname:node_9880,prsc:2|A-1218-TDB,B-2688-OUT;n:type:ShaderForge.SFN_Tex2d,id:5637,x:32514,y:33101,ptovrint:False,ptlb:maskclip(r),ptin:_maskclipr,varname:node_5637,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:67e580e962bb88b4d9f5732ba3d8e8bc,ntxv:0,isnm:False|UVIN-3228-UVOUT;n:type:ShaderForge.SFN_Panner,id:3228,x:32256,y:33107,varname:node_3228,prsc:2,spu:0,spv:1|UVIN-7790-UVOUT,DIST-7138-OUT;n:type:ShaderForge.SFN_TexCoord,id:7790,x:32041,y:33056,varname:node_7790,prsc:2,uv:0;n:type:ShaderForge.SFN_Slider,id:3727,x:31602,y:33293,ptovrint:False,ptlb:masksp,ptin:_masksp,varname:_texsp_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:0,max:2;n:type:ShaderForge.SFN_Multiply,id:7138,x:32017,y:33239,varname:node_7138,prsc:2|A-1218-TDB,B-3727-OUT;n:type:ShaderForge.SFN_Multiply,id:1274,x:32752,y:32605,varname:node_1274,prsc:2|A-7241-RGB,B-9363-RGB,C-1537-RGB;n:type:ShaderForge.SFN_Multiply,id:9820,x:32794,y:33070,varname:node_9820,prsc:2|A-9363-A,B-5637-A;proporder:7241-9363-2688-5637-3727;pass:END;sub:END;*/

Shader "Custom/fireFX" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _texsp ("texsp", Range(-2, 2)) = 0
        _maskclipr ("maskclip(r)", 2D) = "white" {}
        _masksp ("masksp", Range(-2, 2)) = 0
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
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _texsp;
            uniform sampler2D _maskclipr; uniform float4 _maskclipr_ST;
            uniform float _masksp;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 node_1218 = _Time + _TimeEditor;
                float2 node_8605 = (i.uv0+(node_1218.b*_texsp)*float2(0,1));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_8605, _MainTex));
                float2 node_3228 = (i.uv0+(node_1218.b*_masksp)*float2(0,1));
                float4 _maskclipr_var = tex2D(_maskclipr,TRANSFORM_TEX(node_3228, _maskclipr));
                clip((_MainTex_var.a*_maskclipr_var.a) - 0.5);
////// Lighting:
////// Emissive:
                float3 emissive = (_Color.rgb*_MainTex_var.rgb*i.vertexColor.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,i.vertexColor.a);
            }
            ENDCG
        }
   }
    //FallBack "Diffuse"
    //CustomEditor "ShaderForgeMaterialInspector"
}
