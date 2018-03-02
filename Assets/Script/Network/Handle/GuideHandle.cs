using UnityEngine;
using System.Collections.Generic;
using System;
using Tianyu;

public class CGuideHandle : CHandleBase {

    public CGuideHandle(CHandleMgr mgr)
        :base(mgr)
    {

    }

    public override void RegistAllHandle()
    {
        //RegistHandle(MessageID.c_player_guide_info_ret, GuideStepFinish);

        //新手指引
        RegistHandle(MessageID.c_player_guide_info_ret, GuidInfoRet);
        RegistHandle(MessageID.c_player_manipulate_specified_UI, OpenUI);
        RegistHandle(MessageID.c_player_buffer_specified_part, GuidBuffer);
    }
    /// <summary>
    /// 1024引导信息变更通知
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    private bool GuidBuffer(CReadPacket packet)
    {
        //Debug.Log("GuidBuffer");
        Dictionary<string, object> data = packet.data;
        
        Int32[][] bufferdata = data["bd"] as Int32[][];
        if (bufferdata != null)
        {
            for (int i = 0; i < bufferdata.Length; i++)
            {
                
                playerData.GetInstance().selfData.infodata[bufferdata[i][0]] = (uint)bufferdata[i][1];
                //Debug.LogError(bufferdata[i][0]);
                //Debug.LogError(playerData.GetInstance().selfData.infodata[bufferdata[i][0]]);
                //switch (bufferdata[i][0])
                //{
                //    case 833:
                //        //Debug.LogError(833);
                //        //playerData.GetInstance().guideData.state = bufferdata[i][1];
                //        break;
                //    case 832:
                //        //Debug.LogError(832);
                //        break;
                //    case 830:
                //        //Debug.LogError(830);
                //        //playerData.GetInstance().guideData.scripId = bufferdata[i][1];
                //        break;
                //    case 827:
                //        //Debug.LogError(827);

                //        break;
                //    case 831:
                //        //Debug.LogError(831);
                //        //playerData.GetInstance().guideData.typeId = GuideManager.Single().GetBitValue((uint)bufferdata[i][1], 0, 16);
                //        //playerData.GetInstance().guideData.stepId = GuideManager.Single().GetBitValue((uint)bufferdata[i][1], 17, 16);
                //        //playerData.GetInstance().guideData.guideId = playerData.GetInstance().guideData.stepId;
                //        break;
                //    default:
                //        break;
                //}
            }
        }

        //playerData.GetInstance().guideData.uId = Convert.ToInt32(data["uid"]);
        //playerData.GetInstance().selfData.infodata[830] = data[""]
        
        //playerData.GetInstance().guideData.typeId = GuideManager.Single().GetBitValue(playerData.GetInstance().selfData.infodata[831], 0, 16);
        
        
        //if (NextGuidePanel.Single() != null)
        //    NextGuidePanel.Single().Init();


        //if(UIGuidePanel.Single()!=null)
        //    UIGuidePanel.Single().InitGuide();

        return true;
    }
    /// <summary>
    /// 引导打开相应UI
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    private bool OpenUI(CReadPacket packet)
    {
        //Debug.Log("OpenUi");
        //Dictionary<string, object> data = packet.data;
        //byte ad = packet.ReadByte("ad");//窗口操作类型，0关闭，1打开
        //byte ui = packet.ReadByte("ui");//面板id


        //if (ad == 1)
        //{

        //    if (UIGuidePanel.Single() != null && UIGuidePanel.Single().NextGuidePanel != null)
        //    {
        //        UIGuidePanel.Single().NextGuidePanel.SetActive(true);
        //    NextGuidePanel.Single().GuideModel.transform.localPosition = new Vector3(-422, -332, 0);
        //    NextGuidePanel.Single().GuideModel.transform.localRotation = Quaternion.Euler(0, 180, 0);
        //    NextGuidePanel.Single().GuideDialogWin.transform.localPosition = new Vector3(-62, 128, 0);
        //    }

        //}
        //else
        //{
           

        //}
        //--"td" = typeId;
        //--"sp"= stepId;
        //--wd =widgetId


        return true;
        }
    /// <summary>
    /// 引导指引信息
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    private bool GuidInfoRet(CReadPacket packet)
    {
        //Debug.Log("GuidInfoRet");
        //Debug.Log("<color=#10DF11>GuidInfoRet scripId:::</color>" + (int)packet.GetInt("sd"));
        //Debug.Log("<color=#10DF11>GuidInfoRet typeId:::</color>" + (int)packet.GetShort("td"));
        //Debug.Log("<color=#10DF11>GuidInfoRet stepId:::</color>" + (int)packet.GetInt("sp"));
        //Debug.Log("<color=#10DF11>GuidInfoRet uId:::</color>" + (int)packet.GetInt("wd"));
        Dictionary<string, object> data = packet.data;
        playerData.GetInstance().guideData.scripId= (int)packet.GetInt("sd");
        playerData.GetInstance().guideData.typeId = (int)packet.GetShort("td");
        playerData.GetInstance().guideData.stepId = (int)packet.GetInt("sp");
        playerData.GetInstance().guideData.uId = (int)packet.GetInt("wd");

        if (NextGuidePanel.Single() != null)
            NextGuidePanel.Single().Init();

        return true;
    }
    /// <summary>
    /// 某新手引导步骤完成通知
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    //private bool GuideStepFinish(CReadPacket packet)
    //{
    //    Debug.Log("GuideStepFinish");
    //    Dictionary<string, object> data = packet.data;
    //    playerData.GetInstance().guideData.GuideId = int.Parse(data["arg1"].ToString());
    //    //playerData.GetInstance().guideData.Skip=
    //    return true;
    //}

}
