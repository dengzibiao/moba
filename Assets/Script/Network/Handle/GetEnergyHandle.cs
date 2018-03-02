using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;
/// <summary>
/// 定时进餐
/// </summary>
public class CGetEnergyHandle : CHandleBase
{

    public CGetEnergyHandle(CHandleMgr mgr)
        : base(mgr)
    {
    }

    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.common_timeing_dining_ret, TimeDineHandle);
        RegistHandle(MessageID.common_draw_online_reward_ret, GetOnlineRewardResult);
        RegistHandle(MessageID.common_player_level_reward_ret, GetLevelRewardResult);
    }
    public bool TimeDineHandle(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            playerData.GetInstance().singnData.dinning = (data["tdining"].ToString());
            playerData.GetInstance().ActionPointHandler(ActionPointType.Energy, int.Parse(data["thew"].ToString()));
            //Control.ShowGUI(UIPanleID.UIWelfare);
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "获得60点体力");
            UIGetEnergyPanel._instance.OpenTiliEffect();
            return true;
        }
        else
        {
            Debug.Log("进餐失败");
            return false;
        }
        //UIGetEnergyPanel._instance.Refarece();  ///刷新打开
        
    }

    public bool GetOnlineRewardResult(CReadPacket packet)
    {
        Debug.Log("GetOnlineRewardResult领取在线奖励结果");
        Dictionary<string, object> data = packet.data;
        //{msgid=4864,ret=返回值0,desc=返回描述,onlineReward=已签到信息}
        playerData.GetInstance().getEnergyData.resolt = int.Parse(data["ret"].ToString());
        if (playerData.GetInstance().getEnergyData.resolt == 0)
        {
            //Debug.LogError(data["onlineReward"].ToString());
            playerData.GetInstance().singnData.getRewardTime = Auxiliary.GetNowTime();
            if (data.ContainsKey("onlineRewardTime"))
            {
                playerData.GetInstance().singnData.onLineRewardTime = long.Parse(data["onlineRewardTime"].ToString());
            }
            playerData.GetInstance().singnData.onlineReward = data["onlineReward"].ToString();
            if (playerData.GetInstance().singnData.onlineReward.Length >= 9)
            {
                playerData.GetInstance().singnData.onLineTime = int.Parse(playerData.GetInstance().singnData.onlineReward.Substring(6, 3));
                if (playerData.GetInstance().singnData.onLineTime >= 1)
                {
                    playerData.GetInstance().singnData.onLineTime -= 1;//上次累计的在线时长减一，确保后端可领取 优先于前端显示
                }
            }
            if (data.ContainsKey("gold"))
            {
                if (data["gold"] != null)
                {
                    UInt32 gold = UInt32.Parse(data["gold"].ToString());
                    playerData.GetInstance().RoleMoneyHadler(MoneyType.Gold, gold);
                }

            }
            if (data.ContainsKey("diamond"))
            {
                if (data["diamond"] != null)
                {
                    UInt32 diamond = UInt32.Parse(data["diamond"].ToString());
                    playerData.GetInstance().RoleMoneyHadler(MoneyType.Diamond, diamond);
                }

            }
            //UIOnlineGiftBag.Instance.AlreadyGetCount = int.Parse(playerData.GetInstance().singnData.onlineReward.Substring((playerData.GetInstance().singnData.onlineReward.Length - 1), 1));
            playerData.GetInstance().singnData.onlineAlreadyGetCount = int.Parse(playerData.GetInstance().singnData.onlineReward.Substring((playerData.GetInstance().singnData.onlineReward.Length - 1), 1));
            //UIOnlineGiftBag.Instance.IsRefresh = true;
            playerData.GetInstance().singnData.onlineIsRefresh = true;
            playerData.GetInstance().singnData.isCanGetOnlineReward = false;
            UIWelfare._instance.ShowRedTag();
            Control.ShowGUI(UIPanleID.UITaskRewardPanel, EnumOpenUIType.DefaultUIOrSecond);
            return true;
        }
        else
        {
            Debug.Log(data["desc"].ToString());
            return false;
        }
        
    }

    public bool GetLevelRewardResult(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;

        playerData.GetInstance().getEnergyData.resolt = int.Parse(data["ret"].ToString());
        if (playerData.GetInstance().getEnergyData.resolt == 0)
        {
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
            if (data.ContainsKey("gold"))
            {
                if (data["gold"] != null)
                {
                    UInt32 gold = UInt32.Parse(data["gold"].ToString());
                    playerData.GetInstance().RoleMoneyHadler(MoneyType.Gold, gold);
                }

            }
            if (data.ContainsKey("diamond"))
            {
                if (data["diamond"] != null)
                {
                    UInt32 diamond = UInt32.Parse(data["diamond"].ToString());
                    playerData.GetInstance().RoleMoneyHadler(MoneyType.Diamond, diamond);
                }

            }
            //Control.ShowGUI(UIPanleID.UIUpgradeGiftBag);
            UIWelfare._instance.ShowRedTag();
            Control.ShowGUI(UIPanleID.UITaskRewardPanel, EnumOpenUIType.DefaultUIOrSecond);
            return true;
        }
        else
        {
            Debug.Log(data["desc"].ToString());
            return false;
        }
        
    }
}
