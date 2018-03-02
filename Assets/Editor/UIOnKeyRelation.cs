using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UIOnKeyRelation : EditorWindow
{
    /// <summary> 保存到内存.</summary>
    void OnSelectNewFont ( Object obj )
    {
        NGUISettings.ambigiousFont = obj as Font;
        Repaint();
    }

    void OnSelectAtlas ( Object obj )
    {
        NGUISettings.atlas = obj as UIAtlas;
        Repaint();
    }
    /// <summary> 刷新窗口. </summary>
    void OnSelectionChange () { Repaint(); }

    public static bool IsSetNullFont;

    /// <summary>UI绘制区域.</summary>
    void OnGUI ()
    {
        try
        {
            EditorGUIUtility.labelWidth = 80f;
            NGUIEditorTools.DrawHeader("选择断开(关联)字体");
            ComponentSelector.Draw<Font>("Font", (Font)NGUISettings.ambigiousFont, OnSelectNewFont, true, GUILayout.MinWidth(200f));

            //EditorGUIUtility.labelWidth = 160f;
            //NGUIEditorTools.DrawHeader("选择图集");
            //ComponentSelector.Draw<UIAtlas>("UIAtlas", NGUISettings.atlas, OnSelectAtlas, true, GUILayout.MinWidth(200f));

            NGUIEditorTools.DrawSeparator();
        }
        catch(System.Exception ex)
        {
            Debug.LogError(ex.Message);
            NGUISettings.ambigiousFont = null;
            NGUISettings.atlas = null;
        }
    }
}