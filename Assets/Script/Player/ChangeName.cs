using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;
using System.Text.RegularExpressions;
public class ChangeName : GUIBase {

    public GUISingleButton sureBtn;
    public GUISingleButton randomBtn;
    public  UIInput nicknameInput;
    public GUISingleButton canCelBtn;

    public UILabel tipsLab;
    public UISprite diamondSp;
    public static ChangeName _instance;
    public Dictionary<long, PlayerNameNode> nodeDic;
    private int nameIndex = 0;
    private int signIndex = 0;
    private int surNameIndex = 0;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.ChangeName;
    }

    protected override void Init()
    {
        _instance = this;

        nicknameInput = transform.Find("NicknameInput").GetComponent<UIInput>();
        tipsLab = transform.Find("TipsLab").GetComponent<UILabel>();
        diamondSp = transform.Find("DiamondSp").GetComponent<UISprite>();

        sureBtn.onClick = OnSureBtn;
        canCelBtn.onClick = OnCancelBtn;
        randomBtn.onClick = OnRandomClick;
        if (FSDataNodeTable<PlayerNameNode>.GetSingleton().DataNodeList.Count > 0)
        {
            nodeDic = FSDataNodeTable<PlayerNameNode>.GetSingleton().DataNodeList;
            foreach (var nameLists in nodeDic)
            {

                playerData.GetInstance().selfData.nameList.Add(nameLists.Value);
            }

        }
       nicknameInput.value= playerData.GetInstance().selfData.playeName;
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();//显示面板
    }
    protected override void ShowHandler()
    {
        if (playerData.GetInstance().selfData.changeCount ==0)
        {
            tipsLab.text = "本次登录改名免费";
            diamondSp.spriteName = "";
        }
        else
        {
            tipsLab.text = "本次登录改名需花50";
            diamondSp.spriteName = "zuanshi";
        }
 
    }
    void OnSureBtn()
    {
       // UIRole.Instance.OpenChangeNamePanel(false);
        Control.HideGUI(this.GetUIKey());
        if (Regex.IsMatch(nicknameInput.value, @"^\d"))
        {
            UIRole.Instance.OpenChangeNamePanel(true);
            this.gameObject.SetActive(true);
            //UIPromptBox.Instance.ShowLabel("不能以数字开头");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "不能以数字开头");
            return;
        }
        if (Regex.IsMatch(nicknameInput.value, @"\s+"))
        {
            UIRole.Instance.OpenChangeNamePanel(true);
            this.gameObject.SetActive(true);
            //UIPromptBox.Instance.ShowLabel("不能有空格");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "不能有空格");
            return;
        }
        if (nicknameInput.value.Length < 2)
        {
            UIRole.Instance.OpenChangeNamePanel(true);
            this.gameObject.SetActive(true);
            //UIPromptBox.Instance.ShowLabel("不能输入小于2个字符");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "不能输入小于2个字符");
            return;
        }
        else if (nicknameInput.value.Length > 7)
        {
            UIRole.Instance.OpenChangeNamePanel(true);
            this.gameObject.SetActive(true);
            //UIPromptBox.Instance.ShowLabel("不能输入大于7字符");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "不能输入大于7字符");
            return;
        }
        
        serverMgr.GetInstance().SetName(nicknameInput.value);//
        serverMgr.GetInstance().saveData();
        Debug.Log(nicknameInput.value);
        if (DataDefine.isConectSocket)
        {
            ClientSendDataMgr.GetSingle().GetRoleSend().SendRoleInfo(nicknameInput.value);
        }
    }
    void OnCancelBtn()
    {
    //    UIRole.Instance.OpenChangeNamePanel(false);
        Control.HideGUI(this.GetUIKey());
       // UIRoleInfo.instance.gameObject.SetActive(true);
        Control.ShowGUI(UIPanleID.UIRoleInfo,EnumOpenUIType.DefaultUIOrSecond);
    }
    void OnRandomClick()
    {
        //int nameIndex = 0;
        //int signIndex = 0;
        //int surNameIndex = 0;
        for (int i = 0; i < playerData.GetInstance().selfData.nameList.Count; i++)
        {
            if (playerData.GetInstance().selfData.nameList[i].name != null || playerData.GetInstance().selfData.nameList[i].sign != null || playerData.GetInstance().selfData.nameList[i].surname != null)
            {
                nameIndex = Random.Range(0, playerData.GetInstance().selfData.nameList.Count);
                signIndex = Random.Range(0, playerData.GetInstance().selfData.nameList.Count);
                surNameIndex = Random.Range(0, playerData.GetInstance().selfData.nameList.Count);
            }
        }
        nicknameInput.value = playerData.GetInstance().selfData.nameList[nameIndex].name + playerData.GetInstance().selfData.nameList[signIndex].sign + playerData.GetInstance().selfData.nameList[surNameIndex].surname;
        Debug.Log(nicknameInput.value);
    }

    /// <summary>
    /// 打开改名面板
    /// </summary>
    public void OpenChangeNamePanel()
    {
        gameObject.SetActive(true);
    }


}