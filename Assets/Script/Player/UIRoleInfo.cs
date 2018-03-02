using UnityEngine;
using System.Collections;
using System;
using Tianyu;
using System.Collections.Generic;
public class UIRoleInfo : GUIBase
{
    public GUISingleButton closeBtn;

    public GUISingleSprite roleIcon;
    public GUISingleLabel accountIdLab;
    public GUISingleLabel iconLevelLab;
    public GUISingleLabel nameLab;
    public GUISingleLabel levelLab;
    public GUISingleLabel expLab;
    public GUISingleLabel fightLab;
    public GUISingleLabel heroLevelLab;
    public GUISingleButton changeNameBtn;
   // public GUISingleButton changeIconBtn;
    public GUISingleButton changeIconBorderBtn;
    public GUISingleSprite roleIconBorder;
    public GUISingleButton changeTitleNameBtn;

    private RoleIconAttrNode item;
    public  static UIRoleInfo instance;

    public int fcSum = 0;
    public int isVisible = 0;
    public static UIRoleInfo Instance { get { return instance; } set { instance = value; } }
   
    public UIRoleInfo()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIRoleInfo;
    }

    protected override void Init()
    {
        base.Init();        
        closeBtn.onClick = OnCloseBtn;
        changeNameBtn.onClick = OnChangeNameBtn;
       // changeIconBtn.onClick = OnChangeIconBtn;
        changeIconBorderBtn.onClick = OnChangeIconBorderBtn;
        changeTitleNameBtn.onClick = OnChangeTitleNameClick;
   
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }
    protected override void ShowHandler()
    {
        if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList != null && this != null && FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(playerData.GetInstance().iconData.icon_id))
        {
            roleIcon.spriteName = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[playerData.GetInstance().iconData.icon_id].icon_name + "_head";
        }
        if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList != null && this != null && FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(playerData.GetInstance().iconFrameData.iconFrame_id))
        {
            roleIconBorder.spriteName = FSDataNodeTable<RoleIconAttrNode>.GetSingleton().DataNodeList[playerData.GetInstance().iconFrameData.iconFrame_id].icon_name;
        }
        accountIdLab.text = playerData.GetInstance().selfData.playerId.ToString();
        nameLab.text = playerData.GetInstance().selfData.playeName.ToString();
        levelLab.text = playerData.GetInstance().selfData.level.ToString();
        //fightLab.text = UIRole.Instance.fcSum.ToString();

        fcSum = 0;
        for (int i = 0; i < playerData.GetInstance().herodataList.Count; i++)
        {
            fcSum += playerData.GetInstance().herodataList[i].fc;
        }
        if (fightLab != null && fightLab.text != null)
        {
            playerData.GetInstance().selfData.FightLv = fcSum;  ///所有英雄的总战力
            fightLab.text = playerData.GetInstance().selfData.FightLv.ToString();
        }


        HeroData hd = playerData.GetInstance().GetHeroDataByID(playerData.GetInstance().iconData.icon_id);
        iconLevelLab.text = hd.lvl.ToString();

        //iconLevelLab.text = playerData.GetInstance().selfData.level.ToString();///?
        //expBar.state = ProgressState.STRING;
        // expBar.InValue(int.Parse(playerData.GetInstance().selfData.exprience.ToString()), int.Parse(playerData.GetInstance().selfData.maxExprience.ToString()));
        if (FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList.ContainsKey(playerData.GetInstance().selfData.level))
        {
            expLab.text = playerData.GetInstance().selfData.exprience.ToString() + "/" + FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList[playerData.GetInstance().selfData.level].exp;
        }
        
        heroLevelLab.text = FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList[playerData.GetInstance().selfData.level].heroLvLimit.ToString();
    }
    /// <summary>
    /// 打开称号面板按钮事件
    /// </summary>
    private void OnChangeTitleNameClick()
    {
        Hide();
        //Control.ShowGUI(UIPanleID.UIPlayerTitlePanel);
    }

    public void OnChangeNameBtn()
    {
        UIRole.Instance.OpenChangeNamePanel(true);
        Control.HideGUI(this.GetUIKey());
       // this.gameObject.SetActive(false);
    }
    public void OnChangeIconBtn()
    {
        UIRole.Instance.OpenChangeIconPanel(true);
        this.gameObject.SetActive(false);
    }
    public void OnChangeIconBorderBtn()
    {
        UIRole.Instance.OpenChangeIconBorderPanel(true);
        this.gameObject.SetActive(false);
    }
    public void OnCloseBtn()
    {
       // UIRole.Instance.mask.gameObject.SetActive(false);
       // Hide();
        Control.HideGUI(this.GetUIKey());
    }

}