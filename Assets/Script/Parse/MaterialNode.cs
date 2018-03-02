using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class MaterialNode : FSDataNodeBase
{


    public long props_id;
    public string name;
    public byte types;
    public string describe;
    public short grade;//道具品质
    public long next_grade;
    public int[] cprice;//买入价格 [金币,钻石,龙鳞硬币,角斗士硬币,兄弟会币]
    public int sprice;//卖出价格 1：金币；2：钻石

   // public long[] be_equip;//可以合成的道具id 用[,]
    public long[] be_synth;//升级所需 用[id,数量]
    public int syn_cost;
    public long[,] syn_condition;//升级所需[id,numb]
    public short skill_point;//技能点增加
    public short exp_gain;//经验点增加
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
        next_grade = long.Parse(item["next_grade"].ToString());

        int[] nodeIntarr = item["cprice"] as int[];
        cprice = new int[nodeIntarr.Length];
        for (int m = 0; m < nodeIntarr.Length; m++)
        {
            cprice[m] = nodeIntarr[m];
        }

        sprice = short.Parse(item["sprice"].ToString());
        //int[] nodeEquip = (int[])item["be_equip"];
        //be_equip = new long[nodeEquip.Length];
        //for (int m = 0; m < nodeEquip.Length; m++)
        //{
        //    be_equip[m] = nodeEquip[m];
        //}      
        object[] nodeCond = (object[])item["syn_condition"];
        syn_condition = new long[nodeCond.Length, 2];

        if (nodeCond.Length > 0)
        {
           
            for (int i = 0; i < nodeCond.Length; i++)
            {
                int[] node = (int[])nodeCond[i];

                for (int j = 0; j < node.Length; j++)
                {
                    syn_condition[i, j] = node[j];
                }
            }
        }

        syn_cost = int.Parse(item["syn_cost"].ToString());
        if (item["drop_fb"] != null)
        {
            int[] dropIntarr = item["drop_fb"] as int[];
            drop_fb = new int[dropIntarr.Length];
            for (int m = 0; m < dropIntarr.Length; m++)
            {
                drop_fb[m] = dropIntarr[m];
            }
        }
        //  icon_atlas = item["icon_atlas"].ToString();
        icon_name = item["icon_name"].ToString();
        released = byte.Parse(item["released"].ToString());
        // lv_limit = int.Parse(item["lv_limit"].ToString());
        piles = short.Parse(item["piles"].ToString());
    }
    public void setData(ref   ItemNodeState itemstate)
    {
        itemstate.props_id = props_id;
        itemstate.name =name;
        itemstate.types = types;
        itemstate.describe = describe;
        itemstate.grade = grade;
        itemstate.next_grade = next_grade;
        itemstate.cprice = cprice;
        itemstate.sprice = sprice;
        itemstate.syn_condition = syn_condition;
        itemstate.be_synth = be_synth;
        itemstate.syn_cost = syn_cost;
        itemstate.drop_fb = drop_fb;
        //  icon_atlas = item["icon_atlas"].ToString();
        itemstate. icon_name = icon_name;
        itemstate.released = released;
        // lv_limit = int.Parse(item["lv_limit"].ToString());
        itemstate.piles = piles;
    }
}
