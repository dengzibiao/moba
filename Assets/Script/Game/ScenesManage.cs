/*
文件名（File Name）:   ScenesManage.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/

using UnityEngine;
using System.Collections.Generic;
using System;

public class ScenesManage
{

    //场景id和文件名称
    public Dictionary<int, Dictionary<int, string>> dicSceneList = null;
    protected GameObject go = null;
    /// <summary>
    /// The instance.
    /// </summary>
    private static ScenesManage instance = null;
    public static ScenesManage Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ScenesManage();
            }
            return instance;
        }
    }
    //public string GetUIPanleName(int id)
    //{
    //    string name = null;
    //    int sceneId = (int)Singleton<SceneManage>.Instance.Current;
    //    Dictionary<int, string> ls = null;
    //    ls = GetSceneInfo((int)sceneId);
    //    if (ls != null)
    //    {
    //        ls.TryGetValue(id, out name);
    //    }
    //    return name;
    //}
    //public int GetUIPanleID(string name)
    //{
    //    int panleID = -1;
    //    int sceneId = (int)Singleton<SceneManage>.Instance.Current;
    //    Dictionary<int, string> ls = null;
    //    ls = GetSceneInfo((int)sceneId);
    //    if (ls != null)
    //    {
    //        foreach (KeyValuePair<int, string> key in ls)
    //        {
    //            if (key.Value == name)
    //            {
    //                panleID = key.Key;
    //                return panleID;
    //            }
    //        }
    //    }
    //    return panleID;
    //}
    public static void Release()
    {
        ScenesManage.instance.dicSceneList.Clear();
        ScenesManage.instance.dicSceneList = null;
        instance = null;
    }

    private ScenesManage()
    {
        dicSceneList = new Dictionary<int, Dictionary<int, string>>();
    }
    ///// <summary>
    ///// 注册场景到集合
    ///// </summary>
    ///// <param name="sceneId"></param>
    ///// <param name="fileName"></param>
    //public void RegisterSceneUI(string name, UIPanleID Panle, EnumSceneID sceneId, GameObject parent = null, int a = 0, int b = 0, int c = 0, string path = null)
    //{
    //    if (parent != null) go = parent;
    //    if (go == null)
    //    {
    //        try
    //        {
    //            go = GameObject.Find("UI Root").gameObject;
    //            if (go == null) go = GameObject.Find("UI Root(Clone)");
    //        }
    //        catch (Exception)
    //        {


    //        }
    //    }
    //    Vector3 aa = new Vector3(a, b, c);
    //    //CreatPrefab(name, go, aa, path);
    //    //场景ID存在
    //    if (dicSceneList.ContainsKey((int)sceneId))
    //    {//UI列表
    //        Dictionary<int, string> ls = null;
    //        ls = GetSceneInfo((int)sceneId);

    //        if (ls.Count > 0)
    //        {
    //            if (go != null)
    //            {
    //                foreach (var l in ls.Values)
    //                {
    //                    if (!go.transform.Find(l))
    //                    {
    //                        CreatPrefab(l, go, aa, path);
    //                    }
    //                }

    //            }

    //        }
    //        if (!ls.ContainsKey((int)Panle))
    //        {
    //            CreatPrefab(name, go, aa, path);
    //            ls.Add((int)Panle, name);

    //        }
    //    }
    //    else
    //    {
    //        Dictionary<int, string> ls = new Dictionary<int, string>();
    //        ls.Add((int)Panle, name);
    //        dicSceneList.Add((int)sceneId, ls);
    //        CreatPrefab(name, go, aa, path);

    //    }
    //}
    /// <summary>
    /// 名字+父级，LocalPosation,加载路径
    /// </summary>
    /// <param name="name"></param>
    /// <param name="go"></param>
    public void CreatPrefab(string name, GameObject go, Vector3 vc3 = default(Vector3), string path = null)
    {
        Resource.CreatPrefabs(name, go, vc3, path);
    }
    /// <summary>
    /// 移除场景数据
    /// </summary>
    /// <param name="sceneId"></param>
    public void RemoveScene(EnumSceneID sceneId)
    {
        if (dicSceneList.ContainsKey((int)sceneId))
        {
            dicSceneList.Remove((int)sceneId);
        }
    }

    /// <summary>
    /// 获取场景UI数据列表
    /// </summary>
    /// <returns>The scene data.</returns>
    /// <param name="fileName">File name.</param>
    public Dictionary<int, string> GetSceneInfo(int sceneId)
    {
        if (dicSceneList.ContainsKey(sceneId))
        {
            return dicSceneList[sceneId];
        }
        return null;
    }
    /// <summary>
    /// 当前场景是否已经加载过界面true为已加载
    /// </summary>
    /// <param name="name"> 名字为空也返回false</param>
    /// <returns></returns>
    public bool ExistOrNot(string name, string path)
    {
        if (path == null)
        {
            path = GameLibrary.PATH_UIPrefab;
        }
        return Singleton<ResourceManager>.Instance.ExistOrNot(name, path);
    }
    /// <summary>
    /// 根据场景名称获取文件名称
    /// </summary>
    /// <returns>The scene file name.</returns>
    /// <param name="sceneName">SceneID name.</param>
    public string GetSceneFileName(int sceneId)
    {
        if (dicSceneList.ContainsKey(sceneId))
        {
            return dicSceneList[sceneId].ToString();
        }
        Debug.Log("此场景没有注册 " + sceneId);
        return null;
    }

    /// <summary>
    /// 是否注过册此场景
    /// </summary>
    /// <returns><c>true</c>, if register scene was ised, <c>false</c> otherwise.</returns>
    public bool isRegisterScene(int sceneId)
    {
        if (dicSceneList.ContainsKey(sceneId))
        {
            return true;
        }
        return false;
    }
}
