Shader "Hidden/ScionVirtualCamera" 
{	    	
 	Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    
	CGINCLUDE
	#include "UnityCG.cginc" 
	#include "../ShaderIncludes/ScionCommon.cginc" 
	#include "../ShaderIncludes/VirtualCameraCommon.cginc" 	
	
	uniform sampler2D _DownsampledScene;
	uniform sampler2D _PreviousExposureTexture;	 
	uniform sampler2D _MainTex;
	
	#define USE_GEOMETRIC_MEAN
    	
	struct v2f
	{
		float4 pos : SV_POSITION;
	};	
		
	v2f vert(appdata_img v)
	{
		v2f o;
		
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		
		return o; 
	}
    	
	struct v2fUV
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};	
		
	v2fUV vertUV(appdata_img v)
	{
		v2fUV o;
		
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		
		return o; 
	}

	float GetFloatingSceneLuminance(float previousSceneLuminance, out float sceneLuminance)
	{
		#ifdef USE_GEOMETRIC_MEAN
		float3 sceneColor = exp(tex2Dlod(_DownsampledScene, float4(0.5f, 0.5f, 0.0f, 0.0f)).xyz);
		#else
		float3 sceneColor = tex2Dlod(_DownsampledScene, float4(0.5f, 0.5f, 0.0f, 0.0f)).xyz;
		#endif
		
		sceneLuminance = VectorMax(sceneColor);
		float scaledSceneLuminance = LIGHT_INTENSITY_MULT * sceneLuminance;
		
		float floatingSceneLuminance = previousSceneLuminance + (scaledSceneLuminance - previousSceneLuminance) * InvExpNegDeltaTTimesTau;		
		floatingSceneLuminance = -min(-floatingSceneLuminance, 0.0f);
		
		return floatingSceneLuminance;
	} 

	float GetTargetExposureValue(float floatingSceneLuminance)
	{ 
		float EV100 = ComputeEV100(floatingSceneLuminance) - ExposureCompensation;
		return EV100;
	}
	
	float ClampExposure(float exposure)
	{
		return clamp(exposure, MinExposure, MaxExposure);
	}

	PackedCameraOutput AutoPriority(v2f i)
	{		 
		float4 previousResult = tex2Dlod(_PreviousExposureTexture, float4(0.5f, 0.5f, 0.0f, 0.0f));
		float sceneLuminance;
		float floatingSceneLuminance = GetFloatingSceneLuminance(previousResult.x, sceneLuminance);
		float EV100 = GetTargetExposureValue(floatingSceneLuminance);
		
	    //Assume fNumber of 4 and shutter speed of 1/F 
	    float fNumber = 4.0f;	 
	    float shutterSpeed = InvFocalLengthMilliMeter; 
	  
	   	float ISOvalue = clamp(ComputeISO(fNumber, shutterSpeed, EV100), MIN_ISO, MAX_ISO);
	    float evDiff = EV100 - ComputeEV(fNumber, shutterSpeed, ISOvalue);
	    
		shutterSpeed = clamp(shutterSpeed * pow(2.0f, -evDiff), MIN_SHUTTER_SPEED, MAX_SHUTTER_SPEED);	
		evDiff = EV100 - ComputeEV(fNumber, shutterSpeed, ISOvalue);   
		fNumber = clamp(fNumber * pow(sqrt(2.0f), evDiff * 0.5f), MIN_FNUMBER, MAX_FNUMBER);
				
		CameraOutput cameraOutput;
		cameraOutput.floatingSceneLuminance = floatingSceneLuminance; 
		cameraOutput.shutterSpeed = shutterSpeed;
		cameraOutput.ISO = ISOvalue;
		cameraOutput.fNumber = fNumber;
		//cameraOutput.exposure = ClampExposure(StandardOutputBasedExposure(fNumber, shutterSpeed, ISOvalue) * LIGHT_INTENSITY_MULT);
		cameraOutput.exposure = ClampExposure(ConvertEV100ToExposure(EV100) * LIGHT_INTENSITY_MULT);
		cameraOutput.CoCScaleAndBias = ComputeCoCScaleAndBias(fNumber, FocalLength, VCFocalDistance, PixelsPerMeter);
		cameraOutput.sceneLuminance = sceneLuminance;
		
		return PackCameraOutput(cameraOutput);
	}
	 
	PackedCameraOutput ShutterPriority(v2f i)
	{
		float4 previousResult = tex2Dlod(_PreviousExposureTexture, float4(0.5f, 0.5f, 0.0f, 0.0f));
		float sceneLuminance;
		float floatingSceneLuminance = GetFloatingSceneLuminance(previousResult.x, sceneLuminance);
		float EV100 = GetTargetExposureValue(floatingSceneLuminance);
		
	    //Assume aperture of 4
	    float fNumber = 4.0f;
	 
		float ISOvalue = clamp(ComputeISO(fNumber, ManualShutterSpeed, EV100), MIN_ISO, MAX_ISO);
		float evDiff = EV100 - ComputeEV(fNumber, ManualShutterSpeed, ISOvalue);

		fNumber = clamp(fNumber * pow(sqrt(2.0f), evDiff), MIN_FNUMBER, MAX_FNUMBER);
		
		CameraOutput cameraOutput;
		cameraOutput.floatingSceneLuminance = floatingSceneLuminance;
		cameraOutput.shutterSpeed = ManualShutterSpeed;
		cameraOutput.ISO = ISOvalue;
		cameraOutput.fNumber = fNumber;
		//cameraOutput.exposure = ClampExposure(StandardOutputBasedExposure(fNumber, ManualShutterSpeed, ISOvalue) * LIGHT_INTENSITY_MULT);
		cameraOutput.exposure = ClampExposure(ConvertEV100ToExposure(EV100) * LIGHT_INTENSITY_MULT);
		cameraOutput.CoCScaleAndBias = ComputeCoCScaleAndBias(fNumber, FocalLength, VCFocalDistance, PixelsPerMeter);
		cameraOutput.sceneLuminance = sceneLuminance;
		
		return PackCameraOutput(cameraOutput); 
	}
	 
	PackedCameraOutput AperturePriority(v2f i)
	{
		float4 previousResult = tex2Dlod(_PreviousExposureTexture, float4(0.5f, 0.5f, 0.0f, 0.0f));
		float sceneLuminance;
		float floatingSceneLuminance = GetFloatingSceneLuminance(previousResult.x, sceneLuminance);
		float EV100 = GetTargetExposureValue(floatingSceneLuminance);

	    //Assume shutter speed of 1/F
		float shutterSpeed = InvFocalLengthMilliMeter;

		float ISOvalue = clamp(ComputeISO(ManualFNumber, shutterSpeed, EV100), MIN_ISO, MAX_ISO);
		float evDiff = EV100 - ComputeEV(ManualFNumber, shutterSpeed, ISOvalue);

		shutterSpeed = clamp(shutterSpeed * pow(2.0f, -evDiff), MIN_SHUTTER_SPEED, MAX_SHUTTER_SPEED);
		
		CameraOutput cameraOutput;
		cameraOutput.floatingSceneLuminance = floatingSceneLuminance;
		cameraOutput.shutterSpeed = shutterSpeed;
		cameraOutput.ISO = ISOvalue;
		cameraOutput.fNumber = ManualFNumber;
		//cameraOutput.exposure = ClampExposure(StandardOutputBasedExposure(ManualFNumber, shutterSpeed, ISOvalue) * LIGHT_INTENSITY_MULT);
		cameraOutput.exposure = ClampExposure(ConvertEV100ToExposure(EV100) * LIGHT_INTENSITY_MULT);
		cameraOutput.CoCScaleAndBias = ComputeCoCScaleAndBias(ManualFNumber, FocalLength, VCFocalDistance, PixelsPerMeter);
		cameraOutput.sceneLuminance = sceneLuminance;
		
		return PackCameraOutput(cameraOutput);
	}
	
	float4 GeometricDownsample(v2fUV i) : SV_Target0
	{
		float3 texSample = tex2Dlod(_MainTex, float4(i.uv, 0.0f, 0.0f)).xyz;
		
		//Clamp to avoid artifacts
		texSample = min(texSample, 1000.0f);	
		
		#ifdef USE_GEOMETRIC_MEAN
		texSample = log(texSample + 1e-4f);	
		#endif
		
		return float4(texSample, 0.0f);
	}
	
	ENDCG
	

	Category 
	{
		Subshader 
		{
		    ZTest Off
		    Cull Off
		    ZWrite Off
		    Blend Off
		    Fog { Mode off }
			Pass 
			{
				Name "AutoPriority" //Pass 0
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment AutoPriority
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
			Pass 
			{
				Name "ShutterPriority" //Pass 1
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment ShutterPriority
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG	
			}
			Pass 	
			{ 	
				Name "AperturePriority" //Pass 2
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vert
				#pragma fragment AperturePriority
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG		 
			}
			Pass 	
			{ 	
				Name "GeometricDownsample" //Pass 3
			
				CGPROGRAM	
				#pragma target 3.0		
				#pragma vertex vertUV
				#pragma fragment GeometricDownsample
				#pragma fragmentoption ARB_precision_hint_fastest 			
				ENDCG		 
			}
		}
	}	
	Fallback Off	
}



