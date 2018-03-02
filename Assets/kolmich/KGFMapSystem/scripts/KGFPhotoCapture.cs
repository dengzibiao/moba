// <author>Michal Kolasinski</author>
// <email>michal.kolasinski@kolmich.at</email>
// <date>2011-10-03</date>
// <summary>short summary</summary>

using UnityEngine;
using System.Collections;

public class KGFPhotoCapture : MonoBehaviour
{
	private Texture2D itsTexture = null;	//The texture to render the picture to
	
	private void OnPostRender()
	{
//		Debug.LogError("Reading Pixels");
		itsTexture.ReadPixels(new Rect(0,0,Screen.width,Screen.height),0,0);
		itsTexture.Apply();
	}
	
	public void SetTexture(Texture2D theTexture)
	{
		itsTexture = theTexture;
	}
}
