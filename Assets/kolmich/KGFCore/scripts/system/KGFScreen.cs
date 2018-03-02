// <copyright>
// Copyright (c) 2011 All Right Reserved, http://www.kolmich.at/
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// </copyright>
// <author>Christoph Hausjell</author>
// <email>michal.kolasinski@kolmich.at</email>
// <summary>Includes all classes for screen and resolution handling
// </summary>

using System;
using System.Collections;
using System.IO;

using UnityEngine;

/// <summary>
/// Represents the main class of the Kolmich Game Framework Console Module.
/// </summary>
/// <remarks>
/// This class is able to call methods on other objects. It is possible to call public, protected or even private methods of static and non static class instances.
/// To use this class add a command by using the AddCommand() method. This class is implemented as Singleton (only one instance possible).
/// </remarks>
public class KGFScreen : MonoBehaviour
{
	/// <summary>
	/// Defines how the 3D Cameras should be rendered.
	/// If set to eNative itsResolution3D will be set to itsResolutionDisplay, if set to eAutoAdjust itsResolution3D will be autoadjusted by following rules:
	/// For screen aspects of 4:3 the the RenderTexture resolution will be 1024x768, for screen aspects of 16:9 it will be 1366x768 and for screen aspects of 16:10 it will be 1280x800
	/// The rendertexture will be blitted to the screen
	/// </summary>
	public enum eResolutionMode
	{
		eNative,
		eAutoAdjust
	}
	
	#region internal classes
	
	/// <summary>
	/// contains all data available for customization in the Unity3D inspector
	/// </summary>
	[System.Serializable]
	public class KGFDataScreen
	{
		/// <summary>
		/// The currend resolution mode for the 3D content
		/// </summary>
		public KGFScreen.eResolutionMode itsResolutionMode3D = eResolutionMode.eAutoAdjust;
		
		/// <summary>
		/// The currend resolution mode for the 2D content
		/// </summary>
		public KGFScreen.eResolutionMode itsResolutionMode2D = eResolutionMode.eAutoAdjust;
		
		/// <summary>
		/// stores the resolution of the application
		/// </summary>
		/// <remarks>
		/// This member stores the calculated resolution of the application, which is usually not the display resolution.
		/// </remarks>
		[HideInInspector]
		public Resolution itsResolution3D;
		
		/// <summary>
		/// stores the 2D resolution of the application
		/// </summary>
		/// <remarks>
		/// This member stores the calculated resolution of the application, which is usually not the display resolution.
		/// </remarks>
		[HideInInspector]
		public Resolution itsResolution2D;
		
		/// <summary>
		/// stores the resolution of the display
		/// </summary>
		/// <remarks>
		/// This member stores the current real display resolution determined by Screen.currentResolution
		/// </remarks>
		[HideInInspector]
		public Resolution itsResolutionDisplay;
		
		/// <summary>
		/// caches the apsect ratio of the screen
		/// </summary>
		[HideInInspector]
		public float itsAspect3D = 1.0f;
		
		/// <summary>
		/// caches the apsect ratio of the 2D
		/// </summary>
		[HideInInspector]
		public float itsAspect2D = 1.0f;
		
		/// <summary>
		/// caches the apsect ratio of display
		/// </summary>
		[HideInInspector]
		public float itsAspectDisplay = 1.0f;
		
		/// <summary>
		/// the difference between the display resolution and the resolution
		/// </summary>
		[HideInInspector]
		public float itsScaleFactor3D = 1.0f;
		
		[HideInInspector]
		public float itsScaleFactor2D = 1.0f;
		
		/// <summary>
		/// Indicates if fullscreen mode is enabled. -> We never use real fullscreen. All apps should be started using the -popupwindow parameter.
		/// So fullscreen means that the window resolution will be adapted to the current display resolution. else the window resolution will be set to itsResolution.
		/// </summary>
//		public bool itsIsFullScreen = true;
		
		
		public int itsMinWidth = 1024;
		
		public int itsMinHeight = 768;
	}
	#endregion
	
	#region members
	
	//holds the only instance of the debugger
	static KGFScreen itsInstance = null;
	
	private static bool itsAlreadyChecked = false;
	
