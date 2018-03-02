using UnityEngine;
using System.Collections;

public class SDKForAndroid : SDKBase
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaClass jc;
    public SDKForAndroid() : base()
    {
        jc = new AndroidJavaClass("com.sdk.API");
    }
    public override void Init(int flag = 0)
    {
        if (jc!=null)
        {
            jc.CallStatic("Init",flag);
        }
        
    }

    public override void Login()
    {
        if (jc != null)
        {
            jc.CallStatic("Login");
        }
    }

    
    public override void QQLogin()
    {
        if (jc != null)
        {
            jc.CallStatic("QQLogin");
        }
    }

    
    public override void WXLogin()
    {
        if (jc != null)
        {
            jc.CallStatic("WXLogin");
        }
    }

    public override void LogOut()
    {
        if (jc != null)
        {
            jc.CallStatic("LogOut");
        }
    }

    public override void Pay(int num, string orderId)
    {
        if (jc != null)
        {
            jc.CallStatic("Pay",num,orderId);
        }
    }

    public override void GameCenter()
    {
        if (jc != null)
        {
            jc.CallStatic("GameCenter");
        }
    }
#endif
}
