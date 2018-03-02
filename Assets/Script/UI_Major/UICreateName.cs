using UnityEngine;
using System.Collections;
using Tianyu;
using System.Text.RegularExpressions;
using System.Text;

public class UICreateName : GUIBase
{

    public static UICreateName instance;

    public GUISingleButton sureBtn;
    public GUISingleButton randomBtn;
    public GUISingleInput nicknameInput;
    public GUISingleLabel nameLabel;
    public GUISingleLabel promptLabel;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.ChangeName;
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }
    protected override void Init()
    {
        instance = this;
        sureBtn.onClick = OnSureBtn;
        randomBtn.onClick = OnRandomClick;
        EventDelegate.Add(nicknameInput.GetComponent<UIInput>().onChange, OnChange);

        NPCNode npc = FSDataNodeTable<NPCNode>.GetSingleton().FindDataByType(109);
        ModelNode model = FSDataNodeTable<ModelNode>.GetSingleton().FindDataByType(npc.modelid);
        nameLabel.text = npc.npcname;
        GameObject npcGo = HeroPosEmbattle.instance.CreatModelByModelID(model.id, PosType.NpcPos, null, MountAndPet.Null, -200);
        UnityUtil.AddComponetIfNull<NPCMotion>(npcGo);

        NPCNode npcProm = FSDataNodeTable<NPCNode>.GetSingleton().FindDataByType(138);
        promptLabel.text = npcProm.ana;

        //if (string.IsNullOrEmpty(npcProm.voice[0]))
        //    AudioController.Instance.PlayUISound(GameLibrary.Resource_GuideSound + npcProm.voice[0], true);
    }

    /// <summary>
    /// input改变事件
    /// </summary>
    private void OnChange()
    {
        if (nicknameInput.text.Length > 7)//f (Encoding.Default.GetBytes(nicknameInput.text).Length >14)
        {
            //UIPromptBox.Instance.ShowLabel("角色名称最多7个字符~");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "角色名称最多7个字符~");
            nicknameInput.text = nicknameInput.text.Substring(0, 7);
        }
    }

    void OnRandomClick()
    {
        nicknameInput.text = GetRandomName();
    }


    void OnSureBtn()
    {
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
        if (nicknameInput.text.Length < 2)
        {
            //UIPromptBox.Instance.ShowLabel("角色名称至少要2个字符哟~");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "角色名称至少要2个字符哟~");
            return;
        }
        if (nicknameInput.text.Length > 7)
        {
            //UIPromptBox.Instance.ShowLabel("角色名称最多7个字符~");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "角色名称最多7个字符~");
            return;
        }
        sureBtn.isEnabled = false;
        playerData.GetInstance().selfData.playeName = nicknameInput.text;
        serverMgr.GetInstance().SetName(nicknameInput.text);
        GameLibrary.nickName = nicknameInput.text;
        if (!ClientNetMgr.GetSingle().IsConnect())//如果网络没有连接连下
        {
            ClientNetMgr.GetSingle().StartConnect(Globe.SelectedServer.ip, Globe.SelectedServer.port);
        }
        ClientSendDataMgr.GetSingle().GetLoginSend().SendCheckAccount();
        //string heroId = StringUtil.StrReplace((SelectCard.selectCardId).ToString(), "201", 0, 3);
        //GameLibrary.player = long.Parse(heroId);

        //SendMeg();
        //ClientSendDataMgr.GetSingle().GetLoginSend().SendCreateRole(nicknameInput.text, heroId, Globe.SelectedServer.areaId.ToString());
        //ClientSendDataMgr.GetSingle().GetLoginSend().SendCreateRole(nicknameInput.text, heroId, SelectCard.selectAreaId);

        //serverMgr.GetInstance().SetCurrentPoint(UI_HeroDetail.skillCount);
        //serverMgr.GetInstance().SetFullTime(Auxiliary.GetNowTime() + (20 - UI_HeroDetail.skillCount) * 600);
        


    }

    public void SendMeg()
    {
        string heroId = StringUtil.StrReplace((SelectCard.selectCardId).ToString(), "201", 0, 3);
        GameLibrary.player = long.Parse(heroId);
        ClientSendDataMgr.GetSingle().GetLoginSend().SendCreateRole(nicknameInput.text, heroId, Globe.SelectedServer.areaId.ToString());
        serverMgr.GetInstance().saveData();
    }

    /// <summary>
    /// 恢复创建角色按钮的操作，用户名重复等与服务器通讯的操作，等待服务器回复后重置状态
    /// </summary>
    public void EnterGame()
    {
        sureBtn.isEnabled = true;
    }

    public static string GetRandomName()
    {
        InitNameList();
        int nameIndex = 0;
        int signIndex = 0;
        int surNameIndex = 0;
        for (int i = 0; i < playerData.GetInstance().selfData.nameList.Count; i++)
        {
            if (playerData.GetInstance().selfData.nameList[i].name != null || playerData.GetInstance().selfData.nameList[i].sign != null || playerData.GetInstance().selfData.nameList[i].surname != null)
            {
                nameIndex = Random.Range(0, playerData.GetInstance().selfData.nameList.Count);
                signIndex = Random.Range(0, playerData.GetInstance().selfData.nameList.Count);
                surNameIndex = Random.Range(0, playerData.GetInstance().selfData.nameList.Count);
            }
        }
        return playerData.GetInstance().selfData.nameList[nameIndex].name + playerData.GetInstance().selfData.nameList[signIndex].sign + playerData.GetInstance().selfData.nameList[surNameIndex].surname;
    }

    static void InitNameList()
    {
        if (playerData.GetInstance().selfData.nameList.Count > 0)
            return;
        if (FSDataNodeTable<PlayerNameNode>.GetSingleton().DataNodeList.Count > 0)
        {
            // Dictionary<long, PlayerNameNode> nodeDic = FSDataNodeTable<PlayerNameNode>.GetSingleton().DataNodeList;
            foreach (var nameLists in FSDataNodeTable<PlayerNameNode>.GetSingleton().DataNodeList)
            {
                playerData.GetInstance().selfData.nameList.Add(nameLists.Value);
            }
        }
    }

}
