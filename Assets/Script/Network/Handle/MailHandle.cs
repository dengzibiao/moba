using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tianyu;
using UnityEngine.SceneManagement;

public class CMailHandle : CHandleBase
{
    public CMailHandle(CHandleMgr mgr) : base(mgr)
    {
    }

    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.common_player_mail_list_ret, AllMailListResult);
        RegistHandle(MessageID.common_read_mail_ret, SingleMailInfoResult);
        RegistHandle(MessageID.common_distill_mail_item_ret, SingleMailGoodsResult);
        RegistHandle(MessageID.common_delete_mail_ret, DeleteSingleMailResult);
        RegistHandle(MessageID.common_new_mail_count_ret, NewMailCountResult);
        RegistHandle(MessageID.common_have_new_mail_notify, HaveNewMailResulet);
        RegistHandle(MessageID.common_change_mail_newf_ret, BatchChangeMailStateResult);
    }
    /// <summary>
    /// 服务器回调：批量修改邮件已读状态
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    private bool BatchChangeMailStateResult(CReadPacket packet)
    {
        Debug.Log("BatchChangeMailStateResult");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            Debug.Log("批量修改邮件已读状态成功");
        }
        else
        {
            Debug.Log(string.Format("批量修改邮件已读状态失败：{0}", data["desc"].ToString()));
        }

        return true;
    }

    /// <summary>
    /// 服务器推送：新邮箱通知个数
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    private bool HaveNewMailResulet(CReadPacket packet)
    {
        Debug.Log("HaveNewMailResule");
        Dictionary<string, object> data = packet.data;
        int count = int.Parse(data["count"].ToString());
        long newMailID = long.Parse(data["mailId"].ToString());
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", newMailID);
        Singleton<Notification>.Instance.Send(MessageID.common_read_mail_req, newpacket, C2SMessageType.ActiveWait);
        //ClientSendDataMgr.GetSingle().GetMailSend().SendGetSingleMailInfo(newMailID,C2SMessageType.Active);
        //更新主界面邮箱格式提示  这个放到获取单个邮件信息成功后在更改，防止推送成功了，但是获取信息失败的bug
        //if (SceneManager.GetActiveScene().name == "UI_MajorCity" && Control.GetGUI(GameLibrary.UIMailPanel).gameObject.activeSelf)
        //{
        //    GameLibrary.isActiveSendPackahe = true;
        //    ClientSendDataMgr.GetSingle().GetMailSend().SendGetSingleMailInfo(newMailID);
        //    //更新主界面邮箱格式提示  这个放到获取单个邮件信息成功后在更改，防止推送成功了，但是获取信息失败的bug
        //    GameLibrary.isActiveSendPackahe = false;
        //}
        //else if(SceneManager.GetActiveScene().name == "UI_MajorCity")
        //{
        //    playerData.GetInstance().mailData.unReadMailCount = count;
        //    playerData.GetInstance().NewMailHandler(playerData.GetInstance().mailData.unReadMailCount);
        //}
        return true;
    }
    /// <summary>
    /// 服务器回调：登陆时候获取用户新邮箱个数
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    private bool NewMailCountResult(CReadPacket packet)
    {
        Debug.Log("NewMailCountResult");

        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            playerData.GetInstance().mailData.unReadMailCount = int.Parse(data["count"].ToString());
        }
        else
        {
            Debug.Log(string.Format("获取用户新邮箱个数失败：{0}", data["desc"].ToString()));
        }
        return true;
    }
    /// <summary>
    ///  服务器回调：删除单个邮箱
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    private bool DeleteSingleMailResult(CReadPacket packet)
    {
        Debug.Log("DeleteSingleMailResult");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            //更新邮箱列表显示
            playerData.GetInstance().RemoveSingleMailItem(Globe.selectMailId);
            //UIMailPanel.Instance.ShowPrompt("删除邮件");
        }
        else
        {
            Debug.Log(string.Format("删除单个邮箱失败：{0}", data["desc"].ToString()));
        }

        return true;
    }
    /// <summary>
    ///  服务器回调：获得单个邮箱附件中的物品
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    private bool SingleMailGoodsResult(CReadPacket packet)
    {
        Debug.Log("SingleMailGoodsResult");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            //重新获取一下物品列表 之后修改成直接增加到本地
            //ClientSendDataMgr.GetSingle().GetItemSend().SendGetBackPackList();
            //UIMailPanel.Instance.ShowPrompt("领取邮件物品");
            //MailDetail.Instance.SetMailGoodsState();
            //UIMailPanel.Instance.RefreshViewWhenGetGoods();
        }
        else
        {
            Debug.Log(string.Format("获取单个邮箱物品失败：{0}", data["desc"].ToString()));
        }
        return true;
    }

    /// <summary>
    /// 服务器回调：获得单个邮箱信息
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    private bool SingleMailInfoResult(CReadPacket packet)
    {
        Debug.Log("SingleMailInfoResult");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            //object[] mailItem = data["mail"] as object[];
            Dictionary<string, object> mailItem = data["mail"] as Dictionary<string, object>;
            if (mailItem != null)
            {
                MailItemData mailItemData = new MailItemData();
                mailItemData.Id = long.Parse(mailItem["idx"].ToString());
                mailItemData.NiceName = mailItem["sn"].ToString();
                mailItemData.Title = mailItem["tit"].ToString();
                mailItemData.Content = mailItem["c"].ToString();
                mailItemData.MainType = mailItem["mt"].ToString();
                mailItemData.Type = mailItem["st"].ToString();
                mailItemData.IsHaveGetGoods = int.Parse(mailItem["dit"].ToString());
                mailItemData.CreatTime = long.Parse(mailItem["ct"].ToString());
                mailItemData.EndTime = long.Parse(mailItem["ot"].ToString());
                mailItemData.NewMailFlag = int.Parse(mailItem["nf"].ToString());
                if (mailItemData.NewMailFlag == 1)
                {
                    playerData.GetInstance().mailData.unReadMailCount++;
                }
                int goldcount = int.Parse(mailItem["gold"].ToString());
                int diamondcount = int.Parse(mailItem["diamond"].ToString());
                mailItemData.accessoryDataList.Clear();//清空一下附件物品list
                if (goldcount > 0)
                {
                    MailAccessoryData accessoryData = new MailAccessoryData();
                    accessoryData.Type = MailGoodsType.GoldType;
                    accessoryData.Gold = goldcount;
                    accessoryData.Diamond = 0;
                    accessoryData.GradeType = GradeType.Green;
                    if (mailItemData.IsHaveGetGoods == 1)
                    {
                        accessoryData.IsHaveGet = false;
                    }
                    else
                    {
                        accessoryData.IsHaveGet = true;
                    }
                    accessoryData.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                    mailItemData.accessoryDataList.Add(accessoryData);
                }
                if (diamondcount > 0)
                {
                    MailAccessoryData accessoryData = new MailAccessoryData();
                    accessoryData.Type = MailGoodsType.DiamomdType;
                    accessoryData.Diamond = diamondcount;
                    accessoryData.Gold = 0;
                    accessoryData.GradeType = GradeType.Green;
                    if (mailItemData.IsHaveGetGoods == 1)
                    {
                        accessoryData.IsHaveGet = false;
                    }
                    else
                    {
                        accessoryData.IsHaveGet = true;
                    }
                    accessoryData.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                    mailItemData.accessoryDataList.Add(accessoryData);
                }

                object[] itemList = mailItem["item"] as object[];
                if (itemList != null)
                {
                    for (int j = 0; j < itemList.Length; j++)
                    {
                        Dictionary<string, object> itemDataDic = itemList[j] as Dictionary<string, object>;
                        MailAccessoryData accessoryData = new MailAccessoryData();
                        accessoryData.Type = MailGoodsType.ItemType;
                        accessoryData.Id = int.Parse(itemDataDic["id"].ToString());
                        accessoryData.Count = long.Parse(itemDataDic["at"].ToString());
                        if (mailItemData.IsHaveGetGoods == 1)
                        {
                            accessoryData.IsHaveGet = false;
                        }
                        else
                        {
                            accessoryData.IsHaveGet = true;
                        }
                        switch (GameLibrary.Instance().ItemStateList[accessoryData.Id].grade) //(VOManager.Instance().GetCSV<ItemCSV>("Item").GetVO(accessoryData.Id).grade)
                        {
                            case 1:
                                accessoryData.GradeType = GradeType.Gray;
                                break;
                            case 2:
                                accessoryData.GradeType = GradeType.Green;
                                break;
                            case 4:
                                accessoryData.GradeType = GradeType.Blue;
                                break;
                            case 7:
                                accessoryData.GradeType = GradeType.Purple;
                                break;
                            case 11:
                                accessoryData.GradeType = GradeType.Orange;
                                break;
                            case 16:
                                accessoryData.GradeType = GradeType.Red;
                                break;
                        }
                        ItemNodeState itemnodestate = null;
                        if (GameLibrary.Instance().ItemStateList.ContainsKey(accessoryData.Id))
                        {
                            itemnodestate = GameLibrary.Instance().ItemStateList[accessoryData.Id];
                            accessoryData.ItemType = itemnodestate.types;
                            if (itemnodestate.types == 6|| itemnodestate.types == 7)
                            {
                                accessoryData.UiAtlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
                            }
                            else
                            {
                                accessoryData.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                            }
                        }

                        mailItemData.accessoryDataList.Add(accessoryData);
                    }
                }
                playerData.GetInstance().mailData.mailItemList.Insert(0, mailItemData);
                playerData.GetInstance().mailData.currentCount++;
                playerData.GetInstance().mailData.maxCount++;//服务器第一次给的最大值和邮箱当前数量一样，所以暂时也++,
                //if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
                //{
                //    playerData.GetInstance().NewMailHandler(playerData.GetInstance().mailData.unReadMailCount);
                //    //Control.ShowGUI(GameLibrary.UIMail);
                //    //if (Control.GetGUI(GameLibrary.UIMailPanel).gameObject.activeSelf)
                //    //{
                //    //    UIMailPanel.Instance.InitMailItemData();
                //    //}
                //}
               
                //未打开邮箱界面 只增加新邮件信息和更新界面数量提示即可 
                //如果推送新邮件时 邮箱界面在打开界面，刷新一下邮箱内容
               
            }
            //playerData.GetInstance().mailData.unReadMailCount = 0;
            //playerData.GetInstance().mailData.startIndex = int.Parse(data["start"].ToString());
            //playerData.GetInstance().mailData.currentCount = int.Parse(data["count"].ToString());
            //playerData.GetInstance().mailData.maxCount = int.Parse(data["maxCount"].ToString());
            
        }
        else
        {
            Debug.Log(string.Format("获取单个邮箱信息失败：{0}", data["desc"].ToString()));
        }

        return true;
    }
    /// <summary>
    /// 服务器回调：获取所有邮箱信息列表
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>

    private bool AllMailListResult(CReadPacket packet)
    {
        //{msgid=4602,ret=返回值,desc=返回描述,start=开始邮件索引,cc=当前个数,mct=最大个数,mail=[{idx=邮件ID,sn=发信人昵称,tit=标题,c=内容,mt=主类型,st=类型,ct=创建时间,ot=结束时间,nf=新邮件标志,item={id=物品id,at=数量},gold=邮件附带金币,diamond=邮件附带钻石},{}..{}]}

        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            object[] mailItem = data["mail"] as object[];
            if (mailItem != null)
            {
                playerData.GetInstance().mailData.mailItemList.Clear();//清空一下邮件列表
                playerData.GetInstance().mailData.unReadMailCount = 0;
                playerData.GetInstance().mailData.startIndex = int.Parse(data["start"].ToString());
                playerData.GetInstance().mailData.currentCount = int.Parse(data["count"].ToString());
                playerData.GetInstance().mailData.maxCount = int.Parse(data["maxCount"].ToString());
                for (int i = 0; i < mailItem.Length; i++)
                {
                    Dictionary<string, object> mailItemDataDic = mailItem[i] as Dictionary<string, object>;
                    MailItemData mailItemData = new MailItemData();
                    mailItemData.Id = long.Parse(mailItemDataDic["idx"].ToString());
                    mailItemData.NiceName = mailItemDataDic["sn"].ToString();
                    mailItemData.Title = mailItemDataDic["tt"].ToString();
                    mailItemData.Content = mailItemDataDic["c"].ToString();
                    mailItemData.MainType = mailItemDataDic["mt"].ToString();
                    mailItemData.Type = mailItemDataDic["st"].ToString();
                    mailItemData.IsHaveGetGoods = int.Parse(mailItemDataDic["dit"].ToString());
                    mailItemData.CreatTime = long.Parse(mailItemDataDic["ct"].ToString());
                    mailItemData.EndTime = long.Parse(mailItemDataDic["ot"].ToString());
                    mailItemData.NewMailFlag = int.Parse(mailItemDataDic["nf"].ToString());
                    if (mailItemData.NewMailFlag == 1)
                    {
                        playerData.GetInstance().mailData.unReadMailCount++;
                    }
                    int goldcount = int.Parse(mailItemDataDic["gold"].ToString());
                    int diamondcount = int.Parse(mailItemDataDic["diamond"].ToString());
                    mailItemData.accessoryDataList.Clear();//清空一下附件物品list
                    if (goldcount > 0)
                    {
                        MailAccessoryData accessoryData = new MailAccessoryData();
                        accessoryData.Type = MailGoodsType.GoldType;
                        accessoryData.Gold = goldcount;
                        accessoryData.Diamond = 0;
                        accessoryData.GradeType = GradeType.Green;
                        if (mailItemData.IsHaveGetGoods == 1)
                        {
                            accessoryData.IsHaveGet = false;
                        }
                        else
                        {
                            accessoryData.IsHaveGet = true;
                        }
                        accessoryData.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                        mailItemData.accessoryDataList.Add(accessoryData);
                    }
                    if (diamondcount > 0)
                    {
                        MailAccessoryData accessoryData = new MailAccessoryData();
                        accessoryData.Type = MailGoodsType.DiamomdType;
                        accessoryData.Diamond = diamondcount;
                        accessoryData.Gold = 0;
                        accessoryData.GradeType = GradeType.Green;
                        if (mailItemData.IsHaveGetGoods == 1)
                        {
                            accessoryData.IsHaveGet = false;
                        }
                        else
                        {
                            accessoryData.IsHaveGet = true;
                        }
                        accessoryData.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                        mailItemData.accessoryDataList.Add(accessoryData);
                    }

                    object[] itemList = mailItemDataDic["item"] as object[];
                    if (itemList != null)
                    {
                        for (int j = 0; j < itemList.Length; j++)
                        {
                            Dictionary<string, object> itemDataDic = itemList[j] as Dictionary<string, object>;
                            MailAccessoryData accessoryData = new MailAccessoryData();
                            accessoryData.Type = MailGoodsType.ItemType;
                            accessoryData.Id = int.Parse(itemDataDic["id"].ToString());
                            accessoryData.Count = long.Parse(itemDataDic["at"].ToString());
                            if (mailItemData.IsHaveGetGoods == 1)
                            {
                                accessoryData.IsHaveGet = false;
                            }
                            else
                            {
                                accessoryData.IsHaveGet = true;
                            }
                            switch (GameLibrary.Instance().ItemStateList[accessoryData.Id].grade) //(VOManager.Instance().GetCSV<ItemCSV>("Item").GetVO(accessoryData.Id).grade)
                            {
                                case 1:
                                    accessoryData.GradeType = GradeType.Gray;
                                    break;
                                case 2:
                                    accessoryData.GradeType = GradeType.Green;
                                    break;
                                case 4:
                                    accessoryData.GradeType = GradeType.Blue;
                                    break;
                                case 7:
                                    accessoryData.GradeType = GradeType.Purple;
                                    break;
                                case 11:
                                    accessoryData.GradeType = GradeType.Orange;
                                    break;
                                case 16:
                                    accessoryData.GradeType = GradeType.Red;
                                    break;
                            }
                            ItemNodeState itemnodestate = null;
                            if (GameLibrary.Instance().ItemStateList.ContainsKey(accessoryData.Id))
                            {
                                itemnodestate = GameLibrary.Instance().ItemStateList[accessoryData.Id];
                                accessoryData.ItemType = itemnodestate.types;
                                if (itemnodestate.types == 6)
                                {
                                    accessoryData.UiAtlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
                                }
                                else
                                {
                                    accessoryData.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                                }
                            }

                            mailItemData.accessoryDataList.Add(accessoryData);
                        }
                    }
                    playerData.GetInstance().mailData.mailItemList.Add(mailItemData);

                }
                if (SceneManager.GetActiveScene().name == GameLibrary.UI_Major)
                {
                    //playerData.GetInstance().NewMailHandler(playerData.GetInstance().mailData.unReadMailCount);
                    //Control.ShowGUI(UIPanleID.UIMail);
                    //Control.ShowGUI(GameLibrary.UIMailPanel);
                }
            }
        }
        else
        {
            Debug.Log(string.Format("获取邮箱列表失败：{0}", data["desc"].ToString()));
        }
        return true;
    }
}
