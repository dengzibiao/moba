using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GUISingleItemList : GUIComponentBase
{

    #region Init
    public delegate void VoidItemList(int index);
    public VoidItemList onClick;
    public UISprite A_Sprite;
    protected override void Init()
    {
        A_Sprite = this.GetComponent<UISprite>();
        this.onComponentClick = OnComponentClick;
        this.onComponentHover = OnComponentHover;
        IniComponent();
        InitComponent();


        //Collider c = GetComponent<Collider>();
        //if (null==c) this.AddBoxCollider();
    }

    private void InitComponent()
    {
        System.Type classType = this.GetType();
        FieldInfo[] fields = classType.GetFields();
        foreach (FieldInfo item in fields)
        {
            if (dict.TryGetValue(this.GetType().FullName + item.Name, out cop))
            {
                item.SetValue(this, cop);
            }
        }
        this.InitItem();
    }

    protected override void OnStart()
    {
        base.OnStart();
        RegisterComponent();
        ShowHandler();
    }
    /// <summary>
    /// 注册控件id=界面ID+控件ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="go"></param>
    public void RegisterComponentID(int panleID, int id, GameObject go)
    {
        int b = int.Parse(panleID.ToString() + id.ToString());
      //  Debug.Log(b);
        if (Singleton<GUIManager>.Instance.isContainsKey(b))
        {
            Singleton<GUIManager>.Instance.RemoveComponentID(b);
           // Debug.Log("b is contains  ,be remove!");
        }
        Singleton<GUIManager>.Instance.RegisterComponent(b, go);
    }
    /// <summary>
    /// 注册控件虚方法
    /// </summary>
    protected virtual void RegisterComponent() { }
    /// <summary>
    /// 清空引用 释放内存
    /// </summary>
    public void Clear()
    {
        try
        {
            cop = null;
            dict.Clear();
            dict = null;
        }
        catch (Exception)
        {
        }
    }
    public void OnDestroy()
    {
        Clear();
    }
    public void IniComponent()
    {
        GUIComponentBase[] t = this.transform.GetComponentsInChildren<GUIComponentBase>(true);
        string vname = this.GetType().FullName;
        foreach (GUIComponentBase tt in t)
        {
            string name = StringUtil.ToLower(tt.gameObject.name, 0, 1);
            if (!dict.ContainsKey(vname + name))
            dict.Add(vname + name, tt);

        }

    }
    protected virtual void OnComponentHover(bool state)
    {
        // A_Sprite.color = state ? Color.green : Color.white;
    }

    private void OnComponentClick()
    {
        if (onClick != null) onClick(index);
    }

    public int index { set; get; }
    private bool isInit = false;
    #endregion
    protected virtual void InitItem() { }
    public virtual void Info(object obj) { }
    protected virtual void ShowHandler() { }
    private Dictionary<string, GUIComponentBase> dict = new Dictionary<string, GUIComponentBase>();
    private GUIComponentBase cop = null;
}
