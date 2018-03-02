// <author>Michal Kolasinski</author>
// <email>michal.kolasinski@kolmich.at</email>
// <date>2010-09-01</date>
// <summary>short summary</summary>

// PLEASE uncomment these lines if you do own the corresponding modules
//#define KGFDebug
//#define KGFConsole

// comment this, if you do not want to change values while in play mode
#define OnlineChangeMode

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Cache for assetbundles to instantiate them at a later time
/// </summary>
public class KGFMapSystem : KGFModule, KGFICustomGUI, KGFIValidator, KGFICustomInspectorGUI
{
	public KGFMapSystem() : base(new Version(1,1,0,0), new Version(1,1,0,0))
	{}
	
	#region Internal classes
	/// <summary>
	/// Event args for map icon events
	/// </summary>
	public class KGFMapIconEventArgs : EventArgs
	{
	}
	
	public class KGFFlagEventArgs : EventArgs
	{
		public KGFFlagEventArgs(Vector3 thePosition)
		{
			itsPosition = thePosition;
		}
		
		public Vector3 itsPosition = Vector3.zero;
	}
	
	/// <summary>
	/// Base class for all items
	/// </summary>
	[System.Serializable]
	public class KGFDataMinimap
	{
		/// <summary>
		/// Gui settings
		/// </summary>
		public minimap_gui_settings itsAppearanceMiniMap = new KGFMapSystem.minimap_gui_settings();
		
		public minimap_gui_fullscreen_settings itsAppearanceMap = new KGFMapSystem.minimap_gui_fullscreen_settings();
		
		public minimap_fogofwar_settings itsFogOfWar = new KGFMapSystem.minimap_fogofwar_settings();
		
		public minimap_zoom_settings itsZoom = new KGFMapSystem.minimap_zoom_settings();
		
		public minimap_viewport_settings itsViewport = new KGFMapSystem.minimap_viewport_settings();
		
		public minimap_photo_settings itsPhoto = new KGFMapSystem.minimap_photo_settings();
		
		public minimap_userflags_settings itsUserFlags = new KGFMapSystem.minimap_userflags_settings();
		
		public minimap_shader_settings itsShaders = new KGFMapSystem.minimap_shader_settings();
		
		/// <summary>
		/// the minimap camera will attacht to this object.
		/// </summary>
		public GameObject itsTarget = null;
		
		public bool itsIsStatic = true;
		public float itsStaticNorth = 0;
		public bool itsIsActive = true;
		public Color itsColorMap = Color.white;
		public Color itsColorBackground = Color.black;
		public Color itsColorAll = Color.white;
	}
	
	[System.Serializable]
	public class minimap_photo_settings
	{
		public bool itsTakePhoto = true;
		public LayerMask itsPhotoLayers;
	}
	
	[System.Serializable]
	public class minimap_shader_settings
	{
		public Shader itsShaderMapIcon = null;
		public Shader itsShaderPhotoPlane = null;
		public Shader itsShaderFogOfWar = null;
		public Shader itsShaderMapMask = null;
	}
	
	[System.Serializable]
	public class minimap_userflags_settings
	{
		public bool itsActive = true;
		public KGFMapIcon itsMapIcon = null;
		public float itsRemoveClickDistance = 1f;
	}
	
	[System.Serializable]
	public class minimap_viewport_settings
	{
		public bool itsActive = false;
		public Color itsColor = Color.grey;
		public Camera itsCamera;
	}
	
	[System.Serializable]
	public class minimap_gui_settings
	{
		public float itsSize = 0.2f;
		public float itsButtonSize = 30.0f;
		public float itsButtonPadding = 10;
		
		public Texture2D itsButton;
		public Texture2D itsButtonHover;
		public Texture2D itsButtonDown;
		
		public Texture2D itsIconZoomIn;
		public Texture2D itsIconZoomOut;
		public Texture2D itsIconZoomLock;
		public Texture2D itsIconFullscreen;
		public Texture2D itsBackground;
		public int itsBackgroundBorder = 0;
		
		public Texture2D itsMask;
		
		public float itsScaleIcons=1;
		public float itsScaleArrows=0.2f;
		public float itsRadiusArrows = 1;
		
		public KGFAlignmentVertical itsAlignmentVertical = KGFAlignmentVertical.Top;
		public KGFAlignmentHorizontal itsAlignmentHorizontal = KGFAlignmentHorizontal.Right;
	}
	
	[System.Serializable]
	public class minimap_gui_fullscreen_settings
	{
		public float itsButtonSize = 0.1f;
		public float itsButtonPadding = 0;
		public float itsButtonSpace = 0.01f;
		
		public Texture2D itsButton;
		public Texture2D itsButtonHover;
		public Texture2D itsButtonDown;
		
		public Texture2D itsIconZoomIn;
		public Texture2D itsIconZoomOut;
		public Texture2D itsIconZoomLock;
		public Texture2D itsIconFullscreen;
		public Texture2D itsBackground;
		public int itsBackgroundBorder = 0;
		
		public Texture2D itsMask;
		
		public float itsScaleIcons=1;
		
		public KGFAlignmentVertical itsAlignmentVertical = KGFAlignmentVertical.Top;
		public KGFAlignmentHorizontal itsAlignmentHorizontal = KGFAlignmentHorizontal.Right;
		public KGFOrientation itsOrientation;
	}
	
	[System.Serializable]
	public class minimap_fogofwar_settings
	{
		public bool itsActive = true;
		public int itsResolutionX = 10;
		public int itsResolutionY = 10;
		public float itsRevealDistance = 10;
		public float itsRevealedFullDistance = 5;
		public bool itsHideMapIcons = false;
	}
	
	[System.Serializable]
	public class minimap_zoom_settings
	{
		/// <summary>
		/// The start zoom of the minimap in meters (unity3d units). Range 15 means that objects are vivible on the minmap
		/// in a distance of 15 meters in each direction from the target
		/// </summary>
		public float itsZoomStartValue = 20.0f;
		/// <summary>
		/// When zooming this is the minimal range that can be reached.
		/// </summary>
		public float itsZoomMin = 10;
		
		/// <summary>
		/// When zoomout this is the maximal range that can be reached.
		/// </summary>
		public float itsZoomMax = 30;
		
		/// <summary>
		/// When zooming in or out this is the value that will be added or substracted to the current zoom value
		/// </summary>
		public float itsZoomChangeValue = 10.0f;
	}
	
	/// <summary>
	/// Internal class for listing map icon
	/// </summary>
	public class mapicon_listitem_script
	{
		public KGFMapSystem itsModule = null;
		
		public KGFIMapIcon itsMapIcon = null;
		public GameObject itsRepresentationInstance;
		public Transform itsRepresentationInstanceTransform;
		public bool itsRotate;
		public GameObject itsRepresentationArrowInstance;
		public Transform itsRepresentationArrowInstanceTransform;
		
		public Transform itsMapIconTransform;
		private bool itsVisibility = false;
		private bool itsVisibilityArrow = false;
		
		#region public methods
		/// <summary>
		/// Update visibility of map icon and arrows
		/// </summary>
		public void UpdateVisibility()
		{
			if (itsMapIcon != null)
			{
				if (itsRepresentationInstance != null)
				{
					bool aNewVisibility = itsMapIcon.GetIsVisible() && itsVisibility;
					if (aNewVisibility != itsRepresentationInstance.active)
					{
						foreach(Transform aTransform in itsRepresentationInstance.transform)
						{
							GameObject aGameObject = aTransform.gameObject;
							aGameObject.active = aNewVisibility;
						}
						itsRepresentationInstance.active = aNewVisibility;
						
						//itsRepresentationInstance.SetActiveRecursively(aNewVisibility);
						LogInfo(string.Format("Icon of '{0}' (category='{1}') changed visibility to: {2}",
						                      itsMapIcon.GetTransform().name,
						                      itsMapIcon.GetCategory(),
						                      aNewVisibility),itsModule.name,itsModule);
					}
				}
				if (itsRepresentationArrowInstance != null)
				{
					itsRepresentationArrowInstance.SetActiveRecursively(itsMapIcon.GetIsVisible() && itsVisibility && itsVisibilityArrow && itsMapIcon.GetIsArrowVisible());
				}
			}
		}
		
		/// <summary>
		/// Update colors and textures of this icon
		/// </summary>
		public void UpdateIcon()
		{
			if (itsMapIcon != null)
			{
				// colors
				SetColorsInChildren(itsRepresentationArrowInstance,itsMapIcon.GetColor());
				SetColorsInChildren(itsRepresentationInstance,itsMapIcon.GetColor());
				
				// textures
				if (itsRepresentationArrowInstance != null)
				{
					MeshRenderer aRenderer = itsRepresentationArrowInstance.GetComponent<MeshRenderer>();
					if (aRenderer != null)
					{
						aRenderer.material.mainTexture = itsMapIcon.GetTextureArrow();
					}
				}
			}
		}
		
