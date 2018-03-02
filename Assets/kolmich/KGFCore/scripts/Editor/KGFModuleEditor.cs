using UnityEditor;

[CustomEditor(typeof(KGFModule))]
public class KGFModuleEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		KGFGUIUtilityEditor.RenderKGFInspector(this, this.GetType());
	}
}
