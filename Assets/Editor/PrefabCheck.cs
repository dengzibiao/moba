using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class PrefabCheck : MonoBehaviour
{
    // 在菜单来创建 选项 ， 点击该选项执行搜索代码  
    [MenuItem("Tools/Check Font")]
    static void CheckSceneSetting ()
    {
        List<string> dirs = new List<string>();
        GetDirs(Application.dataPath, ref dirs);

    }
    //参数1 为要查找的总路径， 参数2 保存路径  
    private static void GetDirs ( string dirPath, ref List<string> dirs )
    {
        foreach(string path in Directory.GetFiles(dirPath))
        {
            //获取所有文件夹中包含后缀为 .prefab 的路径  
            if(System.IO.Path.GetExtension(path) == ".prefab")
            {
                dirs.Add(path.Substring(path.IndexOf("Assets")));
                GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(path.Substring(path.IndexOf("Assets")), typeof(GameObject));
                if(obj != null)
                {
                    GameObject instance = Instantiate(obj);
                    UILabel[] labels = instance.GetComponentsInChildren<UILabel>();
                    foreach(UILabel label in labels)
                    {
                        if(label.trueTypeFont != null && label.trueTypeFont.name.Contains("Lucida"))
                        {
                            Debug.Log(label.trueTypeFont.name);
                            Debug.Log(path.Substring(path.IndexOf("Assets")));
                        }
                        if(label.bitmapFont != null && label.bitmapFont.name.Contains("Lucida"))
                        {
                            Debug.Log(label.bitmapFont.name);
                            Debug.Log(path.Substring(path.IndexOf("Assets")));
                        }
                    }
                    DestroyImmediate(instance);
                }
                else
                {
                    Debug.LogError(path.Substring(path.IndexOf("Assets")));
                }


            }
        }

        if(Directory.GetDirectories(dirPath).Length > 0)  //遍历所有文件夹  
        {
            foreach(string path in Directory.GetDirectories(dirPath))
            {
                GetDirs(path, ref dirs);
            }
        }
    }
}