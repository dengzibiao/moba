// <author>Christoph Hausjell</author>
// <email>christoph.hausjell@kolmich.at</email>
// <date>2011-03-01</date>
// <summary>this file contasins the class KGFGUIUtility and some enums to switch between diffent GUI styles of the controls</summary>

using System.Runtime.Serialization.Formatters;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// contains methods for easier usage of the Kolmich Game Framework skins.
/// </summary>
/// <remarks>
/// This class defines methods and enumerations to use the KOLMICH Game Framework skins as easy as possible.
/// </remarks>
public class KGFGUIUtility
{
	#region internal enums
	
	/// <summary>
	/// enumeration of all possible button styles
	/// </summary>
	/// <remarks>
	/// Buttons can be used to get userinput on gui. By using button styles with direction, it is possible to group them horizontal or vertical.
	/// </remarks>
	public enum eStyleButton
	{
		eButton,
		eButtonLeft,
		eButtonRight,
		eButtonTop,
		eButtonBottom,
		eButtonMiddle
	}
	
	/// <summary>
	/// enumeration of all possible toggle styles
	/// </summary>
	/// <remarks>
	/// Toggle buttons can be used to allow the user to switch between an on and off state.
	/// </remarks>
	public enum eStyleToggl
	{
		eToggl,
		eTogglStreched,
		eTogglCompact,
		eTogglSuperCompact,
		eTogglRadioStreched,
		eTogglRadioCompact,
		eTogglRadioSuperCompact,
		eTogglSwitch,
		eTogglBoolean,
		eTogglArrow,
		eTogglButton
	}
	
	/// <summary>
	/// enumeration of all possible textfield styles
	/// </summary>
	/// <remarks>
	/// Textfields can be used to get the user`s keyborad input. By using the textfield styles with direction it is possible to group them horizontal or vertical.
	/// </remarks>
	public enum eStyleTextField
	{
		eTextField,
		eTextFieldLeft,
		eTextFieldRight
	}
	
	/// <summary>
	/// enumeration of all possible box styles
	/// </summary>
	/// <remarks>
	/// Boxes can be used to group an align controls. The direction  in the boxname gives inforamtion about the position. this means an eBoxTop style has no bottom padding.
	/// </remarks>
	public enum eStyleBox
	{
		eBox,
		eBoxInvisible,
		eBoxInteractive,
		eBoxLeft,
		eBoxLeftInteractive,
		eBoxRight,
		eBoxRightInteractive,
		eBoxMiddleHorizontal,
		eBoxMiddleHorizontalInteractive,
		eBoxTop,
		eBoxTopInteractive,
		eBoxMiddleVertical,
		eBoxMiddleVerticalInteractive,
		eBoxBottom,
		eBoxBottomInteractive,
		eBoxDark,
		eBoxDarkInteractive,
		eBoxDarkLeft,
		eBoxDarkLeftInteractive,
		eBoxDarkRight,
		eBoxDarkRightInteractive,
		eBoxDarkMiddleHorizontal,
		eBoxDarkMiddleHorizontalInteractive,
		eBoxDarkTop,
		eBoxDarkTopInteractive,
		eBoxDarkBottom,
		eBoxDarkBottomInteractive,
		eBoxDarkMiddleVertical,
		eBoxDarkMiddleVerticalInteractive,
		eBoxDecorated
	}
	
	/// <summary>
	/// enumeration of all possible seperator styles
	/// </summary>
	/// <remarks>
	/// Seperators can be used to create space between horizontal or vertical oriented gui objects.
	/// </remarks>
	public enum eStyleSeparator
	{
		eSeparatorHorizontal,
		eSeparatorVertical,
		eSeparatorVerticalFitInBox,
	}
	
	/// <summary>
	/// enumeration of all possible label styles
	/// </summary>
	/// <remarks>
	/// Labels can be used to show text or icons. By using the style eLabelFitIntoBox the label will not resize the parent control.
	/// </remarks>
	public enum eStyleLabel
	{
		eLabel,
		eLabelFitIntoBox
	}
	
	/// <summary>
	/// enumeration of all possible image styles
	/// </summary>
	/// <remarks>
	/// Images can be used tho show images. Padding and margin of images are always 0. The FreeSize image has height and with 0 so it can be resized.
	/// </remarks>
	public enum eStyleImage
	{
		eImage,
		eImageFitIntoBox
	}
	
	/// <summary>
	/// enumeration of all possible cursor states
	/// </summary>
	/// <remarks>
	/// A cursor state represnents the clicked button direction in a cursor gui element.
	/// </remarks>
	public enum eCursorState
	{
		eUp,
		eRight,
		eDown,
		eLeft,
		eCenter,
		eNone
	}
	
	#endregion
	
	#region member
	
	private static string itsDefaultGuiSkinPath = "KGFSkins/default/skins/skin_default_16";
	private static bool itsResetPath = false;
	private static GUISkin itsSkin = null;
	private static Texture2D itsIcon = null;
	private static Texture2D itsKGFCopyright = null;
	private static Texture2D itsIconHelp = null;
	
	#region default styles
	
	private static GUIStyle itsStyleToggle = null;
	private static GUIStyle	itsStyleTextField = null;
	private static GUIStyle	itsStyleTextFieldLeft = null;
	private static GUIStyle	itsStyleTextFieldRight = null;
	
	private static GUIStyle	itsStyleTextArea  = null;
	private static GUIStyle itsStyleWindow = null;
	
	private static GUIStyle itsStyleHorizontalSlider = null;
	private static GUIStyle itsStyleHorizontalSliderThumb = null;
	
	private static GUIStyle itsStyleVerticalSlider = null;
	private static GUIStyle itsStyleVerticalSliderThumb = null;
	
	private static GUIStyle itsStyleHorizontalScrollbar = null;
	private static GUIStyle itsStyleHorizontalScrollbarThumb = null;
	private static GUIStyle itsStyleHorizontalScrollbarLeftButton = null;
	private static GUIStyle itsStyleHorizontalScrollbarRightButton = null;
	
	private static GUIStyle itsStyleVerticalScrollbar = null;
	private static GUIStyle itsStyleVerticalScrollbarThumb = null;
	private static GUIStyle itsStyleVerticalScrollbarUpButton = null;
	private static GUIStyle itsStyleVerticalScrollbarDownButton = null;
	
	private static GUIStyle itsStyleScrollView = null;
	private static GUIStyle itsStyleMinimap = null;
	private static GUIStyle itsStyleMinimapButton = null;
	
	#endregion
	
	#region custom styles

	private static GUIStyle itsStyleToggleStreched = null;
	private static GUIStyle itsStyleToggleCompact = null;
	private static GUIStyle itsStyleToggleSuperCompact = null;
	
	private static GUIStyle itsStyleToggleRadioStreched = null;
	private static GUIStyle itsStyleToggleRadioCompact = null;
	private static GUIStyle itsStyleToggleRadioSuperCompact = null;
	
	private static GUIStyle itsStyleToggleSwitch = null;
	private static GUIStyle itsStyleToggleBoolean = null;
	private static GUIStyle itsStyleToggleArrow = null;
	private static GUIStyle itsStyleToggleButton = null;
	
	private static GUIStyle itsStyleButton = null;
	private static GUIStyle itsStyleButtonLeft = null;
	private static GUIStyle itsStyleButtonRight = null;
	private static GUIStyle itsStyleButtonTop = null;
	private static GUIStyle itsStyleButtonBottom = null;
	private static GUIStyle itsStyleButtonMiddle = null;
	
	private static GUIStyle itsStyleBox = null;
	private static GUIStyle itsStyleBoxInvisible = null;
	private static GUIStyle itsStyleBoxInteractive = null;
	private static GUIStyle itsStyleBoxLeft = null;
	private static GUIStyle itsStyleBoxLeftInteractive = null;
	private static GUIStyle itsStyleBoxRight = null;
	private static GUIStyle itsStyleBoxRightInteractive = null;
	private static GUIStyle itsStyleBoxMiddleHorizontal = null;
	private static GUIStyle itsStyleBoxMiddleHorizontalInteractive = null;
	private static GUIStyle itsStyleBoxTop = null;
	private static GUIStyle itsStyleBoxTopInteractive = null;
	private static GUIStyle itsStyleBoxBottom = null;
	private static GUIStyle itsStyleBoxBottomInteractive = null;
	private static GUIStyle itsStyleBoxMiddleVertical = null;
	private static GUIStyle itsStyleBoxMiddleVerticalInteractive = null;
	
