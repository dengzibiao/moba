/*
文件名（File Name）:   ServerItemList1.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using Tianyu;

public class UIServerItemList : GUISingleItemList
{
    public GUISingleLabel label;
    public GUISingleButton button;
    public GUISingleSprite highlight;
    private object obj = null;
    /// <summary>
    /// item初始化函数
    /// </summary>
    protected override void InitItem()
    {
        label = this.GetComponentInChildren<GUISingleLabel>();
        button = this.GetComponentInChildren<GUISingleButton>();
        highlight = this.GetComponentInChildren<GUISingleSprite>();
        highlight.gameObject.SetActive(false);
        button.onClick = OnSelectServer;

    }
    private void OnSelectServer()
    {
        highlight.gameObject.SetActive(true);
        UIServerList.Instanse.ShowSpecificServer(highlight.gameObject, ((int)obj * 10 - 9)-1, (int)obj * 10-1);
    }

    public override void Info(object obj)
    {
        if (obj == null)
        {

        }
        else
        {
            this.obj = obj;
            label.text = ((int)obj * 10 - 9) + "-" + (int)obj * 10 + "区";
            if (this.index == 0)
            {
                UIServerList.Instanse.ChangeGoEffrct(highlight.gameObject);
            }
        }

    }

}
