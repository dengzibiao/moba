using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KGFDocumentation))]
public class KGFDocumentationEditor : Editor
{	
	public void OnEnable()
	{
		Debug.LogError("#2");
		KGFDocumentation aDocumentation = target as KGFDocumentation;
		aDocumentation.OpenDocumentation();
	}	
}
