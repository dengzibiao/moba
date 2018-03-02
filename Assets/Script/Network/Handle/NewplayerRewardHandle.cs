/*
文件名（File Name）:   NewplayerRewardHandle.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CNewplayerRewardHandle : CHandleBase
{

    public CNewplayerRewardHandle(CHandleMgr mgr) : base(mgr)
    {
    }

    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.common_newbie_reward_ret, HandleFunction);
    }

    private bool HandleFunction(CReadPacket packet)
    {
        Debug.Log("<color=#FFc937>NewplayerRewardHandle新手登录奖励</color>");
        Dictionary<string, object> data = packet.data;

        int result = int.Parse(data["ret"].ToString());
        if (result == 0)
        {
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
            Control.ShowGUI(UIPanleID.UITaskRewardPanel, EnumOpenUIType.DefaultUIOrSecond);
        }
        else
        {
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
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
        }

        return true;
    }
}
