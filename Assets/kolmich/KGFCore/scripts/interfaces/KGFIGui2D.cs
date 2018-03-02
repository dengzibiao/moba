// <author>Michal Kolasinski</author>
// <email>michal.kolasinski@kolmich.at</email>

using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// the KGFIGui2D interface should be used by all gameObject instead of the OnGUI
/// </summary>
/// <remarks>
/// Instead of putting all gui rendering code into the OnGUI method gameObjects should implement the KGFIGui2D RenderGUI method. 
/// All the rendering code should be putted here. The KGFGuiRenderer will get all the KGFIGui2D objects by using KGFAccessor and 
/// it will render them in his one and only OnGUI method which should be the only one in the whole project
/// </remarks>
public interface KGFIGui2D 
{
	/// <summary>
	/// implement this interface if you want to render some 2d gui
	/// </summary>
	/// <remarks>
	/// Put all your render code into this method
	/// </remarks>	
	void RenderGUI();
	
	/// <summary>
	/// returns a number for drawing order, lower numbers will be drawn first, and so be in the background
	/// higher numbers will be drawn last, and therefore be in the foreground
	/// </summary>
	/// <returns></returns>
	int GetLayer();
}
