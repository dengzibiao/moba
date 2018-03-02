using UnityEngine;
using UnityEditor;

public class KGFEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		KGFGUIUtilityEditor.RenderKGFInspector(this,this.GetType());
	}
}