		/// <summary>
		/// Change the colors of all meshrenderers that are found in all child objects
		/// </summary>
		/// <param name="theGameObject"></param>
		/// <param name="theColor"></param>
		void SetColorsInChildren(GameObject theGameObject,Color theColor)
		{
			if (theGameObject != null)
			{
				MeshRenderer []aMeshrendererList = theGameObject.GetComponentsInChildren<MeshRenderer>(true);
				if (aMeshrendererList != null)
				{
					foreach (MeshRenderer aMeshRenderer in aMeshrendererList)
					{
						Material aMaterial = aMeshRenderer.material;
						if (aMaterial != null)
						{
							aMaterial.color = theColor;
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Change visibility of map icon
		/// </summary>
		/// <param name="theVisible"></param>
		public void SetVisibility(bool theVisible)
		{
			if (theVisible != itsVisibility)
			{
				itsVisibility = theVisible;
				UpdateVisibility();
			}
		}
		
		/// <summary>
		/// Returns TRUE, if the map icon is visible effectively (all conditions need to be true)
		/// </summary>
		/// <returns></returns>
		public bool GetMapIconVisibilityEffective()
		{
			if (itsMapIcon != null)
			{
				return itsVisibility && itsMapIcon.GetIsVisible();
			}
			return false;
		}
		
		/// <summary>
		/// Change visibility of arrow, only displayed if map icon itsself is also visible
		/// </summary>
		/// <param name="theShow"></param>
		public void ShowArrow(bool theShow)
		{
			if (theShow != itsVisibilityArrow && itsRepresentationArrowInstance != null)
			{
				itsVisibilityArrow = theShow;
				UpdateVisibility();
			}
		}
		
		/// <summary>
		/// Get set visibility of arrow
		/// </summary>
		/// <returns></returns>
		public bool GetIsArrowVisible()
		{
			return itsVisibilityArrow;
		}
		
		/// <summary>
		/// Destroy this item
		/// </summary>
		public void Destroy()
		{
			if (itsRepresentationArrowInstance != null)
			{
				GameObject.Destroy(itsRepresentationArrowInstance);
			}
			if (itsRepresentationInstance != null)
			{
				GameObject.Destroy(itsRepresentationInstance);
			}
		}
		#endregion
	}
	#endregion
	
	/// <summary>
	/// Data component
	/// </summary>
	public KGFDataMinimap itsDataModuleMinimap = new KGFDataMinimap();
	
	#region private variables
	float itsFixedYPhoto = 0f;
	float itsFixedYViewPort = 1f;
	float itsFixedYFog = 2f;
	float itsFixedYIcons = 3f;
	float itsFixedYArrows = 10f;
	
	float itsFixedYFlags = 0f;
	
	const string itsMeasureBoxName = "measure_cube";
	const string itsLayerName = "mapsystem";
	
	/// <summary>
	/// State of minimap
	/// </summary>
	private bool itsMinimapActive = true;
	
	/// <summary>
	/// List of all map icons
	/// </summary>
	private List<mapicon_listitem_script> itsListMapIcons = new List<mapicon_listitem_script>();
	
	/// <summary>
	/// Layer for items visible on the minimap
	/// </summary>
	private int itsLayerMinimap;
	/// <summary>
	/// Caching camera target transform for performance reasons. Unity3D .transform access is imperformant
	/// </summary>
	private Transform itsTargetTransform;
	
	/// <summary>
	/// Container transform for all user created flags
	/// </summary>
	private Transform itsContainerFlags;
	/// <summary>
	/// Container transform for all user created icons
	/// </summary>
	private Transform itsContainerUser;
	/// <summary>
	/// Container transform for all map icons
	/// </summary>
	private Transform itsContainerIcons;
	/// <summary>
	/// Container transform for all arrows
	/// </summary>
	private Transform itsContainerIconArrows;
	
	/// <summary>
	/// Material for autocreated photo
	/// </summary>
	private Material itsMaterialMaskedMinimap = null;
	
	/// <summary>
	/// Material for autocreated photo
	/// </summary>
	private Material itsMaterialPhoto = null;
	
	/// <summary>
	/// Material for the viewport
	/// </summary>
	private Material itsMaterialViewport = null;
	
	/// <summary>
	/// this camera will render all objects in the minimap layer
	/// </summary>
	private Camera itsCamera = null;
	/// <summary>
	/// Caching camera transform for performance reasons. Unity3D .transform access is imperformant
	/// </summary>
	private Transform itsCameraTransform;
	/// <summary>
	/// Output camera for minimap
	/// </summary>
	private Camera itsCameraOutput;
	/// <summary>
	/// itsRendertexture will be applied to the material of this plane
	/// </summary>
	private GameObject itsMinimapPlane = null;
	/// <summary>
	/// cachevariable will save performance by preventing using getcomponent in lateupdate
	/// </summary>
	private MeshFilter itsMinimapMeshFilter = null;
	/// <summary>
	/// itsCamera will render into this texture
	/// </summary>
	private RenderTexture itsRendertexture = null;
	
	/// <summary>
	/// Target draw rect of the map
	/// </summary>
	private Rect itsTargetRect;
	
	private Rect itsRectZoomIn;
	private Rect itsRectZoomOut;
	private Rect itsRectStatic;
	private Rect itsRectFullscreen;
	
	/// <summary>
	/// Gui style for buttons
	/// </summary>
	GUIStyle itsGuiStyleButton;
	
	/// <summary>
	/// Gui style for buttons in fullscreen mode
	/// </summary>
	GUIStyle itsGuiStyleButtonFullscreen;
	
	/// <summary>
	/// Empty gui style for background
	/// </summary>
	GUIStyle itsGuiStyleBack;
	
	MeshFilter itsMeshFilterFogOfWarPlane;
//	Color itsColorFogOfWarBlack = new Color(0,0,0,1);
	Color itsColorFogOfWarRevealed = new Color(0,0,0,0);
	Vector2 itsSizeTerrain = Vector2.zero;
	Vector2 itsScalingFogOfWar = Vector2.zero;
	MeshRenderer itsMeshRendererMinimapPlane;
	
	Bounds itsTerrainBounds;
	GameObject itsBoundingBox;
	
	Vector2 ?itsSavedResolution = null;
	/// <summary>
	/// Fullscreen mode active flag
	/// </summary>
	bool itsModeFullscreen = false;
	/// <summary>
	/// The current zoom level in pixels per meter
	/// </summary>
	float itsPixelPerMeter;
	/// <summary>
	/// List of all user created icons
	/// </summary>
	List<KGFMapIcon> itsListUserIcons = new List<KGFMapIcon>();
	/// <summary>
	/// List of all icons created by click
	/// </summary>
	List<KGFIMapIcon> itsListClickIcons = new List<KGFIMapIcon>();
	#endregion
	
	#region public events
	/// <summary>
	/// Triggered if:
	///  * map icon got too far away from character to be displayed on minimap
	///  * map icon got closer so it gots displayed on minimap
	/// </summary>
	public KGFDelegate EventVisibilityOnMinimapChanged = new KGFDelegate();
	
	/// <summary>
	/// Triggered if:
	///  * a new map icon was created by click
	/// </summary>
	public KGFDelegate EventUserFlagSet = new KGFDelegate();
	#endregion
	
	#region Internal methods
	protected override void KGFAwake()
	{
		if (itsDataModuleMinimap.itsFogOfWar.itsHideMapIcons)
		{
			itsFixedYFog = 5;
		}
		else
		{
			itsFixedYFog = 2;
		}
		
		UpdateStyles();
		
		itsLayerMinimap = LayerMask.NameToLayer(itsLayerName);
		if (itsLayerMinimap < 0)
		{
			LogError(string.Format("Missing layer '{0}'.",itsLayerName),name,this);
			enabled = false;
			return;
		}
		
		CreateCameras();
		CreateRenderTexture();
		
		itsContainerFlags = (new GameObject("flags")).transform;
		itsContainerFlags.parent = transform;
		
		itsContainerUser = (new GameObject("user")).transform;
		itsContainerUser.parent = transform;
		
		itsContainerIcons = (new GameObject("icons")).transform;
		itsContainerIcons.parent = transform;
		
		itsContainerIconArrows = (new GameObject("arrows")).transform;
		itsContainerIconArrows.parent = transform;
		
		// register existing map icons
		foreach (KGFIMapIcon anIcon in KGFAccessor.GetObjects<KGFIMapIcon>())
			RegisterIcon(anIcon);
		
		SetTarget(itsDataModuleMinimap.itsTarget);
		// wait for new map icons
		KGFAccessor.RegisterAddEvent<KGFIMapIcon>(OnMapIconAdd);
		KGFAccessor.RegisterRemoveEvent<KGFIMapIcon>(OnMapIconRemove);
		
		#if KGFConsole
		KGFConsole.AddCommand("k.ms.ac","Enable/disable minimap",name,this,"SetMinimapEnabled");
		KGFConsole.AddCommand("k.ms.st","Set static mode",name,this,"SetModeStatic");
		KGFConsole.AddCommand("k.ms.v","Change visibility of icons by category",name,this,"SetIconsVisibleByCategory");
		KGFConsole.AddCommand("k.ms.r","Refresh icon visibility",name,this,"RefreshIconsVisibility");
		
		KGFConsole.AddCommand("k.ms.zi","Zoom in",name,this,"ZoomIn");
		KGFConsole.AddCommand("k.ms.zo","Zoom out",name,this,"ZoomOut");
		KGFConsole.AddCommand("k.ms.zmin","Set zoom to minimum",name,this,"ZoomMin");
		KGFConsole.AddCommand("k.ms.zmax","Set zoom to maximum",name,this,"ZoomMax");
		
		KGFConsole.AddCommand("k.ms.fs","Set fullscreen mode of minimap",name,this,"SetFullscreen");
		KGFConsole.AddCommand("k.ms.vp","Set visibility of camera viewport",name,this,"SetViewportEnabled");
		
		KGFConsole.AddCommand("k.ms.fws","Save current fog of war",name,this,"Save");
		KGFConsole.AddCommand("k.ms.fwl","Load fog of war",name,this,"Load");
		KGFConsole.AddCommand("k.ms.fwr","Reveal fog of war",name,this,"RevealFogOfWar");
		
		#endif
	}
	
	/// <summary>
	/// Checks if the user has pro version installed
	/// </summary>
	/// <returns></returns>
	bool GetHasProVersion()
	{
		return SystemInfo.supportsRenderTextures;
	}
	
	/// <summary>
	/// Returns true if the mouse is hovering over the map or one of the map buttons.
	/// Can be used to disable e.g.: Character movement while interacting with the map
	/// </summary>
	/// <returns></returns>
	public bool GetHover()
	{
		Vector2 aMousePosition = Input.mousePosition;
		aMousePosition.y = Screen.height - aMousePosition.y;
		
		if(itsTargetRect.Contains(aMousePosition))
			return true;
		if(itsRectZoomIn.Contains(aMousePosition))
			return true;
		if(itsRectZoomOut.Contains(aMousePosition))
			return true;
		if(itsRectStatic.Contains(aMousePosition))
			return true;
		if(itsRectFullscreen.Contains(aMousePosition))
			return true;
		return false;
	}
	
	void Start()
	{
		InitBounds();
		AutoCreatePhoto();
		
		if(itsDataModuleMinimap.itsShaders.itsShaderFogOfWar == null)
		{
			LogWarning("itsDataModuleMinimap.itsShaders.itsShaderFogOfWar is not assigned. Please install the standard unity particle package. Assign the Particle Alpha Blend Shader to itsDataModuleMinimap.itsShaders.itsShaderFogOfWar.",name,this);
		}
		else
		{
			if (itsDataModuleMinimap.itsFogOfWar.itsActive)
			{
				InitFogOfWar();
			}
		}
		
		itsMinimapPlane = GenerateMinimapPlane();
		MeshRenderer aMeshRenderer = itsMinimapPlane.GetComponent<MeshRenderer>();
		if(aMeshRenderer == null)
		{
			LogError("Cannot find meshrenderer",name,this);
		}
		else
		{
			aMeshRenderer.material.SetTexture("_MainTex",itsRendertexture);
		}
		
		SetViewportEnabled(itsDataModuleMinimap.itsViewport.itsActive);
		SetMinimapEnabled(itsDataModuleMinimap.itsIsActive);
	}
	
	/// <summary>
	/// Get real map width in pixels
	/// </summary>
	/// <returns></returns>
	float GetWidth()
	{
		if (GetFullscreen())
		{
			return Screen.width;
		}
		else
		{
			return itsDataModuleMinimap.itsAppearanceMiniMap.itsSize * (float)Screen.width;
		}
	}
	
	/// <summary>
	/// Get real map height in pixels
	/// </summary>
	/// <returns></returns>
	float GetHeight()
	{
		if (GetFullscreen())
		{
			return Screen.height;
		}
		else
		{
			return GetWidth();//itsDataModuleMinimap.itsAppearanceMiniMap.itsSize * (float)Screen.height;
		}
	}
	
	/// <summary>
	/// Get real button size in pixels
	/// </summary>
	/// <returns></returns>
	float GetButtonSize()
	{
		if (GetFullscreen())
		{
			return itsDataModuleMinimap.itsAppearanceMap.itsButtonSize * GetWidth();
		}
		else
		{
			return itsDataModuleMinimap.itsAppearanceMiniMap.itsButtonSize * GetWidth();
		}
	}
	
	/// <summary>
	/// Get real button padding in pixels
	/// </summary>
	/// <returns></returns>
	float GetButtonPadding()
	{
		if (GetFullscreen())
		{
			return itsDataModuleMinimap.itsAppearanceMap.itsButtonPadding * GetWidth();
		}
		else
		{
			return itsDataModuleMinimap.itsAppearanceMiniMap.itsButtonPadding * GetWidth();
		}
	}
	
	#region log abstraction
	public static void LogError(string theError,string theCategory,MonoBehaviour theObject)
	{
		#if KGFDebug
		KGFDebug.LogError(theError,theCategory,theObject);
		#else
		Debug.LogError(string.Format("{0} - {1}",theCategory,theError));
		#endif
	}
	
	public static void LogWarning(string theWarning,string theCategory,MonoBehaviour theObject)
	{
		#if KGFDebug
		KGFDebug.LogWarning(theWarning,theCategory,theObject);
		#else
		Debug.LogWarning(string.Format("{0} - {1}",theCategory,theWarning));
		#endif
	}
	
	static void LogInfo(string theError,string theCategory,MonoBehaviour theObject)
	{
		#if KGFDebug
		KGFDebug.LogInfo(theError,theCategory,theObject);
		#else
		Debug.Log(string.Format("{0} - {1}",theCategory,theError));
		#endif
	}
	#endregion
	
	#region mesh creation
	/// <summary>
	/// Generate a new plane mesh. it is 1x1 in size
	/// </summary>
	/// <returns></returns>
	private Mesh GeneratePlaneMeshXZ()
	{
		Mesh aMesh = new Mesh();
		
		Vector3[] aVertices = new Vector3[4];
		aVertices[0] = new Vector3(0.0f,0.0f,0.0f);
		aVertices[1] = new Vector3(1.0f,0.0f,0.0f);
		aVertices[2] = new Vector3(1.0f,0.0f,1.0f);
		aVertices[3] = new Vector3(0.0f,0.0f,1.0f);
		
		Vector3[] aNormals = new Vector3[4];
		aNormals[0] = new Vector3(0.0f,1.0f,0.0f);
		aNormals[1] = new Vector3(0.0f,1.0f,0.0f);
		aNormals[2] = new Vector3(0.0f,1.0f,0.0f);
		aNormals[3] = new Vector3(0.0f,1.0f,0.0f);
		
		Vector2[] anUVs = new Vector2[4];
		anUVs[0] = new Vector2(0.0f,0.0f);
		anUVs[1] = new Vector2(1.0f,0.0f);
		anUVs[2] = new Vector2(1.0f,1.0f);
		anUVs[3] = new Vector2(0.0f,1.0f);
		
		
		int[] aTriangles = new int[6];
		aTriangles[0] = 0;
		aTriangles[1] = 3;
		aTriangles[2] = 2;
		aTriangles[3] = 0;
		aTriangles[4] = 2;
		aTriangles[5] = 1;
		
		aMesh.vertices = aVertices;
		aMesh.normals = aNormals;
		aMesh.uv = anUVs;
		aMesh.triangles = aTriangles;
		
		return aMesh;
	}
	
	/// <summary>
	/// Generate a new plane mesh. it is 1x1 in size
	/// </summary>
	/// <returns></returns>
	private static Mesh GeneratePlaneMeshXZCentered()
	{
		Mesh aMesh = new Mesh();
		
		Vector3[] aVertices = new Vector3[4];
		aVertices[0] = new Vector3(-0.5f,0.0f,-0.5f);
		aVertices[1] = new Vector3(0.5f,0.0f,-0.5f);
		aVertices[2] = new Vector3(0.5f,0.0f,0.5f);
		aVertices[3] = new Vector3(-0.5f,0.0f,0.5f);
		
		Vector3[] aNormals = new Vector3[4];
		aNormals[0] = new Vector3(0.0f,1.0f,0.0f);
		aNormals[1] = new Vector3(0.0f,1.0f,0.0f);
		aNormals[2] = new Vector3(0.0f,1.0f,0.0f);
		aNormals[3] = new Vector3(0.0f,1.0f,0.0f);
		
		Vector2[] anUVs = new Vector2[4];
		anUVs[0] = new Vector2(0.0f,0.0f);
		anUVs[1] = new Vector2(1.0f,0.0f);
		anUVs[2] = new Vector2(1.0f,1.0f);
		anUVs[3] = new Vector2(0.0f,1.0f);
		
		
		int[] aTriangles = new int[6];
		aTriangles[0] = 0;
		aTriangles[1] = 3;
		aTriangles[2] = 2;
		aTriangles[3] = 0;
		aTriangles[4] = 2;
		aTriangles[5] = 1;
		
		aMesh.vertices = aVertices;
		aMesh.normals = aNormals;
		aMesh.uv = anUVs;
		aMesh.triangles = aTriangles;
		
		return aMesh;
	}
	
//	Mesh CreateHexagonMesh()
//	{
//		Mesh aMesh = new Mesh();
//
//		Vector3[] aVertices = new Vector3[4];
//		aVertices[0] = new Vector3(0.0f,0.0f,0.0f);
//		aVertices[1] = new Vector3(1.0f,0.0f,0.0f);
//		aVertices[2] = new Vector3(1.0f,1.0f,0.0f);
//		aVertices[3] = new Vector3(0.0f,1.0f,0.0f);
//
//		Vector3[] aNormals = new Vector3[4];
//		aNormals[0] = new Vector3(0.0f,0.0f,1.0f);
//		aNormals[1] = new Vector3(0.0f,0.0f,1.0f);
//		aNormals[2] = new Vector3(0.0f,0.0f,1.0f);
//		aNormals[3] = new Vector3(0.0f,0.0f,1.0f);
//
	////		Vector2[] anUVs = new Vector2[4];
	////		anUVs[0] = new Vector2(0.0f,0.0f);
	////		anUVs[1] = new Vector2(1.0f,0.0f);
	////		anUVs[2] = new Vector2(1.0f,1.0f);
	////		anUVs[3] = new Vector2(0.0f,1.0f);
//
//
//		int[] aTriangles = new int[6];
//		aTriangles[0] = 0;
//		aTriangles[1] = 3;
//		aTriangles[2] = 2;
//		aTriangles[3] = 0;
//		aTriangles[4] = 2;
//		aTriangles[5] = 1;
//
//		aMesh.vertices = aVertices;
//		aMesh.normals = aNormals;
	////		aMesh.uv = anUVs;
//		aMesh.triangles = aTriangles;
//
//		return aMesh;
//	}
	
	Mesh CreatePlaneMesh(int theWidth,int theHeight)
	{
		Mesh aMesh = new Mesh();
		
		Vector3[] aVertices = new Vector3[(theWidth+1)*(theHeight+1)];
		
		for (int y=0;y<=theHeight;y++)
		{
			for (int x=0;x<=theWidth;x++)
			{
				aVertices[y*(theWidth+1)+x] = new Vector3(x,0.0f,y);
			}
		}
		
		Vector3[] aNormals = new Vector3[aVertices.Length];
		for (int i=0;i<aNormals.Length;i++)
		{
			aNormals[i] = new Vector3(0.0f,1.0f,0.0f);
		}
		
		int[] aTriangles = new int[aVertices.Length * 2 * 3];
		int aTriangleCurrent = 0;
		
		for (int y=0;y<theHeight;y++)
		{
			for (int x=0;x<theWidth;x++)
			{
				int index = y*(theWidth+1) + x;
				
				if (index%2 == 0)
				{
					aTriangles[aTriangleCurrent++] = index;
					aTriangles[aTriangleCurrent++] = index+(theWidth+2);
					aTriangles[aTriangleCurrent++] = index+(theWidth+1);
					aTriangles[aTriangleCurrent++] = index;
					aTriangles[aTriangleCurrent++] = index+1;
					aTriangles[aTriangleCurrent++] = index+theWidth+2;
				}else
				{
					aTriangles[aTriangleCurrent++] = index;
					aTriangles[aTriangleCurrent++] = index+1;
					aTriangles[aTriangleCurrent++] = index+(theWidth+1);
					
					aTriangles[aTriangleCurrent++] = index+1;
					aTriangles[aTriangleCurrent++] = index+theWidth+2;
					aTriangles[aTriangleCurrent++] = index+(theWidth+1);
				}
			}
		}
		
		Vector2[] anUVs = new Vector2[aVertices.Length];
		for (int i=0;i<anUVs.Length;i++)
		{
			anUVs[i] = Vector2.zero;
		}
		
		aMesh.vertices = aVertices;
		aMesh.normals = aNormals;
		aMesh.uv = anUVs;
		aMesh.triangles = aTriangles;
		
		return aMesh;
	}
	#endregion
	
	#region fog of war
	const string itsSaveIDFogOfWarValues = "minimap_FogOfWar_values";
	
	/// <summary>
	/// Serialize fog of war to string
	/// </summary>
	/// <returns></returns>
	string SerializeFogOfWar()
	{
		if (itsMeshFilterFogOfWarPlane != null)
		{
			if (itsMeshFilterFogOfWarPlane.mesh.colors != null)
			{
				string[] anArrayAlphas = new string[itsMeshFilterFogOfWarPlane.mesh.vertices.Length];
				for (int i=0;i<itsMeshFilterFogOfWarPlane.mesh.vertices.Length;i++)
				{
					anArrayAlphas[i] = ""+itsMeshFilterFogOfWarPlane.mesh.colors[i].a;
				}
				string aSaveString = string.Join(";",anArrayAlphas);
				return aSaveString;
			}
		}
		return null;
	}
	
	/// <summary>
	/// Save current fog of war state
	/// </summary>
	void Save(string theSaveGameName)
	{
		string aSaveString = SerializeFogOfWar();
		if (aSaveString != null)
		{
			PlayerPrefs.SetString(theSaveGameName+itsSaveIDFogOfWarValues,aSaveString);
		}
	}
	
	/// <summary>
	/// Load fog of war state from previously serialized string
	/// </summary>
	/// <param name="theSaveString"></param>
	void DeserializeFogOfWar(string theSavedString)
	{
		if (theSavedString != null)
		{
			if (itsMeshFilterFogOfWarPlane != null)
			{
				if (itsMeshFilterFogOfWarPlane.mesh.colors != null)
				{
					Color[] aColorArray = itsMeshFilterFogOfWarPlane.mesh.colors;
					string[] anArrayAlphas = theSavedString.Split(';');
					if (anArrayAlphas.Length == aColorArray.Length)
					{
						for (int i=0;i<anArrayAlphas.Length;i++)
						{
							try{
								aColorArray[i].a = float.Parse(anArrayAlphas[i]);
							}
							catch
							{
								LogError("Could not parse saved fog of war",name,this);
								return;
							}
						}
						itsMeshFilterFogOfWarPlane.mesh.colors = aColorArray;
					}else
					{
						LogError("Saved fog of war size different from current.",name,this);
					}
				}
			}
		}
		else
		{
			LogError("No saved fog of war to load.",name,this);
		}
	}
	
	/// <summary>
	/// Load fog of war state
	/// </summary>
	void Load(string theSaveGameName)
	{
		string aSavedString = PlayerPrefs.GetString(theSaveGameName+itsSaveIDFogOfWarValues,null);
		DeserializeFogOfWar(aSavedString);
	}
	
	/// <summary>
	/// Reveals the whole fog of war
	/// </summary>
	void RevealFogOfWar()
	{
		if(itsMeshFilterFogOfWarPlane == null)
			return;
		if(itsMeshFilterFogOfWarPlane.mesh == null)
			return;
		
		Color[] aColorArray = itsMeshFilterFogOfWarPlane.mesh.colors;
		for(int i = 0; i< aColorArray.Length; i++)
		{
			aColorArray[i].a = 0.0f;
		}
		itsMeshFilterFogOfWarPlane.mesh.colors = aColorArray;
	}
	
	GameObject CreateFogOfWarPlane()
	{
		GameObject aGameobject = new GameObject("fog_of_war");
		aGameobject.transform.parent = transform;
		aGameobject.layer = itsLayerMinimap;
		
		MeshFilter aFilter = aGameobject.AddComponent<MeshFilter>();
		aFilter.mesh = CreatePlaneMesh((int)itsDataModuleMinimap.itsFogOfWar.itsResolutionX,(int)itsDataModuleMinimap.itsFogOfWar.itsResolutionY);
		MeshRenderer aRenderer = aGameobject.AddComponent<MeshRenderer>();
		aRenderer.material = new Material(itsDataModuleMinimap.itsShaders.itsShaderFogOfWar);
		return aGameobject;
	}
	
	void InitFogOfWar()
	{
		itsScalingFogOfWar = new Vector2(itsSizeTerrain.x / itsDataModuleMinimap.itsFogOfWar.itsResolutionX,itsSizeTerrain.y / itsDataModuleMinimap.itsFogOfWar.itsResolutionY);
		
		itsMeshFilterFogOfWarPlane = CreateFogOfWarPlane().GetComponent<MeshFilter>();
		Vector3 aPlanePosition = itsTerrainBounds.center - itsTerrainBounds.extents;
		aPlanePosition.y = itsFixedYFog;
		itsMeshFilterFogOfWarPlane.transform.position = aPlanePosition;
		itsMeshFilterFogOfWarPlane.transform.localScale = new Vector3(itsScalingFogOfWar.x,1,
		                                                              itsScalingFogOfWar.y);
		
		Color [] aColorArray = itsMeshFilterFogOfWarPlane.mesh.colors;
		aColorArray = new Color[itsMeshFilterFogOfWarPlane.mesh.vertexCount];
		for (int i=0;i<aColorArray.Length;i++)
		{
			aColorArray[i] = itsDataModuleMinimap.itsColorBackground;//itsColorFogOfWarBlack;
		}
		itsMeshFilterFogOfWarPlane.mesh.colors = aColorArray;
	}
	
//	void PaintFogOfWarPixel(Vector2 thePoint)
//	{
//		if (itsTextureFogWar != null)
//		{
//			if (thePoint.x >= 0 && thePoint.x < itsTextureFogWar.width &&
//			    thePoint.y >= 0 && thePoint.y < itsTextureFogWar.height)
//			{
//				itsTextureFogWar.SetPixel((int)thePoint.x,(int)thePoint.y,new Color(1,1,1));
//			}
//		}
//	}
//
//	void PaintFogOfWarCircle(Vector2 thePoint, int theRadius)
//	{
//		if (itsTextureFogWar != null)
//		{
//			Vector2 aCurrentPoint = new Vector2();
//			for (int x=(int)thePoint.x-theRadius;x<=thePoint.x+theRadius;x++)
//			{
//				for (int y=(int)thePoint.y-theRadius;y<=thePoint.y+theRadius;y++)
//				{
//					aCurrentPoint.x = x;
//					aCurrentPoint.y = y;
//					if (Vector2.Distance(thePoint,aCurrentPoint) <= theRadius)
//					{
//						PaintFogOfWarPixel(aCurrentPoint);
//					}
//				}
//			}
//			itsTextureFogWar.Apply();
//		}
//	}
	
	//	float itsTimeLastFogOfWarUpdate = 0;
	
	/// <summary>
	/// Reveal fog of war
	/// </summary>
	void UpdateFogOfWar()
	{
		//TODO: this code for much better performance depends on doing animation differently
//		if (Time.time - itsTimeLastFogOfWarUpdate < 0.1f)
//		{
//			return;
//		}
//		itsTimeLastFogOfWarUpdate = Time.time;
		
		Color [] aColorArray = itsMeshFilterFogOfWarPlane.mesh.colors;
		
		// calculate nearest vertext
		Vector3 aMyLocalPosition = itsTargetTransform.position - itsMeshFilterFogOfWarPlane.transform.position;
		Vector2 aNearestVertex = new Vector2(Mathf.RoundToInt((aMyLocalPosition.x/itsSizeTerrain.x)*itsDataModuleMinimap.itsFogOfWar.itsResolutionX),
		                                     Mathf.RoundToInt((aMyLocalPosition.z/itsSizeTerrain.y)*itsDataModuleMinimap.itsFogOfWar.itsResolutionY));
		
		// calculate how many vertexes we have to look at (width and height)
		Vector2 aNeededVertexSize = new Vector2(Mathf.RoundToInt((itsDataModuleMinimap.itsFogOfWar.itsRevealDistance/itsSizeTerrain.x)*itsDataModuleMinimap.itsFogOfWar.itsResolutionX),
		                                        Mathf.RoundToInt((itsDataModuleMinimap.itsFogOfWar.itsRevealDistance/itsSizeTerrain.y)*itsDataModuleMinimap.itsFogOfWar.itsResolutionY))*2;
//			new Vector2(5,5);//Vector2.zero;
//		print("vertex:"+aNearestVertex+" size="+aNeededVertexSize);
		// itsMeshFilterFogOfWarPlane.transform.position;
		// itsSizeTerrain
		// -> itsTargetTransform.position
		Vector3 aVertex = Vector3.zero;
		for (int y=Math.Max(0,(int)(aNearestVertex.y-aNeededVertexSize.y));y<Math.Min(itsDataModuleMinimap.itsFogOfWar.itsResolutionY+1,(int)(aNearestVertex.y+aNeededVertexSize.y));y++)
		{
			for (int x=Math.Max(0,(int)(aNearestVertex.x-aNeededVertexSize.x));x<Math.Min(itsDataModuleMinimap.itsFogOfWar.itsResolutionX+1,(int)(aNearestVertex.x+aNeededVertexSize.x));x++)
			{
				int i = (int)(y*(itsDataModuleMinimap.itsFogOfWar.itsResolutionX+1) + x);
				aVertex = itsMeshFilterFogOfWarPlane.transform.position + new Vector3(itsMeshFilterFogOfWarPlane.mesh.vertices[i].x * itsScalingFogOfWar.x,
				                                                                      0,
				                                                                      itsMeshFilterFogOfWarPlane.mesh.vertices[i].z * itsScalingFogOfWar.y);
				aVertex.y = itsTargetTransform.position.y;
				float aCurrentDistance = Vector3.Distance(aVertex,itsTargetTransform.position);
				if (aCurrentDistance < itsDataModuleMinimap.itsFogOfWar.itsRevealedFullDistance)
				{
					aColorArray[i] = itsColorFogOfWarRevealed;
				}
				else if (aCurrentDistance < itsDataModuleMinimap.itsFogOfWar.itsRevealDistance)
				{
					float aMin = Mathf.Min(aColorArray[i].a,
					                       Mathf.Clamp((aCurrentDistance-itsDataModuleMinimap.itsFogOfWar.itsRevealedFullDistance)/
					                                   (itsDataModuleMinimap.itsFogOfWar.itsRevealDistance-itsDataModuleMinimap.itsFogOfWar.itsRevealedFullDistance),0,1));
					aColorArray[i].a = aMin;
				}
			}
		}
//		for (int i=0;i<itsMeshFilterFogOfWarPlane.mesh.vertices.Length;i++)
//		{
//			aVertex = itsMeshFilterFogOfWarPlane.transform.position + new Vector3(itsMeshFilterFogOfWarPlane.mesh.vertices[i].x * itsScalingFogOfWar.x,
//			                                                                      0,
//			                                                                      itsMeshFilterFogOfWarPlane.mesh.vertices[i].z * itsScalingFogOfWar.y);
//			aVertex.y = itsTargetTransform.position.y;
//			float aCurrentDistance = Vector3.Distance(aVertex,itsTargetTransform.position);
//			if (aCurrentDistance < itsDataModuleMinimap.itsFogOfWarRevealedFullDistance)
//			{
//				aColorArray[i] = itsColorFogOfWarRevealed;
//			}
//			else if (aCurrentDistance < itsDataModuleMinimap.itsFogOfWarRevealDistance)
//			{
//				float aMin = Mathf.Min(aColorArray[i].a,
//				                       Mathf.Clamp((aCurrentDistance-itsDataModuleMinimap.itsFogOfWarRevealedFullDistance)/
//				                                   (itsDataModuleMinimap.itsFogOfWarRevealDistance-itsDataModuleMinimap.itsFogOfWarRevealedFullDistance),0,1));
//				aColorArray[i].a = aMin;
//			}
//		}
		
		itsMeshFilterFogOfWarPlane.mesh.colors = aColorArray;
	}
	#endregion
	
	#region viewport
	GameObject itsGameObjectViewPort = null;
	Mesh itsViewPortCubeMesh = null;
	/// <summary>
	/// Update the viewport represenation in the minimap
	/// </summary>
	void UpdateViewPortCube()
	{
		if (itsDataModuleMinimap.itsTarget == null)
		{
			return;
		}
		if (itsDataModuleMinimap.itsViewport.itsCamera == null)
		{
			return;
		}
		
		// create viewport
		if (itsViewPortCubeMesh == null)
		{
//			GameObject aGameObjectCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			itsGameObjectViewPort = new GameObject();
			itsGameObjectViewPort.AddComponent<MeshFilter>().mesh = GeneratePlaneMeshXZ();
			
			
			itsMaterialViewport = new Material(itsDataModuleMinimap.itsShaders.itsShaderMapIcon);
			itsGameObjectViewPort.AddComponent<MeshRenderer>().material = itsMaterialViewport;
			itsGameObjectViewPort.name = "minimap viewport";
			itsGameObjectViewPort.transform.parent = transform;
			SetLayerRecursively(itsGameObjectViewPort,itsLayerMinimap);
			itsViewPortCubeMesh = itsGameObjectViewPort.GetComponent<MeshFilter>().mesh;
		}
		
		if (itsGameObjectViewPort.active != itsDataModuleMinimap.itsViewport.itsActive)
		{
			itsGameObjectViewPort.SetActiveRecursively(itsDataModuleMinimap.itsViewport.itsActive);
		}
		
		if (!itsDataModuleMinimap.itsViewport.itsActive)
		{
			return;
		}
		
		// change vertices of the viewport
		Vector3 []aVertexList = itsViewPortCubeMesh.vertices;
		
		aVertexList[1] = itsDataModuleMinimap.itsViewport.itsCamera.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height/2,itsDataModuleMinimap.itsViewport.itsCamera.farClipPlane));
		aVertexList[2] = itsDataModuleMinimap.itsViewport.itsCamera.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height/2,itsDataModuleMinimap.itsViewport.itsCamera.nearClipPlane));
		aVertexList[3] = itsDataModuleMinimap.itsViewport.itsCamera.ScreenToWorldPoint(new Vector3(0,Screen.height/2,itsDataModuleMinimap.itsViewport.itsCamera.nearClipPlane));
		aVertexList[0] = itsDataModuleMinimap.itsViewport.itsCamera.ScreenToWorldPoint(new Vector3(0,Screen.height/2,itsDataModuleMinimap.itsViewport.itsCamera.farClipPlane));
		
		for (int i=0;i<4;i++)
		{
			aVertexList[i].y = itsFixedYViewPort;
		}
		
//		aVertexList[4] = itsViewPortCamera.ScreenToWorldPoint(new Vector3(0,0,itsViewPortCamera.farClipPlane));
//		aVertexList[5] = itsViewPortCamera.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,itsViewPortCamera.farClipPlane));
//		aVertexList[6] = itsViewPortCamera.ScreenToWorldPoint(new Vector3(0,Screen.height,itsViewPortCamera.farClipPlane));
//		aVertexList[7] = itsViewPortCamera.ScreenToWorldPoint(new Vector3(Screen.width,0,itsViewPortCamera.farClipPlane));
		
		itsViewPortCubeMesh.vertices = aVertexList;
		itsViewPortCubeMesh.RecalculateBounds();
		
		itsMaterialViewport.SetColor("_Color",itsDataModuleMinimap.itsViewport.itsColor);
	}
	#endregion
	
