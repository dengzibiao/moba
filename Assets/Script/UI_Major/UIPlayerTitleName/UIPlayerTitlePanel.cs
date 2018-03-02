using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Tianyu;
/// <summary>
/// 玩家称号面板
/// </summary>
public class UIPlayerTitlePanel : GUIBase
{

    public GUISingleMultList titleMultList;
    public GUISingleCheckBoxGroup checkBoxs;
    private UIScrollView titleScrollView;
    public GUISingleButton backBtn;
    public UILabel promptLabel;
    public UILabel playerTitleName;
    private TweenPosition tweenP;
    private TweenAlpha tweenA;
    private Vector3 heroShowPos = new Vector3(-195, -210, -363);
    private GameObject heroObj;
    private Transform titleAttributeT;
    private UIGrid grid;
    private Transform nottitleDes;//无称号文字显示
    private UILabel powerLabel;//力量
    private UILabel intelligenceLabel;//智力
    private UILabel agilityLabel;//敏捷
    private UILabel hpLabel;//生命值
    private UILabel attackLabel;//攻击
    private UILabel armorLabel;//护甲
    private UILabel magic_resistLabel;//磨抗
    private UILabel criticalLabel;//暴击
    private UILabel dodgeLabel;//闪避
    private UILabel hitratioLabel;//命中
    private UILabel armorpenetrationLabel;//护甲穿刺
    private UILabel magicpenetrationLabel;//魔法穿刺
    private UILabel suckbloodLabel;//吸血
    private UILabel tenacityLabel;//韧性
    private UILabel mspeed;//移动速度
    private UILabel aspeed;//攻击速度
    private UILabel fanwei;//可视范围

    private TitleNode titleNode;//当前选择的称号信息 默认是玩家佩戴的

    public int firstPlayerTitleID;//起始玩家称号id （用于服务器同步）
    public int endPlayerTitleID = 0;//结束玩家称号id （用于服务器同步）
    public object[] Objs = new object[10];//系统
    public object[] Objs1 = new object[4];//系统

    public object[] allTitles;//全部
    public object[] growupTitles;//成长
    public object[] combatTitles;//战斗
    public object[] topupTitles;//充值
    public object[] activityTitles;//活动

    public List<TitleNode> allTitleList = new List<TitleNode>();
    public List<TitleNode> growupTitleList = new List<TitleNode>();
    public List<TitleNode> combatTitleList = new List<TitleNode>();
    public List<TitleNode> topupTitleList = new List<TitleNode>();
    public List<TitleNode> activityTitleList = new List<TitleNode>();

