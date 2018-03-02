using System.Collections.Generic;
using UnityEngine;
using Tianyu;
public class TaskDataNode : FSDataNodeBase
{

    private int taskid;
    private byte tasktype;//任务类型 主线 支线

    private byte requiretype;//任务要求类型

    //1对话任务npc 2通关任务有副本id  3采集（采集表id） 4提升技能等级5提升英雄装备等级 6杀怪（id和数量 ） 7怪物掉落（采集表id） 8 背包物品（物品id和数量）   9指定地点 (道具使用表id)
    private long opt1;//通关副本id 或者 npcid  ----->  1  2任务
    private long opt2;//采集表id 或者 使用道具表id ----->    7 9 任务
    private long[] opt5;//采集表id数组
    private Dictionary<long, int> idAndcountDic = new Dictionary<long, int>();//怪物id和数量 或者 背包物品id和数量  ------->6  8任务
    private int level;//用户战队等级

    private int place1;
    private int condition1;
    private int setup1;//0:不需要临时创建英雄  1 ：需临时创建出和玩家一起作战的英雄 2：临时创建npc 仅可移动和对话 3：临时创建怪物 仅玩家看的见 可与其对话 对话完进入战斗模式
    private Dictionary<long, int> setup1IDAndLevelDic = new Dictionary<long, int>();//临时创建英雄的id（是怪物表id）  和 等级
    private long opt3;//需要临时创建第一个npcid

    private int place2;
    private int condition2;
    private int setup2;
    private Dictionary<long, int> setup2IDAndLevelDic = new Dictionary<long, int>();
    private long opt4;//需要临时创建第二个npcid

    private int lines;//剧情台词表现 0：无 1：有 这时候通过任务id去读剧情台词变现表

    private int prepose;//前置任务
    private int follow;//后置任务
    private long acceptnpc;//接任务npcid
    private long finishnpc;//交任务npcid
    private string taskname;//任务名称
    private string taskinfo;//任务简介
    private string require;//任务要求
    private int[,] reward_prop;//奖励道具[道具id,数量]    1 战队经验、2 Gold、3 Diamond
    private int[] placeArr;//按place的顺序区分是第一个人物 还是第二个人物
    private int mapID;
    private int task_effects;
    #region GetAndSet
    public int Taskid
    {
        get { return taskid; }
    }
    public byte Type
    {
        get { return tasktype; }
    }
    /*
        1：对话；
        2：通关副本；
        3：采集；
        4：提升技能等级；
        5：提升英雄装备等级；
        6：杀怪；
        7：怪物掉落物；
        8：背包物品；
        9：指定地点；
     */
    public byte Requiretype
    {
        get { return requiretype; }
    }
    public int Level
    {
        get { return level; }
    }

    public int Prepose
    {
        get { return prepose; }
    }
    public int Follow
    {
        get { return follow; }
    }
    public long Acceptnpc
    {
        get { return acceptnpc; }
    }
    public long Finishnpc
    {
        get { return finishnpc; }
    }
    public string Taskname
    {
        get { return taskname; }
    }
    public string Require
    {
        get { return require; }
    }
    public string Taskinfo
    {
        get { return taskinfo; }
    }
    public int[,] Reward_prop
    {
        get { return reward_prop; }
    }
    public long Opt1
    {
        get
        {
            return opt1;
        }

        set
        {
            opt1 = value;
        }
    }

    public long Opt2
    {
        get
        {
            return opt2;
        }

        set
        {
            opt2 = value;
        }
    }

    public Dictionary<long, int> IdAndcountDic
    {
        get
        {
            return idAndcountDic;
        }

        set
        {
            idAndcountDic = value;
        }
    }


    public long Opt3
    {
        get
        {
            return opt3;
        }

        set
        {
            opt3 = value;
        }
    }

    public int Lines
    {
        get
        {
            return lines;
        }

        set
        {
            lines = value;
        }
    }

    public int Place1
    {
        get
        {
            return place1;
        }

        set
        {
            place1 = value;
        }
    }

    public int Condition1
    {
        get
        {
            return condition1;
        }

        set
        {
            condition1 = value;
        }
    }

    public int Place2
    {
        get
        {
            return place2;
        }

        set
        {
            place2 = value;
        }
    }

    public int Condition2
    {
        get
        {
            return condition2;
        }

        set
        {
            condition2 = value;
        }
    }

    public int Setup2
    {
        get
        {
            return setup2;
        }

        set
        {
            setup2 = value;
        }
    }

    public Dictionary<long, int> Setup2IDAndLevelDic
    {
        get
        {
            return setup2IDAndLevelDic;
        }

        set
        {
            setup2IDAndLevelDic = value;
        }
    }

    public long Opt4
    {
        get
        {
            return opt4;
        }

        set
        {
            opt4 = value;
        }
    }

    public int Setup1
    {
        get
        {
            return setup1;
        }

        set
        {
            setup1 = value;
        }
    }

