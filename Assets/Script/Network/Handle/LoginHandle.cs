using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Tianyu;
public class CLoginHandle : CHandleBase
{
    public delegate void LoginBack(long playerId = 0, long heroId = 0, string name = "", int areaId = 0);
    public static LoginBack myLogin;
    public static LoginBack myCreate;
    public CLoginHandle(CHandleMgr mgr) : base(mgr)
    {
    }

    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.login_create_player_ret, CreateRoleResultHandle);
        RegistHandle(MessageID.login_player_login_ret, LoginResultHandle);
        RegistHandle(MessageID.login_check_account_ret, BackCheckAccount);
        RegistHandle(MessageID.c_player_enter_scene, EnterScene);
        RegistHandle(MessageID.s_player_change_scene, ChangeScence);
        RegistHandle(MessageID.c_player_leave_scene, LeaveScene);

    }
    /// <summary>
    /// 申请离开场景返回
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool LeaveScene(CReadPacket packet)
    {

        if (GameLibrary.Instance().isReconect)
        {
            GameLibrary.Instance().isReconect = false;
            return true;
        }
        Dictionary<string, object> data = packet.data;


        //float posy = float.Parse(data[])
        return true;
    }
    /// <summary>
    /// 申请跳转场景返回
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool ChangeScence(CReadPacket packet)
    {

        if (GameLibrary.Instance().isReconect)
        {
            GameLibrary.Instance().isReconect = false;
            return true;
        }
        Dictionary<string, object> data = packet.data;


        //float posy = float.Parse(data[])
        return true;
    }
    public bool EnterScene(CReadPacket packet)
    {
        //m_jv [ "si" ] = sceneId;
        //m_jv [ "px" ] = posX;
        //m_jv [ "py" ] = posY;
        //m_jv [ "pz" ] = posZ;
        Debug.Log("EnterScence");
        if (GameLibrary.Instance().isReconect)
        {
            GameLibrary.Instance().isReconect = false;
            return true;
        }
        Dictionary<string, object> data = packet.data;
        int scenceid = packet.GetInt("si");// int.Parse( data [ "si" ].ToString() );

        playerData.GetInstance().selfData.SetPos(new Vector3(packet.GetFloat("px"), packet.GetFloat("py"), packet.GetFloat("pz")));
        playerData.GetInstance().selfData.mapID = scenceid;
        if (FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.ContainsKey(scenceid))
        {
            string scencename = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList[scenceid].MapName;

            if (Globe.isLoadOutCity)
            {
                //if (SceneManager.GetActiveScene().name== "Loding")
                //{
                //    UI_Loading.LoadScene(scencename, 3);
                //}
                //else
                //{
                //    SceneManager.LoadScene("Loding");
                //}
                GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
                StartLandingShuJu.GetInstance().GetLoadingData(scencename, 3);
                SceneManager.LoadScene("Loding");
                Globe.isLoadOutCity = false;

                
            }
            else
            {
                if (Globe.isFightGuide)
                {
                    Globe.FightGuideSceneName = scencename;
                }
                else
                {
                    GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
                    StartLandingShuJu.GetInstance().GetLoadingData(scencename, 3);
                    SceneManager.LoadScene("Loding");
                }
                //if (!Globe.isEnterScence) {
                //	Globe.isEnterScence = true;
                //}
            }
        }
        else
        {
            if (Globe.isFightGuide)
            {
                Globe.FightGuideSceneName = GameLibrary.UI_Major;
            }
            else
            {
                GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
                StartLandingShuJu.GetInstance().GetLoadingData(GameLibrary.UI_Major, 3);
                SceneManager.LoadScene("Loding");
            }
        }

        //float posy = float.Parse(data[])
        return true;
    }
    public bool KeepAliveHandle(CReadPacket packet)
    {

        return true;
    }

    public bool CreateRoleResultHandle(CReadPacket packet)
    {
        Debug.Log("Login result");
        int resolt = packet.GetInt("ret");
        if (resolt == 0)
        {
            if (GameLibrary.Instance().isReconect)//如果断线重连状态不用接去数据
                return true;
            playerData.GetInstance().selfData.changeCount = 0;
            HandleLoginPacketData(packet);
            //UICreateRole.instance.DestoryObj();
            if (Globe.isFightGuide)
                SceneNewbieGuide.instance.CreateNameOver();
        }
        else
        {
            HandleLoginFailed(packet);
        }
        return true;
    }

    public bool BackCheckAccount(CReadPacket packet)
    {
        Debug.Log("Chech 成功");
        if (GameLibrary.Instance().isReconect)
        {
            ClientSendDataMgr.GetSingle().GetLoginSend().SendPlayerLogin(Globe.SelectedServer.playerId, Globe.SelectedServer.heroId, Globe.SelectedServer.areaId, 1);
            return true;
        }
        Dictionary<string, object> data = packet.data;
        long playerID = packet.GetLong("playerId"); //long.Parse( data [ "playerId" ].ToString() );
        long heroId = packet.GetLong("heroId");//long.Parse( data [ "heroId" ].ToString() );
        string name = packet.GetString("name");//data [ "name" ].ToString();
        int areaId = packet.GetInt("areaId");//int.Parse(data["areaId"].ToString());

        Globe.SelectedServer.playerId = uint.Parse(playerID.ToString());
        Globe.SelectedServer.heroId = int.Parse(heroId.ToString());
        Globe.SelectedServer.playerName = name;
        Globe.SelectedServer.areaId = int.Parse(areaId.ToString());
        UISelectServer.Instance.ResetIsStart(false);
        if (playerID > 0)
        {
            // if ( myLogin != null )
            // {
            //   myLogin( playerID , heroId, name, areaId );
            // }
            UISelectServer.Instance.isLoading = 1;
            Debug.Log("登陆");
        }
        else
        {
            UISelectServer.Instance.isLoading = 2;
            Debug.Log("注册");

            if (String.IsNullOrEmpty(playerData.GetInstance().selfData.playeName))
            {
                CHandleMgr.GetSingle().msgDishandled.Clear();
            }
            else
            {
                UICreateName.instance.SendMeg();
            }

            //	Debug.Log("注册")；
            // if ( myCreate != null )
            // {
            //     myCreate( playerID , heroId , name , areaId );
            // }
        }
        return true;
    }

    public bool LoginResultHandle(CReadPacket packet)
    {
        Debug.Log("<color=#FFc937>Login result登录数据</color>");
        Dictionary<string, object> data = packet.data;

        int resolt = packet.GetInt("ret");
        if (resolt == 0)
        {
            HandleLoginPacketData(packet);
        }
        else
        {
            HandleLoginFailed(packet);
        }
        return true;
    }

    void HandleLoginPacketData(CReadPacket packet)
    {
        playerData.GetInstance().selfData.playerId = packet.GetUint32("playerId");
        playerData.GetInstance().selfData.accountId = packet.GetUint32("account");
        playerData.GetInstance().selfData.playeName = packet.GetString("name");
        playerData.GetInstance().selfData.level = packet.GetInt("level");
        playerData.GetInstance().baginfo.strength = packet.GetInt("thew");//体力
        playerData.GetInstance().baginfo.gold = packet.GetUint32("gold");//金币
        playerData.GetInstance().baginfo.diamond = packet.GetUint32("diamond");//钻石
        playerData.GetInstance().selfData.heroId = packet.GetUint32("heroId");
        playerData.GetInstance().selfData.changeCount = packet.GetInt("changeName");//改名次数
        playerData.GetInstance().selfData.exprience = packet.GetInt("exps");
        playerData.GetInstance().selfData.maxExprience = packet.GetInt("maxExps");
        playerData.GetInstance().selfData.expPool = packet.GetLong("expsPool");//经验池经验值
        if (playerData.GetInstance().selfData.expPool < 0) playerData.GetInstance().selfData.expPool = 0;
        playerData.GetInstance().baginfo.areanCoin = packet.GetUint32("arenaCoin");//竞技场币
        playerData.GetInstance().baginfo.pveCoin = packet.GetUint32("pveCoin");//--龙鳞币
        playerData.GetInstance().baginfo.pvpCoin = packet.GetUint32("pvpCoin");//角斗场币
        playerData.GetInstance().baginfo.rewardCoin = packet.GetUint32("rewardCoin");//悬赏币
        playerData.GetInstance().baginfo.todayBuyStrengthCount = packet.GetInt("buyThew");//购买体力次数
        playerData.GetInstance().actionData.energyBuyTimes = playerData.GetInstance().baginfo.todayBuyStrengthCount;
        playerData.GetInstance().selfData.vip = packet.GetInt("vip");
        playerData.GetInstance().selfData.keyId = packet.GetUint32("ky");
        playerData.GetInstance().actionData.energyRecoverEndTime = packet.GetLong("maxThewTime");
        playerData.GetInstance().InitActionData();
        Auxiliary.SetServerTime(packet.GetDouble("sysTime"));//同步系统时间

        long photoId = playerData.GetInstance().iconData.icon_id = packet.GetInt("photo");//头像
        Dictionary<long, RoleIconAttrNode> pDict = FSDataNodeTable<RoleIconAttrNode>.GetSingleton().DataNodeList;
        if (pDict.ContainsKey(photoId))
            playerData.GetInstance().iconData.icon_name = pDict[photoId].icon_name;
        long photoFrameId = playerData.GetInstance().iconFrameData.iconFrame_id = packet.GetInt("photoFrame"); //头像框
        if (pDict.ContainsKey(photoFrameId))
            playerData.GetInstance().iconFrameData.iconFrame_name = pDict[photoFrameId].icon_name;

        SocietyManager.Single().mySocityID = packet.GetLong("unionId");//公会id
        SocietyManager.Single().societyName = packet.GetString("unionName"); //公会名称
        if (SocietyManager.Single().mySocityID != 0 && SocietyManager.Single().societyName != "")
        {
            SocietyManager.Single().isJoinSociety = true;
            Globe.isHaveSociety = true;
        }
        else
        {
            SocietyManager.Single().isJoinSociety = false;
            Globe.isHaveSociety = false;
        }
        if (packet.GetInt("unionPosition") == 1)
        {
            SocietyManager.Single().societyStatus = SocietyStatus.Member;
        }
        else if (packet.GetInt("unionPosition") == 5)
        {
            SocietyManager.Single().societyStatus = SocietyStatus.President;
        }
        else
        {
            SocietyManager.Single().societyStatus = SocietyStatus.Null;
        }

        MountAndPetNodeData.Instance().currentMountID = packet.GetLong("defMountsId");//当前使用的坐骑id
        MountAndPetNodeData.Instance().currentPetID = packet.GetLong("defPetId");//当前使用的宠物id
        MountAndPetNodeData.Instance().goMountID = packet.GetLong("mountsId");//使用坐骑状态，0为没有骑乘坐骑
        MountAndPetNodeData.Instance().godefPetID = packet.GetLong("petId");//使用的宠物状态

        if (packet.data.ContainsKey("fightHero"))
            LoadHeroList(packet.data["fightHero"], ref Globe.fightHero);
        LoadHeroList(packet.data["adFightHero"], ref Globe.adFightHero);
        LoadHeroList(packet.data["ed1FightHero"], ref Globe.ed1FightHero);
        LoadHeroList(packet.data["ed2FightHero"], ref Globe.ed2FightHero);
        LoadHeroList(packet.data["ed3FightHero"], ref Globe.ed3FightHero);
        LoadHeroList(packet.data["ed4FightHero"], ref Globe.ed4FightHero);
        LoadHeroList(packet.data["ed5FightHero"], ref Globe.ed5FightHero);
        if (packet.data.ContainsKey("arenaFightHero"))
            LoadHeroList(packet.data["arenaFightHero"], ref Globe.arenaFightHero);

        //if (Globe.adFightHero[0] == 0)
        //    LoadHeroList(packet.data["fightHero"], ref Globe.adFightHero);

        serverMgr.GetInstance().saveData();
        // ClientSendDataMgr.GetSingle().GetWalkSend().ping = true;
    }

    void HandleLoginFailed(CReadPacket packet)
    {
        Debug.Log(string.Format("登陆失败：{0}", packet.GetString("desc")));
        // string message = GameLibrary.LOGIN_TXT[packet.GetString("desc")];
        //UIPromptBox.Instance.ShowLabel(packet.GetString("desc"));
        Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, packet.GetString("desc"));
        //if (UICreateRole.instance!=null&&UICreateRole.instance.IsShow())
        //{
        //    UICreateRole.instance.EnterGame();
        //}
        if (UICreateName.instance != null && UICreateName.instance.IsShow())
        {
            UICreateName.instance.EnterGame();
        }
    }

    void EndCallBack()
    {
        //SetHeroList(Globe.fightHero, ref Globe.playHeroList);
        //SetHeroList(Globe.adFightHero, ref Globe.defendTeam);
        //SetHeroList(Globe.ed1FightHero, ref Globe.actGold);
        //SetHeroList(Globe.ed2FightHero, ref Globe.actExpe);
        //SetHeroList(Globe.ed3FightHero, ref Globe.actPower);
        //SetHeroList(Globe.ed4FightHero, ref Globe.actAgile);
        //SetHeroList(Globe.ed5FightHero, ref Globe.actIntel);
        if (null != Globe.playHeroList[0])
            GameLibrary.player = Globe.playHeroList[0].id;
    }

    void SetHeroList(int[] hero, ref HeroData[] hd)
    {
        for (int i = 0; i < hd.Length; i++)
        {
            if (hero[i] != 0)
            {
                HeroData heroData = playerData.GetInstance().GetHeroDataByID(hero[i]);
                hd[i] = heroData;
            }
        }
    }

    void LoadHeroList(object data, ref int[] saveHero, bool isArena = false)
    {
        if (null != data)
        {
            Dictionary<string, object> fightHero = data as Dictionary<string, object>;
            List<string> fightHeroKey = new List<string>(fightHero.Keys);
            if (isArena)
            {
                int index = 1;
                for (int i = 0; i < saveHero.Length; i++)
                {
                    if (fightHeroKey.Contains(index.ToString()) && (i == 0 || i > 3))
                    {
                        saveHero[i] = Convert.ToInt32(fightHero[fightHeroKey[index - 1]]);
                        index++;
                    }
                    else
                    {
                        saveHero[i] = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < fightHeroKey.Count; i++)
                {
                    saveHero[i] = Convert.ToInt32(fightHero[fightHeroKey[i]]);
                }
            }
        }
    }

}

