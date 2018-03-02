using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Tianyu;
public class ShoucangData
{
    public long id;
    public string uuid;
    public int level;
    public int state;
}
public class MountAndPetNodeData
{
    public int seletIndex = 0;
    public bool ShowType = true;
    public List<UIMountNode> mplist = new List<UIMountNode>();
    public List<UIPetNode> petlist = new List<UIPetNode>();
    public static int[] MountAndPetlistl;
   
    public Dictionary<long, ShoucangData> haveMountlist = new Dictionary<long, ShoucangData>();//玩家已经拥有的坐骑
    public Dictionary<long, ShoucangData> havePetList = new Dictionary<long, ShoucangData>();//玩家已经拥有的宠物
    public long currentMountID = 0;//当前使用的坐骑
    public long currentPetID = 0;//当前使用的宠物
    public long goMountID = 0;//如果不为零 玩家一上线就要乘骑的坐骑
    public long godefPetID = 0;//如果不为零 玩家一上线就要带着的宠物
    /// <summary>
    /// 单例
    /// </summary>
    private static MountAndPetNodeData mSingleton;
    public static MountAndPetNodeData Instance()
    {
        if (mSingleton == null)
            mSingleton = new MountAndPetNodeData();
        return mSingleton;
    }
    public bool IsHaveThisMount(long id)
    {
        if (haveMountlist.ContainsKey(id))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsHaveThisPet(long id)
    {
        if (havePetList.ContainsKey(id))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public int GetMountOrPetCount(long id,MountAndPet type)
    {
        bool temp = false;
        int count = 0;
        switch (type)
        {
            case MountAndPet.Mount:

                temp = IsHaveThisMount(long.Parse(StringUtil.StrReplace(id.ToString(), "601", 0, 3)));
                count = temp ? 1 : 0;
                break;
            case MountAndPet.Pet:
                temp = IsHaveThisPet(long.Parse(StringUtil.StrReplace(id.ToString(), "701", 0, 3)));
                count = temp ? 1 : 0;
                break;
        }
        return count;
    }
    public int GetHeroCount(long id)
    {
        int count = 0;
        for (int i = 0; i < playerData.GetInstance().herodataList.Count; i++)
        {
            if (playerData.GetInstance().herodataList[i].id == long.Parse(StringUtil.StrReplace(id.ToString(), "201", 0, 3)))
            {
                count = 1;
                break;
            }
        }
        return count;
    }
    public List<UIMountNode> GetMPlist()
    {
        mplist.Clear();
        foreach (var item in FSDataNodeTable<UIMountNode>.GetSingleton().DataNodeList.Values)
        {
            mplist.Add(item);
        }
        return mplist;
    }
    public List<UIPetNode> Getpetlist()
    {
        petlist.Clear();
        foreach (var item in FSDataNodeTable<UIPetNode>.GetSingleton().DataNodeList.Values)
        {
            petlist.Add(item);
        }
        return petlist;
    }
    //public int[] GetMountAndPetlistl(int[] list)
    //{
    //    MountAndPetlistl = new int[list.Length];
    //    return
    //}
    public List<string> GetHerolist(int mountandpetType)
    {
        List<string> herolist = new List<string>();
        foreach (var item in FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.Values)
        {
            for (int i = 0; i < item.mount_types.Length; i++)
            {
                if (item.mount_types[i] == mountandpetType && item.released == 1)
                {
                    herolist.Add(item.hero_id.ToString());
                }
            }
        }
        return herolist;
    }
}
