// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.16 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.16;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:2865,x:32775,y:32661,varname:node_2865,prsc:2|diff-3825-OUT,spec-3135-OUT,gloss-3135-OUT,normal-370-OUT,amspl-8749-OUT,alpha-2958-OUT,refract-6902-OUT;n:type:ShaderForge.SFN_Tex2d,id:7736,x:31974,y:32423,ptovrint:True,ptlb:Base Color,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a4081536bb9f80b4ab9b27acc019203c,ntxv:2,isnm:False|UVIN-4608-OUT;n:type:ShaderForge.SFN_Tex2d,id:5964,x:31953,y:32916,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:930538e31f5dd454e8482952cc861585,ntxv:3,isnm:True|UVIN-4608-OUT;n:type:ShaderForge.SFN_Tex2d,id:2941,x:31276,y:32993,ptovrint:False,ptlb:noize,ptin:_noize,varname:node_2941,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8924227a4fcf69f4f8d38de371a74822,ntxv:0,isnm:False|UVIN-7849-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:6149,x:30824,y:32940,varname:node_6149,prsc:2,uv:0;n:type:ShaderForge.SFN_Panner,id:7849,x:31059,y:32961,varname:node_7849,prsc:2,spu:0,spv:0.1|UVIN-6149-UVOUT;n:type:ShaderForge.SFN_Multiply,id:7888,x:31462,y:32899,varname:node_7888,prsc:2|A-7129-OUT,B-2941-R;n:type:ShaderForge.SFN_Slider,id:7129,x:30995,y:32825,ptovrint:False,ptlb:noize intensity,ptin:_noizeintensity,varname:node_7129,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Add,id:4608,x:31641,y:32741,varname:node_4608,prsc:2|A-2136-UVOUT,B-7888-OUT;n:type:ShaderForge.SFN_TexCoord,id:2136,x:31385,y:32692,varname:node_2136,prsc:2,uv:0;n:type:ShaderForge.SFN_Lerp,id:370,x:32251,y:32835,varname:node_370,prsc:2|A-5888-OUT,B-5964-RGB,T-428-OUT;n:type:ShaderForge.SFN_Vector3,id:5888,x:31953,y:32783,varname:node_5888,prsc:2,v1:0,v2:0,v3:0.5;n:type:ShaderForge.SFN_Slider,id:428,x:31702,y:33124,ptovrint:False,ptlb:refraction  Intensity,ptin:_refractionIntensity,varname:node_428,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8164595,max:1;n:type:ShaderForge.SFN_ComponentMask,id:655,x:32232,y:32969,varname:node_655,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-5964-RGB;n:type:ShaderForge.SFN_Multiply,id:1175,x:32232,y:33150,varname:node_1175,prsc:2|A-428-OUT,B-1735-OUT;n:type:ShaderForge.SFN_Vector1,id:1735,x:31741,y:33220,varname:node_1735,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Multiply,id:6902,x:32420,y:33120,varname:node_6902,prsc:2|A-655-OUT,B-1175-OUT;n:type:ShaderForge.SFN_Vector1,id:2958,x:32437,y:33013,varname:node_2958,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Vector1,id:8749,x:32437,y:32891,varname:node_8749,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Vector1,id:3135,x:32392,y:32719,varname:node_3135,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Color,id:2109,x:31953,y:32619,ptovrint:False,ptlb:color,ptin:_color,varname:node_2109,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:3825,x:32304,y:32530,varname:node_3825,prsc:2|A-7736-RGB,B-2109-RGB;proporder:5964-7736-2941-7129-428-2109;pass:END;sub:END;*/

