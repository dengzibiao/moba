/*
文件名（File Name）:   BaseScene.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseScene : MonoBehaviour
{
    private EnumSceneID _current = EnumSceneID.Null;
    private string _name = null;

    protected GameObject go = null;

    public EnumSceneID SceneID
    {
        get { return _current; }
        set
        {
            if (_current== value)return; 
            _current = value; Singleton<SceneManage>.Instance.Regist(_current);
        }
    }

    public string Name
    {
        get { return _name; }
        set
        {
            if (value != null)
            {
                _name = value;
            }
        }
    }
    void Awake()
    {
        try
        {
            go = GameObject.Find("UI Root").gameObject;
            if(go==null)go = GameObject.Find("UI Root(Clone)").gameObject;
        }
        catch (Exception)
        {
                       
        }      
        InitState();
        Init();
        //GameLibrary.isInit = true;
    }
    //public void RegistUI(string name,UIPanleID panleID, string path = null, int a=0, int b = 0, int c = 0, string parent = null)
    //{
    //    if (!string.IsNullOrEmpty(parent)) go = GameObject.Find(parent);
    //    ScenesManage.Instance.RegisterSceneUI(name, panleID, _current, go,  a,  b,  c , path);
    //}
    /// <summary>
    /// parent可以为null,也可以自定义,float为vector3.posation的参数
    /// </summary>
    /// <param name="name"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="parent"></param>
    public void CreatPrefab(string name,float a, float b, float c, GameObject parent = null,string path=null)
    {
        Vector3 aa = new Vector3(a, b, c);
        if (aa != default(Vector3))
        {
            ScenesManage.Instance.CreatPrefab(name, parent, aa, path);
        }
        else
        {
            aa = default(Vector3);
            ScenesManage.Instance.CreatPrefab(name, parent, aa, path);
        }

    }
    ///// <summary>
    ///// 重新加载此处需要进一步处理，每次从副本跳转主城时可能会有问题
    ///// </summary>
    //private void Reload()
    //{
    //    for (int i = 0; i < ScenesManage.Instance.GetSceneInfo((int)_current).Count; i++)
    //    {
    //        ScenesManage.Instance.CreatPrefab(ScenesManage.Instance.GetSceneInfo((int)_current)[i], go);
    //    }
    //}
    ////void OnEnable()
    ////{
    ////    Debug.LogError(GameLibrary.isInit);
    ////    if (GameLibrary.isInit && EnumSceneID.UI_MajorCity.ToString() == Application.loadedLevelName)
    ////    {
    ////        Reload();
    ////    }

    ////}
    protected virtual void Init() { }
    protected virtual void InitState() { }

}
