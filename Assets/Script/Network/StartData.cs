using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;
using System;using UnityEngine.SceneManagement;
public class StartData : MonoBehaviour {

    float currtime = 0;
    float count = 60;
	bool isShowMessageDlg = false;
	static StartData m_Singlton;
   public   static   bool isoldversion = true;
	public static StartData GetInstance()
	{
		return m_Singlton;
	}
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
		if (m_Singlton != null) {
			Destroy (m_Singlton.gameObject);
			//return;
		}
		m_Singlton = this;
        CHandleMgr.GetSingle().RegistAllHandle();
        //if(isoldversion)
        //{
        //    if (UIPromptBox.Instance != null)
        //    {
        //        UIPromptBox.Instance.ShowLabel("正在与服务器做版本比较，请稍后");
        //    }
        //    StartCoroutine(VersionLoad());
        //}
       // time = 60;
    }
    public WWW nWWW;
    public IEnumerator VersionLoad()
    {
        
        long[] arg = { serverMgr.GetInstance().GetMobile(), 1 };
        string temp;// = string.Format( url , arg );
        if (DataDefine.isOutLine)
        {
            temp = DataDefine.outserverip+"version.txt";//DataDefine.ServerListOutLineUrl + "?account=" + serverMgr.GetInstance().GetMobile().ToString() + "&types=" + 2.ToString();
        }
        else
        {
            temp = DataDefine.inserverip + "version.txt";//DataDefine.ServerListUrl + "?account=" + serverMgr.GetInstance().GetMobile().ToString() + "&types=" + 2.ToString();
        }
        nWWW = new WWW(temp);
        yield return nWWW;
        if (nWWW == null || !nWWW.isDone)
            yield return nWWW;
        string text = nWWW.data;
        if (nWWW.error == null)
        {
            Dictionary<string, object> aobj = (Dictionary<string, object>)Jsontext.ReadeData(text);
            string serverversion = aobj["version"].ToString();
            //string[] items = text.Split('=');

            if (serverversion.Equals(DataDefine.version))
            {
                // Debug.Log("版本一致"+ items[1]+"==>"+ DataDefine.version);
                if (UIPromptBox.Instance != null)
                {
                    UIPromptBox.Instance.Hide();
                }
            }
            else
            {
                //if (UIPromptBox.Instance != null)
                //{
                //    UIPromptBox.Instance.ShowLabel("您的版本号与服务器不一致，请更新新包");
                //}
                Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的版本号与服务器不一致，请更新新包");
                // Debug.Log("版本不一致" + items[1] + "==>" + DataDefine.version);
            }
        }
    }



        // Update is called once per frame
        void Update()
    {
        ClientNetMgr.GetSingle().Update();
		Globe.lastNetTime += Time.deltaTime;
        currtime += Time.deltaTime;
		if (!ClientNetMgr.GetSingle().m_bServerNet && !isShowMessageDlg) {
			//if (UIPromptBox.Instance != null)
			//{
			//	UIPromptBox.Instance.ShowLabel("与服务器链接丢失，将重新与服务器建立链接");
			//}
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "与服务器链接丢失，将重新与服务器建立链接");
            isShowMessageDlg = true;
		}
        if( currtime >count)
        {
            if ( ClientNetMgr.GetSingle().IsConnect() && ClientSendDataMgr.GetSingle().GetWalkSend().ping )
            {
                ClientSendDataMgr.GetSingle().GetWalkSend().Ping();
                Debug.Log("ping包发送");
            }
            currtime = 0;
        }
        if(Input.GetKeyDown(KeyCode.Escape)||Input.GetKey(KeyCode.Home))
        {
           // ClientSendDataMgr.GetSingle().GetHeroSend().SendQuitGame();//通知服务器存盘
          //  Application.LoadLevel(0);
            //退出游戏
            Application.Quit();
        }

		if ( UISelectServer.Instance!=null&&UISelectServer.Instance. isLoading != 0) {
			if (UISelectServer.Instance.isLoading == 1) {
				UISelectServer.Instance.SendLogin (Globe.SelectedServer.playerId, Globe.SelectedServer.heroId, Globe.SelectedServer.playerName, Globe.SelectedServer.areaId);
				UISelectServer.Instance.isLoading = 0;
			} else if (UISelectServer.Instance.isLoading == 2) {
				UISelectServer.Instance.SendCreate (Globe.SelectedServer.playerId, Globe.SelectedServer.heroId, Globe.SelectedServer.playerName, Globe.SelectedServer.areaId);
				UISelectServer.Instance.isLoading = 0;
			}
		}
        
    }
    //    强制暂停时，先 OnApplicationPause，后 OnApplicationFocus；
    //重新“启动”手机时，先OnApplicationFocus，后 OnApplicationPause；

    
  //  byte numb = 0;
//    void OnApplicationFocus()
//    {

//        #if UNITY_EDITOR
//#elif UNITY_IPHONE || UNITY_ANDROID
//        // if (UISelectServer.Instance != null && UISelectServer.Instance.isstart)
//        {

//            if (numb>0)
//            {
//                if(numb%2==0)//离开
//                {
                   
//                    UIPromptBox.Instance.ShowLabel("请重新登陆");
//                  //  Destroy(gameObject);
//                }
//                else
//                {
//                    Debug.Log("离开");
//                    ClientSendDataMgr.GetSingle().GetHeroSend().SendQuitGame();//通知服务器存盘
//                }
                

//            }
//            numb++;

//        }
//#endif
//    }

    void OnLevelWasLoaded ()
    {
        //Debug.LogError(SceneManager.GetActiveScene().name);
    }

    void OnApplicationQuit ()
    {
       // Debug.LogError("程序退出函数");
       ClientSendDataMgr.GetSingle().GetHeroSend().SendQuitGame();//通知服务器存盘
     //   Application.Quit();
        if(ClientNetMgr.GetSingle().IsConnect())
        {
            ClientNetMgr.GetSingle().Close();
        }

    }
}
