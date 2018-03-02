// <author>Christoph Hausjell</author>
// <email>christoph.hausjell@kolmich.at</email>
// <date>2011-03-01</date>
// <summary>short summary</summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEngine;

public class KGFGUIUtilityEditor
{
	private static Texture2D itsIconHelp = null;
	private static Texture2D itsIconInfo = null;
	private static Texture2D itsIconWarning = null;
	private static Texture2D itsIconError = null;
	private static Texture2D itsIconOK = null;
	
	[MenuItem ("Edit/KGF/help/documentation")]
	public static void DocumentationMenueLink()
	{
		Application.OpenURL("http://www.kolmich.at/documentation");
	}
	
	[MenuItem ("Edit/KGF/help/forum")]
	public static void ForumMenueLink()
	{
		Application.OpenURL("http://www.kolmich.at/forum");
	}
	
	[MenuItem ("Edit/KGF/help/homepage")]
	public static void HomepageMenueLink()
	{
		Application.OpenURL("http://www.kolmich.at");
	}
	
	
	public static void RenderKGFInspector(Editor theEditor)
	{
		RenderKGFInspector(theEditor,theEditor.target.GetType());
	}
	
	public static void RenderKGFInspector(Editor theEditor, Type theType)
	{
		RenderKGFInspector(theEditor,theType,null);
	}
	
