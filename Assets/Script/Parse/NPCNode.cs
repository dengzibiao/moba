using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;

public class NPCNode : FSDataNodeBase
{
    public long npcid;//npcid
    public int sn;//流水号
    public int npcscript;//npc脚本
    public int type;//npc类型
    public string npcname;//npc 名称
    public string ana;//固定语录
    public int modelid ;//模型id
    public int mapid;//场景id
    public Vector3 pos;//位置
    public Quaternion rota;//转向
    public string[] info;//闲时对话
    public string[] voice;//语音
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        npcid = long.Parse(item["key"].ToString());
        sn = int.Parse(item["sn"].ToString());
        npcscript = int.Parse(item["npcscript"].ToString());
        type = int.Parse(item["type"].ToString());
        npcname = item["name"].ToString();
        if (item.ContainsKey("ana")&& item["ana"]!=null)
        {
            ana = item["ana"].ToString();
        }
        modelid =  int.Parse(item["modelid"].ToString());
        mapid = int.Parse(item["mapid"].ToString());
        //pos = new Vector3(float.Parse(item["coordinate1"].ToString()), float.Parse(item["coordinate2"].ToString()), float.Parse(item["coordinate3"].ToString()));
        //rota = Quaternion.Euler(0, float.Parse(item["rotation"].ToString()), 0);
        info = item["info"].ToString().Split('|');
        if (item.ContainsKey("voice"))
        {
            voice = item["voice"].ToString().Split('|');
        }
        
    }
}
