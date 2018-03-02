using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using UnityEngine.SceneManagement;

public class CSocietyHandle : CHandleBase
{
    public CSocietyHandle(CHandleMgr mgr) : base(mgr)
    {
    }
    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.union_query_uncion_list_ret, SocietyListResult);
        RegistHandle(MessageID.union_search_someone_ret, SearchSocietyListResult);
        RegistHandle(MessageID.union_create_someone_ret, CreateSocietyResult);
        RegistHandle(MessageID.union_disband_someone_ret, DissolveSocietyResult);
        RegistHandle(MessageID.union_application_join_ret, ApplicationJoinSocietyResult);
        RegistHandle(MessageID.union_query_application_list_ret, ApplicationJoinSocietyListResult);
        RegistHandle(MessageID.union_approve_application_ret, ApproveJoinSocietyResult);
        RegistHandle(MessageID.union_exits_someone_ret, ExitSocietyResult);
        RegistHandle(MessageID.union_kickout_someone_ret, KickoutSocietyMemberResult);
        RegistHandle(MessageID.union_change_someone_position_ret, PresidentChangeResult);
        RegistHandle(MessageID.union_query_all_member_ret, GetSocietyMemberListResult);
        RegistHandle(MessageID.union_change_some_info_ret, ChangeSocietyInfoResult);
        RegistHandle(MessageID.union_query_detailed_info_ret, GetSocietyInfoResult);
        RegistHandle(MessageID.union_information_changed_notify, PlayerSocietyInfoUpdate);
    }
    public bool SocietyListResult(CReadPacket packet)
    {
        Debug.Log("已创建公会列表结果");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        object[] itemList = data["item"] as object[];
        if (resolt == 0)
        {
            SocietyManager.Single().societyList.Clear();
            //SocietyManager.Single().playerApplicationSocietyList.Clear();
            if (data.ContainsKey("auj"))
            {
                SocietyManager.Single().playerApplicationSocietyList = data["auj"] as int[];
                //if (idarr!=null)
                //{
                //    if (idarr.Length > 0)
                //    {
                //        for (int j = 0; j < idarr.Length; j++)
                //        {
                //            SocietyManager.Single().playerApplicationSocietyList.Add(idarr[j]);
                //        }
                //    }
                //}

            }
            if (itemList != null)
            {
                for (int i = 0; i < itemList.Length; i++)
                {

                    Dictionary<string, object> itemDataDic = itemList[i] as Dictionary<string, object>;
                    if (itemDataDic != null)
                    {

                        SocietyData societyData = new SocietyData();

                        societyData.societyID = long.Parse(itemDataDic["ui"].ToString());
                        societyData.societyName = itemDataDic["un"].ToString();
                        societyData.societyManifesto = itemDataDic["mo"].ToString();
                        societyData.societyIcon = itemDataDic["ic"].ToString();
                        societyData.presidentId = long.Parse(itemDataDic["ci"].ToString());
                        societyData.presidentName = itemDataDic["cn"].ToString();
                        societyData.societyLevel = int.Parse(itemDataDic["lv"].ToString());
                        societyData.createTime = itemDataDic["ct"].ToString();
                        SocietyManager.Single().societyList.Add(societyData);
                    }

                }
            }

            //if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
            //{
            //    if (Control.GetGUI(GameLibrary.UINotJoinSocietyPanel).gameObject.activeSelf)
            //    {
            //        //刷新一下公会列表界面
            //        Debug.Log("刷新公会列表..");
            //        UINotJoinSocietyPanel.Instance.RefreshData(0);
            //    }
            //}
        }
        else
        {
            Debug.Log(string.Format("获取已创建公会列表失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
        }
        return true;
    }
    public bool SearchSocietyListResult(CReadPacket packet)
    {
        Debug.Log("搜索公会列表结果");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        object[] itemList = data["item"] as object[];
        if (resolt == 0)
        {
            SocietyManager.Single().searchSocietyList.Clear();
            if (itemList != null)//如果为空就是没搜索到
            {
                for (int i = 0; i < itemList.Length; i++)
                {

                    Dictionary<string, object> itemDataDic = itemList[i] as Dictionary<string, object>;
                    if (itemDataDic != null)
                    {

                        SocietyData societyData = new SocietyData();

                        societyData.societyID = long.Parse(itemDataDic["ui"].ToString());
                        societyData.societyName = itemDataDic["un"].ToString();
                        societyData.societyManifesto = itemDataDic["mo"].ToString();
                        societyData.societyIcon = itemDataDic["ic"].ToString();
                        societyData.presidentId = long.Parse(itemDataDic["ci"].ToString());
                        societyData.presidentName = itemDataDic["cn"].ToString();
                        societyData.societyLevel = int.Parse(itemDataDic["lv"].ToString());
                        societyData.createTime = itemDataDic["ct"].ToString();
                        SocietyManager.Single().searchSocietyList.Add(societyData);

                    }

                }
            }
            //if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
            //{
            //    if (Control.GetGUI(GameLibrary.UINotJoinSocietyPanel).gameObject.activeSelf)
            //    {
            //        //刷新一下公会列表界面
            //        Debug.Log("刷新搜索列表..");
            //        UINotJoinSocietyPanel.Instance.RefreshData(1);
            //    }
            //}

        }
        else
        {
            Debug.Log(string.Format("搜索公会列表失败：{0}", data["desc"].ToString()));
            //SocietyManager.Single().searchSocietyList.Clear();
            //if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
            //{
            //    if (Control.GetGUI(GameLibrary.UINotJoinSocietyPanel).gameObject.activeSelf)
            //    {
            //        //刷新一下公会列表界面
            //        Debug.Log("刷新搜索列表..");
            //        UINotJoinSocietyPanel.Instance.RefreshData(1);
            //    }
            //}
        }
        return true;
    }
    public bool CreateSocietyResult(CReadPacket packet)
    {
        Debug.Log("创建公会结果");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            //创建公会成功关闭未加入公会的面板
            //Control.HideGUI(GameLibrary.UINotJoinSocietyPanel);
        }
        else
        {
            Debug.Log(string.Format("创建公会失败：{0}", data["desc"].ToString()));
            // UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            object[] obj = new object[5] { data["desc"].ToString(), null, UIPopupType.OnlyShow, null, null };
            Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
        }
        return true;
    }
    public bool DissolveSocietyResult(CReadPacket packet)
    {
        Debug.Log("解散公会结果");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "解散公会成功");
            //if (SceneManager.GetActiveScene().name == GameLibrary.UI_Major)
            //{
            //    if (Control.GetGUI(GameLibrary.UIHaveJoinSocietyPanel).gameObject.activeSelf)
            //    {
            //        Control.HideGUI(GameLibrary.UIHaveJoinSocietyPanel);
            //    }
            //}
            return true;
        }
        else
        {
            Debug.Log(string.Format("解散公会失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString()); 
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            return false;
        }

    }
    public bool ApplicationJoinSocietyResult(CReadPacket packet)
    {
        Debug.Log("申请加入公会结果");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            if (data.ContainsKey("auj"))
            {
                SocietyManager.Single().playerApplicationSocietyList = data["auj"] as int[];
            }
        }
        else
        {
            Debug.Log(string.Format("申请加入公会失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
        }
        return true;
    }
    public bool ApplicationJoinSocietyListResult(CReadPacket packet)
    {
        Debug.Log("获取工会申请列表结果");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());



        if (resolt == 0)
        {

            SocietyManager.Single().SocietyApplicationList.Clear();
            if (data.ContainsKey("item"))
            {
                object[] itemList = itemList = data["item"] as object[];
                if (itemList != null)//如果为空就是没搜索到
                {
                    for (int i = 0; i < itemList.Length; i++)
                    {

                        Dictionary<string, object> itemDataDic = itemList[i] as Dictionary<string, object>;
                        if (itemDataDic != null)
                        {

                            SocietyApplicationData applicationData = new SocietyApplicationData();
                            applicationData.playerId = long.Parse(itemDataDic["playerId"].ToString());
                            applicationData.playeName = itemDataDic["name"].ToString();
                            applicationData.applicationTime = itemDataDic["ct"].ToString();
                            SocietyManager.Single().SocietyApplicationList.Add(applicationData);

                        }

                    }
                }
            }
            //if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
            //{
            //    if (Control.GetGUI(GameLibrary.UIHaveJoinSocietyPanel).gameObject.activeSelf)
            //    {
            //        //刷新公会申请列表
            //        UIHaveJoinSocietyPanel.Instance.SetApplicationSocietyList();
            //    }
            //}
        }
        else
        {
            Debug.Log(string.Format("获取工会申请列表失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
        }
        return true;
    }
    public bool ApproveJoinSocietyResult(CReadPacket packet)
    {
        Debug.Log("批准某人进入工会结果");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            //我自己再获取一下公会申请列表
            //ClientSendDataMgr.GetSingle().GetSocietySend().SendGetApplicationJoinSocietyList(C2SMessageType.ActiveWait);
        }
        else
        {
            Debug.Log(string.Format("批准某人进入工会失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
        }
        return true;
    }
    public bool ExitSocietyResult(CReadPacket packet)
    {
        Debug.Log("退出工会结果");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "退出公会成功");
            //if (SceneManager.GetActiveScene().name == GameLibrary.UI_Major)
            //{
            //    if (Control.GetGUI(GameLibrary.UIHaveJoinSocietyPanel).gameObject.activeSelf)
            //    {
            //        Control.HideGUI(GameLibrary.UIHaveJoinSocietyPanel);
            //    }
            //}
            return true;
        }
        else
        {
            Debug.Log(string.Format("退出工会失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            return false;
        }
    }
    public bool KickoutSocietyMemberResult(CReadPacket packet)
    {
        Debug.Log("踢出公会成员结果");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "踢出成功");
            //再去获取一下公会成员列表
            Dictionary<string, object> newpacket1 = new Dictionary<string, object>();
            newpacket1.Add("arg1", SocietyManager.Single().mySocityID);
            Singleton<Notification>.Instance.Send(MessageID.union_query_all_member_req, newpacket1, C2SMessageType.ActiveWait);
            return true;
            //ClientSendDataMgr.GetSingle().GetSocietySend().SendGetSocietyMemberList(C2SMessageType.ActiveWait, SocietyManager.Single().mySocityID);
        }
        else
        {
            Debug.Log(string.Format("踢出公会成员失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            return false;
        }
    }
    public bool PresidentChangeResult(CReadPacket packet)
    {
        Debug.Log("会长转让结果");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "会长转让成功");
            return true;
        }
        else
        {
            Debug.Log(string.Format("会长转让失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            return false;
        }
    
    }
    public bool GetSocietyMemberListResult(CReadPacket packet)
    {
        Debug.Log("获取当前工会会员列表结果");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        object[] itemList = data["item"] as object[];
        if (resolt == 0)
        {
            SocietyManager.Single().societyMemberlist.Clear();
            if (itemList != null)//如果为空就是没搜索到
            {
                for (int i = 0; i < itemList.Length; i++)
                {

                    Dictionary<string, object> itemDataDic = itemList[i] as Dictionary<string, object>;
                    if (itemDataDic != null)
                    {

                        SocietyMemberData memberData = new SocietyMemberData();
                        memberData.playerId = long.Parse(itemDataDic["id"].ToString());
                        memberData.memberName = itemDataDic["nm"].ToString();
                        memberData.accountId = long.Parse(itemDataDic["ac"].ToString());
                        if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(int.Parse(itemDataDic["pt"].ToString())))
                        {
                            memberData.memberIcon = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[int.Parse(itemDataDic["pt"].ToString())].icon_name + "_head";
                        }
                        if (int.Parse(itemDataDic["up"].ToString()) == 1)
                        {
                            memberData.societyStatus = SocietyStatus.Member;
                        }
                        else if (int.Parse(itemDataDic["up"].ToString()) == 5)
                        {
                            memberData.societyStatus = SocietyStatus.President;
                        }
                        else
                        {
                            memberData.societyStatus = SocietyStatus.Null;
                        }
                        SocietyManager.Single().societyMemberlist.Add(memberData);
                    }

                }
            }
            //if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
            //{
            //    if (Control.GetGUI(GameLibrary.UIHaveJoinSocietyPanel).gameObject.activeSelf)
            //    {
            //        UIHaveJoinSocietyPanel.Instance.SetSocietyMemberList();
            //    }
            //}
        }
        else
        {
            Debug.Log(string.Format("获取当前工会会员列表失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
        }
        return true;
    }
    public bool ChangeSocietyInfoResult(CReadPacket packet)
    {
        Debug.Log("编辑工会信息结果");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "成功修改宣言");
            //修改宣言成功后 获取一下公会详情       
            return true;
            //ClientSendDataMgr.GetSingle().GetSocietySend().SendGetSocietyInfo(C2SMessageType.ActiveWait, SocietyManager.Single().mySocityID);
        }
        else
        {
            Debug.Log(string.Format("编辑工会信息失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            return false;
        }
    
    }
    public bool GetSocietyInfoResult(CReadPacket packet)
    {
        Debug.Log("获取某工会详细信息结果");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            SocietyData societyData = new SocietyData();
            SocietyManager.Single().playerSocietyData.societyID = long.Parse(data["ui"].ToString());
            SocietyManager.Single().playerSocietyData.societyName = data["un"].ToString();
            SocietyManager.Single().playerSocietyData.societyManifesto = data["mo"].ToString();
            SocietyManager.Single().playerSocietyData.societyIcon = data["ic"].ToString();
            SocietyManager.Single().playerSocietyData.presidentId = long.Parse(data["ci"].ToString());
            SocietyManager.Single().playerSocietyData.presidentName = data["cn"].ToString();
            SocietyManager.Single().playerSocietyData.societyLevel = int.Parse(data["lv"].ToString());
            SocietyManager.Single().playerSocietyData.createTime = data["ct"].ToString();
            SocietyManager.Single().playerSocietyData.societyMaxCount = int.Parse(data["mp"].ToString());
            SocietyManager.Single().playerSocietyData.societyCurrentCount = int.Parse(data["cp"].ToString());

            //if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
            //{
            //    if (Control.GetGUI(GameLibrary.UIHaveJoinSocietyPanel).gameObject.activeSelf)
            //    {
            //        UIHaveJoinSocietyPanel.Instance.SetSocietyInfo();
            //    }
            //}
        }
        else
        {
            Debug.Log(string.Format("获取某工会详细信息失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
        }
        return true;
    }
    public bool PlayerSocietyInfoUpdate(CReadPacket packet)
    {
        Debug.Log("玩家公会信息改变");
        //{tp=类型 1加入工会 2退出工会 3权限改变,ui=工会ID,un=工会名称,up=工会职位}
        Dictionary<string, object> data = packet.data;
        int type = int.Parse(data["tp"].ToString());
        SocietyManager.Single().mySocityID = long.Parse(data["ui"].ToString());
        SocietyManager.Single().societyName = data["un"].ToString();
        if (int.Parse(data["up"].ToString()) == 1)
        {
            SocietyManager.Single().societyStatus = SocietyStatus.Member;
        }
        else if (int.Parse(data["up"].ToString()) == 5)
        {
            SocietyManager.Single().societyStatus = SocietyStatus.President;
        }
        else
        {
            SocietyManager.Single().societyStatus = SocietyStatus.Null;
        }
        switch (type)
        {
            case 1://加入公会
                SocietyManager.Single().isJoinSociety = true;
                SetMainHeroName.Instance.RefreshSocietyName();
                Globe.isHaveSociety = true;
                //会长同意了玩家加入公会 玩家被推送该协议(其实也实用于会长创建公会 但是为了方便状态尽量分开)
                //if (SceneManager.GetActiveScene().name == GameLibrary.UI_Major)
                //{
                //    //如果未加入公会面板被打开 就关闭
                //    if (Control.GetGUI(GameLibrary.UINotJoinSocietyPanel).gameObject.activeSelf)
                //    {
                //        Control.HideGUI(GameLibrary.UINotJoinSocietyPanel);
                //        //然后打开加入公会面板
                //        Control.ShowGUI(GameLibrary.UIHaveJoinSocietyPanel);
                //    }

                //}
                break;
            case 2://退出公会
                SocietyManager.Single().isJoinSociety = false;
                SetMainHeroName.Instance.RefreshSocietyName();
                Globe.isHaveSociety = false;
                //如果公会解散了，其他玩家也被推送退出公会协议 这时要判断在主城 并且界面在打开状态就关闭
                //如果是会长 也被推送这个协议 也直接关闭该界面就可以

                //玩家被踢出公会 也被推送该条协议 如果玩家在打开界面关闭掉

                //玩家自己退出公会也被推送该条协议 关闭掉打开界面就可以
                break;
            case 3://权限改变
                SocietyManager.Single().isJoinSociety = true;
                SetMainHeroName.Instance.RefreshSocietyName();
                Globe.isHaveSociety = true;
                //如果在主城 并且加入工会面板在打开状态 就调用一下showhandle界面
                //if (SceneManager.GetActiveScene().name == GameLibrary.UI_Major)
                //{
                //    if (Control.GetGUI(GameLibrary.UIHaveJoinSocietyPanel).gameObject.activeSelf)
                //    {
                //        Control.ShowGUI(GameLibrary.UIHaveJoinSocietyPanel);
                //    }
                //}
                break;
            case 4://创建公会
                //打开加入公会面板
                SocietyManager.Single().isJoinSociety = true;
                SetMainHeroName.Instance.RefreshSocietyName();
                Globe.isHaveSociety = true;
                //Control.ShowGUI(GameLibrary.UIHaveJoinSocietyPanel);
                Control.ShowGUI(UIPanleID.UIHaveJoinSocietyPanel, EnumOpenUIType.OpenNewCloseOld);
                break;
        }

        return true;
    }
}
