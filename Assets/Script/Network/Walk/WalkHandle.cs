using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using UnityEngine.SceneManagement;

public enum UpdatePlayerInfoType
{
    None = 0x0,
    Position = 0x1,//位置
    Orientation = 0x2,//
    Gold = 0x4,
    Diamond = 0x8,
    Exp = 0x10,
    RedPoint = 0x20,
    Hp = 0x40,
    Title = 0x80,
    Vitality = 0x100,
    PvpCoin = 0x200,
    ArenaCoin = 0x400,
    PveCoin = 0x800,
    RewardCoin = 0x1000,
    UnionId = 0x2000,
    UnionName = 0x4000,
    Pet = 0x8000,
    Mount = 0x10000,
    heroId = 0x20000,

};

public class WalkHandle : CHandleBase//WalkHandleBase
{
    //public List<RoleInfo> OtherPlayerInfoList = new List<RoleInfo>();
    //GameObject OtherPlayerParent = null;

    public WalkHandle(CHandleMgr mgr)//WalkHandleMgr mgr )
        : base(mgr)
    {

    }

    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.delete_walk_object, DeleteOtherPlayerHandle);
        RegistHandle(MessageID.add_walk_object, AddOtherPlayerHandle);

        RegistHandle(MessageID.c_update_player_info, OtherPlayerWalkHandle);
        //  RegistHandle( MessageID.update_map_element_info , ScenceElementWalkHandle );
        RegistHandle(MessageID.player_ping_ret, PingBackHandle);
        RegistHandle(MessageID.c_player_attack_ret, AttackHandle);
        RegistHandle(MessageID.c_lose_blood_ret, HpHandle);
        RegistHandle(MessageID.c_someone_dead_ret, DeadHandle);
        RegistHandle(MessageID.c_monster_attack_ret, MonsterAttackHandle);
        RegistHandle(MessageID.c_update_map_element_info, ScenceMapWalkHandle);
        //调试行走不测试。对应不相符。

        RegistHandle(MessageID.player_ping_ret, PingBackHandle);
        RegistHandle(MessageID.c_player_revive_ret, ReVive);
        RegistHandle(MessageID.s_loadfinish, ServerLoadFinish);
        RegistHandle(MessageID.S_player_load_scene_finiReq, LoadFinishReq);

    }
    public bool LoadFinishReq(CReadPacket packet)
    {
        if (!GameLibrary.clientInitRet)
        {
            CallLoad();
            GameLibrary.clientInitRet = true;
            ClientSendDataMgr.GetSingle().GetWalkSend().ping = true;
        }
        return true;
    }


    public bool ServerLoadFinish(CReadPacket packet)
    {

        // CallLoad();
        GameLibrary.serverInit = true;
        //playerData.GetInstance().guideData.uId = 0;
        return true;
    }
    //加载数据
    void CallLoad()
    {
        ClientSendDataMgr.GetSingle().GetHeroSend().SendGetHero(C2SMessageType.Active);//获取英雄列表延迟发送
        ClientSendDataMgr.GetSingle().GetItemSend().SendGetBackPackList(C2SMessageType.Active);
        //ClientSendDataMgr.GetSingle().GetMailSend().SendGetAllMailList(C2SMessageType.Active);
        //获取一下玩家拥有的坐骑和宠物
        ClientSendDataMgr.GetSingle().GetPetSend().SendGetHaveMountOrPetList(C2SMessageType.Active, 2);
        ClientSendDataMgr.GetSingle().GetPetSend().SendGetHaveMountOrPetList(C2SMessageType.Active, 1);

        ClientSendDataMgr.GetSingle().GetGoldHandSend().GetGoldHandTimes(C2SMessageType.Active);

        ClientSendDataMgr.GetSingle().GetMailSend().SendGetNewMailCount(C2SMessageType.Active);//取一下新邮件个数 避免新邮件刷新事件没注册
        //  ClientSendDataMgr.GetSingle().GetHeroSend().SendGetRunesList(C2SMessageType.Active);
        //playerData.GetInstance().InitActionData();
        //ClientSendDataMgr.GetSingle().GetBattleSend().SendQueryWorldMap();//获取世界副本


    }
    //玩家复活消息
    public bool ReVive(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        int hp = packet.GetInt("hp");
        UInt32 sorkey = packet.GetUint32("sk");
        UInt32 tarkey = packet.GetUint32("tk");
        int reviewtype = packet.GetInt("rt");
        if (tarkey == playerData.GetInstance().selfData.keyId)//目标是自己
        {
            //if ( CreatePeople.GetInstance().GetTargert( sorkey ) == null )//发起者是自己
            {
                CharacterState cs = null;
                if (CharacterManager.player != null)
                {
                    cs = CharacterManager.player.GetComponent<CharacterState>();
                    if (cs != null)
                    {
                        cs.SetBorn(hp);
                    }
                }
            }


        }
        else
        {
            if (CreatePeople.GetInstance().OtherplayerDic.ContainsKey(tarkey))
            {
                GameObject obj = CreatePeople.GetInstance().OtherplayerDic[tarkey];
                if (obj != null)
                {
                    CharacterState cs = obj.GetComponent<CharacterState>();
                    if (cs != null)
                    {
                        cs.SetBorn(hp);
                    }
                }
                //CreatePeople.GetInstance().OtherplayerDic.Remove(tarkey);
                //GameObject.Destroy(obj);
                //if (playerData.GetInstance().NearRIarr.ContainsKey(tarkey))
                //{
                //    playerData.GetInstance().NearRIarr.Remove(tarkey);
                //}
            }

            //if( playerData.GetInstance().NearRIarr.ContainsKey(tarkey))
            // {
            //     RoleInfo ri = playerData.GetInstance().NearRIarr[tarkey];
            //     ri.hp = (short)hp;
            //     //if(ri.RoleObj!=null)
            //     //{
            //     //    GameObject.Destroy(ri.RoleObj);
            //     //}
            // }
            //CharacterState cs = null;
            //GameObject obj = CreatePeople.GetInstance().GetTargert(tarkey);
            //if (obj != null)
            //{
            //    cs = obj.GetComponent<CharacterState>();
            //    if (cs != null)
            //    {
            //        cs.SetBorn(hp);
            //    }
            //}
        }
        return false;
    }

    //删除其他玩家
    public bool DeleteOtherPlayerHandle(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        Int32[] str = data["dl"] as Int32[];
        if (str != null)
        {
            for (int i = 0; i < str.Length; i++)
            {
                CreatePeople.GetInstance().DeleteOtherObject(UInt32.Parse(str[i].ToString()));
            }
            return true;
        }
        else
        {
            Int64[] strarr = data["dl"] as Int64[];
            if (strarr != null)
            {
                for (int i = 0; i < strarr.Length; i++)
                {
                    CreatePeople.GetInstance().DeleteOtherObject(UInt32.Parse(strarr[i].ToString()));
                }
                return true;
            }
            else
            {
                object[] strobjarr = data["dl"] as object[];
                if (strobjarr != null)
                {
                    for (int i = 0; i < strobjarr.Length; i++)
                    {
                        CreatePeople.GetInstance().DeleteOtherObject(UInt32.Parse(strobjarr[i].ToString()));
                    }
                    return true;
                }
                else
                {
                    return false;
                }

            }

        }
    }
    public class ScenceElementInfo
    {
        // 参数是mapID地图 key唯一标示 accid账户ID playid玩家ID roleid角色ID posX坐标 posZ坐标　name名字

        public int keyID;
        public int playID;
        public float posX;
        public float posY;
        public float posZ;
        public string name;
        public GameObject RoleObj;
    }
    void DoConcludeRi(RoleInfo ri)
    {
        if (playerData.GetInstance().NearRIarr[ri.keyID].RoleObj != null)
        {
            // GameObject.Destroy(playerData.GetInstance().NearRIarr[ri.keyID].RoleObj);
            CreatePeople.GetInstance().DeleteOtherObject(ri.keyID);

        }
        else
        {
            //RoleInfo oldri = playerData.GetInstance().NearRIarr[ri.keyID];
            //oldri.name = ri.name;
            //oldri.
            playerData.GetInstance().NearRIarr.Remove(ri.keyID);
        }
    }
    //创建场景中的物体
    public bool AddOtherPlayerHandle(CReadPacket packet)
    {
        if (playerData.GetInstance().NearRIarr.Count > 20)
        {
            Debug.Log(packet.ToString());
            return false;
        }

        Dictionary<string, object> data = packet.data;
        object[] alDatas = data["al"] as object[];//addlist

        if (alDatas != null)
        {
            // Debug.Log("al===>" + alDatas.Length);
            for (int i = 0; i < alDatas.Length; i++)
            {
                Dictionary<string, object> playerInfo = (Dictionary<string, object>)alDatas[i];
                if (CreatePeople.GetInstance() == null)
                    return false;
                long key = long.Parse(playerInfo["ky"].ToString());
                RoleInfo ri = CreateRoleInfo(playerInfo);
                if (playerData.GetInstance().NearRIarr.ContainsKey(ri.keyID))
                {
                    //如果已经包含了，是否把新的数据更新过来

                    DoConcludeRi(ri);

                }

                playerData.GetInstance().NearRIarr.Add(ri.keyID, ri);

                if (SceneManager.GetActiveScene().name != GameLibrary.UI_Major
            && SceneManager.GetActiveScene().name != GameLibrary.LGhuangyuan
            && SceneManager.GetActiveScene().name != GameLibrary.PVP_1V1
           )
                {
                    if (GameLibrary.Instance().isLoadOtherPepole)
                        GameLibrary.Instance().isLoadOtherPepole = false;
                    // return false;
                }
                else
                {
                    if (ri.accID == 0)//
                    {
                        CreatePeople.GetInstance().CreatOtherObject(ref ri);
                    }
                    else//其他玩家
                    {

                        if (!CreatePeople.GetInstance().OtherplayerDic.ContainsKey(key))
                        {
                            Debug.Log("创建其他玩家");
                            CreatePeople.GetInstance().CreatOtherObject(ref ri);

                        }
                        else
                        {
                            Debug.Log("玩家已在列表里");
                        }

                    }


                    if (playerData.GetInstance().NearRIarr.ContainsKey(key))
                    {
                        if (playerInfo.ContainsKey("pt"))
                        {
                            playerData.GetInstance().NearRIarr[key].petid = Convert.ToInt64(playerInfo["pt"]);
                            CreatePeople.GetInstance().UpdateOtherPlayerInfo(key, UpdatePlayerInfoType.Pet);
                        }
                        if (playerInfo.ContainsKey("mt"))
                        {
                            playerData.GetInstance().NearRIarr[key].mount = Convert.ToInt64(playerInfo["mt"]);
                            CreatePeople.GetInstance().UpdateOtherPlayerInfo(key, UpdatePlayerInfoType.Mount);
                        }
                    }
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }


    enum UpdateMapElementInfoType
    {
        None = 0x0,
        Position = 0x1,
        Hp = 0x2,
    };

    RoleInfo CreateRoleInfo(Dictionary<string, object> playerInfo)
    {
        RoleInfo tempRI = new RoleInfo();
        tempRI.mapID = UInt32.Parse(playerInfo["si"].ToString());
        MapInfoNode tempMN = null;
        tempRI.keyID = UInt32.Parse(playerInfo["ky"].ToString());
        tempRI.posX = float.Parse(playerInfo["px"].ToString());
        tempRI.posY = float.Parse(playerInfo["py"].ToString());
        tempRI.posZ = float.Parse(playerInfo["pz"].ToString());
        tempRI.accID = UInt32.Parse(playerInfo["ai"].ToString());
        tempRI.playID = UInt32.Parse(playerInfo["pi"].ToString());
        tempRI.roleID = UInt32.Parse(playerInfo["ri"].ToString());
        tempRI.typeid = UInt32.Parse(playerInfo["td"].ToString());
        tempRI.type = UInt32.Parse(playerInfo["tp"].ToString());

        tempRI.rc = int.Parse(playerInfo["rc"].ToString());
        tempRI.name = playerInfo["nm"].ToString();
        if (playerInfo.ContainsKey("hp"))
            tempRI.hp = short.Parse(playerInfo["hp"].ToString());
        if (playerInfo.ContainsKey("unId"))
            tempRI.unionId = UInt32.Parse(playerInfo["unId"].ToString());
        // playerData.GetInstance().NearRIarr[ky].unionName = packet.GetString("unNm");
        // playerData.GetInstance().NearRIarr[ky].RoleObj.

        if (playerInfo.ContainsKey("unNm"))
            tempRI.unionName = playerInfo["unNm"].ToString();
        if (playerInfo.ContainsKey("pt"))
            tempRI.unionId = UInt32.Parse(playerInfo["pt"].ToString());
        if (playerInfo.ContainsKey("mt"))
            tempRI.unionId = UInt32.Parse(playerInfo["mt"].ToString());
        if (tempRI.mapID != 0 && FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.ContainsKey(tempRI.mapID))
        {
            tempMN = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList[tempRI.mapID];

            tempRI.posX = float.Parse(playerInfo["px"].ToString()) + tempMN.Xmin;
            tempRI.posZ = float.Parse(playerInfo["pz"].ToString()) + tempMN.Zmin;

        }
        else
        {
        }
        return tempRI;
    }

    //其他玩家行走消息
    public bool ScenceMapWalkHandle(CReadPacket packet)
    {

        Dictionary<string, object> data = packet.data;
        long fg = long.Parse(data["fg"].ToString());
        long ky = long.Parse(data["ky"].ToString());
        int si = int.Parse(data["si"].ToString());
        RoleInfo roleInfo = null;
        if (playerData.GetInstance().NearRIarr.ContainsKey(ky))//其他玩家
        {
            roleInfo = playerData.GetInstance().NearRIarr[ky];
        }


        if ((fg & (long)(UpdatePlayerInfoType.Position)) > 0)
        {
            if (roleInfo != null)
            {
                roleInfo.posX = float.Parse(data["px"].ToString());
                roleInfo.posY = float.Parse(data["py"].ToString());
                roleInfo.posZ = float.Parse(data["pz"].ToString());
                if (roleInfo.accID != 0)
                {
                    Debug.Log("其他玩家同步消息" + roleInfo);
                }


                if (FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.ContainsKey(si))
                {
                    MapInfoNode tempMN = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList[si];
                    if (tempMN != null)
                    {
                        roleInfo.posX = float.Parse(data["px"].ToString()) + tempMN.Xmin;
                        roleInfo.posZ = float.Parse(data["pz"].ToString()) + tempMN.Zmin;
                    }
                }
                if (ky != playerData.GetInstance().selfData.keyId)
                {
                    CreatePeople.GetInstance().MoveOtherObject(ky,
                        roleInfo.posX, roleInfo.posY, roleInfo.posZ,
                        // packet.GetInt("rc"));
                        int.Parse(data["tp"].ToString()));
                }
            }





        }
        //朝向
        if ((fg & (long)(UpdatePlayerInfoType.Orientation)) > 0)
        {
            if (data.ContainsKey("hp"))
            {

                //    //  MapInfoNode tempMN = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList[int.Parse(data["si"].ToString())];
                CreatePeople.GetInstance().ChangeOtherHp(ky, int.Parse(data["hp"].ToString()));
            }
            else
            {
                if (roleInfo != null)
                {
                    roleInfo.orientX = float.Parse(data["ox"].ToString());
                    roleInfo.orientY = float.Parse(data["oy"].ToString());
                    roleInfo.orientZ = float.Parse(data["oz"].ToString());
                    if (ky != playerData.GetInstance().selfData.keyId)
                    {


                        CreatePeople.GetInstance().SetOtherPlayerOrientation(ky,
                        roleInfo.posX, roleInfo.posY, roleInfo.posZ
                           );
                        int.Parse(data["tp"].ToString());
                    }

                }

            }
        }


        //血量更新
        if ((fg & (long)(UpdatePlayerInfoType.Hp)) > 0)
        {
            if (playerData.GetInstance().NearRIarr.ContainsKey(ky))
            {
                MapInfoNode tempMN = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList[int.Parse(data["si"].ToString())];
                CreatePeople.GetInstance().ChangeOtherHp(ky, packet.GetInt("hp"));
            }
            else
            {
                //自己的血量同步
                if (ky == playerData.GetInstance().selfData.keyId)
                {
                    int count = packet.GetInt("hp");
                    if (CharacterManager.playerCS != null)
                        CharacterManager.playerCS.ChangeHp(count);
                    if (playerData.GetInstance().selfData.hp < count)
                        playerData.GetInstance().selfData.hp = count;
                }
            }

        }

        return true;
    }
    //if (SceneManager.GetActiveScene().name != GameLibrary.UI_Major
    //    && SceneManager.GetActiveScene().name != GameLibrary.LGhuangyuan
    //    && SceneManager.GetActiveScene().name != GameLibrary.PVP_1V1)
    //    return false;






    //其他玩家行走消息
    public bool OtherPlayerWalkHandle(CReadPacket packet)
    {
        //if (SceneManager.GetActiveScene().name != GameLibrary.UI_Major
        //    && SceneManager.GetActiveScene().name != GameLibrary.LGhuangyuan
        //    && SceneManager.GetActiveScene().name != GameLibrary.PVP_1V1)
        //    return false;
        Dictionary<string, object> data = packet.data;
        long fg = long.Parse(data["fg"].ToString());
        long ky = long.Parse(data["ky"].ToString());
        int si = int.Parse(data["si"].ToString());
        RoleInfo roleInfo = null;
        if (playerData.GetInstance().NearRIarr.ContainsKey(ky))//其他玩家
        {
            roleInfo = playerData.GetInstance().NearRIarr[ky];
        }


        if ((fg & (long)(UpdatePlayerInfoType.Position)) > 0)
        {
            if (roleInfo != null)
            {
                roleInfo.posX = float.Parse(data["px"].ToString());
                roleInfo.posY = float.Parse(data["py"].ToString());
                roleInfo.posZ = float.Parse(data["pz"].ToString());
                if (roleInfo.accID != 0)
                {
                    Debug.Log("其他玩家同步消息" + roleInfo);
                }


                if (FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.ContainsKey(si))
                {
                    MapInfoNode tempMN = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList[si];
                    if (tempMN != null)
                    {
                        roleInfo.posX = float.Parse(data["px"].ToString()) + tempMN.Xmin;
                        roleInfo.posZ = float.Parse(data["pz"].ToString()) + tempMN.Zmin;
                    }
                }
                if (ky != playerData.GetInstance().selfData.keyId)
                {
                    CreatePeople.GetInstance().MoveOtherObject(ky,
                        roleInfo.posX, roleInfo.posY, roleInfo.posZ,
                        // packet.GetInt("rc"));
                        int.Parse(data["tp"].ToString()));
                }
            }





        }
        //朝向
        if ((fg & (long)(UpdatePlayerInfoType.Orientation)) > 0)
        {
            if (data.ContainsKey("hp"))
            {

                //    //  MapInfoNode tempMN = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList[int.Parse(data["si"].ToString())];
                CreatePeople.GetInstance().ChangeOtherHp(ky, int.Parse(data["hp"].ToString()));
            }
            else
            {
                if (roleInfo != null)
                {
                    roleInfo.orientX = float.Parse(data["ox"].ToString());
                    roleInfo.orientY = float.Parse(data["oy"].ToString());
                    roleInfo.orientZ = float.Parse(data["oz"].ToString());
                    if (ky != playerData.GetInstance().selfData.keyId)
                    {


                        CreatePeople.GetInstance().SetOtherPlayerOrientation(ky,
                        roleInfo.posX, roleInfo.posY, roleInfo.posZ
                           );
                        int.Parse(data["tp"].ToString());
                    }

                }

            }
        }
        if ((fg & (long)(UpdatePlayerInfoType.heroId)) > 0)
        {
            if (roleInfo != null)//其他玩家
            {
                if (roleInfo.roleID != packet.GetUint32("ri"))
                {
                    roleInfo.roleID = packet.GetUint32("ri");
                    GameObject.Destroy(roleInfo.RoleObj);
                    // if(roleInfo)
                    CreatePeople.GetInstance().CreateOtherPlayer(ref roleInfo, false);
                }
            }
            else//玩家自己
            {

            }
        }
        if ((fg & (long)(UpdatePlayerInfoType.UnionId)) > 0)//公会id
        {
            if (playerData.GetInstance().NearRIarr.ContainsKey(ky))
            {
                playerData.GetInstance().NearRIarr[ky].unionId = packet.GetUint32("unId");
            }

            // playerData.GetInstance().NearRIarr[ky].unionName = packet.GetString("unNm");
            // playerData.GetInstance().NearRIarr[ky].RoleObj.
        }
        if ((fg & (long)(UpdatePlayerInfoType.UnionName)) > 0)//公会id
        {
            if (playerData.GetInstance().NearRIarr.ContainsKey(ky))
            {
                playerData.GetInstance().NearRIarr[ky].unionName = packet.GetString("unNm");
            }
            if (playerData.GetInstance().NearRIarr.ContainsKey(ky))
            {
                playerData.GetInstance().NearRIarr[ky].unionName = packet.GetString("unNm");

                if (playerData.GetInstance().NearRIarr.ContainsKey(ky))
                {
                    RoleInfo ri = playerData.GetInstance().NearRIarr[ky];
                    GameObject tempObj = ri.RoleObj;
                    if (ri.accID != 0)//刷新其他玩家的公会名称
                    {
                        OtherPlayer otherPlayer = tempObj.GetComponent<OtherPlayer>();
                        if (otherPlayer != null)
                        {
                            if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
                            {
                                otherPlayer.RefreshSocietyName(packet.GetString("unNm"));
                            }
                        }
                    }
                    ri = null;

                }

            }
        }
        if ((fg & (long)(UpdatePlayerInfoType.Pet)) > 0)//宠物状态更新
        {
            if (playerData.GetInstance().NearRIarr.ContainsKey(ky))
            {
                playerData.GetInstance().NearRIarr[ky].petid = packet.GetLong("pt");
                CreatePeople.GetInstance().UpdateOtherPlayerInfo(ky, UpdatePlayerInfoType.Pet);
            }
        }
        if ((fg & (long)(UpdatePlayerInfoType.Mount)) > 0)//宠物状态更新
        {
            if (playerData.GetInstance().NearRIarr.ContainsKey(ky))
            {
                playerData.GetInstance().NearRIarr[ky].mount = packet.GetLong("mt");
                CreatePeople.GetInstance().UpdateOtherPlayerInfo(ky, UpdatePlayerInfoType.Mount);
            }
        }

        //血量更新
        if ((fg & (long)(UpdatePlayerInfoType.Hp)) > 0)
        {
            if (playerData.GetInstance().NearRIarr.ContainsKey(ky))
            {
                MapInfoNode tempMN = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList[int.Parse(data["si"].ToString())];
                CreatePeople.GetInstance().ChangeOtherHp(ky, packet.GetInt("hp"));
            }
            else
            {
                //自己的血量同步
                if (ky == playerData.GetInstance().selfData.keyId)
                {
                    int count = packet.GetInt("hp");
                    if (CharacterManager.playerCS != null)
                        CharacterManager.playerCS.ChangeHp(count);
                    if (playerData.GetInstance().selfData.hp < count)
                        playerData.GetInstance().selfData.hp = count;
                }
            }


        }

        if ((fg & (long)(UpdatePlayerInfoType.Title)) > 0)//称号更新
        {
            if (data.ContainsKey("tl"))//称号更新
            {
                //if(playerData.GetInstance().NearRIarr.ContainsKey(ky))
                //{
                //    playerData.GetInstance().NearRIarr[ky].title = int.Parse(data["tl"].ToString());

                //    if (playerData.GetInstance().NearRIarr.ContainsKey(ky))
                //    {
                //        RoleInfo ri = playerData.GetInstance().NearRIarr[ky];
                //        GameObject tempObj = ri.RoleObj;
                //        if (ri.accID != 0)//刷新其他玩家的称号
                //        {
                //            OtherPlayer otherPlayer = tempObj.GetComponent<OtherPlayer>();
                //            if (otherPlayer!=null)
                //            {
                //                if (FSDataNodeTable<TitleNode>.GetSingleton().DataNodeList.ContainsKey(int.Parse(data["tl"].ToString())))
                //                {
                //                    string playerTitleName = FSDataNodeTable<TitleNode>.GetSingleton().DataNodeList[int.Parse(data["tl"].ToString())].titlename;
                //                    if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
                //                    {
                //                        otherPlayer.RefreshTitleName(playerTitleName); 
                //                    }
                //                }
                //            }
                //        }
                //        ri = null;

                //    }

                //}
            }
        }
        if ((fg & (long)(UpdatePlayerInfoType.Vitality)) > 0)//体力更新
        {
            if (ky == playerData.GetInstance().selfData.keyId)
            {
                if (data.ContainsKey("vt"))//体力更新
                {
                    playerData.GetInstance().baginfo.strength = int.Parse(data["vt"].ToString());
                    if (data.ContainsKey("mtwt"))
                    {
                        playerData.GetInstance().actionData.energyRecoverEndTime = long.Parse(data["mtwt"].ToString());
                    }
                    playerData.GetInstance().InitActionData();
                    playerData.GetInstance().ActionPointHandler(ActionPointType.Energy, playerData.GetInstance().baginfo.strength);
                }
            }
        }
        if ((fg & (long)(UpdatePlayerInfoType.RedPoint)) > 0)//红点更新
        {
            if (data.ContainsKey("rd"))//红点更新
            {
                if (data.ContainsKey("rd"))
                {
                    Dictionary<string, object> redData = data["rd"] as Dictionary<string, object>;
                    if (redData != null)
                    {
                        foreach (KeyValuePair<string, object> keyValuePair in redData)
                        {
                            int[] arr = keyValuePair.Value as int[];
                            List<int> temList = null;
                            if (arr != null && arr.Length > 0)
                            {
                                temList = new List<int>();
                                temList.AddRange(arr);
                                //for (int i = 0; i < arr.Length; i++)
                                //{
                                //    temList.Add(arr[i]);
                                //}

                            }
                            Singleton<RedPointManager>.Instance.Add((EnumRedPoint)(int.Parse(keyValuePair.Key)), temList);
                        }
                        Singleton<RedPointManager>.Instance.NotifyChange();
                    }

                }
            }
        }
        if ((fg & (long)(UpdatePlayerInfoType.Gold)) > 0)//金币更新
        {
            if (ky == playerData.GetInstance().selfData.keyId)
            {
                if (data.ContainsKey("gd"))//金币更新
                {
                    long a = long.Parse(data["gd"].ToString());
                    playerData.GetInstance().RoleMoneyHadler(MoneyType.Gold, a);
                }
            }
        }
        if ((fg & (long)(UpdatePlayerInfoType.Diamond)) > 0)//钻石更新
        {
            if (ky == playerData.GetInstance().selfData.keyId)
            {
                if (data.ContainsKey("dd"))//钻石更新
                {
                    long a = long.Parse(data["dd"].ToString());
                    playerData.GetInstance().RoleMoneyHadler(MoneyType.Diamond, a);
                }
            }
        }
        if ((fg & (long)(UpdatePlayerInfoType.PvpCoin)) > 0)//角斗场币更新
        {
            if (ky == playerData.GetInstance().selfData.keyId)
            {
                if (data.ContainsKey("ppc"))//角斗场更新
                {
                    long a = long.Parse(data["ppc"].ToString());
                    playerData.GetInstance().RoleMoneyHadler(MoneyType.PVPcoin, a);
                }
            }
        }
        if ((fg & (long)(UpdatePlayerInfoType.ArenaCoin)) > 0)//竞技场币更新
        {
            if (ky == playerData.GetInstance().selfData.keyId)
            {
                if (data.ContainsKey("ac"))//竞技场币更新
                {
                    long a = long.Parse(data["ac"].ToString());
                    playerData.GetInstance().RoleMoneyHadler(MoneyType.AreanCoin, a);
                }
            }
        }
        if ((fg & (long)(UpdatePlayerInfoType.PveCoin)) > 0)//远征币更新
        {
            if (ky == playerData.GetInstance().selfData.keyId)
            {
                if (data.ContainsKey("pec"))//远征币更新
                {
                    long a = long.Parse(data["pec"].ToString());
                    playerData.GetInstance().RoleMoneyHadler(MoneyType.PVEcion, a);
                }
            }
        }
        if ((fg & (long)(UpdatePlayerInfoType.Exp)) > 0)//人物经验更新
        {
            if (ky == playerData.GetInstance().selfData.keyId)
            {
                long a = 0; long b = 0;
                if (data.ContainsKey("ep"))//战斗经验
                {
                    a = long.Parse(data["ep"].ToString());//当前经验              
                }
                if (data.ContainsKey("plv"))//战队等级
                {
                    b = long.Parse(data["plv"].ToString());//当前等级 
                }
                playerData.GetInstance().RoleLvAndExpHandler((int)b, (int)a);
            }
        }
        if ((fg & (long)(UpdatePlayerInfoType.RewardCoin)) > 0)//悬赏币更新
        {
            if (ky == playerData.GetInstance().selfData.keyId)
            {
                if (data.ContainsKey("rc"))//悬赏币更新
                {
                    long a = long.Parse(data["rc"].ToString());
                    playerData.GetInstance().RoleMoneyHadler(MoneyType.RewardCoin, a);
                }
            }
        }
        return true;
    }


    //场景中其他物体行走消息
    public bool ScenceElementWalkHandle(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        long fg = long.Parse(data["fg"].ToString());

        if ((fg & (long)(UpdateMapElementInfoType.Position)) > 0)
        {
            MapInfoNode tempMN = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList[int.Parse(data["si"].ToString())];
            CreatePeople.GetInstance().MoveOtherObject(uint.Parse(data["ky"].ToString()),
            float.Parse(data["px"].ToString()) + tempMN.Xmin,
            float.Parse(data["py"].ToString()),
            float.Parse(data["pz"].ToString()) + tempMN.Zmin,
            int.Parse(data["rc"].ToString())

            );
            int.Parse(data["tp"].ToString());
        }


        return true;
    }

    public bool PingBackHandle(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;

        uint ct_h = uint.Parse(data["ch"].ToString());
        uint ct_l = uint.Parse(data["cl"].ToString());
        uint st_h = uint.Parse(data["sh"].ToString());
        uint st_l = uint.Parse(data["sl"].ToString());
        {
            System.UInt64 tempNum = ((System.UInt64)(st_h) << 32 | (System.UInt64)(st_l));
            Debug.Log(tempNum);
            if (!ClientSendDataMgr.GetSingle().GetWalkSend().ping)
            {
                ClientSendDataMgr.GetSingle().GetWalkSend().ping = true;
                ClientSendDataMgr.GetSingle().GetWalkSend().Ping();
            }

            // DateTime tempT = new DateTime( 1970 , 1 , 1 , 0 , 0 , 0 );
            // long tempL = long.Parse( tempNum.ToString() );
            // DateTime tempDT1 = tempT.AddMilliseconds( tempL );
            //DateTime tempDT = tempDT1.ToLocalTime();
            //int a1 = tempDT.Year;
            //int a2 = tempDT.Month;
            //int a3 = tempDT.Day;
            //int a4 = tempDT.Hour;
            //int a5 = tempDT.Minute;
            //int a6 = tempDT.Second;
            //int a7 = tempDT.Millisecond;
        }
        return true;
    }

    //人物攻击表现
    public bool AttackHandle(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        uint attackKeyId = uint.Parse(data["sk"].ToString());
        uint targetKeyId = uint.Parse(data["tk"].ToString());//目标id
        int skillIndex = int.Parse(data["sd"].ToString());//技能索引
        Vector3 pos = new Vector3(//玩家位置
            packet.GetFloat("px"),
            packet.GetFloat("py"),
           packet.GetFloat("pz"));
        Vector3 Rot = new Vector3(//玩家朝向
            packet.GetFloat("ox"),
            packet.GetFloat("oy"),
           packet.GetFloat("oz"));
        Vector3 tpos = new Vector3(//目标位置
            packet.GetFloat("tx"),
            packet.GetFloat("ty"),
           packet.GetFloat("tz"));

        AttackType attackType = AttackType.action;
        if (CreatePeople.GetInstance().MonsterDic.ContainsKey(targetKeyId))
        {
            attackType = AttackType.Monster;
        }

        if (CreatePeople.GetInstance().OtherplayerDic.ContainsKey(targetKeyId) || targetKeyId == 0)
        {
            attackType = AttackType.OtherPlayer;
        }
        if (playerData.GetInstance().selfData.keyId == targetKeyId)
        {
            attackType = AttackType.Player;
        }
        CreatePeople.GetInstance().OtherPlayerAttack(attackKeyId, targetKeyId, attackType, skillIndex, pos, Rot, tpos);
        return true;
    }

    public bool HpHandle(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        uint attackKeyId = uint.Parse(data["sk"].ToString());
        uint targetKeyId = uint.Parse(data["tk"].ToString());
        int hp = int.Parse(data["hp"].ToString());
        int skillId = int.Parse(data["sd"].ToString());
        float baseValue = float.Parse(data["bv"].ToString());
        CreatePeople.GetInstance().OtherObjectDamage(attackKeyId, targetKeyId, hp, skillId, baseValue);

        return true;
    }

    public bool DeadHandle(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        uint attackKeyId = uint.Parse(data["sk"].ToString());
        uint targetKeyId = uint.Parse(data["tk"].ToString());
        int rc = int.Parse(data["rc"].ToString());
        if (targetKeyId == 0 || targetKeyId == playerData.GetInstance().selfData.keyId)//自己死亡
        {
            CharacterManager.player.GetComponent<CharacterState>().SetDead();
            if (Singleton<SceneManage>.Instance.Current == EnumSceneID.LGhuangyuan)
            {
                if (UIDeadToReborn.GetInstance() != null)
                    UIDeadToReborn.GetInstance().show();
            }


        }
        else//其他人死亡
        {

            CreatePeople.GetInstance().OtherObjectDead(targetKeyId);

        }
        return true;
    }
    //怪物打人
    public bool MonsterAttackHandle(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        uint attackKeyId = uint.Parse(data["sk"].ToString());
        uint targetKeyId = uint.Parse(data["tk"].ToString());
        int skillId = int.Parse(data["sd"].ToString());
        CreatePeople.GetInstance().MonsterAttack(attackKeyId, targetKeyId, skillId);
        return true;
    }
}
