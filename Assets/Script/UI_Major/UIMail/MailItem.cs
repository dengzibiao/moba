using UnityEngine;
using System.Collections;
using System;

public class MailItem : GUISingleItemList
{
    public GUISingleButton icon;
    public UISprite iconSprite;
    public UISprite mailStateIcon;
    public UISprite importanceIcon;
    public UISprite accessoryIcon;
    public UILabel deleteDate;
    public UILabel mailTitle;
    private MailItemData mailItem;
    private static int residueDay = 0;
    protected override void InitItem()
    {
        base.InitItem();
        icon = transform.Find("Icon").GetComponent<GUISingleButton>();
        iconSprite = transform.Find("Icon").GetComponent<UISprite>();
        mailStateIcon = transform.Find("MailStateIcon").GetComponent<UISprite>();
        importanceIcon = transform.Find("Importance").GetComponent<UISprite>();
        accessoryIcon = transform.Find("Accessory").GetComponent<UISprite>();
        deleteDate = transform.Find("DeleteDate").GetComponent<UILabel>();
        mailTitle = transform.Find("MailTitle").GetComponent<UILabel>();

        icon.onClick = OnIconClick;
    }

    private void OnIconClick()
    {
        Globe.selectMailIndex = index;
        Globe.selectMailId = mailItem.Id;
        MailDetail.Instance.SetData(mailItem,(int)mailItem.Id);
        MailDetail.Instance.setMailItemObj(this.gameObject);
        if (mailItem.NewMailFlag == 1)
        {
            Globe.readMailIdList.Add(mailItem.Id);
        }
        SetMialItemState();
    }
    /// <summary>
    /// 点击邮箱 设置邮箱为已读状态
    /// </summary>
    private void SetMialItemState()
    {
        if (mailItem.NewMailFlag == 0)
        {
            return;
        }
        mailStateIcon.spriteName = "yidu";
        for ( int i = 0;i< playerData.GetInstance().mailData.mailItemList.Count;i++)
        {
            if (mailItem.Id == playerData.GetInstance().mailData.mailItemList[i].Id)
            {
                playerData.GetInstance().mailData.mailItemList[i].NewMailFlag = 0;
                break;
            }
        }
    }
    public override void Info(object obj)
    {
        base.Info(obj);
        if (index == Globe.selectMailIndex)
        {
            MailDetail.Instance.setMailItemObj(this.gameObject);
        }
        mailItem = obj as MailItemData;
        mailTitle.text = mailItem.Title;
        //是否有附件
        if (mailItem.accessoryDataList.Count > 0&&mailItem.IsHaveGetGoods == 1)
        {
            accessoryIcon.gameObject.SetActive(true);
        }
        else
        {
            accessoryIcon.gameObject.SetActive(false);
        }
        //是否是重要邮件TODO

        //是否已读 1 新邮件，0已读邮件
        if (mailItem.NewMailFlag == 1)
        {
            mailStateIcon.spriteName = "weidu";
        }
        else
        {
            mailStateIcon.spriteName = "yidu";
        }

        residueDay = (PropertyManager.ConvertIntDateTime(mailItem.EndTime)-PropertyManager.ConvertIntDateTime(Auxiliary.GetNowTime())).Days;
        if (residueDay <=0)
        {
            deleteDate.text = "1天后过期";
        }
        else
        {
            deleteDate.text = residueDay + "天后过期";
        }
       
    }
    void Update()
    {
        if (index == Globe.selectMailIndex)
        {
            iconSprite.spriteName = "banzixuanzhong";
        }
        else
        {
            iconSprite.spriteName = "";
        }
        if (index == 0)
        {
            mailStateIcon.spriteName = "yidu";
        }
        if (mailItem.EndTime > Auxiliary.GetNowTime())
        {
            TimeSpan residueTime = PropertyManager.ConvertIntDateTime(mailItem.EndTime) - PropertyManager.ConvertIntDateTime(Auxiliary.GetNowTime());
            if (residueDay > residueTime.Days)
            {
                residueDay = residueTime.Days;
                if (residueDay <= 0)
                {
                    deleteDate.text = "1天后过期";
                }
                else
                {
                    deleteDate.text = residueDay + "天后过期";
                }
            }
        }
        else//邮件结束时间大于当前系统时间删除本地邮件 服务器处理方式是：再次去获取全部邮件的时候才触发删除到期邮件
        {
            playerData.GetInstance().RemoveSingleMailItem(mailItem.Id);
        }
        
    }
}
