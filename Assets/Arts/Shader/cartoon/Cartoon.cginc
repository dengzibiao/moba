#ifndef CARTOON_CGINC
#define CARTOON_CGINC
    sampler2D _Ramp;
	fixed4 _LightDir;
	fixed4 _LightColor;
	fixed _LightIntensity;

    half4 LightingRamp (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
        half NdotL = dot (s.Normal, normalize(_LightDir));
        half diff = NdotL * 0.5 + 0.5;
		half ne = dot(s.Normal,viewDir) * 0.5 + 0.5;
        half3 ramp = tex2D (_Ramp, float2(diff,ne)).rgb;
        half4 c;
        c.rgb = s.Albedo *_LightIntensity * _LightColor * ramp * (atten );
        c.a = s.Alpha;
        return c;
    }

    struct Input {
        float2 uv_MainTex;
		float3 viewDir;
    };
    
    sampler2D _MainTex;
    float4 _Color;
	float4 _RimColor;
	float _RimPower;
	
    void surf (Input IN, inout SurfaceOutput o) {
        o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * _Color;
		
		half rim = 1 - saturate(dot(IN.viewDir,o.Normal));
		o.Emission = _RimColor.rgb * (rim * _RimPower);
    }
#endif //end cartoon_cginc