using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;

public class SDKMgr : MonoBehaviour
{
    private static SDKMgr mInstance;
    public static SDKMgr Instance { get { return mInstance; } }
    private SDKBase mSdkApi;
    public static bool valid { get { return mInstance; } }
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        mInstance = this;
#if UNITY_EDITOR
        mSdkApi = new SDKBase();
#elif UNITY_ANDROID
        mSdkApi = new SDKForAndroid();
#endif
    }

    void Start()
    {
        //Init();
    }
    public  void Init(int flag = 0)
    {
        mSdkApi.Init(flag);
    }

    public  void Login()
    {
        mSdkApi.Login();
    }


    public void QQLogin()
    {
        mSdkApi.QQLogin();
    }

    public void WXLogin()
    {
        mSdkApi.WXLogin();
    }

    public  void LogOut()
    {
        mSdkApi.LogOut();
    }

    public PayData currentPayData;
    public class PayData
    {
        public int num;
        public string orderId;

        public PayData(int _num, string _orderId)
        {
            this.num = _num;
            this.orderId = _orderId;
        }
    }
    public  virtual void Pay(PayData data)
    {
        currentPayData = data;
        mSdkApi.Pay(currentPayData.num,currentPayData.orderId);
    }

    public  void GameCenter()
    {
        mSdkApi.GameCenter();
    }

    public void LoginCallBack(string accountId)
    {

        if (DataDefine.isConectSocket)
        {
            StartCoroutine(LoginGame(accountId));
        }

    }
    //string GetTocken(string accountId)
    //{
    //    string text = accountId;
    //    Dictionary<string, object> aobj = (Dictionary<string, object>)Jsontext.ReadeData(text);
    //    int ret = int.Parse(aobj["errno"].ToString());
    //    if (ret == 0)
    //    {
    //        Dictionary<string, object> data = aobj["data"] as Dictionary<string, object>;
    //        string access_token = data["access_token"].ToString();
    //        return access_token;
    //    }
    //    return "";

    //}
    private WWW nwww;
    public IEnumerator LoginGame(string access_token)
    {
       // string[] arg = { GetTocken(accountId), "tianyu", };// { account.text, passWord.text, };
        string temp;// = string.Format( url , arg );
        string passWord = "wuditianyu";
        if (DataDefine.isOutLine)
        {
            temp = DataDefine.LoginOutLineUrl360 + "?account=" + access_token + "&passwd=" + passWord + "&types=" + 1 + "&cv=" + DataDefine.ClientVersion + "&udid=" + SystemInfo.deviceUniqueIdentifier+ "&mc=" + DataDefine.mainchannel; ;
        }
        else
        {
            temp = DataDefine.LoginOutLineUrl360 + "?account=" + access_token + "&passwd=" + passWord + "&types=" + 1 + "&cv=" + DataDefine.ClientVersion + "&udid=" + SystemInfo.deviceUniqueIdentifier+ "&mc=" + DataDefine.mainchannel;
        }
        Debug.Log(string.Format("temp:{0}",temp));
        nwww = new WWW(temp);
        yield return nwww;
        if (nwww == null || !this.nwww.isDone)
            yield return nwww;

        string text  = nwww.data;
        Dictionary<string, object> aobj = (Dictionary<string, object>)Jsontext.ReadeData(text);
        if (aobj != null)
        {

            byte corde = byte.Parse(aobj["ret"].ToString());
            string message = aobj["desc"].ToString();
            if (aobj.ContainsKey("actime"))
                serverMgr.GetInstance().SetCreateAccountTime(aobj["actime"].ToString(),false);

            if (corde > 0)
            {

                //UIPromptBox.Instance.ShowLabel(message);
                Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, message);

            }
            else
            {
                Hide();
                long id = long.Parse(aobj["account"].ToString());
                serverMgr.GetInstance().SetMobile(id);



                // Dictionary<string, object>[] data = (Dictionary<string, object>[])aobj["host"];
                // InitServerList(data);

                //serverMgr.GetInstance().gamedata.Load(account.value)
                serverMgr.GetInstance().SetAccount(id.ToString());//SetAccount(account.text);
                serverMgr.GetInstance().SetPassword(passWord);

                Control.ShowGUI(UIPanleID.UI_SelectServer,EnumOpenUIType.DefaultUIOrSecond);
                StartLandingShuJu.GetInstance().ServerList();
                //serverMgr.GetInstance().saveData();
            }
        }
    }

    public void Hide()
    {

        Control.HideGUI(UIPanleID.UILogin);

    }
}