	#region bounds
	void InitBounds()
	{
		// search for bounding box
		foreach (Transform aChildTransform in transform)
		{
			if (aChildTransform.name == itsMeasureBoxName)
			{
				itsBoundingBox = aChildTransform.gameObject;
				break;
			}
		}
		if (Terrain.activeTerrain != null && itsBoundingBox == null)
			itsBoundingBox = Terrain.activeTerrain.gameObject;
		
		// search for terrain
		if (itsBoundingBox == null)
		{
			// could not find terrain
			LogError("Could not find terrain nor bounding box.",name,this);
			return;
		}
		
		// fit terrain into camera
		if (itsBoundingBox != null)
		{
			Bounds? aBoundsNullable = GetBoundsOfTerrain(itsBoundingBox);
			if (aBoundsNullable == null)
			{
				return;
			}
			itsTerrainBounds = aBoundsNullable.Value;
		}
	}
	
	Bounds? GetBoundsOfTerrain(GameObject theTerrain)
	{
		// measure terrain
		MeshRenderer aMeshrenderer = theTerrain.GetComponent<MeshRenderer>();
		if (aMeshrenderer != null)
		{
			return aMeshrenderer.bounds;
		}
		else
		{
			TerrainCollider aTerrain = theTerrain.GetComponent<TerrainCollider>();
			if (aTerrain != null)
			{
				return aTerrain.bounds;
			}
			else
			{
				// could not measure terrain
				LogError("Could not get measure bounds of terrain.",name,this);
				return null;
			}
		}
	}
	
