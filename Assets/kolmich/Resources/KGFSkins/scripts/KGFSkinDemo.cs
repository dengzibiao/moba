using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KGFSkinDemo : MonoBehaviour
{
	#region members
	
	private bool itsDefaultToggle = true;
	private string itsDefaultTextfield = "default textfield";
	private float itsDefaultSliderHorizontal = 0.3f;
	private float itsDefaultSliderVertical = 0.3f;
	private float itsDefaultScrollbarHorizontal = 0.3f;
	private float itsDefaultScrollbarVertical = 0.3f;
	private string itsDefaultTextarea = "default textarea";
	
	private bool itsUseSkin16 = false;
	private bool itsUseSkin32 = true;
	
	private float itsYOffset = 50.0f;
	private Rect itsWindow0Rect;
	private Rect itsWindow1Rect;
	private Rect itsWindow2Rect;
	private Rect itsWindow3Rect;
	private Rect itsWindow4Rect;
	private Rect itsWindow5Rect;
	
	private Vector2 itsScrollviewPosition1;
	private Vector2 itsScrollviewPosition2;
	
	public Color itsColorNormal;
	public Color itsColorHover;
	public Color itsColorActive;
	public Color itsColorFocused;
	public Color itsColorOnNormal;
	public Color itsColorOnHover;
	public Color itsColorOnActive;
	public Color itsColorOnFocused;
	
	public int itsSkinHeight = 32;
	public string itsSkinName = "fantasy";
	
	private int itsActiveWindowIndex = 0;
	
	#endregion
	
	private enum eDemoStyle
	{
		eNormal,
		eHover,
		eActive,
		eFocused,
		eOnNormal,
		eOnHover,
		eOnActive,
		eOnFocused
	}
	
	private bool itsTogglStretchedState = false;
	private bool itsTogglCompactState = false;
	private bool itsTogglSupercompactState = false;
	private bool itsTogglRadioStrechtedState = false;
	private bool itsTogglRadioCompactState = false;
	private bool itsTogglRadioSupercompactState = false;
	private bool itsTogglSwitchState = false;
	private bool itsTogglBooleanState = false;
	private bool itsTogglArrowState = false;
	private bool itsTogglButtonState = false;
	
	
	
	private string itsTextAreaText = "";
	
	
	//private float itsHorizontalSliderValue = 0.3f;
	//private float itsVerticalSliderValue = 0.3f;
	
	private Rect itsRollingAreaRect;
	
	private bool itsTogglDefault = true;
	private bool itsTogglToggles = false;
	private bool itsTogglButtons = false;
	private bool itsTogglBoxes = false;
	private bool itsTogglBoxesInteractive = false;
	
	public delegate void GuiDelegate();
	
	public void Awake()
	{
		SetCoordinates();
		itsTextAreaText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. öasldköakdfjöskdgs sdgfkjsödfkgsödkgfs öskdgfjösldkgjfös fkdskfdg ";
		SetSkinPath();
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			SetToggl(0);
		}
		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			SetToggl(1);
		}
		if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			SetToggl(2);
		}
		if(Input.GetKeyDown(KeyCode.Alpha4))
		{
			SetToggl(3);
		}
		if(Input.GetKeyDown(KeyCode.Alpha5))
		{
			SetToggl(4);
		}
		if(Input.GetKeyDown(KeyCode.Alpha7))
		{
			itsUseSkin16 = true;
			itsUseSkin32 = false;
			itsSkinHeight = 16;
			SetSkinPath();
			
		}
		if(Input.GetKeyDown(KeyCode.Alpha8))
		{
			itsUseSkin16 = false;
			itsUseSkin32 = true;
			itsSkinHeight = 32;
			SetSkinPath();			
		}
	}
	
	public void SetToggl(int theIndex)
	{
		itsActiveWindowIndex = theIndex;
		itsTogglDefault = false;
		itsTogglToggles = false;
		itsTogglButtons = false;
		itsTogglBoxes = false;
		itsTogglBoxesInteractive = false;
		
		if(theIndex == 0)
			itsTogglDefault = true;
		else if(theIndex == 1)
			itsTogglToggles = true;
		else if(theIndex == 2)
			itsTogglButtons = true;
		else if(theIndex == 3)
			itsTogglBoxes = true;
		else if(theIndex == 4)
			itsTogglBoxesInteractive = true;
	}
	
	public void SetCoordinates()
	{
		float aHeight = itsSkinHeight;
		itsYOffset = 20.0f;
		
		if(aHeight == 32)
		{
			itsYOffset = 40.0f;
		}
		
		float aWindowHeight = Screen.height - itsYOffset;
		
		itsWindow0Rect = new Rect(0.0f,itsYOffset,Screen.width,aWindowHeight);
		itsWindow1Rect = new Rect(0.0f,itsYOffset,Screen.width,aWindowHeight);
		itsWindow2Rect = new Rect(0.0f,itsYOffset,Screen.width,aWindowHeight);
		itsWindow3Rect = new Rect(0.0f,itsYOffset,Screen.width,aWindowHeight);
		itsWindow4Rect = new Rect(0.0f,itsYOffset,Screen.width,aWindowHeight);
		itsWindow5Rect = new Rect(0.0f,itsYOffset,Screen.width,aWindowHeight);
	}

	public void ApplyColors()
	{
		SetCoordinates();
		Color[] aColors = new Color[8];
		aColors[0] = itsColorNormal;
		aColors[1] = itsColorHover;
		aColors[2] = itsColorActive;
		aColors[3] = itsColorFocused;
		aColors[4] = itsColorOnNormal;
		aColors[5] = itsColorOnHover;
		aColors[6] = itsColorOnActive;
		aColors[7] = itsColorOnFocused;
		SetColors(aColors,itsSkinHeight);
		SetFontSize(itsSkinHeight);
		SetHeight(itsSkinHeight);
		SetPadding(itsSkinHeight);
		SetMargin(itsSkinHeight);
		SetContentOffset(itsSkinHeight);
	}
	
	private void SetSkinPath()
	{
		KGFGUIUtility.SetSkinPath("KGFSkins/"+itsSkinName+"/skins/skin_"+itsSkinName+"_"+itsSkinHeight.ToString());
	}
	
	
	public void OnGUI()
	{
		float anOffset = KGFGUIUtility.GetSkinHeight();
		
		float anAreaHeight = KGFGUIUtility.GetSkinHeight() + KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDecorated).padding.vertical;
		
		GUILayout.BeginArea(new Rect(0,0,Screen.width,anAreaHeight));
		{
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDecorated,GUILayout.ExpandWidth(true));
			{
				bool aToggles = itsTogglDefault;
				itsTogglDefault = KGFGUIUtility.Toggle(itsTogglDefault,"default",KGFGUIUtility.eStyleToggl.eTogglRadioStreched);
				if(aToggles != itsTogglDefault)
					SetToggl(0);
				aToggles = itsTogglToggles;
				itsTogglToggles = KGFGUIUtility.Toggle(itsTogglToggles,"toggles",KGFGUIUtility.eStyleToggl.eTogglRadioStreched);
				if(aToggles != itsTogglToggles)
					SetToggl(1);
				aToggles = itsTogglButtons;
				itsTogglButtons = KGFGUIUtility.Toggle(itsTogglButtons,"buttons",KGFGUIUtility.eStyleToggl.eTogglRadioStreched);
				if(aToggles != itsTogglButtons)
					SetToggl(2);
				aToggles = itsTogglBoxes;
				itsTogglBoxes = KGFGUIUtility.Toggle(itsTogglBoxes,"boxes",KGFGUIUtility.eStyleToggl.eTogglRadioStreched);
				if(aToggles != itsTogglBoxes)
					SetToggl(3);
				aToggles = itsTogglBoxesInteractive;
				itsTogglBoxesInteractive = KGFGUIUtility.Toggle(itsTogglBoxesInteractive,"int. boxes",KGFGUIUtility.eStyleToggl.eTogglRadioStreched);
				if(aToggles != itsTogglBoxesInteractive)
					SetToggl(4);
				
				GUILayout.FlexibleSpace();
				
				bool aUseSkin16 = itsUseSkin16;
				itsUseSkin16 = KGFGUIUtility.Toggle(itsUseSkin16,"skin 16",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
				if(aUseSkin16 != itsUseSkin16)
				{
					itsUseSkin16 = true;
					itsUseSkin32 = false;
					itsSkinHeight = 16;
					SetSkinPath();
				}
				bool aUseSkin32 = itsUseSkin32;
				itsUseSkin32 = KGFGUIUtility.Toggle(itsUseSkin32,"skin 32",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
				if(aUseSkin32 != itsUseSkin32)
				{
					itsUseSkin16 = false;
					itsUseSkin32 = true;
					itsSkinHeight = 32;
					SetSkinPath();
				}
			}
			KGFGUIUtility.EndHorizontalBox();
		}
		GUILayout.EndArea();
		
		int aWindowHeight = 450;
		if(KGFGUIUtility.GetSkinHeight() == 32)
			aWindowHeight = 600;
		
		itsWindow0Rect.x = 0;
		itsWindow0Rect.y = anAreaHeight + anOffset;
		itsWindow0Rect.width = Screen.width;
		itsWindow0Rect.height = aWindowHeight;
		itsWindow1Rect.x = 0;
		itsWindow1Rect.y = anAreaHeight + anOffset;
		itsWindow1Rect.width = Screen.width;
		itsWindow1Rect.height = aWindowHeight;
		itsWindow2Rect.x = 0;
		itsWindow2Rect.y = anAreaHeight + anOffset;
		itsWindow2Rect.width = Screen.width;
		itsWindow2Rect.height = aWindowHeight;
		itsWindow3Rect.x = 0;
		itsWindow3Rect.y = anAreaHeight + anOffset;
		itsWindow3Rect.width = Screen.width;
		itsWindow3Rect.height = aWindowHeight;
		itsWindow4Rect.x = 0;
		itsWindow4Rect.y = anAreaHeight + anOffset;
		itsWindow4Rect.width = Screen.width;
		itsWindow4Rect.height = aWindowHeight;
		itsWindow5Rect.x = 0;
		itsWindow5Rect.y = anAreaHeight + anOffset;
		itsWindow5Rect.width = Screen.width;
		itsWindow5Rect.height = aWindowHeight;
		
		if(itsActiveWindowIndex == 0)
			itsWindow0Rect = KGFGUIUtility.Window(-1, itsWindow0Rect, RenderWindow0,"default styles & minimap (height "+KGFGUIUtility.GetSkinHeight()+")");
		if(itsActiveWindowIndex == 1)
			itsWindow1Rect = KGFGUIUtility.Window(0, itsWindow1Rect, RenderWindow1,"custom toggle styles (height "+KGFGUIUtility.GetSkinHeight()+")");
		else if(itsActiveWindowIndex == 2)
			itsWindow2Rect = KGFGUIUtility.Window(1, itsWindow2Rect, RenderWindow2,"custom button styles (height "+KGFGUIUtility.GetSkinHeight()+")");
		else if(itsActiveWindowIndex == 3)
			itsWindow3Rect = KGFGUIUtility.Window(2, itsWindow3Rect, RenderWindow3,"custom box styles (height "+KGFGUIUtility.GetSkinHeight()+")");
		else if(itsActiveWindowIndex == 4)
			itsWindow4Rect = KGFGUIUtility.Window(3, itsWindow4Rect, RenderWindow4,"custom box styles interactive (height "+KGFGUIUtility.GetSkinHeight()+")");
		
		KGFGUIUtility.RenderDropDownList();
	}
	
	private void DrawSeparator()
	{
		GUILayout.Space(5.0f);
		KGFGUIUtility.Separator(KGFGUIUtility.eStyleSeparator.eSeparatorVertical,GUILayout.Height(KGFGUIUtility.GetSkinHeight()));
		GUILayout.Space(5.0f);
	}
	
	private void DrawSeparatorFitInBox()
	{
		GUILayout.Space(5.0f);
		KGFGUIUtility.Separator(KGFGUIUtility.eStyleSeparator.eSeparatorVerticalFitInBox,GUILayout.Height(KGFGUIUtility.GetSkinHeight()));
		GUILayout.Space(5.0f);
	}
	
	private void RenderTitle(float theLabelWidth)
	{
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
		{
			
			KGFGUIUtility.Label("style name:",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(theLabelWidth));
			DrawSeparatorFitInBox();
			KGFGUIUtility.Label("margin:",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(50.0f));
			DrawSeparatorFitInBox();
			KGFGUIUtility.Label("padding:",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(50.0f));
			DrawSeparatorFitInBox();
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label("possible states",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();
	}
	
	void RenderWindow0(int windowID)
	{
		GUI.skin = KGFGUIUtility.GetSkin();
		
		KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxInvisible);
		{
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
			{
				KGFGUIUtility.Label("",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.ExpandWidth(true));
			}
			KGFGUIUtility.EndHorizontalBox();
			
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
			{
//				KGFGUIUtility.BeginVerticalPadding();
//				{
				GUILayout.BeginVertical();
				{
					GUILayout.Space(20.0f);
					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(10.0f);
						GUILayout.Box("default box");
						GUILayout.Space(20.0f);
						GUILayout.Button("default button");
						GUILayout.Space(20.0f);
						itsDefaultToggle = GUILayout.Toggle(itsDefaultToggle,"default toggl");
						GUILayout.Space(20.0f);
						GUILayout.Label("default label");
						GUILayout.Space(20.0f);
						itsDefaultTextfield = GUILayout.TextField(itsDefaultTextfield);
						GUILayout.Space(10.0f);
					}
					GUILayout.EndHorizontal();
					GUILayout.Space(20.0f);
					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(10.0f);
						itsDefaultSliderHorizontal = GUILayout.HorizontalSlider(itsDefaultSliderHorizontal,0.0f,1.0f);
						GUILayout.Space(10.0f);
						itsDefaultScrollbarHorizontal = GUILayout.HorizontalScrollbar(itsDefaultScrollbarHorizontal,0.3f,0.0f,1.0f);
						GUILayout.Space(10.0f);
					}
					GUILayout.EndHorizontal();
					GUILayout.Space(20.0f);
					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(10.0f);
						itsDefaultSliderVertical = GUILayout.VerticalSlider(itsDefaultSliderVertical,0.0f,1.0f);
						GUILayout.Space(20.0f);
						itsDefaultScrollbarVertical = GUILayout.VerticalScrollbar(itsDefaultScrollbarVertical,0.3f,0.0f,1.0f);
						GUILayout.Space(20.0f);
						itsDefaultTextarea = GUILayout.TextArea(itsDefaultTextarea);
						GUILayout.Space(20.0f);
						itsScrollviewPosition2 = GUILayout.BeginScrollView(itsScrollviewPosition2,true,true);
						GUILayout.Label("default scrollview");
						GUILayout.EndScrollView();
						GUILayout.Space(20.0f);
						GUILayout.Label("",KGFGUIUtility.GetStyleMinimapBorder(),GUILayout.Width(200.0f),GUILayout.Height(200.0f));
						GUILayout.Space(10.0f);
					}
					GUILayout.EndHorizontal();
					GUILayout.Space(20.0f);
				}
				GUILayout.EndVertical();
//				}
//				KGFGUIUtility.EndVerticalPadding();
			}
			KGFGUIUtility.EndHorizontalBox();
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkBottom);
			{
				KGFGUIUtility.Label("",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.ExpandWidth(true));
			}
			KGFGUIUtility.EndHorizontalBox();
		}
		KGFGUIUtility.EndVerticalBox();
	}
	
	void RenderWindow1(int windowID)
	{
		float aLabelWidth = 180.0f;
		float aControlWidth = 110.0f;
		float aDescriptionWidth = 50.0f;
		
		
		KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxInvisible);
		{
			RenderTitle(aLabelWidth);
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom);
			{
				GUILayout.BeginVertical();
				{
					GUILayout.Space(KGFGUIUtility.GetSkinHeight()/2.0f);
					//toggl_stretched
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("toggl_stretched",GUILayout.Width(aLabelWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglStreched)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglStreched)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						GUILayout.FlexibleSpace();
						itsTogglStretchedState = KGFGUIUtility.Toggle(itsTogglStretchedState,"normal",KGFGUIUtility.eStyleToggl.eTogglStreched,GUILayout.Width(aControlWidth));
						RenderDemoToggl(KGFGUIUtility.eStyleToggl.eTogglStreched);
						
					}
					GUILayout.EndHorizontal();
					
					//toggl_compact
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("toggl_compact",GUILayout.Width(aLabelWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglCompact)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglCompact)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						GUILayout.FlexibleSpace();
						itsTogglCompactState = KGFGUIUtility.Toggle(itsTogglCompactState,"normal",KGFGUIUtility.eStyleToggl.eTogglCompact,GUILayout.Width(aControlWidth));
						RenderDemoToggl(KGFGUIUtility.eStyleToggl.eTogglCompact);
						
					}
					KGFGUIUtility.EndHorizontalBox();
					
					//toggl_supercompact
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("toggl_supercompact",GUILayout.Width(aLabelWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSuperCompact)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSuperCompact)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						GUILayout.FlexibleSpace();
						itsTogglSupercompactState = KGFGUIUtility.Toggle(itsTogglSupercompactState,"normal",KGFGUIUtility.eStyleToggl.eTogglSuperCompact,GUILayout.Width(aControlWidth));
						RenderDemoToggl(KGFGUIUtility.eStyleToggl.eTogglSuperCompact);
						
					}
					KGFGUIUtility.EndHorizontalBox();
					
					//toggl_radio_stretched
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("toggl_radio_stretched",GUILayout.Width(aLabelWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioStreched)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioStreched)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						GUILayout.FlexibleSpace();
						itsTogglRadioStrechtedState = KGFGUIUtility.Toggle(itsTogglRadioStrechtedState,"normal",KGFGUIUtility.eStyleToggl.eTogglRadioStreched,GUILayout.Width(aControlWidth));
						RenderDemoToggl(KGFGUIUtility.eStyleToggl.eTogglRadioStreched);
						
					}
					KGFGUIUtility.EndHorizontalBox();
					
					//toggl_radio_compact
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("toggl_radio_compact",GUILayout.Width(aLabelWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioCompact)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioCompact)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						GUILayout.FlexibleSpace();
						itsTogglRadioCompactState = KGFGUIUtility.Toggle(itsTogglRadioCompactState,"normal",KGFGUIUtility.eStyleToggl.eTogglRadioCompact,GUILayout.Width(aControlWidth));
						RenderDemoToggl(KGFGUIUtility.eStyleToggl.eTogglRadioCompact);
						
					}
					KGFGUIUtility.EndHorizontalBox();
					
					//toggl_radio_supercompact
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("toggl_radio_supercompact",GUILayout.Width(aLabelWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						GUILayout.FlexibleSpace();
						itsTogglRadioSupercompactState = KGFGUIUtility.Toggle(itsTogglRadioSupercompactState,"normal",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact,GUILayout.Width(aControlWidth));
						RenderDemoToggl(KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
						
					}
					KGFGUIUtility.EndHorizontalBox();
					
					//toggl_switch
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("toggl_switch",GUILayout.Width(aLabelWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSwitch)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSwitch)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						GUILayout.FlexibleSpace();
						itsTogglSwitchState = KGFGUIUtility.Toggle(itsTogglSwitchState,"normal",KGFGUIUtility.eStyleToggl.eTogglSwitch,GUILayout.Width(aControlWidth));
						RenderDemoToggl(KGFGUIUtility.eStyleToggl.eTogglSwitch);
						
					}
					KGFGUIUtility.EndHorizontalBox();
					
					//toggl_boolean
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("toggl_boolean",GUILayout.Width(aLabelWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglBoolean)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglBoolean)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						GUILayout.FlexibleSpace();
						itsTogglBooleanState = KGFGUIUtility.Toggle(itsTogglBooleanState,"normal",KGFGUIUtility.eStyleToggl.eTogglBoolean,GUILayout.Width(aControlWidth));
						RenderDemoToggl(KGFGUIUtility.eStyleToggl.eTogglBoolean);
						
					}
					KGFGUIUtility.EndHorizontalBox();
					
					//toggl_arrow
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("toggl_arrow",GUILayout.Width(aLabelWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglArrow)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglArrow)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						GUILayout.FlexibleSpace();
						itsTogglArrowState = KGFGUIUtility.Toggle(itsTogglArrowState,"normal",KGFGUIUtility.eStyleToggl.eTogglArrow,GUILayout.Width(aControlWidth));
						RenderDemoToggl(KGFGUIUtility.eStyleToggl.eTogglArrow);
						
					}
					KGFGUIUtility.EndHorizontalBox();
					
					//toggl_arrow
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("toggl_button",GUILayout.Width(aLabelWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglButton)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglButton)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						GUILayout.FlexibleSpace();
						itsTogglButtonState = KGFGUIUtility.Toggle(itsTogglButtonState,"normal",KGFGUIUtility.eStyleToggl.eTogglButton,GUILayout.Width(aControlWidth));
						RenderDemoToggl(KGFGUIUtility.eStyleToggl.eTogglButton);
						
					}
					KGFGUIUtility.EndHorizontalBox();
					GUILayout.Space(KGFGUIUtility.GetSkinHeight()/2.0f);
				}
				GUILayout.EndVertical();
			}
			KGFGUIUtility.EndHorizontalBox();
		}
		KGFGUIUtility.EndVerticalBox();
	}
	
	void RenderWindow2(int windowID)
	{
		float aLabelWidth = 220.0f;
		float aControlWidth = 110.0f;
		float aDescriptionWidth = 50.0f;
		
		KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxInvisible);
		{
			RenderTitle(aLabelWidth);
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom);
			{
				GUILayout.BeginVertical();
				{
					GUILayout.Space(KGFGUIUtility.GetSkinHeight()/2.0f);
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("button_left",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonLeft)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonLeft)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Button("normal",KGFGUIUtility.eStyleButton.eButtonLeft, GUILayout.Width(aControlWidth));
						RenderDemoButton(KGFGUIUtility.eStyleButton.eButtonLeft);
						
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("button_right",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonRight)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonRight)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Button("normal",KGFGUIUtility.eStyleButton.eButtonRight, GUILayout.Width(aControlWidth));
						RenderDemoButton(KGFGUIUtility.eStyleButton.eButtonRight);
						
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("button_top",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonTop)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonTop)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Button("normal",KGFGUIUtility.eStyleButton.eButtonTop, GUILayout.Width(aControlWidth));
						RenderDemoButton(KGFGUIUtility.eStyleButton.eButtonTop);
						
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("button_bottom",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonBottom)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonBottom)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Button("normal",KGFGUIUtility.eStyleButton.eButtonBottom, GUILayout.Width(aControlWidth));
						RenderDemoButton(KGFGUIUtility.eStyleButton.eButtonBottom);
						
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("button_middle",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonMiddle)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonMiddle)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Button("normal",KGFGUIUtility.eStyleButton.eButtonMiddle, GUILayout.Width(aControlWidth));
						RenderDemoButton(KGFGUIUtility.eStyleButton.eButtonMiddle);
						
					}
					KGFGUIUtility.EndHorizontalBox();
					GUILayout.Space(KGFGUIUtility.GetSkinHeight()/2.0f);
				}
				GUILayout.EndVertical();
			}
			KGFGUIUtility.EndHorizontalBox();
			
			GUILayout.Space(20.0f);
			
			RenderTitle(aLabelWidth);
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom);
			{
				GUILayout.BeginVertical();
				{
					GUILayout.Space(KGFGUIUtility.GetSkinHeight()/2.0f);
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("textfield_left",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextFieldLeft)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextFieldLeft)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.TextField("normal",KGFGUIUtility.eStyleTextField.eTextFieldLeft,GUILayout.Width(aControlWidth));
						RenderDemoTextField(KGFGUIUtility.eStyleTextField.eTextFieldLeft);
					}
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("textfield_right",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextFieldRight)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextFieldRight)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.TextField("normal",KGFGUIUtility.eStyleTextField.eTextFieldRight,GUILayout.Width(aControlWidth));
						RenderDemoTextField(KGFGUIUtility.eStyleTextField.eTextFieldRight);
					}
					GUILayout.EndHorizontal();
					GUILayout.Space(KGFGUIUtility.GetSkinHeight()/2.0f);
				}
				GUILayout.EndVertical();
			}
			KGFGUIUtility.EndHorizontalBox();
			
			GUILayout.Space(20.0f);
			
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
			{
				KGFGUIUtility.Label("example combinations",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
				GUILayout.FlexibleSpace();
			}
			KGFGUIUtility.EndHorizontalBox();
			
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom);
			{
				GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.BeginVertical();
						GUILayout.FlexibleSpace();
						GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Cursor();
						GUILayout.FlexibleSpace();
						GUILayout.EndHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.EndVertical();
					}
					KGFGUIUtility.EndHorizontalBox();
				}
				KGFGUIUtility.EndVerticalBox();
				
				GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.BeginVertical();
						GUILayout.FlexibleSpace();
						GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.TextField("textfield left",KGFGUIUtility.eStyleTextField.eTextFieldLeft);
						KGFGUIUtility.Button("execute",KGFGUIUtility.eStyleButton.eButtonRight);
						GUILayout.FlexibleSpace();
						GUILayout.EndHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.EndVertical();
					}
					KGFGUIUtility.EndHorizontalBox();
				}
				KGFGUIUtility.EndVerticalBox();
				
				GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.BeginVertical();
						GUILayout.FlexibleSpace();
						GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Button("<",KGFGUIUtility.eStyleButton.eButtonLeft);
						KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontal);
						KGFGUIUtility.Label("page 1 of 20",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
						KGFGUIUtility.EndVerticalBox();
						KGFGUIUtility.Button(">",KGFGUIUtility.eStyleButton.eButtonRight);
						GUILayout.FlexibleSpace();
						GUILayout.EndHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.EndVertical();
					}
					KGFGUIUtility.EndHorizontalBox();
				}
				KGFGUIUtility.EndVerticalBox();
			}
			KGFGUIUtility.EndHorizontalBox();
			
			
			
