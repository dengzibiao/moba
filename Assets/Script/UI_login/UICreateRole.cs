/*

王


*/
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Tianyu;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class UICreateRole : GUIBase
{
    public GUISingleButton backBtn;
    public GUISingleButton sureBtn;
    public GUISingleButton randomBtn;
    public GUISingleInput nicknameInput;
    public GUISingleCheckBoxGroup checkBoxs;
    public GUISingleMultList skillMultList;
    public GUISingleLabel playerDes;
    private UILabel heroName;
    private UILabel des;
    private UIToggle pai1;
    private UIToggle pai2;
    private UIToggle pai3;
    public Transform selectRoleSceneObj;
    public Transform skillPanel;

    private Vector3 heroShowPos = new Vector3(5, -226, 0);//-363
    private GameObject heroObj;
    HeroData hd = null;                          //英雄信息
    //技能id列表
    private List<int> skillIDList = new List<int>();

    //英雄技能数据
    private List<SkillData> skilldata = new List<SkillData>();
    private object[] login;
    private GUISingleCheckBox[] boxs;
    public static UICreateRole instance;

    private UIGrid dingweigrid;
    private UIGrid tediangrid;
    public Transform[] dingweiT;
    public Transform[] tedianT;

    GameObject HeroPosEmb = null;

    SpinWithMouse spinWithMouse;
    BoxCollider collder;

    private PlayerNameNode playerName;
    // public Dictionary<long, PlayerNameNode> nodeDic;
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }

    protected override void Init()
    {
        instance = this;
        HeroPosEmb = GameObject.Find("HeroPosEmbattle");
        dingweigrid = transform.Find("DingweiContainer/Grid").GetComponent<UIGrid>();
        tediangrid = transform.Find("TedianContainer/Grid").GetComponent<UIGrid>();
        login = VOManager.Instance().GetCSV<LoginCSV>("Login").GetVoList();
        sureBtn.onClick = OnSureBtn;
        //randomBtn.onClick = OnRandomClick;
        pai1 = transform.Find("pai1").GetComponent<UIToggle>();
        pai2 = transform.Find("pai2").GetComponent<UIToggle>();
        pai3 = transform.Find("pai3").GetComponent<UIToggle>();
        skillMultList = transform.Find("ScrollView/MultList").GetComponent<GUISingleMultList>();
        skillPanel = transform.Find("SkillDescribePanel");
        heroName = transform.Find("Name").GetComponent<UILabel>();
        des = transform.Find("ScrollViewLabel/Des").GetComponent<UILabel>();
        checkBoxs.onClick = OnCreateRoleClick;
        boxs = checkBoxs.GetBoxList();
        //for (int i = 0; i < boxs.Length; i++)
        //{
        //    boxs[i].GetComponent<UISprite>().spriteName = ((LoginVO)login[i + 1]).card_name.Trim();
        //}
        //checkBoxs.onHover = OnCreateRoleHover;
        //selectRoleSceneObj = GameObject.Find("Xuanren").transform;
        EventDelegate ed2 = new EventDelegate(this, "OnSelectCard");
        pai1.onChange.Add(ed2);
        pai2.onChange.Add(ed2);
        pai3.onChange.Add(ed2);
        backBtn.onClick = OnBackClick;
        //if (FSDataNodeTable<PlayerNameNode>.GetSingleton().DataNodeList.Count > 0)
        //{
        //    nodeDic = FSDataNodeTable<PlayerNameNode>.GetSingleton().DataNodeList;
        //    foreach (var nameLists in nodeDic)
        //    {

        //        playerData.GetInstance().selfData.nameList.Add(nameLists.Value);
        //    }

        //}
        InitNameList();
        //EventDelegate.Add(nicknameInput.GetComponent<UIInput>().onChange, OnChange);
        spinWithMouse = transform.Find("HeroTexture").GetComponent<SpinWithMouse>();
        collder = transform.transform.Find("HeroTexture").GetComponent<BoxCollider>();
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UI_CreateRole;
    }

    static void InitNameList ()
    {
        if(playerData.GetInstance().selfData.nameList.Count > 0)
            return;
        if(FSDataNodeTable<PlayerNameNode>.GetSingleton().DataNodeList.Count > 0)
        {
            // Dictionary<long, PlayerNameNode> nodeDic = FSDataNodeTable<PlayerNameNode>.GetSingleton().DataNodeList;
            foreach(var nameLists in FSDataNodeTable<PlayerNameNode>.GetSingleton().DataNodeList)
            {
                playerData.GetInstance().selfData.nameList.Add(nameLists.Value);
            }
        }
    }

    private void OnBackClick()
    {
        checkBoxs.setMaskState(0);
        transform.parent.Find("Panel").gameObject.SetActive(true);
        Hide();
        Control.HideGUI(this.GetUIKey());
        Control.ShowGUI(UIPanleID.UI_SelectServer, EnumOpenUIType.DefaultUIOrSecond);
        //返回必须重置开始状态，否则登录不了
        UISelectServer.Instance.ResetIsStart(false);
    }

    private void OnCreateRoleHover(int index, bool boo)
    {
        if (boo)
        {
            checkBoxs.transform.GetChild(index).localScale = new Vector3(1.05f, 1.05f, 1.05f);
        }
        else
        {
            checkBoxs.transform.GetChild(index).localScale = new Vector3(1f, 1f, 1f);
        }

    }

    private void OnCreateRoleClick(int index, bool boo)
    {
        int heroId;
        if (boo)
        {
            switch (index)
            {
                case 0:
                    SelectCard.selectCardName = ((LoginVO)login[3]).card_name.Trim(); ;
                    SelectCard.selectCardId = ((LoginVO)login[3]).card_id;
                    InsHero("yx_010");
                    heroId = int.Parse(StringUtil.StrReplace((SelectCard.selectCardId).ToString(), "201", 0, 3));
                    heroName.text = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].name;
                    des.text = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].info;
                    //playerDes.text = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].info;
                    InitSkillData(int.Parse(StringUtil.StrReplace((SelectCard.selectCardId).ToString(), "201", 0, 3)));
                    SetDingweiAndTedian(heroId);
                    skillMultList.InSize(4, 4);
                    skillMultList.Info(skilldata.ToArray());
                    //checkBoxs.HideCheckBoxSprite(0);
                    break;
                case 1:
                    SelectCard.selectCardName = ((LoginVO)login[1]).card_name.Trim(); ;
                    SelectCard.selectCardId = ((LoginVO)login[1]).card_id;
                    InsHero("yx_002");
                    heroId = int.Parse(StringUtil.StrReplace((SelectCard.selectCardId).ToString(), "201", 0, 3));
                    heroName.text = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].name;
                    des.text = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].info;
                    //playerDes.text = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].info;
                    InitSkillData(heroId);
                    SetDingweiAndTedian(heroId);
                    skillMultList.InSize(4, 4);
                    skillMultList.Info(skilldata.ToArray());
                    //checkBoxs.HideCheckBoxSprite(1);
                    break;
                case 2:
                    SelectCard.selectCardName = ((LoginVO)login[2]).card_name.Trim(); ;
                    SelectCard.selectCardId = ((LoginVO)login[2]).card_id;
                    InsHero("yx_014");
                    heroId = int.Parse(StringUtil.StrReplace((SelectCard.selectCardId).ToString(), "201", 0, 3));
                    heroName.text = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].name;
                    des.text = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].info;
                    //playerDes.text = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].info;
                    InitSkillData(int.Parse(StringUtil.StrReplace((SelectCard.selectCardId).ToString(), "201", 0, 3)));
                    SetDingweiAndTedian(heroId);
                    skillMultList.InSize(4, 4);
                    skillMultList.Info(skilldata.ToArray());
                    //checkBoxs.HideCheckBoxSprite(2);
                    break;
                default:
                    break;
            }
        }
    }

    protected override void ShowHandler()
    {
        transform.parent.Find("Panel").gameObject.SetActive(false);
        OnCreateRoleClick(0, true);
    }
    /// <summary>
    /// input改变事件
    /// </summary>
    private void OnChange()
    {
        if (nicknameInput.text.Length > 14)//f (Encoding.Default.GetBytes(nicknameInput.text).Length >14)
        {
            //UIPromptBox.Instance.ShowLabel("角色名称最多14个字符~");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "角色名称最多14个字符~");
            nicknameInput.text = nicknameInput.text.Substring(0, 7);
        }
    }
    /// <summary>
    /// 恢复创建角色按钮的操作，用户名重复等与服务器通讯的操作，等待服务器回复后重置状态
    /// </summary>
    public void EnterGame()
    {
        sureBtn.isEnabled =true;
        backBtn.isEnabled = true;
    }

    void OnSureBtn()
    {

        GameLibrary.dungeonId = 10000;
        string heroId = StringUtil.StrReplace((SelectCard.selectCardId).ToString(), "201", 0, 3);
        GameLibrary.player = long.Parse(heroId);
        Globe.isFightGuide = true;
        //UI_Loading.LoadScene("xinshou", 3);
        GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
        SceneNode sceneNode = FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(GameLibrary.dungeonId);
        serverMgr.GetInstance().SetCreateAccountTime("", true);
        if (null != sceneNode)
        {
            StartLandingShuJu.GetInstance().GetLoadingData(sceneNode.MapName, 3);
            SceneManager.LoadScene("Loding");
        }

        return;

        if (string.IsNullOrEmpty(nicknameInput.text))
        {
            //UIPromptBox.Instance.ShowLabel("请输入昵称!");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "请输入昵称!");
            return;
        }
        if (Regex.IsMatch(nicknameInput.text, @"^\d"))
        {
            //UIPromptBox.Instance.ShowLabel("不能以数字开头");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "不能以数字开头");
            return;
        }
        if (Regex.IsMatch(nicknameInput.text, @"\s+"))
        {
            //UIPromptBox.Instance.ShowLabel("不能有空格");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "不能有空格");
            return;
        }
        if (Encoding.Default.GetBytes(nicknameInput.text).Length < 4)
        {
            //UIPromptBox.Instance.ShowLabel("角色名称至少要4个字符哟~");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "角色名称至少要4个字符哟~");
            return;
        }
        if (nicknameInput.text.Length > 14)//if (Encoding.Default.GetBytes(nicknameInput.text).Length > 14)
        {
            //UIPromptBox.Instance.ShowLabel("角色名称最多14个字符~");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "角色名称最多14个字符~");
            return;
        }
        sureBtn.isEnabled = false;
        backBtn.isEnabled = false;
        GameLibrary.nickName = nicknameInput.text;
        //switch (SelectCard.selectCardName.Substring(3))
        //{
        //    case "Jiansheng":
        //        GameLibrary.player = 201001100;
        //        break;
        //    case "Huonv":
        //        GameLibrary.player = 201001400;
        //        break;
        //    case "Xiongmao":
        //        GameLibrary.player = 201001000;
        //        break;
        //}

        //GameLibrary.player = SelectCard.selectCardName.Substring( 3 );

        //UI_Loading.LoadScene( GameLibrary.UI_Major , 3 );
        if (!ClientNetMgr.GetSingle().IsConnect())//如果网络没有连接连下
        {
            ClientNetMgr.GetSingle().StartConnect(Globe.SelectedServer.ip, Globe.SelectedServer.port);
        }
        
        //初始默认上阵英雄
        //Globe.playHeroList[0] = playerData.GetInstance().FindOrNewHeroDataById(long.Parse(heroId));
        ClientSendDataMgr.GetSingle().GetLoginSend().SendCreateRole(nicknameInput.text, heroId, Globe.SelectedServer.areaId.ToString());
        //ClientSendDataMgr.GetSingle().GetLoginSend().SendCreateRole(nicknameInput.text, heroId, SelectCard.selectAreaId);

        serverMgr.GetInstance().SetName(nicknameInput.text);
        //serverMgr.GetInstance().SetCurrentPoint(UI_HeroDetail.skillCount);
        //serverMgr.GetInstance().SetFullTime(Auxiliary.GetNowTime() + (20 - UI_HeroDetail.skillCount) * 600);
        serverMgr.GetInstance().saveData();
        //this.gameObject.SetActive( false );

    }


    public void DestoryObj()
    {
        if (heroObj != null)
        {
            Destroy(heroObj);
        }
    }
    void OnRandomClick()
    {
        nicknameInput.text = GetRandomName();
    }

    public static string GetRandomName ()
    {
        InitNameList();
        int nameIndex = 0;
        int signIndex = 0;
        int surNameIndex = 0;
        for(int i = 0; i < playerData.GetInstance().selfData.nameList.Count; i++)
        {
            if(playerData.GetInstance().selfData.nameList[i].name != null || playerData.GetInstance().selfData.nameList[i].sign != null || playerData.GetInstance().selfData.nameList[i].surname != null)
            {
                nameIndex = Random.Range(0, playerData.GetInstance().selfData.nameList.Count);
                signIndex = Random.Range(0, playerData.GetInstance().selfData.nameList.Count);
                surNameIndex = Random.Range(0, playerData.GetInstance().selfData.nameList.Count);
            }
        }
        return playerData.GetInstance().selfData.nameList[nameIndex].name + playerData.GetInstance().selfData.nameList[signIndex].sign + playerData.GetInstance().selfData.nameList[surNameIndex].surname;
    }

    void OnSelectCard()
    {
        int heroId;
        if (pai1.value)
        {
            SelectCard.selectCardName = pai1.GetComponent<UISprite>().spriteName;
            SelectCard.selectCardId = ((LoginVO)login[1]).card_id;
            InsHero("yx_011");
            heroId = int.Parse(StringUtil.StrReplace((SelectCard.selectCardId).ToString(), "201", 0, 3));
            heroName.text = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].name;
            des.text = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].describe;
            SetDingweiAndTedian(heroId);
            InitSkillData(heroId);
            skillMultList.InSize(4, 2);
            skillMultList.Info(skilldata.ToArray());
        }
        else if (pai2.value)
        {
            SelectCard.selectCardName = pai2.GetComponent<UISprite>().spriteName;
            SelectCard.selectCardId = ((LoginVO)login[2]).card_id;
            InsHero("yx_014");
            heroId = int.Parse(StringUtil.StrReplace((SelectCard.selectCardId).ToString(), "201", 0, 3));
            heroName.text = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].name;
            des.text = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].describe;
            InitSkillData(int.Parse(StringUtil.StrReplace((SelectCard.selectCardId).ToString(), "201", 0, 3)));
            SetDingweiAndTedian(int.Parse(StringUtil.StrReplace((SelectCard.selectCardId).ToString(), "201", 0, 3)));
            skillMultList.InSize(4, 2);
            skillMultList.Info(skilldata.ToArray());
        }
        else if (pai3.value)
        {
            SelectCard.selectCardName = pai3.GetComponent<UISprite>().spriteName;
            SelectCard.selectCardId = ((LoginVO)login[3]).card_id;
            InsHero("yx_010");
            heroId = int.Parse(StringUtil.StrReplace((SelectCard.selectCardId).ToString(), "201", 0, 3));
            heroName.text = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].name;
            des.text = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].describe;
            InitSkillData(int.Parse(StringUtil.StrReplace((SelectCard.selectCardId).ToString(), "201", 0, 3)));
            SetDingweiAndTedian(int.Parse(StringUtil.StrReplace((SelectCard.selectCardId).ToString(), "201", 0, 3)));
            skillMultList.InSize(4, 2);
            skillMultList.Info(skilldata.ToArray());
        }
    }

    /// <summary>
    /// 初始化技能数据
    /// </summary>
    public void InitSkillData(long selectId)
    {

        //hd = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);

        skillIDList.Clear();
        skilldata.Clear();

        long[] skillid = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[selectId].skill_id;

        for (int i = 3; i < skillid.Length; i++)
        {
            skillIDList.Add(int.Parse(skillid[i].ToString()));
        }
        Dictionary<long, SkillNode> skillList = FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList;

        for (int i = 0; i < 4; i++)
        {

            SkillData data = new SkillData();
            data.name = skillList[skillIDList[i]].skill_name;
            data.icon = skillList[skillIDList[i]].skill_icon;//GameLibrary.skillLevel[skillIDList[i]]
            data.skillId = skillIDList[i];
            data.des = FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList[skillIDList[i]].des;
            skilldata.Add(data);
        }
    }

    void InsHero(string name)
    {
        spinWithMouse.enabled = false;
        collder.enabled = false;
        HeroPosEmbattle.instance.CreatModel(name+"_show", PosType.SelectPos, spinWithMouse, AnimType.Appeared);
    }

    public void SetSpinWithMouse()
    {
        spinWithMouse.enabled = true;
        collder.enabled = true;
    }
    /// <summary>
    /// 设置定位 特点图标
    /// </summary>
    void SetDingweiAndTedian(long selectId)
    {
        //英雄定位 1：战士；2：肉盾；3：刺客；4：射手；5：法师；6：辅助；
        //英雄特点 0：先手；1：突进；2：爆发；3：吸血；4：团控；5：收割；6：控制；7：推进；8：消耗；9：恢复；10：增益；
        if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(selectId))
        {
            HeroNode node = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[selectId];
            int[] dingwei = node.dingwei;
            int[] tedian = node.characteristic;
            for (int j = 0; j < dingweiT.Length; j++)
            {
                dingweiT[j].gameObject.SetActive(false);
            }
            for (int j = 0; j < tedianT.Length; j++)
            {
                tedianT[j].gameObject.SetActive(false);
            }
            if (dingwei != null)
            {
                for (int i = 0; i < dingwei.Length; i++)
                {
                    for (int j = 0; j < dingweiT.Length; j++)
                    {
                        if ((dingwei[i] - 1) == j)
                        {
                            dingweiT[j].gameObject.SetActive(true);
                            break;
                        }
                    }
                }
            }
            if (tedian != null)
            {
                for (int i = 0; i < tedian.Length; i++)
                {
                    for (int j = 0; j < tedianT.Length; j++)
                    {
                        if ((tedian[i]) == j)
                        {
                            tedianT[j].gameObject.SetActive(true);
                            break;
                        }
                    }
                }
            }
            dingweigrid.Reposition();
            tediangrid.Reposition();
        }
    }

}

public struct SelectCard
{

    public static string selectCardName;

    public static long selectCardId;

    public static string selectAreaId;

}