	/// <summary>
	/// The rendertexture used for the resulting camera to render to.
	/// This texture will be blitted to screen using itsResolution.
	/// </summary>
	private RenderTexture itsRenderTexture = null;
	
	/// <summary>
	/// This camera will be rendered at last to the screen, with depth 100.
	/// It is created and attached to the KGFScreen to make sure the PostRenderMethod is called where the blit to screen takes place
	/// </summary>
	/// 
	private Camera itsCamera = null;
	
	/// <summary>
	/// Contains all data that can be modified in the Unity3D inspector.
	/// </summary>
	/// <remarks>
	/// This class goups all available modifiers that can be modified in the Unity3D inspector.
	/// </remarks>
	public KGFDataScreen itsDataModuleScreen = new KGFDataScreen();
	
	const string itsSettingsSection = "screen";
	const string itsSettingsNameWidth = "resolution.width";
	const string itsSettingsNameHeight = "resolution.height";
	const string itsSettingsNameRefreshRate = "refreshrate";
	const string itsSettingsNameIsFulscreen = "fullscreen";
	
	#endregion
	
	#region Unity3D methods
	
	/// <summary>
	/// This method is called from Unity3D Engine when the script instance is being loaded.
	/// </summary>
	protected void Awake()
	{		
		if (itsInstance == null)
		{
			itsInstance = this;
			itsInstance.Init();
		}
		else
		{
			if(itsInstance != this)
			{
				UnityEngine.Debug.Log("there is more than one KFGDebug instance in this scene. please ensure there is always exactly one instance in this scene");
				Destroy(gameObject);
			}
			return;
		}
	}
	#endregion

	#region public methods
	
	/// <summary>
	/// Use this method to get the only instance of the Kolmich Game Framework Console Module.
	/// </summary>
	/// <returns>returns the only instance of the console module.</returns>
	public static KGFScreen GetInstance()
	{
		return itsInstance;
	}

	private static void SetResolution3D(int theWidth, int theHeight)
	{
		SetResolution3D(theWidth,theHeight,60);
	}
	
//	public static void SetIsFullScreen(bool theIsFullScreen)
//	{
//		CheckInstance();
//		if(itsInstance == null)
//			return;
//		itsInstance.itsDataModuleScreen.itsIsFullScreen = theIsFullScreen;
//		itsInstance.SaveSettings();
//	}
	
	public static Resolution GetResolution3D()
	{
		CheckInstance();
		if(itsInstance == null)
			return new Resolution();
		return itsInstance.itsDataModuleScreen.itsResolution3D;
	}
	
	public static Resolution GetResolution2D()
	{
		CheckInstance();
		if(itsInstance == null)
			return new Resolution();
		return itsInstance.itsDataModuleScreen.itsResolution2D;
	}
	
	public static Resolution GetResolutionDisplay()
	{
		CheckInstance();
		if(itsInstance == null)
			return new Resolution();
		return itsInstance.itsDataModuleScreen.itsResolutionDisplay;
	}
	
	public static eResolutionMode GetResolutionMode3D()
	{
		CheckInstance();
		if(itsInstance == null)
			return eResolutionMode.eNative;
		return itsInstance.itsDataModuleScreen.itsResolutionMode3D;
	}
	
	public static eResolutionMode GetResolutionMode2D()
	{
		CheckInstance();
		if(itsInstance == null)
			return eResolutionMode.eNative;
		return itsInstance.itsDataModuleScreen.itsResolutionMode2D;
	}
	
	public static float GetAspect3D()
	{
		CheckInstance();
		if(itsInstance == null)
			return 1.0f;
		return itsInstance.itsDataModuleScreen.itsAspect3D;
	}
	
	public static float GetAspect2D()
	{
		CheckInstance();
		if(itsInstance == null)
			return 1.0f;
		return itsInstance.itsDataModuleScreen.itsAspect2D;
	}
	
	public static float GetScaleFactor3D()
	{
		CheckInstance();
		if(itsInstance == null)
			return 1.0f;
		return itsInstance.itsDataModuleScreen.itsScaleFactor3D;
	}
	
	public static float GetScaleFactor2D()
	{
		CheckInstance();
		if(itsInstance == null)
			return 1.0f;
		return itsInstance.itsDataModuleScreen.itsScaleFactor2D;
	}
	
//	public static bool GetIsFullScreen()
//	{
//		CheckInstance();
//		if(itsInstance == null)
//			return false;
//		return itsInstance.itsDataModuleScreen.itsIsFullScreen;
//	}
	
