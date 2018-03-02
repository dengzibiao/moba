using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

/// <summary>
/// 副本任务对话框
/// </summary>
public class UIFubenTaskDialogue : GUIBase
{

    public delegate void OnDialogEnd();
    public OnDialogEnd DialogEnd;

    //对话Npc 名称
    public UILabel npcName;
    //玩家名称
    public UILabel playerName;
    //对话内容
    public UILabel ChatContont;
    public UILabel playerContont;

    public GameObject NextStepButton;

    public GameObject CloseButton;//跳过按钮事件

    private Transform NPCModelParent;
    private Transform PlayerModelParent;
    private Transform npcLine;
    private Transform playerLine;

    public GameObject npcmodel;
    private bool isNpc = true;//默认是npc 用于闲聊
    private bool isPlayer = false;
    private string npcNameStr;
    private string playerNameStr;
    private bool isSmalltalk = true;
    private bool isDialogueTask = false;
    private bool isBreath = false;
    private UISprite nextSprite;
    //对话内容
    public static List<string> contonts = new List<string>();

    public static int npcid;


    private int plotid;
    PlotLinesNode plotLinesNode;
    public static UIFubenTaskDialogue instance;
    public UIFubenTaskDialogue()
    {
        instance = this;
    }
    public void CreatPrefab(string name, float a, float b, float c, GameObject parent = null, string path = null)
    {
        Vector3 aa = new Vector3(a, b, c);
        if (aa != default(Vector3))
        {
            ScenesManage.Instance.CreatPrefab(name, parent, aa, path);
        }
        else
        {
            aa = default(Vector3);
            ScenesManage.Instance.CreatPrefab(name, parent, aa, path);
        }

    }
    GameObject HeroPosEmb = null;
    protected override void Init()
    {
        if (GameObject.Find("HeroPosEmbattle")==null)
        {
            CreatPrefab("HeroPosEmbattle", 10, 100, 0);
        }
        HeroPosEmb = GameObject.Find("HeroPosEmbattle");
        CloseButton = transform.Find("Close").gameObject;
        UIEventListener.Get(CloseButton).onClick = OnSkipClick;
        UIEventListener.Get(transform.gameObject).onClick = OnNextStep;
        npcName = transform.Find("NpcName").GetComponent<UILabel>();
        playerName = transform.Find("PlayerName").GetComponent<UILabel>();
        ChatContont = transform.Find("NpcDes").GetComponent<UILabel>();
        playerContont = transform.Find("PlayerDes").GetComponent<UILabel>();
        NPCModelParent = transform.Find("NpcGameObject");
        PlayerModelParent = transform.Find("PlayerGameObject");
        npcLine = transform.Find("Empty/NpcSprite");
        playerLine = transform.Find("Empty/PlayerSprite");
        nextSprite = transform.Find("NextSprite").GetComponent<UISprite>();

    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIFubenTaskDialogue;
    }
    void FixedUpdate()
    {
        StarBreathingLight();
    }
    /// <summary>
    /// 呼吸灯效果
    /// </summary>
    void StarBreathingLight()
    {
        if (isBreath)
        {
            nextSprite.alpha += 0.02f;

            if (nextSprite.alpha >= 1)
            {
                isBreath = false;
            }
        }
        else
        {
            nextSprite.alpha -= 0.02f;

            if (nextSprite.alpha <= 0.3f)
            {
                isBreath = true;
            }
        }
    }

