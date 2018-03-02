using UnityEngine;

public class PlatformConfig
{
	public static string wordwrap {
		get {
			if(Application.platform == RuntimePlatform.WindowsEditor)
				return "\r\n";
			else
				return "\n";
		}
	}
}