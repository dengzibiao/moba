// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2011-10-03</date>
// <summary>short summary</summary>

// PLEASE uncomment these lines if you do own the corresponding modules
//#define KGFDebug
//#define KGFConsole

using UnityEngine;
using System;
using System.Collections;

public class KGFMapIcon : KGFObject, KGFIMapIcon, KGFIValidator
{
	/// <summary>
	/// Data component
	/// </summary>
	public KGFDataMapIcon itsDataMapIcon = new KGFDataMapIcon();
	
	private bool itsMapIconIsVisible;
	
	/// <summary>
	/// The map system itselve
	/// </summary>
	private KGFMapSystem itsMapSystem = null;
	
	/// <summary>
	/// Gameobject that will represent the map icon on the minmap
	/// </summary>
	private GameObject itsRepresentation = null;
	
	/// <summary>
	/// Shader used for the MapIcon
	/// </summary>
	private Shader itsShaderMapIcon = null;
	
	#region internal class
	[System.Serializable]
	public class KGFDataMapIcon
	{
		/// <summary>
		/// Category string for map icon
		/// </summary>
		public string itsCategory = "";
		
		/// <summary>
		/// Texture for the map icon
		/// </summary>
		public Texture2D itsTextureIcon = null;
		
		/// <summary>
		/// Texture for the arrow pointing at the map icon (when the icon itselve is not visible on the map anymore)
		/// </summary>
		public Texture2D itsTextureArrow = null;
		
		/// <summary>
		/// if true the icon rotation will follow the gameObject rotation on the map. Else tha icon will stay oriented top down.
		/// </summary>
		public bool itsRotate = false;
		
		/// <summary>
		/// Color for the map icon
		/// </summary>
		public Color itsColor = Color.white;
		
		/// <summary>
		/// Start value for visibility
		/// </summary>
		public bool itsIsVisible = true;
		
		/// <summary>
		/// TRUE if arrow should be used
		/// </summary>
		public bool itsUseArrow = true;
	}
	#endregion
	
	protected override void KGFAwake()
	{
		SetVisibility(itsDataMapIcon.itsIsVisible);	// do this before base.Awake
		
		base.KGFAwake();
	}
	
	private GameObject CreateRepresentation()
	{
		if(itsShaderMapIcon == null)
		{
			itsShaderMapIcon = Shader.Find("ColorTextureAlpha");
			if(itsShaderMapIcon == null)
			{
				LogError("Cannot find shader ColorTextureAlpha",typeof(KGFMapSystem).Name,this);
				return null;
			}
		}
		if(itsDataMapIcon.itsTextureIcon == null)
		{
			LogError("itsDataMapIcon.itsTextureIcon is null",typeof(KGFMapSystem).Name,this);
			return null;
		}
		
		if(itsShaderMapIcon != null & itsDataMapIcon.itsTextureIcon != null)
		{
			return KGFMapSystem.GenerateTexturePlane(itsDataMapIcon.itsTextureIcon,itsShaderMapIcon);
		}
		return null;
	}
	
	#region Methods for KGFIMapIcon
	/// <summary>
	/// Get transform of this map icon
	/// </summary>
	/// <returns></returns>
	public Transform GetTransform()
	{
		if (this == null)
			return null;
		if (gameObject == null)
			return null;
		return transform;
	}
	
	/// <summary>
	/// Returns the name of the gameobject
	/// </summary>
	/// <returns></returns>
	public string GetGameObjectName()
	{
		return gameObject.name;
	}
	
	/// <summary>
	/// Get category of this map icon
	/// </summary>
	/// <returns></returns>
	public virtual string GetCategory()
	{
		return itsDataMapIcon.itsCategory;
	}
	
	/// <summary>
	/// Getter for the color
	/// </summary>
	/// <returns>returns the color of the map icon</returns>
	public Color GetColor()
	{
		return itsDataMapIcon.itsColor;
	}
	
	/// <summary>
	/// Getter for the arrow texture
	/// </summary>
	/// <returns></returns>
	public Texture2D GetTextureArrow()
	{
		return itsDataMapIcon.itsTextureArrow;
	}
	
	/// <summary>
	/// Returns if the minimap icon rotation should follow the gameobject rotation
	/// </summary>
	/// <returns></returns>
	public bool GetRotate()
	{
		return itsDataMapIcon.itsRotate;
	}
	
