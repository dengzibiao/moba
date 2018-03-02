using UnityEngine;
using System.Collections;

/// <summary>
/// the KGFIValidator interface is used to validate the status of prefabs in the scene.
/// </summary>
/// <remarks>
/// The status of the KGFIValidator is displayed in the Unity3D inspector as a message list. If there aree no errors, warnings or infos, there will be a green box showing "All component values are valid".
/// In case of having troubles with any of the KGFModules check the inspector for further information.
/// </remarks>
public interface KGFIValidator
{
	/// <summary>
	/// implement this interface to be able to show the validation status in the Unity3D inspector.
	/// </summary>
	/// <remarks>
	/// Use this method to implement validation logic that results should be displayed in the Unity3D inspector.
	/// </remarks>
	/// <returns>returns a message list with all errors, warnings and infos</returns>
	KGFMessageList Validate();
}
