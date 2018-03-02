using UnityEngine;
using System.Collections;
using System;
using Tianyu;
using System.Collections.Generic;

public enum OpenLevelType
{
    NormalOpen,
    GwtWay,
    ByIDOpen,
    SysOpen//系统开启时触发的
}

public class UILevel : GUIBase
{

    public static UILevel instance;

    public Transform TfWorldMap;
    public Transform TfMapContainer;
    public Transform TfSceneEntry = null;

    public OpenLevelType openType = OpenLevelType.NormalOpen;

    bool taskOpen = false;
    bool isReceiveData = false;

    public UILevel()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UILevel;
    }

    //void OnEnable()
    //{
    //    if (openType != OpenLevelType.NormalOpen) return;
    //    ShowMap();
    //    Control.PlayBgmWithUI(GameLibrary.UILevel);
    //}

    protected override void ShowHandler()
    {
        base.ShowHandler();
        if (openType == OpenLevelType.NormalOpen)
        {
            ShowMap();
        }
        else if (openType == OpenLevelType.ByIDOpen)
        {
            OpenLevel();
        }
        Control.PlayBgmWithUI(UIPanleID.UILevel);
    }

    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams.Length > 0)
        {
            openType = (OpenLevelType)uiParams[0];
            if (uiParams.Length > 1)
            {
                taskOpen = true;
                mapid = (int)uiParams[1];
            }
        }
        base.SetUI(uiParams);
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        if (CheckMapData())
        {
            isReceiveData = true;
            //ClientSendDataMgr.GetSingle().GetBattleSend().SendQueryWorldMap();//获取世界副本

            Singleton<Notification>.Instance.RegistMessageID(MessageID.pve_worldmap_list_ret, this.GetUIKey());
            Singleton<Notification>.Instance.RegistMessageID(MessageID.pve_worldmap_list_ret, UIPanleID.UILevel);
            Singleton<Notification>.Instance.RegistMessageID(MessageID.pve_dungeon_list_ret, this.GetUIKey());
            Singleton<Notification>.Instance.RegistMessageID(MessageID.pve_dungeon_list_ret, UIPanleID.UILevel);
            Singleton<Notification>.Instance.Send(MessageID.pve_worldmap_list_req, C2SMessageType.ActiveWait);

        }
        else
        {
            //if (taskOpen)
            //{
            //    //OpenLevel();
            //}
            //else
            //{
                Show();
            //}
        }
    }

    bool isShow = false;

    public override void ReceiveData(uint messageID)
    {
        base.ReceiveData(messageID);

        if (!isReceiveData) return;

        switch (messageID)
        {
            case MessageID.pve_worldmap_list_ret:
                Dictionary<string, object> newpacket1 = new Dictionary<string, object>();
                newpacket1.Add("arg1", playerData.GetInstance().worldMap);
                newpacket1.Add("arg2", 1);
                Singleton<Notification>.Instance.Send(MessageID.pve_dungeon_list_req, newpacket1, C2SMessageType.ActiveWait);
                Dictionary<string, object> newpacket2 = new Dictionary<string, object>();
                newpacket2.Add("arg1", playerData.GetInstance().worldMap);
                newpacket2.Add("arg2", 2);
                Singleton<Notification>.Instance.Send(MessageID.pve_dungeon_list_req, newpacket2, C2SMessageType.ActiveWait);
                //ClientSendDataMgr.GetSingle().GetBattleSend().SendQueryDungeonList(playerData.GetInstance().worldMap, 1);
                //ClientSendDataMgr.GetSingle().GetBattleSend().SendQueryDungeonList(playerData.GetInstance().worldMap, 2);
                break;
            case MessageID.pve_dungeon_list_ret:
                if (isShow)
                {
                    switch (openType)
                    {
                        case OpenLevelType.NormalOpen:
                            Show();
                            break;
                        case OpenLevelType.GwtWay:
                            Control.Show(UIPanleID.UIGoodsGetWayPanel);
                            break;
                        case OpenLevelType.ByIDOpen:
                            Show();
                            //OpenLevel();
                            break;
                        case OpenLevelType.SysOpen:
                            if (IsShow())
                                Show();
                            break;
                    }
                    isShow = false;
                    isReceiveData = false;
                }
                else
                {
                    isShow = true;
                }
                break;
        }
    }

    protected override void OnDoDestroy()
    {
        base.OnDoDestroy();


    }

    public void HideUILevel()
    {
        Control.HideGUI();
        //Control.HideGUI(UIPanleID.UILevel);
    }

    void ShowMap()
    {
        if (CheckMapData())
        {
            return;
        }
        else
        {
            Control.HideGUI(UIPanleID.UIMoney);
            TfMapContainer.gameObject.SetActive(true);
            taskOpen = false;
            if (Globe.LastMapID == 0)
            {
                if (playerData.GetInstance().CanEnterMap.Contains(playerData.GetInstance().worldMap[playerData.GetInstance().worldMap.Count - 1]))
                {
                    TfMapContainer.GetComponent<UIMapContainer>().RefreshUI(FSDataNodeTable<MapNode>.GetSingleton().FindDataByType(playerData.GetInstance().worldMap[playerData.GetInstance().worldMap.Count - 1]));
                }
                else
                {
                    TfMapContainer.GetComponent<UIMapContainer>().RefreshUI(FSDataNodeTable<MapNode>.GetSingleton().FindDataByType(playerData.GetInstance().CanEnterMap[playerData.GetInstance().CanEnterMap.Count - 1]));
                }
            }
            else
            {
                TfMapContainer.GetComponent<UIMapContainer>().RefreshUI(FSDataNodeTable<MapNode>.GetSingleton().FindDataByType(Globe.LastMapID), Globe.LastMapType);
            }
        }
    }

    public void SaveMapID(int mapID, int types)
    {
        if (taskOpen) return;
        Globe.LastMapID = mapID;
        Globe.LastMapType = types;
    }

    void FirstOpen()
    {
        MapNode mapOrdinary = FSDataNodeTable<MapNode>.GetSingleton().FindDataByType(playerData.GetInstance().CanEnterMap[playerData.GetInstance().CanEnterMap.Count - 1]);
        if (playerData.GetInstance().CanEnterMap.Count < 2)
        {
            TfMapContainer.GetComponent<UIMapContainer>().RefreshUI(mapOrdinary);
        }
        else
        {
            //int ordinaryID = playerData.GetInstance().CanEnterMap[playerData.GetInstance().CanEnterMap.Count - 1];
            int eliteID = playerData.GetInstance().CanEnterMap[playerData.GetInstance().CanEnterMap.Count - 2];
            int maxOrdinaryID = 0;
            int maxEliteID = 0;
            MapNode mapElite = FSDataNodeTable<MapNode>.GetSingleton().FindDataByType(eliteID);
            CheckMaxMapID(mapOrdinary.ordinary, mapOrdinary, ref maxOrdinaryID);
            CheckMaxMapID(mapElite.elite, mapElite, ref maxEliteID);
            if (maxOrdinaryID == 0 || maxEliteID == 0 || maxOrdinaryID - maxEliteID >= 115)
            {
                TfMapContainer.GetComponent<UIMapContainer>().RefreshUI(mapOrdinary);
            }
            else
            {
                TfMapContainer.GetComponent<UIMapContainer>().RefreshUI(mapElite, 2);
            }
        }
    }

    void CheckMaxMapID(long[] map, MapNode mapNode, ref int maxID)
    {
        for (int i = 0; i < map.Length; i++)
        {
            if (GameLibrary.mapOrdinary.ContainsKey(mapNode.MapId) && GameLibrary.mapOrdinary[mapNode.MapId].ContainsKey((int)map[i]))
            {
                if (Globe.GetStar(GameLibrary.mapOrdinary[mapNode.MapId][(int)map[i]]) <= -1)
                {
                    maxID = (int)map[i - 1];
                    break;
                }
                else
                {
                    maxID = (int)map[i];
                }
            }
        }
    }

    #region 外部接口打开副本界面
    int mapid;

    public void OpenLevel()
    {
        Control.HideGUI(UIPanleID.UIMoney);
        if (mapid != 0 && mapid % 100 == 0)
        {
            OpenMap();
        }
        else
        {
            OpenScene();
        }
    }

    public bool GetLevelData()
    {
        if (CheckMapData())
        {
            openType = OpenLevelType.GwtWay;
            ClientSendDataMgr.GetSingle().GetBattleSend().SendQueryWorldMap();//获取世界副本
            return false;
        }
        return true;
    }

    void OpenMap()
    {
        MapNode map = FSDataNodeTable<MapNode>.GetSingleton().FindDataByType(mapid);
        if (null == map)
        {
            Globe.backPanelParameter = null;
            Debug.Log("No configuration is read : " + mapid);
            return;
        }
        TfMapContainer.gameObject.SetActive(true);
        if (TfMapContainer.GetComponent<UIMapContainer>() != null)
            TfMapContainer.GetComponent<UIMapContainer>().RefreshUI(map, Globe.backPanelParameter != null ? (int)Globe.backPanelParameter[2] : 1);
        else
        {
            Debug.Log("没有相应的组件");
        }
        Globe.backPanelParameter = null;
    }

    void OpenScene()
    {
        SceneNode scene = FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(mapid != 0 ? mapid : 102);
        if (null == scene)
        {
            Debug.Log("No configuration is read : " + mapid);
            return;
        }
        Control.HideGUI(UIPanleID.UIMoney);
        TfMapContainer.gameObject.SetActive(true);

        TfWorldMap.GetComponent<UIWorldMap>().OnMapBtnClick(scene.bigmap_id);
        if (scene.Type == 2 && TfMapContainer.GetComponent<UIMapContainer>().isOpenElite)
            TfMapContainer.GetComponent<UIMapContainer>().ChangeDifficultyToElite();

        Dictionary<int, int[]> mapInfo = new Dictionary<int, int[]>();
        if (!GameLibrary.mapOrdinary.TryGetValue(scene.bigmap_id, out mapInfo)) return;

        if (mapInfo.ContainsKey(mapid) && mapid != 0)
        {
            if (Globe.GetStar(mapInfo[mapid]) >= 0)
            {
                TfMapContainer.GetComponent<UIMapContainer>().ShowSceneEntry(scene);
            }
        }
    }

    bool CheckMapData()
    {
        return null == playerData.GetInstance().worldMap || playerData.GetInstance().worldMap.Count <= 0;
    }
    #endregion

}