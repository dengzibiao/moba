using UnityEngine;
using System.Collections;

namespace ScionEngine
{
	public static class ScionUtility
	{
		public const float DefaultWhitePoint = 11.2f;

		public static float GetWhitePointMultiplier(float whitePoint)
		{
			return DefaultWhitePoint / whitePoint;
		}

		public static float CoCToPixels(float widthInPixels)
		{
			return widthInPixels / (VirtualCamera.FilmWidth * 0.001f);
		}
		
		public static float ComputeApertureDiameter(float fNumber, float focalLength)
		{
			return focalLength / fNumber; 
		}
		
		public static float Square(float val) { return val * val; }
		public static float GetFocalLength(float tanHalfFoV)
		{
			//fov = 2 * atan(0.5 / focalLength);
			//fov / 2 = atan(0.5 / focalLength)
			//tan(fov / 2) = 0.5 / focalLength
			//focalLength = 0.5 / tan(fov / 2);

			float focalLength = (VirtualCamera.FilmWidth * 0.5f / tanHalfFoV) * 0.001f; //Convert from millimters to meters
			return focalLength;
		}
		
		private static Texture2D s_WhiteTexture = null;
		public static Texture2D WhiteTexture
		{
			get 
			{
				if (s_WhiteTexture == null)
				{
					s_WhiteTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false, true);
					s_WhiteTexture.hideFlags = HideFlags.HideAndDontSave;
					s_WhiteTexture.SetPixel(0, 0, Color.white);
					s_WhiteTexture.Apply(false, true);
				}
				return s_WhiteTexture;
			}
		}
	}

}