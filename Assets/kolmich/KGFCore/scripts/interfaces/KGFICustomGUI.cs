// <author>Christoph Hausjell</author>
// <email>christoph.hausjell@kolmich.at</email>
// <summary>Displays a list of modules that implement the KGFICustomGUI interface. By clicking a module icon the custom window of the module will be opened.</summary>

using UnityEngine;
using System.Collections;

public interface KGFICustomGUI
{
	string GetName();
	string GetHeaderName();
	Texture2D GetIcon();
	void Render();
}
