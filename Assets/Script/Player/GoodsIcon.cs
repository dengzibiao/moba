using UnityEngine;
using System.Collections;
using System;
using Tianyu;
public class GoodsIcon : GUISingleItemList {

    public GUISingleButton icon;
    public Transform xuanZhongIcon;
    private RoleIconAttrNode item;
    public static GoodsIcon _instance;
    int iconLen = 30;//头像长度
    protected override void InitItem()
    {
        _instance = this;
        icon=transform.Find("Icon").GetComponent<GUISingleButton>();
        xuanZhongIcon = transform.Find("XuanZhongIcon");
        if(index==0)
        {
            transform.gameObject.SetActive(true);
        }
        icon.onClick = OnIconClick;
    }
    private void OnIconClick()
    {
        Globe.seletIndex = index; 
        if(item.icon_id<20000)
        {
            for (int i = 0; i <= iconLen; i++)
            {
                if (i <= 9) { 
                if (item.icon_name == "yx_00" + i)
                {
                    ChangeIcon._instance.roleIcon.spriteName = "yx_00" + i + "_head";
                    UIRole.Instance.roleIcon.spriteName = "yx_00" + i + "_head";
                    UIRoleInfo.Instance.roleIcon.spriteName = "yx_00" + i + "_head";
                }
                }
                else { 
                if (item.icon_name == "yx_0" + i)
                {
                    ChangeIcon._instance.roleIcon.spriteName = "yx_0" + i + "_head";
                    UIRole.Instance.roleIcon.spriteName = "yx_0" + i + "_head";
                    UIRoleInfo.Instance.roleIcon.spriteName = "yx_0" + i + "_head";

                }}
            }

            playerData.GetInstance().iconData.icon_id = item.icon_id;
           //ClientSendDataMgr.GetSingle().GetRoleSend().SendIcon();
        }
        
    }
    public override void Info(object obj)
    {
      
        if(null==obj)
        {
          
        }
        else
	    {
            item = (RoleIconAttrNode)obj;
            Debug.Log(item.icon_name);
            icon.spriteName = item.icon_name;
            
	    }

    }
    public void Update()
    {
        if(index==Globe.seletIndex)
        {
            
            xuanZhongIcon.gameObject.SetActive(true);
    
        }
        else
        {
            xuanZhongIcon.gameObject.SetActive(false);
        }
    }
}