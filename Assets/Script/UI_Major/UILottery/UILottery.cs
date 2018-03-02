using UnityEngine;
using System.Collections;

public class UILottery : GUIBase
{
    public GameObject GoldLottery; //引导挂点
    public GameObject DiamondLottery; //引导挂点

    public GUISingleButton backBtn;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UILottery;
    }

    protected override void Init()
    {
        backBtn.onClick = OnBackClick;

    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_lucky_gamble_list_ret, this.GetUIKey());
        Singleton<Notification>.Instance.Send(MessageID.common_lucky_gamble_list_req);
    }

    public override void ReceiveData(uint messageID)
    {
        Show();
        base.ReceiveData(messageID);

    }

    private void OnBackClick()
    {
        Control.HideGUI();
        Control.PlayBGmByClose(UIPanleID.UILottery);
    }

    protected override void ShowHandler()
    {
        base.ShowHandler();
        Control.PlayBgmWithUI(UIPanleID.UILottery);
    }

    protected override void RegisterComponent()
    {
        base.RegisterComponent();
        RegisterComponentID(40, 31, backBtn.gameObject);
        RegisterComponentID(13, 31, GoldLottery);
        RegisterComponentID(45, 31, DiamondLottery);

    }
    protected override void RegisterIsOver()
    {
        base.RegisterIsOver();
    }
}