	private static GUIStyle itsStyleBoxDark = null;
	private static GUIStyle itsStyleBoxDarkInteractive = null;
	private static GUIStyle itsStyleBoxDarkLeft = null;
	private static GUIStyle itsStyleBoxDarkLeftInteractive = null;
	private static GUIStyle itsStyleBoxDarkRight = null;
	private static GUIStyle itsStyleBoxDarkRightInteractive = null;
	private static GUIStyle itsStyleBoxDarkMiddleHorizontal = null;
	private static GUIStyle itsStyleBoxDarkMiddleHorizontalInteractive = null;
	private static GUIStyle itsStyleBoxDarkTop = null;
	private static GUIStyle itsStyleBoxDarkTopInteractive = null;
	private static GUIStyle itsStyleBoxDarkBottom = null;
	private static GUIStyle itsStyleBoxDarkBottomInteractive = null;
	private static GUIStyle itsStyleBoxDarkMiddleVertical = null;
	private static GUIStyle itsStyleBoxDarkMiddleVerticalInteractive = null;
	private static GUIStyle itsStyleBoxDecorated = null;
	
	private static GUIStyle itsStyleSeparatorVertical = null;
	private static GUIStyle itsStyleSeparatorVerticalFitInBox = null;
	private static GUIStyle itsStyleSeparatorHorizontal = null;
	
	private static GUIStyle itsStyleLabel = null;
	private static GUIStyle itsStyleLabelFitInToBox = null;
	
	private static GUIStyle itsStyleTable = null;
	private static GUIStyle itsStyleTableHeadingRow = null;
	private static GUIStyle itsStyleTableHeadingCell = null;
	private static GUIStyle itsStyleTableRow = null;
	private static GUIStyle itsStyleTableRowCell = null;
	
	private static GUIStyle itsStyleCursor = null;
	#endregion
	
	#endregion
	
	#region editor colors
	
	/// <summary>
	/// predefined default color which can be used in editor classes
	/// </summary>
	public static Color itsEditorColorContent
	{
		get { return new Color(0.1f, 0.1f, 0.1f); }
	}
	
	/// <summary>
	/// predefined default color which can be used in editor classes
	/// </summary>
	public static Color itsEditorColorTitle
	{
		get { return new Color(0.1f, 0.1f, 0.1f); }
	}
	
	/// <summary>
	/// predefined default color which can be used in editor classes
	/// </summary>
	public static Color itsEditorDocumentation
	{
		get { return new Color(0.74f, 0.79f, 0.64f); }
	}
	
	/// <summary>
	/// predefined default color which can be used in editor classes
	/// </summary>
	public static Color itsEditorColorDefault
	{
		get { return new Color(1.0f, 1.0f, 1.0f); }
		//get { return new Color(0.84f, 0.89f, 0.74f); }
	}
	
	/// <summary>
	/// predefined default color which should be used in editor classes to display information, or notes
	/// </summary>
	public static Color itsEditorColorInfo
	{
		get { return new Color(1.0f,1.0f,1.0f); }
	}
	
	/// <summary>
	/// predefined default color which should be used in editor classes to display warnings
	/// </summary>
	public static Color itsEditorColorWarning
	{
		get { return new Color(1f, 1f, 0.0f); }
	}
	
	/// <summary>
	/// predefined default color which should be used in editor classes to display errors
	/// </summary>
	public static Color itsEditorColorError
	{
		get { return new Color(0.9f, 0.5f, 0.5f); }
	}
	
	#endregion
	
	#region methods
	
	#region getter methods
	
	/// <summary>
	/// returns the height of the button control which is the current skin heigt. -> can be 16, 32, 64
	/// </summary>
	/// <returns>returns the current height of the selected skin.</returns>
	public static float GetSkinHeight()
	{
		if(itsStyleButton != null)
		{
			return itsStyleButton.fixedHeight;
		}
		else
		{
			return 16.0f;
		}
	}
	
	/// <summary>
	/// returns the current selected skin.
	/// </summary>
	/// <returns>returns the current selected skin. Returns null if no skin is selected.</returns>
	public static GUISkin GetSkin()
	{
		if(itsSkin != null)
		{
			return itsSkin;
		}
		
		return null;
	}
	
	public static Texture2D GetLogo()
	{
		if(itsIcon == null)
		{
			itsIcon = Resources.Load("KGFCore/textures/logo") as Texture2D;
		}
		
		return itsIcon;
	}
	
	public static Texture2D GetHelpIcon()
	{
		if(itsIconHelp == null)
		{
			itsIconHelp = Resources.Load("KGFCore/textures/help") as Texture2D;
		}
		
		return itsIconHelp;
	}
	
	public static Texture2D GetKGFCopyright()
	{
		if(itsKGFCopyright == null)
		{
			itsKGFCopyright = Resources.Load("KGFCore/textures/kgf_copyright_512x256") as Texture2D;
		}
		
		return itsKGFCopyright;
	}
	
	/// <summary>
	/// returns the guistyle for the requested toggle type
	/// </summary>
	/// <param name="theTogglStyle">the type of requested toggle style</param>
	/// <returns>returns the requested toggle style. Returns the default toggle style if no custom style was found.</returns>
	public static GUIStyle GetStyleToggl(eStyleToggl theTogglStyle)
	{
		if(theTogglStyle == eStyleToggl.eTogglStreched && itsStyleToggleStreched != null)
		{
			return itsStyleToggleStreched;
		}
		else if(theTogglStyle == eStyleToggl.eTogglCompact && itsStyleToggleCompact != null)
		{
			return itsStyleToggleCompact;
		}
		else if(theTogglStyle == eStyleToggl.eTogglSuperCompact && itsStyleToggleSuperCompact != null)
		{
			return itsStyleToggleSuperCompact;
		}
		else if(theTogglStyle == eStyleToggl.eTogglRadioStreched && itsStyleToggleRadioStreched != null)
		{
			return itsStyleToggleRadioStreched;
		}
		else if(theTogglStyle == eStyleToggl.eTogglRadioCompact && itsStyleToggleRadioCompact != null)
		{
			return itsStyleToggleRadioCompact;
		}
		else if(theTogglStyle == eStyleToggl.eTogglRadioSuperCompact && itsStyleToggleRadioSuperCompact != null)
		{
			return itsStyleToggleRadioSuperCompact;
		}
		else if(theTogglStyle == eStyleToggl.eTogglSwitch && itsStyleToggleSwitch != null)
		{
			return itsStyleToggleSwitch;
		}
		else if(theTogglStyle == eStyleToggl.eTogglBoolean && itsStyleToggleBoolean != null)
		{
			return itsStyleToggleBoolean;
		}
		else if(theTogglStyle == eStyleToggl.eTogglArrow && itsStyleToggleArrow != null)
		{
			return itsStyleToggleArrow;
		}
		else if(theTogglStyle == eStyleToggl.eTogglButton && itsStyleToggleButton != null)
		{
			return itsStyleToggleButton;
		}
		
		if(itsStyleToggle != null)
		{
			return itsStyleToggle;
		}
		else
		{
			return GUI.skin.toggle;
		}
	}
	
	/// <summary>
	/// returns the guistyle for a textfield
	/// </summary>
	/// <param name="theStyleTextField">the type of textfield style</param>
	/// <returns>returns the requested textfield style. Returns the default textfield style if no custom style was found.</returns>
	public static GUIStyle GetStyleTextField(eStyleTextField theStyleTextField)
	{
		if(theStyleTextField == eStyleTextField.eTextField && itsStyleTextField != null)
		{
			return itsStyleTextField;
		}
		else if(theStyleTextField == eStyleTextField.eTextFieldLeft && itsStyleTextFieldLeft != null)
		{
			return itsStyleTextFieldLeft;
		}
		else if(theStyleTextField == eStyleTextField.eTextFieldRight && itsStyleTextFieldRight != null)
		{
			return itsStyleTextFieldRight;
		}
		
		return GUI.skin.textField;
	}
	
	/// <summary>
	/// returns the guistyle for a textarea
	/// </summary>
	/// <returns>returns the textarea style. Returns the default textarea style if no custom style was found.</returns>
	public static GUIStyle GetStyleTextArea()
	{
		if(itsStyleTextArea != null)
		{
			return itsStyleTextArea;
		}
		
		return GUI.skin.textArea;
	}
	
	#region horizontal slider
	
	/// <summary>
	/// returns the guistyle for a horizontal slider
	/// </summary>
	/// <returns>returns the horizontal slider style. Returns the default horizontal slider style if no custom style was found.</returns>
	public static GUIStyle GetStyleHorizontalSlider()
	{
		if(itsStyleHorizontalSlider != null)
		{
			return itsStyleHorizontalSlider;
		}
		
		return GUI.skin.horizontalSlider;
	}
	
	/// <summary>
	/// returns the guistyle for a horizontalt slider thumb
	/// </summary>
	/// <returns>returns the horizontal slider thumb style. Returns the default horizontal slider thumb style if no custom style was found.</returns>
	public static GUIStyle GetStyleHorizontalSliderThumb()
	{
		if(itsStyleHorizontalSliderThumb != null)
		{
			return itsStyleHorizontalSliderThumb;
		}
		
		return GUI.skin.horizontalSliderThumb;
	}
	
