using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;
/// <summary>
/// 任务奖励 （主线 日常 悬赏...）
/// </summary>
public class TaskRewardNode : FSDataNodeBase
{
    private int id;
    private int[,] itemArr;
    private int taskType;
    private int gold;
    private int diamond;
    private int zhanduiExp;
    private int heroExp;
    private int xuanshangGold;

    /// <summary>
    /// 奖励id
    /// </summary>
    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }

    /// <summary>
    /// 类型
    /// </summary>
    public int TaskType
    {
        get
        {
            return taskType;
        }

        set
        {
            taskType = value;
        }
    }
    /// <summary>
    /// 金币
    /// </summary>
    public int Gold
    {
        get
        {
            return gold;
        }

        set
        {
            gold = value;
        }
    }
    /// <summary>
    /// 钻石
    /// </summary>
    public int Diamond
    {
        get
        {
            return diamond;
        }

        set
        {
            diamond = value;
        }
    }
    /// <summary>
    /// 战队经验
    /// </summary>
    public int ZhanduiExp
    {
        get
        {
            return zhanduiExp;
        }

        set
        {
            zhanduiExp = value;
        }
    }
    /// <summary>
    /// 英雄经验
    /// </summary>
    public int HeroExp
    {
        get
        {
            return heroExp;
        }

        set
        {
            heroExp = value;
        }
    }
    /// <summary>
    /// 悬赏币
    /// </summary>
    public int XuanshangGold
    {
        get
        {
            return xuanshangGold;
        }

        set
        {
            xuanshangGold = value;
        }
    }
    /// <summary>
    /// 奖励物品
    /// </summary>
    public int[,] ItemArr
    {
        get
        {
            return itemArr;
        }

        set
        {
            itemArr = value;
        }
    }

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id = int.Parse(item["id"].ToString());
        if (item.ContainsKey("item") && item["item"] != null)
        {
            object[] nodeCond = (object[])item["item"];
            ItemArr = new int[nodeCond.Length, 2];

            if (nodeCond.Length > 0)
            {
                for (int i = 0; i < nodeCond.Length; i++)
                {
                    int[] node = (int[])nodeCond[i];

                    for (int j = 0; j < node.Length; j++)
                    {
                        ItemArr[i, j] = node[j];
                    }
                }
            }
        }
        taskType = int.Parse(item["type"].ToString());
        gold = int.Parse(item["114000100"].ToString());
        diamond = int.Parse(item["114000200"].ToString());
        zhanduiExp = int.Parse(item["114000300"].ToString());
        heroExp = int.Parse(item["114000400"].ToString());
        xuanshangGold = int.Parse(item["114000800"].ToString());
    }
}
