// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Hidden/Ultimate/BokehTexture" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}

	_Intensity ("Intensity", Float) = 1
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	Cull Off 
    ZWrite Off 
	//Blend SrcAlpha One
	Blend SrcAlpha OneMinusSrcAlpha
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"

			float4x4 _MeshProjectionMatrix; 
			float4x4 _MeshTransformationMatrix; 
			half _Intensity;


			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _Tint;

			v2f vert (appdata_full v)
			{
				v2f o;
				//o.vertex = mul(_MeshTransformationMatrix, v.vertex);
				//o.vertex = mul(_MeshProjectionMatrix, v.vertex);

				o.vertex = v.vertex;

				o.texcoord = v.texcoord;


				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);


				col.xyz *=  _Tint;
				col.a *= _Intensity;

				return col;
			}
		ENDCG
	}
}

}
