using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIMailPanel : GUIBase {

    private static UIMailPanel instance;

    public GUISingleButton backBtn;
    public GUISingleMultList mailMultList;
    public object[] objs;
    public GameObject mailDetailObj;
    public UILabel promptLabel;
    private TweenPosition tweenP;
    private TweenAlpha tweenA;

    private UILabel notMail;
     private List<MailItemData> mailItemList = new List<MailItemData>();
    List<object> allList = new List<object>();

    public static UIMailPanel Instance { get { return instance; } set { instance = value; } }
    public UIMailPanel()
    {
        instance = this;
    }
    protected override void Init()
    {
        base.Init();
        instance = this;
        mailMultList = transform.Find("MailScrollView/MailMultList").GetComponent<GUISingleMultList>();
        mailDetailObj = transform.Find("MailDetail").gameObject;
        promptLabel = transform.Find("PromptLabel").GetComponent<UILabel>();
        notMail = transform.Find("NotMail").GetComponent<UILabel>();
        tweenA = promptLabel.transform.GetComponent<TweenAlpha>();
        tweenP = promptLabel.transform.GetComponent<TweenPosition>();
        backBtn.onClick = OnBackBtnClick;

    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIMailPanel;
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_player_mail_list_ret, UIPanleID.UIMailPanel);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_distill_mail_item_ret, UIPanleID.UIMailPanel);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_delete_mail_ret, UIPanleID.UIMailPanel);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_read_mail_ret, UIPanleID.UIMailPanel);
        Singleton<Notification>.Instance.Send(MessageID.common_player_mail_list_req, C2SMessageType.ActiveWait);
    }
    public override void ReceiveData(UInt32 messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            case MessageID.common_player_mail_list_ret://获取邮件列表
                Show();
                break;
            case MessageID.common_distill_mail_item_ret://领取邮件物品
                ShowPrompt("领取邮件物品");
                MailDetail.Instance.SetMailGoodsState();
                RefreshViewWhenGetGoods();
                break;
            case MessageID.common_delete_mail_ret://删除邮件
                InitMailItemData();//删除邮件成功后刷新界面
                ShowPrompt("删除邮件");
                break;
            case MessageID.common_read_mail_ret://获得新邮件
                InitMailItemData();
                break;
        }
    }
    private void OnBackBtnClick()
    {
        Control.HideGUI();
        //Hide();
        //每次打开邮箱 默认打开第一封邮件
        Globe.selectMailIndex = 0;
        //关闭的时候 发送批量修改邮件阅读状态协议
        if (Globe.readMailIdList.Count >0)
        {
            Dictionary<string, object> newpacket = new Dictionary<string, object>();
            newpacket.Add("arg1", Globe.readMailIdList);
            Singleton<Notification>.Instance.Send(MessageID.common_change_mail_newf_req, newpacket, C2SMessageType.ActiveWait);
            //ClientSendDataMgr.GetSingle().GetMailSend().SendBatchChangeMailState(Globe.readMailIdList,C2SMessageType.Active);
            //关闭界面的时候去修改邮箱未阅读数量。这样避免协议发送失败。数量跟显示不统一
            playerData.GetInstance().mailData.unReadMailCount -= Globe.readMailIdList.Count;
            if (SceneManager.GetActiveScene().name == GameLibrary.UI_Major)
            {
                playerData.GetInstance().NewMailHandler(playerData.GetInstance().mailData.unReadMailCount);
            }
        }
    }
    public void ShowPrompt(string s)
    {
        promptLabel.color = Color.green;
        promptLabel.text = s;
        tweenP.ResetToBeginning();
        tweenA.ResetToBeginning();
        tweenP.PlayForward();
        tweenA.PlayForward();
    }
    public void InitMailItemData()
    {
        mailItemList = playerData.GetInstance().mailData.mailItemList;
        objs = mailItemList.ToArray();

        //初始化邮箱item
        mailMultList.InSize(objs.Length, 1);
        mailMultList.Info(objs);
        mailMultList.transform.parent.GetComponent<UIScrollView>().ResetPosition();
        if (objs.Length > 0)
        {
            notMail.gameObject.SetActive(false);
            mailDetailObj.SetActive(true);
            MailDetail.Instance.SetData(mailItemList[0],(int)mailItemList[0].Id);
            Globe.selectMailIndex = 0;
            Globe.selectMailId = mailItemList[0].Id;
            //每次打开 默认打开第一封
            if (mailItemList[0].NewMailFlag == 1)
            {
                Globe.readMailIdList.Add(mailItemList[0].Id);
                SetMialItemState();
            }
        }
        else
        {
            mailDetailObj.SetActive(false);
            notMail.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 默认打开的一封邮件 状态设置
    /// </summary>
    private void SetMialItemState()
    {
        for (int i = 0; i < playerData.GetInstance().mailData.mailItemList.Count; i++)
        {
            if (mailItemList[0].Id == playerData.GetInstance().mailData.mailItemList[i].Id)
            {
                playerData.GetInstance().mailData.mailItemList[i].NewMailFlag = 0;
                break;
            }
        }
    }

    protected override void ShowHandler()
    {
        base.ShowHandler();
        //每次打开邮箱 清空一下 邮件阅读列表
        Globe.readMailIdList.Clear();
        InitMailItemData();
    }
    /// <summary>
    /// 当成功领取邮件物品后 刷新一下当前邮件信息
    /// </summary>
    public void RefreshViewWhenGetGoods()
    {
        mailItemList = playerData.GetInstance().mailData.mailItemList;
        objs = mailItemList.ToArray();
        //初始化邮箱item
        //mailMultList.InSize(objs.Length, 1);
        //mailMultList.Info(objs);
        //mailMultList.transform.parent.GetComponent<UIScrollView>().ResetPosition();
        //mailMultList.transform.parent.position = new Vector3(mailMultList.transform.parent.position.x, Globe.selectMailIndex*120, mailMultList.transform.parent.position.z);
        if (objs.Length > 0)
        {
            notMail.gameObject.SetActive(false);
            mailDetailObj.SetActive(true);
            MailDetail.Instance.SetData(mailItemList[Globe.selectMailIndex], (int)mailItemList[Globe.selectMailIndex].Id);
        }
        else
        {
            mailDetailObj.SetActive(false);
            notMail.gameObject.SetActive(true);
        }
    }
}
