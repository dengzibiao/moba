using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class EquipOpItem : GUIBase {
   public UISprite iconFram;
    public UISprite icon;
    public UILabel level;
    public UISprite upGradeArrow;
    public GUISingleButton iconbutton;
    public long itemid;
    Transform hintEffect;
    EquipData ed;
    // Use this for initialization
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    protected override void Init()
    {
        if (iconbutton != null)
            iconbutton.SetOnclick(ChechIcon);
        hintEffect = transform.Find("UI_SS_JianTou_01");
        //IntensifyBtn.onClick = ChangeTabeIntens;
        //EvolveBtn.onClick = ChangeTabeEvolve;
    }
  
    void ChechIcon()
    {
        EquipDevelop.GetSingolton().index = ed.site - 1;
        EquipDevelop.GetSingolton().ChangeSelect();
    }
    public void InitData(EquipData equipdata)
    {
        ed = equipdata;
        if (null == ed)
            return;
        level.text = ed.level+"级";
        iconFram.spriteName = UISign_in.GetspriteName(ed.grade);
        
        ItemNodeState itemnode = GameLibrary.Instance().ItemStateList[ed.id];
        if(null!=itemnode)
        {
            icon.spriteName = itemnode.icon_name;            
            if (1==itemnode.types)
            {
                if (null != upGradeArrow)
                {
                    upGradeArrow.enabled = isUpLv();
                    upGradeArrow.spriteName = GetArrowName(EquipDevelop.GetSingolton().table);
                }
            }
        }
		if(hintEffect==null)
        hintEffect = transform.Find("UI_SS_JianTou_01");
        if (hintEffect!=null)
        {
            hintEffect.gameObject.SetActive(false);
            if (EquipDevelop.GetSingolton().table==1)// EquipIntensifyPanel.instance.IsShow()
            {
                hintEffect.gameObject.SetActive(IsItemConditon());
            }
            else if(EquipDevelop.GetSingolton().table == 0) //(EquipStrengthePanel.instance.IsShow())
            {
				if (IsStrengeOneLv(0)&&IsStrengeOneLv(1))
                {
					hintEffect.gameObject.SetActive(true);
                }
                else
                {
					hintEffect.gameObject.SetActive(false);
                }
            }
        }


    }
    public bool IsStrengeOneLv(byte types)
    {

        if (ed != null)
        {
            if (types == 0)//玩家等级是否满足升级要求
            {
                if (ed.level + 1 > playerData.GetInstance().selectHeroDetail.lvl)//装备等级不能超过英雄等级
                {
                    return false;
                }
            }

            else if (types == 1)//判断升级所需花费是否满足
            {
                long count = 0;

                EquipUpgradeNode itemupnode = FSDataNodeTable<EquipUpgradeNode>.GetSingleton().DataNodeList[ed.level];

                count = itemupnode.consume;
                if (playerData.GetInstance().baginfo.gold < count)//背包里的钱不够升级所需
                {
                    return false;
                }
            }

        }
        return true;
    }
    bool IsItemMaterialcondition = false;//进化材料是否满足
    float noconditonMatalId = 0;//所缺少的材料id
    bool Ismoney = false;//金是否满足
    ItemNodeState ins;
    ItemNodeState nexins;
    //判断进化所需材料是否满足
    bool IsItemConditon()
    {
        ItemNodeState ins = GameLibrary.Instance().ItemStateList[ed.id];

		ItemNodeState nexins = null;
		if (ins.next_grade != 0) {
			nexins =GameLibrary.Instance ().ItemStateList [ins.next_grade];
		} 
        IsItemMaterialcondition = true;//进化材料是否满足
        noconditonMatalId = 0;//所缺材料id
        Ismoney = true;//金是否满足

        long matalid;
        long cont = 0;
      
		if (nexins != null) {
			//英雄品质不足
			int needlv = (1 +playerData.GetInstance().selectHeroDetail.grade) * playerData.GetInstance().selectHeroDetail.grade / 2 + 1;
			if (ins.grade + 1 > needlv) {
				return false;
			}
			if (nexins.syn_cost > playerData.GetInstance().baginfo.gold)
			{
				Ismoney = false;
				return false;
			}
			for (long i = 0; i < nexins.syn_condition.Length / 2; i++) {

				matalid = nexins.syn_condition [i, 0];
				cont = nexins.syn_condition [i, 1];
				if (matalid != 0) {
					if (GoodsDataOperation.GetInstance ().GetItemCountById (matalid) < cont) {
						IsItemMaterialcondition = false;//进化材料是否满足
						noconditonMatalId = matalid;//所缺材料id
						return false;
					}
				}

			}
		} else {
			return false;
		}
        return true;
    }
    //升级当前装备所需
    public Dictionary<string, int> GetUpOneEquipNum(out long money)
    {
       long Money = 0;
       int lv = 0;
        bool stateOK = false;
       bool LvState = true;
        money = 0;
       // long  upMoney = playerData.GetInstance().baginfo.gold;
        Dictionary<string, int> equipUp = new Dictionary<string, int>();

        if (ed != null)
        {
            if (ed.level + 1 < playerData.GetInstance().selectHeroDetail.lvl)//装备等级不能超过英雄等级
            {
                EquipUpgradeNode itemupnode = FSDataNodeTable<EquipUpgradeNode>.GetSingleton().DataNodeList[ed.level];

                Money = itemupnode.consume;//装备升级到下一级所需消耗

                if (playerData.GetInstance().baginfo.gold > Money)
                {
                    stateOK = true;
                    lv = itemupnode.id + 1;
                    money = playerData.GetInstance().baginfo.gold - Money;
                }

                if (stateOK == true)
                {
                    equipUp.Add(ed.site.ToString(), lv - ed.level);
                }
            }
           
        }
       
        return equipUp;
    }
    //强化按钮
    void OneBtnIntensifyBtnClick()
    {
        long money;
        Dictionary<string, int> equipUp = GetUpOneEquipNum(out money);
        if (null!=equipUp &&equipUp.Count > 0)
        {
            ClientSendDataMgr.GetSingle().GetHeroSend().SendUpGradeHE(playerData.GetInstance().selectHeroDetail.id, equipUp, (int)money, C2SMessageType.Active);
        }
        else
        {
            if (ed.level + 1 > playerData.GetInstance().selectHeroDetail.lvl)
            {
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "请提升英雄等级");
            }
            else
            {
                //金币不足演出点金手界面
                Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
            }
        }
    }
    public bool isUpLv()
    {
        if (ed != null)
        {
            if (ed.level + 1 < playerData.GetInstance().selectHeroDetail.lvl)//装备等级不能超过英雄等级
            {
                return false;
            }
            else
            {
                EquipUpgradeNode itemupnode = FSDataNodeTable<EquipUpgradeNode>.GetSingleton().DataNodeList[ed.level];

               long Money = itemupnode.consume;//装备升级到下一级所需消耗

                if (playerData.GetInstance().baginfo.gold < Money)//背包里的钱不够升级所需
                {
                    return false;
                }
            }
        }

        return true;
    }
	
    public string GetArrowName(byte table)
    {
        string strenge = "dalvjiantou";
        switch (table)
        {
            case 0:
                strenge = "dalvjiantou";
                break;
            case 1:
                strenge = "zisejiantou";
                break;
        }
        return strenge;
    }
}