    //下一步点击事件
    private void OnNextStep(GameObject go)
    {
        if (plotLinesNode.Nextplot == 0)
        {
            Destroy(npcmodel);
            Control.HideGUI(this.GetUIKey());
            //Hide();
            //告诉对话完成
            Debug.Log("对话完毕");
            HeroPosEmb.transform.Find("NpcPos").gameObject.SetActive(false);
            if (null != DialogEnd)
                DialogEnd();
            else
                Time.timeScale = 1;
        }
        else
        {
            SetData(plotLinesNode.Nextplot);
            RefreshData();
            //Control.ShowGUI(GameLibrary.UIFubenTaskDialogue);
        }

    }
    //跳过事件
    private void OnSkipClick(GameObject go)
    {
        Destroy(npcmodel);
        Control.HideGUI(this.GetUIKey());
        //Hide();
        HeroPosEmb.transform.Find("NpcPos").gameObject.SetActive(false);
        //告诉 对话完成
        if (null != DialogEnd)
            DialogEnd();
        else
            Time.timeScale = 1;
        Debug.Log("对话完毕");
    }
    /// <summary>
    /// 外部传入剧情台词表现表的序号
    /// </summary>
    /// <param name="id">剧情台词表现表的序号</param>
    public void SetData(int id)
    {
        plotid = id;
        if (FSDataNodeTable<PlotLinesNode>.GetSingleton().DataNodeList.ContainsKey(id))
        {
            plotLinesNode = FSDataNodeTable<PlotLinesNode>.GetSingleton().DataNodeList[id];
        }
    }
    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams != null && uiParams.Length > 0)
        {
            plotid = (int)uiParams[0];
            if (FSDataNodeTable<PlotLinesNode>.GetSingleton().DataNodeList.ContainsKey(plotid))
            {
                plotLinesNode = FSDataNodeTable<PlotLinesNode>.GetSingleton().DataNodeList[plotid];
            }
        }
        base.SetUI();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        this.State = EnumObjectState.Ready;
        Show();
    }
    void RefreshData()
    {
        Destroy(npcmodel);

        //一进来先看是不是玩家  要显示不同的内容 和 加载不同的模型
        if (plotLinesNode != null)
        {
            if (plotLinesNode.SpeakerType == 4)
            {
                isPlayer = true;
            }
            else
            {
                isPlayer = false;
            }
        }

        // playerName.transform.gameObject.SetActive(isPlayer ? true : false);
        // playerContont.transform.gameObject.SetActive(isPlayer ? true : false);
        //playerLine.transform.gameObject.SetActive(isPlayer ? true : false);

        //npcName.transform.gameObject.SetActive(!isPlayer ? true : false);
        //ChatContont.transform.gameObject.SetActive(!isPlayer ? true : false);
        //npcLine.transform.gameObject.SetActive(!isPlayer ? true : false);
        CrearteNpcModel(plotLinesNode);
    }
    protected override void ShowHandler()
    {
        RefreshData();
    }
    private GameObject heroObj;
    void CrearteNpcModel(PlotLinesNode node) //type 1:模型读取NPC模型表  2:模型读取英雄表  3:模型读取怪物表 4:玩家自己
    {
        string tem = "";
        if (node.SpeakerType == 1)//npc
        {
            if (FSDataNodeTable<NPCNode>.GetSingleton().DataNodeList.ContainsKey(long.Parse(node.SpeakerID)))
            {
                int modelID = FSDataNodeTable<NPCNode>.GetSingleton().DataNodeList[long.Parse(node.SpeakerID)].modelid;
                if (FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList.ContainsKey(modelID))
                {
                    tem = FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList[modelID].respath;
                    npcmodel = HeroPosEmbattle.instance.CreatModelByModelID((int)modelID, PosType.NpcPos, transform.Find("HeroTexture").GetComponent<SpinWithMouse>(), MountAndPet.Null, 160);
                }
                playerName.text = FSDataNodeTable<NPCNode>.GetSingleton().DataNodeList[long.Parse(node.SpeakerID)].npcname;
               
            }
           
        }
        else if (node.SpeakerType == 2)//英雄
        {
            if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(long.Parse(node.SpeakerID)))
            {
                tem = GameLibrary.Hero_URL + FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[long.Parse(node.SpeakerID)].icon_name + "_show";//传入英雄id
                npcmodel = HeroPosEmbattle.instance.CreatModelByModelID(FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[long.Parse(node.SpeakerID)].model, PosType.NpcPos, transform.Find("HeroTexture").GetComponent<SpinWithMouse>(), MountAndPet.Null, 160);
                //npcmodel = HeroPosEmbattle.instance.CreatModel(FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[long.Parse(node.SpeakerID)].icon_name + "_show", PosType.NpcPos, transform.Find("HeroTexture").GetComponent<SpinWithMouse>(), AnimType.None, 160);
            }
            playerName.text = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[long.Parse(node.SpeakerID)].name;
           
        }
        else if (node.SpeakerType == 3)//怪物
        {
            if (FSDataNodeTable<MonsterAttrNode>.GetSingleton().DataNodeList.ContainsKey(long.Parse(node.SpeakerID)))
            {
                tem = GameLibrary.Monster_URL + FSDataNodeTable<MonsterAttrNode>.GetSingleton().DataNodeList[long.Parse(node.SpeakerID)].icon_name;//传入怪物id
                playerName.text = FSDataNodeTable<MonsterAttrNode>.GetSingleton().DataNodeList[long.Parse(node.SpeakerID)].name;
                npcmodel = HeroPosEmbattle.instance.CreatModel(FSDataNodeTable<MonsterAttrNode>.GetSingleton().DataNodeList[long.Parse(node.SpeakerID)].icon_name, PosType.NpcPos, transform.Find("HeroTexture").GetComponent<SpinWithMouse>(), AnimType.None, 160);
            }

        }
        else if (node.SpeakerType == 4)//玩家自己
        {
            if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(long.Parse(GameLibrary.player.ToString())))
            {
                tem = GameLibrary.Hero_URL + FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[long.Parse(GameLibrary.player.ToString())].icon_name + "_show";
                npcmodel = HeroPosEmbattle.instance.CreatModelByModelID(FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[long.Parse(GameLibrary.player.ToString())].model, PosType.NpcPos, transform.Find("HeroTexture").GetComponent<SpinWithMouse>(), MountAndPet.Null, 160);
                //npcmodel = HeroPosEmbattle.instance.CreatModel(FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[long.Parse(GameLibrary.player.ToString())].icon_name + "_show", PosType.NpcPos, transform.Find("HeroTexture").GetComponent<SpinWithMouse>(), AnimType.None, 160);
            }
            playerName.text = playerData.GetInstance().selfData.playeName;
           
        }

        //ModelNode modelNode = FSDataNodeTable<ModelNode>.GetSingleton().FindDataByType(modelid);
        //GameObject obj = Resources.Load(tem) as GameObject;
        //if (obj != null)
        //{
        //    npcmodel = Instantiate(obj) as GameObject;
        //}
       
        if (npcmodel!=null)
        {
            if (node.SpeakerType == 1 || node.SpeakerType == 2 || node.SpeakerType == 3)
            {
                //npcmodel.transform.parent = PlayerModelParent;
                playerContont.text = node.Content.Replace("<selfname>", playerData.GetInstance().selfData.playeName);
                if (node.SpeakerType == 3)//如果是怪物 要去掉怪物身上的脚本
                {
                    if (npcmodel.GetComponent<PlayerMotion>() != null)
                    {
                        npcmodel.GetComponent<PlayerMotion>().enabled = false;
                    }
                    if (npcmodel.GetComponent<UnityEngine.AI.NavMeshAgent>() != null)
                    {
                        npcmodel.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
                    }
                }
            }
            else if (node.SpeakerType == 4)
            {
                //npcmodel.transform.parent = PlayerModelParent;
                playerContont.text = node.Content.Replace("<selfname>", playerData.GetInstance().selfData.playeName);
            }

            //npcmodel.transform.localPosition = Vector3.zero;
            //npcmodel.transform.localScale = Vector3.one;
            //npcmodel.transform.localRotation = Quaternion.Euler(Vector3.zero);
            //SkinnedMeshRenderer[] skinnedMeshRenderer = npcmodel.GetComponentsInChildren<SkinnedMeshRenderer>();
            //for (int i = 0; i < skinnedMeshRenderer.Length; i++)
            //{
            //    skinnedMeshRenderer[i].gameObject.layer = 5;
            //}
            //npcmodel.layer = 5;
            //foreach (Transform tran in npcmodel.transform.GetComponentInChildren<Transform>())
            //{
            //    tran.gameObject.layer = 5;
            //}
        }
        
    }


    /// <summary>
    /// 转换任务对话内容
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private string ConvertTaskDialogueDes(string str)
    {
        //StringUtil.StrReplace((SelectCard.selectCardId).ToString(), "201", 0, 3)
        int namePos = str.IndexOf("|");
        if (namePos > 0)
        {
            string temStr = str.Substring(0, namePos);
            if (temStr == "<selfname>")
            {
                isPlayer = true;
                isNpc = false;
                playerNameStr = playerData.GetInstance().selfData.playeName;
            }
            else
            {
                isNpc = true;
                isPlayer = false;
                npcNameStr = temStr;
            }
        }

        string str1 = StringUtil.StrRemove(str, 0, namePos + 1);//以除掉对话内容开头的npc名字 或玩家名字
        //除开头的名字，替换上玩家的名字  替换只能直接返回 不然不起作用
        return str1.Replace("<selfname>", playerData.GetInstance().selfData.playeName).Replace("c1", GameLibrary.C1).Replace("c2", GameLibrary.C2).Replace("c3", GameLibrary.C3).Replace("c4", GameLibrary.C4).Replace("c5", GameLibrary.C5).Replace("c6", GameLibrary.C6);//替换对话内容中的颜色
    }
}
