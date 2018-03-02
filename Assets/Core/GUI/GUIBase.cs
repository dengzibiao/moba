using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public abstract class GUIBase : MonoBehaviour
{
    #region 执行顺序说明
    //  SetUI()
    //  OnLoadData()
    //  ReceiveData()
    //  Init();
    //  RegisterComponent();
    //  RegisterIsOver();
    //  ShowHandler();
    //  OnUpdate
    //  OnRelease
    //  OnDestroy
    #endregion
    #region Init
    /// <summary>
    /// 状态变化事件
    /// </summary>
    public event StateChangedHandler StateChanged;
    private GUIComponentBase cop = null;
    private bool isInit = false;
    public int UIID = 0;//用于新手引导获取不到空间的时候的标记
    public bool IsShow()
    {
        return ((base.enabled && base.isActiveAndEnabled) && base.gameObject.activeInHierarchy);

    }
    //状态.
    protected EnumObjectState state = EnumObjectState.Initial;
    /// <summary>
    /// 状态
    /// </summary>
    public EnumObjectState State
    {
        protected set
        {
            if (state != value)
            {
                EnumObjectState OldState = state;
                state = value;
                if (StateChanged != null)
                {
                    StateChanged(this, state, OldState);
                }
            }
        }
        get { return this.state; }
    }
    public void SetUIWhenOpening(params object[] uiParams)
    {
        //  Debug.LogError("SetUIWhenOpening");
        SetUI(uiParams);
        Game.Instance.StartCoroutine(OnLoadDataAsyn());
    }
    /// <summary>
    /// 主要用于加载之前传送需求你参数
    /// </summary>
    /// <param name="uiParams"></param>
    protected virtual void SetUI(params object[] uiParams)
    {
        // Debug.LogError("SetUI");
        this.State = EnumObjectState.Loading;
    }
    /// <summary>
    /// 用于加载数据
    /// </summary>
    protected virtual void OnLoadData()
    {

    }
    public virtual void ReceiveData(UInt32 messageID)
    {
        this.State = EnumObjectState.Ready;
    }
    void Awake()
    {
        this.State = EnumObjectState.Initial;
        try
        {
            //GUIComponentBase[] cop = this.transform.GetComponentsInChildren<GUIComponentBase>();
            //for (int i = 0; i < cop.Length; i++)
            //{
            //    string name = StringUtil.ToLower(cop[i].gameObject.name, 0, 1);
            //    RegisterComponent(name, cop[i]);
            //}
            //mGUIName = this.GetType().FullName;
            //mPanelId = (UIPanleID)ScenesManage.Instance.GetUIPanleID(mGUIName);
           // Singleton<GUIManager>.Instance.RegisterGUI(mGUIName, this);

            isRepel = false;

            InitComponent();

            //Debug.LogError("Init");
        }
        catch (System.Exception)
        {

        }


    }
    public abstract UIPanleID GetUIKey();
    void Start()
    {
        Init();
        RegisterComponent();
        RegisterIsOver();
        isInit = true;
        ShowHandler();
    }
    void Update()
    {
        if (this.State == EnumObjectState.Ready)
        {
            OnUpdate(Time.deltaTime);
        }
    }
    /// <summary>
    /// 播放打开界面音乐
    /// </summary>
    protected virtual void OnPlayOpenUIAudio()
    {

    }
    /// <summary>
    /// 关闭界面时候回被调用
    /// </summary>
    public void Release()
    {

        this.State = EnumObjectState.Closing;

        GameObject.Destroy(gameObject);//动态记载或删除时开启
        OnRelease();
    }


    /// <summary>
    /// 释放资源，释放变量
    /// </summary>
    protected virtual void OnRelease()
    {
        // Debug.Log("界面关闭 name = " + transform.name);
        this.State = EnumObjectState.None;
        //关闭音乐
        this.OnPlayCloseUIEffectAudio();
        this.OnPlayCloseUIAudio();
    }
    protected virtual void SetDepthToTop()
    {
        Singleton<GUIManager>.Instance.Set2Top(this.gameObject);
    }
    /// <summary>
    /// 销毁的时候调用
    /// </summary>
    protected virtual void OnDoDestroy()
    {
        // Debug.Log("界面释放 name = " + transform.name);
        this.State = EnumObjectState.None;
    }

    protected virtual void OnPlayCloseUIAudio()
    {

    }
    /// <summary>
    /// 关闭音效
    /// </summary>
    protected virtual void OnPlayCloseUIEffectAudio()
    {

    }

    /// <summary>
    /// 播放打开界面音效
    /// </summary>
    protected virtual void OnPlayOpenUIEffectAudio()
    {

    }
    private void InitComponent()
    {
        System.Type classType = this.GetType();
        FieldInfo[] fields = classType.GetFields();
        foreach (FieldInfo item in fields)
        {
            string name = StringUtil.ToUpper(item.Name, 0, 1);
            if (transform.Find(name) != null)
            {
                if (transform.Find(name).GetComponent<GUIComponentBase>() != null)
                {
                    cop = transform.Find(name).GetComponent<GUIComponentBase>();
                    item.SetValue(this, cop);
                }
            }
        }
    }



    /// <summary>
    /// 注册控件id=界面ID+控件ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="go"></param>
    public void RegisterComponentID(int panleID, int id, GameObject go)
    {
        //int a = Control.GetPanleID(name);
        int b = int.Parse(panleID.ToString() + id.ToString());
        Singleton<GUIManager>.Instance.RegisterComponent(b, go);
    }
    public void Show()
    {
        if (gameObject != null)
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
            gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);

    }



    private IEnumerator OnLoadDataAsyn()
    {
        yield return new WaitForSeconds(0);
        if (this.State == EnumObjectState.Loading)
        {
            // Debug.LogError("OnLoadData");
            this.OnLoadData();

        }
    }

    public void ShowOrHide()
    {
        this.gameObject.ShowOrHide();
    }
    /// <summary>
    /// 是否随场景切换销毁
    /// </summary>
    public bool isRepel { set; get; }
    /// <summary>
    /// 清空引用 释放内存
    /// </summary>
    public void Clear()
    {
        try
        {
            cop = null;
        }
        catch (Exception)
        {
        }
    }

    void OnEnable()
    {
        if (isInit)
        {
            ShowHandler();
        }

    }
    void OnDestroy()
    {
        OnDoDestroy();
        Clear();
    }
    #endregion
    protected virtual void Init()
    {
        //播放音乐
        this.OnPlayOpenUIEffectAudio();
        this.OnPlayOpenUIAudio();
    }
    /// <summary>
    /// 注册控件虚方法
    /// </summary>
    protected virtual void RegisterComponent() { }

    protected virtual void RegisterIsOver()
    {
        if (UIID != 0)
        {
            if (UIGuidePanel.Single())
            {
                UIGuidePanel.Single().uicallback();
                UIID = 0;
            }

        }
    }

    protected virtual void OnUpdate(float deltaTime) { }
    /// <summary>
    /// 每次打开界面的时候都会执行
    /// </summary>
    protected virtual void ShowHandler() { }
}
