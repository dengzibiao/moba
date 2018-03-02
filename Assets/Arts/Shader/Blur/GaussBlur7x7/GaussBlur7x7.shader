Shader "Blur/GaussBlur7x7"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Value("Value",range(0,1)) = 1
		// _TexWH("Tex Width,Height",vector) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				// UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _MainTex_TexelSize;
			half _Value;
			// fixed4 _TexWH;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			//gauss filter mat
			static float gaussFilter[7] = {-5,-2,-1, 0, 1,2, 5};
			static half2 wh = half2(1.0/1024.0,1.0/1024.0);
			
			fixed4 frag (v2f v) : SV_Target
			{
				// sample the texture
				fixed4 col = fixed4(0,0,0,0);
				// fixed4 originalColor = ;
				for(int i=0;i<7;i++){
					for(int j=0;j<7;j++){
						float2 uvOffset = float2(gaussFilter[i].x,gaussFilter[j].x);
						uvOffset *= _MainTex_TexelSize.xy;
						col += tex2D(_MainTex,v.uv + uvOffset);
					}
				}
				col /= 49;
				return lerp(tex2D(_MainTex,v.uv),col,_Value); 
			}
			ENDCG
		}
	}
}