	#endregion
	
	#region horizontal scrollbar
	
	/// <summary>
	/// returns the guistyle for a horizontal scrollbar
	/// </summary>
	/// <returns>returns the horizontal scrollbar style. Returns the default horizontal scrollbar style if no custom style was found.</returns>
	public static GUIStyle GetStyleHorizontalScrollbar()
	{
		if(itsStyleHorizontalScrollbar != null)
		{
			return itsStyleHorizontalScrollbar;
		}
		
		return GUI.skin.horizontalScrollbar;
	}
	
	/// <summary>
	/// returns the guistyle for a horizontal scrollbar thumb
	/// </summary>
	/// <returns>returns the horizontal scrollbar thumb style. Returns the default horizontal scrollbar thumb style if no custom style was found.</returns>
	public static GUIStyle GetStyleHorizontalScrollbarThumb()
	{
		if(itsStyleHorizontalScrollbarThumb != null)
		{
			return itsStyleHorizontalScrollbarThumb;
		}
		
		return GUI.skin.horizontalScrollbarThumb;
	}
	
	/// <summary>
	/// returns the guistyle for the left button of a horizontal scrollbar
	/// </summary>
	/// <returns>returns the horizontal scrollbar left button style. Returns the default horizontal scrollbar left button style if no custom style was found.</returns>
	public static GUIStyle GetStyleHorizontalScrollbarLeftButton()
	{
		if(itsStyleHorizontalScrollbarLeftButton != null)
		{
			return itsStyleHorizontalScrollbarLeftButton;
		}
		
		return GUI.skin.horizontalScrollbarLeftButton;
	}
	
	/// <summary>
	/// returns the guistyle for the right button of a horizontal scrollbar
	/// </summary>
	/// <returns>returns the horizontal scrollbar right button style. Returns the default horizontal scrollbar right button style if no custom style was found.</returns>
	public static GUIStyle GetStyleHorizontalScrollbarRightButton()
	{
		if(itsStyleHorizontalScrollbarRightButton != null)
		{
			return itsStyleHorizontalScrollbarRightButton;
		}
		
		return GUI.skin.horizontalScrollbarRightButton;
	}
	
	#endregion
	
	#region vertical slider
	
	/// <summary>
	/// returns the guistyle for a vertical slider
	/// </summary>
	/// <returns>returns the vertical slider style. Returns the default vertical slider style if no custom style was found.</returns>
	public static GUIStyle GetStyleVerticalSlider()
	{
		if(itsStyleVerticalSlider != null)
		{
			return itsStyleVerticalSlider;
		}
		
		return GUI.skin.verticalSlider;
	}
	
	/// <summary>
	/// returns the guistyle for a horizontalt slider thumb
	/// </summary>
	/// <returns>returns the vertical slider thumb style. Returns the default vertical slider thumb style if no custom style was found.</returns>
	public static GUIStyle GetStyleVerticalSliderThumb()
	{
		if(itsStyleVerticalSliderThumb != null)
		{
			return itsStyleVerticalSliderThumb;
		}
		
		return GUI.skin.verticalSliderThumb;
	}
	
	#endregion
	
	#region vertical scrollbar
	
	/// <summary>
	/// returns the guistyle for a vertival scrollbar
	/// </summary>
	/// <returns>returns the vertival scrollbar style. Returns the default vertival scrollbar style if no custom style was found.</returns>
	public static GUIStyle GetStyleVerticalScrollbar()
	{
		if(itsStyleVerticalScrollbar != null)
		{
			return itsStyleVerticalScrollbar;
		}
		
		return GUI.skin.verticalScrollbar;
	}
	
	/// <summary>
	/// returns the guistyle for a vertical scrollbar thumb
	/// </summary>
	/// <returns>returns the vertical scrollbar thumb style. Returns the default vertical scrollbar thumb style if no custom style was found.</returns>
	public static GUIStyle GetStyleVerticalScrollbarThumb()
	{
		if(itsStyleVerticalScrollbarThumb != null)
		{
			return itsStyleVerticalScrollbarThumb;
		}
		
		return GUI.skin.verticalScrollbarThumb;
	}
	
	/// <summary>
	/// returns the guistyle for the up button of a vertical scrollbar
	/// </summary>
	/// <returns>returns the vertical scrollbar up button style. Returns the default vertical scrollbar up button style if no custom style was found.</returns>
	public static GUIStyle GetStyleVerticalScrollbarUpButton()
	{
		if(itsStyleVerticalScrollbarUpButton != null)
		{
			return itsStyleVerticalScrollbarUpButton;
		}
		
		return GUI.skin.verticalScrollbarUpButton;
	}
	
	/// <summary>
	/// returns the guistyle for the down button of a vertical scrollbar
	/// </summary>
	/// <returns>returns the vertical scrollbar down button style. Returns the default vertical scrollbar down button style if no custom style was found.</returns>
	public static GUIStyle GetStyleVerticalScrollbarDownButton()
	{
		if(itsStyleVerticalScrollbarDownButton != null)
		{
			return itsStyleVerticalScrollbarDownButton;
		}
		
		return GUI.skin.verticalScrollbarDownButton;
	}
	
	#endregion
	
	#region minimap styles
	
	/// <summary>
	/// returns the guistyle for a scrollview
	/// </summary>
	/// <returns>returns the scrollview style. Returns the default scrollview style if no custom style was found.</returns>
	public static GUIStyle GetStyleScrollView()
	{
		if(itsStyleScrollView != null)
		{
			return itsStyleScrollView;
		}
		
		return GUI.skin.scrollView;
	}
	
	/// <summary>
	/// returns the guistyle for a minimap border
	/// </summary>
	/// <returns>returns the minimap border style. Returns the default box style if no custom style was found.</returns>
	public static GUIStyle GetStyleMinimapBorder()
	{
		if(itsStyleMinimap != null)
		{
			return itsStyleMinimap;
		}
		
		return GUI.skin.box;
	}
	
	/// <summary>
	/// returns the guistyle for a minimap buttons
	/// </summary>
	/// <returns>returns the minimap button style. Returns the default button style if no custom style was found.</returns>
	public static GUIStyle GetStyleMinimapButton()
	{
		if(itsStyleMinimapButton != null)
		{
			return itsStyleMinimapButton;
		}
		
		return GUI.skin.button;
	}
	
	#endregion
	
	/// <summary>
	/// returns the requested guistyle for a button
	/// </summary>
	/// <param name="theStyleButton">the type of button style</param>
	/// <returns>returns the requested button style. Returns the default button style if no custom style was found.</returns>
	public static GUIStyle GetStyleButton(eStyleButton theStyleButton)
	{
		if(theStyleButton == eStyleButton.eButton && itsStyleButton != null)
		{
			return itsStyleButton;
		}
		else if(theStyleButton == eStyleButton.eButtonLeft && itsStyleButtonLeft != null)
		{
			return itsStyleButtonLeft;
		}
		else if(theStyleButton == eStyleButton.eButtonRight && itsStyleButtonRight != null)
		{
			return itsStyleButtonRight;
		}
		else if(theStyleButton == eStyleButton.eButtonTop && itsStyleButtonTop != null)
		{
			return itsStyleButtonTop;
		}
		else if(theStyleButton == eStyleButton.eButtonBottom && itsStyleButtonBottom != null)
		{
			return itsStyleButtonBottom;
		}
		else if(theStyleButton == eStyleButton.eButtonMiddle && itsStyleButtonMiddle != null)
		{
			return itsStyleButtonMiddle;
		}
		
		return GUI.skin.button;
	}
	
