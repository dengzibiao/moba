using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MailDetail : MonoBehaviour
{
    private static MailDetail instance;
    public UILabel mailType;//邮箱类型
    public UILabel contentLabel;//邮箱文字内容
    public UILabel mailTitle;//邮件标题
    public UILabel mailSenderNick;//邮件发送者昵称
    public UILabel mailSendDate;//邮件发送时间
    public UILabel mailCount;//邮件数量
    public GUISingleButton deleteMailBtn;//删除邮件按钮
    public GUISingleButton cDeleteMailBtn;
    public GUISingleButton getMailGoods;//领取邮件附件按钮

    public GameObject goodsScrollViewObj;//附件scrollview对象
    public GUISingleMultList goodsMultist;

    public UIScrollView characterScrollView;//文字内容scrollview
    public UIScrollView goodsScrollView;

    public object[] obj;
    private bool isCanGetGoods = false;

    private MailItemData mailItem;
    private GameObject deletePromptObj;//删除邮件弹出框
    private UILabel promptLabel;
    private GUISingleButton ensureBtn;//确认删除按钮
    private GUISingleButton cancelBtm;//取消删除按钮
    private GameObject mailItemObj;
    List<MailAccessoryData> accessoryDataList;
    public static MailDetail Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MailDetail();
            }
            return instance;
        }

    }
    void Awake()
    {
        instance = this;

        mailType = transform.Find("MailType").GetComponent<UILabel>();
        mailTitle = transform.Find("MailTitile").GetComponent<UILabel>();
        mailSenderNick = transform.Find("MailSenderNick").GetComponent<UILabel>();
        mailSendDate = transform.Find("MailSendDate").GetComponent<UILabel>();
        mailCount = transform.Find("MailCount").GetComponent<UILabel>();
        deleteMailBtn = transform.Find("DeleteMailBtn").GetComponent<GUISingleButton>();
        cDeleteMailBtn = transform.Find("CDeleteMailBtn").GetComponent<GUISingleButton>();
        getMailGoods = transform.Find("GetMailGoods").GetComponent<GUISingleButton>();
        goodsScrollViewObj = transform.Find("GoodsScrollView").gameObject;
        goodsScrollView = goodsScrollViewObj.GetComponent<UIScrollView>();
        goodsMultist = transform.Find("GoodsScrollView/goodsMultList").GetComponent<GUISingleMultList>();

        characterScrollView = transform.Find("CharacterScrollView").GetComponent<UIScrollView>();
        contentLabel = transform.Find("CharacterScrollView/MailContent").GetComponent<UILabel>();

        deletePromptObj = transform.Find("DeletePrompt").gameObject;
        ensureBtn = transform.Find("DeletePrompt/EnsureBtn").GetComponent<GUISingleButton>();
        cancelBtm = transform.Find("DeletePrompt/CancelBtn").GetComponent<GUISingleButton>();
        promptLabel = transform.Find("DeletePrompt/Label").GetComponent<UILabel>();
        deletePromptObj.SetActive(false);
        deleteMailBtn.onClick = OnDeleteMailClick;
        cDeleteMailBtn.onClick = OnDeleteMailClick;
        getMailGoods.onClick = OnGetMailGoodsClick;
        ensureBtn.onClick = OnEnSureDeleteMailClick;
        cancelBtm.onClick = OnCancelDeleteMailClick;
    }
    public void setMailItemObj(GameObject obj)
    {
        mailItemObj = obj;
    }
    private void OnCancelDeleteMailClick()
    {
        deletePromptObj.SetActive(false);
    }
    /// <summary>
    /// 二次确认删除邮件事件
    /// </summary>
    private void OnEnSureDeleteMailClick()
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", mailItem.Id);
        Singleton<Notification>.Instance.Send(MessageID.common_delete_mail_req, newpacket, C2SMessageType.ActiveWait);
        //ClientSendDataMgr.GetSingle().GetMailSend().SendDeleteSingleMail(mailItem.Id,C2SMessageType.ActiveWait);
        //deletePromptObj.SetActive(false);
    }

    private void OnGetMailGoodsClick()
    {
        accessoryDataList = mailItem.accessoryDataList;
        //先取得该邮件附件信息
        //for (int i = 0;i< playerData.GetInstance().mailData.mailItemList.Count;i++)
        //{
        //    if (Globe.selectMailId == playerData.GetInstance().mailData.mailItemList[i].Id)
        //    {
        //        accessoryDataList = playerData.GetInstance().mailData.mailItemList[i].accessoryDataList;
        //        break;
        //    }
        //}
        if (accessoryDataList.Count <= 0)
        {
            return;
        }
        if (mailItem.IsHaveGetGoods == 0)
        {
            return;
        }
        //for (int j = 0; j < accessoryDataList.Count; j++)
        //{

        //    if (accessoryDataList[j].Type == MailGoodsType.GoldType || accessoryDataList[j].Type == MailGoodsType.DiamomdType)
        //    {
        //        continue;
        //    }
        //    if (accessoryDataList[j].Type == MailGoodsType.ItemType)
        //    {
        //        //判断是否满足领取条件
        //        //如果背包中找到该物品 并且（附件物品数量 + 背包中该物品数量）大于 该物品的堆叠上限 不满足条件
        //        if (playerData.GetInstance().GetItemDatatByID(accessoryDataList[j].Id) != null)
        //        {
        //            if (accessoryDataList[j].Count + playerData.GetInstance().GetItemCountById(accessoryDataList[j].Id) > playerData.GetInstance().GetItemDatatByID(accessoryDataList[j].Id).Piles)
        //            {
        //                isCanGetGoods = false;
        //                break;
        //            }
        //        }
        //        else
        //        {
        //            ItemNodeState itemstate = null;
        //            if (GameLibrary.Instance().ItemStateList.ContainsKey(accessoryDataList[j].Id))
        //            {
        //                itemstate = GameLibrary.Instance().ItemStateList[accessoryDataList[j].Id];
        //                //背包中没有该物品 并且附件物品数量 大于 该物品的堆叠上线 不满足条件
        //                if (accessoryDataList[j].Count > itemstate.piles)
        //                {
        //                    isCanGetGoods = false;
        //                    break;
        //                }
        //            }

        //        }
        //    }
        //    isCanGetGoods = true;
        //}
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", mailItem.Id);
        Singleton<Notification>.Instance.Send(MessageID.common_distill_mail_item_req, newpacket,C2SMessageType.ActiveWait);
        //ClientSendDataMgr.GetSingle().GetMailSend().SendGetSingleMailGoods(mailItem.Id, C2SMessageType.ActiveWait);
        //if (isCanGetGoods)
        //{
            
        //    //现在领取邮件物品成功后 是去获取一下背包信息协议。之后直接增加本地 
        //}
        //else
        //{
        //    Control.ShowGUI(GameLibrary.UIPromptBox);
        //    UIPromptBox.Instance.ShowLabel("当前背包容量不足，请清理后重试");
        //}

    }
    /// <summary>
    /// 邮箱物品领取成功后 修改物品领取状态
    /// </summary>
    public void SetMailGoodsState()
    {
        //邮件物品领取成功后 将物品提示图标隐藏
        if (mailItemObj != null)
        {
            mailItemObj.transform.Find("Accessory").gameObject.SetActive(false);
        }
        for (int k = 0; k < playerData.GetInstance().mailData.mailItemList.Count; k++)
        {
            if (mailItem.Id == playerData.GetInstance().mailData.mailItemList[k].Id)
            {
                mailItem.IsHaveGetGoods = 0;
                playerData.GetInstance().mailData.mailItemList[k].IsHaveGetGoods = 0;
                accessoryDataList = playerData.GetInstance().mailData.mailItemList[k].accessoryDataList;
                getMailGoods.gameObject.SetActive(false);
                break;
            }
        }
        for (int m = 0; m < accessoryDataList.Count; m++)
        {
            accessoryDataList[m].IsHaveGet = true;
        }
        ////将领取的物品放入背包
        //for (int i = 0;i<accessoryDataList.Count;i++)
        //{
        //    if (accessoryDataList[i].Type == MailGoodsType.GoldType)
        //    {
        //        playerData.GetInstance().MoneyHadler(MoneyType.Gold, accessoryDataList[i].Gold);
        //    }
        //    else if (accessoryDataList[i].Type == MailGoodsType.DiamomdType)
        //    {
        //        playerData.GetInstance().MoneyHadler(MoneyType.Diamond, accessoryDataList[i].Diamond);
        //    }
        //    else if (accessoryDataList[i].Type == MailGoodsType.ItemType)
        //    {
        //        GoodsDataOperation.GetInstance().AddGoods(accessoryDataList[i].Id,accessoryDataList[i].Count);
        //    }
        //}
    }
    private void OnDeleteMailClick()
    {
        if (mailItem.IsHaveGetGoods == 1)
        {
            //deletePromptObj.SetActive(true);
            //promptLabel.text = "有附件还未领取，是否确认删除";
            object[] obj = new object[5] { "有附件还未领取，是否确认删除？", "", UIPopupType.EnSure, this.gameObject, "OnEnSureDeleteMailClick" };
            Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
            //Control.ShowGUI(GameLibrary.UIPromptBox);
            //UIPromptBox.Instance.ShowLabel("有附件还未领取，是否确认删除");
        }
        else if (mailItem.IsHaveGetGoods == 0)
        {
            Dictionary<string, object> newpacket = new Dictionary<string, object>();
            newpacket.Add("arg1", mailItem.Id);
            Singleton<Notification>.Instance.Send(MessageID.common_delete_mail_req, newpacket, C2SMessageType.ActiveWait);
            //ClientSendDataMgr.GetSingle().GetMailSend().SendDeleteSingleMail(mailItem.Id,C2SMessageType.ActiveWait);
        }
    }

    void Start()
    {
        //Debug.LogError("sss");
        //SetData(0);
    }
    /// <summary>
    /// 设置邮件的详细内容
    /// </summary>
    public void SetData(MailItemData mailItemData, int index)
    {
        mailItem = mailItemData;
        mailTitle.text = mailItem.Title;//邮箱标题

        if (mailItem.accessoryDataList.Count > 0)
        {
            obj = mailItem.accessoryDataList.ToArray();
            //初始化邮件中的附件内容
            goodsScrollView.ResetPosition();
            goodsMultist.ScrollView = goodsScrollViewObj.transform;
            goodsScrollViewObj.SetActive(true);
            goodsMultist.gameObject.SetActive(true);
            goodsMultist.InSize(obj.Length, 5);
            goodsMultist.Info(obj);
            getMailGoods.gameObject.SetActive(true);
            if (mailItem.IsHaveGetGoods == 1)
            {
                getMailGoods.gameObject.SetActive(true);
            }
            else if (mailItem.IsHaveGetGoods == 0)
            {
                getMailGoods.gameObject.SetActive(false);
            }
            SetBtnPosition();
        }
        else
        {
            goodsScrollView.ResetPosition();
            goodsScrollViewObj.SetActive(false);
            goodsMultist.gameObject.SetActive(false);
            getMailGoods.gameObject.SetActive(false);

        }
        //设置邮件文字内容
        characterScrollView.ResetPosition();
        contentLabel.text = mailItem.Content;

        //邮件数量
        mailCount.text = playerData.GetInstance().mailData.currentCount + "/" + playerData.GetInstance().mailData.maxCount;
        //邮件发送者昵称
        mailSenderNick.text = mailItem.NiceName;
        //邮件发送时间 
        mailSendDate.text = TimeManager.Instance.GetAllTimeClockText(mailItem.CreatTime);
    }
    private void SetBtnPosition()
    {
        if (mailItem.IsHaveGetGoods == 1)
        {
            deleteMailBtn.gameObject.SetActive(true);
            cDeleteMailBtn.gameObject.SetActive(false);
            //deleteMailBtn.transform.localPosition = new Vector3(-150,-253,0);
        }
        else if (mailItem.IsHaveGetGoods == 0)
        {
            deleteMailBtn.gameObject.SetActive(false);
            cDeleteMailBtn.gameObject.SetActive(true);
            //deleteMailBtn.transform.localPosition = new Vector3(-6, -253, 0);
        }
    }
}
