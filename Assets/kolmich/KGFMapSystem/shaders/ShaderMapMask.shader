Shader "MaskedTexture"
{
   Properties
   {
   	  _Color ("_Color", Color) = (1,1,1,1)
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _Mask ("Culling Mask", 2D) = "white" {}
      _Cutoff ("Alpha cutoff", Range (0,1)) = 0.1
   }
   SubShader
   {
	  Tags {"Queue"="Overlay+500"}   
      Lighting Off
      ZWrite On
      Blend SrcAlpha OneMinusSrcAlpha
      //AlphaTest GEqual [_Cutoff]
	  ZTest Always
	            
      Pass
      {
         SetTexture [_Mask] {combine texture}
         SetTexture [_MainTex]
         {
         	constantColor [_Color]
         	combine texture*constant, previous*constant
         }
      }
   }
} 