using UnityEngine;
using System.Collections;
using System;
using Pathfinding.Serialization.JsonFx;
using System.Collections.Generic;
using Tianyu;


public class UILogin : GUIBase
{
    // public static string url = "http://192.168.3.251/mp/ac/Login.php?mobile={0}&passwd={1}";
    // public string playerlisturl = "http://192.168.3.251/mp/ac/getplayerlist.php?account={0}";
    public GUISingleButton loginBtn, qQLoginBtn, wXLoginBtn;
    public GUISingleButton closeBtn;
    public GUISingleButton registerBtn;
    public GUISingleInput account;
    public GUISingleInput passWord;
    private UIToggle remPassword;
    private UIToggle autoLogin;
    //	string accounttex = "";
    private WWW nwww;

    public static Dictionary<string, object> roleinfo;
    protected override void Init()
    {
        loginBtn.onClick = OnLogin;
        qQLoginBtn.onClick = OnQQLogin;
        wXLoginBtn.onClick = OnWXLogin;
        registerBtn.onClick = OnRegisterClick;
        remPassword = transform.Find("RemPassword").GetComponent<UIToggle>();
        autoLogin = transform.Find("AutoLogin").GetComponent<UIToggle>();
        EventDelegate ed4 = new EventDelegate(this, "OnRemPassword");
        remPassword.onChange.Add(ed4);

        EventDelegate ed5 = new EventDelegate(this, "OnAutoLogin");
        autoLogin.onChange.Add(ed5);

        roleinfo = new Dictionary<string, object>();
#if QiHuRelease
        loginBtn.transform.localScale = Vector3.zero;
        registerBtn.transform.localScale = Vector3.zero;
        account.transform.localScale = Vector3.zero;
        passWord.transform.localScale = Vector3.zero;
#elif TencentRelease
        qQLoginBtn.gameObject.SetActive(true);
        wXLoginBtn.gameObject.SetActive(true);
        loginBtn.transform.localScale = Vector3.zero;
        registerBtn.transform.localScale = Vector3.zero;
        account.transform.localScale = Vector3.zero;
        passWord.transform.localScale = Vector3.zero;
#else
        loginBtn.transform.localScale = Vector3.one;
        registerBtn.transform.localScale = Vector3.one;
        account.transform.localScale = Vector3.one;
        passWord.transform.localScale = Vector3.one;
#endif
    }



    protected override void ShowHandler()
    {
        serverMgr.GetInstance().GetAccount();
        if (serverMgr.GetInstance().GetAccount() != "")
        {
            account.text = serverMgr.GetInstance().GetAccount();//SystemInfo.deviceUniqueIdentifier;//serverMgr.GetInstance().GetAccount();
        }
        if (serverMgr.GetInstance().GetPassword() != "")
        {
            passWord.text = serverMgr.GetInstance().GetPassword();
        }
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UILogin;
    }

    private void OnQQLogin()
    {
        SDKMgr.Instance.QQLogin();
    }

    private void OnWXLogin()
    {
        SDKMgr.Instance.WXLogin();
    }

