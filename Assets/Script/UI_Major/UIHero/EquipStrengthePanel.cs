using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;
using System;
//装备强化面板
public class EquipStrengthePanel : GUIBase {

    public UILabel strengeToLvLabel;//新手引导挂点

    //  public UILabel StrengAllGoldLab;//强化全部消耗
    // public GUISingleButton StrengAllBtn;
    public EquipOpItem selectEquip;//所选装备
    public UILabel equipname;//装备名称
    public UILabel currentLv;//装备的当前等级
    public UILabel nextLv;//装备的下一级
    public UILabel EquipStrenAttrname;//装备强化属性名称
    public UILabel equipstrenAttrMiddle;//装备强化当前等级属性值
    public UILabel EquipStrenAttrRight;//装备强化下一等级属性值
    public UILabel EquipOneLvCost;//装备升一级所需消耗
    public UILabel EquipToLvCost;//装备一键升级升级多次的花费
    public GUISingleButton strengeOnebtn;//强化一次按钮
    public GUISingleButton strengeToLv;//一键强化按钮
    public GUISingleLabel allEquopToLvCost;//所有装备一键强化的花费
    public GUISingleButton strengAllEquipBtn;//所有装备一键强化

    //public GUISingleMultList EquipStrenMultList;//装备强化属性增加列表
    public int isuplv = 0;//能强化到的等级
    public long toLvConst = 0;//能强化到的装备等级所需花费
    public long oneLvconst = 0;//强化下一级所需花费；
    public UILabel strengeToLvname;//一键强化按钮名字
    public int equipsendLvUp;
    EquipData ed;

