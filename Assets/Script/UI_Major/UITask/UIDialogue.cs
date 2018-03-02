/*
文件名（File Name）:   UIDialogue.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;

public delegate void OnChatFinish(long npcid);
public class UIDialogue : GUIBase
{
    //对话执行完事件
    public static OnChatFinish OnChatFinishEvent;

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

    public Transform acceptBtn;
    public Transform completeBtn;
    public GameObject npcmodel;
    private bool isNpc = true;//默认是npc 用于闲聊
    private bool isPlayer = false;
    private string npcNameStr;
    private string playerNameStr;
    private bool isSmalltalk = true;
    private bool isDialogueTask = false;
    private bool isBreath = false;
    private UISprite nextSprite;
    GameObject HeroPosEmb = null;
    //对话内容
    public static List<string> contonts = new List<string>();

    public static long npcid;
    public static UIDialogue instance;
    public UIDialogue()
    {
        instance = this;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="a">是否是闲聊</param>
    /// <param name="b">是否是剧情任务</param>
    public void SetData(bool a,bool b)
    {
        isSmalltalk = a;
        isDialogueTask = b;
    }
    protected override void Init()
    {
        HeroPosEmb = GameObject.Find("HeroPosEmbattle");
        UIEventListener.Get(CloseButton).onClick = OnSkipClick;
        UIEventListener.Get(NextStepButton).onClick = OnNextStep;
        npcName = transform.Find("NpcName").GetComponent<UILabel>();
        playerName = transform.Find("PlayerName").GetComponent<UILabel>();
        ChatContont = transform.Find("NpcDes").GetComponent<UILabel>();
        playerContont = transform.Find("PlayerDes").GetComponent<UILabel>();
        NPCModelParent = transform.Find("NpcGameObject");
        PlayerModelParent = transform.Find("PlayerGameObject");
        npcLine = transform.Find("Empty/NpcSprite");
        playerLine = transform.Find("Empty/PlayerSprite");
        nextSprite = transform.Find("NextSprite").GetComponent<UISprite>();
        acceptBtn = transform.Find("AcceptBtn");
        completeBtn = transform.Find("CompleteBtn");
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIDialogue;
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
    //protected override void SetUI(params object[] uiParams)
    //{
    //    isSmalltalk = (bool)uiParams[0];
    //    isDialogueTask = (bool)uiParams[1];
    //    npcid = (long)uiParams[2];
    //    contonts.Add((string)uiParams[3]);
    //    base.SetUI();
    //}
    protected override void OnLoadData()
    {
        base.OnLoadData();
        //Singleton<Notification>.Instance.RegistMessageID(MessageID.common_open_mission_dialog_ret, UIPanleID.UIDialogue);
        this.State = EnumObjectState.Ready;
        Show();
    }
    public override void ReceiveData(UInt32 messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            case MessageID.common_open_mission_dialog_ret:
                //Show();
                //RefreshDialogueData();

                break;
        }
    }
    protected override void ShowHandler()
    {
        RefreshDialogueData();
    }
    public void RefreshDialogueData()
    {
        Destroy(npcmodel);
        //闲聊显示跳过按钮
        //任务对话 如果是剧情任务 不显示跳过按钮 不是剧情任务显示跳过按钮
        if (TaskManager.Single().isSmalltalk)
        {
            CloseButton.gameObject.SetActive(true);
        }
        else if (!TaskManager.Single().isSmalltalk && TaskManager.Single().isDialogueTask)
        {
            CloseButton.gameObject.SetActive(false);
        }
        else if (!TaskManager.Single().isSmalltalk && !TaskManager.Single().isDialogueTask)
        {
            CloseButton.gameObject.SetActive(true);
        }
        HideTaskHidePanel();

        PlayChatAnimation();
        //playerName.transform.gameObject.SetActive(isPlayer ? true : false);
        //npcName.transform.gameObject.SetActive(isNpc ? true : false);
        if (isPlayer)//玩家
        {
            playerName.text = playerNameStr;
            CrearteNpcModel(GameLibrary.player, 2);
        }
        else if (isNpc)//npc
        {
            NPCNode npcNode = FSDataNodeTable<NPCNode>.GetSingleton().FindDataByType(TaskManager.Single().npcid);//查找npc模型
            playerName.text = npcNode.npcname;
            //npcName.text = npcNode.npcname;
            CrearteNpcModel(npcNode.modelid, 1);
        }

        if (TaskManager.Single().CurrentShowDialogItem != null && TaskManager.Single().CurrentShowDialogItem.user[2] == 1 && !isSmalltalk)
        {

            acceptBtn.gameObject.SetActive(true);
            completeBtn.gameObject.SetActive(false);
            nextSprite.gameObject.SetActive(false);
        }
        else if (TaskManager.Single().CurrentShowDialogItem != null && TaskManager.Single().CurrentShowDialogItem.user[2] == 2 && !isSmalltalk)
        {
            completeBtn.gameObject.SetActive(true);
            acceptBtn.gameObject.SetActive(false);
            nextSprite.gameObject.SetActive(false);
        }
        else
        {
            acceptBtn.gameObject.SetActive(false);
            completeBtn.gameObject.SetActive(false);
            nextSprite.gameObject.SetActive(true);
        }
    }
    private void OnSkipClick(GameObject go)
    {
        SkipCurrentTaskTalk(TaskManager.Single().isSmalltalk);
    }
    private void OnClose(GameObject go)
    {
        contonts.Clear();
        TaskManager.Single().contonts.Clear();
        ShowTaskHidePanel();
        Control.HideGUI(this.GetUIKey());
        //Hide();
        HeroPosEmb.transform.Find("NpcPos").gameObject.SetActive(false);
        if (heroObj != null)
        {
            Destroy(heroObj);
        }
        if (OnChatFinishEvent != null)
        {
            OnChatFinishEvent(TaskManager.Single().npcid);
            OnChatFinishEvent = null;
        }
        isDialogueTask = false;
        isSmalltalk = true;
        isNpc = true;
        isPlayer = false;
        TaskManager.Single().isSmalltalk = true;
        TaskManager.Single().isDialogueTask = false;
    }

    //下一步点击事件
    private void OnNextStep(GameObject go)
    {
        if (TaskManager.Single().isSmalltalk)
        {
            ShowTaskHidePanel();
            OnClose(null);
        }
        else
        {
            if ((TaskManager.Single().CurrentShowDialogItem != null && TaskManager.Single().CurrentShowDialogItem.user[2] == 1 && !TaskManager.Single().isSmalltalk) || (TaskManager.Single().CurrentShowDialogItem != null && TaskManager.Single().CurrentShowDialogItem.user[2] == 2 && !TaskManager.Single().isSmalltalk))
            {
                ShowTaskHidePanel();
                OnClose(null);
            }
            else
            {
                //contonts.Clear();
                //HeroPosEmb.transform.Find("NpcPos").gameObject.SetActive(false);
                //if (heroObj != null)
                //{
                //    Destroy(heroObj);
                //}
                if (OnChatFinishEvent != null)
                {
                    OnChatFinishEvent(TaskManager.Single().npcid);
                    //OnChatFinishEvent = null;
                }
                //isDialogueTask = false;
                //isSmalltalk = true;
                //isNpc = true;
                //isPlayer = false;
            }
        }
        
    }

    //播放对话动画
    void PlayChatAnimation()
    {
        if (TaskManager.Single().contonts.Count>0)
        {
            //ChatContont.text = contonts[0];
            
            //string str = ConvertTaskDialogueDes(contonts[0]);
            string str = ConvertTaskDialogueDes(TaskManager.Single().contonts[0]);
            //playerContont.transform.gameObject.SetActive(isPlayer?true:false);
            //playerLine.transform.gameObject.SetActive(isPlayer ? true : false);
            //ChatContont.transform.gameObject.SetActive(isNpc?true:false);
            //npcLine.transform.gameObject.SetActive(isNpc ? true : false);
            if (isPlayer)
            {
                playerContont.text = str;
            }
            else if (isNpc)
            {
                playerContont.text = str;
                //ChatContont.text = str;
            }
        }
        
    }
    private GameObject heroObj;
    void CrearteNpcModel(long modelid,int type)
    {
        string tem = "";
        if (type == 1)//1 : npc 2 :玩家
        {
            if (FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList.ContainsKey(modelid))
            {
                tem = FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList[modelid].respath;
                heroObj = HeroPosEmbattle.instance.CreatModelByModelID((int)modelid, PosType.NpcPos, transform.Find("HeroTexture").GetComponent<SpinWithMouse>(),MountAndPet.Null,160);
            }
        }
        else if (type == 2)
        {
            if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(modelid))
            {
                tem = GameLibrary.Hero_URL + FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[modelid].icon_name + "_show";//传入英雄id
                heroObj = HeroPosEmbattle.instance.CreatModelByModelID(FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[modelid].model, PosType.NpcPos, transform.Find("HeroTexture").GetComponent<SpinWithMouse>(), MountAndPet.Null, 160);
                //heroObj = HeroPosEmbattle.instance.CreatModel(FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[modelid].icon_name + "_show", PosType.NpcPos, transform.Find("HeroTexture").GetComponent<SpinWithMouse>(), AnimType.None,160);
            }
        }

        ////ModelNode modelNode = FSDataNodeTable<ModelNode>.GetSingleton().FindDataByType(modelid);
        //GameObject obj = Resources.Load(tem) as GameObject;
        //if (obj!=null)
        //{
        //    npcmodel = Instantiate(obj) as GameObject;
        //}
        //else
        //{
        //    Debug.Log(tem + "模型没有找到");
        //}
        // if(npcmodel!=null)
        //    npcmodel.transform.parent = PlayerModelParent;
        ////if (type == 1)
        ////{
        ////    if(npcmodel!=null)
        ////    npcmodel.transform.parent = NPCModelParent;
        ////}
        ////else if (type == 2)
        ////{
        ////    npcmodel.transform.parent = PlayerModelParent;
        ////}
        
        //npcmodel.transform.localPosition = Vector3.zero;
        //npcmodel.transform.localScale = Vector3.one;
        //npcmodel.transform.localRotation = Quaternion.Euler(Vector3.zero);
        //SkinnedMeshRenderer[] skinnedMeshRenderer = npcmodel.GetComponentsInChildren<SkinnedMeshRenderer>();
        //for (int i = 0; i < skinnedMeshRenderer.Length; i++)
        //{
        //    skinnedMeshRenderer[i].gameObject.layer = 5;
        //}
    }
    //StringUtil.StrReplace
    /// <summary>
    /// 转换任务对话内容
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private string ConvertTaskDialogueDes(string str)
    {
        if (str==null||str == "")
        {
            return "";
        }
        //StringUtil.StrReplace((SelectCard.selectCardId).ToString(), "201", 0, 3)
        if (str == null) return str;
        int namePos = str.IndexOf("|");
        if (namePos > 0)
        {
            string temStr= str.Substring(0, namePos);
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

        string str1 = StringUtil.StrRemove(str,0,namePos+1);//以除掉对话内容开头的npc名字 或玩家名字
        //除开头的名字，替换上玩家的名字  替换只能直接返回 不然不起作用
        return str1.Replace("<selfname>", playerData.GetInstance().selfData.playeName).Replace("c1", GameLibrary.C1).Replace("c2", GameLibrary.C2).Replace("c3", GameLibrary.C3).Replace("c4", GameLibrary.C4).Replace("c5", GameLibrary.C5).Replace("c6", GameLibrary.C6);//替换对话内容中的颜色
    }
    public void ShowTaskHidePanel()
    {
        //UIPanleID[] arr= new UIPanleID[] { UIPanleID.UIMoney, UIPanleID.UIRole, UIPanleID.UISetting, UIPanleID.UI_TaskTracker, UIPanleID.UIChat, UIPanleID.UIMail,UIPanleID.UITaskEffectPanel };
        //Control.ShowGUI(arr, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UIMoney,EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UIRole,EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UISetting,EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UITaskTracker, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UIChat, EnumOpenUIType.DefaultUIOrSecond);
        //Control.ShowGUI(GameLibrary.UIMail);
    }

    public void HideTaskHidePanel()
    {
        Control.HideGUI(UIPanleID.UIMoney);
        Control.HideGUI(UIPanleID.UIRole);
        Control.HideGUI(UIPanleID.UISetting);
        Control.HideGUI(UIPanleID.UITaskTracker);
        Control.HideGUI(UIPanleID.UIChat);
        //Control.HideGUI(GameLibrary.UIMail);
        if (NextGuidePanel.Single().content!=null)
        {
            NextGuidePanel.Single().content.SetActive(false);
        }
    }
    /// <summary>
    /// 跳过当前任务对话(闲聊的跳过不发协议关闭按钮  非对话任务的跳过 发送跳过下一)
    /// </summary>
    public void SkipCurrentTaskTalk(bool isSmalltalk)
    {
        contonts.Clear();
        TaskManager.Single().contonts.Clear();
        ShowTaskHidePanel();
        Control.HideGUI(this.GetUIKey());
        //Hide();
        HeroPosEmb.transform.Find("NpcPos").gameObject.SetActive(false);
        if (heroObj != null)
        {
            Destroy(heroObj);
        }
        if (!isSmalltalk)
        {
            ClientSendDataMgr.GetSingle().GetTaskSend().OpenDialogUI(
                          TaskManager.Single().CurrentShowDialogItem.msId,
                          0,
                          TaskManager.Single().CurrentShowDialogItem.user[0],
                          1,
                          TaskManager.Single().CurrentShowDialogItem.user[2],
                          0
                          );
        }
        isDialogueTask = false;
        isSmalltalk = true;
        isNpc = true;
        isPlayer = false;
        TaskManager.Single().isSmalltalk = true;
        TaskManager.Single().isDialogueTask = false;

    }
}