	/// <summary>
	/// returns the requested guistyle for a box
	/// </summary>
	/// <param name="theStyleBox">the type of box style</param>
	/// <returns>returns the requested box style. Returns the default box style if no custom style was found.</returns>
	public static GUIStyle GetStyleBox(eStyleBox theStyleBox)
	{
		if(theStyleBox == eStyleBox.eBox && itsStyleBox != null)
		{
			return itsStyleBox;
		}
		else if(theStyleBox == eStyleBox.eBoxInvisible&& itsStyleBoxInvisible != null)
		{
			return itsStyleBoxInvisible;
		}
		else if(theStyleBox == eStyleBox.eBoxInteractive && itsStyleBox != null)
		{
			return itsStyleBoxInteractive;
		}
		else if(theStyleBox == eStyleBox.eBoxLeft && itsStyleBoxLeft != null)
		{
			return itsStyleBoxLeft;
		}
		else if(theStyleBox == eStyleBox.eBoxLeftInteractive && itsStyleBoxLeft != null)
		{
			return itsStyleBoxLeftInteractive;
		}
		else if(theStyleBox == eStyleBox.eBoxRight && itsStyleBoxRight != null)
		{
			return itsStyleBoxRight;
		}
		else if(theStyleBox == eStyleBox.eBoxRightInteractive && itsStyleBoxRight != null)
		{
			return itsStyleBoxRightInteractive;
		}
		else if(theStyleBox == eStyleBox.eBoxMiddleHorizontal && itsStyleBoxMiddleHorizontal != null)
		{
			return itsStyleBoxMiddleHorizontal;
		}
		else if(theStyleBox == eStyleBox.eBoxMiddleHorizontalInteractive && itsStyleBoxMiddleHorizontal != null)
		{
			return itsStyleBoxMiddleHorizontalInteractive;
		}
		else if(theStyleBox == eStyleBox.eBoxTop && itsStyleBoxTop != null)
		{
			return itsStyleBoxTop;
		}
		else if(theStyleBox == eStyleBox.eBoxTopInteractive && itsStyleBoxTop != null)
		{
			return itsStyleBoxTopInteractive;
		}
		else if(theStyleBox == eStyleBox.eBoxBottom && itsStyleBoxBottom != null)
		{
			return itsStyleBoxBottom;}
		else if(theStyleBox == eStyleBox.eBoxBottomInteractive && itsStyleBoxBottom != null)
		{
			return itsStyleBoxBottomInteractive;
		}
		else if(theStyleBox == eStyleBox.eBoxMiddleVertical && itsStyleBoxMiddleVertical != null)
		{
			return itsStyleBoxMiddleVertical;
		}
		else if(theStyleBox == eStyleBox.eBoxMiddleVerticalInteractive && itsStyleBoxMiddleVertical != null)
		{
			return itsStyleBoxMiddleVerticalInteractive;
		}
		else if(theStyleBox == eStyleBox.eBoxDark && itsStyleBoxDark != null)
		{
			return itsStyleBoxDark;
		}
		else if(theStyleBox == eStyleBox.eBoxDarkInteractive && itsStyleBoxDark != null)
		{
			return itsStyleBoxDarkInteractive;
		}
		else if(theStyleBox == eStyleBox.eBoxDarkLeft && itsStyleBoxDarkLeft != null)
		{
			return itsStyleBoxDarkLeft;
		}
		else if(theStyleBox == eStyleBox.eBoxDarkLeftInteractive && itsStyleBoxDarkLeft != null)
		{
			return itsStyleBoxDarkLeftInteractive;
		}
		else if(theStyleBox == eStyleBox.eBoxDarkRight && itsStyleBoxDarkRight != null)
		{
			return itsStyleBoxDarkRight;
		}
		else if(theStyleBox == eStyleBox.eBoxDarkRightInteractive && itsStyleBoxDarkRight != null)
		{
			return itsStyleBoxDarkRightInteractive;
		}
		else if(theStyleBox == eStyleBox.eBoxDarkMiddleHorizontal && itsStyleBoxDarkMiddleHorizontal != null)
		{
			return itsStyleBoxDarkMiddleHorizontal;
		}
		else if(theStyleBox == eStyleBox.eBoxDarkMiddleHorizontalInteractive && itsStyleBoxDarkMiddleHorizontal != null)
		{
			return itsStyleBoxDarkMiddleHorizontalInteractive;
		}
		else if(theStyleBox == eStyleBox.eBoxDarkTop && itsStyleBoxDarkTop != null)
		{
			return itsStyleBoxDarkTop;
		}
		else if(theStyleBox == eStyleBox.eBoxDarkTopInteractive && itsStyleBoxDarkTop != null)
		{
			return itsStyleBoxDarkTopInteractive;
		}
		else if(theStyleBox == eStyleBox.eBoxDarkBottom && itsStyleBoxDarkBottom != null)
		{
			return itsStyleBoxDarkBottom;
		}
		else if(theStyleBox == eStyleBox.eBoxDarkBottomInteractive && itsStyleBoxDarkBottom != null)
		{
			return itsStyleBoxDarkBottomInteractive;
		}
		else if(theStyleBox == eStyleBox.eBoxDarkMiddleVertical && itsStyleBoxDarkMiddleVertical != null)
		{
			return itsStyleBoxDarkMiddleVertical;
		}
		else if(theStyleBox == eStyleBox.eBoxDarkMiddleVerticalInteractive && itsStyleBoxDarkMiddleVertical != null)
		{
			return itsStyleBoxDarkMiddleVerticalInteractive;
		}
		else if(theStyleBox == eStyleBox.eBoxDecorated && itsStyleBoxDecorated != null)
		{
			return itsStyleBoxDecorated;
		}
		
		return GUI.skin.box;
	}
	
	/// <summary>
	/// returns the requested guistyle for a seperator
	/// </summary>
	/// <param name="theStyleSeparator">the type of the seperator</param>
	/// <returns>returns the requested seperator style. Returns the default label style if no custom style was found.</returns>
	public static GUIStyle GetStyleSeparator(eStyleSeparator theStyleSeparator)
	{
		if(theStyleSeparator == eStyleSeparator.eSeparatorHorizontal && itsStyleSeparatorHorizontal != null)
		{
			return itsStyleSeparatorHorizontal;
		}
		else if(theStyleSeparator == eStyleSeparator.eSeparatorVertical && itsStyleSeparatorVertical != null)
		{
			return itsStyleSeparatorVertical;
		}
		else if(theStyleSeparator == eStyleSeparator.eSeparatorVerticalFitInBox && itsStyleSeparatorVerticalFitInBox != null)
		{
			return itsStyleSeparatorVerticalFitInBox;
		}
		
		return GUI.skin.label;
	}
	
	/// <summary>
	/// returns the requested guistyle for a label
	/// </summary>
	/// <param name="theStyleSeparator">the type of the label</param>
	/// <returns>returns the requested label style. Returns the default label style if no custom style was found.</returns>
	public static GUIStyle GetStyleLabel(eStyleLabel theStyleLabel)
	{
		if(theStyleLabel == eStyleLabel.eLabel && itsStyleLabel != null)
		{
			return itsStyleLabel;
		}
		if(theStyleLabel == eStyleLabel.eLabelFitIntoBox && itsStyleLabelFitInToBox != null)
		{
			return itsStyleLabelFitInToBox;
		}
		
		return GUI.skin.box;
	}
	
	/// <summary>
	/// returns the requested guistyle for a window
	/// </summary>
	/// <param name="theStyleSeparator">the type of the window</param>
	/// <returns>returns the requested window style. Returns the default window style if no custom style was found.</returns>
	public static GUIStyle GetStyleWindow()
	{
		if(itsStyleWindow != null)
		{
			return itsStyleWindow;
		}
		
		return GUI.skin.window;
	}
	
	/// <summary>
	/// returns the requested guistyle for the cursor style
	/// </summary>
	/// <returns>returns the requested windowTitle style. Returns the label if no custom style was found.</returns>
	public static GUIStyle GetStyleCursor()
	{
		if(itsStyleCursor != null)
		{
			return itsStyleCursor;
		}
		
		return itsStyleCursor;
	}
	
	#region table styles
	
	/// <summary>
	/// returns the requested guistyle for the custom table style
	/// </summary>
	/// <returns>returns the requested custom table style. Returns default box style if no custom style was found.</returns>
	public static GUIStyle GetTableStyle()
	{
		Init();
		
		if(itsStyleTable != null)
		{
			return itsStyleTable;
		}
		else
		{
			return GUI.skin.box;
		}
	}
	
	/// <summary>
	/// returns the requested guistyle for the custom table_heading_row style
	/// </summary>
	/// <returns>returns the requested custom table row style. Returns default box style if no custom style was found.</returns>
	public static GUIStyle GetTableHeadingRowStyle()
	{
		Init();

		if(itsStyleTableHeadingRow != null)
		{
			return itsStyleTableHeadingRow;
		}
		else
		{
			return GUI.skin.box;
		}
	}
	
	/// <summary>
	/// returns the requested guistyle for the custom table_heading_cell style
	/// </summary>
	/// <returns>returns the requested custom table cell style. Returns default box style if no custom style was found.</returns>
	public static GUIStyle GetTableHeadingCellStyle()
	{
		Init();

		if(itsStyleTableHeadingCell != null)
		{
			return itsStyleTableHeadingCell;
		}
		else
		{
			return GUI.skin.box;
		}
	}
	
	/// <summary>
	/// returns the requested guistyle for the custom table_row style
	/// </summary>
	/// <returns>returns the requested custom table row style. Returns default box style if no custom style was found.</returns>
	public static GUIStyle GetTableRowStyle()
	{
		Init();

		if(itsStyleTableRow != null)
		{
			return itsStyleTableRow;
		}
		else
		{
			return GUI.skin.box;
		}
	}
	
	/// <summary>
	/// returns the requested guistyle for the custom table_cell style
	/// </summary>
	/// <returns>returns the requested custom table cell style. Returns default box style if no custom style was found.</returns>
	public static GUIStyle GetTableCellStyle()
	{
		Init();
		
		if(itsStyleTableRowCell != null)
		{
			return itsStyleTableRowCell;
		}
		else
		{
			return GUI.skin.box;
		}
	}
	
