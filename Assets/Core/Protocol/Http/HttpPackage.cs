using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum HttpState
{
    GET, POST
}

public class HttpPackage: MonoBehaviour
{

    public string url = string.Empty;

    private static HttpPackage instance;
    private Dictionary<string, string> dic = new Dictionary<string, string>();

    public static HttpPackage Instance()
    {
        if(instance == null) instance = new HttpPackage();
        return instance;
    }

    /// <summary>
    /// 传递值对
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void TransmitValue(string key, string value)
    {
        dic.Add(key, value);
    }

    /// <summary>
    /// 连接服务器，发送数据
    /// http://192.168.3.98/mp/ac/serverList.php  
    /// </summary>
    public void ConnectServer(string url, Action<string> callback, HttpState state = HttpState.GET)
    {
        if(state == HttpState.GET)
        {
            StartCoroutine(GET(url, callback));
        }
        else
        {
            StartCoroutine(POST(url, callback));
        }
    }

    //GET请求
    IEnumerator GET(string url, Action<string> callback = null)
    {

        string link = "?";
        int c = 0;
        foreach(KeyValuePair<string, string> item in dic)
        {
            if(c == 0)
            {
                link += item.Key + "=" + item.Value;
                c++;
            }
            else
            {
                link += "&" + item.Key + "=" + item.Value;
            }
        }

        WWW www = new WWW(url + link);
        yield return www;

        if(www.error != null)
        {
           // Debug.Log("error is :" + www.error);

        }
        else
        {
            if(callback != null) callback(www.text);
        }
    }

    //POST请求
    IEnumerator POST(string url, Action<string> callback = null, int state = 0)
    {
        WWW www;
        if(state == 0)
        {
            WWWForm form = new WWWForm();

            foreach(KeyValuePair<string, string> item in dic)
            {
                form.AddField(item.Key, item.Value);
            }

            www = new WWW(url, form);

        }
        else
        {
            WWWForm form = new WWWForm();

            string valueString = "";
            foreach(KeyValuePair<string, string> item in dic)
            {
                //按照规则将key，value组成字符串
                valueString += item.Key + ":" + item.Value + "|";
            }

            byte[] byteStream = System.Text.Encoding.Default.GetBytes(valueString);

            //表示登录功能
            form.AddBinaryData("login", byteStream);

            www = new WWW(url, form);
        }

        yield return www;

        if(www.error != null)
        {
          //  Debug.Log("error is :" + www.error);

        }
        else
        {
            if(callback != null) callback(www.text);
        }
    }


}