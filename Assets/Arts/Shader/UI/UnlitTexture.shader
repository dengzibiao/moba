Shader "Unlit/UnlitTexture"
{
	Properties
	{
		[PerRendererData] _MainTex ("Texture", 2D) = "" {}
		_Color("Color",color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha 

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			float4 _Color;
			struct appdata
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color:TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color:TEXCOORD1;
			};

			sampler2D _MainTex;
			// float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				// o.color = v.color;
				return o;
			}
			
			float4 frag (v2f i) : COLOR
			{
				// sample the texture
				float4 col = tex2D(_MainTex, i.uv);
				col *= _Color;
				// col.rgb *= 1.0/1.8;
				return col;
			}
			ENDCG
		}
	}
}
