using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AtlasType
{
    WoodenAtlas,
}
public enum ShaderType
{
    ColorGray,
}

public class ResourceManager
{

    private Dictionary<string, UIAtlas> atlas_Dict = new Dictionary<string, UIAtlas>();

    /// <summary>
    /// Resouce.load
    /// </summary>
    private Dictionary<string, Object> ResourceAssets = new Dictionary<string, Object>();//名字
    private static ResourceManager instance = null;
    public static ResourceManager Instance()
    {
        if (instance == null) instance = new ResourceManager();
        return instance;
    }

    public UIAtlas GetUIAtlas(string type)
    {
        UIAtlas ua = null;

        if (!atlas_Dict.ContainsKey(type))
        {
            ua = Resources.Load("Atlas/" + type, typeof(UIAtlas)) as UIAtlas;
            atlas_Dict.Add(type, ua);
        }
        else
        {
            atlas_Dict.TryGetValue(type, out ua);
        }

        return ua;
    }
    public void AddAtlas(string name, UIAtlas ual)
    {
        if (!atlas_Dict.ContainsKey(name))
        {
            atlas_Dict.Add(name, ual);
        }
    }

    public Shader GetShader(string type)
    {
        return Shader.Find(type);
    }

    public GameObject LoadPrefab(string name)
    {
        return Load(name) as GameObject;
    }
    public GameObject LoadPrefab(string name, string path)
    {
        return Load(name, path) as GameObject;
    }
    /// <summary>
    /// 加载Resources文件夹中的对象
    /// </summary>
    /// <returns>The load.</returns>
    /// <param name="name">Name.</param>
    public UnityEngine.Object Load(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        string subName = "";
        if (name.Contains("/"))
        {
            int count = name.Length - name.LastIndexOf("/") - 1;
            subName = name.Substring(name.LastIndexOf("/") + 1, count);
        }
        if (ResourceAssets.ContainsKey(name))
        {
            return ResourceAssets[name];
        }
        UnityEngine.Object obj = Resources.Load(name);

        if (obj != null)
        {
            ResourceAssets.Add(name, obj);
            return obj;
        }
        return null;
    }

    /// <summary>
    /// 加载
    /// </summary>
    /// <param name="name">名字</param>
    /// <param name="path">路径</param>
    /// <returns></returns>
    public UnityEngine.Object Load(string name, string path)
    {
        if (string.IsNullOrEmpty(name)) return null;

        if (name.Length > 0 && name[0] == '/')
            name = name.Remove(0, 1);

        string fullName = path + name;
        if (ResourceAssets.ContainsKey(fullName))
        {
            return ResourceAssets[fullName];
        }
        UnityEngine.Object obj = Resources.Load(fullName);

        if (obj != null)
        {
            ResourceAssets.Add(fullName, obj);
            return obj;
        }
        return null;
    }
    /// <summary>
    /// 卸载
    /// </summary>
    /// <param name="name"></param>
    /// <param name="path"></param>
    public void Unload(string name, string path)
    {
        string fullName = path + name;
        if (ResourceAssets.ContainsKey(fullName))
        {
            Resources.UnloadAsset(ResourceAssets[fullName]);
        }
    }

    public bool ExistOrNot(string name, string path)
    {
        if (string.IsNullOrEmpty(name)) return false;
        if (name.Length > 0 && name[0] == '/')
            name = name.Remove(0, 1);

        string fullName = path + name;
        Object obj = null;
        ResourceAssets.TryGetValue(fullName, out obj);
        if (obj != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
