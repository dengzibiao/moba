using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Tianyu;
using System;

public class UI_Loading : GUIBase
{
    public GUISingleProgressBar progressBar;
    public Texture2D[] tex;
    private string[] des = new string[] { "当你不知道做什么的时候，就点击左侧的任务追踪吧！", "在祭坛可以抽取当前热点英雄哦！", "当你血量不足时，屏幕会出现红色提示的哦！" };
    int index = 0;
    //异步加载场景
    private AsyncOperation async;
    private AsyncOperation op = null;
    private bool isExit = false;
    private UILabel label;

    [HideInInspector]
    public MobaMatched mobaMatched;
    UITexture texture;

    protected override void Init()
    {
        this.isRepel = true;

        texture = transform.Find("BG").GetComponent<UITexture>();
        label = transform.Find("Label").GetComponent<UILabel>();
        //texture.SetAnchor(transform.parent.gameObject, 0, 0, 0, 0);
        //if (GameLibrary.SceneType(SceneType.MB1))
        //{
        //    //transform.FindChild("ProgressBar_circle").gameObject.SetActive(true);
        //    GameObject mobaMatchedGo = Resources.Load<GameObject>("Prefab/UIPanel/MobaMatched");
        //    mobaMatched = NGUITools.AddChild(gameObject, mobaMatchedGo).GetComponent<MobaMatched>();
        //    mobaMatched.GetComponent<UIPanel>().depth = UIPanel.nextUnusedDepth + 1;
        //    mobaMatched.Init(Globe.mobaMyTeam, Globe.mobaEnemyTeam);
        //}
        //else if (GameLibrary.SceneType(SceneType.MB3))
        //{

        //}
        //else
        //{
        //}
        transform.FindChild("ProgressBar_liner").gameObject.SetActive(true);

        progressBar = GetComponentInChildren<GUISingleProgressBar>();
        progressBar.onChange = OnProgressBarChange;
        //if (Globe.LoadScenceName != GameLibrary.PVP_Moba)
        //    progressBar.maxWidth = 795;
        progressBar.maxValue = 100;
        progressBar.currentValue = 0;

        //随机背景图
        TimeSpan tiimeSpan = new TimeSpan();
        if (!string.IsNullOrEmpty(serverMgr.GetInstance().GetCreateAccountTime()))
            tiimeSpan = TimeManager.Instance.CheckTimeNowadays(serverMgr.GetInstance().GetCreateAccountTime(), false);
        //currentyMd = TimeManager.Instance.CheckTimeIsNowadays(serverMgr.GetInstance().GetCreateAccountTime(), false, true);
        if (tiimeSpan.Hours < 1)
            index = UnityEngine.Random.Range(0, 4);
        else
            index = UnityEngine.Random.Range(0, tex.Length);
        texture.mainTexture = tex[index];

        //随机文字小提示
        int desindex = UnityEngine.Random.Range(0, des.Length);
        label.text = des[desindex];
        //切换场景打断声音
        AudioController.Instance.StopUISound();
        //清空所有的战斗声音
        AudioController.Instance.ClearEffectSound();
        StartCoroutine(StartLoading());
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    /// <summary>
    /// 进度条
    /// </summary>
    /// <param name="percent"></param>
    private void OnProgressBarChange(float percent)
    {
        if (percent >= 1)
        {
            Globe.isOnce = true;
            if (Globe.completed != null)
            {
                Globe.completed();
            }
            op.allowSceneActivation = true;
            // serverMgr.setloginstate(-1);
        }
    }

    /// <summary>
    /// 跳转场景
    /// 时间控制的为加载条最后百分之10的时间
    /// </summary>
    /// <param name="SceneName">场景名称</param>
    /// <param name="LoadTime">最后百分之10加载用的时间</param>
    /// <param name="callBack">百分之90时的回调</param>
    /// <param name="completed">加载完成之后的回调</param>
    public static void LoadScene(string SceneName, float LoadTime, CallBack callBack = null, CallBack completed = null)
    {
        // StartLandingShuJu.setloginstate(1);
        //从Resources中加载实例化加载界面
        //Instantiate(Resources.Load("Prefab/UIPanel/UILoading"), Vector3.zero, Quaternion.identity);

        //if (null != loadGO)
        //{
        Control.ShowGUI(UIPanleID.UILoading, EnumOpenUIType.DefaultUIOrSecond);
        Control.Show(UIPanleID.UILoading);
        //}
        //else
        //{
        //    GameObject go = Resources.Load("Prefab/UIPanel/UILoading") as GameObject;
        //    GameObject load = NGUITools.AddChild(FindObjectOfType<UIRoot>().gameObject, go);
        //    load.SetActive(true);
        //}

        Globe.LoadScenceName = SceneName;
        Globe.LoadTime = LoadTime;
        Globe.callBack = callBack;
        Globe.completed = completed;
    }

    public static void LoadScene(int SceneID, float LoadTime, CallBack callBack = null, CallBack completed = null)
    {
        GameLibrary.dungeonId = SceneID;
        LoadScene(FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(SceneID).MapName, LoadTime, callBack, completed);
    }

    void Update()
    {
        if (isExit)
        {
            progressBar.currentValue += 1 / Globe.LoadTime * Time.deltaTime * progressBar.maxValue * 0.1f;
            if (progressBar.currentValue >= progressBar.maxValue)
            {
                progressBar.currentValue = progressBar.maxValue;
            }
        }
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    IEnumerator StartLoading()
    {
        int displayProgress = 0;
        int toProgress = 0;

        op = SceneManager.LoadSceneAsync(Globe.LoadScenceName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            toProgress = Mathf.CeilToInt(op.progress * progressBar.maxValue);
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                SetLoadingPercentage(displayProgress);
                yield return new WaitForFixedUpdate();
            }
            yield return null;
        }

        toProgress = (int)progressBar.maxValue;

        while (displayProgress < toProgress)
        {
            ++displayProgress;
            SetLoadingPercentage(displayProgress);
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// 设置加载条的百分百
    /// </summary>
    /// <param name="v"></param>
    private void SetLoadingPercentage(float v)
    {
        //进度条显示百分之90
        progressBar.currentValue = v * 0.9f;

        if (progressBar.currentValue >= (progressBar.maxValue * 0.9f))
        {
            Control.ClearGUI();
            if (Globe.callBack != null)
            {
                Globe.callBack();
            }
            isExit = true;
        }
    }

}
