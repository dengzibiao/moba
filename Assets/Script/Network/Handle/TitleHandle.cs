using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;

public class CTitleHandle : CHandleBase
{
    public CTitleHandle(CHandleMgr mgr) : base(mgr)
    {
    }
    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.common_title_list_ret, GetTitleListResult);
        RegistHandle(MessageID.common_title_wear_or_takeoff_ret, ChangeTitleStateResult);
    }

    public bool GetTitleListResult(CReadPacket packet)
    {
        Debug.Log("GetTitleListResult");
        Dictionary<string, object> data = packet.data;
        //{msgid=306, misinfo=[{titleInfo=称号ID, deadLine=截至时间, isWear是否穿戴}, ...], ret=返回值0}
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            if (data.ContainsKey("titleInfo"))
            {
                object[] titleObj = data["titleInfo"] as object[];
                if (titleObj != null)
                {
                    playerData.GetInstance().selfData.playerTitleDic.Clear();
                    playerData.GetInstance().selfData.playerTitleId = 0;
                    playerData.GetInstance().selfData.playerTitleName = "";
                    for (int i = 0; i < titleObj.Length; i++)
                    {
                        Int32[] intAarry = titleObj[i] as Int32[];

                        Int64 time = (Int64)intAarry[1];
                        time = time << 32;
                        time = time | (UInt32)intAarry[2];
                        Debug.Log(i + "index____" + intAarry[0] + "___" + intAarry[1] + "___" + intAarry[2] + "___" + intAarry[3] + "___time" + time);
                        if (playerData.GetInstance().selfData.playerTitleDic.ContainsKey((int)intAarry[0]))
                        {
                            playerData.GetInstance().selfData.playerTitleDic[(int)intAarry[0]] = time.ToString();
                        }
                        else
                        {
                            playerData.GetInstance().selfData.playerTitleDic.Add((int)intAarry[0], time.ToString());
                        }
                        if (intAarry[3] == 1)//目前身上的称号
                        {
                            playerData.GetInstance().selfData.playerTitleId = (int)intAarry[0];
                            if (FSDataNodeTable<TitleNode>.GetSingleton().DataNodeList.ContainsKey((int)intAarry[0]))
                            {
                                playerData.GetInstance().selfData.playerTitleName = FSDataNodeTable<TitleNode>.GetSingleton().DataNodeList[(int)intAarry[0]].titlename;
                            }
                        }
                    }
                }
            }


            return true;
        }
        else
        {
            return false;
            Debug.Log("称号列表获取失败");
        }

       
    }
    public bool ChangeTitleStateResult(CReadPacket packet)
    {
        Debug.Log("ChangeTitleStateResult");
        Dictionary<string, object> data = packet.data;

        playerData.GetInstance().getEnergyData.resolt = int.Parse(data["ret"].ToString());
        
        if (playerData.GetInstance().getEnergyData.resolt == 0)
        {
            int titleID = int.Parse(data["titleId"].ToString()) ;
            int type = int.Parse(data["oprType"].ToString());
            //如果是穿戴
            if (type == 1)
            {
                //回复成功后
                UIPlayerTitlePanel.Instance.ShowPrompt("装备成功");
                if (playerData.GetInstance().selfData.playerTitleDic.ContainsKey(titleID))
                {
                    if (FSDataNodeTable<TitleNode>.GetSingleton().DataNodeList.ContainsKey(titleID))
                    {
                        TitleNode titleNode = FSDataNodeTable<TitleNode>.GetSingleton().DataNodeList[titleID];
                        playerData.GetInstance().selfData.playerTitleName = titleNode.titlename;
                        playerData.GetInstance().selfData.playerTitleId = titleNode.titleid;
                        UIPlayerTitlePanel.Instance.SetPlayerTitleName(playerData.GetInstance().selfData.playerTitleName);
                        UIPlayerTitlePanel.Instance.RefreshPanel();//刷新界面
                    }
                }

            }//如果是卸下
            else if (type == 2)
            {
                //回复成功后
                UIPlayerTitlePanel.Instance.ShowPrompt("卸下称号");
                playerData.GetInstance().selfData.playerTitleName = "";
                playerData.GetInstance().selfData.playerTitleId = 0;
                UIPlayerTitlePanel.Instance.SetPlayerTitleName("");
                UIPlayerTitlePanel.Instance.RefreshPanel();//刷新界面
            }
        }
        else
        {
            Debug.Log(data["desc"].ToString());
        }

        return true;
    }
}
