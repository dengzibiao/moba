using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor; 
#endif

namespace ScionEngine
{
#if UNITY_EDITOR
	[ExecuteInEditMode]
	[InitializeOnLoad]
#endif
	public static class ShaderSettings
	{
		public class IndexOption
		{
			private int curValue;
			private string[] keywords;
			
			public IndexOption(string[] _keywords)
			{
				keywords = _keywords;
				curValue = -1;
			}

			public void SetIndex(int index) 
			{
				if (index != curValue) 
				{
					SetKeyword(index);
					curValue = index;
				}
			}

			public void Disable()
			{
				//Exhaustive disable, issues w going between play n edit mode
				for (int i = 0; i < keywords.Length; i++) Shader.DisableKeyword(keywords[i]);
				curValue = -1;

				//if (curValue == -1) return;
				//Shader.DisableKeyword(keywords[curValue]);
				//curValue = -1;
			}

			public bool IsActive(int index)
			{
				return curValue == index;
			}
			
			public bool IsActive(string keyword)
			{
				for (int i = 0; i < keywords.Length; i++)
				{
					if (keyword == keywords[i]) return IsActive(i);
				}
				return false;
			}
			
			private void SetKeyword(int index)
			{
				for (int i = 0; i < keywords.Length; i++)
				{
					if (i == index) 
					{
						Shader.EnableKeyword(keywords[i]);
					}
					else Shader.DisableKeyword(keywords[i]);
				}
			}
			
			private void DisableKeyword(int index)
			{
				Shader.DisableKeyword(keywords[index]);
			}
		}
		
		
		public const string ExposureAutoKW 		= "SC_EXPOSURE_AUTO";
		public const string ExposureManualKW 	= "SC_EXPOSURE_MANUAL";
		private static readonly string[] ExposureKeywords = new string[] 
		{ ExposureAutoKW, ExposureManualKW };
		
		public static IndexOption ExposureSettings = new IndexOption(ExposureKeywords);
		
		
		public const string DepthFocusManualKW 		= "SC_DOF_FOCUS_MANUAL";
		public const string DepthFocusManualRangeKW = "SC_DOF_FOCUS_RANGE";
		public const string DepthFocusCenterKW 		= "SC_DOF_FOCUS_CENTER";
		private static readonly string[] DepthFocusKeywords = new string[] 
		{ DepthFocusManualKW, DepthFocusManualRangeKW, DepthFocusCenterKW };
		
		public static IndexOption DepthFocusSettings = new IndexOption(DepthFocusKeywords);
		

		public const string DepthOfFieldMaskOnKW 	= "SC_DOF_MASK_ON";
		private static readonly string[] DepthOfFieldMaskKeywords = new string[] 
		{ DepthOfFieldMaskOnKW };
		
		public static IndexOption DepthOfFieldMaskSettings = new IndexOption(DepthOfFieldMaskKeywords);
		

		public const string ChromaticAberrationOnKW 	= "SC_CHROMATIC_ABERRATION_ON";
		private static readonly string[] ChromaticAberrationKeywords = new string[] 
		{ ChromaticAberrationOnKW };
		
		public static IndexOption ChromaticAberrationSettings = new IndexOption(ChromaticAberrationKeywords);
		

		public const string LensFlareOnKW 	= "SC_LENS_FLARE_ON";
		private static readonly string[] LensFlareKeywords = new string[] 
		{ LensFlareOnKW };
		
		public static IndexOption LensFlareSettings = new IndexOption(LensFlareKeywords);
		
		
		public const string TonemappingReinhardKW 		= "SC_TONEMAPPING_REINHARD";
		public const string TonemappingLumaReinhardKW 	= "SC_TONEMAPPING_LUMAREINHARD";
		public const string TonemappingFilmicKW 		= "SC_TONEMAPPING_FILMIC";
		public const string TonemappingPhotographicKW 	= "SC_TONEMAPPING_PHOTOGRAPHIC";
		private static readonly string[] TonemappingKeywords = new string[] 
		{ TonemappingReinhardKW, TonemappingLumaReinhardKW, TonemappingFilmicKW, TonemappingPhotographicKW };
		
		public static IndexOption TonemappingSettings = new IndexOption(TonemappingKeywords);
		
		
		//public const string ColorGradingOffKW 		= "SC_COLOR_CORRECTION_OFF";
		public const string ColorGradingOn1TexKW 	= "SC_COLOR_CORRECTION_1_TEX";
		public const string ColorGradingOn2TexKW 	= "SC_COLOR_CORRECTION_2_TEX";
		private static readonly string[] ColorGradingKeywords = new string[] 
		{ ColorGradingOn1TexKW, ColorGradingOn2TexKW };
		
		public static IndexOption ColorGradingSettings = new IndexOption(ColorGradingKeywords);
	}
}




