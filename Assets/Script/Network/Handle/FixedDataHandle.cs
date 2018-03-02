using UnityEngine;
using System.Collections.Generic;
using System;

public class CFixedDataHandle : CHandleBase
{

    public CFixedDataHandle(CHandleMgr mgr)
        :base(mgr)
    {

    }

    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.c_plsyer_Fixed_info_ret,FixedInfo);
    }
   
    private bool FixedInfo(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        int pos = packet.GetInt("sPos");
        int endpos = packet.GetInt("ePos");
        int[] info = data["info"] as int[];
        if (info !=null)
        {
            for (int i = 0; i < info.Length; i++)
            {
                playerData.GetInstance().selfData.infodata[pos + i] = uint.Parse(info[i].ToString());
            }            
        }
        else
        {
           System.Object[] infoarr = data["info"] as System.Object[];
            for (int i = 0; i < infoarr.Length; i++)
            {
                playerData.GetInstance().selfData.infodata[pos + i] = uint.Parse(infoarr[i].ToString());
            }
           // GuideManager.Single().InitData();
        }

       



        return true;
    }
}
