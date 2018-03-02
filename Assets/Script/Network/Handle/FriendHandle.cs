/*
文件名（File Name）:   FriendHandle.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tianyu;

public class CFriendHandle : CHandleBase
{
    public CFriendHandle(CHandleMgr mgr) : base(mgr)
    {
    }
    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.common_recommended_friend_list_ret, FriendsRecommendResult);
        RegistHandle(MessageID.common_player_friend_list_ret, FriendsListResult);
        RegistHandle(MessageID.common_search_friend_ret, FriendsSearchResult);
        RegistHandle(MessageID.common_add_friend_ret, FriendsAddtResult);
        RegistHandle(MessageID.common_delete_friend_ret, FriendsDeleteResult);
        RegistHandle(MessageID.common_friend_function_listret, FriendsFunctionResult);
        RegistHandle(MessageID.common_allow_add_friend_ret, AgreeOrRefuseResult);
    }

    private bool FriendsFunctionResult(CReadPacket packet)
    {

        Debug.Log(" FriendsFunctionResult   接收申请列表或黑名单");
        Dictionary<string, object> data = packet.data;

        int result = int.Parse(data["ret"].ToString());
        if (result == 0)
        {
            if (data.ContainsKey("item"))
            {
                playerData.GetInstance().friendListData.applyforList.Clear();
                if (null != data["item"] as object[])
                {
                    object[] goodList = data["item"] as object[];
                    for (int i = 0; i < goodList.Length; i++)
                    {
                        Dictionary<string, object> goodInfo = (Dictionary<string, object>)goodList[i];
                        FriendData info = new FriendData();
                        if (goodInfo.ContainsKey("nm"))
                        {
                            info.Name = goodInfo["nm"].ToString();
                        }
                        if (goodInfo.ContainsKey("id"))
                        {
                            info.PlayerId = long.Parse(goodInfo["id"].ToString());
                        }
                        if (goodInfo.ContainsKey("aid"))
                        {
                            info.AcountPlayerId = long.Parse(goodInfo["aid"].ToString());
                        }
                        if (goodInfo.ContainsKey("ptf"))
                        {

                            if (null != FSDataNodeTable<RoleIconAttrNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["ptf"].ToString())))
                            {
                                RoleIconAttrNode vo = FSDataNodeTable<RoleIconAttrNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["ptf"].ToString()));
                                info.PlayerFrame = vo.icon_name;
                            }
                        }
                        if (goodInfo.ContainsKey("lv"))
                        {
                            info.Level = int.Parse(goodInfo["lv"].ToString());
                        }
                        if (goodInfo.ContainsKey("hlv"))
                        {
                            info.HeroLevel = int.Parse(goodInfo["hlv"].ToString());
                        }
                        if (goodInfo.ContainsKey("pt"))
                        {
                            if (null != FSDataNodeTable<HeroNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["pt"].ToString())))
                            {
                                HeroNode vo = FSDataNodeTable<HeroNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["pt"].ToString()));
                                info.PlayerIcon = vo.icon_name + "_head";
                            }
                        }
                        if (goodInfo.ContainsKey("afc"))
                        {
                            info.Fighting = int.Parse(goodInfo["afc"].ToString());
                        }
                        if (goodInfo.ContainsKey("tl"))
                        {
                            if (0 != int.Parse(goodInfo["tl"].ToString()))
                            {
                                if (null != FSDataNodeTable<TitleNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["tl"].ToString())))
                                {
                                    TitleNode vo = FSDataNodeTable<TitleNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["tl"].ToString()));
                                    info.Title = vo.titlename;
                                }
                            }
                            else
                            {
                                info.Title = "称号:无";
                            }
                        }
                        playerData.GetInstance().friendListData.applyforList.Add(info);
                    }
                }
                else
                {
                    return false;
                }

            }
            return true;
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            return false;
        }

    }

    private bool FriendsDeleteResult(CReadPacket packet)
    {
        Debug.Log(" FriendsDeleteResult     删除好友");
        Dictionary<string, object> data = packet.data;

        int result = int.Parse(data["ret"].ToString());
        if (result == 0)
        {
            return true;
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            return false;
        }

    }

    private bool FriendsAddtResult(CReadPacket packet)
    {
        Debug.Log(" FriendsAddtResult  添加好友");
        Dictionary<string, object> data = packet.data;

        int result = int.Parse(data["ret"].ToString());
        if (result == 0)
        {
            return true;
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            return false;
        }

    }

    private bool FriendsSearchResult(CReadPacket packet)
    {
        Debug.Log(" FriendsSearchResult     搜索好友");
        Dictionary<string, object> data = packet.data;

        int result = int.Parse(data["ret"].ToString());
        if (result == 0)
        {
            if (data.ContainsKey("item"))
            {
                playerData.GetInstance().friendListData.searchList.Clear();
                if (null != data["item"] as object[])
                {
                    object[] goodList = data["item"] as object[];
                    for (int i = 0; i < goodList.Length; i++)
                    {
                        Dictionary<string, object> goodInfo = (Dictionary<string, object>)goodList[i];
                        FriendData info = new FriendData();
                        if (goodInfo.ContainsKey("nm"))
                        {
                            info.Name = goodInfo["nm"].ToString();
                        }
                        if (goodInfo.ContainsKey("id"))
                        {
                            info.PlayerId = long.Parse(goodInfo["id"].ToString());
                        }
                        if (goodInfo.ContainsKey("aid"))
                        {
                            info.AcountPlayerId = long.Parse(goodInfo["aid"].ToString());
                        }
                        if (goodInfo.ContainsKey("ptf"))
                        {

                            if (null != FSDataNodeTable<RoleIconAttrNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["ptf"].ToString())))
                            {
                                RoleIconAttrNode vo = FSDataNodeTable<RoleIconAttrNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["ptf"].ToString()));
                                info.PlayerFrame = vo.icon_name;
                            }
                        }
                        if (goodInfo.ContainsKey("lv"))
                        {
                            info.Level = int.Parse(goodInfo["lv"].ToString());
                        }
                        if (goodInfo.ContainsKey("hlv"))
                        {
                            info.HeroLevel = int.Parse(goodInfo["hlv"].ToString());
                        }
                        if (goodInfo.ContainsKey("pt"))
                        {
                            if (null != FSDataNodeTable<HeroNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["pt"].ToString())))
                            {
                                HeroNode vo = FSDataNodeTable<HeroNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["pt"].ToString()));
                                info.PlayerIcon = vo.icon_name + "_head";
                            }
                        }
                        if (goodInfo.ContainsKey("afc"))
                        {
                            info.Fighting = int.Parse(goodInfo["afc"].ToString());
                        }
                        if (goodInfo.ContainsKey("tl"))
                        {
                            if (0 != int.Parse(goodInfo["tl"].ToString()))
                            {
                                if (null != FSDataNodeTable<TitleNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["tl"].ToString())))
                                {
                                    TitleNode vo = FSDataNodeTable<TitleNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["tl"].ToString()));
                                    info.Title = vo.titlename;
                                }
                            }
                            else
                            {
                                info.Title = "称号:无";
                            }
                        }
                        playerData.GetInstance().friendListData.searchList.Add(info);
                    }
                }
            }
            return true;
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            return false;
        }

    }

    private bool FriendsListResult(CReadPacket packet)
    {
        Debug.Log(" FriendsListResult   好友列表");
        Dictionary<string, object> data = packet.data;

        int result = int.Parse(data["ret"].ToString());
        if (result == 0)
        {
            if (data.ContainsKey("item"))
            {
                playerData.GetInstance().friendListData.friendList.Clear();
                if (null != data["item"] as object[])
                {
                    object[] goodList = data["item"] as object[];
                    for (int i = 0; i < goodList.Length; i++)
                    {
                        Dictionary<string, object> goodInfo = (Dictionary<string, object>)goodList[i];
                        FriendData info = new FriendData();
                        if (goodInfo.ContainsKey("nm"))
                        {
                            info.Name = goodInfo["nm"].ToString();
                        }
                        if (goodInfo.ContainsKey("id"))
                        {
                            info.PlayerId = long.Parse(goodInfo["id"].ToString());
                        }
                        if (goodInfo.ContainsKey("aid"))
                        {
                            info.AcountPlayerId = long.Parse(goodInfo["aid"].ToString());
                        }
                        if (goodInfo.ContainsKey("ptf"))
                        {

                            if (null != FSDataNodeTable<RoleIconAttrNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["ptf"].ToString())))
                            {
                                RoleIconAttrNode vo = FSDataNodeTable<RoleIconAttrNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["ptf"].ToString()));
                                info.PlayerFrame = vo.icon_name;
                            }
                        }
                        if (goodInfo.ContainsKey("lv"))
                        {
                            info.Level = int.Parse(goodInfo["lv"].ToString());
                        }
                        if (goodInfo.ContainsKey("hlv"))
                        {
                            info.HeroLevel = int.Parse(goodInfo["hlv"].ToString());
                        }
                        if (goodInfo.ContainsKey("pt"))
                        {
                            if (null != FSDataNodeTable<HeroNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["pt"].ToString())))
                            {
                                HeroNode vo = FSDataNodeTable<HeroNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["pt"].ToString()));
                                info.PlayerIcon = vo.icon_name + "_head";
                            }
                        }
                        if (goodInfo.ContainsKey("afc"))
                        {
                            info.Fighting = int.Parse(goodInfo["afc"].ToString());
                        }
                        if (goodInfo.ContainsKey("tl"))
                        {
                            if (0 != int.Parse(goodInfo["tl"].ToString()))
                            {
                                if (null != FSDataNodeTable<TitleNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["tl"].ToString())))
                                {
                                    TitleNode vo = FSDataNodeTable<TitleNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["tl"].ToString()));
                                    info.Title = vo.titlename;
                                }
                            }
                            else
                            {
                                info.Title = "称号:无";
                            }
                        }
                        playerData.GetInstance().friendListData.friendList.Add(info);
                    }
                }
            }
            return true;
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            return false;
        }

    }

    private bool FriendsRecommendResult(CReadPacket packet)
    {
        Debug.Log(" FriendsRecommendResult  推荐好友");
        Dictionary<string, object> data = packet.data;

        int result = int.Parse(data["ret"].ToString());
        if (result == 0)
        {
            if (data.ContainsKey("item"))
            {

                playerData.GetInstance().friendListData.RecommendfriendList.Clear();
                if (null != data["item"] as object[])
                {
                    object[] goodList = data["item"] as object[];
                    for (int i = 0; i < goodList.Length; i++)
                    {
                        Dictionary<string, object> goodInfo = (Dictionary<string, object>)goodList[i];
                        FriendData info = new FriendData();
                        if (goodInfo.ContainsKey("nm"))
                        {
                            info.Name = goodInfo["nm"].ToString();
                        }
                        if (goodInfo.ContainsKey("id"))
                        {
                            info.PlayerId = long.Parse(goodInfo["id"].ToString());
                        }
                        if (goodInfo.ContainsKey("aid"))
                        {
                            info.AcountPlayerId = long.Parse(goodInfo["aid"].ToString());
                        }
                        if (goodInfo.ContainsKey("ptf"))
                        {

                            if (null != FSDataNodeTable<RoleIconAttrNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["ptf"].ToString())))
                            {
                                RoleIconAttrNode vo = FSDataNodeTable<RoleIconAttrNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["ptf"].ToString()));
                                info.PlayerFrame = vo.icon_name;
                            }
                        }
                        if (goodInfo.ContainsKey("lv"))
                        {
                            info.Level = int.Parse(goodInfo["lv"].ToString());
                        }
                        if (goodInfo.ContainsKey("hlv"))
                        {
                            info.HeroLevel = int.Parse(goodInfo["hlv"].ToString());
                        }
                        if (goodInfo.ContainsKey("pt"))
                        {
                            if (null != FSDataNodeTable<HeroNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["pt"].ToString())))
                            {
                                HeroNode vo = FSDataNodeTable<HeroNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["pt"].ToString()));
                                info.PlayerIcon = vo.icon_name + "_head";
                            }
                        }
                        if (goodInfo.ContainsKey("afc"))
                        {
                            info.Fighting = int.Parse(goodInfo["afc"].ToString());
                        }
                        if (goodInfo.ContainsKey("tl"))
                        {
                            if (0 != int.Parse(goodInfo["tl"].ToString()))
                            {
                                if (null != FSDataNodeTable<TitleNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["tl"].ToString())))
                                {
                                    TitleNode vo = FSDataNodeTable<TitleNode>.GetSingleton().FindDataByType(int.Parse(goodInfo["tl"].ToString()));
                                    info.Title = vo.titlename;
                                }
                            }
                            else
                            {
                                info.Title = "称号:无";
                            }
                        }
                        playerData.GetInstance().friendListData.RecommendfriendList.Add(info);

                    }
                    if (playerData.GetInstance().friendListData.RecommendfriendList.Count > 0)
                    {

                        List<FriendData> itemLt = new List<FriendData>();
                        playerData.GetInstance().friendListData.RecommendfriendList.Sort((a, b) => b.Fighting - a.Fighting);
                        FriendData temp = null;
                        for (int i = 0; i < playerData.GetInstance().friendListData.RecommendfriendList.Count; i++)
                        {
                            for (int j = i + 1; j < playerData.GetInstance().friendListData.RecommendfriendList.Count; j++)
                            {
                                if (playerData.GetInstance().friendListData.RecommendfriendList[j].Fighting == playerData.GetInstance().friendListData.RecommendfriendList[i].Fighting && playerData.GetInstance().friendListData.RecommendfriendList[j].Level > playerData.GetInstance().friendListData.RecommendfriendList[i].Level)
                                {
                                    temp = playerData.GetInstance().friendListData.RecommendfriendList[j];
                                    playerData.GetInstance().friendListData.RecommendfriendList[j] = playerData.GetInstance().friendListData.RecommendfriendList[i];
                                    playerData.GetInstance().friendListData.RecommendfriendList[i] = temp;
                                }
                            }
                        }
                    }
                }

            }
            return true;
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            return false;
        }

    }
    private bool AgreeOrRefuseResult(CReadPacket packet)
    {
        Debug.Log(" FriendsRecommendResult  同意或拒绝加为好友");
        Dictionary<string, object> data = packet.data;

        int result = int.Parse(data["ret"].ToString());
        if (result == 0)
        {
            return true;
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            return false;
        }

    }
}
