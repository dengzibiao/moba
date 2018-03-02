using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;
public class ChangeIconBorder : GUIBase {

    public GUISingleMultList goodsMultList;
    public GUISingleButton closeBtn;
    public GUISingleSprite roleIcon;
    public GUISingleSprite roleIconBorder;
    private RoleIconAttrNode item;
    public Transform view;
    public static ChangeIconBorder _instance;

    List<RoleIconAttrNode> keyList = new List<RoleIconAttrNode>();
    public object[] objs;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.ChangeIconBorder;
    }

    protected override void Init()
    {
        _instance = this;
        base.Init();
      
        closeBtn.onClick = OnCloseBtn;
        goodsMultList = transform.FindComponent<GUISingleMultList>("GoodsMultList");
        view = transform.Find("GoodsListScrollView");
        goodsMultList.ScrollView = view;
       
        
        //roleIcon.spriteName = UIRoleInfo.Instance.roleIcon.spriteName;
        //roleIconBorder.spriteName = UIRoleInfo.Instance.roleIconBorder.spriteName;
        if (FSDataNodeTable<RoleIconAttrNode>.GetSingleton().DataNodeList.ContainsKey(playerData.GetInstance().iconFrameData.iconFrame_id))
        {
            foreach(int key in FSDataNodeTable<RoleIconAttrNode>.GetSingleton().DataNodeList.Keys)
            {
                if(key>20000)
                {

                    RoleIconAttrNode a = FSDataNodeTable<RoleIconAttrNode>.GetSingleton().DataNodeList[key];
                    keyList.Add(a);
                }
            }
 
        }
    }
    protected override void ShowHandler()
    {
        roleIcon.spriteName = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[playerData.GetInstance().iconData.icon_id].icon_name + "_head";
        roleIconBorder.spriteName = UIRole.Instance.roleIconBorder.spriteName;
        CreatIconData();
       // roleIcon.spriteName = UIRoleInfo.Instance.roleIcon.spriteName;
       // roleIcon.spriteName = playerData.GetInstance().iconFrameData.iconFrame_name;
    }
    /// <summary>
    /// 生成头像框
    /// </summary>
    public void CreatIconData()
    {
        Debug.Log(objs);
        goodsMultList.InSize(keyList.ToArray().Length, 4);
        goodsMultList.Info(keyList.ToArray());
    }
    public void OnCloseBtn()
    {
        UIRole.Instance.OpenChangeIconBorderPanel(false);
        ClientSendDataMgr.GetSingle().GetRoleSend().SendIcon();
    }

}