	/// <summary>
	/// Create bounding box and scale it to contain all scene game objects, if terrain is found it is used
	/// </summary>
	void DoUpdateMeasure()
	{
		// remove old measure boxes
		Transform [] aChildTransformList = GetComponentsInChildren<Transform>();
		for (int i=0;i<aChildTransformList.Length;i++)
		{
			Transform aChildTransform = aChildTransformList[i];
			if (aChildTransform.name == itsMeasureBoxName)
			{
				GameObject.DestroyImmediate(aChildTransform.gameObject);
			}
		}
		
		// create bounding box if not already there
		if (itsBoundingBox == null)
		{
			itsBoundingBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
			itsBoundingBox.transform.parent = transform;
			itsBoundingBox.name = itsMeasureBoxName;
            //itsBoundingBox.collider.enabled = false;
            //itsBoundingBox.renderer.enabled = false;
        }
		
		Bounds ?aBounds = null;
		
		// try to find terrain
		if (Terrain.activeTerrain != null)
		{
			aBounds = GetBoundsOfTerrain(Terrain.activeTerrain.gameObject);
		}
		
		// no terrain -> measure all game objects
		Renderer []aListRenderers = UnityEngine.Object.FindObjectsOfType(typeof(Renderer)) as Renderer[];
		if (aListRenderers != null)
		{
			foreach (Renderer aRenderer in aListRenderers)
			{
				// do not add self
				if (aRenderer.gameObject != itsBoundingBox)
				{
					if (aBounds == null)
					{
						aBounds = aRenderer.bounds;
					}
					else
					{
						Bounds aLocalBounds = aBounds.Value;
						aLocalBounds.Encapsulate(aRenderer.bounds);
						aBounds = aLocalBounds;
					}
				}
			}
//			print("bounds:"+aBounds);
			if (aBounds.HasValue)
			{
				itsBoundingBox.transform.position = aBounds.Value.center;
				itsBoundingBox.transform.localScale = aBounds.Value.size;
			}
		}
	}
	#endregion
	
	#region auto photo
	void AutoCreatePhoto()
	{
		// check if autocreate mode is active.
		if (itsDataModuleMinimap.itsPhoto.itsTakePhoto)
		{
			// make photo of terrain
			GameObject aTempCameraGameObject = null;
			
			Camera aTempCamera = null;
			{
				// create temp camera
				aTempCameraGameObject = new GameObject("TempCamera");
				aTempCameraGameObject.transform.eulerAngles = new Vector3(90,0,0);
				aTempCamera = aTempCameraGameObject.AddComponent<Camera>();
				aTempCamera.enabled = false;
				aTempCamera.clearFlags = CameraClearFlags.SolidColor;
				aTempCamera.backgroundColor = Color.red;
				//aTempCamera.isOrthoGraphic = true;
				
				// calc height of camera or set orthographic size
			}
			
			// position camera over terrain
			aTempCameraGameObject.transform.position = itsTerrainBounds.center + new Vector3(0,itsTerrainBounds.extents.y+1,0);
			//aTempCameraGameObject.transform.position = itsTerrainBounds.center + new Vector3(0,100,0);
			aTempCamera.orthographicSize = Mathf.Max(itsTerrainBounds.extents.x,itsTerrainBounds.extents.z);
			aTempCamera.farClipPlane = 2.0f + itsTerrainBounds.size.y;
			itsSizeTerrain = new Vector2(itsTerrainBounds.size.x,itsTerrainBounds.size.z);
			
			// render to texture
			Texture2D aPlaneTexture = new Texture2D(Screen.width,Screen.height);
			{
				// create render texture
				//RenderTexture aRenderTexture = new RenderTexture(1024,768,16);
				//aRenderTexture.Create();
				
				// assign to camera
				aTempCamera.cullingMask = itsDataModuleMinimap.itsPhoto.itsPhotoLayers;
				aTempCamera.gameObject.active = false;
				//aTempCamera.targetTexture = aRenderTexture;
				aTempCamera.aspect = itsTerrainBounds.size.x / itsTerrainBounds.size.z;
				aTempCamera.backgroundColor = itsDataModuleMinimap.itsColorBackground;
				aTempCamera.clearFlags = CameraClearFlags.SolidColor;
				KGFPhotoCapture aPhotoCapture = aTempCameraGameObject.AddComponent<KGFPhotoCapture>();
				
				aPhotoCapture.SetTexture(aPlaneTexture);
				
				//			print("camera:"+aTempCamera.GetScreenWidth()+"/"+aTempCamera.GetScreenHeight());
				
				//			// wait for 1 frame
				aTempCamera.Render();
				
				// copy rendertexture to texture2d
				
				//			if (itsDataModuleMinimap.itsUseFogOfWar)
				//			{
				//				itsTextureFogWar = new Texture2D(aRenderTexture.width,aRenderTexture.height);
				//				Color []aColorArray = new Color[aRenderTexture.width*aRenderTexture.height];
				//				for (int i=0;i<aColorArray.Length;i++)
				//				{
				//					aColorArray[i] = new Color(0,0,0);
				//				}
				//				itsTextureFogWar.SetPixels(0,0,aRenderTexture.width,aRenderTexture.height,aColorArray);
				//				itsTextureFogWar.Apply();
				//			}
				//RenderTexture.active = aRenderTexture;
				//aPlaneTexture.ReadPixels(new Rect(0,0,1024,768),0,0);
				//			print("-- "+aRenderTexture.width+"/"+aRenderTexture.height);
				//aPlaneTexture.Apply();
//				RenderTexture.active = null;
//
//				// destroy render texture
//				aRenderTexture.Release();
			}
			
			// destroy temp camera
			GameObject.Destroy(aTempCameraGameObject);
			
			// show photo in plane under terrain
			{
				// create plane
				// create material with photo
				// assign material to plane
				GameObject aPhotoPlane = GeneratePhotoPlane(aPlaneTexture);
				
				// use minimap layer
				SetLayerRecursively(aPhotoPlane.gameObject,itsLayerMinimap);
				
				// rescale plane
				Vector3 aPhotoPlanePosition = itsTerrainBounds.center - itsTerrainBounds.extents;
				aPhotoPlanePosition.y = itsFixedYPhoto;
				aPhotoPlane.transform.position = aPhotoPlanePosition;
				aPhotoPlane.transform.localScale = new Vector3(itsTerrainBounds.size.x,1,itsTerrainBounds.size.z);//Vector3.one * Mathf.Max(aBounds.size.x,aBounds.size.z);
			}
		}
	}
	
	#endregion
	
	/// <summary>
	/// Set layer recursively
	/// </summary>
	/// <param name="theGameObject"></param>
	/// <param name="theLayer"></param>
	void SetLayerRecursively(GameObject theGameObject, int theLayer)
	{
		theGameObject.layer = theLayer;
		foreach(Transform aTransform in theGameObject.transform)
		{
			GameObject aGameObject = aTransform.gameObject;
			SetLayerRecursively(aGameObject,theLayer);
		}
	}
	
	/// <summary>
	/// creates the ortographic minimap camera and assigns all parameters
	/// </summary>
	void CreateCameras()
	{
		GameObject aCamera = new GameObject("minimapcamera");
		aCamera.transform.parent = transform;
		itsCamera = aCamera.AddComponent<Camera>();
		itsCamera.aspect = 1.0f;
		itsCamera.orthographic = true;
		itsCamera.clearFlags = CameraClearFlags.SolidColor;
        //�Լ������Ŀ��Ϊԭʼֵ
        itsCamera.cullingMask = 1 << itsLayerMinimap;
        //itsCamera.cullingMask = (1 << 0) + (1 << 16);
        //itsCamera.cullingMask = -1;
        itsCamera.backgroundColor = itsDataModuleMinimap.itsColorBackground;
		
		itsCameraTransform = itsCamera.transform;
		itsCameraTransform.position = new Vector3(0.0f,200.0f,0.0f);
		itsCameraTransform.rotation = Quaternion.Euler(90.0f,0.0f,0.0f);
		
		aCamera = new GameObject("outputcamera");
		itsCameraOutput = aCamera.AddComponent<Camera>();
		itsCameraOutput.transform.parent = itsCamera.transform;
		itsCameraOutput.transform.localPosition = Vector3.zero;
		itsCameraOutput.transform.localEulerAngles = new Vector3(0,180,0);
		itsCameraOutput.transform.localScale = Vector3.one;
		itsCameraOutput.orthographic = true;
		itsCameraOutput.clearFlags = CameraClearFlags.Depth;
		itsCameraOutput.depth = 1000;
		
		itsPixelPerMeter = itsDataModuleMinimap.itsZoom.itsZoomStartValue;
		UpdateOrthographicSize();
	}

	/// <summary>
	/// creates the rendertexture the minmap camera will render to
	/// </summary>
	void CreateRenderTexture()
	{
		itsRendertexture = new RenderTexture(2048,2048,16,RenderTextureFormat.ARGB32);
		if(itsRendertexture != null)
		{
			itsRendertexture.isPowerOfTwo = true;
			itsRendertexture.name = "minimap_rendertexture";
			itsRendertexture.Create();
			itsCamera.targetTexture = itsRendertexture;
		}
		else
		{
			LogError("cannot create rendertexture for minimap",name,this);
		}
	}
	
	/// <summary>
	/// For performance reasons this method should not be used.
	/// Include the RenderGUI method into your one and only OnGUI methos of
	/// your project and delete this OnGUI method.
	/// </summary>
	void OnGUI()	//only for debug purpose
	{
		RenderGUI();
	}
	
	void OnMapIconAdd(object theSender, EventArgs theArgs)
	{
		KGFAccessor.KGFAccessorEventargs anEventArgs = theArgs as KGFAccessor.KGFAccessorEventargs;
		if (anEventArgs != null)
		{
			KGFIMapIcon aMapIcon = anEventArgs.GetObject() as KGFIMapIcon;
			if (aMapIcon != null)
			{
				RegisterIcon(aMapIcon);
			}
		}
	}
	
	void OnMapIconRemove(object theSender, EventArgs theArgs)
	{
		KGFAccessor.KGFAccessorEventargs anEventArgs = theArgs as KGFAccessor.KGFAccessorEventargs;
		if (anEventArgs != null)
		{
			KGFIMapIcon aMapIcon = anEventArgs.GetObject() as KGFIMapIcon;
			if (aMapIcon != null)
			{
				UnregisterMapIcon(aMapIcon);
			}
		}
	}
	
	KGFMapIcon CreateIconInternal(Vector3 theWorldPoint,KGFMapIcon theIcon,Transform theParent)
	{
		GameObject aGameObject = (GameObject)GameObject.Instantiate(theIcon.gameObject);
		aGameObject.name = "Flag";
		aGameObject.transform.parent = itsContainerFlags;
		aGameObject.transform.position = theWorldPoint;
		return aGameObject.GetComponent<KGFMapIcon>();
	}
	
	/// <summary>
	/// generates the 2D Plane that will show the rendertexture of the minimap camera.
	/// This plane will be alighted in the correspronding corner in the viewport of the Camera.main
	/// </summary>
	/// <returns></returns>
	private GameObject GenerateMinimapPlane()
	{
		GameObject aMiniMapPlane = new GameObject("output_plane");
		aMiniMapPlane.transform.parent = transform;
		itsMinimapMeshFilter = aMiniMapPlane.AddComponent<MeshFilter>();
		itsMinimapMeshFilter.mesh = GeneratePlaneMeshXZ();
		
		itsMeshRendererMinimapPlane = aMiniMapPlane.gameObject.AddComponent<MeshRenderer>();
		itsMeshRendererMinimapPlane.material = new Material(itsDataModuleMinimap.itsShaders.itsShaderMapMask);
		itsMeshRendererMinimapPlane.material.SetTexture("_Mask",itsDataModuleMinimap.itsAppearanceMiniMap.itsMask);
		
		itsMaterialMaskedMinimap = itsMeshRendererMinimapPlane.material;
		return aMiniMapPlane;
	}
	
	public void SetMask(Texture2D theMinimapMask, Texture2D theMapMask)
	{
		if(itsMeshRendererMinimapPlane.material == null)
			return;
		
		itsDataModuleMinimap.itsAppearanceMiniMap.itsMask = theMinimapMask;
		itsDataModuleMinimap.itsAppearanceMap.itsMask = theMapMask;
		
		UpdateMaskTexture();
	}

	/// <summary>
	/// Generate a new plane mesh on a new gameobject for a map icon
	/// </summary>
	/// <param name="theTexture"></param>
	/// <returns></returns>
	public static GameObject GenerateTexturePlane(Texture2D theTexture, Shader theShader)
	{
		GameObject aGameObject = new GameObject("MapIconPlane");
		MeshFilter aMiniMapPlane = aGameObject.AddComponent<MeshFilter>();
		aMiniMapPlane.transform.eulerAngles = new Vector3(0,0,0);
		aMiniMapPlane.mesh = GeneratePlaneMeshXZCentered();
		
		MeshRenderer aMeshRenderer = aMiniMapPlane.gameObject.AddComponent<MeshRenderer>();
		aMeshRenderer.material = new Material(theShader);
		aMeshRenderer.material.mainTexture = theTexture;
		aMeshRenderer.castShadows = false;
		aMeshRenderer.receiveShadows = false;
		
		return aGameObject;
	}

	/// <summary>
	/// Generate a new photo plane
	/// </summary>
	/// <returns></returns>
	private GameObject GeneratePhotoPlane(Texture2D theTexture)
	{
//		GameObject aMiniMapPlaneRoot = new GameObject("minimap_photo_plane");
//		aMiniMapPlaneRoot.transform.parent = transform;
//		aMiniMapPlaneRoot.transform.localPosition = Vector3.zero;
//		aMiniMapPlaneRoot.transform.localRotation = Quaternion.identity;
//		aMiniMapPlaneRoot.transform.localScale = Vector3.one;
		
		GameObject aGameObject = new GameObject("photo_plane");
		aGameObject.transform.parent = transform;
		MeshFilter aMiniMapPlane = aGameObject.AddComponent<MeshFilter>();
		aMiniMapPlane.transform.eulerAngles = Vector3.zero;
		aMiniMapPlane.transform.position = Vector3.zero;
		aMiniMapPlane.mesh = GeneratePlaneMeshXZ();
		
		MeshRenderer aMeshRenderer = aMiniMapPlane.gameObject.AddComponent<MeshRenderer>();
		aMeshRenderer.material = new Material(itsDataModuleMinimap.itsShaders.itsShaderPhotoPlane);
		aMeshRenderer.material.mainTexture = theTexture;
		itsMaterialPhoto = aMeshRenderer.material;
		return aGameObject;
	}
	
	void UpdateMinimapOutputPlane()
	{
		Camera aCameraThatFilmsPlane = itsCameraOutput;//Camera.main;
		
		if(aCameraThatFilmsPlane != null && itsMinimapPlane != null)
		{
			if(itsMinimapMeshFilter == null) return;
			
			Mesh aMiniMapMesh = itsMinimapMeshFilter.mesh;
			if(aMiniMapMesh == null) return;
			
			Rect aRect = itsTargetRect;
			aRect.y = Screen.height - aRect.y;
			
			
			
			Vector3[] aVertices = aMiniMapMesh.vertices;
			aVertices[0] = aCameraThatFilmsPlane.ScreenToWorldPoint(new Vector3(aRect.x,aRect.y - aRect.height,aCameraThatFilmsPlane.nearClipPlane+0.01f));
			aVertices[1] = aCameraThatFilmsPlane.ScreenToWorldPoint(new Vector3(aRect.x + aRect.width,aRect.y - aRect.height,aCameraThatFilmsPlane.nearClipPlane+0.01f));
			aVertices[2] = aCameraThatFilmsPlane.ScreenToWorldPoint(new Vector3(aRect.x + aRect.width,aRect.y,aCameraThatFilmsPlane.nearClipPlane+0.01f));
			aVertices[3] = aCameraThatFilmsPlane.ScreenToWorldPoint(new Vector3(aRect.x,aRect.y,aCameraThatFilmsPlane.nearClipPlane+0.01f));
			
			aMiniMapMesh.vertices = aVertices;
			aMiniMapMesh.RecalculateBounds();
			
			Vector3 aNormal = aCameraThatFilmsPlane.transform.forward;
			
			Vector3[] aNormals = aMiniMapMesh.normals;
			aNormals[0] = aNormal;
			aNormals[1] = aNormal;
			aNormals[2] = aNormal;
			aNormals[3] = aNormal;
			aMiniMapMesh.normals = aNormals;
		}
	}
	
