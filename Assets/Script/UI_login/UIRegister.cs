/*

王


*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;


public class UIRegister : GUIBase
{
    //public static string url = "http://192.168.3.251/mp/ac/Reg.php?mobile={0}&passwd={1}";
	public GUISingleInput account;
    public GUISingleInput password;
    public GUISingleButton backBtn;
    //public GUISingleButton closeBtn;
    public GUISingleButton commitBtn;
    private WWW nwww;
	//string accounttex = "8899939393877575757";//SystemInfo.deviceUniqueIdentifier;
    protected override void Init()
    {
        commitBtn.onClick = OnCommitBtn;
        //closeBtn.onClick = OnClose;
        backBtn.onClick = OnClose;
		//account.text = accounttex;
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }

    void OnCommitBtn()
    {
		string acc = account.text;//accounttex; //account.text.Trim();
        string paw = password.text.Trim();
        if (acc.Length < 8)
        {
            //UIPromptBox.Instance.ShowLabel("帐号长度不符");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "帐号长度不符");
            return;
        }
        if (paw.Length < 8)
        {
            //UIPromptBox.Instance.ShowLabel("密码长度不符");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "密码长度不符");
            return;
        }
        if (account.text == paw)
        {
            //UIPromptBox.Instance.ShowLabel("帐号与密码不能相同，请重新输入");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "帐号与密码不能相同，请重新输入");
            return;
        }
        StartCoroutine(Register());
        //跳转选择服务器
        playerData.GetInstance().selfData.mobelNumb = account.text;//accounttex;
        playerData.GetInstance().selfData.password = paw;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIRegister;
    }
    public IEnumerator Register()
    {
		string[] arg = { account.text, password.text,DataDefine.ClientVersion, SystemInfo.deviceUniqueIdentifier,DataDefine.mainchannel.ToString() };
        string temp;// = string.Format( url , arg );
        if (DataDefine.isOutLine)
        {
            temp = string.Format(DataDefine.RegistOutLineUrl, arg);
        }
        else {
            temp = string.Format(DataDefine.RegistLineUrl, arg);
        }


        this.nwww = new WWW(temp);
        yield return nwww;
        if (this.nwww == null || !this.nwww.isDone)
            yield return nwww;

        string text = this.nwww.text;

        Dictionary<string, object> aobj = (Dictionary<string, object>)Jsontext.ReadeData(text);

        byte corde = byte.Parse(aobj["ret"].ToString());
        string message = aobj["desc"].ToString();

        print("值" + corde);

        if (corde == 1)
        {
            //UIPromptBox.Instance.ShowLabel(message);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, message);
        }
        else
        {
            // Dictionary<string , object> data = ( Dictionary<string , object> ) aobj [ "data" ];
          // Dictionary<string, object>[] serverarr = (Dictionary<string, object>[])aobj["host"];

          //  InitServerList(serverarr);

            Control.HideGUI(this.GetUIKey());
			serverMgr.GetInstance ().SetAccount (account.text);//SetAccount(account.text);
            serverMgr.GetInstance().SetPassword(password.text);
            serverMgr.GetInstance().SetMobile(long.Parse(aobj["account"].ToString()));
            // Control.ShowGUI(GameLibrary.UI_SelectServer);
            StartLandingShuJu.GetInstance().ServerList();
          //  serverMgr.GetInstance().saveData();
        }
    }

    void InitServerList(Dictionary<string, object>[] serverarr)
    {

        for (int i = 0; i < serverarr.Length; i++)
        {
            ServeData dater = new ServeData();
            dater.name = serverarr[i]["name"].ToString();
            dater.ip = serverarr[i]["ip"].ToString();
            dater.port = int.Parse(serverarr[i]["port"].ToString());
            dater.state = byte.Parse(serverarr[i]["state"].ToString());
            dater.Desc = serverarr[i]["desc"].ToString();
            dater.areaId = int.Parse(serverarr[i]["areaId"].ToString());
            dater.playerId = uint.Parse(serverarr[i]["playerId"].ToString());
            if (dater.playerId != 0)
            {
                if (serverarr[i].ContainsKey("playerName"))
                    dater.playerName = serverarr[i]["playerName"].ToString();
                if (serverarr[i].ContainsKey("heroId"))
                    dater.heroId = long.Parse(serverarr[i]["heroId"].ToString());
            }
            serverMgr.GetInstance().serverkeymap.Add(dater.areaId, dater.name);
            serverMgr.GetInstance().serverlist.Add(dater);
        }
        Globe.SelectedServer = serverMgr.GetInstance().serverlist[serverarr.Length - 1];
        UISelectServer.Instance.InitServerList();
    }
    void OnClose()
    {
        Control.HideGUI(this.GetUIKey());
        Control.ShowGUI(UIPanleID.UILogin, EnumOpenUIType.DefaultUIOrSecond);

    }
}