Shader "Shader Forge/niuqu" {
    Properties {
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _MainTex ("Base Color", 2D) = "black" {}
        _noize ("noize", 2D) = "white" {}
        _noizeintensity ("noize intensity", Range(0, 1)) = 1
        _refractionIntensity ("refraction  Intensity", Range(0, 1)) = 0.8164595
        _color ("color", Color) = (1,1,1,1)
		_range("range", Range(0, 1)) = 0.1
		[HideInInspector]_WaterTexture("Grab Color", 2D) = "black" {}
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _WaterTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform sampler2D _noize; uniform float4 _noize_ST;
            uniform float _noizeintensity;
            uniform float _refractionIntensity;
            uniform float4 _color;
			uniform float _range;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 screenPos : TEXCOORD7;
                UNITY_FOG_COORDS(8)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD9;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
            #endif
            #ifdef DYNAMICLIGHTMAP_ON
                o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
            #endif
            o.normalDir = UnityObjectToWorldNormal(v.normal);
            o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
            o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
            o.posWorld = mul(unity_ObjectToWorld, v.vertex);
            float3 lightColor = _LightColor0.rgb;
            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
            UNITY_TRANSFER_FOG(o,o.pos);
            o.screenPos = o.pos;
            return o;
        }
        float4 frag(VertexOutput i) : COLOR {
            #if UNITY_UV_STARTS_AT_TOP
                float grabSign = -_ProjectionParams.x;
            #else
                float grabSign = _ProjectionParams.x;
            #endif
            i.normalDir = normalize(i.normalDir);
            i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
            i.screenPos.y *= _ProjectionParams.x;
            float4 node_736 = _Time + _TimeEditor;
            float2 node_7849 = (i.uv0+node_736.g*float2(0,_range));
            float4 _noize_var = tex2D(_noize,TRANSFORM_TEX(node_7849, _noize));
            float2 node_4608 = (i.uv0+(_noizeintensity*_noize_var.r));
            float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_4608, _BumpMap)));
            float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (_BumpMap_var.rgb.rg*(_refractionIntensity*0.2));
            float4 sceneColor = tex2D(_WaterTexture, sceneUVs);
            float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/// Vectors:
            float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            float3 normalLocal = lerp(float3(0,0,0.5),_BumpMap_var.rgb,_refractionIntensity);
            float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
            float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
            float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
            float3 lightColor = _LightColor0.rgb;
            float3 halfDirection = normalize(viewDirection+lightDirection);
// Lighting:
            float attenuation = 1;
            float3 attenColor = attenuation * _LightColor0.xyz;
            float Pi = 3.141592654;
            float InvPi = 0.31830988618;
///// Gloss:
            float node_3135 = 1.5;
            float gloss = node_3135;
            float specPow = exp2( gloss * 10.0+1.0);
/// GI Data:
            UnityLight light;
            #ifdef LIGHTMAP_OFF
                light.color = lightColor;
                light.dir = lightDirection;
                light.ndotl = LambertTerm (normalDirection, light.dir);
            #else
                light.color = half3(0.f, 0.f, 0.f);
                light.ndotl = 0.0f;
                light.dir = half3(0.f, 0.f, 0.f);
            #endif
            UnityGIInput d;
            d.light = light;
            d.worldPos = i.posWorld.xyz;
            d.worldViewDir = viewDirection;
            d.atten = attenuation;
            #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                d.ambient = 0;
                d.lightmapUV = i.ambientOrLightmapUV;
            #else
                d.ambient = i.ambientOrLightmapUV;
            #endif
            d.boxMax[0] = unity_SpecCube0_BoxMax;
            d.boxMin[0] = unity_SpecCube0_BoxMin;
            d.probePosition[0] = unity_SpecCube0_ProbePosition;
            d.probeHDR[0] = unity_SpecCube0_HDR;
            d.boxMax[1] = unity_SpecCube1_BoxMax;
            d.boxMin[1] = unity_SpecCube1_BoxMin;
            d.probePosition[1] = unity_SpecCube1_ProbePosition;
            d.probeHDR[1] = unity_SpecCube1_HDR;
            UnityGI gi = UnityGlobalIllumination (d, 1, gloss, normalDirection);
            lightDirection = gi.light.dir;
            lightColor = gi.light.color;
// Specular:
            float NdotL = max(0, dot( normalDirection, lightDirection ));
            float node_8749 = 0.5;
            float LdotH = max(0.0,dot(lightDirection, halfDirection));
            float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4608, _MainTex));
            float3 diffuseColor = (_MainTex_var.rgb*_color.rgb); // Need this for specular when using metallic
            float specularMonochrome;
            float3 specularColor;
            diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, node_3135, specularColor, specularMonochrome );
            specularMonochrome = 1-specularMonochrome;
            float NdotV = max(0.0,dot( normalDirection, viewDirection ));
            float NdotH = max(0.0,dot( normalDirection, halfDirection ));
            float VdotH = max(0.0,dot( viewDirection, halfDirection ));
            float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
            float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
            float specularPBL = max(0, (NdotL*visTerm*normTerm) * unity_LightGammaCorrectionConsts_PIDiv4 );
            float3 directSpecular = 1 * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
            half grazingTerm = saturate( gloss + specularMonochrome );
            float3 indirectSpecular = (gi.indirect.specular + float3(node_8749,node_8749,node_8749));
            indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
            float3 specular = (directSpecular + indirectSpecular);
