using UnityEngine;
using System.Collections;
using System;

public class UIMail : GUIBase
{
   // public GUISingleButton btn;
    public GUISingleLabel newMailCount;
    public UISprite newMailHint;
    private UISprite mailSprite;

    protected override void Init()
    {
        base.Init();
       // btn = transform.Find("Empty/Btn").GetComponent<GUISingleButton>();
        newMailCount = transform.Find("NewMailCount").GetComponent<GUISingleLabel>();
       // mailSprite = transform.Find("MailSprite").GetComponent<UISprite>();
        newMailHint = transform.Find("NewMailHint").GetComponent<UISprite>();
     //   btn.onClick = OnBtnClick;
        playerData.GetInstance().NewMailHint += SetNewMailHint;

    }
    protected override void ShowHandler()
    {
        base.ShowHandler();
        //避免读取到数据但是事件没注册
        SetNewMailHint(playerData.GetInstance().mailData.unReadMailCount);
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    private void SetNewMailHint(int count)
    {
        if (count > 0)
        {
            if (newMailHint!=null)
            {
                newMailHint.gameObject.SetActive(true);
            }
            if (newMailCount!=null)
            {
                newMailCount.gameObject.SetActive(true);
            }
            //newMailCount.text = count + "";
            newMailCount.text ="";
        }
        else
        {
            newMailHint.gameObject.SetActive(false);
            newMailCount.gameObject.SetActive(false);
        }
    }
    //private void OnBtnClick()
    //{
    //    //Resource.CreatPrefabs("UIMailPanel", transform.parent.gameObject);
    //    //ClientSendDataMgr.GetSingle().GetMailSend().SendGetAllMailList();
    //    Control.ShowGUI(GameLibrary.UIMailPanel);
    //    //playerData.GetInstance().NewMailHandler(0);
    //}

	
}
