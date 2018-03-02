using UnityEngine;
using System.Collections;

public class ItemTest : GUISingleItemList
{

    public GUISingleButton mailBtn;
    public UISprite icon;
    public UILabel label;

    /// <summary>
    /// item初始化函数
    /// </summary>
    protected override void InitItem()
    {
        //初始化
        mailBtn = transform.Find("MailBtn").GetComponent<GUISingleButton>();
        mailBtn.onClick = OnBtnClick;
    }

    public override void Info(object obj)
    {
        if(obj == null)
        {
            //icon.spriteName = item.icon;
            label.text = "";
        }
        else
        {
            //icon.spriteName = item.icon;
            label.text = Random.value * 10 + "";
        }

    }

    private void OnBtnClick()
    {
        print("我是钉子" + index);
    }
}
