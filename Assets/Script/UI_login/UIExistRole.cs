/*

王


*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class UIExistRole : GUISingleItemList
{
    private UILabel zoneName;

    private UISprite roleIcon;

    private UILabel nameLabel;

    private UILabel levelLabel;

    //private UILabel powerLabel;

    private UISprite zoneState;

    private GUISingleButton serverBtn;

    private object obj;
    private byte state;
    protected override void InitItem()
    {
        roleIcon = transform.Find("RoleIcon").GetComponent<UISprite>();
        nameLabel = transform.Find("NameLabel").GetComponent<UILabel>();
        levelLabel = transform.Find("LevelLabel").GetComponent<UILabel>();
        //powerLabel = transform.Find( "PowerLabel" ).GetComponent<UILabel>();
        serverBtn = transform.Find("ServerBtn").GetComponent<GUISingleButton>();
        zoneName = transform.Find("ServerBtn/Label").GetComponent<UILabel>();
        zoneState = transform.Find("ServerBtn/Sprite").GetComponent<UISprite>();
        serverBtn.onClick = OnServerBtnClick;
       // SetData();
    }
    public override void Info(object obj)
    {
        if (obj == null)
        {

        }
        else
        {
            this.obj = obj;
            //roleIcon.spriteName头像赋值
            //roleIcon.spriteName =
            //    FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[((ServeData) obj).heroId].icon_name+"_head";
            nameLabel.text = ((ServeData)obj).playerName;
            zoneName.text= ((ServeData)obj).name;
            state = ((ServeData)obj).state;
            switch (((ServeData)obj).state)
            {
                case 0:
                    zoneState.spriteName = "huidian";
                    break;
                case 2:
                    zoneState.spriteName = "lvdian";
                    break;
                case 1:
                    zoneState.spriteName = "hongdian";
                    break;
            }

        }

    }

    void OnServerBtnClick ()
    {
        GameLibrary.nickName = nameLabel.text;
        Globe.SelectedServer = (ServeData) obj;
        GameLibrary.player = Globe.SelectedServer.heroId;
        nameLabel.text = Globe.SelectedServer.playerName;
        Control.HideGUI(UIPanleID.UI_ServerList);
        Control.ShowGUI(UIPanleID.UI_SelectServer, EnumOpenUIType.DefaultUIOrSecond, false, Globe.SelectedServer);
    }
}
