Shader "yhqqxq/Rim" 
{
    Properties 
	{
         _MainTex ("Texture", 2D) = "white" {}
         _BumpMap ("Bumpmap", 2D) = "bump" {}       
	     _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)     
	     _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0   
    }
    SubShader
	 {
      Tags { "RenderType" = "Opaque" }

      CGPROGRAM
      #pragma surface surf Lambert
      struct Input 
	  {
          float2 uv_MainTex;
          float2 uv_BumpMap; 
		  float3 viewDir;  //viewDir 意为World Space View Direction。就是当前坐标的视角方向,就是摄像机看过来的方向    
	  };

      sampler2D _MainTex;
      sampler2D _BumpMap;      
	  float4 _RimColor;     
	  float _RimPower;

	  void surf (Input IN, inout SurfaceOutput o) 
	  {
          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
          o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));             

		  half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
		  //Normalize函数，用于获取到的viewDir坐标转成一个单位向量且方向不变
		  //dot: o.Normal就是单位向量。外加Normalize了viewDir。因此求得的点积就是夹角的cos值
		  //因为cos值越大，夹角越小，所以，这时取反来。这样，夹角越大，所反射上的颜色就越多。于是就得到的两边发光的效果
		  //saturate算出[0,1]之间的最靠近（最小值但大于所指的值）的值,将cos的值控制在0-1之间

		   o.Emission = _RimColor.rgb * pow (rim, _RimPower); 

	}

      ENDCG
  } 

    Fallback "Diffuse"
}