	/// <summary>
	/// renders the default Kolmich game framework inspector window (TitleBar, Default Unity Inspector, Infobox, Buttons)
	/// </summary>
	/// <param name="theEditor">
	/// 	<see cref="System.String"/>
	/// </param>
	public static void RenderKGFInspector(Editor theEditor, Type theType, Action theHandler)
	{
		#region icon loading
		if(itsIconHelp == null)
		{
			itsIconHelp = Resources.Load("KGFCore/textures/help") as Texture2D;
		}
		
		if(itsIconInfo == null)
		{
			itsIconInfo = Resources.Load("KGFCore/textures/info") as Texture2D;
		}
		
		if(itsIconWarning == null)
		{
			itsIconWarning = Resources.Load("KGFCore/textures/warning") as Texture2D;
		}
		
		if(itsIconError == null)
		{
			itsIconError = Resources.Load("KGFCore/textures/error") as Texture2D;
		}
		
		if(itsIconOK == null)
		{
			itsIconOK = Resources.Load("KGFCore/textures/ok") as Texture2D;
		}
		#endregion
		
		//set the look to Unity default
		EditorGUIUtility.LookLikeControls();
		
		KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDecorated, GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(true));
		{
			//render the title of the Inspector
			RenderTitle(theEditor.target);
			
			//render the path and the reference id
			RenderPath(theEditor);
			
			KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
			{
				KGFGUIUtility.BeginHorizontalPadding();
				{
					DrawCustomInspector(theEditor);
//					DrawCustomInspectorReflection(theEditor.target,theEditor.target,0);
//					theEditor.DrawDefaultInspector();
					if (theHandler != null)
						theHandler();
				}
				KGFGUIUtility.EndHorizontalPadding();
			}
			KGFGUIUtility.EndVerticalBox();
			
			// check if the object is a prefab
			PrefabType aPrefabType = PrefabUtility.GetPrefabType(theEditor.target);
			bool theIsPrefab = !(aPrefabType == PrefabType.PrefabInstance || aPrefabType == PrefabType.None || aPrefabType == PrefabType.DisconnectedPrefabInstance);
			
			// draw custom inspector gui
			RenderObjectCustomGui(theEditor.target,theIsPrefab);
			
			// draw error checking gui
			RenderInspectorErrorChecking(theEditor.target);
			
			// help button
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkBottom);
			{
				if(KGFGUIUtility.Button(itsIconHelp, "documentation", KGFGUIUtility.eStyleButton.eButton, GUILayout.ExpandWidth(true)))
				{
					Application.OpenURL("http://www.kolmich.at/documentation/");
				}
				
				if(KGFGUIUtility.Button(itsIconHelp, "forum", KGFGUIUtility.eStyleButton.eButton, GUILayout.ExpandWidth(true)))
				{
					Application.OpenURL("http://www.kolmich.at/forum");
				}
				
				if(KGFGUIUtility.Button(itsIconHelp, "homepage", KGFGUIUtility.eStyleButton.eButton, GUILayout.ExpandWidth(true)))
				{
					Application.OpenURL("http://www.kolmich.at/");
				}
			}
			KGFGUIUtility.EndHorizontalBox();
		}
		KGFGUIUtility.EndVerticalBox();
	}
	
	/// <summary>
	/// Draw custom inspector window
	/// </summary>
	/// <param name="theEditor"></param>
	static void DrawCustomInspector(Editor theEditor)
	{
		bool anIsCustomType;
		
		SerializedObject aSerializedObject = new SerializedObject(theEditor.target);
		aSerializedObject.Update();
		
		SerializedProperty aProperty = aSerializedObject.GetIterator();
		if (aProperty.NextVisible(true))
		{
			do{
				// TODO: check for ErrorAttributes and check the errors (attribute classes are commented out at the end of this file)
				
				// draw space before field
				EditorGUILayout.BeginHorizontal();
				for (int i=0;i<aProperty.depth;i++)
					GUILayout.Space(10);
				// draw field
				DrawCustomInspectorField(theEditor.target,aProperty,out anIsCustomType);
				EditorGUILayout.EndHorizontal();
			}while(aProperty.NextVisible(aProperty.isExpanded && !anIsCustomType));
		}
		aSerializedObject.ApplyModifiedProperties();
	}
	
	/// <summary>
	/// Draw custom inspector window
	/// </summary>
	/// <param name="theEditor"></param>
	static void DrawCustomInspectorReflection(UnityEngine.Object theTarget, object theInstance, int theLevel)
	{
		Type aTargetType = theInstance.GetType();
		foreach (FieldInfo aField in aTargetType.GetFields())
		{
			if (aField.IsNotSerialized)
				continue;
			if (CheckForAttribute<HideInInspector>(aField.GetCustomAttributes(true)))
				continue;
			if (aField.IsInitOnly)
				continue;
			
			bool aFoundType;
			// draw field
			DrawCustomInspectorReflectionField(theTarget,aField,theInstance,out aFoundType);
			if (!aFoundType)
			{
				EditorGUILayout.Foldout(true,aField.Name);
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Space(10*theLevel);
					EditorGUILayout.BeginVertical();
					{
						DrawCustomInspectorReflection(theTarget,aField.GetValue(theInstance),theLevel+1);
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
			}
		}
	}
	
	/// <summary>
	/// Check for attribute
	/// </summary>
	/// <param name="theMethodInfo"></param>
	/// <returns></returns>
	static bool CheckForAttribute<T> (object[] theAttributes) where T : Attribute
	{
		if (theAttributes == null)
		{
			return false;
		}
		for (int i = 0; i < theAttributes.Length; i ++)
		{
			if (theAttributes [i] is T)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// SetDirty on target if value changed
	/// </summary>
	/// <param name="theNewValue"></param>
	/// <param name="theOldValue"></param>
	/// <returns>the new value</returns>
	static T ChangeCheck<T>(UnityEngine.Object theTarget, T theNewValue,T theOldValue)
	{
		if ((""+theNewValue) != (""+theOldValue))
		{
			EditorUtility.SetDirty(theTarget);
		}
		
		return theNewValue;
	}
	
	/// <summary>
	/// Draw field for property.
	/// Add custom code for some datatypes here
	/// </summary>
	/// <param name="theProperty"></param>
	static void DrawCustomInspectorField(UnityEngine.Object theTarget, SerializedProperty theProperty,out bool theIsCustomType)
	{
		// check for datatypes with custom handling
		if (theProperty.type == "KGFEventData")
		{
		}else
		{
			// use default handling
			EditorGUILayout.PropertyField(theProperty,false);
			theIsCustomType = false;
			return;
		}
		theIsCustomType = true;
	}
	
	static void DrawCustomInspectorReflectionField(UnityEngine.Object theTarget, FieldInfo theField, object theInstance, out bool theFoundType)
	{
		object theNewValue = DrawField(theTarget,theField.FieldType,theField.Name,theField.GetValue(theInstance),out theFoundType);
		if (theFoundType)
		{
			theField.SetValue(theInstance,theNewValue);
		}
	}
	
	/// <summary>
	/// Draw a single parameter field
	/// </summary>
	/// <param name="theTarget">the editor target</param>
	/// <param name="theFieldType">type information of the value</param>
	/// <param name="theName">the field label</param>
	/// <param name="theValue">the field value</param>
	public static object DrawField(UnityEngine.Object theTarget, Type theFieldType, string theName, object theValue)
	{
		bool aFoundType;
		return DrawField(theTarget,theFieldType,theName,theValue,out aFoundType);
	}
	
	/// <summary>
	/// Draw a single parameter field
	/// </summary>
	/// <param name="theTarget">the editor target</param>
	/// <param name="theFieldType">type information of the value</param>
	/// <param name="theName">the field label</param>
	/// <param name="theValue">the field value</param>
	public static object DrawField(UnityEngine.Object theTarget, Type theFieldType, string theName, object theValue, out bool theFoundType)
	{
		theFoundType = true;
		if (typeof(UnityEngine.Object).IsAssignableFrom(theFieldType))
		{
			// if the parameter is derrived from unityengine.object, display an object field
			UnityEngine.Object aObject = EditorGUILayout.ObjectField(theName, (UnityEngine.Object)theValue, theFieldType,true);
			if (aObject != (UnityEngine.Object)theValue)
			{
				EditorUtility.SetDirty(theTarget);
				return aObject;
			}
		} else
		{
			// Search for parameter with right name in EventParameter script and display it
			if (typeof(int).IsAssignableFrom(theFieldType))
			{
				return ChangeCheck<int>(theTarget,EditorGUILayout.IntField(theName,(int)theValue),(int)theValue);
			}
			else if (typeof(double).IsAssignableFrom(theFieldType))
			{
				return (double)ChangeCheck<double>(theTarget,(double)EditorGUILayout.FloatField(theName,(float)((double)theValue)),(double)theValue);
			}
			else if (typeof(float).IsAssignableFrom(theFieldType))
			{
				return ChangeCheck<float>(theTarget,EditorGUILayout.FloatField(theName,(float)theValue),(float)theValue);
			}
			else if (typeof(string).IsAssignableFrom(theFieldType))
			{
				if (theValue == null)
					theValue = "";
				return ChangeCheck<string>(theTarget,EditorGUILayout.TextField(theName,(string)theValue),(string)theValue);
			}
			else if (typeof(Color).IsAssignableFrom(theFieldType))
			{
				return ChangeCheck<Color>(theTarget,EditorGUILayout.ColorField(theName,(Color)theValue),(Color)theValue);
			}
			else if (typeof(Rect).IsAssignableFrom(theFieldType))
			{
				return ChangeCheck<Rect>(theTarget,EditorGUILayout.RectField(theName,(Rect)theValue),(Rect)theValue);
			}
			else if (typeof(Vector2).IsAssignableFrom(theFieldType))
			{
				return ChangeCheck<Vector2>(theTarget,EditorGUILayout.Vector2Field(theName,(Vector2)theValue),(Vector2)theValue);
			}
			else if (typeof(Vector3).IsAssignableFrom(theFieldType))
			{
				return ChangeCheck<Vector3>(theTarget,EditorGUILayout.Vector3Field(theName,(Vector3)theValue),(Vector3)theValue);
			}
			else if (typeof(Vector4).IsAssignableFrom(theFieldType))
			{
				return ChangeCheck<Vector4>(theTarget,EditorGUILayout.Vector4Field(theName,(Vector4)theValue),(Vector4)theValue);
			}
		}
		theFoundType = false;
		return theValue;
	}

	/// <summary>
	/// renders a title bar with image and empty title text
	/// </summary>
	/// <param name="theTitle">
	/// A <see cref="System.String"/>
	/// </param>
	public static void RenderTitle(UnityEngine.Object theTarget)
	{
		RenderTitle(theTarget, string.Empty);
	}

	/// <summary>
	/// renders a title bar with image and title text
	/// </summary>
	/// <param name="theTitle">
	/// A <see cref="System.String"/>
	/// </param>
	public static void RenderTitle(UnityEngine.Object theTarget, string theTitle)
	{
		//get the icon of the module
		Texture2D aTexture = null;
		
		if (theTarget is KGFModule)
		{
			aTexture = (theTarget as KGFModule).GetIcon();
		}
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop, GUILayout.ExpandWidth(true));
		{
			//ident the tile with a seperator right
			string aTitle = "\t";
			
			//check if the Target is null
			if(theTarget != null)
			{
				//append the type of the target to the title
				aTitle += theTarget.GetType().ToString();
			}
			
			if(theTitle != string.Empty)
			{
				//append the titlestring to the title
				aTitle += theTitle;
			}

			KGFGUIUtility.Label(aTitle, aTexture, KGFGUIUtility.eStyleLabel.eLabel, GUILayout.ExpandWidth(true));
		}
		KGFGUIUtility.EndHorizontalBox();
	}

	public static void RenderPath(Editor theEditor)
	{
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBox);
		{
			if (theEditor.target is Component)
			{
				RenderPath((Component)theEditor.target);
			}
		}
		KGFGUIUtility.EndHorizontalBox();
	}

	/// <summary>
	/// renders the gameobject path up to the root
	/// </summary>
	/// <param name="theComponent"></param>
	public static void RenderPath(Component theComponent)
	{
		GameObject aGameObject = theComponent.gameObject;
		
		// generate path for the gameObject
		string aPath = aGameObject.name;
		
		while (aGameObject.transform.parent != null)
		{
			aGameObject = aGameObject.transform.parent.gameObject;
			aPath = string.Format("{0}/{1}", aGameObject.name, aPath);
		}
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
		{
			KGFGUIUtility.Label("path",KGFGUIUtility.eStyleLabel.eLabel, GUILayout.ExpandWidth(false));
			
			if (PrefabUtility.GetPrefabType(theComponent) == PrefabType.Prefab)
			{
				KGFGUIUtility.TextField(AssetDatabase.GetAssetPath(theComponent), KGFGUIUtility.eStyleTextField.eTextField, GUILayout.ExpandWidth(true));
			}
			else
			{
				KGFGUIUtility.TextField(aPath, KGFGUIUtility.eStyleTextField.eTextField, GUILayout.ExpandWidth(true));
			}
		}
		KGFGUIUtility.EndHorizontalBox();
	}

	/// <summary>
	/// Render a custom gui for each object
	/// </summary>
	/// <param name="theTarget"></param>
	/// <param name="theIsPrefab"></param>
	static void RenderObjectCustomGui(UnityEngine.Object theTarget, bool theIsPrefab)
	{
		KGFICustomInspectorGUI anObjectScript = theTarget as KGFICustomInspectorGUI;
		if (anObjectScript != null)
		{
			KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical, GUILayout.ExpandWidth(true));
			anObjectScript.DrawInspectorGUI(theTarget,theIsPrefab);
			KGFGUIUtility.EndVerticalBox();
		}
	}

	/// <summary>
	/// renders a section for found errors in the inspector
	/// </summary>
	/// <param name="theTarget">the currently displayed data object</param>
	public static void RenderInspectorErrorChecking(UnityEngine.Object theTarget)
	{
		KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical, GUILayout.ExpandWidth(true));
		{
			KGFMessageList aMessageList = null;
			KGFIValidator anObjectScript = theTarget as KGFIValidator;
			
			if(anObjectScript != null)
			{
				aMessageList = anObjectScript.Validate();
				
				// render infos
				bool aShowOK = true;
				
				KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxInvisible, GUILayout.ExpandWidth(true));
				{
					
					#region render info
					
					/*
					string[] aInfoList = aMessageList.GetInfoArray();
					
					if (aInfoList.Length > 0)
					{
						aShowOK = false;
						
						foreach (string aMessage in aInfoList)
						{
							RenderWarning(aMessage);
						}
					}
					 */
					
					#endregion
					
					#region render errors
					string[] anErrorList = aMessageList.GetErrorArray();
					if (anErrorList.Length > 0)
					{
						aShowOK = false;
						
						foreach (string aMessage in anErrorList)
						{
							RenderError(aMessage);
						}
					}
					#endregion
					
					#region render warnings
					anErrorList = aMessageList.GetWarningArray();
					if (anErrorList.Length > 0)
					{
						aShowOK = false;
						foreach (string aMessage in anErrorList)
						{
							RenderWarning(aMessage);
						}
					}
					#endregion
					
					if(aShowOK)
					{
						RenderOK();
					}
				}
				KGFGUIUtility.EndVerticalBox();
			}
			else
			{
				RenderError("the module doesn`t implement the KGFIValidator interface");
			}
		}
		KGFGUIUtility.EndVerticalBox();
	}

	/// <summary>
	/// renders warning box in warning color
	/// </summary>
	/// <param name="theWarning">
	/// A <see cref="System.String"/>
	/// </param>
	public static void RenderWarning(string theWarning)
	{
		SetColorWarning();
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBox, GUILayout.ExpandWidth(true));
		{
			KGFGUIUtility.Label(theWarning, itsIconWarning, KGFGUIUtility.eStyleLabel.eLabel, GUILayout.ExpandWidth(true));
		}
		KGFGUIUtility.EndHorizontalBox();
		SetColorDefault();
	}

	/// <summary>
	/// renders error box in error color
	/// </summary>
	/// <param name="theError">
	/// A <see cref="System.String"/>
	/// </param>
	public static void RenderError(string theError)
	{
		SetColorError();
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBox, GUILayout.ExpandWidth(true));
		{
			KGFGUIUtility.Label(theError, itsIconError, KGFGUIUtility.eStyleLabel.eLabel, GUILayout.ExpandWidth(true));
		}
		KGFGUIUtility.EndHorizontalBox();
		SetColorDefault();
	}

	/// <summary>
	/// renders error box in error color
	/// </summary>
	/// <param name="theError">
	/// A <see cref="System.String"/>
	/// </param>
	public static void RenderOK()
	{
		SetColorOK();
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBox, GUILayout.ExpandWidth(true));
		{
			KGFGUIUtility.Label("All component values are valid", itsIconOK, KGFGUIUtility.eStyleLabel.eLabel, GUILayout.ExpandWidth(true));
		}
		KGFGUIUtility.EndHorizontalBox();
		SetColorDefault();
	}

	#region set colors

	public static void SetColorOK()
	{
		GUI.backgroundColor = Color.green;
		GUI.contentColor = Color.white;
	}

	public static void SetColorError()
	{
		GUI.backgroundColor = Color.red;
		GUI.contentColor = Color.white;
	}

	public static void SetColorWarning()
	{
		GUI.backgroundColor = Color.yellow;
		GUI.contentColor = Color.white;
	}

	public static void SetColorInfo()
	{
		GUI.backgroundColor = Color.white;
		GUI.contentColor = Color.white;
	}

	public static void SetColorDefault()
	{
		GUI.backgroundColor = Color.white;
		GUI.contentColor = Color.white;
	}

	#endregion
}

//public class RangeCheckAttribute : System.Attribute
//{
//	float itsMin,itsMax;
//
//	public RangeCheckAttribute(float theMin,float theMax)
//	{
//		itsMin = theMin;
//		itsMax = theMax;
//	}
//
//	public float GetMin()
//	{
//		return itsMin;
//	}
//
//	public float GetMax()
//	{
//		return itsMax;
//	}
//}
//
//public class NullCheckAttribute : System.Attribute
//{}