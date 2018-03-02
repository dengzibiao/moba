/*
文件名（File Name）:   LotteryHandle.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-6-20 18:13:4
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;

public class CLotteryHandle : CHandleBase
{
    public CLotteryHandle(CHandleMgr mgr) : base(mgr)
    {
    }

    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.common_lucky_gamble_ret, CreateGoodsHandle);
    }
   
    private bool CreateGoodsHandle(CReadPacket packet)
    {
      
        GameLibrary.Instance().PackedCount++;

        if(GameLibrary.Instance().PackedCount>1)
        {
            return false;
        }
        Debug.Log("<color=#FFc937>CreateGoodsHandle抽奖数据</color>");
        //bug.Log(" <color=#FF4040>Lottery ResultGood</color>");
        Dictionary<string, object> data = packet.data;


        int result = int.Parse(data["ret"].ToString());
        if (result == 0)
        {
            Singleton<LotteryResultManager>.Instance.LotteryHandler(data);
        }
        else
        {   
            Debug.Log(string.Format("获取商店物品列表失败：{0}", data["desc"].ToString()));
            return false;
        }
        return true;
    }
}
