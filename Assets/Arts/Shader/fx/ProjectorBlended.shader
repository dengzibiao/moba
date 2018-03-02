// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Effects/Projector/AlphaBlended" {
	Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,1)
	_ColorStrength ("Color strength", Float) = 1.0
	_MainTex ("Main Texture", 2D) = "gray" {}
	//_CutoutTex ("Cutout Texture (R)", 2D) = "white" {}
	_FalloffTex ("FallOff", 2D) = "white" {}
	}
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			Fog { Mode Off}
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			fixed4 _TintColor;
			fixed _ColorStrength;

			struct v2f {
				float4 uvMainTex : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				//float4 uv_CutoutTex : TEXCOORD3;
				float4 pos : SV_POSITION;
				float2 texcoord : TEXCOORD2;
			};
			
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			float4 _MainTex_ST;


			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, vertex);
				o.uvMainTex = mul (unity_Projector, vertex);
				o.uvFalloff = mul (unity_ProjectorClip, vertex);
				//o.uv_CutoutTex = mul (_Projector, vertex);
				//o.uv = TRANSFORM_TEX (mul (_Projector, vertex).xy, _MainTex);
				
				o.texcoord = TRANSFORM_TEX( o.uvMainTex.xyz,_MainTex);

				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _CutoutTex;
			sampler2D _FalloffTex;
			
			fixed4 frag (v2f i) : COLOR
			{
				//fixed4 texS = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				//texS.a = 1.0-texS.a;
 
				fixed4 texF = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
				
				//fixed4 texC = tex2Dproj (_CutoutTex, UNITY_PROJ_COORD(i.uv_CutoutTex));

				//return texS;
				//fixed4 tex = tex2Dproj(_MainTex, UNITY_PROJ_COORD(i.uvMainTex));
				
				fixed4 tex;
				if(i.uvMainTex.y<0 || i.uvMainTex.y>1 || i.uvMainTex.x < 0 || i.uvMainTex.x > 1) {
					tex = fixed4 (0,0,0,0);
				}
				else { 
					tex =  tex2D(_MainTex, float2(i.texcoord));
				}

				fixed4 res = lerp(fixed4(1,1,1,0), tex * _TintColor * _ColorStrength*tex.a, texF.a);
				if(res.a > 1 ) res.a = 1;
				//res.a *= texC.a;
				return res;
			}
			ENDCG
		}
	}
}