	/// <summary>
	/// Clean click icons list from destroyed game objects
	/// </summary>
	void CleanClickIconsList()
	{
		for (int i=itsListClickIcons.Count-1;i>=0;i--)
		{
			if (itsListClickIcons[i] == null)
			{
				itsListClickIcons.RemoveAt(i);
				continue;
			}
			if (itsListClickIcons[i] is MonoBehaviour)
			{
				if (((MonoBehaviour)itsListClickIcons[i]) == null)
				{
					itsListClickIcons.RemoveAt(i);
					continue;
				}
			}
		}
	}
	
	/// <summary>
	/// set foreground or background rendering of icons (if they should disappear behind fog of war or not)
	/// </summary>
	/// <param name="anIcon"></param>
	void UpdateIconLayer(KGFIMapIcon theMapIcon)
	{
		GameObject aSpatialNewMapIcon = theMapIcon.GetRepresentation();
		MeshRenderer aRepRenderer = aSpatialNewMapIcon.GetComponent<MeshRenderer>();
		CleanClickIconsList();
		if (itsDataModuleMinimap.itsFogOfWar.itsHideMapIcons && !itsListClickIcons.Contains(theMapIcon))
		{
			aRepRenderer.material.renderQueue = 3000;
		}else
		{
			aRepRenderer.material.renderQueue = 3200;
		}
	}
	
	/// <summary>
	/// Register map icon
	/// </summary>
	/// <param name="theMapIcon"></param>
	void RegisterIcon(KGFIMapIcon theMapIcon)
	{
		// create copy of static representation
		GameObject aSpatialArrow = null;
		// create copy of representation
		GameObject aSpatialNewMapIcon = null;
		
		aSpatialNewMapIcon = theMapIcon.GetRepresentation();
		if(aSpatialNewMapIcon == null)
		{
			LogError("missing icon representation for: "+theMapIcon.GetGameObjectName(),name,this);
			return;
		}
		
		UpdateIconLayer(theMapIcon);
		
		if (theMapIcon.GetTextureArrow() != null)
		{
			aSpatialArrow = GenerateTexturePlane(theMapIcon.GetTextureArrow(),itsDataModuleMinimap.itsShaders.itsShaderMapIcon);
			aSpatialArrow.transform.parent = itsContainerIconArrows;
			aSpatialArrow.transform.localPosition = Vector3.zero;
			aSpatialArrow.transform.localScale = Vector3.one;
			aSpatialArrow.GetComponent<MeshRenderer>().material.renderQueue = 3200;
			SetLayerRecursively(aSpatialArrow.gameObject,itsLayerMinimap);
		}
		
		// reparent it
		aSpatialNewMapIcon.transform.parent = itsContainerIcons;
		aSpatialNewMapIcon.transform.position = Vector3.zero;
		SetLayerRecursively(aSpatialNewMapIcon.gameObject,itsLayerMinimap);

		// remember it
		mapicon_listitem_script aNewItem = new mapicon_listitem_script();
		aNewItem.itsModule = this;
		aNewItem.itsMapIcon = theMapIcon;
		aNewItem.itsRepresentationInstance = aSpatialNewMapIcon;
		aNewItem.itsRepresentationInstanceTransform = aSpatialNewMapIcon.transform;
		aNewItem.itsRotate = theMapIcon.GetRotate();
		
		aNewItem.itsRepresentationArrowInstance = aSpatialArrow;
		aNewItem.itsMapIconTransform = theMapIcon.GetTransform();
		aNewItem.SetVisibility(true);
		if (aSpatialArrow != null)
			aNewItem.itsRepresentationArrowInstanceTransform = aSpatialArrow.transform;
		itsListMapIcons.Add(aNewItem);
		
		aNewItem.UpdateIcon();
		UpdateIconScale();
		
		LogInfo(string.Format("Added icon of category '{0}' for '{1}'",theMapIcon.GetCategory(),theMapIcon.GetTransform().name),name,this);
	}
	
	/// <summary>
	/// Unregister a map icon
	/// </summary>
	/// <param name="theMapIcon"></param>
	void UnregisterMapIcon(KGFIMapIcon theMapIcon)
	{
		for (int i=0;i<itsListMapIcons.Count;i++)
		{
			mapicon_listitem_script anItem = itsListMapIcons[i];
			if (anItem.itsMapIcon == theMapIcon)
			{
				LogInfo("Removed map icon of "+anItem.itsMapIconTransform.gameObject.GetObjectPath(),name,this);
				
				anItem.Destroy();
				itsListMapIcons.RemoveAt(i);
				break;
			}
		}
	}
	
	/// <summary>
	/// Update scaling of each map icon that is equally sized on every zoom level
	/// </summary>
	void UpdateIconScale()
	{
		float aScaleIcons = GetScaleIcons();
		float aScaleArrows = GetScaleArrows();
		
		foreach (mapicon_listitem_script aListItem in itsListMapIcons)
		{
			if (aListItem.itsRepresentationInstanceTransform != null)
			{
				aListItem.itsRepresentationInstanceTransform.localScale = Vector3.one*aScaleIcons;
			}
			if (aListItem.itsRepresentationArrowInstanceTransform != null)
			{
				aListItem.itsRepresentationArrowInstanceTransform.localScale = Vector3.one*aScaleArrows;
			}
		}
	}
	
	/// <summary>
	/// Returns the scale factor for the arrows
	/// </summary>
	/// <returns></returns>
	float GetScaleArrows()
	{
		if (GetFullscreen())
		{
			return 0;
		}
		
		return GetCurrentRange() * itsDataModuleMinimap.itsAppearanceMiniMap.itsScaleArrows * 2;
	}
	
	/// <summary>
	/// Returns the scale factor for the icons
	/// </summary>
	/// <returns></returns>
	float GetScaleIcons()
	{
		if (GetFullscreen())
		{
			return GetCurrentRange() * itsDataModuleMinimap.itsAppearanceMap.itsScaleIcons * 2 * (itsSavedResolution.Value.x / (float)GetWidth());
		}
		else
		{
			return GetCurrentRange() * itsDataModuleMinimap.itsAppearanceMiniMap.itsScaleIcons * 2;
		}
	}
	
	Texture2D itsTextureRenderMaskCurrent = null;
	void UpdateMaskTexture()
	{
		if (GetFullscreen())
		{
			itsTextureRenderMaskCurrent = itsDataModuleMinimap.itsAppearanceMap.itsMask;
		}else
		{
			itsTextureRenderMaskCurrent = itsDataModuleMinimap.itsAppearanceMiniMap.itsMask;
		}
		itsMeshRendererMinimapPlane.material.SetTexture("_Mask",itsTextureRenderMaskCurrent);
		
		if (GetHasProVersion()) //  && itsTextureRenderMaskCurrent != null
		{
			itsCameraOutput.enabled = true;
			itsCamera.targetTexture = itsRendertexture;
			itsCamera.rect = new Rect(0,0,1,1);
		}else
		{
			itsCameraOutput.enabled = false;
			itsCamera.targetTexture = null;
			itsCamera.pixelRect = new Rect(itsTargetRect.x,Screen.height - itsTargetRect.y - itsTargetRect.height,itsTargetRect.width,itsTargetRect.height);
		}
	}
	
	void Update()
	{
		UnityEngine.Profiling.Profiler.BeginSample("UpdateZoomCorrectionScale()");
		#if OnlineChangeMode
		UpdateIconScale();
		UpdateOrthographicSize();
		#endif
		UnityEngine.Profiling.Profiler.EndSample();
		
		if (itsTargetTransform == null)
		{
			enabled = false;
			return;
		}
		
		UpdateMaskTexture();
		
		UnityEngine.Profiling.Profiler.BeginSample("UpdateFogOfWar()");
		if (itsDataModuleMinimap.itsFogOfWar.itsActive && itsDataModuleMinimap.itsShaders.itsShaderFogOfWar != null)
		{
			UpdateFogOfWar();
		}
		UnityEngine.Profiling.Profiler.EndSample();
		
		UnityEngine.Profiling.Profiler.BeginSample("CheckForClicksOnMinimap()");
		CheckForClicksOnMinimap();
		UnityEngine.Profiling.Profiler.EndSample();
		
		UnityEngine.Profiling.Profiler.BeginSample("UpdateMapIconRotation()");
		UpdateMapIconRotation();
		UnityEngine.Profiling.Profiler.EndSample();
		
		UnityEngine.Profiling.Profiler.BeginSample("UpdateViewPortCube()");
		UpdateViewPortCube();
		UnityEngine.Profiling.Profiler.EndSample();
		
		itsMaterialPhoto.SetColor("_Color",itsDataModuleMinimap.itsColorMap);
		itsMaterialMaskedMinimap.SetColor("_Color",itsDataModuleMinimap.itsColorAll);
	}
	
	void LateUpdate()
	{
		if (GetHasProVersion()) //  && itsTextureRenderMaskCurrent != null
		{
			UpdateMinimapOutputPlane();
		}
	}
	
//	Mesh CreateMeshCube()
//	{
//		Mesh aMesh = new Mesh();
//
//		// vertices
//		aMesh.vertices = new Vector3[8];
//		aMesh.vertices[0] = new Vector3(0,1,0);
//		aMesh.vertices[1] = new Vector3(0,1,1);
//		aMesh.vertices[2] = new Vector3(1,1,1);
//		aMesh.vertices[3] = new Vector3(1,1,0);
//
//		aMesh.vertices[4] = new Vector3(0,0,0);
//		aMesh.vertices[5] = new Vector3(0,0,1);
//		aMesh.vertices[6] = new Vector3(1,0,1);
//		aMesh.vertices[7] = new Vector3(1,0,0);
//
//		// tris
//		aMesh.triangles = new int[12 * 3];
//		// tris: side 1
//		aMesh.triangles[0] = 0;
//		aMesh.triangles[1] = 1;
//		aMesh.triangles[2] = 3;
//
//		aMesh.triangles[3] = 1;
//		aMesh.triangles[4] = 2;
//		aMesh.triangles[5] = 3;
//
//		// tris: side 2
//		aMesh.triangles[6] = 0;
//		aMesh.triangles[7] = 3;
//		aMesh.triangles[8] = 7;
//
//		aMesh.triangles[9] = 0;
//		aMesh.triangles[10] = 7;
//		aMesh.triangles[11] = 4;
//
//		// tris: side 3
//		aMesh.triangles[12] = 3;
//		aMesh.triangles[13] = 6;
//		aMesh.triangles[14] = 7;
//
//		aMesh.triangles[15] = 3;
//		aMesh.triangles[16] = 2;
//		aMesh.triangles[17] = 6;
//
//		// tris: side 4
//		aMesh.triangles[18] = 5;
//		aMesh.triangles[19] = 4;
//		aMesh.triangles[20] = 7;
//
//		aMesh.triangles[21] = 6;
//		aMesh.triangles[22] = 5;
//		aMesh.triangles[23] = 7;
//
//		// tris: side 5
//		aMesh.triangles[24] = 2;
//		aMesh.triangles[25] = 5;
//		aMesh.triangles[26] = 6;
//
//		aMesh.triangles[27] = 2;
//		aMesh.triangles[28] = 1;
//		aMesh.triangles[29] = 5;
//
//		// tris: side 6
//		aMesh.triangles[30] = 1;
//		aMesh.triangles[31] = 0;
//		aMesh.triangles[32] = 4;
//
//		aMesh.triangles[33] = 1;
//		aMesh.triangles[34] = 4;
//		aMesh.triangles[35] = 5;
//
//		return aMesh;
//	}
	
	void CheckForClicksOnMinimap()
	{
		if (!itsDataModuleMinimap.itsUserFlags.itsActive)
		{
			return;
		}
		
		// check for click
		if (Input.GetMouseButtonUp(0))
		{
			Vector2 aMousePosition = Input.mousePosition;
			aMousePosition.y = Screen.height - aMousePosition.y;
			
			// check if mouse click was on minimap
			if (itsTargetRect.Contains(aMousePosition))
			{
				if(itsRectFullscreen.Contains(aMousePosition) || itsRectStatic.Contains(aMousePosition) || itsRectZoomIn.Contains(aMousePosition) || itsRectZoomOut.Contains(aMousePosition))
				{
					return;
				}
				
				// calculate point of click on minimap
				Vector2 aPercentofImageClick = new Vector2((aMousePosition.x - itsTargetRect.x) / (itsTargetRect.width),
				                                           (aMousePosition.y - itsTargetRect.y) / (itsTargetRect.height));
				Vector2 aVirtual2DCoordinateOfClick = aPercentofImageClick - new Vector2(0.5f,0.5f);
				
				Vector3 aClickWorldPoint =
					itsCameraTransform.position +
					itsCameraTransform.up * aVirtual2DCoordinateOfClick.y * itsCamera.orthographicSize * (-2) +
					itsCameraTransform.right * aVirtual2DCoordinateOfClick.x * itsCamera.orthographicSize * 2 * itsCamera.aspect;
				aClickWorldPoint.y = itsFixedYFlags;
				
				// check if there is already a user flag at this position
				bool aFoundUserFlag = false;
                //foreach (Transform aUserFlagTransform in itsContainerFlags)
                //{
                //    if (Vector3.Distance(aUserFlagTransform.position, aClickWorldPoint) <= itsDataModuleMinimap.itsUserFlags.itsRemoveClickDistance)
                //    {
                //        LogInfo(string.Format("Removed user flag at {0}", aUserFlagTransform.position), name, this);
                //        GameObject.Destroy(aUserFlagTransform.gameObject);
                //        aFoundUserFlag = true;
                //    }
                //}

                //if (!aFoundUserFlag)
                //{
                //    create a flag object at this point

                //    EventUserFlagSet.Trigger(this, new KGFFlagEventArgs(aClickWorldPoint));
                //    LogInfo(string.Format("Added user flag at {0}", aClickWorldPoint), name, this);
                //    KGFMapIcon anIcon = CreateIconInternal(aClickWorldPoint, itsDataModuleMinimap.itsUserFlags.itsMapIcon, itsContainerFlags);
                //    itsListClickIcons.Add(anIcon);
                //    UpdateIconLayer(anIcon);
                //}

                SetFullscreen(!GetFullscreen());
            }
		}
	}
	
