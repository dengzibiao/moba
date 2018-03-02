using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Tianyu;
public class PlayerNameNode : FSDataNodeBase
{


    public int id;
    public string name;
    public string sign;
    public string surname;
    public override void parseJson(object jd)
    {
        Dictionary<string, object> nameDic = (Dictionary<string, object>)jd;
        id = int.Parse(nameDic["id"].ToString());
        if (nameDic["name"]!=null)
        {
            name = nameDic["name"].ToString();
        }
        if (nameDic["sign"]!=null)
        {
            sign = nameDic["sign"].ToString();

        }
        if (nameDic["surname"]!=null)
        {
            surname = nameDic["surname"].ToString();

        }
    }
}
