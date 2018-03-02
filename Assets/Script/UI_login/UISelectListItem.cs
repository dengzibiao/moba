/*

王


*/
using UnityEngine;
using System.Collections;
using Tianyu;


public class UISelectListItem : GUISingleItemList
{

    public  GUISingleLabel serverName;

    public GUISingleSprite serverState;
    public GUISingleButton serverBtn;
    private object obj=null;

    protected override void InitItem()
    {
        serverName = this.GetComponentInChildren<GUISingleLabel>();
        serverState = this.GetComponentInChildren<GUISingleSprite>();
        serverBtn= this.GetComponentInChildren<GUISingleButton>();
        serverBtn.onClick = OnClick;
    }
    public override void Info(object obj)
    {
        if (obj == null)
        {

        }
        else
        {
            this.obj = obj;
            serverName.text = ((ServeData)obj).name+"【"+((ServeData)obj).areaId + "服】";
            switch (((ServeData)obj).state)
            {
                case 0:
                    serverState.spriteName = "huidian";
                    break;
                case 2:
                    serverState.spriteName = "lvdian";
                    break;
                case 1:
                    serverState.spriteName = "hongdian";
                    break;
            }

        }

    }

    void OnClick ()
    {
        Control.HideGUI(UIPanleID.UI_ServerList);
        Globe.SelectedServer = ((ServeData)obj);
        UIServerList.Instanse.loginEffect.gameObject.SetActive(true);
        if (Globe.SelectedServer != null)
        {
			if (Globe.SelectedServer.playerId != 0) {

				serverMgr.GetInstance().SetName(((ServeData)obj).playerName );
                GameLibrary.player = ((ServeData)obj).heroId;
                GameLibrary.nickName = ((ServeData)obj).playerName;//角色的显示信息
			}
		}
        Control.ShowGUI(UIPanleID.UI_SelectServer, EnumOpenUIType.DefaultUIOrSecond, false, (ServeData)obj);
    }



}
