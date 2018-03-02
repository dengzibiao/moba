using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
//装备进化面板
public class EquipEvolvePanel : GUIBase
{
    public static EquipEvolvePanel instance;
    public UILabel currentEquipName;
    public UILabel nextEquipName;
    public UISprite curentFram;
    public UISprite nextFram;
    public UILabel currentEquiplv;
    public UILabel nextEquipLv;
    public UISprite currentIcon;
    public UISprite nextIcon;
  
    public UISprite[] matalFram;
    public GUISingleButton[] matalIcon;
    public UILabel[] needNumb;

    public UILabel needMoney;
    public GUISingleLabel allGoldLab;
    public GUISingleButton Intensify;//进化按钮
    public GUISingleButton allEvolvesBtn;//全部进化按钮
    ItemNodeState ins;
    ItemNodeState nexins;

    public UILabel attrL;
    public UILabel attrM;
    public UILabel attrR;
   public int selectIndex = 0;//选中材料的索引值
    public UISprite selectFram;//选中框
    private EquipData ed = null;
    bool IsItemMaterialcondition = false;//进化材料是否满足
    float noconditonMatalId = 0;//所缺少的材料id
    bool Ismoney = false;//金是否满足
    public Transform redPoint;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.EquipEvolvePanel;
    }

    public EquipEvolvePanel()
    {
        instance = this;
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_update_item_list_ret, UIPanleID.EquipEvolvePanel);
    }
    public override void ReceiveData(UInt32 messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            case MessageID.common_update_item_list_ret://物品变化
                RefreshItemData();//进化成功后刷新材料数据
                Debug.LogError("刷新材料");
                break;
        }
    }
    public void RefreshItemData()
    {
        if (this.gameObject.activeInHierarchy)
        {
            InitData(ed);
        }
    }
    public void InitData(EquipData equidata)
    {
        ed = equidata;
        if (ed == null)
            return;
         ins =  GameLibrary.Instance().ItemStateList[ed.id];
		if (ins.next_grade != 0)
			nexins = GameLibrary.Instance ().ItemStateList [ins.next_grade];
		else {
			nexins = null;
		}

        //currentEquipName.text = ins.name;
        currentEquipName.text = GoodsDataOperation.GetInstance().JointEquipNameColour(ins.name, (GradeType)ins.grade);
        currentIcon.spriteName = ins.icon_name;
        curentFram.spriteName = UISign_in.GetspriteName(ed.grade);

		if (nexins != null) {
			nextFram.spriteName = UISign_in.GetspriteName (ed.grade + 1);
			nextIcon.spriteName = nexins.icon_name;
			//nextEquipName.text = nexins.name;
			nextEquipName.text = GoodsDataOperation.GetInstance().JointEquipNameColour(nexins.name, (GradeType)(nexins.grade));
            nextFram.gameObject.SetActive (true);
		} else {
			nextFram.gameObject.SetActive (false);
			nextEquipName.text = "";
		}
	

        nextEquipLv.text = currentEquiplv.text = ed.level + "级";
        ItemNodeState needMatal;
        long matalid;
        int numbcount = 0;
		if (nexins != null) {
			for (long i = 0; i < 3; i++) {
				if (i < nexins.syn_condition.Length / 2) {
					matalid = nexins.syn_condition [i, 0];
					if (matalid != 0) {
						matalFram [i].gameObject.SetActive (true);

						needMatal = GameLibrary.Instance ().ItemStateList [matalid];
						if (needMatal.types == 1) {
							matalIcon [i].AtlasName = ResourceManager.Instance ().GetUIAtlas ("UIEquip");
						} else {
							matalIcon [i].AtlasName = ResourceManager.Instance ().GetUIAtlas ("Prop");
						}
						matalFram [i].spriteName = UISign_in.GetspriteName (needMatal.grade);
						matalIcon [i].spriteName = needMatal.icon_name;
						if (GoodsDataOperation.GetInstance ().GetItemCountById (matalid) >= nexins.syn_condition [i, 1]) {
							needNumb [i].color = Color.white;
						} else {
							needNumb [i].color = Color.red;
						}
						numbcount = 0;
						if (playerData.GetInstance ().baginfo.ItemDic.ContainsKey (matalid)) {
							numbcount = playerData.GetInstance ().baginfo.ItemDic [matalid].Count;
						}
						needNumb [i].text = numbcount + "/" + nexins.syn_condition [i, 1].ToString ();
                    
					} else {
						matalFram [i].gameObject.SetActive (false);
					}
				} else {
					matalFram [i].gameObject.SetActive (false);
				}
			}
		} else {
			for (long i = 0; i < 3; i++) {
				matalFram [i].gameObject.SetActive (false);
			}
		}
        //当前装备
       // ItemNodeState item = GameLibrary.Instance().ItemStateList[ed.id];
       
        //基础装备
        // long baseid = ed.id / 100 * 100;
        ItemNodeState baseItem = GameLibrary.Instance().ItemStateList[ed.baseId];
        short[] basepropertys = baseItem.propertylist;
        short[] propertys = ins.propertylist;
		short[] nexpropers = null;
		if(nexins!=null)
            nexpropers = nexins.propertylist;
        if (basepropertys == null)
            return;
        //int index = 0;
        string name = "";
        string currentattrstr = "";
        string nextlvIncreasseAttrstr = "";
        for (int i = 0; i < basepropertys.Length; i++)
        {
            if (basepropertys[i] > 0)
            {
                name += propertyname[i] + "\n";
                currentattrstr += (basepropertys[i] + propertys[i] * ed.level).ToString() + "\n";
				if (nexpropers != null) {
                    float increaseVal = (nexpropers[i] - propertys[i]) * ed.level;
                    nextlvIncreasseAttrstr += "+ " + increaseVal.ToString () + "\n";
				}
            }
        }
        allGoldLab.text = CalculateAllMoney().ToString();//所有进化的消耗金币
        IsItemConditon();//判断是否满足进化条件
		if (IsItemMaterialcondition&&nexins!=null)//材料满足时亮起
        {
            //redPoint.gameObject.SetActive(true);
            Intensify.SetState(GUISingleButton.State.Normal);
            Intensify.onClick = EquipEvolve;
        }
        else
        {
            //redPoint.gameObject.SetActive(false);
            Intensify.SetState(GUISingleButton.State.Disabled);
            Intensify.onClick = null;
        }
        //全部进化按钮显示判断
        if (IsHaveMaterial())
        {
            allEvolvesBtn.SetState(GUISingleButton.State.Normal);
            allEvolvesBtn.onClick = AllEvolvesClick;
        }
        else
        {
            allEvolvesBtn.SetState(GUISingleButton.State.Disabled);
            allEvolvesBtn.onClick = null;
        }

            attrL.text = name;
        attrM.text = currentattrstr;
        attrR.text = nextlvIncreasseAttrstr;
		if (nexins != null) {
			needMoney.gameObject.SetActive (true);
			needMoney.text = nexins.syn_cost.ToString ();
			if (nexins.syn_cost <= playerData.GetInstance().baginfo.gold)
			{

				needMoney.color = Color.white;          
			}
			else
			{
				needMoney.color = Color.red ;
			}
		}
		else
		{
			needMoney.gameObject.SetActive (false);
		}
       
        

    }

    private void AllEvolvesClick()
    {
        CalculateAllEvolves();
    }
    #region 全部进化函数
    /// <summary>
    /// 计算全部进化所需要的金币（同品级内只计算金币数，不管材料够不够）
    /// </summary>
    /// <returns></returns>
    public long CalculateAllMoney()
    {
        long costmoney = 0;
        List<ItemData> itemlist = playerData.GetInstance().baginfo.itemlist;
        List<int> siteList = new List<int>();
        ItemNodeState currentins;
        ItemNodeState nextins;
        long currentGold = playerData.GetInstance().baginfo.gold;
        for (int i = 0; i < playerData.GetInstance().selectHeroDetail.equipSite.Count; i++)
        {
            currentins = GameLibrary.Instance().ItemStateList[playerData.GetInstance().selectHeroDetail.equipSite[i + 1].id];
            if (ins.next_grade != 0)
                nextins = GameLibrary.Instance().ItemStateList[currentins.next_grade];
            else
            {
                nextins = null;
            }
            if (nextins != null)
            {
                //英雄品质不足的不计算钱
                int needlv = (1 + playerData.GetInstance().selectHeroDetail.grade) * playerData.GetInstance().selectHeroDetail.grade / 2 + 1;
                if (currentins.grade + 1 <= needlv)
                {
                    costmoney += nextins.syn_cost;
                }
                
                //if (IsItemCodition(nextins, ref currentGold))
                //{
                //    costmoney += nextins.syn_cost;
                //}
                //else
                //{
                //    continue;
                //}
            }
        }
        return costmoney;
    }
    //是否有材料满足进化（用于是否显示按钮）
    public bool IsHaveMaterial()
    {
        ItemNodeState currentins;
        ItemNodeState nextins;
        int count = 0;
        for (int i = 0; i < playerData.GetInstance().selectHeroDetail.equipSite.Count; i++)
        {
            currentins = GameLibrary.Instance().ItemStateList[playerData.GetInstance().selectHeroDetail.equipSite[i + 1].id];
            if (currentins.next_grade != 0)
                nextins = GameLibrary.Instance().ItemStateList[currentins.next_grade];
            else
            {
                nextins = null;
            }
            if (nextins != null)
            {

                long matalid = 0;
                long cont = 0;
                for (long j = 0; j < nextins.syn_condition.Length / 2; j++)
                {

                    matalid = nextins.syn_condition[j, 0];
                    cont = nextins.syn_condition[j, 1];
                    if (matalid != 0)
                    {
                        if (GoodsDataOperation.GetInstance().GetItemCountById(matalid) < cont)
                        {
                            IsItemMaterialcondition = false;//进化材料是否满足
                            noconditonMatalId = matalid;//所缺材料id
                            count++;                           //材料不够
                            break;
                        }
                        //材料满足但是装备品质已经超过了英雄品质
                        int needlv = (1 + playerData.GetInstance().selectHeroDetail.grade) * playerData.GetInstance().selectHeroDetail.grade / 2 + 1;
                        if (currentins.grade + 1 > needlv)
                        {
                            count++;
                            break;
                        }
                    }
                }
               
            }
        }
        if (count >= 6) return false;
        else return true;
    }
    public Dictionary<long, ItemData> ItemDic = new Dictionary<long, ItemData>();
    //计算所有进化
    public void CalculateAllEvolves()
    {
        ItemDic = playerData.GetInstance().baginfo.ItemDic;
        List<int> siteList = new List<int>();
        ItemNodeState currentins;
        ItemNodeState nextins;
        long currentGold = playerData.GetInstance().baginfo.gold;
        for (int i = 0; i < playerData.GetInstance().selectHeroDetail.equipSite.Count; i++)
        {
            currentins = GameLibrary.Instance().ItemStateList[playerData.GetInstance().selectHeroDetail.equipSite[i+1].id];
            if (currentins.next_grade != 0)
                nextins = GameLibrary.Instance().ItemStateList[currentins.next_grade];
            else
            {
                nextins = null;
            }
            if (nextins!=null)
            {
                if (IsItemCodition(nextins, ref currentGold,ref ItemDic))
                {
                    //英雄品质不足的也不添加
                    int needlv = (1 + playerData.GetInstance().selectHeroDetail.grade) * playerData.GetInstance().selectHeroDetail.grade / 2 + 1;
                    if (currentins.grade + 1 <= needlv)
                    {
                        siteList.Add(playerData.GetInstance().selectHeroDetail.equipSite[i + 1].site);
                        currentGold -= nexins.syn_cost;

                    }
                }
                else
                {
                    continue;
                }
            }
        }
        foreach (int a in siteList)
        {
            Debug.LogError(a);
        }
        if (siteList.Count == 0 && CalculateAllMoney() <= 0)
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "请提升英雄品质");
        }
        else if (siteList.Count == 0)
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "金币不足");
            Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
        }
        else
        {
            Debug.LogError("进化");
        }
    }
    //获得数量
    public int GetItemCountById(long itemid)
    {
        if (ItemDic.ContainsKey(itemid))
        {
            return ItemDic[itemid].Count;
        }
        return 0;
    }
    //减材料
    public void RemoveItemCount(long itemid,int count)
    {
        if (ItemDic.ContainsKey(itemid))
        {
            if (ItemDic[itemid].Count > count)
            {
                ItemDic[itemid].Count -= count;
            }
            else if(ItemDic[itemid].Count==count)
            {
                ItemDic.Remove(itemid);
            }
        }
    }
    bool IsItemCodition(ItemNodeState nextins, ref long money,ref Dictionary<long, ItemData> ItemDic)
    {
        ItemNodeState myNextins = nextins;
        long matalid = 0;
        long cont = 0;
        if (myNextins != null)
        {
            //钱不够了
            if (myNextins.syn_cost > money)
            {
                Ismoney = false;
                return false;
            }

            for (long j = 0; j < myNextins.syn_condition.Length / 2; j++)
            {

                matalid = myNextins.syn_condition[j, 0];
                cont = myNextins.syn_condition[j, 1];
                if (matalid != 0)
                {
                    if (GetItemCountById(matalid) < cont)
                    {
                        IsItemMaterialcondition = false;//进化材料是否满足
                        noconditonMatalId = matalid;//所缺材料id
                        return false;                           //材料不够
                    }
                }

            }
        }
        else
        {
            return false;
            //没有下一级了
        }
        money -= myNextins.syn_cost;
        //RemoveItemCount(matalid,(int)cont);//6件装备进化不会消耗相同的材料
        return true;
    }
    #endregion
    bool IsItemCodition(ItemNodeState nextins,ref long money)
    {
        ItemNodeState myNextins = nextins;
        long matalid;
        long cont = 0;
        if (myNextins != null)
        {
            //钱不够了
            if (myNextins.syn_cost > money)
            {
                Ismoney = false;
                return false;
            }

            for (long j = 0; j < myNextins.syn_condition.Length / 2; j++)
            {

                matalid = myNextins.syn_condition[j, 0];
                cont = myNextins.syn_condition[j, 1];
                if (matalid != 0)
                {
                    if (GoodsDataOperation.GetInstance().GetItemCountById(matalid) < cont)
                    {
                        IsItemMaterialcondition = false;//进化材料是否满足
                        noconditonMatalId = matalid;//所缺材料id
                        return false;                           //材料不够
                    }
                }

            }
        }
        else
        {
            return false;
            //没有下一级了
        }
        money -= myNextins.syn_cost;
        return true;
    }
    string[] propertyname = { "力量","智力","敏捷","生命","攻击","护甲","魔抗","暴击","闪避","命中",
        "护甲穿透","魔法穿透","吸血","韧性","移动速","攻击速度","攻击距离","生命恢复"};

    //判断进化所需材料是否满足
    bool IsItemConditon()
    {
        IsItemMaterialcondition = true;//进化材料是否满足
        noconditonMatalId = 0;//所缺材料id
        Ismoney = true;//金是否满足

        long matalid;
        long cont = 0;
		if (nexins != null) {
			if (nexins.syn_cost > playerData.GetInstance ().baginfo.gold) {
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
    //进化按钮事件
    void EquipEvolve()
    {
        int needlv = (1 +playerData.GetInstance().selectHeroDetail.grade) * playerData.GetInstance().selectHeroDetail.grade / 2 + 1;
        if (ins.grade + 1 > needlv)
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "请提升英雄品质");
            return;
        }
        //if (IsItemMaterialcondition)
        //{
        //    UITooltips.Instance.SetBlackerBottom_Text("进化材料不足，请查看");
        //    Control.ShowGUI(GameLibrary.UITooltips);
        //    return;
        //}
        if(!Ismoney)
        {
            // UITooltips.Instance.SetBlackerBottom_Text("进化金币不足");
            //  Control.ShowGUI(GameLibrary.UITooltips);
            //金币不足时打开点金手面板
            Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
            return;
        }
        //发送通信协议
        ClientSendDataMgr.GetSingle().GetHeroSend().SendHeroEMon(playerData.GetInstance().selectHeroDetail.id, EquipDevelop.GetSingolton().index+1);

    }


    protected override void Init()
    {
        matalIcon[0].onClick = onClick1;
        matalIcon[1].onClick = onClick2;
        matalIcon[2].onClick = onClick3;
    }
    void initComb(long id)
    {
        if (id == 0)
            return;
        ItemNodeState selectMaterial = GameLibrary.Instance().ItemStateList[id];
        if (selectMaterial.syn_condition.Length > 0)
        {

            //   if(selec)
            if(!EquipDevelop.GetSingolton().equipIntensifyDlg.equipCompoundPanel.gameObject.activeSelf)
            EquipDevelop.GetSingolton().equipIntensifyDlg.equipCompoundPanel.gameObject.SetActive(true);
           // EquipDevelop.GetSingolton().equipIntensifyDlg.equipCompoundPanel.InitData(selectMaterial);
            EquipDevelop.GetSingolton().equipIntensifyDlg.SetIntersifyData(EquipDevelop.GetSingolton().checkIndex + 1, id);
             gameObject.SetActive(false);
        }
        else
        {
            //UIGoodsGetWayPanel.Instance.SetID(id);
            //if (UILevel.instance.GetLevelData())
            //{
            //    Control.ShowGUI(GameLibrary.UIGoodsGetWayPanel);
            //}
            Control.ShowGUI(UIPanleID.UIGoodsGetWayPanel, EnumOpenUIType.DefaultUIOrSecond, false, id);
            //出处面板   drop_fb 掉落副本的列表
        }


    }
void onClick1()
    {        
		if (nexins == null)
			return;
        selectFram.transform.parent = matalFram[0].transform;
        selectIndex = 0;

        initComb(nexins.syn_condition[0, 0]);
    }
    void onClick2()
    {
		if (nexins == null)
			return;
        selectFram.transform.parent = matalFram[1].transform;
        selectIndex = 1;
        initComb(nexins.syn_condition[1, 0]);
    }
    void onClick3()
    {
		if (nexins == null)
			return;
        selectFram.transform.parent = matalFram[2].transform;
        selectIndex = 2;
        initComb(nexins.syn_condition[2, 0]);
    }
    protected override void ShowHandler()
    {
        if (EquipDevelop.GetSingolton().index + 1 < 6)
        {
            InitData(playerData.GetInstance().selectHeroDetail.equipSite[EquipDevelop.GetSingolton().index + 1]);
        }
        else
        {
            InitData(playerData.GetInstance().selectHeroDetail.equipSite[6]);
        }
    }
        // Update is called once per frame
        void Update () {
	
	}

    protected override void RegisterComponent()
    {
        base.RegisterComponent();

        RegisterComponentID(37, 120, Intensify.gameObject);
      
    }
    protected override void RegisterIsOver()
    {
        base.RegisterIsOver();
    }
}
