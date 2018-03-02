using UnityEngine;
using System.Collections;

public class GoodsBorderIcon : GUISingleItemList
{

    public GUISingleButton icon;
    public Transform xuanZhongIcon;
    private RoleIconAttrNode item;
    public static GoodsBorderIcon _instance;
    protected override void InitItem()
    {
        _instance = this;
        icon = transform.Find("Icon").GetComponent<GUISingleButton>();
        xuanZhongIcon = transform.Find("XuanZhongIcon");
        if (index == 0)
        {
            transform.gameObject.SetActive(true);
        }
        icon.onClick = OnIconClick;
    }
    private void OnIconClick()
    {
        Globe.seletIndex = index;

         ChangeIconBorder._instance.roleIconBorder.spriteName = item.icon_name;
         UIRole.Instance.roleIconBorder.spriteName = item.icon_name;
         playerData.GetInstance().iconFrameData.iconFrame_id = item.icon_id;
         UIRoleInfo.Instance.roleIconBorder.spriteName = item.icon_name;
        // ClientSendDataMgr.GetSingle().GetRoleSend().SendIcon();


    }
    public override void Info(object obj)
    {

        if (null == obj)
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
        if (index == Globe.seletIndex)
        {

            xuanZhongIcon.gameObject.SetActive(true);

        }
        else
        {
            xuanZhongIcon.gameObject.SetActive(false);
        }
    }
}
