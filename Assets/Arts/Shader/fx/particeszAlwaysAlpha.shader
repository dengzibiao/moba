Shader "Custom/ZAlwaysaddAlpha" {
	Properties {
		_MainTex ("Particle Texture", 2D) = "white" {}
		//_Mask ("Mask", 2D) = "white" {}
	}

	Category {
		Tags { "Queue"="transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest Always
        ZWrite Off
		Cull Off Lighting Off Fog { Color (0,0,0,0) }
		BindChannels {
 			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
		
		SubShader {
			Pass {
 				//SetTexture [_Mask] {combine texture * primary}
				SetTexture [_MainTex] {
					combine texture * previous
				}
			}
		}
	}
}
