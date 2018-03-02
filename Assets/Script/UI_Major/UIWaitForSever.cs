/*
文件名（File Name）:   UIWaitForSever.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;

public class UIWaitForSever : GUIBase
{
    private long maxTimer = 15;
    private long lostTime;
    private float lostSecTime;
    private bool isShowTime;
    private UISprite sprite;
    private UISprite bg;
    protected override void Init()
    {
        base.Init();
        sprite = transform.FindChild("Sprite").GetComponent<UISprite>();
        bg = transform.FindChild("bg").GetComponent<UISprite>();
        lostTime = maxTimer;
        isShowTime = false;
        sprite.gameObject.SetActive(false);
        bg.gameObject.SetActive(false);
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIWaitForSever;
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();

    }

    protected override void ShowHandler()
    {
        this.State = EnumObjectState.Ready;
        base.ShowHandler();
        lostTime = maxTimer;
        isShowTime = true;
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (isShowTime)
        {
            if (maxTimer - lostTime > 3)
            {
                bg.gameObject.SetActive(true);
                sprite.gameObject.SetActive(true);
                sprite.transform.Rotate(0, 0, -2.5f);
            }

            lostSecTime += deltaTime;
            if (lostSecTime >= 1)
            {
                lostTime -= 1;
                lostSecTime = 0;
            }
            if (lostTime <= 0)
            {
                isShowTime = false;
                Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "网络已中断请重新连接");
                GameLibrary.isSendPackage = false;
                Hide();
                Control.HideGUI(this.GetUIKey());
            }
        }
    }
}
