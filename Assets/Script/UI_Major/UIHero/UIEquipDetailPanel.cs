using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;

public class UIEquipDetailPanel : GUIBase
{
    public GUISingleButton fosterBtn;
    public GUISingleButton closeBtn;
    public GUISingleSprite icon;
    public GUISingleSprite grade;
    public UILabel equipName;
    public UILabel level;
    public UILabel currentattrLabel;
    public UILabel nameLabel;
    public GameObject backObj;
    private EquipData equipData;
    private ItemNodeState itemData;
    GradeType gradeType;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIEquipDetailPanel;
    }
    public static UIEquipDetailPanel instance;
    public UIEquipDetailPanel()
    {
        instance = this;
    }
    protected override void Init()
    {
        equipName = transform.Find("EquipName").GetComponent<UILabel>();
        backObj = transform.Find("Back").gameObject;
        level = transform.Find("Level").GetComponent<UILabel>();
        nameLabel = transform.Find("NameLabel").GetComponent<UILabel>();
        currentattrLabel = transform.Find("CurrentattrLabel").GetComponent<UILabel>();
        fosterBtn.onClick = OnFosterClick;
        UIEventListener.Get(backObj).onClick += OnCloseClick;
        closeBtn.onClick = OnBackClick;
    }
    public void SetData(EquipData data)
    {
        equipData = data;
    }
    protected override void SetUI(params object[] uiParams)
    {
        equipData = (EquipData)uiParams[0];
        base.SetUI();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        this.State = EnumObjectState.Ready;
        Show();
    }
    private void OnFosterClick()
    {
        //打开装备培养界面
        Debug.Log("打开装备培养界面");
        HeroPosEmbattle.instance.HideModel();
        EquipDevelop.GetSingolton().index = equipData.site - 1;
        //Control.ShowGUI(GameLibrary.EquipDevelop);
        Control.ShowGUI(UIPanleID.EquipDevelop, EnumOpenUIType.OpenNewCloseOld);
        UI_HeroDetail.instance.HeroID = 0;
        //Control.HideGUI(GameLibrary.UI_HeroDetail);
        //Control.HideGUI(GameLibrary.UIHeroList);
        //Control.HideGUI(UIPanleID.UIHeroList);
        //Hide();
        Control.HideGUI(this.GetUIKey());

    }
    private void OnCloseClick(GameObject go)
    {
        //Hide();
        Control.HideGUI(this.GetUIKey());
    }
    private void OnBackClick()
    {
        //Hide();
        Control.HideGUI(this.GetUIKey());
    }
    protected override void ShowHandler()
    {
        if (equipData!=null)
        {
            if (GameLibrary.Instance().ItemStateList.ContainsKey(equipData.id))
            {
                itemData = GameLibrary.Instance().ItemStateList[equipData.id];
                icon.spriteName = itemData.icon_name;
                level.text = equipData.level + "级";
                grade.spriteName = GoodsDataOperation.GetInstance().GetFrameByGrade(equipData.grade);
                equipName.text = GoodsDataOperation.GetInstance().JointEquipNameColour(itemData.name, (GradeType)equipData.grade);
                GeEquipStrengthArr();
            }
        }
    }
    private void GeEquipStrengthArr()
    {
        //当前装备
        ItemNodeState item = GameLibrary.Instance().ItemStateList[equipData.id];
        //基础装备
        long baseid = equipData.id / 100 * 100;
        ItemNodeState baseItem = GameLibrary.Instance().ItemStateList[baseid];
        short[] basepropertys = baseItem.propertylist;
        short[] propertys = item.propertylist;
        if (basepropertys == null)
            return;
        int index = 0;
        string name = "";
        string currentattrstr = "";
        string nextlvattrstr = "";
        for (int i = 0; i < basepropertys.Length; i++)
        {
            if (basepropertys[i] > 0)
            {
                name += propertyname[i] + "\n";
                currentattrstr += (basepropertys[i] + propertys[i] * equipData.level).ToString() + "\n";
                nextlvattrstr += "+ " + propertys[i].ToString() + "\n";

            }
        }
        nameLabel.text = name;
        currentattrLabel.text = currentattrstr;
    }
    string[] propertyname = { "力量","智力","敏捷","生命","攻击","护甲","魔抗","暴击","闪避","命中",
        "护甲穿透","魔法穿透","吸血","韧性","移动速","攻击速度","攻击距离","生命恢复"};
}
