// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.17 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.17;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:8268,x:33093,y:33010,varname:node_8268,prsc:2|emission-3094-OUT;n:type:ShaderForge.SFN_Tex2d,id:4147,x:32218,y:32808,ptovrint:False,ptlb:Tex,ptin:_Tex,varname:node_4147,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2c0b84c2ccdea9e4ebf9f699e180ddb2,ntxv:0,isnm:False|UVIN-4000-UVOUT;n:type:ShaderForge.SFN_NormalVector,id:7572,x:31739,y:33235,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:8882,x:31933,y:33154,varname:node_8882,prsc:2,dt:0|A-2892-OUT,B-7572-OUT;n:type:ShaderForge.SFN_ViewVector,id:2892,x:31739,y:33086,varname:node_2892,prsc:2;n:type:ShaderForge.SFN_Power,id:5968,x:32349,y:33219,varname:node_5968,prsc:2|VAL-5643-OUT,EXP-6970-OUT;n:type:ShaderForge.SFN_OneMinus,id:5643,x:32135,y:33180,varname:node_5643,prsc:2|IN-8882-OUT;n:type:ShaderForge.SFN_Slider,id:6970,x:31956,y:33339,ptovrint:False,ptlb:Pow,ptin:_Pow,varname:node_6970,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:3;n:type:ShaderForge.SFN_Multiply,id:3094,x:32841,y:33106,varname:node_3094,prsc:2|A-9344-OUT,B-5968-OUT;n:type:ShaderForge.SFN_TexCoord,id:3933,x:31798,y:32649,varname:node_3933,prsc:2,uv:0;n:type:ShaderForge.SFN_Time,id:7288,x:31654,y:32785,varname:node_7288,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:4547,x:31654,y:32934,ptovrint:False,ptlb:UVspd,ptin:_UVspd,varname:node_4547,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.2;n:type:ShaderForge.SFN_Multiply,id:203,x:31849,y:32841,varname:node_203,prsc:2|A-7288-T,B-4547-OUT;n:type:ShaderForge.SFN_Panner,id:4000,x:32016,y:32790,varname:node_4000,prsc:2,spu:1,spv:1|UVIN-3933-UVOUT,DIST-203-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6124,x:32430,y:33104,ptovrint:False,ptlb:pp,ptin:_pp,varname:node_6124,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_Multiply,id:9344,x:32606,y:33019,varname:node_9344,prsc:2|A-9966-OUT,B-6124-OUT;n:type:ShaderForge.SFN_Color,id:3114,x:32154,y:32982,ptovrint:False,ptlb:coloer,ptin:_color,varname:node_3114,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.9558824,c3:0.9558824,c4:1;n:type:ShaderForge.SFN_Multiply,id:9966,x:32430,y:32948,varname:node_9966,prsc:2|A-4147-RGB,B-3114-RGB;proporder:4147-3114-6970-4547-6124;pass:END;sub:END;*/

Shader "Custom/rimlight" {
    Properties {
        _Tex ("Tex", 2D) = "white" {}
        _color ("color", Color) = (1,0.9558824,0.9558824,1)
        _Pow ("Pow", Range(0, 3)) = 2
        _UVspd ("UVspd", Float ) = 0.2
        _pp ("pp", Float ) = 3
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Tex; uniform float4 _Tex_ST;
            uniform float _Pow;
            uniform float _UVspd;
            uniform float _pp;
            uniform float4 _color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 node_7288 = _Time + _TimeEditor;
                float2 node_4000 = (i.uv0+(node_7288.g*_UVspd)*float2(1,1));
                float4 _Tex_var = tex2D(_Tex,TRANSFORM_TEX(node_4000, _Tex));
                float3 emissive = (((_Tex_var.rgb*_color.rgb)*_pp)*pow((1.0 - dot(viewDirection,i.normalDir)),_Pow));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
