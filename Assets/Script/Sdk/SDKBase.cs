using UnityEngine;
using System.Collections;

public class SDKBase
{
    public virtual void Init(int flag=0)
    { }

    public virtual void Login()
    { }

    public virtual void QQLogin()
    { }

    public virtual void WXLogin()
    { }
    public virtual void LogOut()
    { }

    public virtual void Pay(int num,string orderId)
    { }

    public virtual void GameCenter()
    { }
}