    GameObject HeroPosEmb = null;
    private static UIPlayerTitlePanel instance;
    public static UIPlayerTitlePanel Instance { get { return instance; } set { instance = value; } }
    public UIPlayerTitlePanel()
    {
        instance = this;
    }
    protected override void Init()
    {
        base.Init();
        instance = this;
        HeroPosEmb = GameObject.Find("HeroPosEmbattle");
        checkBoxs = transform.Find("CheckBoxs").GetComponent<GUISingleCheckBoxGroup>();
        titleMultList = transform.Find("ScrollView/MultList").GetComponent<GUISingleMultList>();
        backBtn = transform.Find("BackBtn").GetComponent<GUISingleButton>();
        titleScrollView = transform.Find("ScrollView").GetComponent<UIScrollView>();
        promptLabel = transform.Find("PromptLabel").GetComponent<UILabel>();
        playerTitleName = transform.Find("PlayerTitleName").GetComponent<UILabel>();
        tweenA = promptLabel.transform.GetComponent<TweenAlpha>();
        tweenP = promptLabel.transform.GetComponent<TweenPosition>();
        titleAttributeT = transform.Find("TitileAttribute");
        grid = transform.Find("TitileAttribute/Grid").GetComponent<UIGrid>();
        powerLabel = transform.Find("TitileAttribute/Grid/powerLabel").GetComponent<UILabel>();
        intelligenceLabel = transform.Find("TitileAttribute/Grid/intelligenceLabel").GetComponent<UILabel>();
        agilityLabel = transform.Find("TitileAttribute/Grid/agilityLabel").GetComponent<UILabel>();
        hpLabel = transform.Find("TitileAttribute/Grid/hpLabel").GetComponent<UILabel>();
        attackLabel = transform.Find("TitileAttribute/Grid/attackLabel").GetComponent<UILabel>();
        armorLabel = transform.Find("TitileAttribute/Grid/armorLabel").GetComponent<UILabel>();
        magic_resistLabel = transform.Find("TitileAttribute/Grid/magic_resistLabel").GetComponent<UILabel>();
        criticalLabel = transform.Find("TitileAttribute/Grid/criticalLabel").GetComponent<UILabel>();
        dodgeLabel = transform.Find("TitileAttribute/Grid/dodgeLabel").GetComponent<UILabel>();
        hitratioLabel = transform.Find("TitileAttribute/Grid/hitratioLabel").GetComponent<UILabel>();
        armorpenetrationLabel = transform.Find("TitileAttribute/Grid/armorpenetrationLabel").GetComponent<UILabel>();
        magicpenetrationLabel = transform.Find("TitileAttribute/Grid/magicpenetrationLabel").GetComponent<UILabel>();
        suckbloodLabel = transform.Find("TitileAttribute/Grid/suckbloodLabel").GetComponent<UILabel>();
        tenacityLabel = transform.Find("TitileAttribute/Grid/tenacityLabel").GetComponent<UILabel>();
        mspeed = transform.Find("TitileAttribute/Grid/mspeed").GetComponent<UILabel>();
        aspeed = transform.Find("TitileAttribute/Grid/aspeed").GetComponent<UILabel>();
        fanwei = transform.Find("TitileAttribute/Grid/fanwei").GetComponent<UILabel>();
        nottitleDes = transform.Find("NotTitleDes");
        backBtn.onClick = OnBackClick;
        checkBoxs.onClick = OnTitleClick;

        //InitTitleData();
        //checkBoxs.DefauleIndex = 0;
        //OnTitleClick(0, true); 
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    /// <summary>
    /// 返回按钮事件
    /// </summary>
    private void OnBackClick()
    {
        Globe.selectTitleIndex = -1;
        HeroPosEmb.transform.Find("TitlePos").gameObject.SetActive(false);
        if (heroObj != null)
        {
            Destroy(heroObj);
        }
        SetMainHeroName.Instance.RefreshTitleName();
        HeroPosEmbattle.instance.HideModel();
        //返回时候同步消息
        // 最后点击的是激活的称号 并且发生替换  才同步
        //if (playerData.GetInstance().selfData.playerTitleDic.ContainsKey(endPlayerTitleID) && endPlayerTitleID != playerData.GetInstance().selfData.playerTitleId)
        //{
        //    //向服务器发送 endPlayerTitleID
        //    playerData.GetInstance().selfData.playerTitleId = endPlayerTitleID;
        //    if (FSDataNodeTable<TitleNode>.GetSingleton().DataNodeList.ContainsKey(endPlayerTitleID))
        //    {
        //        playerData.GetInstance().selfData.playerTitleName = FSDataNodeTable<TitleNode>.GetSingleton().DataNodeList[endPlayerTitleID].titleiconname;
        //    }
        //    Debug.Log("同步称号");

        //}
        //最后点击的是未激活称号 或者最后点击称号跟最初的一样 都不同步
        //UI_Role.Instance.mask.gameObject.SetActive(false);
        UIRoleInfo.instance.gameObject.SetActive(true);
        Hide();
    }
    /// <summary>
    /// 提示消息接口
    /// </summary>
    /// <param name="s"></param>
    public void ShowPrompt(string s)
    {
        promptLabel.color = Color.green;
        promptLabel.text = s;
        tweenP.ResetToBeginning();
        tweenA.ResetToBeginning();
        tweenP.PlayForward();
        tweenA.PlayForward();
    }
    private void OnTitleClick(int index, bool boo)
    {

        if (boo)
        {
            checkBoxs.transform.GetChild(index).Find("Label").GetComponent<UILabel>().color = Color.white;
            Globe.selectTitleIndex = -1;//称号默认不选中
            Globe.selectTitleTypeIndex = index;
            showLabel(index);
            switch (index)
            {
                case 0:
                    titleMultList.InSize(allTitles.Length, 1);
                    titleMultList.Info(allTitles);
                    titleScrollView.ResetPosition();
                    break;
                case 1:
                    titleMultList.InSize(growupTitles.Length, 1);
                    titleMultList.Info(growupTitles);
                    titleScrollView.ResetPosition();
                    break;
                case 2:
                    titleMultList.InSize(combatTitles.Length, 1);
                    titleMultList.Info(combatTitles);
                    titleScrollView.ResetPosition();
                    break;
                case 3:
                    titleMultList.InSize(topupTitles.Length, 1);
                    titleMultList.Info(topupTitles);
                    titleScrollView.ResetPosition();
                    break;
                case 4:
                    titleMultList.InSize(activityTitles.Length, 1);
                    titleMultList.Info(activityTitles);
                    titleScrollView.ResetPosition();
                    break;
                default:
                    break;
            }
        }
    }
    void showLabel(int index)
    {
        switch (index)
        {
            case 0:
                nottitleDes.gameObject.SetActive(allTitles.Length>0?false:true);
                break;
            case 1:
                nottitleDes.gameObject.SetActive(growupTitles.Length > 0 ? false : true);
                break;
            case 2:
                nottitleDes.gameObject.SetActive(combatTitles.Length > 0 ? false : true);
                break;
            case 3:
                nottitleDes.gameObject.SetActive(topupTitles.Length > 0 ? false : true);
                break;
            case 4:
                nottitleDes.gameObject.SetActive(activityTitles.Length > 0 ? false : true);
                break;
            default:
                break;
        }
    }
    protected override void ShowHandler()
    {
        base.ShowHandler();
        InitTitleData();
        checkBoxs.DefauleIndex = 0;
        OnTitleClick(0, true); 

        firstPlayerTitleID = playerData.GetInstance().selfData.playerTitleId;
        if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(GameLibrary.player))
        {
            HeroNode heroNode = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[GameLibrary.player];
            InsHero(heroNode);
        }
        SetPlayerTitleName(playerData.GetInstance().selfData.playerTitleName);
        SetTitleAttribute(playerData.GetInstance().selfData.playerTitleId);
    }
    /// <summary>
    ///  更换称号后 刷新界面显示
    /// </summary>
    public void RefreshPanel()
    {
        InitTitleData();
        SetPlayerTitleName(playerData.GetInstance().selfData.playerTitleName);
        SetTitleAttribute(playerData.GetInstance().selfData.playerTitleId);
        checkBoxs.DefauleIndex = Globe.selectTitleTypeIndex;
        OnTitleClick(Globe.selectTitleTypeIndex, true);
    }
    /// <summary>
    /// 初始化称号信息
    /// </summary>
    public void InitTitleData()
    {
        allTitleList.Clear();
        growupTitleList.Clear();
        combatTitleList.Clear();
        topupTitleList.Clear();
        activityTitleList.Clear();
        //称号类型 0：成长；1：战斗；2：充值；
        List<TitleNode> allActiveList = new List<TitleNode>();//临时用于保存已经被激活称号 方便插入到列表开头
        List<TitleNode> growupActiveList = new List<TitleNode>();
        List<TitleNode> combatActiveList = new List<TitleNode>();
        List<TitleNode> topupActiveList = new List<TitleNode>();
        Dictionary<long, TitleNode> titleDic = FSDataNodeTable<TitleNode>.GetSingleton().DataNodeList;
        foreach (TitleNode val in titleDic.Values)
        {
            //将玩家已经佩戴的称号放到列表第一位 然后是激活的称号 然后是未激活的称号
            if (val.titleid == playerData.GetInstance().selfData.playerTitleId)
            {
                allActiveList.Insert(0, val);
                //allTitleList.Insert(0, val);
            }
            else if (playerData.GetInstance().selfData.playerTitleDic.ContainsKey(val.titleid))
            {
                allActiveList.Add(val);
               
            }
            else
            {
                allTitleList.Add(val);
            }
            
            if (val.type == 0)
            {
                if (val.titleid == playerData.GetInstance().selfData.playerTitleId)
                {
                    growupActiveList.Insert(0, val);

                }
                else if (playerData.GetInstance().selfData.playerTitleDic.ContainsKey(val.titleid))
                {
                    growupActiveList.Add(val);
                   
                }
                else
                {
                    growupTitleList.Add(val);
                }
            }
            
            if (val.type == 1)
            {
                if (val.titleid == playerData.GetInstance().selfData.playerTitleId)
                {
                    combatActiveList.Insert(0, val);
                }
                else if (playerData.GetInstance().selfData.playerTitleDic.ContainsKey(val.titleid))
                {
                    combatActiveList.Add(val);
                    
                }
                else
                {
                    combatTitleList.Add(val);
                }
            }
           
            if (val.type == 2)
            {
                if (val.titleid == playerData.GetInstance().selfData.playerTitleId)
                {
                    topupActiveList.Insert(0, val);
                }
                else if (playerData.GetInstance().selfData.playerTitleDic.ContainsKey(val.titleid))
                {
                    topupActiveList.Add(val);

                }
                else
                {
                    topupTitleList.Add(val);
                }
            }
        }
        allTitleList.InsertRange(0, allActiveList);
        growupTitleList.InsertRange(0, growupActiveList);
        combatTitleList.InsertRange(0, combatActiveList);
        topupTitleList.InsertRange(0, topupActiveList);

        //临时数组用完删除
        allActiveList = null;
        growupActiveList = null;
        combatActiveList = null;
        topupActiveList = null;
        //Dictionary<long, TitleNode> titleDic = FSDataNodeTable<TitleNode>.GetSingleton().DataNodeList;
        //foreach (TitleNode val in titleDic.Values)
        //{
        //    //将玩家已经佩戴的称号放到列表第一位
        //    if (val.titleid == playerData.GetInstance().selfData.playerTitleId)
        //    {
        //        allTitleList.Insert(0, val);
        //    }
        //    else
        //    {
        //        allTitleList.Add(val);
        //    }

        //    if (val.type == 0)
        //    {
        //        if (val.titleid == playerData.GetInstance().selfData.playerTitleId)
        //        {
        //            growupTitleList.Insert(0, val);
        //        }
        //        else
        //        {
        //            growupTitleList.Add(val);
        //        }
        //    }
        //    if (val.type == 1)
        //    {
        //        if (val.titleid == playerData.GetInstance().selfData.playerTitleId)
        //        {
        //            combatTitleList.Insert(0, val);
        //        }
        //        else
        //        {
        //            combatTitleList.Add(val);
        //        }
        //    }
        //    if (val.type == 2)
        //    {
        //        if (val.titleid == playerData.GetInstance().selfData.playerTitleId)
        //        {
        //            topupTitleList.Insert(0, val);
        //        }
        //        else
        //        {
        //            topupTitleList.Add(val);
        //        }
        //    }
        //}

        allTitles = allTitleList.ToArray();
        growupTitles = growupTitleList.ToArray();
        combatTitles = combatTitleList.ToArray();
        topupTitles = topupTitleList.ToArray();
        activityTitles = activityTitleList.ToArray();
    }
    /// <summary>
    /// 设置左侧称号图像展示
    /// </summary>
    /// <param name="name"></param>
    public void SetPlayerTitleName(string name)
    {
        playerTitleName.text = name;
    }
    /// <summary>
    /// 实例化英雄展示模型
    /// </summary>
    public void InsHero(HeroNode insHero)
    {
        heroObj = HeroPosEmbattle.instance.CreatModel(insHero.icon_name+"_show", PosType.TitlePos, transform.Find("HeroTexture").GetComponent<SpinWithMouse>());
    }
    /// <summary>
    /// 设置称号属性展示
    /// </summary>
    /// <param name="id"></param>
    public void SetTitleAttribute(int id)
    {
        //"[00ff00]" + "20" +"[-]" 
        if (id == 0)
        {
            titleAttributeT.gameObject.SetActive(false);
            return;
        }
        else
        {
            titleAttributeT.gameObject.SetActive(true);
        }
        if (FSDataNodeTable<TitleNode>.GetSingleton().DataNodeList.ContainsKey(id))
        {
            titleNode = FSDataNodeTable<TitleNode>.GetSingleton().DataNodeList[id];
            powerLabel.text = "力量:" + "[00ff00]" + "+" + titleNode.power + "[-]";
            intelligenceLabel.text = "智力:" + "[00ff00]" + "+" + titleNode.intelligence + "[-]";
            agilityLabel.text = "敏捷:" + "[00ff00]" + "+" + titleNode.agility + "[-]";
            hpLabel.text = "生命值:" + "[00ff00]" + "+" + titleNode.hp + "[-]";
            attackLabel.text = "攻击:" + "[00ff00]" + "+" + titleNode.attack + "[-]";
            armorLabel.text = "护甲:" + "[00ff00]" + "+" + titleNode.armor + "[-]";
            magic_resistLabel.text = "磨抗:" + "[00ff00]" + "+" + titleNode.magicresist + "[-]";
            criticalLabel.text = "暴击:" + "[00ff00]" + "+" + titleNode.critical + "[-]";
            dodgeLabel.text = "闪避:" + "[00ff00]" + "+" + titleNode.dodge + "[-]";
            hitratioLabel.text = "命中:"  + "[00ff00]" + "+" + titleNode.hitratio + "[-]";
            armorpenetrationLabel.text = "护甲穿透:" + "[00ff00]" + "+" + titleNode.armorpenetration + "[-]";
            magicpenetrationLabel.text = "魔法穿透:" + "[00ff00]" + "+" + titleNode.magic_penetration + "[-]";
            suckbloodLabel.text = "吸血:" + "[00ff00]" + "+" + titleNode.suck_blood + "[-]";
            tenacityLabel.text = "韧性:" + "[00ff00]" + "+" + titleNode.tenacity + "[-]";
            mspeed.text= "移动速度:" + "[00ff00]" + "+" + titleNode.movementspeed + "[-]";
            aspeed.text = "攻击速度:" + "[00ff00]" + "+" + titleNode.attackspeed + "[-]";
            fanwei.text = "可视范围:" + "[00ff00]" + "+" + titleNode.strikingdistance + "[-]";
            powerLabel.gameObject.SetActive(titleNode.power > 0 ? true : false);
            intelligenceLabel.gameObject.SetActive(titleNode.intelligence > 0 ? true : false);
            agilityLabel.gameObject.SetActive(titleNode.agility > 0 ? true : false);
            hpLabel.gameObject.SetActive(titleNode.hp > 0 ? true : false);
            attackLabel.gameObject.SetActive(titleNode.attack > 0 ? true : false);
            armorLabel.gameObject.SetActive(titleNode.armor > 0 ? true : false);
            magic_resistLabel.gameObject.SetActive(titleNode.magicresist > 0 ? true : false);
            criticalLabel.gameObject.SetActive(titleNode.critical > 0 ? true : false);
            dodgeLabel.gameObject.SetActive(titleNode.dodge > 0 ? true : false);
            hitratioLabel.gameObject.SetActive(titleNode.hitratio > 0 ? true : false);
            armorpenetrationLabel.gameObject.SetActive(titleNode.armorpenetration > 0 ? true : false);
            magicpenetrationLabel.gameObject.SetActive(titleNode.magic_penetration > 0 ? true : false);
            suckbloodLabel.gameObject.SetActive(titleNode.suck_blood > 0 ? true : false);
            tenacityLabel.gameObject.SetActive(titleNode.tenacity > 0 ? true : false);
            mspeed.gameObject.SetActive(titleNode.movementspeed > 0 ? true : false);
            aspeed.gameObject.SetActive(titleNode.attackspeed > 0 ? true : false);
            fanwei.gameObject.SetActive(titleNode.strikingdistance > 0 ? true : false);
            grid.Reposition();
        }
        
    }
}
