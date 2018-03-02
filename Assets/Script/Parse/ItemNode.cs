using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class ItemNode : FSDataNodeBase
{
    public double[,] syn_condition;
    public long props_id;
    public string name;
    public byte types;
    public string describe;
    public short grade;
    public int[] cprice;//买入价格 [金币,钻石,龙鳞硬币,角斗士硬币,兄弟会币]
    public int insprice;//卖出价格 1：金币；2：钻石
    public short skill_point;//技能点增加
    public short exp_gain;//经验点增加
    public short power_add;//体力增加
    public int[] drop_fb;//物品产出类型 0：没有出处；1：掉落副本；2：竞技场；3：公会；4：远征 
    public string icon_name;//图标名称
    public byte released;  //当前版本是否开放 0 : No，1：Yes 
    public short piles;//堆叠数量：默认999



    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = jd as Dictionary<string, object>;
        props_id = long.Parse(item["props_id"].ToString());
        name = item["name"].ToString();
        types = byte.Parse(item["types"].ToString());
        describe = item["describe"].ToString();
        grade = short.Parse(item["grade"].ToString());
        //  next_grade = long.Parse(item["next_grade"].ToString());
        int[] nodeIntarr = item["cprice"] as int[];
        cprice = new int[nodeIntarr.Length];
        for (int m = 0; m < nodeIntarr.Length; m++)
        {
            cprice[m] = nodeIntarr[m];
        }
        insprice = int.Parse(item["sprice"].ToString());
      
        skill_point = short.Parse(item["skill_point"].ToString());
        exp_gain = short.Parse(item["exp_gain"].ToString());
        power_add = short.Parse(item["power_add"].ToString());
        if (item["drop_fb"] != null)
        {
            int[] dropIntarr = item["drop_fb"] as int[];
            if (dropIntarr!=null)
            {
                drop_fb = new int[dropIntarr.Length];
                for (int m = 0; m < dropIntarr.Length; m++)
                {
                    drop_fb[m] = dropIntarr[m];
                }
            }
           
        }
        //  icon_atlas = item["icon_atlas"].ToString();
        icon_name = item["icon_name"].ToString();
        released = byte.Parse(item["released"].ToString());
        piles = short.Parse(item["piles"].ToString());
    }

    public void setData(ref ItemNodeState itemstate)
    {
        itemstate.props_id = props_id;
        itemstate.name = name;
        itemstate.types = types;
        itemstate.describe = describe;
        itemstate.grade = grade;
        itemstate.cprice = cprice;
        itemstate.sprice = insprice;
        itemstate.skill_point = skill_point;
        itemstate.exp_gain = exp_gain;
        itemstate.power_add = power_add;
        itemstate.drop_fb = drop_fb;
        itemstate.icon_name = icon_name;
        itemstate.released = released;
        itemstate.piles = piles;
    }
}
