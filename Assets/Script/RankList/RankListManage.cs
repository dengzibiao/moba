using UnityEngine;
using System.Collections;

public class RankListManage
{
    public int count=0;
    private int _type;
    private static RankListManage instance;

    public static RankListManage GetInstace()
    {
        if(instance==null)
        {
            instance = new RankListManage();
        }
        return instance;
    }
    /// <summary>
    /// 请求各排行榜数据
    /// </summary>
    public void GetRankListData(int type) 
    {
        this._type = type;
       
        if (count >= 0&&count<=2)//0/1/2
        {
            //ClientSendDataMgr.GetSingle().GetRankListSend().SendRankList(this._type, 0, 5);//0-15/16-31/32-46
            count++;
            if (count == 2)//如果是决斗场排行榜，接收到第二次数据 就显示
            {
                //if (Control.GetGUI(GameLibrary.UIRankList).gameObject.activeSelf)
              //  {
                   // UIRankList._instance.RefrashData();
              //  }
            }
        }
        else if (count == 3)
        {
            count = 0;
          //  if (Control.GetGUI(GameLibrary.UIRankList).gameObject.activeSelf)
         //   {
                UIRankList._instance.OpenCheckBox();
           // }

        }
        //else if (count == 3)//最后一页余2
        //{
        //    ClientSendDataMgr.GetSingle().GetRankListSend().SendRankList(this._type, 45, 49);//48-49
        //    count++;
        //}
        //else if (count == 4)
        //{
        //    count = 0;
        //    if (type == 5||type ==6)
        //    {
        //        if (Control.GetGUI(GameLibrary.UIRankList).gameObject.activeSelf)
        //        {
        //            UIRankList._instance.RefrashData();
        //            UIRankList._instance.OpenCheckBox();
        //        }
        //        //Control.ShowGUI(GameLibrary.UIRankList);
        //    }
        //    if (Control.GetGUI(GameLibrary.UIRankList).gameObject.activeSelf)
        //    {
        //        UIRankList._instance.OpenCheckBox();
        //    }

        //}
    }
}
