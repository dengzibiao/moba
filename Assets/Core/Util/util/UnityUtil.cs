using UnityEngine;
using System.Collections;

public static class UnityUtil
{

    /// <summary>
    /// 设置本地坐标, xyz带符号表示加减
    /// </summary>
    public static void SetLocalVector3(this Transform trans, float x = 0f, float y = 0f, float z = 0f)
    {
        trans.localPosition = new Vector3(trans.localPosition.x + x, trans.localPosition.y + y, trans.localPosition.z + z);
    }

    /// <summary>
    /// 设置坐标反转,1:不反转 -1:反转
    /// </summary>
    public static void ReverScale(this Transform trans, int x, int y, int z)
    {
        trans.localScale = new Vector3(x * trans.localScale.x, y * trans.localScale.y, z * trans.localScale.z);
    }

    /// <summary>
    /// 设置隐藏OR显示
    /// </summary>
    public static void ShowOrHide(this GameObject go)
    {
        go.SetActive(!go.activeSelf);
    }
    public static void ShowOrHide(this GameObject go, GameObject g)
    {
        go.SetActive(!g.activeSelf);
    }

    public static T FindComponent<T>(this Transform trans, string name)
    {
  
      //  if (trans != null && trans.FindChild(name) != null)
            return trans.FindChild(name).GetComponent<T>();
  
    }

    public static T FindCtrl<T>(GameObject go, string name) where T : Component
    {
        GameObject obj = FindObjectRecursively(go, name);

        if (obj == null)
            return null;
        return obj.GetComponent<T>();
    }

    public static GameObject FindObjectRecursively(GameObject go, string name)
    {
        Transform t = null;
        GameObject o = null;
        if (go == null)
        {
          //  Debug.Log("gameobject is null ~~Name === " + name);
        }
        for (int i = 0; i < go.transform.childCount; i++)
        {
            t = go.transform.GetChild(i);
            if (t.name == name)
                return t.gameObject;

            o = FindObjectRecursively(t.gameObject, name);
            if (o != null)
                return o;
        }
        return null;
    }

    public static T AddComponetIfNull<T> ( GameObject go ) where T : Component
    {
        T comp = null;
        if (go != null)
        {
            comp = go.GetComponent<T>();
        }
        if(comp == null)
        {
            comp = go.AddComponent<T>();
        }
        return comp;
    }

    public static void SetBtnState(GameObject btn, bool isEnable)
    {
        if (btn.GetComponent<UIButton>())
            btn.GetComponent<UIButton>().enabled = isEnable;
        btn.GetComponent<UISprite>().color = isEnable ? new Color(1, 1, 1) : new Color(0, 0, 0);
        btn.GetComponent<BoxCollider>().enabled = isEnable;
    }
}