	void UpdateMapIconRotation()
	{
		itsCameraTransform.position = itsTargetTransform.position;
		itsCameraTransform.Translate(0.0f,0.0f,-200.0f);
		
		if (itsDataModuleMinimap.itsIsStatic)
		{
			itsCameraTransform.eulerAngles = new Vector3(0,itsDataModuleMinimap.itsStaticNorth,0);//Quaternion.LookRotation(itsDataModuleMinimap.itsGameObjectNorth.transform.forward,Vector3.up);
			itsCameraTransform.Rotate(90.0f,0.0f,0.0f);
		}
		else
		{
			Vector3 aForwardVector = itsTargetTransform.forward;
			aForwardVector.y = 0.0f;
			aForwardVector.Normalize();
			itsCameraTransform.rotation = Quaternion.LookRotation(aForwardVector,Vector3.up);
			itsCameraTransform.Rotate(90.0f,0.0f,0.0f);
		}
		
		// for every map icon
		for (int i=itsListMapIcons.Count-1;i>=0;i--)
		{
			mapicon_listitem_script aListItem = itsListMapIcons[i];
			
			// remove map icons of destroyed gameobjects
			if (aListItem.itsMapIconTransform == null)
			{
				itsListMapIcons.RemoveAt(i);
				continue;
			}
			
			// MAP ICON
			if (aListItem.GetMapIconVisibilityEffective())
			{
				// rotation
				Vector3 aRotation = aListItem.itsRepresentationInstanceTransform.eulerAngles;
				aRotation.x = 0;
				aRotation.z = 0;
				
				{
					if (aListItem.itsRotate)
					{
						aRotation.y = aListItem.itsMapIconTransform.eulerAngles.y;
					}
					else
					{
						aRotation.y = itsCameraTransform.eulerAngles.y;
					}
				}
				
				aListItem.itsRepresentationInstanceTransform.eulerAngles = aRotation;
				
				// position
				Vector3 anOriginalPosition = aListItem.itsMapIconTransform.position;
				
				aListItem.itsRepresentationInstanceTransform.position = new Vector3(anOriginalPosition.x,itsFixedYIcons,anOriginalPosition.z);
				
				// ARROW
				if (aListItem.itsRepresentationArrowInstance != null)
				{
					// calc new visibility: visible if map icon is in visible state and outside radius
					Vector3 aDistanceLine = itsTargetTransform.position - aListItem.itsRepresentationInstanceTransform.position;
					aDistanceLine.y = 0;
					
					bool aNewVisibilityArrow = aDistanceLine.magnitude > GetCurrentRange();
					if (aNewVisibilityArrow != aListItem.GetIsArrowVisible())
					{
						aListItem.ShowArrow(aNewVisibilityArrow);
						if (aNewVisibilityArrow)
						{
							LogInfo(string.Format("Icon '{0}' got invisible",aListItem.itsMapIconTransform.name),name,this);
						}else
						{
							LogInfo(string.Format("Icon '{0}' got visible",aListItem.itsMapIconTransform.name),name,this);
						}
						EventVisibilityOnMinimapChanged.Trigger(this,new KGFMapIconEventArgs());
					}
					
					if (aListItem.GetIsArrowVisible())
					{
						float anAngle = Vector3.Angle(Vector3.forward,aDistanceLine);
						if (Vector3.Dot(Vector3.right,aDistanceLine) < 0)
							anAngle = 360 - anAngle;
						anAngle += 180;
						
						// position
//						Vector3 aDistanceLineBorder = aDistanceLine.normalized * (GetCurrentRange()) * itsDataModuleMinimap.itsAppearanceMiniMap.itsRadiusArrows;// * (1 + aMultiplicator);
						Vector3 aVector;// = itsTargetTransform.position - aDistanceLineBorder;
						
						float aRadius = GetCurrentRange() * itsDataModuleMinimap.itsAppearanceMiniMap.itsRadiusArrows;
						float aCorrectedAngle = anAngle - itsCameraTransform.localEulerAngles.y;
						aVector = itsTargetTransform.position +
							itsCameraTransform.right * aRadius * itsCamera.aspect * Mathf.Sin(aCorrectedAngle * Mathf.Deg2Rad) +
							itsCameraTransform.up * aRadius * Mathf.Cos(aCorrectedAngle * Mathf.Deg2Rad);
						
						aVector.y = itsFixedYArrows;
						aListItem.itsRepresentationArrowInstanceTransform.position = aVector;
						
						// rotation
						aListItem.itsRepresentationArrowInstanceTransform.eulerAngles = new Vector3(0,anAngle,0);
					}
				}
			}
		}
	}
	#endregion
	
	#region Public methods
	/// <summary>
	/// Update internally created styles with icon textures
	/// </summary>
	public void UpdateStyles()
	{
		itsGuiStyleBack = new GUIStyle();
		itsGuiStyleBack.normal.background = itsDataModuleMinimap.itsAppearanceMiniMap.itsBackground;
		
		itsGuiStyleButton = new GUIStyle();
		itsGuiStyleButton.normal.background = itsDataModuleMinimap.itsAppearanceMiniMap.itsButton;
		itsGuiStyleButton.hover.background = itsDataModuleMinimap.itsAppearanceMiniMap.itsButtonHover;
		itsGuiStyleButton.active.background = itsDataModuleMinimap.itsAppearanceMiniMap.itsButtonDown;
		
		itsGuiStyleButtonFullscreen = new GUIStyle();
		itsGuiStyleButtonFullscreen.normal.background = itsDataModuleMinimap.itsAppearanceMap.itsButton;
		itsGuiStyleButtonFullscreen.hover.background = itsDataModuleMinimap.itsAppearanceMap.itsButtonHover;
		itsGuiStyleButtonFullscreen.active.background = itsDataModuleMinimap.itsAppearanceMap.itsButtonDown;
	}
	
	/// <summary>
	/// Update the map icon (color, texture)
	/// </summary>
	/// <param name="theIcon"></param>
	public void UpdateIcon(KGFIMapIcon theIcon)
	{
		foreach (mapicon_listitem_script anItem in itsListMapIcons)
		{
			if (anItem.itsMapIcon == theIcon)
			{
				anItem.UpdateIcon();
			}
		}
	}
	
	/// <summary>
	/// Serialize current minimap state to string
	/// Currently only fog of war is supported
	/// </summary>
	/// <returns></returns>
	public string GetSaveString()
	{
		return SerializeFogOfWar();
	}
	
	/// <summary>
	/// Load minimap state from string.
	/// Currently only fog of war is supported
	/// </summary>
	/// <param name="theSavedString">A string you can get from GetSaveString()</param>
	public void LoadFromString(string theSavedString)
	{
		DeserializeFogOfWar(theSavedString);
	}
	
	/// <summary>
	/// Create a flag map icon at a given world point
	/// </summary>
	/// <param name="theWorldPoint">The point in world coordinates where the new icon should be placed</param>
	/// <param name="theCategory">The category of the icon</param>
	/// <returns>The newly created icon</returns>
	public KGFMapIcon CreateIcon(Vector3 theWorldPoint,KGFMapIcon theMapIcon)
	{
		KGFMapIcon aNewIcon = CreateIconInternal(theWorldPoint,theMapIcon,itsContainerUser);
		itsListUserIcons.Add(aNewIcon);
		return aNewIcon;
	}
	
	/// <summary>
	/// Remove a user created map icon
	/// </summary>
	/// <param name="theIcon">The user created map icon to remove</param>
	public void RemoveIcon(KGFMapIcon theIcon)
	{
		if (itsListUserIcons.Contains(theIcon))
		{
			UnregisterMapIcon(theIcon);
			itsListUserIcons.Remove(theIcon);
		}
		else
		{
			LogError("Not a user created icon",name,this);
		}
	}
	
	/// <summary>
	/// Get all user icons created with CreateIcon
	/// </summary>
	/// <returns>A list of all user created icons</returns>
	public KGFMapIcon[] GetUserIcons()
	{
		return itsListUserIcons.ToArray();
	}
	
	/// <summary>
	/// Get all user icons created by click
	/// </summary>
	/// <returns>A list of all user created icons</returns>
	public KGFMapIcon[] GetUserFlags()
	{
		List<KGFMapIcon> aList = new List<KGFMapIcon>();
		
		foreach (Transform aChild in itsContainerFlags.transform)
		{
			aList.Add(aChild.GetComponent<KGFMapIcon>());
		}
		
		return aList.ToArray();
	}
	
	/// <summary>
	/// Enable/disable minimap
	/// </summary>
	/// <param name="theEnable"></param>
	public void SetMinimapEnabled(bool theEnable)
	{
		if (itsMinimapActive != theEnable)
		{
			itsMinimapActive = theEnable;
			gameObject.active = theEnable;
			itsMinimapPlane.active = theEnable;
			
			LogInfo("New map system state:"+theEnable,name,this);
		}
	}
	
	/// <summary>
	/// Refresh map icon visibility of all map icons, call this if your IMapIcon derived class instances changed visibility
	/// </summary>
	public void RefreshIconsVisibility()
	{
		foreach (mapicon_listitem_script aListItem in itsListMapIcons)
		{
			aListItem.UpdateVisibility();
		}
	}
	
	/// <summary>
	/// Set visibility of map icons by category
	/// </summary>
	/// <param name="theCategory"></param>
	public void SetIconsVisibleByCategory(string theCategory, bool theVisible)
	{
		LogInfo(string.Format("Icon category '{0}' changed visibility to: {1}",theCategory,theVisible),name,this);
		foreach (mapicon_listitem_script anitem in itsListMapIcons)
		{
			if (anitem.itsMapIcon.GetCategory() == theCategory)
			{
				anitem.SetVisibility(theVisible);
			}
		}
	}
	
	/// <summary>
	/// Set target that is centered on minimap
	/// </summary>
	/// <param name="theTarget"></param>
	public void SetTarget(GameObject theTarget)
	{
		if(theTarget == null)
		{
			LogError("Assign your character to KGFMapsystem.itsTarget. KGFMapSystem will not work without a target.",name,this);
			return;
		}
		itsDataModuleMinimap.itsTarget = theTarget;
		itsTargetTransform = theTarget.transform;
	}
	
	/// <summary>
	/// Returns current absolute zoom level
	/// </summary>
	/// <returns></returns>
	public float GetCurrentRange()
	{
		return itsCamera.orthographicSize;
	}
	
	/// <summary>
	/// Enable or disable static mode
	/// </summary>
	/// <param name="theModeStatic"></param>
	public void SetModeStatic(bool theModeStatic)
	{
		itsDataModuleMinimap.itsIsStatic = theModeStatic;
	}
	
	/// <summary>
	/// TRUE, if static mode is active
	/// </summary>
	/// <returns></returns>
	public bool GetModeStatic()
	{
		return itsDataModuleMinimap.itsIsStatic;
	}
	
	/// <summary>
	/// Set the size of the minimap on the screen
	/// </summary>
	/// <param name="theSize"></param>
	public void SetMinimapSize(int theSize)
	{
		itsDataModuleMinimap.itsAppearanceMiniMap.itsSize = theSize;
		UpdateOrthographicSize();
	}
	
	/// <summary>
	/// Get active status of fullscreen mode
	/// </summary>
	/// <returns></returns>
	public bool GetFullscreen()
	{
		return itsModeFullscreen;
	}
	
	/// <summary>
	/// Set the fullscreen mode
	/// </summary>
	/// <param name="theFullscreenMode"></param>
	public void SetFullscreen(bool theFullscreenMode)
	{
		if (theFullscreenMode && itsSavedResolution == null)
		{
			itsSavedResolution = new Vector2(GetWidth(),GetHeight());
		}
		else if (!theFullscreenMode && itsSavedResolution != null)
		{
			itsSavedResolution = null;
		}
		else
		{
			return;
		}
		
		itsModeFullscreen = theFullscreenMode;
		UpdateTargetRect();
		UpdateMaskTexture();
		UpdateOrthographicSize();
		UpdateIconScale();
	}
	
	/// <summary>
	/// Get gui style for buttons
	/// </summary>
	/// <returns></returns>
	GUIStyle GetButtonStyle()
	{
		if (GetFullscreen())
		{
			return itsGuiStyleButtonFullscreen;
		}
		else
		{
			return itsGuiStyleButton;
		}
	}
	
	/// <summary>
	/// Simle draw button method
	/// </summary>
	/// <param name="theTexture"></param>
	/// <returns></returns>
	bool DrawButton(Rect theRect, Texture2D theTexture)
	{
		if (theTexture == null)
			return false;
		
		return GUI.Button(theRect,theTexture,GetButtonStyle());
	}
	
	/// <summary>
	/// Update itsTargetRect with new values
	/// </summary>
	void UpdateTargetRect()
	{
		switch (itsDataModuleMinimap.itsAppearanceMiniMap.itsAlignmentHorizontal)
		{
			case KGFAlignmentHorizontal.Left:
				itsTargetRect.x = 0;
				break;
			case KGFAlignmentHorizontal.Middle:
				itsTargetRect.x = (Screen.width-GetWidth())/2;
				break;
			case KGFAlignmentHorizontal.Right:
				itsTargetRect.x = Screen.width-GetWidth();
				break;
		}
		switch (itsDataModuleMinimap.itsAppearanceMiniMap.itsAlignmentVertical)
		{
			case KGFAlignmentVertical.Top:
				itsTargetRect.y = 0;
				break;
			case KGFAlignmentVertical.Middle:
				itsTargetRect.y = (Screen.height-GetHeight())/2;
				break;
			case KGFAlignmentVertical.Bottom:
				itsTargetRect.y = Screen.height-GetHeight();
				break;
		}
		
		itsTargetRect.width = GetWidth();
		itsTargetRect.height = GetHeight();
	}
	