//			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom);
//			{
//				KGFGUIUtility.Label("DropDownBox",GUILayout.Width(aLabelWidth));
//				GUILayout.FlexibleSpace();
//				GUILayout.BeginVertical();
//				{
//					itsDropDown.Render();
//				}
//				GUILayout.EndVertical();
//				RenderDemoButton(KGFGUIUtility.eStyleButton.eButtonLeft);
//			}
//			KGFGUIUtility.EndHorizontalBox();
		}
		KGFGUIUtility.EndVerticalBox();
	}
	
	void RenderWindow3(int windowID)
	{
		float aLabelWidth = 200.0f;
		float aControlWidth = 110.0f;
		float aDescriptionWidth = 50.0f;
		
		KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxInvisible);
		{
			RenderTitle(aLabelWidth);
			
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom);
			{
				GUILayout.BeginVertical();
				{
					GUILayout.Space(KGFGUIUtility.GetSkinHeight()/2.0f);
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_left",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxLeft)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxLeft)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxLeft, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxLeft);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_middle_horizontal",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontal)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontal)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxMiddleHorizontal, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontal);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_right",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxRight)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxRight)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxRight, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxRight);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_top",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxTop)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxTop)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxTop, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxTop);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_middle_vertical",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxMiddleVertical, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_bottom",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxBottom)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxBottom)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxBottom, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxBottom);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					//-------- dark
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_dark",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDark)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDark)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxDark, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxDark);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_dark_left",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkLeft)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkLeft)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxDarkLeft, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxDarkLeft);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_dark_middle_horizontal",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontal)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontal)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontal, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontal);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_dark_right",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkRight)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkRight)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxDarkRight, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxDarkRight);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_dark_top",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkTop)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkTop)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxDarkTop, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_dark_middle_vertical",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVertical)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVertical)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxDarkMiddleVertical, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVertical);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_dark_bottom",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkBottom)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkBottom)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxDarkBottom, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxDarkBottom);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_decorated",GUILayout.Width(aLabelWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDecorated)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDecorated)),GUILayout.Width(aDescriptionWidth));
						DrawSeparator();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxDecorated, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxDecorated);
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.Space(KGFGUIUtility.GetSkinHeight()/2.0f);
				GUILayout.EndVertical();
			}
			KGFGUIUtility.EndHorizontalBox();
		}
		KGFGUIUtility.EndVerticalBox();
	}
	
	void RenderWindow4(int windowID)
	{
		float aLabelWidth = 200.0f;
		float aControlWidth = 110.0f;
		float aDescriptionWidth = 50.0f;
		
		KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxInvisible);
		{
			RenderTitle(aLabelWidth);
			
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom);
			{
				GUILayout.BeginVertical();
				{
					GUILayout.Space(KGFGUIUtility.GetSkinHeight()/2.0f);
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_interactive",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxInteractive, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxInteractive);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_left_interactive",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxLeftInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxLeftInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxLeftInteractive, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxLeftInteractive);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_middle_horizontal_interactive",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontalInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontalInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxMiddleHorizontalInteractive, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontalInteractive);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_right_interactive",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxRightInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxRightInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxRightInteractive, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxRightInteractive);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_top_interactive",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxTopInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxTopInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxTopInteractive, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxTopInteractive);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_middle_vertical_interactive",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleVerticalInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleVerticalInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxMiddleVerticalInteractive, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxMiddleVerticalInteractive);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_bottom_interactive",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxBottomInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxBottomInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxBottomInteractive, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxBottomInteractive);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					//-------- dark
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_dark_interactive",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxDarkInteractive, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxDarkInteractive);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_dark_left_interactive",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkLeftInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkLeftInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxDarkLeftInteractive, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxDarkLeftInteractive);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_dark_middle_horizontal_interactive",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontalInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontalInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontalInteractive, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontalInteractive);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_dark_right_interactive",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkRightInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkRightInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxDarkRightInteractive, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxDarkRightInteractive);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_dark_top_interactive",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkTopInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkTopInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxDarkTopInteractive, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxDarkTopInteractive);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_dark_middle_vertical_interactive",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVerticalInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVerticalInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxDarkMiddleVerticalInteractive, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVerticalInteractive);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label("box_dark_bottom_interactive",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aLabelWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetMarginString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkBottomInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						KGFGUIUtility.Label(GetPaddingString(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkBottomInteractive)),KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(aDescriptionWidth));
						DrawSeparatorFitInBox();
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Box("normal",KGFGUIUtility.eStyleBox.eBoxDarkBottomInteractive, GUILayout.Width(aControlWidth));
						RenderDemoBox(KGFGUIUtility.eStyleBox.eBoxDarkBottomInteractive);
					}
					KGFGUIUtility.EndHorizontalBox();
				}
				GUILayout.Space(KGFGUIUtility.GetSkinHeight()/2.0f);
				GUILayout.EndVertical();
			}
			KGFGUIUtility.EndHorizontalBox();
		}
		KGFGUIUtility.EndVerticalBox();
	}
	
	private GUIStyle GetStyle(GUIStyle theStyle, eDemoStyle theDemoStyle)
	{
		GUIStyle aReturnStyle = new GUIStyle(theStyle);
		if(theDemoStyle == eDemoStyle.eHover)
		{
			aReturnStyle.normal.background = aReturnStyle.hover.background;
			aReturnStyle.normal.textColor = aReturnStyle.hover.textColor;
		}
		else if(theDemoStyle == eDemoStyle.eActive)
		{
			aReturnStyle.normal.background = aReturnStyle.active.background;
			aReturnStyle.normal.textColor = aReturnStyle.active.textColor;
		}
		else if(theDemoStyle == eDemoStyle.eFocused)
		{
			aReturnStyle.normal.background = aReturnStyle.focused.background;
			aReturnStyle.normal.textColor = aReturnStyle.focused.textColor;
		}
		if(theDemoStyle == eDemoStyle.eOnNormal)
		{
			aReturnStyle.normal.background = aReturnStyle.onNormal.background;
			aReturnStyle.normal.textColor = aReturnStyle.onNormal.textColor;
		}
		if(theDemoStyle == eDemoStyle.eOnHover)
		{
			aReturnStyle.normal.background = aReturnStyle.onHover.background;
			aReturnStyle.normal.textColor = aReturnStyle.onHover.textColor;
		}
		else if(theDemoStyle == eDemoStyle.eOnActive)
		{
			aReturnStyle.normal.background = aReturnStyle.onActive.background;
			aReturnStyle.normal.textColor = aReturnStyle.onActive.textColor;
		}
		else if(theDemoStyle == eDemoStyle.eOnFocused)
		{
			aReturnStyle.normal.background = aReturnStyle.onFocused.background;
			aReturnStyle.normal.textColor = aReturnStyle.onFocused.textColor;
		}
		
		
		aReturnStyle.hover.background = null;
		aReturnStyle.active.background = null;
		aReturnStyle.focused.background = null;
		aReturnStyle.onNormal.background = null;
		aReturnStyle.onHover.background = null;
		aReturnStyle.onActive.background = null;
		aReturnStyle.onFocused.background = null;
		return aReturnStyle;
	}
	
	private void RenderDemoToggl(KGFGUIUtility.eStyleToggl theStyle)
	{
		float aWidth = 110.0f;
		GUIStyle aStyle = KGFGUIUtility.GetStyleToggl(theStyle);
		//GUILayout.Toggle(false,"normal",GetStyleToggl(theStyle,eDemoStyle.eNormal),GUILayout.Width(aWidth));
		GUILayout.Toggle(false,"hover",GetStyle(aStyle,eDemoStyle.eHover),GUILayout.Width(aWidth));
		GUILayout.Toggle(true,"onnormal",GetStyle(aStyle,eDemoStyle.eOnNormal),GUILayout.Width(aWidth));
		GUILayout.Toggle(true,"onhover",GetStyle(aStyle,eDemoStyle.eOnHover),GUILayout.Width(aWidth));
	}
	
	private void RenderDemoTextField(KGFGUIUtility.eStyleTextField theStyle)
	{
		float aWidth = 110.0f;
		GUIStyle aStyle = KGFGUIUtility.GetStyleTextField(theStyle);
		
		GUILayout.TextField("hover",GetStyle(aStyle,eDemoStyle.eHover),GUILayout.Width(aWidth));
		GUILayout.TextField("focused",GetStyle(aStyle,eDemoStyle.eFocused),GUILayout.Width(aWidth));
		GUILayout.Space(10.0f);
	}
	
	private void RenderDemoTextArea()
	{
		float aWidth = 110.0f;
		GUIStyle aStyle = KGFGUIUtility.GetStyleTextArea();
		
		GUILayout.TextArea(itsTextAreaText,GetStyle(aStyle,eDemoStyle.eHover),GUILayout.Width(aWidth),GUILayout.Height(100.0f));
		GUILayout.TextArea(itsTextAreaText,GetStyle(aStyle,eDemoStyle.eFocused),GUILayout.Width(aWidth),GUILayout.Height(100.0f));
	}
	
	private void RenderDemoButton(KGFGUIUtility.eStyleButton theStyle)
	{
		float aWidth = 110.0f;
		GUIStyle aStyle = KGFGUIUtility.GetStyleButton(theStyle);
		GUILayout.Button("hover",GetStyle(aStyle,eDemoStyle.eHover),GUILayout.Width(aWidth));
		GUILayout.Button("active",GetStyle(aStyle,eDemoStyle.eOnNormal),GUILayout.Width(aWidth));
		GUILayout.Space(10.0f);
	}
	
	private void RenderDemoBox(KGFGUIUtility.eStyleBox theStyle)
	{
		float aWidth = 110.0f;
		GUIStyle aStyle = KGFGUIUtility.GetStyleBox(theStyle);
		GUILayout.Box("hover",GetStyle(aStyle,eDemoStyle.eHover),GUILayout.Width(aWidth));
		GUILayout.Box("onnormal",GetStyle(aStyle,eDemoStyle.eOnNormal),GUILayout.Width(aWidth));
		GUILayout.Box("onhover",GetStyle(aStyle,eDemoStyle.eOnHover),GUILayout.Width(aWidth));
		GUILayout.Space(10.0f);
	}
	
	private string GetMarginString(GUIStyle theStyle)
	{
		string aReturnString = theStyle.margin.left + ","+theStyle.margin.right + ","+theStyle.margin.top + ","+theStyle.margin.bottom;
		return aReturnString;
	}
	
	private string GetPaddingString(GUIStyle theStyle)
	{
		string aReturnString = theStyle.padding.left + ","+theStyle.padding.right + ","+theStyle.padding.top + ","+theStyle.padding.bottom;
		return aReturnString;
	}
	
	private static void SetFontSize(int theHeight)
	{
		int aSize = 12;
		if(theHeight == 32)
		{
			aSize = 16;
		}
		
		int aWindowSize = 16;
		if(theHeight == 32)
		{
			aWindowSize = 20;
		}
		
		SetFontSizeForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eToggl), aSize, 0);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextField),aSize, 0);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextFieldLeft),aSize, 0);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextFieldRight),aSize, 0);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleTextArea(),aSize, 2);
		
		SetFontSizeForStyle(KGFGUIUtility.GetStyleWindow(),aWindowSize,3);
		
		SetFontSizeForStyle(KGFGUIUtility.GetStyleHorizontalSlider(),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleHorizontalSliderThumb(),aSize, 1);
		
		SetFontSizeForStyle(KGFGUIUtility.GetStyleVerticalSlider(),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleVerticalSliderThumb(),aSize, 1);
		
		SetFontSizeForStyle(KGFGUIUtility.GetStyleHorizontalScrollbar(),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleHorizontalScrollbarThumb(),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleHorizontalScrollbarLeftButton(),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleHorizontalScrollbarRightButton(),aSize, 1);
		
		SetFontSizeForStyle(KGFGUIUtility.GetStyleVerticalScrollbar(),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleVerticalScrollbarThumb(),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleVerticalScrollbarUpButton(),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleVerticalScrollbarDownButton(),aSize, 1);
		
		SetFontSizeForStyle(KGFGUIUtility.GetStyleScrollView(),aSize, 1);
		
		//custom styles
		SetFontSizeForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglStreched),aSize, 0);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglCompact),aSize, 0);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSuperCompact),aSize, 0);
		
		SetFontSizeForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioStreched),aSize, 0);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioCompact),aSize, 0);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact),aSize, 0);
		
		SetFontSizeForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSwitch),aSize, 0);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglBoolean),aSize, 0);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglArrow),aSize, 0);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglButton),aSize, 1);
		
		SetFontSizeForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButton),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonLeft),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonRight),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonTop),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonBottom),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonMiddle),aSize, 1);
		
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBox),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxInvisible),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxInteractive),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxLeft),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxLeftInteractive),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxRight),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxRightInteractive),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontal),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontalInteractive),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxTop),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxTopInteractive),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxBottom),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxBottomInteractive),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleVerticalInteractive),aSize, 1);
		
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDark),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkInteractive),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkLeft),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkLeftInteractive),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkRight),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkRightInteractive),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontal),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontalInteractive),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkTop),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkTopInteractive),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkBottom),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkBottomInteractive),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVertical),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVerticalInteractive),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDecorated),aSize, 1);
		
		SetFontSizeForStyle(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorVertical),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorVerticalFitInBox),aSize, 1);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorHorizontal),aSize, 1);
		
		SetFontSizeForStyle(KGFGUIUtility.GetStyleLabel(KGFGUIUtility.eStyleLabel.eLabel),aSize, 0);
		SetFontSizeForStyle(KGFGUIUtility.GetStyleLabel(KGFGUIUtility.eStyleLabel.eLabelFitIntoBox),aSize, 0);
	}
	
	private static void SetFontSizeForStyle(GUIStyle theStyle, int theFontSize, int theAlignment)
	{
		theStyle.fontSize = theFontSize;
		theStyle.clipping = TextClipping.Overflow;
		if(theAlignment == 0)
			theStyle.alignment = TextAnchor.MiddleLeft;
		else if(theAlignment == 1)
			theStyle.alignment = TextAnchor.MiddleCenter;
		else if(theAlignment == 2)
			theStyle.alignment = TextAnchor.UpperLeft;
		else if(theAlignment == 3)
			theStyle.alignment = TextAnchor.UpperCenter;
	}
	
	private static void SetPadding(int theHeight)
	{
		int aLeftPadding = 2;
		int aRightPadding = 6;
		int aTopPadding = 2;
		int aBottomPadding = 2;
		if(theHeight == 32)
		{
			aLeftPadding = 8;
			aRightPadding = 10;
			aTopPadding = 8;
			aBottomPadding = 8;
		}
		
		/*
		int aWindowPadding = 20;	//16 + 2 + 2
		if(theHeight == 32)
		{
			aWindowPadding = 48;		//32 + 8 + 8
		}
		 */
		
		SetPaddingForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eToggl),0,(int)KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eToggl).contentOffset.x,aTopPadding,aBottomPadding);
		SetPaddingForStyle(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextField),aLeftPadding,aRightPadding,aTopPadding,aBottomPadding);
		SetPaddingForStyle(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextFieldLeft),aLeftPadding,aRightPadding,aTopPadding,aBottomPadding);
		SetPaddingForStyle(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextFieldRight),aLeftPadding,aRightPadding,aTopPadding,aBottomPadding);
		SetPaddingForStyle(KGFGUIUtility.GetStyleTextArea(),aLeftPadding,aRightPadding,aTopPadding,aBottomPadding);
		
		//SetPaddingForStyle(itsStyleWindow,aLeftPadding,aRightPadding,aWindowPadding,aWindowPadding);
		
		SetPaddingForStyle(KGFGUIUtility.GetStyleHorizontalSlider(),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleHorizontalSliderThumb(),0,0,0,0);
		
		SetPaddingForStyle(KGFGUIUtility.GetStyleVerticalSlider(),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleVerticalSliderThumb(),0,0,0,0);
		
		SetPaddingForStyle(KGFGUIUtility.GetStyleHorizontalScrollbar(),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleHorizontalScrollbarThumb(),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleHorizontalScrollbarLeftButton(),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleHorizontalScrollbarRightButton(),0,0,0,0);
		
		SetPaddingForStyle(KGFGUIUtility.GetStyleVerticalScrollbar(),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleVerticalScrollbarThumb(),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleVerticalScrollbarUpButton(),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleVerticalScrollbarDownButton(),0,0,0,0);
		
		SetPaddingForStyle(KGFGUIUtility.GetStyleScrollView(),0,0,0,0);
		
		//custom styles
		SetPaddingForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglStreched),0,(int)KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglStreched).contentOffset.x+aRightPadding,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglCompact),0,(int)KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglCompact).contentOffset.x+aRightPadding,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSuperCompact),0,(int)KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSuperCompact).contentOffset.x+aRightPadding,0,0);
		
		SetPaddingForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioStreched),0,(int)KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioStreched).contentOffset.x+aRightPadding,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioCompact),0,(int)KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioCompact).contentOffset.x+aRightPadding,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact),0,(int)KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact).contentOffset.x+aRightPadding,0,0);
		
		SetPaddingForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSwitch),0,(int)KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSwitch).contentOffset.x+aRightPadding,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglBoolean),0,(int)KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglBoolean).contentOffset.x+aRightPadding,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglArrow),0,(int)KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglArrow).contentOffset.x+aRightPadding,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglButton),0,(int)KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglButton).contentOffset.x+aRightPadding,0,0);
		
		SetPaddingForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButton),aLeftPadding,aRightPadding,aTopPadding,aBottomPadding);
		SetPaddingForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonLeft),aLeftPadding,aRightPadding,aTopPadding,aBottomPadding);
		SetPaddingForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonRight),aLeftPadding,aRightPadding,aTopPadding,aBottomPadding);
		SetPaddingForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonTop),aLeftPadding,aRightPadding,aTopPadding,aBottomPadding);
		SetPaddingForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonBottom),aLeftPadding,aRightPadding,aTopPadding,aBottomPadding);
		SetPaddingForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonMiddle),aLeftPadding,aRightPadding,aTopPadding,aBottomPadding);
		
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBox),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxInvisible),aLeftPadding,aRightPadding,aTopPadding,aBottomPadding);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxInteractive),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxLeft),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxLeftInteractive),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxRight),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxRightInteractive),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontal),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontalInteractive),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxTop),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxTopInteractive),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxBottom),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxBottomInteractive),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleVerticalInteractive),0,0,0,0);
		
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDark),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkInteractive),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkLeft),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkLeftInteractive),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkRight),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkRightInteractive),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontal),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontalInteractive),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkTop),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkTopInteractive),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkBottom),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkBottomInteractive),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVertical),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVerticalInteractive),0,0,0,0);
		//SetPaddingForStyle(itsStyleBoxDecorated,aLeftPadding,aLeftPadding,aLeftPadding,aLeftPadding);
		
		SetPaddingForStyle(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorVertical),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorVerticalFitInBox),0,0,0,0);
		SetPaddingForStyle(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorHorizontal),0,0,0,0);
		
		SetPaddingForStyle(KGFGUIUtility.GetStyleLabel(KGFGUIUtility.eStyleLabel.eLabel),aTopPadding,aTopPadding,aTopPadding,aTopPadding);
		SetPaddingForStyle(KGFGUIUtility.GetStyleLabel(KGFGUIUtility.eStyleLabel.eLabelFitIntoBox),aTopPadding,aTopPadding,aTopPadding,aTopPadding);
	}
	
	private static void SetPaddingForStyle(GUIStyle theStyle, int thePaddingLeft, int thePaddingRight, int thePaddingTop, int thePaddingBottom)
	{
		theStyle.padding.left = thePaddingLeft;
		theStyle.padding.right = thePaddingRight;
		theStyle.padding.top = thePaddingTop;
		theStyle.padding.bottom = thePaddingBottom;
	}
	
	public static void SetMargin(int theHeight)
	{
		int aLeftMargin = 2;
		int aRightMargin = 2;
		int aTopMargin = 2;
		int aBottomMargin = 2;
		if(theHeight == 32)
		{
			aLeftMargin = 8;
			aRightMargin = 8;
			aTopMargin = 8;
			aBottomMargin = 8;
		}
		
		SetMarginForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eToggl),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		SetMarginForStyle(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextField),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		SetMarginForStyle(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextFieldLeft),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextFieldRight),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleTextArea(),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		
		
		SetMarginForStyle(KGFGUIUtility.GetStyleWindow(),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		
		SetMarginForStyle(KGFGUIUtility.GetStyleHorizontalSlider(),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		SetMarginForStyle(KGFGUIUtility.GetStyleHorizontalSliderThumb(),0,0,0,0);
		
		SetMarginForStyle(KGFGUIUtility.GetStyleVerticalSlider(),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		SetMarginForStyle(KGFGUIUtility.GetStyleVerticalSliderThumb(),0,0,0,0);
		
		SetMarginForStyle(KGFGUIUtility.GetStyleHorizontalScrollbar(),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleHorizontalScrollbarThumb(),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleHorizontalScrollbarLeftButton(),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleHorizontalScrollbarRightButton(),0,0,0,0);
		
		SetMarginForStyle(KGFGUIUtility.GetStyleVerticalScrollbar(),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleVerticalScrollbarThumb(),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleVerticalScrollbarUpButton(),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleVerticalScrollbarDownButton(),0,0,0,0);
		
		SetMarginForStyle(KGFGUIUtility.GetStyleScrollView(),0,0,0,0);
		
		//custom styles
		SetMarginForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglStreched),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		SetMarginForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglCompact),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		SetMarginForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSuperCompact),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		
		SetMarginForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioStreched),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		SetMarginForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioCompact),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		SetMarginForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		
		SetMarginForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSwitch),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		SetMarginForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglBoolean),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		SetMarginForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglArrow),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		SetMarginForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglButton),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		
		SetMarginForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButton),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		SetMarginForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonLeft),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonRight),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonTop),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonBottom),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonMiddle),0,0,0,0);
		
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBox),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxInvisible),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxInteractive),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxLeft),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxLeftInteractive),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxRight),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxRightInteractive),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontal),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontalInteractive),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxTop),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxTopInteractive),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxBottom),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxBottomInteractive),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleVerticalInteractive),0,0,0,0);
		
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDark),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkInteractive),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkLeft),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkLeftInteractive),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkRight),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkRightInteractive),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontal),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontalInteractive),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkTop),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkTopInteractive),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkBottom),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkBottomInteractive),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVertical),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVerticalInteractive),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDecorated),0,0,aTopMargin,aBottomMargin);
		
		SetMarginForStyle(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorVertical),0,0,aTopMargin,aBottomMargin);
		SetMarginForStyle(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorVerticalFitInBox),0,0,0,0);
		SetMarginForStyle(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorHorizontal),0,0,0,0);
		
		SetMarginForStyle(KGFGUIUtility.GetStyleLabel(KGFGUIUtility.eStyleLabel.eLabel),aLeftMargin,aRightMargin,aTopMargin,aBottomMargin);
		SetMarginForStyle(KGFGUIUtility.GetStyleLabel(KGFGUIUtility.eStyleLabel.eLabelFitIntoBox),aLeftMargin,aRightMargin,0,0);
	}
	
	public static void SetHeight(int theHeight)
	{
		int aSize = 16;
		if(theHeight == 32)
			aSize = 32;
		
		int aSeparatorSize = 16;
		
		SetHeight(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eToggl),0,aSize,false,false);
		SetHeight(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextField),0,aSize,true,false);
		SetHeight(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextFieldLeft),0,aSize,true,false);
		SetHeight(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextFieldRight),0,aSize,true,false);
		SetHeight(KGFGUIUtility.GetStyleTextArea(),0,0,true,true);
		
		SetHeight(KGFGUIUtility.GetStyleWindow(),0,0,false,false);
		
		SetHeight(KGFGUIUtility.GetStyleHorizontalSlider(),0,aSize,true,false);
		SetHeight(KGFGUIUtility.GetStyleHorizontalSliderThumb(),aSize,aSize,false,false);
		
		SetHeight(KGFGUIUtility.GetStyleVerticalSlider(),aSize,0,false,true);
		SetHeight(KGFGUIUtility.GetStyleVerticalSliderThumb(),aSize,aSize,false,false);
		
		SetHeight(KGFGUIUtility.GetStyleHorizontalScrollbar(),0,aSize,true,false);
		SetHeight(KGFGUIUtility.GetStyleHorizontalScrollbarThumb(),0,aSize,false,false);
		SetHeight(KGFGUIUtility.GetStyleHorizontalScrollbarLeftButton(),aSize,aSize,false,false);
		SetHeight(KGFGUIUtility.GetStyleHorizontalScrollbarRightButton(),aSize,aSize,false,false);
		
		SetHeight(KGFGUIUtility.GetStyleVerticalScrollbar(),aSize,0,false,true);
		SetHeight(KGFGUIUtility.GetStyleVerticalScrollbarThumb(),aSize,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleVerticalScrollbarUpButton(),aSize,aSize,false,false);
		SetHeight(KGFGUIUtility.GetStyleVerticalScrollbarDownButton(),aSize,aSize,false,false);
		
		SetHeight(KGFGUIUtility.GetStyleScrollView(),0,0,true,true);
		
		//custom styles
		SetHeight(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglStreched),0,aSize,true,false);
		SetHeight(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglCompact),0,aSize,true,false);
		SetHeight(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSuperCompact),0,aSize,true,false);
		
		SetHeight(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioStreched),0,aSize,true,false);
		SetHeight(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioCompact),0,aSize,true,false);
		SetHeight(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact),0,aSize,true,false);
		
		SetHeight(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSwitch),0,aSize,true,false);
		SetHeight(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglBoolean),0,aSize,true,false);
		SetHeight(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglArrow),0,aSize,true,false);
		SetHeight(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglButton),0,aSize,true,false);
		
		SetHeight(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButton),0,aSize,true,false);
		SetHeight(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonLeft),0,aSize,true,false);
		SetHeight(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonRight),0,aSize,true,false);
		SetHeight(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonTop),0,aSize,true,false);
		SetHeight(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonBottom),0,aSize,true,false);
		SetHeight(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonMiddle),0,aSize,true,false);
		
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBox),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxInvisible),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxInteractive),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxLeft),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxLeftInteractive),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxRight),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxRightInteractive),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontal),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontalInteractive),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxTop),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxTopInteractive),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxBottom),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxBottomInteractive),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleVerticalInteractive),0,0,false,false);
		
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDark),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkInteractive),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkLeft),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkLeftInteractive),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkRight),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkRightInteractive),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontal),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontalInteractive),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkTop),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkTopInteractive),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkBottom),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkBottomInteractive),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVertical),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVerticalInteractive),0,0,false,false);
		SetHeight(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDecorated),0,0,false,false);
		
		SetHeight(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorVertical),aSeparatorSize,0,false,true);
		SetHeight(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorVerticalFitInBox),aSeparatorSize,0,false,true);
		SetHeight(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorHorizontal),0,aSeparatorSize,true,false);
		
		SetHeight(KGFGUIUtility.GetStyleLabel(KGFGUIUtility.eStyleLabel.eLabel),0,aSize,false,false);
		SetHeight(KGFGUIUtility.GetStyleLabel(KGFGUIUtility.eStyleLabel.eLabelFitIntoBox),0,theHeight,false,false);
	}
	
	public static void SetContentOffset(int theHeight)
	{
		int aContentOffset = 16;
		if(theHeight == 32)
			aContentOffset = 36;
		
		/*
		int aContentOffsetWindow = -20;
		if(theHeight == 32)
			aContentOffsetWindow = -48;
		 */
		
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eToggl),aContentOffset,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextField),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextFieldLeft),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextFieldRight),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleTextArea(),0,0);
		
		//SetContentOffsetForStyle(KGFGUIUtility.GetStyleWindow(),0,aContentOffsetWindow);
		
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleHorizontalSlider(),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleHorizontalSliderThumb(),0,0);
		
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleVerticalSlider(),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleVerticalSliderThumb(),0,0);
		
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleHorizontalScrollbar(),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleHorizontalScrollbarThumb(),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleHorizontalScrollbarLeftButton(),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleHorizontalScrollbarRightButton(),0,0);
		
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleVerticalScrollbar(),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleVerticalScrollbarThumb(),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleVerticalScrollbarUpButton(),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleVerticalScrollbarDownButton(),0,0);
		
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleScrollView(),0,0);
		
		//custom styles
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglStreched),aContentOffset,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglCompact),aContentOffset,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSuperCompact),aContentOffset,0);
		
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioStreched),aContentOffset,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioCompact),aContentOffset,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact),aContentOffset,0);
		
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSwitch),aContentOffset,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglBoolean),aContentOffset,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglArrow),aContentOffset,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglButton),0,0);
		
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButton),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonLeft),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonRight),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonTop),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonBottom),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonMiddle),0,0);
		
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBox),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxInvisible),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxInteractive),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxLeft),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxLeftInteractive),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxRight),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxRightInteractive),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontal),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontalInteractive),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxTop),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxTopInteractive),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxBottom),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxBottomInteractive),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleVerticalInteractive),0,0);
		
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDark),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkInteractive),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkLeft),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkLeftInteractive),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkRight),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkRightInteractive),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontal),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontalInteractive),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkTop),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkTopInteractive),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkBottom),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkBottomInteractive),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVertical),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVerticalInteractive),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDecorated),0,0);
		
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorVertical),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorVerticalFitInBox),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorHorizontal),0,0);
		
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleLabel(KGFGUIUtility.eStyleLabel.eLabel),0,0);
		SetContentOffsetForStyle(KGFGUIUtility.GetStyleLabel(KGFGUIUtility.eStyleLabel.eLabelFitIntoBox),0,0);
	}
	
	public static void SetColorsForStyle(GUIStyle theStyle, Color[] theColors)
	{
		theStyle.normal.textColor = theColors[0];
		theStyle.hover.textColor = theColors[1];
		theStyle.active.textColor = theColors[2];
		theStyle.focused.textColor = theColors[3];
		theStyle.onNormal.textColor = theColors[4];
		theStyle.onHover.textColor = theColors[5];
		theStyle.onActive.textColor = theColors[6];
		theStyle.onFocused.textColor = theColors[7];
	}
	
	public static void SetHeight(GUIStyle theStyle, int theWidth, int theHeight,bool theStretchWidth, bool theStretchHeight)
	{
		theStyle.fixedWidth = theWidth;
		theStyle.fixedHeight = theHeight;
		if(theStretchWidth == true)
			theStyle.stretchWidth = true;
		else
			theStyle.stretchWidth = false;
		if(theStretchHeight == true)
			theStyle.stretchHeight = true;
		else
			theStyle.stretchHeight = false;
	}
	
	public static void SetMarginForStyle(GUIStyle theStyle, int theMarginLeft, int theMarginRight, int theMarginTop, int theMarginBottom)
	{
		theStyle.margin.left = theMarginLeft;
		theStyle.margin.right = theMarginRight;
		theStyle.margin.top = theMarginTop;
		theStyle.margin.bottom = theMarginBottom;
	}
	
	public static void SetContentOffsetForStyle(GUIStyle theStyle, int theX, int theY)
	{
		theStyle.contentOffset = new Vector2(theX,theY);
	}
	
	public static void SetColors(Color[] theColors, int theHeight)
	{
		SetColorsForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eToggl),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextField),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextFieldLeft),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleTextField(KGFGUIUtility.eStyleTextField.eTextFieldRight),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleTextArea(),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleWindow(),theColors);
		
		SetColorsForStyle(KGFGUIUtility.GetStyleHorizontalSlider(),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleHorizontalSliderThumb(),theColors);
		
		SetColorsForStyle(KGFGUIUtility.GetStyleVerticalSlider(),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleVerticalSliderThumb(),theColors);
		
		SetColorsForStyle(KGFGUIUtility.GetStyleHorizontalScrollbar(),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleHorizontalScrollbarThumb(),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleHorizontalScrollbarLeftButton(),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleHorizontalScrollbarRightButton(),theColors);
		
		SetColorsForStyle(KGFGUIUtility.GetStyleVerticalScrollbar(),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleVerticalScrollbarThumb(),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleVerticalScrollbarUpButton(),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleVerticalScrollbarDownButton(),theColors);
		
		SetColorsForStyle(KGFGUIUtility.GetStyleScrollView(),theColors);
		
		//custom styles
		SetColorsForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglStreched),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglCompact),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSuperCompact),theColors);
		
		SetColorsForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioStreched),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioCompact),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact),theColors);
		
		SetColorsForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglSwitch),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglBoolean),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglArrow),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglButton),theColors);
		
		SetColorsForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButton),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonLeft),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonRight),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonTop),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonBottom),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButtonMiddle),theColors);
		
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBox),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxInvisible),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxInteractive),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxLeft),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxLeftInteractive),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxRight),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxRightInteractive),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontal),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleHorizontalInteractive),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxTop),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxTopInteractive),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxBottom),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxBottomInteractive),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxMiddleVerticalInteractive),theColors);
		
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDark),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkInteractive),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkLeft),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkLeftInteractive),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkRight),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkRightInteractive),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontal),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleHorizontalInteractive),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkTop),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkTopInteractive),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkBottom),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkBottomInteractive),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVertical),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVerticalInteractive),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDecorated),theColors);
		
		SetColorsForStyle(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorVertical),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorVerticalFitInBox),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleSeparator(KGFGUIUtility.eStyleSeparator.eSeparatorHorizontal),theColors);
		
		SetColorsForStyle(KGFGUIUtility.GetStyleLabel(KGFGUIUtility.eStyleLabel.eLabel),theColors);
		SetColorsForStyle(KGFGUIUtility.GetStyleLabel(KGFGUIUtility.eStyleLabel.eLabelFitIntoBox),theColors);
	}
}