/// Diffuse:
            NdotL = max(0.0,dot( normalDirection, lightDirection ));
            half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
            float3 directDiffuse = ((1 +(fd90 - 1)*pow((1.00001-NdotL), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL) * attenColor;
            float3 indirectDiffuse = float3(0,0,0);
            indirectDiffuse += gi.indirect.diffuse;
            float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
// Final Color:
            float3 finalColor = diffuse + specular;
            fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,0.3),1);
            UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
            return finalRGBA;
        }
        ENDCG
    }
    Pass {
        Name "FORWARD_DELTA"
        Tags {
            "LightMode"="ForwardAdd"
        }
        Blend One One
        ZWrite Off
        
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #define UNITY_PASS_FORWARDADD
        #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
        #define _GLOSSYENV 1
        #include "UnityCG.cginc"
        #include "AutoLight.cginc"
        #include "Lighting.cginc"
        #include "UnityPBSLighting.cginc"
        #include "UnityStandardBRDF.cginc"
        #pragma multi_compile_fwdadd
        #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
        #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
        #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
        #pragma multi_compile_fog
        #pragma exclude_renderers metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
        #pragma target 3.0
        uniform sampler2D _WaterTexture;
        uniform float4 _TimeEditor;
        uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
        uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
        uniform sampler2D _noize; uniform float4 _noize_ST;
        uniform float _noizeintensity;
        uniform float _refractionIntensity;
        uniform float4 _color;
		uniform float _range;
        struct VertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 tangent : TANGENT;
            float2 texcoord0 : TEXCOORD0;
            float2 texcoord1 : TEXCOORD1;
            float2 texcoord2 : TEXCOORD2;
        };
        struct VertexOutput {
            float4 pos : SV_POSITION;
            float2 uv0 : TEXCOORD0;
            float2 uv1 : TEXCOORD1;
            float2 uv2 : TEXCOORD2;
            float4 posWorld : TEXCOORD3;
            float3 normalDir : TEXCOORD4;
            float3 tangentDir : TEXCOORD5;
            float3 bitangentDir : TEXCOORD6;
            float4 screenPos : TEXCOORD7;
            LIGHTING_COORDS(8,9)
        };
        VertexOutput vert (VertexInput v) {
            VertexOutput o = (VertexOutput)0;
            o.uv0 = v.texcoord0;
            o.uv1 = v.texcoord1;
            o.uv2 = v.texcoord2;
            o.normalDir = UnityObjectToWorldNormal(v.normal);
            o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
            o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
            o.posWorld = mul(unity_ObjectToWorld, v.vertex);
            float3 lightColor = _LightColor0.rgb;
            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
            o.screenPos = o.pos;
            TRANSFER_VERTEX_TO_FRAGMENT(o)
            return o;
        }
        float4 frag(VertexOutput i) : COLOR {
            #if UNITY_UV_STARTS_AT_TOP
                float grabSign = -_ProjectionParams.x;
            #else
                float grabSign = _ProjectionParams.x;
            #endif
            i.normalDir = normalize(i.normalDir);
            i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
            i.screenPos.y *= _ProjectionParams.x;
            float4 node_5359 = _Time + _TimeEditor;
            float2 node_7849 = (i.uv0+node_5359.g*float2(0,_range));
            float4 _noize_var = tex2D(_noize,TRANSFORM_TEX(node_7849, _noize));
            float2 node_4608 = (i.uv0+(_noizeintensity*_noize_var.r));
            float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_4608, _BumpMap)));
            float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (_BumpMap_var.rgb.rg*(_refractionIntensity*0.2));
            float4 sceneColor = tex2D(_WaterTexture, sceneUVs);
            float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/// Vectors:
            float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            float3 normalLocal = lerp(float3(0,0,0.5),_BumpMap_var.rgb,_refractionIntensity);
            float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
            float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
            float3 lightColor = _LightColor0.rgb;
            float3 halfDirection = normalize(viewDirection+lightDirection);