    public Transform effEquipEff;//装备特效
    public Transform oneStrengthRed;//强化一次红点
    public Transform allStrengthRed;//一键强化红点
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.EquipStrengthePanel;
    }
    public static EquipStrengthePanel instance;
    public EquipStrengthePanel()
    {
        instance = this;
    }


    protected override void Init()
    {
        base.Init();
        instance = this;
        effEquipEff = transform.Find("EquipIcon/UI_QiangHua_01");
        //oneStrengthRed = transform.Find("OneStrengrhRed");
        //allStrengthRed = transform.Find("AllStrengrhRed");
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_upgrade_hero_equipment_ret, UIPanleID.EquipStrengthePanel);
    }
    public override void ReceiveData(UInt32 messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            case MessageID.common_upgrade_hero_equipment_ret://装备强化
                OpenEquipStrengEff();//显示装备强化特效
                Debug.LogError("播放强化特效");
                break;
        }
    }

    public void InitData(EquipData equipitem)
    {
        ed = equipitem;
        if (ed != null)
        {
            selectEquip.InitData(ed);
            currentLv.text = ed.level.ToString() + "级";
            nextLv.text = (ed.level + 1).ToString() + "级";
        }
        //当前装备
        ItemNodeState item = GameLibrary.Instance().ItemStateList[ed.id];
        if(null !=item)
        {
            //equipname.text = item.name;
            equipname.text = GoodsDataOperation.GetInstance().JointEquipNameColour(item.name, (GradeType)ed.grade);
        }
        //基础装备
        long baseid = ed.id / 100 * 100;
        ItemNodeState baseItem = GameLibrary.Instance().ItemStateList[baseid];
        short[] basepropertys = baseItem.propertylist;
        short[] propertys = item.propertylist;
        if (basepropertys == null)
            return;
        int index = 0;
        string name = "";
        string currentattrstr = "";
        string nextlvattrstr = "";
        for(int i = 0;i<basepropertys.Length;i++)
        {
            if(basepropertys[i]>0)
            {
                name += propertyname[i]+"\n";
                currentattrstr += (basepropertys[i] + propertys[i] * ed.level).ToString()+"\n";
                nextlvattrstr    += "+ "+ propertys[i].ToString()+"\n";
                
            }
        }
        EquipStrenAttrname.text = name;
        equipstrenAttrMiddle.text = currentattrstr;
        EquipStrenAttrRight.text = nextlvattrstr;
        oneLvconst = GetStrenthOne(ed);
        EquipOneLvCost.text = oneLvconst.ToString();
        toLvConst = StrengeToLvCost();
        EquipToLvCost.text = toLvConst.ToString();
        allEquopToLvCost.text = AllStrengeToLvCost().ToString();
        if (!IsStrengeOneLv(0))
        {
            strengeOnebtn.SetState( GUISingleButton.State.Disabled);
            strengeToLv.SetState(GUISingleButton.State.Disabled);
            //oneStrengthRed.gameObject.SetActive(false);
            //allStrengthRed.gameObject.SetActive(false);
        }
        else
        {
            strengeOnebtn.SetState(GUISingleButton.State.Normal);
            strengeToLv.SetState(GUISingleButton.State.Normal);
            //oneStrengthRed.gameObject.SetActive(true);
            //allStrengthRed.gameObject.SetActive(true);
        }
        //全部按钮显示判断
        //if (long.Parse(allEquopToLvCost.text) > 0)
        //{
        //    strengAllEquipBtn.SetState(GUISingleButton.State.Normal);
        //}
        //else
        //{
        //    strengAllEquipBtn.SetState(GUISingleButton.State.Disabled);
        //}
        strengeOnebtn.onClick = StrengeOneLv;//强化一次
        strengeToLv.onClick = StrengeToLv;//一键强化，强化多次
        strengAllEquipBtn.onClick = StrengeAllToLv;

    }

    public void StrengeAllToLv()
    {
        CalculateAllStrenge();
    }

    string[]  propertyname= { "力量","智力","敏捷","生命","攻击","护甲","魔抗","暴击","闪避","命中",
        "护甲穿透","魔法穿透","吸血","韧性","移动速","攻击速度","攻击距离","生命恢复"};

    public long StrengeToLvCost()
    {
        long equipconst = 0;

        if (ed != null)
        {
            int equiplv = ed.level;
            EquipUpgradeNode itemupnode;
            for (;equiplv <= playerData.GetInstance().selectHeroDetail.lvl-1;)
            {
                 itemupnode = FSDataNodeTable<EquipUpgradeNode>.GetSingleton().DataNodeList[equiplv];

              //  if (playerData.GetInstance().baginfo.gold > equipconst + itemupnode.consume)//背包里的钱不够升级所需
                {
                    equipconst += itemupnode.consume;
                    equiplv++;
                    
                }
               // else
                  //  break;
            }
            isuplv = equiplv;
            return equipconst;

        }
        return equipconst;
    }
    public int GetLvByCost(UInt32 cost)
    {
        if (cost <= oneLvconst)
            return 1;
        else
        {
            return isuplv;
        }
      //  int lv = 0;
      //  EquipUpgradeNode itemupnode = FSDataNodeTable<EquipUpgradeNode>.GetSingleton().DataNodeList[ed.level];

        //  if (playerData.GetInstance().baginfo.gold > equipconst + itemupnode.consume)//背包里的钱不够升级所需

       // equipconst += itemupnode.consume;

    }
    public bool IsStrengeOneLv(byte types)
    {

        if (ed != null)
        {
            if(types == 0)//玩家等级是否满足升级要求
            {
                if (ed.level + 1 > playerData.GetInstance().selectHeroDetail.lvl)//装备等级不能超过英雄等级
                {
                    return false;
                }
            }
           
            else if( types == 1)//判断升级所需花费是否满足
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
    public long GetStrenthOne(EquipData ed)
    {
        long count = 0;
       
        EquipUpgradeNode itemupnode = FSDataNodeTable<EquipUpgradeNode>.GetSingleton().DataNodeList[ed.level];

        count = itemupnode.consume;
        //装备升级到下一级所需消耗
        
        return count;
    }
    protected override void ShowHandler()
    {
        effEquipEff.gameObject.SetActive(false);
        if (EquipDevelop.GetSingolton().index + 1 < 6)
        {
            InitData(playerData.GetInstance().selectHeroDetail.equipSite[EquipDevelop.GetSingolton().index + 1]);
        }
        else
        {
            InitData(playerData.GetInstance().selectHeroDetail.equipSite[6]);
        }
    }
    public void closeDlg()
    {
        Hide();
    }
      
    public void StrengeOneLv()
    {
      //  long currentGold = playerData.GetInstance().baginfo.gold;

        if (!IsStrengeOneLv(0))//英雄等级不满足
        {
            // PromptPanel.instance.ShowPrompt("请提升英雄等级");
            //UIPromptBox.Instance.ShowLabel("请提升英雄等级");
            //Control.ShowGUI(GameLibrary.UIPromptBox);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "请提升英雄等级");
            return;
        }
       

        if(IsStrengeOneLv(1))//  if (itemupnode.consume < currentGold)
        {
            // upGradelvl = 1;
            EquipUpgradeNode itemupnode = FSDataNodeTable<EquipUpgradeNode>.GetSingleton().DataNodeList[ed.level];
            Dictionary<string, int> equipUp = new Dictionary<string, int>();
            equipUp.Add(ed.site.ToString(), 1);
            equipsendLvUp = 1;
            ClientSendDataMgr.GetSingle().GetHeroSend().SendUpGradeHE(playerData.GetInstance().selectHeroDetail.id, equipUp, (int)itemupnode.consume, C2SMessageType.Active);
        }
        else
        {
            Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
            //UIPromptBox.Instance.ShowLabel("金币不足");
            //Control.ShowGUI(GameLibrary.UIPromptBox);
            // PromptPanel.instance.ShowPrompt("金币不足");
        }
    }
    //计算所有装备强化到满级所需要的钱
    public long AllStrengeToLvCost()
    {
        long equipconst = 0;
        for (int i = 0; i < playerData.GetInstance().selectHeroDetail.equipSite.Count; i++)
        {
            if (playerData.GetInstance().selectHeroDetail.equipSite[i + 1].level >= playerData.GetInstance().selectHeroDetail.lvl)
            {
                continue;
            }
            int equiplv = playerData.GetInstance().selectHeroDetail.equipSite[i + 1].level;
            EquipUpgradeNode itemupnode;
            for (; equiplv <= playerData.GetInstance().selectHeroDetail.lvl - 1;)
            {
                itemupnode = FSDataNodeTable<EquipUpgradeNode>.GetSingleton().DataNodeList[equiplv];
                equipconst += itemupnode.consume;
                equiplv++;
            }
        }
        return equipconst;
    }
    public void CalculateAllStrenge()
    {
        long equipconst = 0;
        int lvup = 0;
        Dictionary<string, int> equipUp = new Dictionary<string, int>();
        long currentGold = playerData.GetInstance().baginfo.gold;
        for (int i=0;i< playerData.GetInstance().selectHeroDetail.equipSite.Count;i++)
        {
            if (playerData.GetInstance().selectHeroDetail.equipSite[i+1].level>= playerData.GetInstance().selectHeroDetail.lvl)
            {
                continue;
            }
            int equiplv = playerData.GetInstance().selectHeroDetail.equipSite[i + 1].level;
            EquipUpgradeNode itemupnode;
            //以钱为准 如果钱够就升到最大等级 如果钱不够就升到相应的等级
            for (; equiplv <= playerData.GetInstance().selectHeroDetail.lvl - 1;)
            {
                itemupnode = FSDataNodeTable<EquipUpgradeNode>.GetSingleton().DataNodeList[equiplv];
                equipconst += itemupnode.consume;
                equiplv++;
                //如果钱不够了 就跳出循环
                if (equipconst>currentGold)
                {
                    equipconst -= itemupnode.consume;
                    equiplv--;
                    break;
                }
            }
            isuplv = equiplv - playerData.GetInstance().selectHeroDetail.equipSite[i + 1].level;
            //花费的总金币比当前金币少 并且提升的等级>0添加到列表
            if (equipconst < currentGold&&isuplv>0)
            {
                equipUp.Add(playerData.GetInstance().selectHeroDetail.equipSite[i + 1].site.ToString(), isuplv);
                equipsendLvUp = isuplv;
                //currentGold = currentGold - equipconst;
                //equipconst = 0;
                isuplv = 0;
            }
        }
        if (equipUp != null && equipUp.Count > 0)
        {
            ClientSendDataMgr.GetSingle().GetHeroSend().SendUpGradeHE(playerData.GetInstance().selectHeroDetail.id, equipUp, (int)equipconst, C2SMessageType.Active);
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "金币不足");
            Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
        }
        foreach (var a in equipUp)
        {
            Debug.LogError(a.Key+"----"+a.Value);
        }
    }
    public void StrengeToLv()
    {
        if (ed.level >= playerData.GetInstance().selectHeroDetail.lvl)
        {
          //  PromptPanel.instance.ShowPrompt("请提升英雄等级");
            //UIPromptBox.Instance.ShowLabel("请提升英雄等级");
            //Control.ShowGUI(GameLibrary.UIPromptBox);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "请提升英雄等级");
            return;
        }
        long equipconst = 0;
        int lvup = 0;
        if (ed != null)
        {
            int equiplv = ed.level;
            EquipUpgradeNode itemupnode;
            for (; equiplv <= playerData.GetInstance().selectHeroDetail.lvl - 1;)
            {
                itemupnode = FSDataNodeTable<EquipUpgradeNode>.GetSingleton().DataNodeList[equiplv];
                equipconst += itemupnode.consume;
                equiplv++;
            }

            long currentGold = playerData.GetInstance().baginfo.gold;
            Dictionary<string, int> equipUp = new Dictionary<string, int>();

            isuplv = equiplv-ed.level;
            if (equipconst < currentGold)
            {
                // upGradelvl = 1;

                equipUp.Add(ed.site.ToString(), isuplv);
                equipsendLvUp = isuplv;
                ClientSendDataMgr.GetSingle().GetHeroSend().SendUpGradeHE(playerData.GetInstance().selectHeroDetail.id, equipUp, (int)equipconst, C2SMessageType.Active);
            }
            else
            {
                //UIPromptBox.Instance.ShowLabel("金币不足");
                //Control.ShowGUI(GameLibrary.UIPromptBox);
                Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "金币不足");
                //  PromptPanel.instance.ShowPrompt("金币不足");
            }
           
        }
      //  return equipconst;
    }

    protected override void RegisterComponent()
    {
        base.RegisterComponent();

        RegisterComponentID(34, 120, strengeToLvLabel.gameObject);

    }
    protected override void RegisterIsOver()
    {
        base.RegisterIsOver();
    }
    /// <summary>
    /// 装备强化特效
    /// </summary>
    public void OpenEquipStrengEff()
    {
        effEquipEff.gameObject.SetActive(true);
        StartCoroutine(HideEquipStrengEff());
    }
    IEnumerator HideEquipStrengEff()
    {
        yield return new WaitForSeconds(1f);
        effEquipEff.gameObject.SetActive(false);
    }
}
