using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;
using System;

public class TitleNode : FSDataNodeBase
{
    /// <summary>
    /// 称号id
    /// </summary>
    public int titleid;
    /// <summary>
    /// 称号名称
    /// </summary>
    public string titlename;
    /// <summary>
    /// 称号说明
    /// </summary>
    public string des;
    /// <summary>
    /// 称号类型 0：成长；1：战斗；2：充值；
    /// </summary>
    public int type; 
    /// <summary> 
    /// 解锁条件 0：默认；1：战队等级；2：拥有英雄数量；3：角斗场排名（结算后）；4：竞技场累计胜利次数；5：竞技场排名（结算后）；6：总战力排名；
    /// </summary>
    public int unlockcondition;
    /// <summary>
    /// 解锁参数 
    /// </summary> 
    public int[] unlockparameters;
    /// <summary>
    /// 有效时间（小时） 0为永久
    /// </summary>
    public int time;
    /// <summary>
    /// 力量
    /// </summary>
    public int power;
    /// <summary>
    /// 智力
    /// </summary>
    public int intelligence;
    /// <summary>
    /// 敏捷
    /// </summary>
    public int agility;
    /// <summary>
    /// 生命值
    /// </summary>
    public int hp;
    /// <summary>
    /// 攻击
    /// </summary>
    public int attack;
    /// <summary>
    /// 护甲
    /// </summary>
    public int armor;
    /// <summary>
    /// 魔抗
    /// </summary>
    public int magicresist;
    /// <summary>
    /// 暴击 
    /// </summary>
    public int critical;
    /// <summary>
    /// 闪避
    /// </summary>
    public int dodge;
    /// <summary>
    /// 命中
    /// </summary>
    public int hitratio;
    /// <summary>
    /// 护甲穿透
    /// </summary>
    public int armorpenetration;
    /// <summary>
    /// 魔法穿透
    /// </summary>
    public int magic_penetration;
    /// <summary>
    /// 吸血
    /// </summary>
    public int suck_blood;
    /// <summary>
    /// 韧性
    /// </summary>
    public int tenacity;
    /// <summary>
    /// 初始移动速度
    /// </summary>
    public int movementspeed;
    /// <summary>
    /// 初始攻击速度
    /// </summary>
    public int attackspeed;
    /// <summary>
    /// 英雄可视范围
    /// </summary>
    public int strikingdistance;
    /// <summary>
    /// 称号图片名字
    /// </summary>
    public string titleiconname;

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        titleid = int.Parse(item["title_id"].ToString());
        titlename = item["title_name"].ToString();
        des = item["title_explain"].ToString();
        type = int.Parse(item["title_type"].ToString());
        unlockcondition = int.Parse(item["unlock_condition"].ToString());

        int[] nodeIntarr = item["unlock_parameters"] as int[];
        if(nodeIntarr!=null)
        {
            unlockparameters = new int[nodeIntarr.Length];
            for (int m = 0; m < nodeIntarr.Length; m++)
            {
                unlockparameters[m] = nodeIntarr[m];
            }
        }
        time = int.Parse(item["time"].ToString());
        power = int.Parse(item["power"].ToString());
        intelligence = int.Parse(item["intelligence"].ToString());
        agility = int.Parse(item["agility"].ToString());
        hp = int.Parse(item["hp"].ToString());
        attack = int.Parse(item["attack"].ToString());
        armor = int.Parse(item["armor"].ToString());
        magicresist = int.Parse(item["magic_resist"].ToString());
        critical = int.Parse(item["critical"].ToString());
        dodge = int.Parse(item["dodge"].ToString());
        hitratio = int.Parse(item["hit_ratio"].ToString());
        armorpenetration = int.Parse(item["armor_penetration"].ToString());
        magic_penetration = int.Parse(item["magic_penetration"].ToString());
        suck_blood = int.Parse(item["suck_blood"].ToString());
        tenacity = int.Parse(item["tenacity"].ToString());
        movementspeed = int.Parse(item["movement_speed"].ToString());
        attackspeed = int.Parse(item["attack_speed"].ToString());
        strikingdistance = int.Parse(item["striking_distance"].ToString());
        titleiconname = item["title_icon"].ToString();


    }
}
