using UnityEngine;
using System.Collections;

public class KGFGUIUtilityTutorial : MonoBehaviour
{
	void OnGUI ()
	{
		int aWidth = 300;
		int aHeight = 250;
		
		Rect aRect = new Rect((Screen.width - aWidth) / 2, (Screen.height - aHeight) / 2, aWidth, aHeight);
		
		GUILayout.BeginArea(aRect);
		{
			KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxInvisible, GUILayout.ExpandHeight(true));
			{
				KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
				{
					GUILayout.FlexibleSpace();
					KGFGUIUtility.Label("KGFGUIUtility Tutorial", KGFGUIUtility.eStyleLabel.eLabel);
					GUILayout.FlexibleSpace();
				}
				KGFGUIUtility.EndHorizontalBox();
				
				KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical, GUILayout.ExpandHeight(true));
				{
					GUILayout.FlexibleSpace();
					
					KGFGUIUtility.BeginHorizontalPadding();
					{
						KGFGUIUtility.Button("Top", KGFGUIUtility.eStyleButton.eButtonTop, GUILayout.ExpandWidth(true));
						KGFGUIUtility.Button("Middle", KGFGUIUtility.eStyleButton.eButtonMiddle, GUILayout.ExpandWidth(true));
						KGFGUIUtility.Button("Bottom", KGFGUIUtility.eStyleButton.eButtonBottom, GUILayout.ExpandWidth(true));
					}
					KGFGUIUtility.EndHorizontalPadding();
					
					GUILayout.FlexibleSpace();
				}
				KGFGUIUtility.EndVerticalBox();
				
				KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkBottom);
				{
					KGFGUIUtility.BeginVerticalPadding();
					{
						KGFGUIUtility.Button("Left", KGFGUIUtility.eStyleButton.eButtonLeft, GUILayout.ExpandWidth(true));
						KGFGUIUtility.Button("Center", KGFGUIUtility.eStyleButton.eButtonMiddle, GUILayout.ExpandWidth(true));
						KGFGUIUtility.Button("Right", KGFGUIUtility.eStyleButton.eButtonRight, GUILayout.ExpandWidth(true));
					}
					KGFGUIUtility.EndVerticalPadding();
				}
				KGFGUIUtility.EndHorizontalBox();
			}
			KGFGUIUtility.EndVerticalBox();
		}
		GUILayout.EndArea();
	}
}