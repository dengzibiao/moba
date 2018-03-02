// <author>Christoph Hausjell</author>
// <email>christoph.hausjell@kolmich.at</email>
// <date>2012-03-13</date>
// <summary>this interface is used to build custom control sets for GUI elemnts.</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// the GUI Control interface of the Kolmich Game Framework.
/// </summary>
/// <remarks>
/// This interface is used to create custom gui controls like the KGFGUIDataTable or the KGFGUIDropDown.
/// </remarks>
public interface KGFIControl
{
	/// <summary>
	/// Use this method to render the control. only call Render() in an OnGUI() method of Unity3d.
	/// </summary>
	/// <remarks>
	/// This method contains the code how the control should be rendered. For proper use only call this methos in an OnGUI method of Unity3D.
	/// </remarks>
	void Render();
	
	/// <summary>
	/// returns the name of the Control.
	/// </summary>
	/// <remarks>
	/// This method to get the name of a control.
	/// </remarks>
	///<returns>returns the name of this KGFIControl.</returns>
	string GetName();
	
	/// <summary>
	/// retunrns the cisible state of a control.
	/// </summary>
	/// <remarks>
	/// If the controls visibility is set to true the control should be rendered.
	/// </remarks>
	///<returns>returns the visibility of this control. true if the control is visible.</returns>
	bool IsVisible();
}