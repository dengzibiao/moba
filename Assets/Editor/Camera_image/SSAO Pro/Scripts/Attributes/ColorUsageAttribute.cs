// SSAO Pro - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com

#if !(UNITY_4_5 || UNITY_4_6 || UNITY_5_0)
#define UNITY_5_1_PLUS
#endif

// Compatibility layer for Unity < 5.1

namespace SSAOProUtils
{
#if !(UNITY_5_1_PLUS)
	using UnityEngine;

	public class ColorUsageAttribute : PropertyAttribute
	{
		public ColorUsageAttribute(bool showAlpha) { }
		public ColorUsageAttribute(bool showAlpha, bool hdr, float minBrightness, float maxBrightness, float minExposureValue, float maxExposureValue) { }
	}
#endif
}
