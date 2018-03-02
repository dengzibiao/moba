using UnityEngine;
using System.Collections;
using System;

public class UISkillAndGoldHintPanel : GUIBase
{

    public UILabel titleLabel;
    public UILabel des;
    public UISprite mask;
    public GUISingleButton backBtn;
    public GUISingleButton buyBtn;
    private UILabel backbtnLabel;
    private UILabel buybtnLabel;
    private byte types;

    public static UISkillAndGoldHintPanel instance;

    private int count = 2;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UISkillAndGoldHintPanel;
    }

    protected override void Init()
    {
        instance = this;

        UIEventListener.Get(mask.gameObject).onClick += OnMaskClick;

        backbtnLabel = backBtn.transform.FindChild("Label").GetComponent<UILabel>();
        buybtnLabel = buyBtn.transform.FindChild("Label").GetComponent<UILabel>();
        backBtn.onClick = OnBackBtnClick;
        buyBtn.onClick = OnBuyBtnClick;
    }

    private void OnBuyBtnClick()
    {
        if (types == 1 || types == 2 || types == 3)
        {
            if (playerData.GetInstance().baginfo.diamond > 20)
            {
                //UI_HeroDetail.skillCount = 20;
                playerData.GetInstance().skillPoints = 20;
                playerData.GetInstance().MoneyHadler(MoneyType.Diamond, -20);
                count--;
            }
        }
        else if(types == 4)
        {

        }
        Control.HideGUI(UIPanleID.UIMask);
        Hide();
    }

    private void OnBackBtnClick()
    {
        Control.HideGUI(UIPanleID.UIMask);
        Hide();
    }

    void OnMaskClick(GameObject go)
    {
        Hide();
    }

    public void SetData(byte type)
    {
        // titleLabel = transform.Find("title/Label").GetComponent<UILabel>();
        // des = transform.Find("Des").GetComponent<UILabel>();
        // backBtn = transform.Find("BackBtn").GetComponent<GUISingleButton>();
        //  buyBtn = transform.Find("BuyBtn").GetComponent<GUISingleButton>();
        types = type;
        if (type == 1)
        {
            titleLabel.text = "技能点不足";
            des.text = "你当前技能点数不足。" + "\n" + "是否消耗" + "[2dd740]" + "20" +"[-]" + "钻石购买20点技能点?" + "\n" + "今日可购买次数："+ "[2dd740]"+count+"[-]";
        }
        else if (type == 2)
        {
            titleLabel.text = "金币不足";
            des.text = "你的金币不足。" + "/n" +"是否前往金币购买界面?"+ "今日可购买次数："+ "[2dd740]"+"2"+"[-]";
        }else if (type==3)
        {
            titleLabel.text = "升级失败";
            des.text = "技能等级不能大于英雄等级";
        }
        else if(type == 4)
        {
            titleLabel.text = "飞行器";
            des.text = "您要去对面吗，现在是试营业可以给票钱，等正式营业了，多多惠顾哈！！！";
            buybtnLabel.text = "去对面";
        }
    }
}