	/// <summary>
	/// Get TRUE if map icon should be visible
	/// </summary>
	/// <returns></returns>
	public virtual bool GetIsVisible()
	{
		return itsMapIconIsVisible;
	}
	
	/// <summary>
	/// Returns if the arrow icon is allowed to be displayed
	/// </summary>
	/// <returns></returns>
	public bool GetIsArrowVisible()
	{
		return itsDataMapIcon.itsUseArrow;
	}
	
	/// <summary>
	/// returns the gameobject that will be displayed on the map for this map icon
	/// </summary>
	/// <returns></returns>
	public GameObject GetRepresentation()
	{
		if(itsRepresentation == null)
			itsRepresentation = CreateRepresentation();
		return itsRepresentation;
	}
	
	#endregion
	
	#region Methods for KGFIValidator
	public KGFMessageList Validate()
	{
		KGFMessageList aMessageList = new KGFMessageList();
		
		if(itsDataMapIcon.itsCategory == string.Empty)
		{
			aMessageList.AddError("itsDataMapIcon.itsCategory is empty");
		}
		
		if(itsDataMapIcon.itsTextureIcon == null)
		{
			aMessageList.AddError("itsDataMapIcon.itsTextureIcon is null");
		}
		return aMessageList;
	}
	#endregion
	
	#region public methods
	/// <summary>
	/// Setter for the color
	/// </summary>
	/// <param name="theColor"></param>
	public void SetColor(Color theColor)
	{
		itsDataMapIcon.itsColor = theColor;
		
		// update KGFMinimap module
		if (itsMapSystem == null)
		{
			itsMapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (itsMapSystem != null)
		{
			itsMapSystem.UpdateIcon(this);
		}
	}
	
	/// <summary>
	/// Set the category for this map icon
	/// </summary>
	/// <param name="theCategory"></param>
	public void SetCategory(string theCategory)
	{
		itsDataMapIcon.itsCategory = theCategory;
	}
	
	/// <summary>
	/// Change the visibility of the map icon
	/// </summary>
	/// <param name="theVisibility"></param>
	public void SetVisibility(bool theVisibility)
	{
		// change visibility
		itsMapIconIsVisible = theVisibility;
		
		// update KGFMinimap module
		if (itsMapSystem == null)
		{
			itsMapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (itsMapSystem != null)
		{
			itsMapSystem.RefreshIconsVisibility();
		}
	}
	
	/// <summary>
	/// Update texture of icon
	/// </summary>
	/// <param name="theTexture"></param>
	public void SetTextureIcon(Texture2D theTexture)
	{
		itsDataMapIcon.itsTextureIcon = theTexture;
		
		if (itsRepresentation == null)
			itsRepresentation = CreateRepresentation();
		if (itsRepresentation != null)
		{
			MeshRenderer aRenderer = itsRepresentation.GetComponent<MeshRenderer>();
			if (aRenderer != null)
			{
				aRenderer.material.mainTexture = itsDataMapIcon.itsTextureIcon;
			}
		}
	}
	
	/// <summary>
	/// Update the arrow icon of this map icon
	/// </summary>
	/// <param name="theTexture">The new texture to be used as arrow icon</param>
	public void SetTextureArrow(Texture2D theTexture)
	{
		itsDataMapIcon.itsTextureArrow = theTexture;
		
		// update KGFMinimap module
		if (itsMapSystem == null)
		{
			itsMapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (itsMapSystem != null)
		{
			itsMapSystem.UpdateIcon(this);
		}
	}
	
	/// <summary>
	/// Set the use-arrow flag of this map icon
	/// </summary>
	/// <param name="theIsArrowUsed"></param>
	public void SetArrowUsage(bool theIsArrowUsed)
	{
		itsDataMapIcon.itsUseArrow = theIsArrowUsed;
		// update KGFMinimap module
		if (itsMapSystem == null)
		{
			itsMapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (itsMapSystem != null)
		{
			itsMapSystem.RefreshIconsVisibility();
		}
	}
	#endregion
	
	#region log abstraction
	public static void LogError(string theError,string theCategory,MonoBehaviour theObject)
	{
		#if KGFDebug
		KGFDebug.LogError(theError,theCategory,theObject);
		#else
		Debug.LogError(string.Format("{0} - {1}",theCategory,theError));
		#endif
	}
	#endregion
}