// Lighting:
            float attenuation = LIGHT_ATTENUATION(i);
            float3 attenColor = attenuation * _LightColor0.xyz;
            float Pi = 3.141592654;
            float InvPi = 0.31830988618;
///// Gloss:
            float node_3135 = 1.5;
            float gloss = node_3135;
            float specPow = exp2( gloss * 10.0+1.0);
// Specular:
            float NdotL = max(0, dot( normalDirection, lightDirection ));
            float LdotH = max(0.0,dot(lightDirection, halfDirection));
            float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4608, _MainTex));
            float3 diffuseColor = (_MainTex_var.rgb*_color.rgb); // Need this for specular when using metallic
            float specularMonochrome;
            float3 specularColor;
            diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, node_3135, specularColor, specularMonochrome );
            specularMonochrome = 1-specularMonochrome;
            float NdotV = max(0.0,dot( normalDirection, viewDirection ));
            float NdotH = max(0.0,dot( normalDirection, halfDirection ));
            float VdotH = max(0.0,dot( viewDirection, halfDirection ));
            float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
            float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
            float specularPBL = max(0, (NdotL*visTerm*normTerm) * unity_LightGammaCorrectionConsts_PIDiv4 );
            float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
            float3 specular = directSpecular;
/// Diffuse:
            NdotL = max(0.0,dot( normalDirection, lightDirection ));
            half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
            float3 directDiffuse = ((1 +(fd90 - 1)*pow((1.00001-NdotL), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL) * attenColor;
            float3 diffuse = directDiffuse * diffuseColor;
// Final Color:
            float3 finalColor = diffuse + specular;
            return fixed4(finalColor * 0.3,0);
        }
        ENDCG
    }
    Pass {
        Name "Meta"
        Tags {
            "LightMode"="Meta"
        }
        Cull Off
        
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #define UNITY_PASS_META 1
        #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
        #define _GLOSSYENV 1
        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        #include "UnityPBSLighting.cginc"
        #include "UnityStandardBRDF.cginc"
        #include "UnityMetaPass.cginc"
        #pragma fragmentoption ARB_precision_hint_fastest
        #pragma multi_compile_shadowcaster
        #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
        #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
        #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
        #pragma multi_compile_fog
        #pragma exclude_renderers metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
        #pragma target 3.0
        uniform float4 _TimeEditor;
        uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
        uniform sampler2D _noize; uniform float4 _noize_ST;
        uniform float _noizeintensity;
        uniform float4 _color;
		uniform float _range;
        struct VertexInput {
            float4 vertex : POSITION;
            float2 texcoord0 : TEXCOORD0;
            float2 texcoord1 : TEXCOORD1;
            float2 texcoord2 : TEXCOORD2;
        };
        struct VertexOutput {
            float4 pos : SV_POSITION;
            float2 uv0 : TEXCOORD0;
            float2 uv1 : TEXCOORD1;
            float2 uv2 : TEXCOORD2;
            float4 posWorld : TEXCOORD3;
        };
        VertexOutput vert (VertexInput v) {
            VertexOutput o = (VertexOutput)0;
            o.uv0 = v.texcoord0;
            o.uv1 = v.texcoord1;
            o.uv2 = v.texcoord2;
            o.posWorld = mul(unity_ObjectToWorld, v.vertex);
            o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
            return o;
        }
        float4 frag(VertexOutput i) : SV_Target {
/// Vectors:
            float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            UnityMetaInput o;
            UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
            
            o.Emission = 0;
            
            float4 node_8976 = _Time + _TimeEditor;
            float2 node_7849 = (i.uv0+node_8976.g*float2(0,_range));
            float4 _noize_var = tex2D(_noize,TRANSFORM_TEX(node_7849, _noize));
            float2 node_4608 = (i.uv0+(_noizeintensity*_noize_var.r));
            float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4608, _MainTex));
            float3 diffColor = (_MainTex_var.rgb*_color.rgb);
            float specularMonochrome;
            float3 specColor;
            float node_3135 = 1.5;
            diffColor = DiffuseAndSpecularFromMetallic( diffColor, node_3135, specColor, specularMonochrome );
            float roughness = 1.0 - node_3135;
            o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
            
            return UnityMetaFragment( o );
        }
        ENDCG
    }
}
FallBack "Diffuse"
}
