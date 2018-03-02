using UnityEngine;
using System.Collections;
using System;

public class PlayerTitleItem : GUISingleItemList
{

    public UILabel titleName;//称号图标
    public UILabel pathWay;//获得路径
    public UILabel addAttribute;
    public UILabel usefullLife;//有效期
    public GUISingleButton putonBtn;
    public GUISingleButton getoffBtn;
    public GUISingleButton icon;
    public Transform noNactivetedT;//未激活标志
    private TitleNode titleNode;
    private Transform onPlayerBody;//当前玩家已经佩戴的称号标志
    private Transform xuanzhong;//知否选中
    protected override void InitItem()
    {
        base.InitItem();
        titleName = transform.Find("TitleName").GetComponent<UILabel>();
        pathWay = transform.Find("Pathway").GetComponent<UILabel>();
        usefullLife = transform.Find("Usefullife").GetComponent<UILabel>();
        putonBtn = transform.Find("PutonBtn").GetComponent<GUISingleButton>();
        getoffBtn = transform.Find("GetoffBtn").GetComponent<GUISingleButton>();
        icon = transform.Find("Icon").GetComponent<GUISingleButton>();
        noNactivetedT = transform.Find("Nonactivated");
        onPlayerBody = transform.Find("OnPlayerBody");
        xuanzhong = transform.Find("Xuanzhong");
        icon.onClick = OnIconClick;
        putonBtn.onClick = OnPutOnClick;
        getoffBtn.onClick = OnGetOffClick;
    }

    private void OnIconClick()
    {
        Globe.selectTitleIndex = index;
        UIPlayerTitlePanel.Instance.endPlayerTitleID = titleNode.titleid;
        //点击称号只做展示用
        UIPlayerTitlePanel.Instance.SetPlayerTitleName(titleNode.titlename);
        UIPlayerTitlePanel.Instance.SetTitleAttribute(titleNode.titleid);
        //Globe.selectTitleId = ;
    }

    public override void Info(object obj)
    {
        base.Info(obj);
        titleNode = (TitleNode)obj;
        pathWay.text = titleNode.des;
        //先判断是否已经激活
        if (playerData.GetInstance().selfData.playerTitleDic.ContainsKey(titleNode.titleid))
        {
            noNactivetedT.gameObject.SetActive(false);
            usefullLife.gameObject.SetActive(true);//激活状态才有 有效期
            usefullLife.text = SubstringTime(playerData.GetInstance().selfData.playerTitleDic[titleNode.titleid]);
            titleName.text = titleNode.titlename;
            //icon.GetComponent<UISprite>().color = new Color(1, 1, 1);
            titleName.GetComponent<UILabel>().color = new Color(1, 1, 1f);
            //然后在判断是否当前携带的
            if (titleNode.titleid == playerData.GetInstance().selfData.playerTitleId)
            {
                onPlayerBody.gameObject.SetActive(true);//显示对号
                putonBtn.gameObject.SetActive(false);//隐藏穿上按钮
                getoffBtn.gameObject.SetActive(true);//显示脱下按钮
            }
            else
            {
                onPlayerBody.gameObject.SetActive(false);//隐藏对号
                putonBtn.gameObject.SetActive(true);//显示穿上按钮
                getoffBtn.gameObject.SetActive(false);//隐藏脱下按钮
            }
        }
        else
        {
            //未激活称号
            noNactivetedT.gameObject.SetActive(true);
            usefullLife.gameObject.SetActive(false);
            titleName.text = titleNode.titlename;//未激活的称号图标为灰色
            titleName.GetComponent<UILabel>().color = new Color(0.5f, 0.5f, 0.5f);
            onPlayerBody.gameObject.SetActive(false);
            putonBtn.gameObject.SetActive(false);
            putonBtn.gameObject.SetActive(false);
        }
       
        
    }
    /// <summary>
    /// 截取有效期字符串
    /// </summary>
    /// <param name="str">有效期字符串</param>
    /// <returns></returns>
    private string SubstringTime(string str)
    {
        if (IsNum(str))
        {
            if (str.Length >= 10)
            {
                return "有效期至：" + str.Substring(0, 2) + "年" + str.Substring(2, 2) + "月" + str.Substring(4, 2) + "日" + str.Substring(8, 2) + "分";
            }
        }
        else
        {
            return "永久";
        }
        return "";
        
    }
    /// <summary>
    /// 判断字符串是否是纯数字
    /// </summary>
    /// <param name="str">字符串</param>
    /// <returns></returns>
    public  bool IsNum(string str)
    {
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] < '0' || str[i] > '9')
                return false;
        }
        return true;
    }
    /// <summary>
    /// 卸下称号
    /// </summary>
    private void OnGetOffClick()
    {
        ClientSendDataMgr.GetSingle().GetTitleSend().SendChangeTitleState(C2SMessageType.ActiveWait, titleNode.titleid, 2);
    }

    /// <summary>
    /// 装备称号
    /// </summary>
    private void OnPutOnClick()
    {
        ClientSendDataMgr.GetSingle().GetTitleSend().SendChangeTitleState(C2SMessageType.ActiveWait,titleNode.titleid,1);
    }

    void Update()
    {
        if (index == Globe.selectTitleIndex)
        {
            xuanzhong.gameObject.SetActive(true);
        }
        else
        {
            xuanzhong.gameObject.SetActive(false);
        }

    }
}