	public static Vector3 GetConvertedEventCurrentMousePosition(Vector2 theEventCurrentMousePosition)
	{
		Vector3 aConvertedMousePosition = Input.mousePosition*GetScaleFactor3D();
		Vector3 aMousePosition = Input.mousePosition;
		float anXDifference = aConvertedMousePosition.x - aMousePosition.x;
		float anYDifference = aConvertedMousePosition.y - aMousePosition.y;	//difference in display coordinates
		
		anXDifference /= GetScaleFactor3D();
		anYDifference /= GetScaleFactor3D();	//difference in screen coordinates
		
		Vector2 aConvertedEventCurentMousePosition = new Vector2(theEventCurrentMousePosition.x + anXDifference,theEventCurrentMousePosition.y - anYDifference);
		
		return aConvertedEventCurentMousePosition;
	}
	
	public static Vector3 GetMousePositionDisplay()
	{
		CheckInstance();
		if(itsInstance == null)
			return Vector3.zero;
		
		float anX = Input.mousePosition.x * KGFScreen.GetScaleFactor3D();
		float anY = Screen.height - (Input.mousePosition.y*KGFScreen.GetScaleFactor3D());
		Vector3 aMousePosition = new Vector3(anX,anY,Input.mousePosition.z);
		return aMousePosition;
	}
	
	public static Vector3 GetMousePosition2D()
	{
		CheckInstance();
		if(itsInstance == null)
			return Vector3.zero;
		
		if(GetResolutionMode3D() == GetResolutionMode2D())	//both modes are the same
			return Input.mousePosition;
		else if(GetResolutionMode2D() == eResolutionMode.eNative && GetResolutionMode3D() == eResolutionMode.eAutoAdjust)
			return Input.mousePosition * KGFScreen.GetScaleFactor3D();
		else if(GetResolutionMode2D() == eResolutionMode.eAutoAdjust && GetResolutionMode3D() == eResolutionMode.eNative)
			return Input.mousePosition / KGFScreen.GetScaleFactor2D();
		
		return Input.mousePosition;
	}
	
	public static Vector3 GetMousePositio3D()
	{
		CheckInstance();
		if(itsInstance == null)
			return Vector3.zero;
		
		return Input.mousePosition;
	}
	
	/// <summary>
	/// converts an x/y position in display coordinates to an x/y position in screen coordinates
	/// </summary>
	/// <param name="theDisplayPosition"></param>
	/// <returns></returns>
	public static Vector2 DisplayToScreen(Vector2 theDisplayPosition)
	{
		return theDisplayPosition / GetScaleFactor3D();
	}
	
	/// <summary>
	/// converts an x/y position in display coordinates to an x/y position in screen corrdinates normalized to 0/1
	/// </summary>
	/// <param name="theDisplayPosition"></param>
	/// <returns></returns>
	public static Vector2 DisplayToScreenNormalized(Vector2 theDisplayPosition)
	{
		Vector2 aReturnPosition = new Vector2(0.0f,0.0f);
		Vector2 aScreenPosition = DisplayToScreen(theDisplayPosition);
		aReturnPosition.x = aScreenPosition.x/GetResolution3D().width;
		aReturnPosition.y = aScreenPosition.y/GetResolution3D().height;
		return aReturnPosition;
	}
	
	/// <summary>
	/// converts an x/y width/height rect in display coordinates to an x/y width/height rect in screen coordinates
	/// </summary>
	/// <param name="theDisplayPosition"></param>
	/// <returns></returns>
	public static Rect DisplayToScreen(Rect theDisplayRect)
	{
		Rect aReturnRect = new Rect(0.0f,0.0f,1.0f,1.0f);
		aReturnRect.x = theDisplayRect.x/GetScaleFactor3D();
		aReturnRect.y = theDisplayRect.y/GetScaleFactor3D();
		aReturnRect.width = theDisplayRect.width/GetScaleFactor3D();
		aReturnRect.height = theDisplayRect.height/GetScaleFactor3D();
		return aReturnRect;
	}
	
