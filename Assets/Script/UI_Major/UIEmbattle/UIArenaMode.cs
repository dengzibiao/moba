using UnityEngine;
using System.Collections;
using Tianyu;
public class UIArenaMode : GUIBase
{
    public static UIArenaMode instance;
    public UIButton backBtn, soloBtn, v3Btn, v5Btn, detailBtn;
    public UILabel remainCnt;
    public UILabel arenaCoin;
    public UIButton shopBtn;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIArenaModePanel;
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }

    protected override void Init()
    {
        base.Init();
        instance = this;
        UIEventListener.Get(backBtn.gameObject).onClick = OnBackClick;
        UIEventListener.Get(v3Btn.gameObject).onClick = V3BtnClick;
        UIEventListener.Get(soloBtn.gameObject).onClick = V1BtnClick;
        UIEventListener.Get(v5Btn.gameObject).onClick = V5BtnClick;
        UIEventListener.Get(shopBtn.gameObject).onClick = OnShopBtn;
        remainCnt.text = string.Format("剩余挑战次数：{0}", 10);
        arenaCoin.text = string.Format("{0}", playerData.GetInstance().baginfo.areanCoin.ToString());


    }

    void OnShopBtn(GameObject go)
    {
        //if (UIShop.Instance.checkBoxs != null)
        //{
        //    UIShop.Instance.checkBoxs.DefauleIndex =2;
        //}
        //else
        //{
        //UIShop.Instance.IsShop(2);
        //Control.ShowGUI(GameLibrary.UIShop);
        object[] obj = new object[2] { 0, 2 };
        Control.ShowGUI(UIPanleID.UIShopPanel, EnumOpenUIType.OpenNewCloseOld, false, obj);
        // }
    }
    void OnBackClick(GameObject go)
    {
        //Hide();
        //Control.HideGUI(UIPanleID.UIArenaModePanel);
        //Control.ShowGUI(GameLibrary.UIPvP);
        Control.HideGUI();
    }
    //3v3按钮
    void V3BtnClick(GameObject go)
    {
        if (DataDefine.isSkipFunction || FunctionOpenMng.GetInstance().GetFunctionOpen(36))
        {
            Control.ShowGUI(UIPanleID.UIEmbattle, EnumOpenUIType.OpenNewCloseOld, false, OpenSourceType.Moba3V3);
        }
        else
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[36].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }

    }
    //1v1功能
    void V1BtnClick(GameObject go)
    {
        if (DataDefine.isSkipFunction || FunctionOpenMng.GetInstance().GetFunctionOpen(17))
        {
            Control.ShowGUI(UIPanleID.UIEmbattle, EnumOpenUIType.OpenNewCloseOld, false, OpenSourceType.Moba);
        }
        else
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[17].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }

    }
    //5v5按钮
    void V5BtnClick(GameObject go)
    {
        //if (FunctionOpenMng.GetInstance().GetValu(17))
        //{
        //    UIEmbattle.sourceType = OpenSourceType.Moba5V5;
        //    Hide();
        //    Control.ShowGUI(GameLibrary.UI_Embattle);
        //}
        //else
        {
            string text = "此功能暂未开放，敬请期待 ^_^ ";//FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[17].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }

    }

    protected override void RegisterComponent()
    {
        base.RegisterComponent();
        RegisterComponentID(25, 110, soloBtn.gameObject);

    }
    protected override void RegisterIsOver()
    {
        base.RegisterIsOver();
    }
}
