using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;
public class EquipDevelop : GUIBase {

    public GameObject EvolveBtnLabel;//新手引导挂点
    //public GameObject selectFram;//新手引导挂点
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.EquipDevelop;
    }

    static EquipDevelop singleton;
    public  static EquipDevelop GetSingolton()
    {
        return singleton;
    }
    // Use this for initialization
    //void Start()
    //{

    //    singleton = this;

    //}
    public Transform content;//面板内容

    public GUISingleCheckBoxGroup EquipOptTable;    //标签栏
    public EquipStrengthePanel equipStrenthDlg;//强化面板
    public EquipIntensifyPanel equipIntensifyDlg;//进化面板
    public Transform strenthEffect;
    public Transform intensifyEffect;
   
    public UISprite IntensifyImg;//强化按钮的图片
    public UISprite EvolveImg;//进化按钮的图片
    public UISprite selectFram;//装备选中框资源
    public GUISingleButton HeroPanel;//装备列表


    public EquipOpItem[] equipItemarr;//装备列表
    public Transform[] equipEffectarr;//装备强化特效数组
    public Transform[] equipjinEffectarr;//装备进步啊特效数组

    public UISprite heroFram;//所选英雄头像框图片
    public GUISingleButton heroIcon;//所选英雄头像图片
    public GUISingleButton SwitchHeroBtn;//切换英雄按钮
    public UISprite[] startarr;//英雄星级资源
    public UILabel herolv;//英雄等级显示
    public UIGrid starGrid;
                                        
    //site 1武器 2头盔 3胸甲 4腿甲 5护手 6鞋子
    public int sites = 0;
    //分页
    public byte table = 0;
    public int index = 0;
    public int checkIndex = 0;

    public GUISingleButton BackBtn;

    public EquipData  ed;


    public GUISingleMultList allofMultList;             //所有英雄
    //public object[] obj;                                //所有英雄
    Dictionary<long, object> heroList;                  //所有英雄字典

    List<long> count = new List<long>();                //所有英雄的键值 

    public EquipDevelop()
    {
        singleton = this;
    }
    HeroData hd;

    protected override void Init()
    {
        base.Init();
        singleton = this;
        heroIcon.onClick = HeroIconOnClick;
        starGrid = transform.Find("content/heroicon/Grid").GetComponent<UIGrid>();
        if (content == null)
        {
            content = transform.FindChild("content");
        }
        if (equipItemarr == null)
        {
            Transform obj;
            for (int i = 0; i < 6; i++)
            {
                obj = content.FindChild("Equip_" + (i + 1).ToString());
                if (obj != null)
                {
                    equipItemarr[i] = obj.GetComponent<EquipOpItem>();
                }
            }
        }

        if (BackBtn == null)
        {
            BackBtn = transform.Find("content/BackBtn").GetComponent<GUISingleButton>();
        }
        // SetTabButton();
        BackBtn.onClick = OnClose;
        EquipOptTable.onClick = SetTabButton;
        SwitchHeroBtn.onClick = ChangHero;
        HeroPanel.onClick = OnCloseHeroList;

        ////获取全部英雄
        //Dictionary<long, HeroNode> objDic = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList;
        //object hn;
        //int  count = playerData.GetInstance().herodataList.Count;

        //obj = new object[count];

        //for(int i = 0;i< playerData.GetInstance().herodataList.Count;i++)
        //{
        //    hn = objDic[playerData.GetInstance().herodataList[0].id];
        //    obj[i] = hn;           
        //}
        //IntensifyBtn.onClick = ChangeTabeIntens;
        //   EvolveBtn.onClick = ChangeTabeEvolve;
    }
    // Update is called once per frame
    void Update () {
	
	}
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_player_hero_info_ret, UIPanleID.EquipDevelop);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_upgrade_hero_equipment_ret, UIPanleID.EquipDevelop);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_hero_equipment_volve_ret, UIPanleID.EquipDevelop);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_hero_equipment_compound_ret, UIPanleID.EquipDevelop);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_update_item_list_ret, UIPanleID.EquipDevelop);
        playerData.GetInstance().isEquipDevelop = true;
        if (playerData.GetInstance().selectHeroDetail != null)
        {
            ClientSendDataMgr.GetSingle().GetHeroSend().SendGetHeroInfo(playerData.GetInstance().selectHeroDetail.node.hero_id, C2SMessageType.ActiveWait);
        }
        else
        {
            ClientSendDataMgr.GetSingle().GetHeroSend().SendGetHeroInfo(Globe.fightHero[0], C2SMessageType.ActiveWait);
        }
    }
    public override void ReceiveData(UInt32 messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            case MessageID.common_player_hero_info_ret:
                Show();
                Control.PlayBgmWithUI(UIPanleID.EquipDevelop);
                break;
            case MessageID.common_upgrade_hero_equipment_ret://装备强化
                RefreshUI(EquipDevelopManager.Single().hd);
                EquipStrengthePanel.instance.OpenEquipStrengEff();//显示装备强化特效
                OpenEquipEff(EquipDevelopManager.Single().item);
                break;
            case MessageID.common_hero_equipment_volve_ret://装备进化
                RefreshUI(EquipDevelopManager.Single().hd);
                EquipIntensifyPanel.instance.OpenEquipIntensEff();//显示进化特效
                OpenEquipjinhuaEff(EquipDevelopManager.Single().item);
                break;
            case MessageID.common_hero_equipment_compound_ret://材料合成
                EquipCompoundPanel.instance.OpenMaterialCompountEff();//播放合成特效
                break;
            case MessageID.common_update_item_list_ret://物品变化
                EquipEvolvePanel.instance.RefreshItemData();//进化成功后刷新材料数据
                if (EquipCompoundPanel.instance!=null&&EquipCompoundPanel.instance.IsShow())
                {
                    EquipCompoundPanel.instance.RefreshItemData();//材料合成成功刷新数据
                }
                break;
        }
    }
    /// <summary>
    /// 点击切换到英雄界面
    /// </summary>
    void HeroIconOnClick()
    {
        playerData.GetInstance().selectHeroDetail.id = hd.id;
        //Control.ShowGUI(GameLibrary.UI_HeroDetail);
        //Hide();
        //Control.HideGUI();
        Control.ShowGUI(UIPanleID.UIHeroDetail, EnumOpenUIType.OpenNewCloseOld, false, false);
    }
    void OnCloseHeroList()
    {
        HeroPanel.gameObject.SetActive(false);
    }
    void ChangHero()
    {
        HeroPanel.gameObject.SetActive(true);
        InitHeroData();
        //Control.ShowGUI(GameLibrary.EquipHeroListPanel);
    }
    void InitHeroData()
    {
        //全部已拥有英雄的字典
        heroList = new Dictionary<long, object>();
        Dictionary<long, HeroNode> objDic = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList;
        object hn;
        for (int i = 0; i < playerData.GetInstance().herodataList.Count; i++)
        {
            hn = objDic[playerData.GetInstance().herodataList[i].id];
            heroList.Add(playerData.GetInstance().herodataList[i].id, hn);
        }
      //  int index = 0;
        object[] objs = new object[playerData.GetInstance().herodataList.Count];
        object obje = null;
        //已经拥有的英雄
        for (int i = 0; i < playerData.GetInstance().herodataList.Count; i++)
        {

            if (heroList.TryGetValue(playerData.GetInstance().herodataList[i].id, out obje))
            {
                objs[i] = obje;
                count.Remove(playerData.GetInstance().herodataList[i].id);
               // index++;
                heroList.Remove(playerData.GetInstance().herodataList[i].id);
            }
        }
        //进行显示
        InitHeroList(objs);

    }
    /// <summary>
    /// 英雄列表数据显示
    /// </summary>
    /// <param name="allHero"></param>
    void InitHeroList(object[] allHero)
    {



        //全部英雄
        allofMultList.InSize(allHero.Length, 5);
        allofMultList.Info(allHero);

    }
    public void ChangeSelect()
    {
        if (null != equipItemarr)
        {
            // equipItemarr[0].InitData(hd.equipSite[]);
            
             selectFram.transform.parent = equipItemarr[index].transform;
            selectFram.transform.localPosition = Vector3.zero;
        }
        ed = playerData.GetInstance().selectHeroDetail.equipSite[index + 1];
        if(equipStrenthDlg.IsShow())
        equipStrenthDlg.InitData(ed);
        if (equipIntensifyDlg.IsShow())
            equipIntensifyDlg.InitData(ed);
      //  equipStrenthDlg
      //  equipStrenthDlg.selectEquip.InitData(playerData.GetInstance().selectHeroDetail.equipSite[index + 1]);
    }
   public void RefreshUI(HeroData hd)
    {
        InitEquip(hd);
    }
    protected override void ShowHandler()
    {
        //  SetTabButton(0, true);
        //index = 0;
        RefreshUI(playerData.GetInstance().selectHeroDetail);
        for (int i = 0; i < equipEffectarr.Length; i++)
        {
            equipEffectarr[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < equipjinEffectarr.Length; i++)
        {
            equipjinEffectarr[i].gameObject.SetActive(false);
        }
        //  SetTabButton();
    }
    void OnClose()
    {
        index = 0;
        Control.HideGUI();
        //Hide();
        Control.PlayBGmByClose(this.GetUIKey());
    }

    void SetTabButton(int index,bool boo)
    {
        table = (byte)index;
        switch (index)
        {
            case 0:
                //IntensifyImg.spriteName = "yeqian-xuanzhong";
                //EvolveImg.spriteName = "yeqian";
              //  Control.ShowGUI(GameLibrary.EquipStrengthePanel);
              
                equipStrenthDlg.gameObject.SetActive(true);
                equipStrenthDlg.InitData(playerData.GetInstance().selectHeroDetail.equipSite[EquipDevelop.GetSingolton().index + 1]);
                equipIntensifyDlg.gameObject.SetActive(false);
                ReshEquipItem();
                strenthEffect.gameObject.SetActive(true);
                intensifyEffect.gameObject.SetActive(false);
                break;
            case 1:
                //IntensifyImg.spriteName = "yeqian";
                //EvolveImg.spriteName = "yeqian-xuanzhong";
                 equipStrenthDlg.gameObject.SetActive(false);
               // Control.HideGUI(GameLibrary.EquipStrengthePanel);
                equipIntensifyDlg.gameObject.SetActive(true);
                equipIntensifyDlg.InitData(playerData.GetInstance().selectHeroDetail.equipSite[EquipDevelop.GetSingolton().index + 1]);
                ReshEquipItem();
                strenthEffect.gameObject.SetActive(false);
                intensifyEffect.gameObject.SetActive(true);
                break;
        }
    }
    //string GetHerogradeFram(int grade)
    //{
    //    string str = "";
    //    switch(grade)
    //    {
    //        case 1:
    //            break;
    //    }
    //    return str;
    //}
    void SetHeroStar(int lv)
    {
        for(int i = 0;i< startarr.Length;i++)
        {
            if (i < lv )
            {
                startarr[i].gameObject.SetActive(true);
            }
            else
            {
                startarr[i].gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// 初始化英雄装备
    /// </summary>
    public void InitEquip(HeroData hd)
    {
       
        if (null!=hd&&null!=hd.node)
        {
            heroIcon.spriteName = hd.node.icon_name;
            heroFram.spriteName = UISign_in.GetHeroGradeName(hd.grade);
            SetHeroStar(hd.star);
            starGrid.Reposition();
            herolv.text = hd.lvl.ToString()+"级";
            this.hd = hd;
        }
        // heroIcon.spriteName = hd.
        //英雄头像数据初始化
        if (null != equipItemarr)
        {
            // equipItemarr[0].InitData(hd.equipSite[]);
            for (int i = 0; i < equipItemarr.Length; i++)
            {
                equipItemarr[i].InitData(hd.equipSite[i+1]);

            }
        }
        selectFram.transform.parent =  equipItemarr[index].transform;
        selectFram.transform.localPosition = Vector3.zero;
        SetTabButton(table, true);
        HeroPanel.gameObject.SetActive(false);
    }
    public void ReshEquipItem()
    {
        if (null != equipItemarr&&this.hd!=null)
        {
            // equipItemarr[0].InitData(hd.equipSite[]);
            for (int i = 0; i < equipItemarr.Length; i++)
            {
                equipItemarr[i].InitData(this.hd.equipSite[i + 1]);

            }
        }
    }
    protected override void RegisterComponent()
    {
        base.RegisterComponent();
        
        RegisterComponentID(38, 120, BackBtn.gameObject);
       
    }
    protected override void RegisterIsOver()
    {
        base.RegisterIsOver();
    }
    public void OpenEquipEff(Dictionary<string, object> equipUp)
    {
        for (int i =0;i< equipEffectarr.Length;i++)
        {
            if (equipUp.ContainsKey((i+1).ToString()))
            {
                equipEffectarr[i].gameObject.SetActive(true);
            }
        }
        StartCoroutine(HideEquipEff());
    }
    IEnumerator HideEquipEff()
    {
        yield return new WaitForSeconds(1f);
        for (int i =0;i<equipEffectarr.Length;i++)
        {
            equipEffectarr[i].gameObject.SetActive(false);
        }
        //effEquipEff.gameObject.SetActive(false);
    }
    public void OpenEquipjinhuaEff(Dictionary<string, object> equipUp)
    {
        for (int i = 0; i < equipjinEffectarr.Length; i++)
        {
            if (equipUp.ContainsKey((i + 1).ToString()))
            {
                equipjinEffectarr[i].gameObject.SetActive(true);
            }
        }
        StartCoroutine(HideEquipjinhuaEff());
    }
    IEnumerator HideEquipjinhuaEff()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < equipjinEffectarr.Length; i++)
        {
            equipjinEffectarr[i].gameObject.SetActive(false);
        }
        //effEquipEff.gameObject.SetActive(false);
    }
}