	/// <summary>
	/// converts an x/y width/height rect in display coordinates to an x/y width/height rect in screen corrdinates normalized to 0/1
	/// </summary>
	/// <param name="theDisplayPosition"></param>
	/// <returns></returns>
	public static Rect DisplayToScreenNormalized(Rect theDisplayRect)
	{
		Rect aReturnRect = new Rect(0.0f,0.0f,1.0f,1.0f);
		Rect aScreenRect = DisplayToScreen(theDisplayRect);
		aReturnRect.x = aScreenRect.x/GetResolution3D().width;
		aReturnRect.y = aScreenRect.y/GetResolution3D().height;
		aReturnRect.width = aScreenRect.width/GetResolution3D().width;
		aReturnRect.height = aScreenRect.height/GetResolution3D().height;
		return aReturnRect;
	}
	
	/// <summary>
	/// converts an x/y width/height rect in display coordinates to an x/y width/height rect in screen corrdinates normalized to 0/1
	/// </summary>
	/// <param name="theDisplayPosition"></param>
	/// <returns></returns>
	public static Rect NormalizedTo2DScreen(Rect theDisplayRect)
	{
		Rect aReturnRect = new Rect(0.0f,0.0f,1.0f,1.0f);
		aReturnRect.x = GetResolution2D().width*theDisplayRect.x;
		aReturnRect.y = GetResolution2D().height*theDisplayRect.y;
		aReturnRect.width = GetResolution2D().width*theDisplayRect.width;
		aReturnRect.height = GetResolution2D().height*theDisplayRect.height;
		return aReturnRect;
	}
	

	public static RenderTexture GetRenderTexture()
	{
		CheckInstance();
		if(itsInstance == null)
			return null;
		itsInstance.CreateCamera();		//create camera if rendertexture is requested, because from this moment on some camera renders to it.
		return itsInstance.itsRenderTexture;
	}
	
	/// <summary>
	/// Call this method in the OnPostRender of your camera to blit the rendertexture to screen
	/// </summary>
	/// <returns></returns>
	public static void BlitToScreen()
	{
		CheckInstance();
		if(itsInstance == null)
			return;
		if(itsInstance.itsRenderTexture != null)
		{
			Graphics.Blit(itsInstance.itsRenderTexture,(RenderTexture)null);
		}
	}

	#endregion

	#region private methods
	private void CreateCamera()
	{
		if(itsCamera != null)
			return;
		gameObject.AddComponent<Camera>();
		itsCamera = gameObject.GetComponent<Camera>();
		itsCamera.clearFlags = CameraClearFlags.SolidColor;
		itsCamera.backgroundColor = Color.black;
		itsCamera.cullingMask = 0;
		itsCamera.orthographic = true;
		itsCamera.orthographicSize = 1.0f;
		itsCamera.depth = 100;
		itsCamera.farClipPlane = 1;
		itsCamera.nearClipPlane = 0.5f;
	}
	
	private static void SetResolution3D(int theWidth, int theHeight, int theRefreshRate)
	{
		CheckInstance();
		if(itsInstance == null)
			return;
		itsInstance.itsDataModuleScreen.itsResolution3D.width = theWidth;
		itsInstance.itsDataModuleScreen.itsResolution3D.height = theHeight;
		itsInstance.itsDataModuleScreen.itsResolution3D.refreshRate = theRefreshRate;
		itsInstance.itsDataModuleScreen.itsAspect3D = ReadAspect(theWidth,theHeight);
		itsInstance.itsDataModuleScreen.itsScaleFactor3D = (float)GetResolutionDisplay().width/(float)theWidth;
		
		Debug.Log("set resolution 3D to: "+theWidth+"/"+theHeight+"/"+theRefreshRate);		
		itsInstance.CreateRenderTexture();
	}
	
	private static void SetResolution2D(int theWidth, int theHeight)
	{
		CheckInstance();
		if(itsInstance == null)
			return;
		itsInstance.itsDataModuleScreen.itsResolution2D.width = theWidth;
		itsInstance.itsDataModuleScreen.itsResolution2D.height = theHeight;
		itsInstance.itsDataModuleScreen.itsResolution2D.refreshRate = 0;
		itsInstance.itsDataModuleScreen.itsAspect2D = ReadAspect(theWidth,theHeight);
		itsInstance.itsDataModuleScreen.itsScaleFactor2D = (float)GetResolutionDisplay().width/(float)theWidth;
		
		Debug.Log("set resolution 2D to: "+theWidth+"/"+theHeight);		
	}
	