	/// <summary>
	/// renders the gui of the minimap
	/// </summary>
	public void RenderGUI()
	{
		if (!itsMinimapActive)
			return;
		
		UpdateTargetRect();
		
		float aButtonSize = GetButtonSize();
		float aButtonPadding = GetButtonPadding();
		
		// background
		if (GetFullscreen())
		{
			itsGuiStyleBack.normal.background = itsDataModuleMinimap.itsAppearanceMap.itsBackground;
			itsGuiStyleBack.border = new RectOffset(itsDataModuleMinimap.itsAppearanceMap.itsBackgroundBorder,
			                                        itsDataModuleMinimap.itsAppearanceMap.itsBackgroundBorder,
			                                        itsDataModuleMinimap.itsAppearanceMap.itsBackgroundBorder,
			                                        itsDataModuleMinimap.itsAppearanceMap.itsBackgroundBorder);
		}
		else
		{
			itsGuiStyleBack.normal.background = itsDataModuleMinimap.itsAppearanceMiniMap.itsBackground;
			itsGuiStyleBack.border = new RectOffset(itsDataModuleMinimap.itsAppearanceMiniMap.itsBackgroundBorder,
			                                        itsDataModuleMinimap.itsAppearanceMiniMap.itsBackgroundBorder,
			                                        itsDataModuleMinimap.itsAppearanceMiniMap.itsBackgroundBorder,
			                                        itsDataModuleMinimap.itsAppearanceMiniMap.itsBackgroundBorder);
		}
		GUI.Box(itsTargetRect,"",itsGuiStyleBack);
		
		// calc button rects
		if (GetFullscreen())
		{
			int aSpace = (int)(itsDataModuleMinimap.itsAppearanceMap.itsButtonSpace * GetWidth());
			int aButtonCount = 4;
			int aButtonsLongSide = (int)(((aButtonCount-1)*aSpace)+GetButtonSize()*aButtonCount);
			int aButtonsShortSide = (int)(GetButtonSize());
			
			Rect aRect = new Rect();
			
			switch(itsDataModuleMinimap.itsAppearanceMap.itsAlignmentHorizontal)
			{
				case KGFAlignmentHorizontal.Left:
					aRect.x = itsTargetRect.x + aButtonPadding;
					break;
				case KGFAlignmentHorizontal.Middle:
					if (itsDataModuleMinimap.itsAppearanceMap.itsOrientation == KGFOrientation.Horizontal)
						aRect.x = (itsTargetRect.xMax-itsTargetRect.xMin)/2-aButtonsLongSide/2;
					else
						aRect.x = (itsTargetRect.xMax-itsTargetRect.xMin)/2-aButtonsShortSide/2;
					break;
				case KGFAlignmentHorizontal.Right:
					if (itsDataModuleMinimap.itsAppearanceMap.itsOrientation == KGFOrientation.Horizontal)
						aRect.x = itsTargetRect.xMax-aButtonsLongSide-aButtonPadding;
					else
						aRect.x = itsTargetRect.xMax-aButtonsShortSide-aButtonPadding;
					break;
			}
			
			switch(itsDataModuleMinimap.itsAppearanceMap.itsAlignmentVertical)
			{
				case KGFAlignmentVertical.Top:
					aRect.y = itsTargetRect.y + aButtonPadding;
					break;
				case KGFAlignmentVertical.Middle:
					if (itsDataModuleMinimap.itsAppearanceMap.itsOrientation == KGFOrientation.Horizontal)
						aRect.y = (itsTargetRect.yMax-itsTargetRect.yMin)/2-aButtonsShortSide/2;
					else
						aRect.y = (itsTargetRect.yMax-itsTargetRect.yMin)/2-aButtonsLongSide/2;
					break;
				case KGFAlignmentVertical.Bottom:
					if (itsDataModuleMinimap.itsAppearanceMap.itsOrientation == KGFOrientation.Horizontal)
						aRect.y = itsTargetRect.yMax-aButtonsShortSide-aButtonPadding;
					else
						aRect.y = itsTargetRect.yMax-aButtonsLongSide-aButtonPadding;
					break;
			}
			aRect.width = GetButtonSize();
			aRect.height = GetButtonSize();
			
			itsRectZoomIn = itsRectZoomOut = itsRectStatic = itsRectFullscreen = aRect;
			if (itsDataModuleMinimap.itsAppearanceMap.itsOrientation == KGFOrientation.Horizontal)
			{
				itsRectZoomOut.x = itsRectZoomIn.x+aSpace+GetButtonSize();
				itsRectStatic.x = itsRectZoomOut.x+aSpace+GetButtonSize();
				itsRectFullscreen.x = itsRectStatic.x+aSpace+GetButtonSize();
			}
			else
			{
				itsRectZoomOut.y = itsRectZoomIn.y+aSpace+GetButtonSize();
				itsRectStatic.y = itsRectZoomOut.y+aSpace+GetButtonSize();
				itsRectFullscreen.y = itsRectStatic.y+aSpace+GetButtonSize();
			}
		}
		else
		{
			// left top button
			itsRectZoomIn = new Rect(itsTargetRect.x+aButtonPadding,
			                         itsTargetRect.y+aButtonPadding,
			                         aButtonSize,aButtonSize);
			
			// left bottom button
			itsRectZoomOut = new Rect(itsTargetRect.x+aButtonPadding,
			                          itsTargetRect.y+itsTargetRect.height-aButtonSize-aButtonPadding,
			                          aButtonSize,aButtonSize);
			
			// right top button
			itsRectStatic = new Rect(itsTargetRect.x+itsTargetRect.width-aButtonSize-aButtonPadding,
			                         itsTargetRect.y+aButtonPadding,
			                         aButtonSize,aButtonSize);
			
			// right bottom button
			itsRectFullscreen = new Rect(itsTargetRect.x+itsTargetRect.width-aButtonSize-aButtonPadding,
			                             itsTargetRect.y+itsTargetRect.height-aButtonSize-aButtonPadding,
			                             aButtonSize,aButtonSize);
		}
		// draw buttons
		if (DrawButton(itsRectZoomIn,GetFullscreen()?itsDataModuleMinimap.itsAppearanceMap.itsIconZoomIn:itsDataModuleMinimap.itsAppearanceMiniMap.itsIconZoomIn))
		{
			ZoomIn();
		}
		if (DrawButton(itsRectZoomOut,GetFullscreen()?itsDataModuleMinimap.itsAppearanceMap.itsIconZoomOut:itsDataModuleMinimap.itsAppearanceMiniMap.itsIconZoomOut))
		{
			ZoomOut();
		}
		if (DrawButton(itsRectStatic,GetFullscreen()?itsDataModuleMinimap.itsAppearanceMap.itsIconZoomLock:itsDataModuleMinimap.itsAppearanceMiniMap.itsIconZoomLock))
		{
			SetModeStatic(!GetModeStatic());
		}
		if (DrawButton(itsRectFullscreen,GetFullscreen()?itsDataModuleMinimap.itsAppearanceMap.itsIconFullscreen:itsDataModuleMinimap.itsAppearanceMiniMap.itsIconFullscreen))
		{
			SetFullscreen(!GetFullscreen());
		}
	}
	
	void UpdateOrthographicSize()
	{
		itsCamera.orthographicSize = ((float)GetHeight() / itsPixelPerMeter)/2;
		itsCamera.aspect = ((float)GetWidth())/((float)GetHeight());
//		itsCamera.ResetAspect();
	}
	
	void CorrectCurrentZoom()
	{
		itsPixelPerMeter = Mathf.Min(itsPixelPerMeter,itsDataModuleMinimap.itsZoom.itsZoomMax);
		itsPixelPerMeter = Mathf.Max(itsPixelPerMeter,itsDataModuleMinimap.itsZoom.itsZoomMin);
	}
	
	/// <summary>
	/// Get current zoom factor
	/// </summary>
	/// <returns>The current zoom factor in pixels per meter</returns>
	public float GetZoom()
	{
		return itsPixelPerMeter;
	}
	
	/// <summary>
	/// Set current zoom factor
	/// </summary>
	/// <param name="theZoom">The zoom factor in pixels per meter</param>
	public void SetZoom(float theZoom)
	{
		itsPixelPerMeter = theZoom;
		CorrectCurrentZoom();
		UpdateOrthographicSize();
		UpdateIconScale();
	}
	
	/// <summary>
	/// Extends the minimap range by itsZoomRange
	/// The range will be clamped to itsMaxRange
	/// </summary>
	public void ZoomOut()
	{
		SetZoom(GetZoom() - itsDataModuleMinimap.itsZoom.itsZoomChangeValue);
	}
	
	/// <summary>
	/// Reduces the minimap range y itsZoomRange
	/// The range will be clamped to itsMinrange
	/// </summary>
	public void ZoomIn()
	{
		SetZoom(GetZoom() + itsDataModuleMinimap.itsZoom.itsZoomChangeValue);
	}
	
	/// <summary>
	/// Set zoom to minimum
	/// </summary>
	public void ZoomMin()
	{
		SetZoom(itsDataModuleMinimap.itsZoom.itsZoomMin);
	}
	
	/// <summary>
	/// Set zoom to maximum
	/// </summary>
	public void ZoomMax()
	{
		SetZoom(itsDataModuleMinimap.itsZoom.itsZoomMax);
	}
	
	/// <summary>
	/// Enable/disable display of viewport
	/// </summary>
	/// <param name="theEnable"></param>
	public void SetViewportEnabled(bool theEnable)
	{
		itsDataModuleMinimap.itsViewport.itsActive = theEnable;
	}
	
	/// <summary>
	/// Returns the percentage of revealed fog of war
	/// Value is between 0 and 1
	/// </summary>
	/// <returns></returns>
	public float GetRevealedPercent()
	{
		float aSum = 0;
		if (itsMeshFilterFogOfWarPlane != null)
		{
			foreach (Color aColor in itsMeshFilterFogOfWarPlane.mesh.colors)
			{
				aSum += aColor.a;
			}
			return 1 - aSum/itsMeshFilterFogOfWarPlane.mesh.colors.Length;
		}
		return 0;
	}
	#endregion
	
	#region Methods for KGFICustomGUI
	public override string GetName()
	{
		return name;
	}
	
	public string GetHeaderName()
	{
		return name;
	}
	
	public override Texture2D GetIcon()
	{
		return (Texture2D)Resources.Load("KGFMapSystem/textures/mapsystem_small", typeof(Texture2D));
	}
	
	string[] GetNamesFromLayerMask(LayerMask theLayers)
	{
		List<string> aNameList = new List<string>();
		for (int i=0;i<32;i++)
		{
			if ((theLayers & (1 << i)) != 0)
			{
				string aName = LayerMask.LayerToName(i);
				if (aName.Trim() != string.Empty)
				{
					aNameList.Add(aName);
				}
			}
		}
		return aNameList.ToArray();
	}
	
