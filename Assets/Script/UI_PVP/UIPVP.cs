using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;

public class UIPVP : GUIBase
{

    public static UIPVP instance;

    public GUISingleButton backBtn;
    public GUISingleButton arenaBtn;
    public GUISingleButton abattoirBtn;
    public GUISingleButton arenaPracticeBtn;
    public GUISingleButton abattoirPracticeBtn;
    public GUISingleSprite redPoint;
    public GameObject tianzhan_guangquan;
    public GameObject tianzhan_guangquan_zi;

    public UIPanel arenaPracticePanel;

    UIArenaMode amView;

    public UIPVP()
    {
        instance = this;
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
        Control.PlayBgmWithUI(UIPanleID.UIPvP);
    }

    protected override void Init()
    {
        base.Init();

        backBtn.onClick = OnBackClick;
        arenaBtn.onClick = OnArenaBtnClick;
        abattoirBtn.onClick = OnAbattoirBtnClick;
        arenaPracticeBtn.onClick = OnArenaPracticeBtnClick;
        abattoirPracticeBtn.onClick = OnAbattoirPracticeBtnClick;

        if (null != Globe.backPanelParameter && (UIPanleID)Globe.backPanelParameter[0] == UIPanleID.UIPvP)
        {
            //SendGetList();
            Control.ShowGUI(UIPanleID.UIAbattiorList, EnumOpenUIType.OpenNewCloseOld, false, false);
            Globe.backPanelParameter = null;
        }

    }

    protected override void ShowHandler()
    {
        base.ShowHandler();
        ShowRedPoint(Singleton<RedPointManager>.Instance.GetRedList());
    }
    private void ShowRedPoint(Dictionary<int, List<int>> redlist)
    {

        if (GameLibrary.ArenaNumber != null && GameLibrary.ArenaNumber.Length > 2)
        {
            if (int.Parse(GameLibrary.ArenaNumber[3].ToString()) > 0)
            {
                Singleton<RedPointManager>.Instance.AddChildFlag(EnumRedPoint.RD_UIPVP, 1);
                redPoint.ShowOrHide(true);

            }
            else
            {
                Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RD_UIPVP, 1);
                redPoint.ShowOrHide(false);
            }
        }
    }
    void OnArenaPracticeBtnClick()
    {
        arenaPracticePanel.gameObject.SetActive(true);
    }

    void OnAbattoirPracticeBtnClick()
    {
        arenaPracticePanel.gameObject.SetActive(true);
    }

    void OnArenaBtnClick()
    {

        if (DataDefine.isSkipFunction || FunctionOpenMng.GetInstance().GetFunctionOpen(17))
        {
            //竞技场功能开启
            //Hide();
            Control.ShowGUI(UIPanleID.UIArenaModePanel, EnumOpenUIType.OpenNewCloseOld, false);
        }
        else
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[17].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }


    }

    void OnConfirmLineup(bool isDefine)
    {
        if (isDefine)
        {
            Debug.Log("Moba");
        }
    }

    public void OnAbattoirBtnClick()
    {
        if (FunctionOpenMng.GetInstance().GetFunctionOpen(8))
        {
            //角斗场功能开启
            Control.ShowGUI(UIPanleID.UIAbattiorList, EnumOpenUIType.OpenNewCloseOld, false, false);
        }
        else
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[8].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }

    }

    //void SendGetList()
    //{
    //    Globe.IsRefreshArena = false;
    //    ClientSendDataMgr.GetSingle().GetBattleSend().SendQueryArenaList(0);
    //}

    void OnBackClick()
    {
        //Hide();
        Control.HideGUI();
        Control.PlayBGmByClose(this.GetUIKey());
    }

    protected override void RegisterComponent()
    {
        base.RegisterComponent();
        RegisterComponentID(24, 71, arenaBtn.gameObject);
        RegisterComponentID(28, 71, abattoirBtn.gameObject);

    }
    protected override void RegisterIsOver()
    {
        base.RegisterIsOver();
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIPvP;
    }

}
