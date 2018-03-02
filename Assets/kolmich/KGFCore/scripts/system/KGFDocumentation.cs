using UnityEngine;
using System.Collections;
using System.IO;

public class KGFDocumentation : MonoBehaviour 
{	
	public void OpenDocumentation()
	{
		string aDocumentationPath = Application.dataPath;
		aDocumentationPath = Path.Combine(aDocumentationPath,"kolmich");
		aDocumentationPath = Path.Combine(aDocumentationPath,"documentation");		
		aDocumentationPath = Path.Combine(aDocumentationPath,"files");		
		aDocumentationPath += Path.DirectorySeparatorChar;
		aDocumentationPath += "documentation.html";
		aDocumentationPath.Replace('/',Path.DirectorySeparatorChar);		
		Application.OpenURL ("file://"+aDocumentationPath);
	}
}
