using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tianyu;

public class CActionPointHandle : CHandleBase
{
    public CActionPointHandle(CHandleMgr mgr) : base(mgr)
    {
    }

    public override void RegistAllHandle()
    {
        //	RegistHandle( MessageID.Item_D2C_UpdateItemInfo, UpdateItemInfo );
        RegistHandle(MessageID.common_buy_action_point_ret, BuyActionPointResult);
    }

    public bool BuyActionPointResult(CReadPacket packet)
    {
      //  Debug.Log("BuyActionPointResult");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            int actionPointType = int.Parse(data["types"].ToString());
            if (actionPointType == 1)
            {
                playerData.GetInstance().ActionPointHandler(ActionPointType.Energy, int.Parse(data["thew"].ToString()));
                playerData.GetInstance().actionData.energyBuyTimes++;
                playerData.GetInstance().actionData.energyBuyTimes = int.Parse(data["bat"].ToString());
                playerData.GetInstance().actionData.maxEnergyBuyTimes = int.Parse(data["max"].ToString());
            }
            else if (actionPointType == 2)
            {
                playerData.GetInstance().ActionPointHandler(ActionPointType.Vitality, int.Parse(data["vigour"].ToString()));
                playerData.GetInstance().actionData.vitalityBuyTimes++;
            }
        }
        else
        {
            // Debug.Log(string.Format("购买体力失败：{0}", data["desc"].ToString()));
            //Control.ShowGUI(GameLibrary.UIPromptBox);
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
        }
        return true;
    }
}
