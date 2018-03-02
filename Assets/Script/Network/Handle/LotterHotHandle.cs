/*
文件名（File Name）:   LotterHotHandle.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-7-5 14:15:43
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class CLotterHotHandle : CHandleBase
{
    public CLotterHotHandle(CHandleMgr mgr) : base(mgr)
    {
    }
    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.common_lucky_gamble_list_ret, ShowHotHandle);
    }

    private bool ShowHotHandle(CReadPacket packet)
    {
        Debug.Log("<color=#FFc937>Lottery HotResult抽奖每日热点</color>");
        Dictionary<string, object> data = packet.data;

        int result = int.Parse(data["ret"].ToString());
        if (result == 0)
        {
            if (data.ContainsKey("item"))
            {
                if (null!= data["item"])
                {
                    int[] goodList = data["item"] as int[];
                    playerData.GetInstance().lotteryInfo.diamondDrawCounnt = int.Parse(data["dd"].ToString());
                    playerData.GetInstance().lotteryInfo.goldDrawCount = int.Parse(data["gd"].ToString());
                    playerData.GetInstance().lotteryInfo.diamondTime = long.Parse(data["ddt"].ToString());
                    playerData.GetInstance().lotteryInfo.goldTime = long.Parse(data["gdt"].ToString());
                    for (int i = 0; i < goodList.Length; i++)
                    {
                        ItemData itemInfo = new ItemData();
                        itemInfo.Id = int.Parse(goodList[i].ToString());
                        ItemNodeState vo = GameLibrary.Instance().ItemStateList[itemInfo.Id];
                        itemInfo.Name = vo.name;
                        itemInfo.Describe = vo.describe;
                        itemInfo.IconName = vo.icon_name;
                        switch (vo.grade)
                        {
                            case 1:
                                itemInfo.GradeTYPE = GradeType.Gray;
                                break;
                            case 2:
                                itemInfo.GradeTYPE = GradeType.Green;
                                break;
                            case 4:
                                itemInfo.GradeTYPE = GradeType.Blue;
                                break;
                            case 7:
                                itemInfo.GradeTYPE = GradeType.Purple;
                                break;
                            case 11:
                                itemInfo.GradeTYPE = GradeType.Orange;
                                break;
                            case 16:
                                itemInfo.GradeTYPE = GradeType.Red;
                                break;
                        }
                       playerData.GetInstance().lotteryInfo.hotList.Add(itemInfo);
                    }
                    //Control.ShowGUI(GameLibrary.UI_Lottery);
                }
            }
           
         
        }
        return true;
    }
}
