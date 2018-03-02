using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tianyu;
using UnityEngine.SceneManagement;

public class UISign_inHandle : CHandleBase
{
    public UISign_inHandle(CHandleMgr mgr) : base(mgr)
    {

    }
    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.common_everyday_sign_list_ret, GetUISign_inList);// 每日签到列表
        RegistHandle(MessageID.common_everyday_sign_ret, GetUISign_in);// 每日签到
        RegistHandle(MessageID.common_everyday_sign_reward_ret, GetUISign_inCumulative);// 每日签到累计奖励
        RegistHandle(MessageID.common_everyday_sign_again_ret, GetPatchUISign_in);// 每日签到补签 
    }

    private bool GetUISign_inList(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            playerData.GetInstance().singnData.Signed = (data["signed"].ToString());
            playerData.GetInstance().singnData.dinning = (data["tdining"].ToString());
            playerData.GetInstance().singnData.lvReward = (data["lvReward"].ToString());
            int[] lv = data["lvReward"] as int[];
            if (lv != null)
            {
                playerData.GetInstance().singnData.alreadylevelRewardDic.Clear();
                for (int i = 0; i < lv.Length; i++)
                {
                    playerData.GetInstance().singnData.alreadylevelRewardDic.Add(int.Parse(lv[i].ToString()), int.Parse(lv[i].ToString()));
                }
            }
            if (data.ContainsKey("onlineRewardTime"))
            {
                playerData.GetInstance().singnData.onLineRewardTime = long.Parse(data["onlineRewardTime"].ToString());
            }
            playerData.GetInstance().singnData.onlineReward = (data["onlineReward"].ToString());
            playerData.GetInstance().singnData.onLineTime = int.Parse(playerData.GetInstance().singnData.onlineReward.Substring(6, 3));
            if (playerData.GetInstance().singnData.onLineTime >= 1)
            {
                playerData.GetInstance().singnData.onLineTime -= 1;//上次累计的在线时长减一，确保后端可领取 优先于前端显示
            }
            //UIOnlineGiftBag.Instance.AlreadyGetCount = int.Parse(playerData.GetInstance().singnData.onlineReward.Substring((playerData.GetInstance().singnData.onlineReward.Length - 1), 1));
            playerData.GetInstance().singnData.onlineAlreadyGetCount = int.Parse(playerData.GetInstance().singnData.onlineReward.Substring((playerData.GetInstance().singnData.onlineReward.Length - 1), 1));
            playerData.GetInstance().singnData.logintime = long.Parse(data["loginTime"].ToString());
            if (playerData.GetInstance().singnData.logintime > playerData.GetInstance().singnData.getRewardTime)
            {
                playerData.GetInstance().singnData.getRewardTime = playerData.GetInstance().singnData.logintime;
            }
            playerData.GetInstance().newPlayerRewardList.timeList.Clear();
            if (data.ContainsKey("newbieReward"))
            {
                if (null != data["newbieReward"])
                {
                    int goodList = int.Parse(data["newbieReward"].ToString());
                    if (goodList != 0)
                    {
                        for (int i = 0; i < 15; i++)
                        {
                        
                            playerData.GetInstance().newPlayerRewardList.timeList.Add(goodList);
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
    private bool GetUISign_in(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            playerData.GetInstance().singnData.Signed = (data["signed"].ToString());
            playerData.GetInstance().baginfo.gold = UInt32.Parse((data["gold"].ToString()));
            playerData.GetInstance().baginfo.diamond = UInt32.Parse((data["diamond"].ToString()));
            playerData.GetInstance().RoleMoneyHadler(MoneyType.Gold, playerData.GetInstance().baginfo.gold);
            playerData.GetInstance().RoleMoneyHadler(MoneyType.Diamond, playerData.GetInstance().baginfo.diamond);
            UISign_in.Instance().GetHeroDebris();
            //Control.ShowGUI(UIPanleID.UIWelfare);
            Control.ShowGUI(UIPanleID.UITaskRewardPanel, EnumOpenUIType.DefaultUIOrSecond);
        }
        else
        {
            //UIgoodstips.Instance.Setgoods("goodsname", "decs", "imgname");
            //Control.ShowGUI(GameLibrary.UIgoodstips);

        }
        return true;
    }
    private bool GetUISign_inCumulative(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            playerData.GetInstance().singnData.Signed = (data["signed"].ToString());
            //Control.ShowGUI(UIPanleID.UIWelfare);
            GoodsDataOperation.GetInstance().AddGoods(UISign_inData.Instance().DoodsID, UISign_inData.Instance().DoodsNum);
        }
        else
        {
            UIgoodstips.Instances.Setgoods(UISign_inData.Instance().ItemNode, UISign_inData.Instance().ID);
            Control.Show(UIPanleID.UIgoodstips);
        }
        return true;
    }
    private bool GetPatchUISign_in(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            playerData.GetInstance().singnData.Signed = (data["signed"].ToString());
            //playerData.GetInstance().baginfo.gold = int.Parse((data["gold"].ToString()));
            playerData.GetInstance().baginfo.diamond = UInt32.Parse((data["diamond"].ToString()));
            playerData.GetInstance().RoleMoneyHadler(MoneyType.Gold, playerData.GetInstance().baginfo.gold);
            playerData.GetInstance().RoleMoneyHadler(MoneyType.Diamond, playerData.GetInstance().baginfo.diamond);
            UISign_in.Instance().GetHeroDebris();
            //Control.ShowGUI(UIPanleID.UIWelfare);    
            return true;
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            return false;
        }
    
    }
}
