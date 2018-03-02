// <author>Christoph Hausjell</author>
// <email>christoph.hausjell@kolmich.at</email>
// <summary>Includes all classes for the KOLMICH Game Framework Version control system of modules and the KGFCore.
// </summary>

using System;
using System.CodeDom;
using UnityEngine;
using System.Collections;

/// <summary>
///	Contains the current version of a module and the required KGFCore version.
/// </summary>
/// <remarks>
/// This class is the base class for each module in the KOLMICH Game Framework. By calling the constructor of a module this class checks the installed KGFCore version.
/// </remarks>
public static class KGFCoreVersion
{
	// contains the current KGFCoreVersion
	static Version itsVersion = new Version(1,1,0,0);
	
	/// <summary>
	///	use this method to get the current version of the KGFCore.
	/// </summary>
	/// <returns>returns the current version of the KGFCore.</returns>
	static public Version GetCurrentVersion()
	{
		return itsVersion.Clone() as Version;
	}
}