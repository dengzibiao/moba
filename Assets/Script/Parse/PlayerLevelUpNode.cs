using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;
using System;

public class PlayerLevelUpNode : FSDataNodeBase
{
    /// <summary>
    /// id
    /// </summary>
    public int id;
    /// <summary>
    /// 升到下一级所需要的经验
    /// </summary>
    public int exp;
    /// <summary>
    /// 英雄级别上限
    /// </summary>
    public int heroLvLimit;
    /// <summary>
    /// 战队等级提升奖励的体力
    /// </summary>
    public int rewardPower;
    /// <summary>
    /// 体力上限
    /// </summary>
    public int maxPower;
    /// <summary>
    /// 好友上限
    /// </summary>
    public int maxFiend;
    /// <summary>
    /// 活力上限
    /// </summary>
    public int maxVitality;
    /// <summary>
    /// 战队等级提升奖励的体力
    /// </summary>
    public int power_reward;
    /// <summary>
    /// 营地复活CD时间
    /// </summary>
    public int resurrection_cd;

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id = int.Parse(item["id"].ToString());
        exp = int.Parse(item["exp"].ToString());
        heroLvLimit = int.Parse(item["hero_lv_limit"].ToString());
        rewardPower = int.Parse(item["power_reward"].ToString());
        maxPower = int.Parse(item["max_power"].ToString());
        maxFiend = int.Parse(item["max_friend"].ToString());
        maxVitality = int.Parse(item["max_vitality"].ToString());
        power_reward = int.Parse(item["power_reward"].ToString());
        resurrection_cd = int.Parse(item["resurrection_cd"].ToString());
    }
}
