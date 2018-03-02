using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class RuneNode : FSDataNodeBase
{    
    public long props_id;//道具id
    public string name;//道具名字
    public byte types;//道具类型 1：装备，2：材料，3：材料碎片，4：金币道具，5：经验道具，6：英雄灵魂石，7：英雄整卡，8：符文，9：恢复道具，10：其他，11：宝箱。(4,5,9,11为消耗品)
    public string describe;//道具介绍
    public short grade;//道具品质
    public long next_grade;//进阶结果
    public int[] cprice;//买入价格 [金币,钻石,龙鳞硬币,角斗士硬币,兄弟会币]
    public int sprice;//卖出价格 1：金币；2：钻石
    public short power;//力量加成
    public short intelligence;//智力加成
    public short agility;//敏捷加成
    public short hp;//生命值加成
    public short attack;//攻击加成
    public short armor;//护甲加成
    public short magic_resist;//魔抗加成
    public short critical;//暴击加成
    public short dodge;//闪避加成
    public short hit_ratio;   //命中加成
    public short armor_penetration;//护甲穿透加成
    public short magic_penetration;//魔法穿透加成
    public short suck_blood;//吸血加成
    public short tenacity;//韧性加成
    public short movement_speed;//移动速度加成
    public short attack_speed;//攻击速度加成
    public short striking_distance;//攻击距离加成
    //public long[] be_equip;//可装备英雄
  //  public long[] be_synth;//可合成装备
    public long[,] syn_condition;//升级所需 用[id,数量]
    public int syn_cost; //合成花费金币

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
        power = short.Parse(item["power"].ToString());
        intelligence = short.Parse(item["intelligence"].ToString());
        agility = short.Parse(item["agility"].ToString());
        hp = short.Parse(item["hp"].ToString());
        attack = short.Parse(item["attack"].ToString());
        armor = short.Parse(item["armor"].ToString());
        magic_resist = short.Parse(item["magic_resist"].ToString());
        critical = short.Parse(item["critical"].ToString());
        dodge = short.Parse(item["dodge"].ToString());
        hit_ratio = short.Parse(item["hit_ratio"].ToString());
        armor_penetration = short.Parse(item["armor_penetration"].ToString());
        magic_penetration = short.Parse(item["magic_penetration"].ToString());
        suck_blood = short.Parse(item["suck_blood"].ToString());
        tenacity = short.Parse(item["tenacity"].ToString());
        movement_speed = short.Parse(item["movement_speed"].ToString());
        attack_speed = short.Parse(item["attack_speed"].ToString());
        striking_distance = short.Parse(item["striking_distance"].ToString());

        object[] nodeCond = item["syn_condition"]as object[];
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
        piles = short.Parse(item["piles"].ToString());
    }
    public void setData(ref ItemNodeState itemstate)
    {
        itemstate.props_id = props_id;
        itemstate.name = name;
        itemstate.types = types;
        itemstate.describe = describe;
        itemstate.grade = grade;
        itemstate.next_grade = next_grade;
        itemstate.cprice = cprice;
        itemstate.sprice = sprice;
        itemstate.power =  power;//力量加成
        itemstate.intelligence =  intelligence;//智力加成
        itemstate.agility =  agility;//敏捷加成
        itemstate.hp = hp;//生命值加成
        itemstate.attack = attack;//攻击加成
        itemstate.armor =  armor;//护甲加成
        itemstate.magic_resist =  magic_resist;//魔抗加成
        itemstate.critical =  critical;//暴击加成
        itemstate.dodge =  dodge;//闪避加成
        itemstate.hit_ratio =  hit_ratio;   //命中加成
        itemstate.armor_penetration = armor_penetration;//护甲穿透加成
        itemstate.magic_penetration =magic_penetration;//魔法穿透加成
        itemstate.suck_blood = suck_blood;//吸血加成
        itemstate.tenacity =  tenacity;//韧性加成
        itemstate.movement_speed =  movement_speed;//移动速度加成
        itemstate.attack_speed =  attack_speed;//攻击速度加成
        itemstate.striking_distance =striking_distance;//攻击距离加成
        //itemstate.be_equip =be_equip;//可装备英雄
        //itemstate.be_synth =  be_synth;//可合成装备
        itemstate.syn_condition = syn_condition;//升级所需 用[id,数量]


        itemstate.syn_cost = syn_cost;
        itemstate.drop_fb = drop_fb;
        //  icon_atlas = item["icon_atlas"].ToString();
        itemstate.icon_name = icon_name;
        itemstate.released = released;
        itemstate.piles = piles;
    }
}