	private static void CheckInstance()
	{
		// check if the cheat module is already activated
		if(itsInstance == null)
		{
			UnityEngine.Object theObject = UnityEngine.Object.FindObjectOfType(typeof(KGFScreen));
			
			if(theObject != null)
			{
				itsInstance = theObject as KGFScreen;
				itsInstance.Init();
			}
			else
			{
				if(!itsAlreadyChecked)
				{
					UnityEngine.Debug.LogError("KGFScreen is not running. Make sure that there is an instance of the KGFScreen prefab in the current scene.");
					itsAlreadyChecked = true;
				}
			}
		}
	}
	
	private void Init()
	{
		Screen.SetResolution(Screen.currentResolution.width,Screen.currentResolution.height,false);
		StartCoroutine(SetResolutionDelayed());
	}
	
	IEnumerator SetResolutionDelayed()
	{
		yield return new WaitForSeconds(1.0f);
		ReadResolutionDisplay();
		
		Debug.Log("display resolution set to: "+GetResolutionDisplay().width+"/"+GetResolutionDisplay().height);		
		
		float anAspect = ReadAspect(GetResolutionDisplay().width,GetResolutionDisplay().height);
		
		int aHeight = itsDataModuleScreen.itsMinHeight;
		int aWidth = (int)(aHeight*anAspect);
		if(aWidth < itsDataModuleScreen.itsMinWidth)
		{
			aWidth = itsDataModuleScreen.itsMinWidth;
			aHeight = (int)(itsDataModuleScreen.itsMinWidth/anAspect);
		}
		
		eResolutionMode aResolutionMode3D = GetResolutionMode3D();
		
		if(aResolutionMode3D == eResolutionMode.eNative)
		{
			SetResolution3D(GetResolutionDisplay().width,GetResolutionDisplay().height);
		}
		else if(aResolutionMode3D == eResolutionMode.eAutoAdjust)
		{
			SetResolution3D(aWidth,aHeight);
		}
		if(itsDataModuleScreen.itsResolutionMode2D == eResolutionMode.eNative)
		{
			SetResolution2D(GetResolutionDisplay().width,GetResolutionDisplay().height);
		}
		else if(itsDataModuleScreen.itsResolutionMode2D == eResolutionMode.eAutoAdjust)
		{
			SetResolution2D(aWidth,aHeight);
		}
	}
	
	/// <summary>
	/// sets the current display resolution
	/// </summary>
	private void ReadResolutionDisplay()
	{
		itsInstance.itsDataModuleScreen.itsResolutionDisplay = new Resolution();
		itsInstance.itsDataModuleScreen.itsResolutionDisplay.width = Screen.width;
		itsInstance.itsDataModuleScreen.itsResolutionDisplay.height = Screen.height;
		itsInstance.itsDataModuleScreen.itsResolutionDisplay.refreshRate = 60;
		itsInstance.itsDataModuleScreen.itsAspectDisplay = ReadAspect(Screen.width,Screen.height);
	}
	
	private static float ReadAspect(int theWidth, int theHeight)
	{
		return (float)theWidth/(float)theHeight;
	}

	/// <summary>
	/// Create a rendertexture with resolution itsResolution
	/// </summary>
	private void CreateRenderTexture()
	{
		if(itsRenderTexture == null)
		{
			itsRenderTexture = new RenderTexture(GetResolution3D().width,GetResolution3D().height,16,RenderTextureFormat.ARGB32);
		}
		else
		{
			if(itsRenderTexture.width != GetResolution3D().width)
			{
				itsRenderTexture.Release();
				itsRenderTexture = new RenderTexture(GetResolution3D().width,GetResolution3D().height,16,RenderTextureFormat.ARGB32);
			}
		}
		itsRenderTexture.isPowerOfTwo = true;
		itsRenderTexture.name = "KGFScreenRenderTexture";
		itsRenderTexture.Create();
	}
	
	private void OnPostRender()
	{
		KGFScreen.BlitToScreen();
	}
	#endregion
}