using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tianyu;

public class CGoldHandHandle : CHandleBase
{
    public CGoldHandHandle(CHandleMgr mgr) : base(mgr)
    {
    }
    public override void RegistAllHandle()
    {
        //	RegistHandle( MessageID.Item_D2C_UpdateItemInfo, UpdateItemInfo );
        RegistHandle(MessageID.common_lucky_draw_count_ret, GetGoldHandTimesResult);
        RegistHandle(MessageID.common_use_lucky_draw_ret, UseGoldHandResult);
    }
   
    
    public bool GetGoldHandTimesResult(CReadPacket packet)
    {
        // UnityEngine.StartCoroutine(GetGoldHandTimesFun(packet));
        Debug.Log("GetGoldHandTimesResult");
        Dictionary<string, object> data = packet.data;
        int resolt = 1;
        if (data.ContainsKey("ret"))
        {
            resolt = int.Parse(data["ret"].ToString());
        }
        if (resolt == 0)
        {
            if (data.ContainsKey("max"))
                playerData.GetInstance().goldHand.maxcount = int.Parse(data["max"].ToString());
            //if (data.ContainsKey("cur"))
            //    playerData.GetInstance().goldHand.curcount = int.Parse(data["cur"].ToString());
            if (data.ContainsKey("bat"))
                playerData.GetInstance().goldHand.alreadyUseCount = int.Parse(data["bat"].ToString());
            if (data.ContainsKey("bal"))
                playerData.GetInstance().goldHand.id = int.Parse(data["bal"].ToString());
            if (data.ContainsKey("balt"))
                playerData.GetInstance().goldHand.time = int.Parse(data["balt"].ToString());
            if (playerData.GetInstance().goldHand.curcount < 0)
            {
                playerData.GetInstance().goldHand.curcount = 0;
            }
            //GameLibrary.isActiveSendPackahe = false;
        }
        else
        {
            //Debug.Log(string.Format("获取点金手次数失败：{0}", data["desc"].ToString()));
            Debug.Log("获取点金手次数失败");
        }
        return true;
    }

    public bool UseGoldHandResult(CReadPacket packet)
    {
        Debug.Log("UseGoldHandResult");
        Dictionary<string, object> data = packet.data;
        object[] infoList = data["info"] as object[];
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            //playerData.GetInstance().goldHand.curcount = int.Parse(data["curtimes"].ToString());
            //if (playerData.GetInstance().goldHand.curcount < 0)
            //{
            //    playerData.GetInstance().goldHand.curcount = 0;
            //}
            if (data.ContainsKey("max"))
                playerData.GetInstance().goldHand.maxcount = int.Parse(data["max"].ToString());
            if (data.ContainsKey("bat"))
                playerData.GetInstance().goldHand.alreadyUseCount = int.Parse(data["bat"].ToString());
            if (data.ContainsKey("bal"))
                playerData.GetInstance().goldHand.id = int.Parse(data["bal"].ToString());
            if (data.ContainsKey("balt"))
                playerData.GetInstance().goldHand.time = int.Parse(data["balt"].ToString());
            playerData.GetInstance().RoleMoneyHadler(MoneyType.Gold, UInt32.Parse(data["goldsum"].ToString()));
            playerData.GetInstance().RoleMoneyHadler(MoneyType.Diamond, UInt32.Parse(data["diamondsum"].ToString()));
            UIGoldHand.Instance.lastDataCount = playerData.GetInstance().goldHand.goldHandList.Count;
            for (int i = 0;i<infoList.Length;i++)
            {
                Dictionary<string, object> infoDataDic = infoList[i] as Dictionary<string, object>;
                GoldHandItemData itemdata = new GoldHandItemData();
                itemdata.goldCount = int.Parse(infoDataDic["golds"].ToString());
                itemdata.jewelCount = int.Parse(infoDataDic["diamonds"].ToString());
                itemdata.critCount = int.Parse(infoDataDic["times"].ToString());
                //1倍就是它自己，2倍比1倍多1倍。
                if (itemdata.critCount > 1)
                {
                    itemdata.isCrit = true;
                }
                else
                {
                    itemdata.isCrit = false;
                }
                playerData.GetInstance().goldHand.goldHandList.Add(itemdata);
            }
        }
        else
        {
            Debug.Log(string.Format("点金手失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            //Control.ShowGUI(GameLibrary.UIPromptBox); 
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
        }
        return true;
    }
}
