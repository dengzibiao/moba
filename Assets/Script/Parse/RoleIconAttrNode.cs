using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Tianyu;
public class RoleIconAttrNode : FSDataNodeBase {

    public long icon_id;//id
    public string icon_name;//头像名字

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        icon_id=long.Parse(item["icon_id"].ToString());
        icon_name = item["icon_name"].ToString();
    }
}