	void DrawCustomGuiMain()
	{
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsTarget");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsTarget.gameObject.GetObjectPath());
		}
		KGFGUIUtility.EndHorizontalBox();
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsStaticNorth");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsStaticNorth);
		}
		KGFGUIUtility.EndHorizontalBox();
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom);
		{
			KGFGUIUtility.Label("Enabled");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsIsActive);
		}
		KGFGUIUtility.EndHorizontalBox();
	}
	
	void DrawCustomGuiAppearanceMinimap()
	{
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsSize");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsAppearanceMiniMap.itsSize);
		}
		KGFGUIUtility.EndHorizontalBox();
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsButtonSize");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsAppearanceMiniMap.itsButtonSize);
		}
		KGFGUIUtility.EndHorizontalBox();
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsButtonPadding");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsAppearanceMiniMap.itsButtonPadding);
		}
		KGFGUIUtility.EndHorizontalBox();
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsScaleIcons");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsAppearanceMiniMap.itsScaleIcons);
		}
		KGFGUIUtility.EndHorizontalBox();
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsScaleArrows");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsAppearanceMiniMap.itsScaleArrows);
		}
		KGFGUIUtility.EndHorizontalBox();
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom);
		{
			KGFGUIUtility.Label("itsRadiusArrows");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsAppearanceMiniMap.itsRadiusArrows);
		}
		KGFGUIUtility.EndHorizontalBox();
	}
	
	void DrawCustomGuiAppearanceMap()
	{
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsButtonSize");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsAppearanceMap.itsButtonSize);
		}
		KGFGUIUtility.EndHorizontalBox();
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsButtonPadding");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsAppearanceMap.itsButtonPadding);
		}
		KGFGUIUtility.EndHorizontalBox();
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsButtonSpace");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsAppearanceMap.itsButtonSpace);
		}
		KGFGUIUtility.EndHorizontalBox();
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom);
		{
			KGFGUIUtility.Label("itsScaleIcons");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsAppearanceMap.itsScaleIcons);
		}
		KGFGUIUtility.EndHorizontalBox();
	}
	
	void DrawCustomGuiFogOfWar()
	{
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("Enabled");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsFogOfWar.itsActive);
		}
		KGFGUIUtility.EndHorizontalBox();
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsResolutionX");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsFogOfWar.itsResolutionX);
		}
		KGFGUIUtility.EndHorizontalBox();
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsResolutionY");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsFogOfWar.itsResolutionY);
		}
		KGFGUIUtility.EndHorizontalBox();
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsRevealDistance");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsFogOfWar.itsRevealDistance);
		}
		KGFGUIUtility.EndHorizontalBox();
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsRevealedFullDistance");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsFogOfWar.itsRevealedFullDistance);
		}
		KGFGUIUtility.EndHorizontalBox();
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom);
		{
			KGFGUIUtility.Label("Revealed");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(string.Format("{0:0.00}%",GetRevealedPercent()*100));
		}
		KGFGUIUtility.EndHorizontalBox();
	}
	
	void DrawCustomGuiZoom()
	{
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("Current zoom");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsPixelPerMeter);
		}
		KGFGUIUtility.EndHorizontalBox();
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsZoomStartValue");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsZoom.itsZoomStartValue);
		}
		KGFGUIUtility.EndHorizontalBox();
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsZoomMin");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsZoom.itsZoomMin);
		}
		KGFGUIUtility.EndHorizontalBox();
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsZoomMax");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsZoom.itsZoomMax);
		}
		KGFGUIUtility.EndHorizontalBox();
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom);
		{
			KGFGUIUtility.Label("itsZoomChangeValue");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsZoom.itsZoomChangeValue);
		}
		KGFGUIUtility.EndHorizontalBox();
	}
	
	void DrawCustomGuiViewport()
	{
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("Enabled");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsViewport.itsActive);
		}
		KGFGUIUtility.EndHorizontalBox();
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom);
		{
			KGFGUIUtility.Label("itsCamera");
			GUILayout.FlexibleSpace();
			if (itsDataModuleMinimap.itsViewport.itsCamera != null)
				KGFGUIUtility.Label(""+itsDataModuleMinimap.itsViewport.itsCamera.gameObject.GetObjectPath());
			else
				KGFGUIUtility.Label("NONE");
		}
		KGFGUIUtility.EndHorizontalBox();
	}
	
	void DrawCustomGuiPhoto()
	{
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("Enabled");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsPhoto.itsTakePhoto);
		}
		KGFGUIUtility.EndHorizontalBox();
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom);
		{
			KGFGUIUtility.Label("itsPhotoLayers");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxInvisible);
			{
				foreach (string aName in GetNamesFromLayerMask(itsDataModuleMinimap.itsPhoto.itsPhotoLayers))
				{
					GUILayout.Label(aName);
				}
			}
			KGFGUIUtility.EndVerticalBox();
		}
		KGFGUIUtility.EndHorizontalBox();
	}
	
	void DrawCustomGuiMapIcons()
	{
		// create statistics
		int aCountVisible = 0;
		Dictionary<string,int> aListCategories = new Dictionary<string, int>();
		foreach (mapicon_listitem_script anItem in itsListMapIcons)
		{
			if (anItem.GetMapIconVisibilityEffective())
			{
				aCountVisible++;
			}
			
			if (!aListCategories.ContainsKey(anItem.itsMapIcon.GetCategory()))
			{
				aListCategories[anItem.itsMapIcon.GetCategory()] = 1;
			}else{
				aListCategories[anItem.itsMapIcon.GetCategory()]++;
			}
		}
		
		// draw statistics
		KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBox);
		{
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
			{
				KGFGUIUtility.Label("Icons");
				GUILayout.FlexibleSpace();
				KGFGUIUtility.Label(""+itsListMapIcons.Count);
			}
			KGFGUIUtility.EndHorizontalBox();
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
			{
				KGFGUIUtility.Label("Icons visible");
				GUILayout.FlexibleSpace();
				KGFGUIUtility.Label(""+aCountVisible);
			}
			KGFGUIUtility.EndHorizontalBox();
			
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
			{
				KGFGUIUtility.Label("Icons by category");
				GUILayout.FlexibleSpace();
//				KGFGUIUtility.Label(""+aListCategories.Count);
			}
			KGFGUIUtility.EndHorizontalBox();
			
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom);
			{
				KGFGUIUtility.Space();
				KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBox);
				{
					int i = 0;
					foreach (KeyValuePair<string,int>aCategory in aListCategories)
					{
						if (i == 0)
						{
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxTop);
						}
						else if (i == aListCategories.Count - 1)
						{
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom);
						}
						else
						{
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
						}
						{
							KGFGUIUtility.Label(string.Format("\'{0}\'",aCategory.Key));
							GUILayout.FlexibleSpace();
							KGFGUIUtility.Label(""+aCategory.Value);
						}
						KGFGUIUtility.EndHorizontalBox();
						i++;
					}
				}
				KGFGUIUtility.EndVerticalBox();
			}
			KGFGUIUtility.EndHorizontalBox();
		}
		KGFGUIUtility.EndVerticalBox();
	}
	
	void DrawCustomGuiUserFlags()
	{
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("Enabled");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsUserFlags.itsActive);
		}
		KGFGUIUtility.EndHorizontalBox();
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsMapIcon");
			GUILayout.FlexibleSpace();
			if (itsDataModuleMinimap.itsUserFlags.itsMapIcon != null)
				KGFGUIUtility.Label(""+itsDataModuleMinimap.itsUserFlags.itsMapIcon.gameObject.GetObjectPath());
			else
				KGFGUIUtility.Label("NONE");
		}
		KGFGUIUtility.EndHorizontalBox();
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("itsRemoveClickDistance");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+itsDataModuleMinimap.itsUserFlags.itsRemoveClickDistance);
		}
		KGFGUIUtility.EndHorizontalBox();
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom);
		{
			KGFGUIUtility.Label("Flag count");
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label(""+GetUserFlags().Length);
		}
		KGFGUIUtility.EndHorizontalBox();
	}
	
	Vector2 itsCustomGuiPosition = Vector2.zero;
	public void Render()
	{
		itsCustomGuiPosition = KGFGUIUtility.BeginScrollView(itsCustomGuiPosition,false,false);
		{
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxInvisible);
			{
				KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxInvisible);
				{
					KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDecorated);
					{
						KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
						KGFGUIUtility.Label("Main",GetIcon(),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
						GUILayout.FlexibleSpace();
						KGFGUIUtility.EndHorizontalBox();
						DrawCustomGuiMain();
					}
					KGFGUIUtility.EndVerticalBox();
					
					KGFGUIUtility.Space();
					
					KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDecorated);
					{
						KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
						KGFGUIUtility.Label("Appearance Minimap",GetIcon(),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
						GUILayout.FlexibleSpace();
						KGFGUIUtility.EndHorizontalBox();
						DrawCustomGuiAppearanceMinimap();
					}
					KGFGUIUtility.EndVerticalBox();
					
					KGFGUIUtility.Space();
					
					KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDecorated);
					{
						KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
						KGFGUIUtility.Label("Appearance Map",GetIcon(),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
						GUILayout.FlexibleSpace();
						KGFGUIUtility.EndHorizontalBox();
						DrawCustomGuiAppearanceMap();
					}
					KGFGUIUtility.EndVerticalBox();
					
					KGFGUIUtility.Space();
					
					KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDecorated);
					{
						KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
						KGFGUIUtility.Label("Fog of war",GetIcon(),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
						GUILayout.FlexibleSpace();
						KGFGUIUtility.EndHorizontalBox();
						DrawCustomGuiFogOfWar();
					}
					KGFGUIUtility.EndVerticalBox();
					
					KGFGUIUtility.Space();
					
					KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDecorated);
					{
						KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
						KGFGUIUtility.Label("Zoom",GetIcon(),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
						GUILayout.FlexibleSpace();
						KGFGUIUtility.EndHorizontalBox();
						DrawCustomGuiZoom();
					}
					KGFGUIUtility.EndVerticalBox();
				}
				KGFGUIUtility.EndVerticalBox();
				
				KGFGUIUtility.Space();
				
				KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxInvisible);
				{
					KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDecorated);
					{
						KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
						KGFGUIUtility.Label("Viewport",GetIcon(),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
						GUILayout.FlexibleSpace();
						KGFGUIUtility.EndHorizontalBox();
						DrawCustomGuiViewport();
					}
					KGFGUIUtility.EndVerticalBox();
					
					KGFGUIUtility.Space();
					
					KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDecorated);
					{
						KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
						KGFGUIUtility.Label("Photo",GetIcon(),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
						GUILayout.FlexibleSpace();
						KGFGUIUtility.EndHorizontalBox();
						DrawCustomGuiPhoto();
					}
					KGFGUIUtility.EndVerticalBox();
					
					KGFGUIUtility.Space();
					
					KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDecorated);
					{
						KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
						KGFGUIUtility.Label("Map Icons",GetIcon(),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
						GUILayout.FlexibleSpace();
						KGFGUIUtility.EndHorizontalBox();
						DrawCustomGuiMapIcons();
					}
					KGFGUIUtility.EndVerticalBox();
					
					KGFGUIUtility.Space();
					
					KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDecorated);
					{
						KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
						KGFGUIUtility.Label("User flags",GetIcon(),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
						GUILayout.FlexibleSpace();
						KGFGUIUtility.EndHorizontalBox();
						DrawCustomGuiUserFlags();
					}
					KGFGUIUtility.EndVerticalBox();
				}
				KGFGUIUtility.EndVerticalBox();
			}
			KGFGUIUtility.EndHorizontalBox();
		}
		KGFGUIUtility.EndScrollView();
	}
	#endregion
	
	#region Methods for KGFIValidator
	void NullError(ref KGFMessageList theMessageList,string theName, object theValue)
	{
		if (theValue == null)
		{
			theMessageList.AddError(string.Format("value of '{0}' must not be null",theName));
		}
	}
	
	void RegionError(ref KGFMessageList theMessageList,string theName, float theValue,float theMin,float theMax)
	{
		if (theValue < theMin ||theValue > theMax)
		{
			theMessageList.AddError(string.Format("Value has to be between {0} and {1} ({2})",theMin,theMax,theName));
		}
	}
	
	void PositiveError(ref KGFMessageList theMessageList,string theName, float theValue)
	{
		if (theValue < 0)
		{
			theMessageList.AddError(string.Format("{0} must be positive",theName));
		}
	}
	
	public override KGFMessageList Validate()
	{
		KGFMessageList aMessageList = new KGFMessageList();
		
		// main
		RegionError(ref aMessageList,"itsDataModuleMinimap.itsStaticNorth",itsDataModuleMinimap.itsStaticNorth,0,360);
		if (itsDataModuleMinimap.itsTarget == null)
		{
			aMessageList.AddError("itsTarget must not be null. Please add a target that is always centered on the minimap (e.g.: the character).");
		}
		
		// appearance minimap
		if (itsDataModuleMinimap.itsAppearanceMiniMap.itsMask != null && !GetHasProVersion())
		{
			aMessageList.AddError("Masking texture does only work in Unity Pro version. (itsAppearanceMiniMap.itsMask)");
		}
		RegionError(ref aMessageList,"itsAppearanceMiniMap.itsSize",itsDataModuleMinimap.itsAppearanceMiniMap.itsSize,0,1);
		RegionError(ref aMessageList,"itsAppearanceMiniMap.itsButtonSize",itsDataModuleMinimap.itsAppearanceMiniMap.itsButtonSize,0,1);
		RegionError(ref aMessageList,"itsAppearanceMiniMap.itsButtonPadding",itsDataModuleMinimap.itsAppearanceMiniMap.itsButtonPadding,0,1);
		RegionError(ref aMessageList,"itsAppearanceMiniMap.itsScaleArrows",itsDataModuleMinimap.itsAppearanceMiniMap.itsScaleArrows,0,1);
		RegionError(ref aMessageList,"itsAppearanceMiniMap.itsScaleIcons",itsDataModuleMinimap.itsAppearanceMiniMap.itsScaleIcons,0,1);
		RegionError(ref aMessageList,"itsAppearanceMiniMap.itsRadiusArrows",itsDataModuleMinimap.itsAppearanceMiniMap.itsRadiusArrows,0,1);
		PositiveError(ref aMessageList,"itsAppearanceMiniMap.itsBackgroundBorder",itsDataModuleMinimap.itsAppearanceMiniMap.itsBackgroundBorder);
		if (itsDataModuleMinimap.itsAppearanceMiniMap.itsBackground == null)
		{
			aMessageList.AddWarning("Appearance texture should be set (itsAppearance.itsBackground)");
		}
		if (itsDataModuleMinimap.itsAppearanceMiniMap.itsButton == null)
		{
			aMessageList.AddWarning("Appearance texture should be set (itsAppearance.itsButton)");
		}
		if (itsDataModuleMinimap.itsAppearanceMiniMap.itsButtonDown == null)
		{
			aMessageList.AddWarning("Appearance texture should be set (itsAppearance.itsButtonDown)");
		}
		if (itsDataModuleMinimap.itsAppearanceMiniMap.itsButtonHover == null)
		{
			aMessageList.AddWarning("Appearance texture should be set (itsAppearance.itsButtonHover)");
		}
		if (itsDataModuleMinimap.itsAppearanceMiniMap.itsIconZoomIn == null)
		{
			aMessageList.AddWarning("Appearance texture should be set (itsAppearance.itsIconZoomIn)");
		}
		if (itsDataModuleMinimap.itsAppearanceMiniMap.itsIconZoomOut == null)
		{
			aMessageList.AddWarning("Appearance texture should be set (itsAppearance.itsIconZoomOut)");
		}
		if (itsDataModuleMinimap.itsAppearanceMiniMap.itsIconZoomLock == null)
		{
			aMessageList.AddWarning("Appearance texture should be set (itsAppearance.itsIconZoomLock)");
		}
		if (itsDataModuleMinimap.itsAppearanceMiniMap.itsIconFullscreen == null)
		{
			aMessageList.AddWarning("Appearance texture should be set (itsAppearance.itsIconFullscreen)");
		}
		
		// appearance map
		RegionError(ref aMessageList,"itsAppearanceMap.itsButtonSize",itsDataModuleMinimap.itsAppearanceMap.itsButtonSize,0,1);
		RegionError(ref aMessageList,"itsAppearanceMap.itsButtonPadding",itsDataModuleMinimap.itsAppearanceMap.itsButtonPadding,0,1);
		RegionError(ref aMessageList,"itsAppearanceMap.itsButtonSpace",itsDataModuleMinimap.itsAppearanceMap.itsButtonSpace,0,1);
		RegionError(ref aMessageList,"itsAppearanceMap.itsScaleIcons",itsDataModuleMinimap.itsAppearanceMap.itsScaleIcons,0,1);
		PositiveError(ref aMessageList,"itsAppearanceMap.itsBackgroundBorder",itsDataModuleMinimap.itsAppearanceMap.itsBackgroundBorder);
		if (itsDataModuleMinimap.itsColorAll != Color.white && !GetHasProVersion())
		{
			aMessageList.AddError("itsColorAll does only work in Unity Pro version. (itsColorAll)");
		}
		if (itsDataModuleMinimap.itsAppearanceMap.itsMask != null && !GetHasProVersion())
		{
			aMessageList.AddError("Masking texture does only work in Unity Pro version. (itsAppearanceMap.itsMask)");
		}
		if (itsDataModuleMinimap.itsAppearanceMap.itsButton == null)
		{
			aMessageList.AddWarning("Appearance texture should be set (itsAppearanceMap.itsButton)");
		}
		if (itsDataModuleMinimap.itsAppearanceMap.itsButtonDown == null)
		{
			aMessageList.AddWarning("Appearance texture should be set (itsAppearanceMap.itsButtonDown)");
		}
		if (itsDataModuleMinimap.itsAppearanceMap.itsButtonHover == null)
		{
			aMessageList.AddWarning("Appearance texture should be set (itsAppearanceMap.itsButtonHover)");
		}
		if (itsDataModuleMinimap.itsAppearanceMap.itsIconZoomIn == null)
		{
			aMessageList.AddWarning("Appearance texture should be set (itsAppearanceMap.itsIconZoomIn)");
		}
		if (itsDataModuleMinimap.itsAppearanceMap.itsIconZoomOut == null)
		{
			aMessageList.AddWarning("Appearance texture should be set (itsAppearanceMap.itsIconZoomOut)");
		}
		if (itsDataModuleMinimap.itsAppearanceMap.itsIconZoomLock == null)
		{
			aMessageList.AddWarning("Appearance texture should be set (itsAppearanceMap.itsIconZoomLock)");
		}
		if (itsDataModuleMinimap.itsAppearanceMap.itsIconFullscreen == null)
		{
			aMessageList.AddWarning("Appearance texture should be set (itsAppearanceMap.itsIconFullscreen)");
		}
		
		// fog-of-war
		PositiveError(ref aMessageList,"itsFogOfWar.itsResolutionX",itsDataModuleMinimap.itsFogOfWar.itsResolutionX);
		PositiveError(ref aMessageList,"itsFogOfWar.itsResolutionY",itsDataModuleMinimap.itsFogOfWar.itsResolutionY);
		PositiveError(ref aMessageList,"itsFogOfWar.itsRevealDistance",itsDataModuleMinimap.itsFogOfWar.itsRevealDistance);
		PositiveError(ref aMessageList,"itsFogOfWar.itsRevealedFullDistance",itsDataModuleMinimap.itsFogOfWar.itsRevealedFullDistance);
		if (itsDataModuleMinimap.itsFogOfWar.itsRevealedFullDistance > itsDataModuleMinimap.itsFogOfWar.itsRevealDistance)
		{
			aMessageList.AddError("itsFogOfWar.itsRevealDistance must be bigger than itsFogOfWar.itsRevealedFullDistance");
		}
		
		// zoom
		PositiveError(ref aMessageList,"itsZoom.itsZoomChangeValue",itsDataModuleMinimap.itsZoom.itsZoomChangeValue);
		PositiveError(ref aMessageList,"itsZoom.itsZoomMax",itsDataModuleMinimap.itsZoom.itsZoomMax);
		PositiveError(ref aMessageList,"itsZoom.itsZoomMin",itsDataModuleMinimap.itsZoom.itsZoomMin);
		PositiveError(ref aMessageList,"itsZoom.itsZoomStartValue",itsDataModuleMinimap.itsZoom.itsZoomStartValue);
		if (itsDataModuleMinimap.itsZoom.itsZoomMin > itsDataModuleMinimap.itsZoom.itsZoomMax)
		{
			aMessageList.AddError("itsZoom.itsZoomMax must be bigger than itsZoom.itsZoomMin");
		}
		if (itsDataModuleMinimap.itsZoom.itsZoomStartValue < itsDataModuleMinimap.itsZoom.itsZoomMin ||
		    itsDataModuleMinimap.itsZoom.itsZoomStartValue > itsDataModuleMinimap.itsZoom.itsZoomMax)
		{
			aMessageList.AddError("itsZoom.itsZoomStartValue has to be between itsZoom.itsZoomMin and itsZoom.itsZoomMin");
		}
		
		// viewport
		if (itsDataModuleMinimap.itsViewport.itsActive && itsDataModuleMinimap.itsViewport.itsCamera == null)
		{
			aMessageList.AddError("Active viewport needs a camera (itsViewport.itsCamera)");
		}
		if (itsDataModuleMinimap.itsViewport.itsColor.a == 0)
		{
			aMessageList.AddError("Viewport will be invisible if itsColor.a == 0");
		}
		
		// photo
		if (itsDataModuleMinimap.itsPhoto.itsTakePhoto && itsDataModuleMinimap.itsPhoto.itsPhotoLayers == 0)
		{
			aMessageList.AddError("itsPhoto.itsPhotoLayers has to contain some layers for the photo not to be empty");
		}
		
		// user flags
		if (itsDataModuleMinimap.itsUserFlags.itsActive)
		{
			
		}
		
		// layer check
		if (LayerMask.NameToLayer(itsLayerName) < 0)
		{
			aMessageList.AddError(string.Format("The map system needs a layer with the name '{0}'",itsLayerName));
		}
		
		
		if(itsDataModuleMinimap.itsShaders.itsShaderMapIcon == null)
		{
			aMessageList.AddError(string.Format("itsDataModuleMinimap.itsShaders.itsShaderMapIcon is null"));
		}
		
		if(itsDataModuleMinimap.itsShaders.itsShaderPhotoPlane == null)
		{
			aMessageList.AddError(string.Format("itsDataModuleMinimap.itsShaders.itsShaderPhotoPlane is null"));
		}
		
		if(itsDataModuleMinimap.itsShaders.itsShaderMapMask == null)
		{
			aMessageList.AddError(string.Format("itsDataModuleMinimap.itsShaders.itsShaderMapMask is null"));
		}
		
		if(itsDataModuleMinimap.itsFogOfWar.itsActive == true && itsDataModuleMinimap.itsShaders.itsShaderFogOfWar == null)
		{
			aMessageList.AddWarning(string.Format("itsDataModuleMinimap.itsShaders.itsShaderFogOfWar is null, fog of war will not work"));
		}
		
		Transform aTransform = transform.Find("measure_cube");
		if(itsDataModuleMinimap.itsPhoto.itsTakePhoto == true && aTransform == null)
		{
			aMessageList.AddError(string.Format("please press the RecalcPhotoArea button"));
		}
		
		return aMessageList;
	}
	#endregion
	
	#region KGFModule
	public override string GetForumPath()
	{
		return "";
	}
	
	public override string GetDocumentationPath()
	{
		return "";
	}
	#endregion
	
	#region KGFICustomInspectorGUI
	public void DrawInspectorGUI(object theObject, bool theIsPrefab)
	{
		if (!theIsPrefab)
		{
			if (KGFGUIUtility.Button("RecalcPhotoArea",KGFGUIUtility.eStyleButton.eButton))
			{
				DoUpdateMeasure();
			}
		}
	}
	#endregion
}