    private void OnLogin()
    {
        ////   if (account.text.Length < 8)
        ////  {
        ////   UIPromptBox.Instance.ShowLabel("帐号长度不符");
        ////  return;
        //// }

#if QiHuRelease
        {
            if (DataDefine.isConectSocket)
            {
                SDKMgr.Instance.Login();
            }
        }

#else

        if (passWord.text.Length < 8)
        {
            UIPromptBox.Instance.ShowLabel("密码长度不符");
            return;
        }
        if (account.text == "")
        {
            UIPromptBox.Instance.ShowLabel("密码长度不符");
            return;
            // MessageManager.Instance.ShowMessage("账号不能为空！");
            Debug.Log("账号不能为空！");
            return;
        }
        if (DataDefine.isConectSocket)
        {
            StartCoroutine(Login());
        }
        else
        {
            Hide();
            Control.HideGUI(this.GetUIKey());
            serverMgr.GetInstance().SetAccount(account.text);
            serverMgr.GetInstance().SetPassword(passWord.text);
            if (transform.parent.Find("UISelectServer") != null)
                transform.parent.Find("UISelectServer").gameObject.SetActive(true);
            serverMgr.GetInstance().saveData();

        }
        playerData.GetInstance().selfData.mobelNumb = account.text;//SystemInfo.deviceUniqueIdentifier;//account.text;
        playerData.GetInstance().selfData.password = passWord.text;
#endif


        //      else
        //      {
        //          Hide();
        //	serverMgr.GetInstance ().SetAccount (account.text);
        //          serverMgr.GetInstance().SetPassword(passWord.text);
        //	if(transform.parent.Find("UISelectServer")!=null)
        //	transform.parent.Find("UISelectServer").gameObject.SetActive(true);
        //          serverMgr.GetInstance().saveData();

        //      }

        //playerData.GetInstance ().selfData.mobelNumb = account.text;//SystemInfo.deviceUniqueIdentifier;//account.text;
        //      playerData.GetInstance().selfData.password = passWord.text;
        //StartCoroutine(PlayerList());

    }
    byte result;
    //http请求登陆消息
    public IEnumerator Login()
    {
        string[] arg = { account.text, passWord.text, };// { account.text, passWord.text, };
        string temp;// = string.Format( url , arg );
        if (DataDefine.isOutLine)
        {
            temp = DataDefine.LoginOutLineUrl + "?account=" + account.text + "&passwd=" + passWord.text + "&types=" + 1 + "&cv=" + DataDefine.ClientVersion + "&udid=" + SystemInfo.deviceUniqueIdentifier+"&mc="+DataDefine.mainchannel;
        }
        else {
            temp = DataDefine.LoginUrl + "?account=" + account.text + "&passwd=" + passWord.text + "&types=" + 1 + "&cv=" + DataDefine.ClientVersion + "&udid=" + SystemInfo.deviceUniqueIdentifier+"&mc=" + DataDefine.mainchannel;
        }

        nwww = new WWW(temp);
        yield return nwww;
        if (nwww == null || !this.nwww.isDone)
            yield return nwww;

        string text = nwww.data;
        Dictionary<string, object> aobj = (Dictionary<string, object>)Jsontext.ReadeData(text);
        if (aobj != null)
        {

            byte corde = byte.Parse(aobj["ret"].ToString());
            string message = aobj["desc"].ToString();
            if (aobj.ContainsKey("actime"))
                serverMgr.GetInstance().SetCreateAccountTime(aobj["actime"].ToString(), false);

            if (corde == 1)
            {

                UIPromptBox.Instance.ShowLabel(message);

            }
            else
            {
                Hide();
                Control.HideGUI(this.GetUIKey());
                serverMgr.GetInstance().SetMobile(long.Parse(aobj["account"].ToString()));



                // Dictionary<string, object>[] data = (Dictionary<string, object>[])aobj["host"];
                //InitServerList(data);

                //serverMgr.GetInstance().gamedata.Load(account.value)
                serverMgr.GetInstance().SetAccount(account.text);//SetAccount(account.text);
                serverMgr.GetInstance().SetPassword(passWord.text);
              //  Control.ShowGUI(UIPanleID.UI_SelectServer, EnumOpenUIType.DefaultUIOrSecond);
                StartLandingShuJu.GetInstance().ServerList();
                serverMgr.GetInstance().saveData();
            }
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
    /// <summary>
    ///注册按钮
    /// </summary>
    private void OnRegisterClick()
    {
        Control.ShowGUI(UIPanleID.UIRegister, EnumOpenUIType.DefaultUIOrSecond);
        Control.HideGUI(this.GetUIKey());
    }
    /// <summary>
    /// 记住密码
    /// </summary>
    /// <param name="state"></param>
    private void OnRemPassword()
    {
        //print( remPassword.value );
    }
    private void OnAutoLogin()
    {
        // print( autoLogin.value );
    }

}