	#endregion
	
	#endregion
	
	/// <summary>
	/// resets the path of the skin
	/// </summary>
	/// <param name="thePath">the new filepath of the guiskin.</param>
	public static void SetSkinPath(string thePath)
	{
		itsDefaultGuiSkinPath = thePath;
		itsResetPath = true;
	}
	
	/// <summary>
	/// returns the path of the skin
	/// </summary>
	/// <param name="thePath">the new filepath of the guiskin.</param>
	public static string GetSkinPath()
	{
		return itsDefaultGuiSkinPath;
	}
	
	/// <summary>
	/// reloads the selected skin elements
	/// </summary>
	private static void Init()
	{
		Init(false);
	}
	
	/// <summary>
	/// This method has to be called before using this class
	/// </summary>
	private static void Init(bool theForceInit)
	{		
		if(itsSkin != null && !theForceInit && !itsResetPath)
		{
			return;
		}
		
		itsResetPath = false;
		
		Debug.Log("Loading skin: "+itsDefaultGuiSkinPath);
		itsSkin = Resources.Load(itsDefaultGuiSkinPath) as GUISkin;
		
		if(itsSkin == null)
		{
			Debug.Log("Kolmich Game Framework default skin wasn`t found");
			itsSkin = GUI.skin;
			return;
		}
		
		GUI.skin = itsSkin;
		
		//cache the styles
		itsStyleToggle = itsSkin.GetStyle("toggle");
		itsStyleTextField = itsSkin.GetStyle("textfield");
		itsStyleTextFieldLeft = itsSkin.GetStyle("textfield_left");
		itsStyleTextFieldRight = itsSkin.GetStyle("textfield_right");
		itsStyleTextArea = itsSkin.GetStyle("textarea");
		itsStyleWindow = itsSkin.GetStyle("window");
		
		itsStyleHorizontalSlider = itsSkin.GetStyle("horizontalslider");
		itsStyleHorizontalSliderThumb = itsSkin.GetStyle("horizontalsliderthumb");
		
		itsStyleVerticalSlider = itsSkin.GetStyle("verticalslider");
		itsStyleVerticalSliderThumb = itsSkin.GetStyle("verticalsliderthumb");
		
		itsStyleHorizontalScrollbar = itsSkin.GetStyle("horizontalscrollbar");
		itsStyleHorizontalScrollbarThumb = itsSkin.GetStyle("horizontalscrollbarthumb");
		itsStyleHorizontalScrollbarLeftButton = itsSkin.GetStyle("horizontalscrollbarleftbutton");
		itsStyleHorizontalScrollbarRightButton = itsSkin.GetStyle("horizontalscrollbarrightbutton");
		
		itsStyleVerticalScrollbar = itsSkin.GetStyle("verticalscrollbar");
		itsStyleVerticalScrollbarThumb = itsSkin.GetStyle("verticalscrollbarthumb");
		itsStyleVerticalScrollbarUpButton = itsSkin.GetStyle("verticalscrollbarupbutton");
		itsStyleVerticalScrollbarDownButton = itsSkin.GetStyle("verticalscrollbardownbutton");
		
		itsStyleScrollView = itsSkin.GetStyle("scrollview");
		itsStyleMinimap = itsSkin.GetStyle("minimap");
		itsStyleMinimapButton = itsSkin.GetStyle("minimap_button");
		
		itsStyleToggleStreched = itsSkin.GetStyle("toggle_stretched");
		itsStyleToggleCompact = itsSkin.GetStyle("toggle_compact");
		itsStyleToggleSuperCompact = itsSkin.GetStyle("toggle_supercompact");
		
		itsStyleToggleRadioStreched = itsSkin.GetStyle("toggle_radio_stretched");
		itsStyleToggleRadioCompact = itsSkin.GetStyle("toggle_radio_compact");
		itsStyleToggleRadioSuperCompact = itsSkin.GetStyle("toggle_radio_supercompact");
		
		itsStyleToggleSwitch = itsSkin.GetStyle("toggle_switch");
		itsStyleToggleBoolean = itsSkin.GetStyle("toggle_boolean");
		itsStyleToggleArrow = itsSkin.GetStyle("toggle_arrow");
		itsStyleToggleButton = itsSkin.GetStyle("toggle_button");
		
		itsStyleButton = itsSkin.GetStyle("Button");
		itsStyleButtonLeft = itsSkin.GetStyle("button_left");
		itsStyleButtonRight = itsSkin.GetStyle("button_right");
		itsStyleButtonTop = itsSkin.GetStyle("button_top");
		itsStyleButtonBottom = itsSkin.GetStyle("button_bottom");
		itsStyleButtonMiddle = itsSkin.GetStyle("button_middle");
		
		itsStyleBox = itsSkin.GetStyle("Box");
		itsStyleBoxInvisible = itsSkin.GetStyle("box_invisible");
		itsStyleBoxInteractive = itsSkin.GetStyle("box_interactive");
		itsStyleBoxLeft = itsSkin.GetStyle("box_left");
		itsStyleBoxLeftInteractive = itsSkin.GetStyle("box_left_interactive");
		itsStyleBoxRight = itsSkin.GetStyle("box_right");
		itsStyleBoxRightInteractive = itsSkin.GetStyle("box_right_interactive");
		itsStyleBoxMiddleHorizontal = itsSkin.GetStyle("box_middle_horizontal");
		itsStyleBoxMiddleHorizontalInteractive = itsSkin.GetStyle("box_middle_horizontal_interactive");
		itsStyleBoxTop = itsSkin.GetStyle("box_top");
		itsStyleBoxTopInteractive = itsSkin.GetStyle("box_top_interactive");
		itsStyleBoxBottom = itsSkin.GetStyle("box_bottom");
		itsStyleBoxBottomInteractive = itsSkin.GetStyle("box_bottom_interactive");
		itsStyleBoxMiddleVertical = itsSkin.GetStyle("box_middle_vertical");
		itsStyleBoxMiddleVerticalInteractive = itsSkin.GetStyle("box_middle_vertical_interactive");
		
		itsStyleBoxDark = itsSkin.GetStyle("box_dark");
		itsStyleBoxDarkInteractive = itsSkin.GetStyle("box_dark_interactive");
		itsStyleBoxDarkLeft = itsSkin.GetStyle("box_dark_left");
		itsStyleBoxDarkLeftInteractive = itsSkin.GetStyle("box_dark_left_interactive");
		itsStyleBoxDarkRight = itsSkin.GetStyle("box_dark_right");
		itsStyleBoxDarkRightInteractive = itsSkin.GetStyle("box_dark_right_interactive");
		itsStyleBoxDarkMiddleHorizontal = itsSkin.GetStyle("box_dark_middle_horizontal");
		itsStyleBoxDarkMiddleHorizontalInteractive = itsSkin.GetStyle("box_dark_middle_horizontal_interactive");
		itsStyleBoxDarkTop = itsSkin.GetStyle("box_dark_top");
		itsStyleBoxDarkTopInteractive = itsSkin.GetStyle("box_dark_top_interactive");
		itsStyleBoxDarkBottom = itsSkin.GetStyle("box_dark_bottom");
		itsStyleBoxDarkBottomInteractive = itsSkin.GetStyle("box_dark_bottom_interactive");
		itsStyleBoxDarkMiddleVertical = itsSkin.GetStyle("box_dark_middle_vertical");
		itsStyleBoxDarkMiddleVerticalInteractive = itsSkin.GetStyle("box_dark_middle_vertical_interactive");
		itsStyleBoxDecorated = itsSkin.GetStyle("box_decorated");
		
		itsStyleSeparatorVertical = itsSkin.GetStyle("separator_vertical");
		itsStyleSeparatorVerticalFitInBox = itsSkin.GetStyle("separator_vertical_fitinbox");
		itsStyleSeparatorHorizontal = itsSkin.GetStyle("separator_horizontal");
		
		itsStyleLabel = itsSkin.GetStyle("label");
		itsStyleLabelFitInToBox = itsSkin.GetStyle("label_fitintobox");
		
		itsStyleCursor = itsSkin.GetStyle("mouse_cursor");
	}
	
	#region gui draw methods
	
	/// <summary>
	/// Use this method to draw a window title with icon an text.
	/// </summary>
	/// <remarks>
	/// Use this method to draw a title into a window or box. If the icon parameter is null, the window will display the text only. To properly close the window header call EndWindowHeader().
	/// </remarks>
	/// <param name="theTitle">the window title</param>
	/// <param name="theIcon">the icon displayed before the window text</param>
	public static void BeginWindowHeader(string theTitle, Texture2D theIcon)
	{
		Init();

		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDark);
		
