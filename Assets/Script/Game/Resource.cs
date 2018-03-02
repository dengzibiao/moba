/*
文件名（File Name）:   Resource.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using Tianyu;

/// <summary>
/// 资源加载
/// </summary>
public static class Resource
{
    //动态添加预制件到场景对象中
    public static GameObject CreatPrefabs(string name, GameObject parent, Vector3 localPos = default(Vector3), string path = null)
    {
        if (path == null) path = GameLibrary.PATH_UIPrefab;
        GameObject Prefabs = Singleton<ResourceManager>.Instance.LoadPrefab(name, path);
        if (Prefabs == null)
        {
            //   Debug.LogError("Cannot Find " + path + name);
            return null;
        }
        GameObject obj = GameObject.Instantiate(Prefabs) as GameObject;
        if (obj != null && parent != null && localPos != default(Vector3))
        {
            obj.transform.parent = parent.transform;
            obj.transform.localPosition = localPos;
            obj.transform.localRotation = Quaternion.identity;
            obj.name = name;
        }
        else if (obj != null && parent != null)
        {
            obj.transform.parent = parent.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.name = name;
        }
        else if (obj != null)
        {
            obj.transform.localPosition = localPos;
            obj.transform.localRotation = Quaternion.identity;
            obj.name = name;
        }
        if (!name.StartsWith("yx") && !name.StartsWith("zq") && !name.StartsWith("cw"))
        {
            obj.transform.localScale = Vector3.one;
        }

        return obj;
    }

    public static GameObject CreateCharacter(string name, GameObject parent)
    {
        return CreatPrefabs(name, parent, Vector3.zero, GetMonsterModelPath(name));
    }
    public static GameObject CreateCharacter(string name, GameObject parent, Vector3 pos)
    {
        return CreatPrefabs(name, parent, pos, GetMonsterModelPath(name));
    }
    public static GameObject CreateCharacter(CharacterAttrNode attrNode, GameObject parent, Vector3 pos = default(Vector3), int groupIndex = -99)
    {
        if (attrNode == null || attrNode.model == 0) return null;

        string modelName = attrNode.modelNode.modelPath;
        if (attrNode is HeroAttrNode && !GameLibrary.IsMajorOrLogin())
        {
            modelName = attrNode.modelNode.modelLowPath;
        }
        string modelPath = attrNode.modelNode.modelRoot;
        if (groupIndex != -99) modelName = modelName + groupIndex;
        return CreatPrefabs(modelName, parent, pos, modelPath);
    }

    static string GetMonsterModelPath(string modelName)
    {
        return modelName.StartsWith("yx") ? GameLibrary.Hero_URL : GameLibrary.Monster_URL;
    }

    public static string GetNameByPath(int modelID)
    {
        if (modelID == 0) return null;
        ModelNode node = FSDataNodeTable<ModelNode>.GetSingleton().FindDataByType(modelID);
        if (null == node) return null;

        string modelName = "";
        int i = node.respath.LastIndexOf("/");
        modelName = node.respath.Substring(i + 1, node.respath.Length - i - 1);
        return modelName;
    }
}