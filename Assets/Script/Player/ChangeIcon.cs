using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;

 
public class ChangeIcon :GUIBase{

    public GUISingleMultList goodsMultList;
    public GUISingleButton closeBtn;
    public GUISingleSprite roleIcon;
    public GUISingleSprite roleIconBorder;

    public List<IconData> iconList = new List<IconData>();
    public Transform view;
    public static ChangeIcon _instance;
    List<RoleIconAttrNode> keyList = new List<RoleIconAttrNode>();
    public object[] objs;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.ChangeIcon;
    }

    protected override void Init()
    {
        _instance = this;
        base.Init();
        closeBtn.onClick = OnCloseBtn;
        goodsMultList = transform.FindComponent<GUISingleMultList>("GoodsMultList");
        view = transform.Find("GoodsListScrollView");
        goodsMultList.ScrollView = view;

           foreach (int key in FSDataNodeTable<RoleIconAttrNode>.GetSingleton().DataNodeList.Keys)
           {
               if (key < 10014||key==10030)
               {

                   RoleIconAttrNode a = FSDataNodeTable<RoleIconAttrNode>.GetSingleton().DataNodeList[key];
                   keyList.Add(a);
               }
           }

    }
    protected override void ShowHandler()
    {
        roleIcon.spriteName = UIRole.Instance.roleIcon.spriteName;
        roleIconBorder.spriteName = UIRole.Instance.roleIconBorder.spriteName;
      
        CreatIconData();
       
    }
    /// <summary>
    /// 生成头像
    /// </summary>
    public void CreatIconData()
    {
        goodsMultList.InSize((keyList.ToArray()).Length, 5);
        goodsMultList.Info(keyList.ToArray());
    }

    public void OnCloseBtn()
    {
        UIRole.Instance.OpenChangeIconPanel(false);
        ClientSendDataMgr.GetSingle().GetRoleSend().SendIcon();
    }
}
