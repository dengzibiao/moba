using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class NPCManager
{
    private static NPCManager single;
    public static NPCManager GetSingle()
    {
        if (single == null)
        {
            single = new NPCManager();
        }
        return single;
    }

    public Dictionary<long, GameObject> npcCon = new Dictionary<long, GameObject>();
    public Dictionary<long, GameObject> npcNameDic = new Dictionary<long, GameObject>();


    public void AddNpcNameModel(long key,GameObject go)
    {
        if (npcNameDic.ContainsKey(key))
        {
            npcNameDic[key] = go;

        }
        else
        {
            npcNameDic.Add(key, go);
        }
    }
    public void CreateNpcModel(long npcid, GameObject go)
    {
        if (!npcCon.ContainsKey(npcid) || npcCon[npcid] == null)
        {
            if (npcCon.ContainsKey(npcid))
            {
                npcCon[npcid] = go;
            }
            else
            {
                npcCon.Add(npcid, go);
            }
        }
    }

    public GameObject GetNpcObj(long npcid)
    {
        if (npcCon.ContainsKey(npcid))
        {
            return npcCon[npcid];
        }
        Debug.Log("this Npc Model Not Create npcid:" + npcid + " Please Check Secence Setting");
        return null;
    }

    public List<int> GetNpcIdList()
    {
        List<int> list = new List<int>();
        foreach (int id in npcCon.Keys)
        {
            list.Add(id);
        }
        return list;
    }
}
