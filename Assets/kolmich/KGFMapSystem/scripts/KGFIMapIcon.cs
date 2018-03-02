// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2011-10-03</date>
// <summary>short summary</summary>

using UnityEngine;
using System.Collections;

public interface KGFIMapIcon
{
	/// <summary>
	/// Get the category of map icon (The visibility of all map icons of a category can be enabled/disabled at once by the KGFMapSystem)
	/// </summary>
	/// <returns></returns>
	string GetCategory();
	
	/// <summary>
	/// This is the color in which the map icon and its arrow will be displayed on the minimap and map
	/// </summary>
	/// <returns></returns>
	Color GetColor();
	
	/// <summary>
	/// this texture will be used as arrow that will point in the direction of the map icon if it is outside the minimap.
	/// </summary>
	/// <returns></returns>
	Texture2D GetTextureArrow();
	
	/// <summary>
	/// Indicates if the icon on the minimap will follow the rotation of the gameobject
	/// </summary>
	/// <returns></returns>
	bool GetRotate();
	
	/// <summary>
	/// Returns if map icon should be visible at the moment (this method should return a cached value, used very often)
	/// </summary>
	/// <returns></returns>
	bool GetIsVisible();
	
	/// <summary>
	/// Returns if the arrow icon is allowd to be displayed.
	/// </summary>
	/// <returns></returns>
	bool GetIsArrowVisible();
	
	/// <summary>
	/// Get transform of map icon
	/// </summary>
	/// <returns></returns>
	Transform GetTransform();
	
	/// <summary>
	/// This method should return the name of the gameObject the interface is attached to (used for debug reasons)
	/// </summary>
	/// <returns></returns>
	string GetGameObjectName();
	
	/// <summary>
	/// This method should return a gameObject that will represent the map icon on the minimap. This gameobject must not be the map icon itself.
	/// This method can for example return a plane with a mapicon texture on it or simply a red cube. This gameobject will be transformed by the
	/// mapsystem to match the position of the map icon.
	/// </summary>
	/// <returns></returns>
	GameObject GetRepresentation();
}