		KGFGUIUtility.Label("",theIcon,eStyleLabel.eLabel,GUILayout.Width(GetSkinHeight()));
		KGFGUIUtility.Label(theTitle,eStyleLabel.eLabel);
	}
	
	/// <summary>
	/// Use this method to end the a window title.
	/// </summary>
	/// <remarks>
	/// This method ends a window title. Before calling this method call BeginWindowHeader() to start the window header properly.
	/// </remarks>
	/// <param name="theCloseButton">true if the close button of the window should be displayed.</param>
	/// <returns>returns true if the close button was clicked</returns>
	public static bool EndWindowHeader(bool theCloseButton)
	{
		bool aClick = false;
		
		if(theCloseButton)
		{
			Init();
			aClick = KGFGUIUtility.Button("x", KGFGUIUtility.eStyleButton.eButton,GUILayout.Width(KGFGUIUtility.GetSkinHeight()));
		}
		
		KGFGUIUtility.EndHorizontalBox();
		
		return aClick;
	}
	
	/// <summary>
	/// Use this method to draw the current open drop down box.
	/// </summary>
	/// <remarks>
	/// If using the KGFGUIDropDown this method must be called after all other render operations in the OnGUI function. The open drop down list will be rendered on top of the screen.
	/// </remarks>
	public static void RenderDropDownList()
	{
		if(KGFGUIDropDown.itsOpenInstance != null && KGFGUIDropDown.itsCorrectedOffset)
		{
			GUI.depth = 0;
			
			Rect aListRect;
			bool aDirection;
			
			if(KGFGUIDropDown.itsOpenInstance.itsDirection == KGFGUIDropDown.eDropDirection.eDown
			   || (KGFGUIDropDown.itsOpenInstance.itsDirection == KGFGUIDropDown.eDropDirection.eAuto
			       && (KGFGUIDropDown.itsOpenInstance.itsLastRect.y + KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButton).fixedHeight + KGFGUIDropDown.itsOpenInstance.itsHeight) < Screen.height))
			{
				aListRect = new Rect(KGFGUIDropDown.itsOpenInstance.itsLastRect.x,
				                     KGFGUIDropDown.itsOpenInstance.itsLastRect.y + KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButton).fixedHeight,
				                     KGFGUIDropDown.itsOpenInstance.itsWidth,
				                     KGFGUIDropDown.itsOpenInstance.itsHeight);
				aDirection = true;
			}
			else
			{
				aListRect = new Rect(KGFGUIDropDown.itsOpenInstance.itsLastRect.x,
				                     KGFGUIDropDown.itsOpenInstance.itsLastRect.y - KGFGUIDropDown.itsOpenInstance.itsHeight,
				                     KGFGUIDropDown.itsOpenInstance.itsWidth,
				                     KGFGUIDropDown.itsOpenInstance.itsHeight);
				aDirection = false;
			}
			
			/*
			if(Application.isPlaying)
			{
			 */
			GUILayout.BeginArea(aListRect);
			{
				KGFGUIDropDown.itsOpenInstance.itsScrollPosition = KGFGUIUtility.BeginScrollView(KGFGUIDropDown.itsOpenInstance.itsScrollPosition, false, false, GUILayout.ExpandWidth(true));
				{
					foreach(string aEntry in KGFGUIDropDown.itsOpenInstance.GetEntrys())
					{
						if(aEntry != string.Empty)
						{
							if(KGFGUIUtility.Button(aEntry, KGFGUIUtility.eStyleButton.eButtonMiddle, GUILayout.ExpandWidth(true)))
							{
								KGFGUIDropDown.itsOpenInstance.SetSelectedItem(aEntry);
								KGFGUIDropDown.itsOpenInstance = null;
								break;
							}
						}
					}
				}
				GUILayout.EndScrollView();
			}
			GUILayout.EndArea();
			/*
			}
			else
			{
				
			}*/

			if(aDirection)
			{
				aListRect.y -= KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButton).fixedHeight;
				aListRect.height += KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButton).fixedHeight;
			}
			else
			{
				//aListRect.y -= KGFGUIUtility.GetButtonStyle().fixedHeight;
				aListRect.height += KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButton).fixedHeight;
			}
			
			Vector3 aMousePosition = Input.mousePosition;
			aMousePosition.y = Screen.height - aMousePosition.y;
			
			//check if the rect contains the mouse and the pressed mouse button is the left mouse button
			if(!aListRect.Contains(aMousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				KGFGUIDropDown.itsOpenInstance = null;
			}
			
			if(KGFGUIDropDown.itsOpenInstance != null)
			{
				if(aListRect.Contains(aMousePosition))
				{
					KGFGUIDropDown.itsOpenInstance.itsHover = true;
				}
				else
				{
					KGFGUIDropDown.itsOpenInstance.itsHover = false;
				}
			}
		}
	}
	
	/// <summary>
	/// Use this method to draw a space in the size of the skin height
	/// </summary>
	/// <remarks>
	/// Use GUILayout to draw a custom sized space. If you use KGFGUIUtility.Space the space will always match the current size of the skin,
	/// so changing the skin will also adapt the space to the correct size
	/// </remarks>
	public static void Space()
	{
		GUILayout.Space(GetSkinHeight());
	}
	
	/// <summary>
	/// Use this method to draw a space in half of the size
	/// </summary>
	/// <remarks>
	/// Use GUILayout to draw a custom sized space. If you use KGFGUIUtility.SpaceSmall the space will always match the current size of the skin/2.0f,
	/// so changing the skin will also adapt the space to the correct size
	/// </remarks>
	public static void SpaceSmall()
	{
		GUILayout.Space(GetSkinHeight()/2.0f);
	}
	
	
	#region label
	
	/// <summary>
	/// Use this method to draw a label
	/// </summary>
	/// <remarks>
	/// Use this method to draw a label. The style of the label will be set to the default skin style.
	/// </remarks>
	/// <param name="theText">the text inside the lable</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void Label(string theText, params GUILayoutOption[] theLayout)
	{
		Label(theText, eStyleLabel.eLabel, theLayout);
	}

	/// <summary>
	/// Use this method to draw a label
	/// </summary>
	/// <remarks>
	/// Use this method to draw a label with a specified style.
	/// </remarks>
	/// <param name="theText">the text inside the label</param>
	/// <param name="theStyleLabel">the style of the label</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void Label(string theText, eStyleLabel theStyleLabel, params GUILayoutOption[] theLayout)
	{
		Label(theText,null,theStyleLabel,theLayout);
	}
	
	/// <summary>
	/// Use this method to draw a label with text and an icon
	/// </summary>
	/// <remarks>
	/// Use this method to draw a label with a specified style and an icon.
	/// </remarks>
	/// <param name="theText">the text inside the label</param>
	/// <param name="theImage">the icon inside the label</param>
	/// <param name="theStyleLabel">the style of the label</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void Label(string theText, Texture2D theImage, eStyleLabel theStyleLabel, params GUILayoutOption[] theLayout)
	{
		Init();
		GUIContent aGuiContent = null;
		
		if(theImage != null)
		{
			aGuiContent = new GUIContent(theText, theImage);
		}
		else
		{
			aGuiContent = new GUIContent(theText);
		}
		
		GUILayout.Label(aGuiContent, GetStyleLabel(theStyleLabel), theLayout);
	}
	
	#endregion
	
	/// <summary>
	/// Use this method to draw a seperator
	/// </summary>
	/// <remarks>
	/// Use this method to draw a seperator for a spacing between two elements
	/// </remarks>
	/// <param name="theStyleSeparator">the style of the seperator</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void Separator(eStyleSeparator theStyleSeparator, params GUILayoutOption[] theLayout)
	{
		Init();
		GUILayout.Label("", GetStyleSeparator(theStyleSeparator),theLayout);
	}

	/// <summary>
	/// Use this method to draw a toggle button
	/// </summary>
	/// <remarks>
	/// Use this method to draw a toggle button. to check if the state of the button has changed check the return value of this method.
	/// </remarks>
	/// <param name="theValue">the current state of the toggle button</param>
	/// <param name="theText">the text of the toggle button</param>
	/// <param name="theToggleStyle">the style of the toggle button</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns true if the close toggle button was clicked</returns>
	public static bool Toggle(bool theValue, string theText, eStyleToggl theToggleStyle, params GUILayoutOption[] theLayout)
	{
		Init();
		return GUILayout.Toggle(theValue, theText, GetStyleToggl(theToggleStyle), theLayout);
	}
	
	/// <summary>
	/// Use this method to draw a toggle button
	/// </summary>
	/// <remarks>
	/// Use this method to draw a toggle button. to check if the state of the button has changed check the return value of this method.
	/// </remarks>
	/// <param name="theValue">the current state of the toggle button</param>
	/// <param name="theImage">the image of the toggle button</param>
	/// <param name="theToggleStyle">the style of the toggle button</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns true if the close toggle button was clicked</returns>
	public static bool Toggle(bool theValue, Texture2D theImage, eStyleToggl theToggleStyle, params GUILayoutOption[] theLayout)
	{
		Init();
		return GUILayout.Toggle(theValue, theImage, GetStyleToggl(theToggleStyle), theLayout);
	}

	#region window
	
	/// <summary>
	/// Use this method to draw a window
	/// </summary>
	/// <remarks>
	/// Use this method to draw a window. To get the new position of the window check the return value of this method.
	/// </remarks>
	/// <param name="theId">the unique window id</param>
	/// <param name="theRect">the windows rectangle</param>
	/// <param name="theFunction">the function that draw this window</param>
	/// <param name="theText">the window header text</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the new window rectangle if the window was draged to a differnt screen position</returns>
	public static Rect Window(int theId, Rect theRect, GUI.WindowFunction theFunction, string theText, params GUILayoutOption[] theLayout)
	{
		return Window(theId,theRect,theFunction,null,theText,theLayout);
	}
	
	/// <summary>
	/// Use this method to draw a window
	/// </summary>
	/// <remarks>
	/// Use this method to draw a window. To get the new position of the window check the return value of this method.
	/// </remarks>
	/// <param name="theId">the unique window id</param>
	/// <param name="theRect">the windows rectangle</param>
	/// <param name="theFunction">the function that draw this window</param>
	/// <param name="theImage">the window header icon</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the new window rectangle if the window was draged to a differnt screen position</returns>
	public static Rect Window(int theId, Rect theRect, GUI.WindowFunction theFunction, Texture theImage, params GUILayoutOption[] theLayout)
	{
		return Window(theId,theRect,theFunction,theImage,string.Empty,theLayout);
	}
	
	/// <summary>
	/// Use this method to draw a window
	/// </summary>
	/// <remarks>
	/// Use this method to draw a window. To get the new position of the window check the return value of this method.
	/// </remarks>
	/// <param name="theId">the unique window id</param>
	/// <param name="theRect">the windows rectangle</param>
	/// <param name="theFunction">the function that draw this window</param>
	/// <param name="theImage">the window header icon</param>
	/// <param name="theText">the window header text</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the new window rectangle if the window was draged to a differnt screen position</returns>
	public static Rect Window(int theId, Rect theRect, GUI.WindowFunction theFunction, Texture theImage, string theText, params GUILayoutOption[] theLayout)
	{
		Init();
		
		GUIContent aGuiContent = null;
		if(theImage != null)
		{
			aGuiContent = new GUIContent(theText, theImage);
		}
		else
		{
			aGuiContent = new GUIContent(theText);
		}
		
		if(itsStyleWindow != null)
		{
			return GUILayout.Window(theId, theRect,theFunction,aGuiContent,itsStyleWindow,theLayout);
		}
		else
		{
			return GUILayout.Window(theId, theRect,theFunction,aGuiContent,theLayout);
		}
	}

	#endregion
	
	#region box
	
	/// <summary>
	/// Use this method to draw a box
	/// </summary>
	/// <remarks>
	/// Use this method to draw a box.
	/// </remarks>
	/// <param name="theText">the text inside the box</param>
	/// <param name="theStyleBox">the style of the box</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void Box(string theText, eStyleBox theStyleBox, params GUILayoutOption[] theLayout)
	{
		Box(null, theText, theStyleBox, theLayout);
	}

	/// <summary>
	/// Use this method to draw a box
	/// </summary>
	/// <remarks>
	/// Use this method to draw a box.
	/// </remarks>
	/// <param name="theText">the text inside the box</param>
	/// <param name="theStyleBox">the style of the box</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void Box(Texture theImage, eStyleBox theStyleBox, params GUILayoutOption[] theLayout)
	{
		Box(theImage, "", theStyleBox, theLayout);
	}
	
	/// <summary>
	/// Use this method to draw a box
	/// </summary>
	/// <remarks>
	/// Use this method to draw a box.
	/// </remarks>
	/// <param name="theImage">the icon of the box</param>
	/// <param name="theText">the text inside the box</param>
	/// <param name="theStyleBox">the style of the box</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void Box(Texture theImage, string theText, eStyleBox theStyleBox, params GUILayoutOption[] theLayout)
	{
		Init();
		GUIContent aGuiContent = null;
		if(theImage != null)
		{
			aGuiContent = new GUIContent(theText, theImage);
		}
		else
		{
			aGuiContent = new GUIContent(theText);
		}
		
		GUILayout.Box(aGuiContent, GetStyleBox(theStyleBox), theLayout);
	}
	
	#endregion
	
	/// <summary>
	/// Use this method to draw a vertical box
	/// </summary>
	/// <remarks>
	/// Use this method to draw a vertical box. For proper usage call EndVerticalBox() after using this function.
	/// </remarks>
	/// <param name="theStyleBox">the style of the box</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void BeginVerticalBox(eStyleBox theStyleBox, params GUILayoutOption[] theLayout)
	{
		Init();
		
		GUILayout.BeginVertical(GetStyleBox(theStyleBox), theLayout);
	}
	
	/// <summary>
	/// Use this method to end a vertical box
	/// </summary>
	/// <remarks>
	/// Use this method to end a vertical box. For proper usage call BeginVerticalBox() before using this function.
	/// </remarks>
	public static void EndVerticalBox()
	{
		GUILayout.EndVertical();
	}

	/// <summary>
	/// Use this method to draw a box with vertical padding
	/// </summary>
	/// <remarks>
	/// Use this method to draw a box with padding. For proper usage call EndVerticalPadding() after using this function.
	/// </remarks>
	public static void BeginVerticalPadding()
	{
		GUILayout.BeginVertical();
		BeginHorizontalBox(eStyleBox.eBoxInvisible);
	}
	
	/// <summary>
	/// Use this method to end a box with vertical padding
	/// </summary>
	/// <remarks>
	/// Use this method to end a box with vertical padding. For proper usage call BeginVerticalBox() before using this function.
	/// </remarks>
	public static void EndVerticalPadding()
	{
		EndHorizontalBox();
		GUILayout.EndVertical();
	}
	
	/// <summary>
	/// Use this method to draw a box with horizontal padding
	/// </summary>
	/// <remarks>
	/// Use this method to draw a box with horizontal padding. For proper usage call EndHorizontalPadding() after using this function.
	/// </remarks>
	public static void BeginHorizontalPadding()
	{
		GUILayout.BeginHorizontal();
		BeginVerticalBox(eStyleBox.eBoxInvisible);
	}
	
	/// <summary>
	/// Use this method to end a box with horizontal padding
	/// </summary>
	/// <remarks>
	/// Use this method to end a box with horizontal padding. For proper usage call BeginHorizontalPadding() before using this function.
	/// </remarks>
	public static void EndHorizontalPadding()
	{
		EndVerticalBox();
		GUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// Use this method to draw a horizontal box
	/// </summary>
	/// <remarks>
	/// Use this method to draw a horizontal box. For proper usage call EndHorizontalBox() after using this function.
	/// </remarks>
	/// <param name="theStyleBox">the style of the box</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void BeginHorizontalBox(eStyleBox theStyleBox, params GUILayoutOption[] theLayout)
	{
		Init();
		
		GUILayout.BeginHorizontal(GetStyleBox(theStyleBox), theLayout);
	}
	
	/// <summary>
	/// Use this method to end a vertical box
	/// </summary>
	/// <remarks>
	/// Use this method to end a vertical box. For proper usage call BeginHorizontalBox() before using this function.
	/// </remarks>
	public static void EndHorizontalBox()
	{
		GUILayout.EndHorizontal();
	}

	/// <summary>
	/// Use this method to draw a scroll view
	/// </summary>
	/// <remarks>
	/// Use this method to draw a scroll view. To get the new position of the scrollview check the return value of this method. For proper use call EndScrollView() after using this method.
	/// </remarks>
	/// <param name="thePosition">the current position of the scrollview</param>
	/// <param name="theHorizontalAlwaysVisible">if the horizontal scrollbar is always visible</param>
	/// <param name="theVerticalAlwaysVisible">if the vertical scrollbar is always visible</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the new window rectangle if the window was draged to a differnt screen position</returns>
	public static Vector2 BeginScrollView(Vector2 thePosition, bool theHorizontalAlwaysVisible, bool theVerticalAlwaysVisible, params GUILayoutOption[] theLayout)
	{
		Init();
		GUI.skin = itsSkin;
		
		if(itsStyleHorizontalScrollbar != null && itsStyleVerticalScrollbar != null)
		{
			return GUILayout.BeginScrollView(thePosition, theHorizontalAlwaysVisible, theVerticalAlwaysVisible, itsStyleHorizontalScrollbar, itsStyleVerticalScrollbar, theLayout);
		}
		else
		{
			return GUILayout.BeginScrollView(thePosition, theHorizontalAlwaysVisible, theVerticalAlwaysVisible, theLayout);
		}
	}
	
	/// <summary>
	/// Use this method to end a scroll view
	/// </summary>
	/// <remarks>
	/// Use this method to end a scroll view. For proper usage call BeginScrollView() before using this function.
	/// </remarks>
	public static void EndScrollView()
	{
		GUILayout.EndScrollView();
	}

	/// <summary>
	/// Use this method to draw a text field
	/// </summary>
	/// <remarks>
	/// Use this method to draw a text field. To get the new text inside the control use the return value of this method.
	/// </remarks>
	/// <param name="theText">the current text of the textbox</param>
	/// <param name="theStyleTextField">the style of the text field</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the new text inside the control</returns>
	public static string TextField(string theText, eStyleTextField theStyleTextField,  params GUILayoutOption[] theLayout)
	{
		Init();
		
		return GUILayout.TextField(theText, GetStyleTextField(theStyleTextField), theLayout);
	}
	
	/// <summary>
	/// Use this method to draw a text area
	/// </summary>
	/// <remarks>
	/// Use this method to draw a text area. To get the new text inside the control use the return value of this method.
	/// </remarks>
	/// <param name="theText">the current text of the textbox</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the new text inside the control</returns>
	public static string TextArea(string theText, params GUILayoutOption[] theLayout)
	{
		Init();
		
		if(itsStyleTextArea != null)
		{
			return GUILayout.TextArea(theText, itsStyleTextArea, theLayout);
		}
		else
		{
			return GUILayout.TextArea(theText, theLayout);
		}
	}

	#region button
	
	/// <summary>
	/// Use this method to draw a button
	/// </summary>
	/// <remarks>
	/// Use this method to draw a button. To get the state of the utton use the return value of this method.
	/// </remarks>
	/// <param name="theText">the text inside the button</param>
	/// <param name="theButtonStyle">the render style of the button</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the state of the button in this frame. returns true if the button is clicked.</returns>
	public static bool Button(string theText, eStyleButton theButtonStyle, params GUILayoutOption[] theLayout)
	{
		return Button(null, theText, theButtonStyle, theLayout);
	}

	/// <summary>
	/// Use this method to draw a button
	/// </summary>
	/// <remarks>
	/// Use this method to draw a button. To get the state of the utton use the return value of this method.
	/// </remarks>
	/// <param name="theImage">the icon inside the button</param>
	/// <param name="theButtonStyle">the render style of the button</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the state of the button in this frame. returns true if the button is clicked.</returns>
	public static bool Button(Texture theImage, eStyleButton theButtonStyle, params GUILayoutOption[] theLayout)
	{
		return Button(theImage, "", theButtonStyle, theLayout);
	}
	
	/// <summary>
	/// Use this method to draw a button
	/// </summary>
	/// <remarks>
	/// Use this method to draw a button. To get the state of the utton use the return value of this method.
	/// </remarks>
	/// <param name="theImage">the icon inside the button (icon is before text)</param>
	/// <param name="theText">the text inside the button (text is after icon)</param>
	/// <param name="theButtonStyle">the render style of the button</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the state of the button in this frame. returns true if the button is clicked.</returns>
	public static bool Button(Texture theImage, string theText, eStyleButton theButtonStyle, params GUILayoutOption[] theLayout)
	{
		GUIContent aGuiContent = null;
		
		if(theImage != null)
		{
			aGuiContent = new GUIContent(theText, theImage);
		}
		else
		{
			aGuiContent = new GUIContent(theText);
		}
		
		Init();

		return GUILayout.Button(aGuiContent, GetStyleButton(theButtonStyle), theLayout);
	}
	
	#endregion
	
	/// <summary>
	/// Use this method to draw a cursor with up, right, down, left and center button
	/// </summary>
	/// <remarks>
	/// Use this method to draw a text area. To get the new text inside the control use the return value of this method.
	/// </remarks>
	/// <returns>returns the state of the cursor in this frame</returns>
	public static eCursorState Cursor()
	{
		return Cursor(null, null, null, null, null);
	}
	
	/// <summary>
	/// Use this method to draw a cursor with up, right, down, left and center button
	/// </summary>
	/// <remarks>
	/// Use this method to draw a text area. To get the new text inside the control use the return value of this method.
	/// </remarks>
	/// <param name="theUp">the text inside the up button</param>
	/// <param name="theRight">the text inside the right button</param>
	/// <param name="theDown">the text inside the down button</param>
	/// <param name="theLeft">the text inside the left button</param>
	/// <param name="theCenter">the text inside the center button</param>
	/// <returns>returns the state of the cursor in this frame</returns>
	public static eCursorState Cursor(Texture theUp, Texture theRight, Texture theDown, Texture theLeft, Texture theCenter)
	{
		float aHeight = GetSkinHeight();
		float aTotalControlSize = aHeight*3.0f;
		
		eCursorState aState = eCursorState.eNone;
		
		GUILayout.BeginVertical(GUILayout.ExpandWidth(false),GUILayout.ExpandHeight(false));
		{
			BeginHorizontalBox(eStyleBox.eBoxInvisible);
			{
				GUILayout.BeginVertical(GUILayout.Width(aTotalControlSize),GUILayout.Height(aTotalControlSize));
				{
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false),GUILayout.ExpandHeight(false));
					{
						GUILayout.Space(aHeight);
						
						if(theUp != null)
						{
							if(Button(theUp,eStyleButton.eButtonTop,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eUp;
							}
						}
						else
						{
							if(Button("",eStyleButton.eButtonTop,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eUp;
							}
						}
						GUILayout.Space(aHeight);
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false),GUILayout.ExpandHeight(false));
					{
						if(theLeft != null)
						{
							if(Button(theLeft,eStyleButton.eButtonLeft,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eLeft;
							}
						}
						else
						{
							if(Button("",eStyleButton.eButtonLeft,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eLeft;
							}
						}
						
						if(theCenter != null)
						{
							if(Button(theCenter,eStyleButton.eButtonLeft,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eCenter;
							}
						}
						else
						{
							if(Button("",eStyleButton.eButtonMiddle,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eCenter;
							}
						}
						
						if(theRight != null)
						{
							if(Button(theRight, eStyleButton.eButtonLeft,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eRight;
							}
						}
						else
						{
							if(Button("",eStyleButton.eButtonRight,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eRight;
							}
						}
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false),GUILayout.ExpandHeight(false));
					{
						GUILayout.Space(aHeight);
						
						if(theDown != null)
						{
							if(Button(theDown, eStyleButton.eButtonLeft,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eDown;
							}
						}
						else
						{
							if(Button("",eStyleButton.eButtonBottom,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eDown;
							}
						}
						GUILayout.Space(aHeight);
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
			}
			EndHorizontalBox();
		}
		GUILayout.EndVertical();
		
		return aState;
	}

	#region slider
	
	/// <summary>
	/// use this method to draw a horizontal slider
	/// </summary>
	/// <remarks>
	/// This method draws a horizontal oriented slider and returns the current slider position.
	/// </remarks>
	/// <param name="theValue">the current slider position</param>
	/// <param name="theLeftValue">the minimum value of the slider</param>
	/// <param name="theRightValue">the maximum value of the slider</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the new position of the slider.</returns>
	public static float HorizontalSlider(float theValue, float theLeftValue, float theRightValue, params GUILayoutOption[] theLayout)
	{
		Init();
		
		if(itsStyleHorizontalSlider != null && itsStyleHorizontalSliderThumb != null)
		{
			return GUILayout.HorizontalSlider(theValue, theLeftValue, theRightValue, itsStyleHorizontalSlider, itsStyleHorizontalSliderThumb, theLayout);
		}
		else
		{
			return GUILayout.HorizontalSlider(theValue, theLeftValue, theRightValue, theLayout);
		}
	}
	
	/// <summary>
	/// use this method to draw a vertival slider
	/// </summary>
	/// <remarks>
	/// This method draws a vertical oriented slider and returns the current slider position.
	/// </remarks>
	/// <param name="theValue">the current slider position</param>
	/// <param name="theLeftValue">the minimum value of the slider</param>
	/// <param name="theRightValue">the maximum value of the slider</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the new position of the slider.</returns>
	public static float VerticalSlider(float theValue, float theLeftValue, float theRightValue, params GUILayoutOption[] theLayout)
	{
		Init();
		
		if(itsStyleVerticalSlider != null && itsStyleVerticalSliderThumb != null)
		{
			return GUILayout.VerticalSlider(theValue, theLeftValue, theRightValue, itsStyleVerticalSlider, itsStyleVerticalSliderThumb, theLayout);
		}
		else
		{
			return GUILayout.VerticalSlider(theValue, theLeftValue, theRightValue, theLayout);
		}
	}
	
	#endregion
	
	#endregion
	
	#endregion
}