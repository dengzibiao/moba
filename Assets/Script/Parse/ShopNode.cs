using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
/// <summary>
/// 商店表
/// </summary>
public class ShopNode :FSDataNodeBase {

    public int id;
    public string npcDialogue;
    public string saleShow;
	public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        if (item["id"]!=null)
        {
            id = int.Parse(item["id"].ToString());
        }
        if (item["npc_dialogue"]!=null)
        {
            npcDialogue = item["npc_dialogue"].ToString();
        }
        if (item["sale_show"]!=null)
        {
            saleShow = item["sale_show"].ToString();
        }

    }
}
