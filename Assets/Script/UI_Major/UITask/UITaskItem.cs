/*
文件名（File Name）:   UITaskItem.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;

public class UITaskItem : GUISingleItemList
{
    //任务内容
    public GUISingleLabel contont;
    //点击按钮
    public GUISingleButton btn;
    //任务状态
    public GUISingleSprite taskstate;

    public int currentindex;

    protected override void InitItem()
    {
        btn = transform.FindChild("Btn").GetComponent<GUISingleButton>();
    }

    //这样写有点扯 该写个类的
    public override void Info(object obj)
    {
        //关闭按钮
        if (int.Parse(((string[])obj)[0]) == -1)
        {
            contont.text = "路过而已";
            btn.onClick = OnClose;
            taskstate.gameObject.SetActive(false);
        }
        else
        {
            switch (int.Parse(((string[])obj)[2]))
            {
                case 0:
                    taskstate.spriteName = "kelignqu";
                    break; 
                case 1:
                    taskstate.spriteName = "kelignqu";
                    break;
                case 2:
                    taskstate.spriteName = "kelignqu";
                    break;
                case 3:
                    taskstate.spriteName = "kewanchneg";
                    break;
                case 4:
                    taskstate.spriteName = "kewanchneg";
                    break;
                case 5:
                    taskstate.spriteName = "";
                    break;
                default:
                    break;
            }
            btn.onClick = BtnOnClick;
            currentindex = int.Parse(((string[])obj)[0]);

            //主线
            if (int.Parse(((string[])obj)[3]) == 1)
            {
                contont.text = "[主]" + ((string[])obj)[1];
            }
            else
            {
                contont.text = "[支]" + ((string[])obj)[1];
            }
            taskstate.MakePixelPerfect();
        }
    }

    public void BtnOnClick()
    {
        Control.HideGUI(UIPanleID.UITaskTracker);
        ClientSendDataMgr.GetSingle().GetTaskSend().OpenDialogUI(
            TaskManager.Single().CurrentShowDialogItem.msId,
            currentindex,
            TaskManager.Single().CurrentShowDialogItem.user[0],
            0,
            TaskManager.Single().CurrentShowDialogItem.user[2],
            0
            );
        TaskManager.Single().isAcceptTask = true;
        //发消息
        //TaskOperation.Single().SetCurrentTaskItem(taskItem);
    }

    public void OnClose()
    {
        Control.ShowGUI(UIPanleID.UIMoney, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UIRole, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UISetting, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UITaskTracker, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UIChat, EnumOpenUIType.DefaultUIOrSecond);
        //Control.ShowGUI(GameLibrary.UIMail);
        Control.HideGUI(UIPanleID.UITaskTracker);
    }
}
