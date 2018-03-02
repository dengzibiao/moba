using System.Collections;
using UnityEditor;
using UnityEngine;
using System.IO;


//%代表ctrl，#代表Shift，&代表Alt
public class ResourceEdit
{
    [MenuItem("Plugin/打开字体图集选择面板", false, 9)]
    [MenuItem("Assets/Custom/打开字体图集选择面板", false, 0)]
    static public void OpenConnectAtlasPanel ()
    {
        EditorWindow.GetWindow<UIOnKeyRelation>(false, "ConnectAtlasPanel", true);
    }
    [MenuItem("Plugin/断开字体关联", false, 9)]
    [MenuItem("Assets/Custom/断开字体关联", false, 1)]
    public static void CorrectionPublicDisconnectFontFunction ()
    {
        if(NGUISettings.ambigiousFont == null)
        {
            Debug.LogError("对不起！你没有指定字体！");
        }
        else
        {
            CorrectionPublicDisconnectFont();
        }
    }
    [MenuItem("Plugin/重新指定字体", false, 9)]
    [MenuItem("Assets/Custom/重新指定字体", false, 2)]
    public static void CorrectionPublicFontFunction ()
    {
        if(NGUISettings.ambigiousFont == null)
        {
            Debug.LogError("对不起！你没有指定字体！");
        }
        else
        {
            CorrectionOfTheFont();
        }
    }

    [MenuItem("Assets/Custom/替换所有字体", false, 2)]
    public static void CorrectionPublicFontAllFunction ()
    {
        if(NGUISettings.ambigiousFont == null)
        {
            Debug.LogError("对不起！你没有指定字体！");
        }
        else
        {
            CorrectionPublicFont(NGUISettings.ambigiousFont as Font, null);
        }
    }


    private static void SaveDealFinishPrefab ( GameObject go, string path )
    {
        if(File.Exists(path) == true)
        {
            Object prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            PrefabUtility.ReplacePrefab(go, prefab);
        }
        else
        {
            PrefabUtility.CreatePrefab(path, go);
        }
    }


    private static void CorrectionOfTheFont ()
    {

        CorrectionPublicFont(NGUISettings.ambigiousFont as Font, null);
    }

    private static void CorrectionPublicDisconnectFont ()
    {
        CorrectionPublicFont(null, NGUISettings.ambigiousFont as Font);
    }

    private static void CorrectionPublicFont ( Font replace, Font matching )
    {
        if(NGUISettings.ambigiousFont == null)
        {
            Debug.LogError("Select Font Is Null...");
            return;
        }
        else
        {
            Object[] selectObjs = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);
            foreach(Object selectObj in selectObjs)
            {
                GameObject obj = (GameObject)selectObj;
                if(obj == null || selectObj == null)
                {
                    Debug.LogWarning("ERROR:Obj Is Null !!!");
                    continue;
                }
                string path = AssetDatabase.GetAssetPath(selectObj);
                if(path.Length < 1 || path.EndsWith(".prefab") == false)
                {
                    Debug.LogWarning("ERROR:Folder=" + path);
                }
                else
                {
                    Debug.Log("Selected Folder=" + path);
                    GameObject clone = GameObject.Instantiate(obj) as GameObject;
                    UILabel[] labels = clone.GetComponentsInChildren<UILabel>(true);
                    foreach(UILabel label in labels)
                    {
                        if(label.trueTypeFont == matching)
                        {
                            label.trueTypeFont = replace;
                        }
                    }
                    SaveDealFinishPrefab(clone, path);
                    GameObject.DestroyImmediate(clone);
                    Debug.Log("Connect Font Success=" + path);
                }
            }
            AssetDatabase.Refresh();
        }
    }
}