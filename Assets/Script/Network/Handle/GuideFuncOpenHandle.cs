using UnityEngine;
using System.Collections.Generic;
using Tianyu;
using System;

public class CGuideFuncOpenHandle : CHandleBase
{

    public CGuideFuncOpenHandle(CHandleMgr mgr) : base(mgr)
    {

    }

    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.guide_function_opening_notify, GuideFunctionOpening);
    }

    public bool GuideFunctionOpening(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        int[] item = data["func"] as int[];
        if (null != item)
        {
            UnLockFunctionNode lockfunciton = null;
            List<int> chapterId = new List<int>();
            for (int i = 0; i < item.Length; i++)
            {
                lockfunciton = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().FindDataByType(item[i]);
                if (null == lockfunciton) continue;
                if (lockfunciton.chapter_id != 0)//解锁副本
                    chapterId.Add(lockfunciton.chapter_id);
                //设置相应的功能位开启
                FunctionOpenMng.GetInstance().SetValue(item[i]);
                //if(lockfunciton.id == 1|| lockfunciton.id == 2|| lockfunciton.id == 3|| lockfunciton.id ==4|| lockfunciton.id ==5|| lockfunciton.id ==6)
                //{
                //    UILevel.instance.openType = OpenLevelType.SysOpen;
                //    ClientSendDataMgr.GetSingle().GetBattleSend().SendQueryWorldMap();//获取世界副本
                //}
            }
            if (chapterId.Count > 0)
            {
                Dictionary<string, object> newpacket1 = new Dictionary<string, object>();
                newpacket1.Add("arg1", chapterId);
                newpacket1.Add("arg2", 1);
                Singleton<Notification>.Instance.Send(MessageID.pve_dungeon_list_req, newpacket1, C2SMessageType.ActiveWait);
                Dictionary<string, object> newpacket2 = new Dictionary<string, object>();
                newpacket2.Add("arg1", chapterId);
                newpacket2.Add("arg2", 2);
                Singleton<Notification>.Instance.Send(MessageID.pve_dungeon_list_req, newpacket2, C2SMessageType.ActiveWait);
                //ClientSendDataMgr.GetSingle().GetBattleSend().SendQueryDungeonList(chapterId, 1);
                //ClientSendDataMgr.GetSingle().GetBattleSend().SendQueryDungeonList(chapterId, 2);
            }
           
        }
        return true;
    }


}
