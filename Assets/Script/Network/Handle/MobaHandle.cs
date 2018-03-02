using UnityEngine;
using System;
using System.Collections.Generic;

public class CMobaHandle : CHandleBase
{
    public CMobaHandle ( CHandleMgr mgr) : base(mgr)
    {

    }

    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.pve_search_moba_list_ret, GetMobaMatch);
        RegistHandle( MessageID.pve_moba_settlement_ret, GetMobaResult);
        RegistHandle( MessageID.pve_draw_moba_reward_ret, GetFlopResult);
    }

    // {msgid=4652,ret=返回值,desc=返回描述,mh={},dh={}}
    // mh 我方出战英雄战斗属性 dh 对方出战英雄属性
    private bool GetMobaMatch ( CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        int ret = int.Parse(data["ret"].ToString());
        int type = int.Parse(data["types"].ToString());
        if (ret == 0)
        {
            if (type == 1)
            {
                Globe.mobaMyTeam = HeroData.GetHeroDatasFromDict((Dictionary<string, object>)data["mh1"], 4);
                Globe.mobaEnemyTeam = HeroData.GetHeroDatasFromDict((Dictionary<string, object>)data["dh1"], 4);
            }
            else if (type==2)
            {
                Globe.moba3v3MyTeam1 = HeroData.GetHeroDatasFromDict((Dictionary<string, object>)data["mh1"], 4);
                // 匹配数据改为读配置表
                // Globe.moba3v3MyTeam2 = HeroData.GetHeroDatasFromDict((Dictionary<string, object>)data["mh2"], 4);
                // Globe.moba3v3MyTeam3 = HeroData.GetHeroDatasFromDict((Dictionary<string, object>)data["mh3"], 4);
                // Globe.moba3v3EnemyTeam1 = HeroData.GetHeroDatasFromDict((Dictionary<string, object>)data["dh1"], 4);
                // Globe.moba3v3EnemyTeam2 = HeroData.GetHeroDatasFromDict((Dictionary<string, object>)data["dh2"], 4);
                // Globe.moba3v3EnemyTeam3 = HeroData.GetHeroDatasFromDict((Dictionary<string, object>)data["dh3"], 4);
            }
            //if (UIEmbattle.instance!=null)
            //{
            //    UIEmbattle.instance.MobaMatchedAndSwitch(UIEmbattle.sourceType);
            //}
        }
        else
        {
            Debug.Log(string.Format("获取世界列表错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    // {msgid=4658,ret=返回值,desc=返回描述,item={"1"={id=x,at=x,cs=x},"2"={id=x,at=x,cs=x}..}}
    // item 抽奖信息 共5个
    private bool GetMobaResult ( CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        int ret = int.Parse(data["ret"].ToString());
        if (ret == 0)
        {
            if(SceneUIManager.instance == null || SceneUIManager.instance.FlopCardPanel == null)
                return false;
            Control.HideGUI(UIPanleID.UITheBattlePanel);
            Dictionary<string, object> dict = (Dictionary<string, object>)data["item"];
            if (dict != null && dict.Count > 0)
                SceneUIManager.instance.FlopCardPanel.Show(dict, data["arenaCoin"].ToString());
            // Time.timeScale = 0;  
            return true;
        }
        else
        {
            Debug.Log(string.Format("进入副本请求错误：{0}", data["desc"].ToString()));
            return false;
        }
    }

    // {msgid=4610,ret=返回值,desc=返回描述}
    private bool GetFlopResult ( CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        int ret = int.Parse(data["ret"].ToString());
        if (ret == 0)
        {
            SceneUIManager.instance.FlopCardPanel.Flopped((int[])data["dn"]);
        }
        else
        {
            Debug.Log(string.Format("开始副本战斗错误：{0}", data["desc"].ToString()));
        }
        return true;
    }

    private bool GetFightSettlementResultHandle(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            if (!data.ContainsKey("item"))
                return true;

            int gold = 0;

            if (data.ContainsKey("gold"))
            {
                gold = int.Parse(data["gold"].ToString());
                GameLibrary.receiveGolds = gold;
            }

            object[] item = data["item"] as object[];

            Dictionary<long, int> receiveGoods = new Dictionary<long, int>();

            if (null != item)
            {
                for (int i = 0; i < item.Length; i++)
                {
                    int id = int.Parse((item[i] as Dictionary<string, object>)["id"].ToString());
                    int at = int.Parse((item[i] as Dictionary<string, object>)["at"].ToString());
                    receiveGoods.Add(id, at);
                }
            }
            
            GameLibrary.receiveGoods = receiveGoods;

        }
        else
        {
            Debug.Log(string.Format("副本战斗结算错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    private bool GetDrawDungeonBoxRewardResultHandle(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {

            Debug.Log("领取成功");

        }
        else
        {
            Debug.Log(string.Format("领取副本列表箱子奖励错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    private bool GetFlashDungeonFightResultHandle(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {

            object[] item = data["item"] as object[];

            if (null != item)
            {

            }

            //int gold = Convert.ToInt32(data["gold"]);
            //int diamond = Convert.ToInt32(data["diamond"]);

        }
        else
        {
            Debug.Log(string.Format("副本扫荡错误：{0}", data["desc"].ToString()));
        }

        return true;

    }
}
