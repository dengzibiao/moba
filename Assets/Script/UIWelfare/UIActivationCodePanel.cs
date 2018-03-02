using UnityEngine;
using System.Collections;

public class UIActivationCodePanel : GUIBase {

    public GUISingleInput input;
    public GUISingleButton exchangeBtn;
    //public UIButton btn;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }


    protected override void Init()
    {
        base.Init();
        //input = transform.Find("Input").GetComponent<UIInput>();
        exchangeBtn.onClick = OnExchangeOnclick;
        //exchangeBtn = transform.Find("ExchangeBtn").GetComponent<UIButton>();
        //EventDelegate ed = new EventDelegate(this, "OnExchangeOnclick");
        //exchangeBtn.onClick.Add(ed);
    }
    protected override void ShowHandler()
    {
       // input.value = "";
        //UIEventListener.Get(btn).onClick = OnExchangeOnclick;
    }
    private void OnExchangeOnclick()
    {
        Debug.Log(input.text+"已发送");
    }
}
