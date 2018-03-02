using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class UISelectServer : GUIBase
{
    private static UISelectServer instance;
    public GUISingleButton backBtn;
    public GUISingleButton uIGameAfficheBtn;
    public GUISingleButton serverBtn;
    public GUISingleButton startGameBtn;
    public GUISingleLabel serverName;
    public GUISingleLabel stateLabel;
    public GUISingleSprite stateSprite;
    private Transform bg;
    private int dataState;

    public byte isLoading = 0;//0 没有意义，1登陆2注册
    public bool isstart = false;
    public void ResetIsStart(bool valu)
    {
        isstart = valu;
    }
    public static UISelectServer Instance
    {
        get { return instance; }
        set { instance = value; }
    }
    public UISelectServer()
    {
        instance = this;
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }

    protected override void Init()
    {
        bg = transform.parent.Find("Panel/BG");
        serverBtn.onClick = OnSelectClick;
        startGameBtn.onClick = OnEnterClick;
        uIGameAfficheBtn.onClick = GameAfficheBtnClick;
        startGameBtn.state = GUISingleButton.State.Disabled;
        //  int index = serverMgr.GetInstance().serverkeymap[serverMgr.GetInstance().GetServer()];
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UI_SelectServer;
    }
    protected override void ShowHandler()
    {
        transform.parent.Find("Effect").gameObject.SetActive(true);
        bg.gameObject.SetActive(true);
        InitServerList();
    }
    public void InitServerList()
    {
    
        if (Globe.SelectedServer != null && serverName.text!=null)
        {

            serverName.text = Globe.SelectedServer.name;
            SetState(Globe.SelectedServer.state);
            startGameBtn.state = GUISingleButton.State.Normal;
        }
    }

    void SetState(int state)
    {
        switch (state)
        {
            case -1:
                stateSprite.spriteName = "";
                stateLabel.text = "服务器关闭";
                stateLabel.color = Color.black;
                break;
            case 0:
                stateSprite.spriteName = "huidian";
                stateLabel.text = "维修";
                stateLabel.color = Color.gray;
                break;
            case 2:
                stateSprite.spriteName = "lvdian";
                stateLabel.text = "新服";
                stateLabel.color = Color.green;
                break;
            case 1:
                stateSprite.spriteName = "hongdian";
                stateLabel.text = "爆满";
                stateLabel.color = Color.red;
                break;
        }
    }

    void GameAfficheBtnClick()
    {
        Control.ShowGUI(UIPanleID.UIGameAffiche, EnumOpenUIType.DefaultUIOrSecond);
    }
    void OnSelectClick()
    {
        Control.ShowGUI(UIPanleID.UI_ServerList, EnumOpenUIType.DefaultUIOrSecond);
        Hide();
        Control.HideGUI(this.GetUIKey());
        isstart = false;
    }

    void OnEnterClick()
    {
        if (!isstart)
        {
            if (!ClientNetMgr.GetSingle().IsConnect())
            {
                ClientNetMgr.GetSingle().StartConnect(Globe.SelectedServer.ip, Globe.SelectedServer.port);
                ClientNetMgr.GetSingle().bReConnect = true;
            }

            ClientSendDataMgr.GetSingle().GetLoginSend().SendCheckAccount();
            //   CLoginHandle.myLogin = SendLogin;
            // CLoginHandle.myCreate = SendCreate;
            isstart = true;
        }

        //if ( DataDefine.isConectSocket )
        //{
        //    if ( Globe.SelectedServer != null )
        //    {
        //        SelectCard.selectAreaId = Globe.SelectedServer.areaId.ToString();
        //    }

        //    if ( Globe.SelectedServer != null && Globe.SelectedServer.playerId != 0 )
        //    {
        //        string name = serverMgr.GetInstance().GetPlayNameDataByName( Globe.SelectedServer.name );
        //        if ( name != null )
        //        {
        //            GameLibrary.nickName = name;
        //            serverMgr.GetInstance().SetName( name );

        //        }
        //        long hId = serverMgr.GetInstance().GetServerDataByName( Globe.SelectedServer.name ).heroId;
        //        if ( hId != 0 )
        //        {
        //            Globe.playHeroList [ 0 ] = playerData.GetInstance().FindOrNewHeroDataById( hId );
        //            GameLibrary.player = Globe.playHeroList [ 0 ].id;
        //            if ( !ClientNetMgr.GetSingle().IsConnect() )
        //            {
        //                ClientNetMgr.GetSingle().StartConnect( Globe.SelectedServer.ip , Globe.SelectedServer.port );
        //            }
        //            //ClientSendDataMgr.GetSingle().GetLoginSend().SendPlayerLogin(Globe.SelectedServer.playerId, Globe.SelectedServer.heroId, Globe.SelectedServer.areaId, 1);
        //        }
        //        else
        //        {
        //            //没有英雄 ---应该到选择英雄面板

        //            Control.ShowGUI( GameLibrary.UI_ServerList );
        //            Hide();
        //            // SelectCard.selectAreaId = dic.Key.ToString();
        //        }

        //        // transform.root.Find( "UICreateRole" ).gameObject.SetActive( true );

        //        //GameLibrary.nickName = serverMgr.GetInstance().GetName();          

        //    }
        //    else
        //    {
        //        Control.ShowGUI( GameLibrary.UI_CreateRole );
        //        Hide();
        //    }
        //    serverMgr.GetInstance().SetServer( serverName.text );
        //    serverMgr.GetInstance().saveData();
        //}
        //else
        //{
        //    //TODO：不联网读本地配置
        //    //serverMgr.GetInstance().SetServer(serverName.text);
        //    //serverMgr.GetInstance().saveData();
        //    //UI_Loading.LoadScene(GameLibrary.UI_Major, 3);
        //}


    }

    public void SendLogin(long PlayerId, long HeroId, string Name, int AreaId)
    {
        if (DataDefine.isConectSocket)
        {
            if (Globe.SelectedServer != null)
            {
                SelectCard.selectAreaId = Globe.SelectedServer.areaId.ToString();
            }

            if (Globe.SelectedServer != null)
            {
                //string name = serverMgr.GetInstance().GetPlayNameDataByName( Globe.SelectedServer.name );
                string name = Name;
                if (name != null)
                {
                    GameLibrary.nickName = name;
                    serverMgr.GetInstance().SetName(name);

                }
                //long hId = serverMgr.GetInstance().GetServerDataByName( Globe.SelectedServer.name ).heroId;
                long hId = HeroId;
                if (hId != 0)
                {
                    //if (!ClientNetMgr.GetSingle().IsConnect())
                    //Globe.playHeroList[0] = playerData.GetInstance().FindOrNewHeroDataById(hId);
                    //GameLibrary.player = Globe.playHeroList[0].id;
                    if (!ClientNetMgr.GetSingle().IsConnect())
                    {
                        ClientNetMgr.GetSingle().StartConnect(Globe.SelectedServer.ip, Globe.SelectedServer.port);
                    }
                    ClientSendDataMgr.GetSingle().GetLoginSend().SendPlayerLogin(Globe.SelectedServer.playerId, Globe.SelectedServer.heroId, Globe.SelectedServer.areaId, 1);
                }
            }
            transform.parent.Find("Effect").gameObject.SetActive(false);
            serverMgr.GetInstance().SetServer(serverName.text); serverMgr.GetInstance().saveData();
        }
    }

    public void SendCreate(long PlayerId, long HeroId, string Name, int AreaId)
    {
        if (DataDefine.isConectSocket)
        {
            Control.ShowGUI(UIPanleID.UI_CreateRole, EnumOpenUIType.DefaultUIOrSecond);
            Control.HideGUI(this.GetUIKey());
            transform.parent.Find("Effect").gameObject.SetActive(false);
            serverMgr.GetInstance().SetServer(serverName.text);
            serverMgr.GetInstance().saveData();
        }
    }

    void LoadMajor()
    {

    }
}
