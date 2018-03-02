using UnityEngine;
using UnityEditor;
using System.IO;

public class PrefabEditor
{
    [MenuItem("Prefab/Create Prefabs")]
    static void CreatePrefab ()
    {
        bool replayAll = false;
        
        string[] paths = EditorApplication.currentScene.Split(char.Parse("/"));
        string _sceneName = paths[paths.Length - 1].Replace(".unity", "");
        //string _sceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
        Debug.Log("Application.loadedLevelName=" + Application.loadedLevelName);
        GameObject[] objs = Selection.gameObjects;

        foreach(GameObject go in objs)
        {
            string _path = "Assets/Resource/Map/" + _sceneName + "/";
            Directory.CreateDirectory(_path);
            string localPath = _path + go.name + ".prefab";
            if(!replayAll && AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
            {
                if(EditorUtility.DisplayDialog("Are you sure?",
                    "One of this prefab already exists. Do you want to overwrite it?",
                    "Yes",
                    "No"))
                {
                    replayAll = true;
                    CreateNew(go, localPath);
                }
                else
                {
                    return;
                }
            }
            else
                CreateNew(go, localPath);
        }
    }

    [MenuItem("Prefab/Create Prefabs", true)]
    static bool ValidateCreatePrefab ()
    {
        return Selection.activeGameObject != null;
    }

    static void CreateNew ( GameObject obj, string localPath )
    {
        Object prefab = PrefabUtility.CreateEmptyPrefab(localPath);
        Debug.Log("create prefab :" + localPath);
        PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.Default);
    }
}