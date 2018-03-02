using System;
using System.Collections;
using UnityEngine;

public class KGFMapSystemStyleDemo : KGFModule
{
	public MapSystemStyle[]itsStyles = new KGFMapSystemStyleDemo.MapSystemStyle[0];
	
	public Texture2D itsKOLMICHTexture = null;
	
	[System.SerializableAttribute]
	public class MapSystemStyle
	{
		public string itsName;
		public Texture2D itsBackgroundMinimap;
		public Texture2D itsBackgroundMap;
		public Texture2D itsButton;
		public Texture2D itsButtonHover;
		public Texture2D itsButtonDown;
		public Texture2D itsButtonZoomIn;
		public Texture2D itsButtonZoomOut;
		public Texture2D itsButtonMap;
		public Texture2D itsButtonLock;
		public Color itsColorMap;
		public Color itsColorAll;
		public Texture2D itsMinimapMask;
		public Texture2D itsMapMask;
		public float itsPaddingButtons;
		public Color itsViewportColor;
	}
	
//	GUIStyle itsGuiStyle = null;
		
	
	public KGFMapSystemStyleDemo() : base(new Version(1,0,0,0), new Version(1,1,0,0))
	{}
	
//	protected override void KGFAwake()
//	{
//		base.KGFAwake();
//		
//		UpdateStyle(0);
//	}
	
	/// <summary>
	/// Draw buttons
	/// </summary>
//	void OnGUI()
//	{
//		int aButtonSize = 50;
//		int aButtonSpacing = 10;
//		
//		for (int i=0;i<itsStyles.Length;i++)
//		{
//			if (GUI.Button(new Rect(10,10+(i*(aButtonSize+aButtonSpacing)),100,aButtonSize),itsStyles[i].itsName))
//			{
//				UpdateStyle(i);
//			}
//		}
//		
//		GUI.Label(new Rect(Screen.width - 200.0f,Screen.height - 50.0f,200.0f,50.0f),itsKOLMICHTexture);
//	}
	
	/// <summary>
	/// Update the style of the map system
	/// </summary>
	/// <param name="theIndex"></param>
	void UpdateStyle(int theIndex)
	{
		MapSystemStyle aStyle = itsStyles[theIndex];
		
		KGFMapSystem aMapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		if (aMapSystem != null)
		{
			aMapSystem.itsDataModuleMinimap.itsAppearanceMap.itsBackground = aStyle.itsBackgroundMap;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsBackground = aStyle.itsBackgroundMinimap;
			
			aMapSystem.itsDataModuleMinimap.itsAppearanceMap.itsButton = aStyle.itsButton;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMap.itsButtonHover = aStyle.itsButtonHover;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMap.itsButtonDown = aStyle.itsButtonDown;
			
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsButton = aStyle.itsButton;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsButtonHover = aStyle.itsButtonHover;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsButtonDown = aStyle.itsButtonDown;
			
			
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsIconZoomIn = aStyle.itsButtonZoomIn;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsIconZoomOut = aStyle.itsButtonZoomOut;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsIconFullscreen = aStyle.itsButtonMap;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsIconZoomLock = aStyle.itsButtonLock;
			aMapSystem.itsDataModuleMinimap.itsColorMap = aStyle.itsColorMap;					
			
			aMapSystem.itsDataModuleMinimap.itsAppearanceMap.itsIconZoomIn = aStyle.itsButtonZoomIn;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMap.itsIconZoomOut = aStyle.itsButtonZoomOut;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMap.itsIconFullscreen = aStyle.itsButtonMap;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMap.itsIconZoomLock = aStyle.itsButtonLock;
			
			aMapSystem.SetMask(aStyle.itsMinimapMask, aStyle.itsMapMask);
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsButtonPadding = aStyle.itsPaddingButtons;
			aMapSystem.itsDataModuleMinimap.itsColorAll = aStyle.itsColorAll;
			
			aMapSystem.itsDataModuleMinimap.itsViewport.itsColor = aStyle.itsViewportColor;
			
			
			aMapSystem.UpdateStyles();
		}
		
//		itsGuiStyle = new GUIStyle();
//		itsGuiStyle.normal.background = aStyle.itsButton;
//		itsGuiStyle.hover.background = aStyle.itsButtonHover;
//		itsGuiStyle.active.background = aStyle.itsButtonDown;
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
			UpdateStyle(0);
		else if(Input.GetKeyDown(KeyCode.Alpha2))
			UpdateStyle(1);
		else if(Input.GetKeyDown(KeyCode.Alpha3))
			UpdateStyle(2);			
		else if(Input.GetKeyDown(KeyCode.Alpha4))
			UpdateStyle(3);			
	}
	
	#region KGFModule methods
	public override KGFMessageList Validate()
	{
		return new KGFMessageList();
	}
	
	public override string GetName()
	{
		return name;
	}
	
	public override Texture2D GetIcon()
	{
		return null;
	}
	
	public override string GetForumPath()
	{
		return "";
	}
	
	public override string GetDocumentationPath()
	{
		return "";
	}
	#endregion
}
