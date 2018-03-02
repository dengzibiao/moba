using UnityEngine;
using System.Collections;
using System;

public class UIPromptBox : GUIBase
{
    public GUISingleButton cancelBtn;
    public GUISingleButton ensureBtn;
    public GUISingleLabel  label;
    private  static UIPromptBox instance;
    private string str =null;
    public static UIPromptBox Instance
    {
        get { return instance; }
        set { instance = value; }
    }
    public UIPromptBox()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
       return UIPanleID.UIPromptBox;
    }

    protected override void Init()
    {
        base.Init();
        cancelBtn.onClick = OnCancelClick;
        ensureBtn.onClick = OnEnsureClick;       
    }
    /// <summary>
    /// 显示文本内容
    /// </summary>
    /// <param name="str"></param>
    public void  ShowLabel(string str)
    {
        this.str = str;
        Show();
    }
    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams!=null&& uiParams.Length>0)
        {
            this.str = (string)uiParams[0];
        }
        base.SetUI();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        this.State = EnumObjectState.Ready;
        if (str != null)
        {
            Show();
        }
        else
        {
            Hide();
            Control.HideGUI(this.GetUIKey());
        }
        
    }
    void RefreshData(string str)
    {
        label.text = str;
    }
    protected override void ShowHandler()
    {
        label.text = str;
    }
    private void OnEnsureClick()
    {
		if (UISelectServer.Instance != null)
			UISelectServer.Instance.isstart = false;
		ClientNetMgr.GetSingle ().m_bServerNet = true;

		//Application.LoadLevel(0);
        if (str == "网络已中断请重新连接")
        {
            Control.HideGUI(this.GetUIKey());
            //Hide();
//           // GameLibrary.Instance().isReconect = true;
//           // ReLogin();
//			if (UISelectServer.Instance != null)
//				UISelectServer.Instance.isstart = false;
//			ClientNetMgr.GetSingle ().m_bServerNet = true;
//			ClientSendDataMgr.GetSingle ().GetWalkSend ().ping = false;
//            ClientNetMgr.GetSingle().Close();
//            //Globe.isEnterScence = false;
//            //Destroy (StartLandingShuJu.GetInstance ().gameObject);
//            playerData.GetInstance ().NearRIarr.Clear ();
//			playerData.GetInstance ().selfData.oldMapID = 0;
//			Application.LoadLevel(0);
//#if QiHuRelease
//            SDKMgr.Instance.Login();
//#endif     
        }
        else if(str == "想去野外吗？来吧！")
        {
            Singleton<SceneManage>.Instance.Current = EnumSceneID.LGhuangyuan;
            ClientSendDataMgr.GetSingle().GetBattleSend().SendGetHerosBattleAttr(Globe.fightHero);
            ClientSendDataMgr.GetSingle().GetLoginSend().SendChengeScene(20000, 20100,2);
            //Hide();
            Control.HideGUI(this.GetUIKey());
        }
        else if(str == "服务器维护中！请重新登录")
        {
            Application.Quit();
        }
		else if(str =="与服务器链接丢失，将重新与服务器建立链接")
        {
			//GameLibrary.Instance().isReconect = true;
			//ReLogin();
			if (UISelectServer.Instance != null)
				UISelectServer.Instance.isstart = false;

			ClientNetMgr.GetSingle ().m_bServerNet = true;
            ClientNetMgr.GetSingle().Close();
			ClientSendDataMgr.GetSingle ().GetWalkSend ().ping = false;
			playerData.GetInstance ().NearRIarr.Clear ();
			playerData.GetInstance ().selfData.oldMapID = 0;
            playerData.GetInstance().worldMap.Clear();
			Application.LoadLevel(0);
#if QiHuRelease
            SDKMgr.Instance.Login();
#endif


        }
        else if(str == "正在与服务器做版本比较，请稍后")
        {
            
        }
        else if(str == "您的版本号与服务器不一致，请更新新包")
        {
            Application.Quit();
        }
        else
        {
            Control.HideGUI(this.GetUIKey());
            //Hide();
        }
    }
    /// <summary>
    /// 重新连接服务器
    /// </summary>
    private void ReLogin()
    {
        if (Globe.SelectedServer != null)
        {
            bool bConnect = ClientNetMgr.GetSingle().StartConnect(Globe.SelectedServer.ip, Globe.SelectedServer.port);

            if (bConnect)
            {

                Control.ShowGUI(UIPanleID.UIWaitForSever, EnumOpenUIType.DefaultUIOrSecond);
                ClientSendDataMgr.GetSingle().GetLoginSend().SendCheckAccount();
                //Hide();
                Control.HideGUI(this.GetUIKey());
            }
            else
            {
                //ShowLabel("重连失败，请检查网络重启游戏！");
                RefreshData("重连失败，请检查网络重启游戏！");
            }
        }
        else
        {
            //Hide();
            Control.HideGUI(this.GetUIKey());
        }
    }
    /// <summary>
    /// 取消
    /// </summary>
    private void OnCancelClick()
    {
        //Hide();
        Control.HideGUI(this.GetUIKey());
    }
}