    public Dictionary<long, int> Setup1IDAndLevelDic
    {
        get
        {
            return setup1IDAndLevelDic;
        }

        set
        {
            setup1IDAndLevelDic = value;
        }
    }

    public int[] PlaceArr
    {
        get
        {
            return placeArr;
        }

        set
        {
            placeArr = value;
        }
    }

    public long[] Opt5
    {
        get
        {
            return opt5;
        }

        set
        {
            opt5 = value;
        }
    }

    public int MapID
    {
        get
        {
            return mapID;
        }

        set
        {
            mapID = value;
        }
    }

    public int Task_effects
    {
        get
        {
            return task_effects;
        }

        set
        {
            task_effects = value;
        }
    }


    #endregion
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        taskid = int.Parse(item["taskid"].ToString());
        tasktype = byte.Parse(item["task_type"].ToString());
        requiretype = byte.Parse(item["require_type"].ToString());
        level = int.Parse(item["level"].ToString());
        prepose = int.Parse(item["prepose"].ToString());
        follow = int.Parse(item["follow"].ToString());
        acceptnpc = long.Parse(item["accept_npc"].ToString());
        finishnpc = long.Parse(item["finish_npc"].ToString());
        taskname = item["task_name"].ToString();
        taskinfo = item["info"].ToString();
        require = item["require"].ToString();
        setup1 = int.Parse(item["set_up1"].ToString());
        place1 = int.Parse(item["place1"].ToString());
        condition1 = int.Parse(item["condition1"].ToString());
        mapID = int.Parse(item["map_id"].ToString());

        place2 = int.Parse(item["place2"].ToString());
        condition2 = int.Parse(item["condition2"].ToString());
        setup2 = int.Parse(item["set_up2"].ToString());
        task_effects = int.Parse(item["Task_effects"].ToString());


        object[] nodeCond = (object[])item["reward_prop"];
        reward_prop = new int[nodeCond.Length, 2];

        if (nodeCond.Length > 0)
        {
            for (int i = 0; i < nodeCond.Length; i++)
            {
                int[] node = (int[])nodeCond[i];

                for (int j = 0; j < node.Length; j++)
                {
                    reward_prop[i, j] = node[j];
                }
            }
        }


        if (requiretype == 1 || requiretype == 2)
        {
            opt1 = long.Parse(item["Tracking_index"].ToString());
        }
        else if (requiretype == 3)
        {
            long[] nodeEquip = item["Tracking_index"] as long[];
            if (nodeEquip != null)
            {
                opt5 = new long[nodeEquip.Length];
                for (int m = 0; m < nodeEquip.Length; m++)
                {
                    opt5[m] = nodeEquip[m];
                }
            }
        }
        else if (requiretype == 7 || requiretype == 9)
        {
            opt2 = long.Parse(item["Tracking_index"].ToString());
        }
        else if (requiretype == 6 || requiretype == 8)
        {
            object[] itemObject = (object[])item["Tracking_index"];
            if (itemObject.Length > 0)
            {
                for (int i = 0; i < itemObject.Length; i++)
                {
                    int[] intarr = itemObject[i] as int[];
                    if (IdAndcountDic.ContainsKey(intarr[0]))
                    {
                        IdAndcountDic[intarr[0]] = intarr[1];
                    }
                    else
                    {
                        idAndcountDic.Add(intarr[0], intarr[1]);
                    }
                }
            }

        }

        if (setup1 == 1 || setup1 == 3)
        {
            object[] allObj = (object[])item["all_monster1"];
            if (allObj.Length > 0)
            {
                for (int i = 0; i < allObj.Length; i++)
                {
                    int[] arr = allObj[i] as int[];
                    if (setup1IDAndLevelDic.ContainsKey(arr[0]))
                    {
                        setup1IDAndLevelDic[arr[0]] = arr[1];
                    }
                    else
                    {
                        setup1IDAndLevelDic.Add(arr[0], arr[1]);
                    }
                }
            }
        }
        else if (setup1 == 2)
        {
            opt3 = long.Parse(item["all_monster1"].ToString());
        }
        else if (setup1 == 0)
        {

        }

        lines = int.Parse(item["lines"].ToString());


        if (setup2 == 1 || setup2 == 3)
        {
            object[] allObj2 = (object[])item["all_monster2"];
            if (allObj2.Length > 0)
            {
                for (int i = 0; i < allObj2.Length; i++)
                {
                    int[] arr = allObj2[i] as int[];
                    if (setup2IDAndLevelDic.ContainsKey(arr[0]))
                    {
                        setup2IDAndLevelDic[arr[0]] = arr[1];
                    }
                    else
                    {
                        setup2IDAndLevelDic.Add(arr[0], arr[1]);
                    }
                }
            }
        }
        else if (setup2 == 2)
        {
            opt4 = long.Parse(item["all_monster2"].ToString());
        }
        else if(setup1 == 0)
        {


        }
        PlaceArr = new int[2] {place1,place2};
    }
}

