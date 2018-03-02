using UnityEngine;
using Tianyu;
using System.Collections;
using System.Collections.Generic;

public class VipNode : FSDataNodeBase
{
  public   short id;             //vip等级
    public short sweep;              // 每天可免费领取扫荡券&W张。
    public short buy_power;          //每天可购买体力&W次。
    public short gold_pointing;      //每天可使用点金手&W次。
    public short reset_elite;        // 每天可重置精英关卡&W次。
    public short abattoir_ticket;    //每天可购买角斗场门票&W次。
    public short arena_ticket;       //每天可购买竞技场门票&W次。
    public short skill_limit;        //技能点上限增加至&W次。
    public short retroactive_limit;  //补签上限增加至&W次。
    public short friend_limit;       //好友上限增加至&W人。
    public short arena_frequency;    //竞技场免费翻牌&W次。

  
    public override void parseJson ( object jd )
    {
        Dictionary<string , object> item = ( Dictionary<string , object> ) jd;
        id = short.Parse( item ["id"].ToString() );
        sweep = short.Parse(item["sweep"].ToString());
        buy_power = short.Parse(item["buy_power"].ToString());

        gold_pointing = short.Parse(item["gold_pointing"].ToString());
        reset_elite = short.Parse(item["reset_elite"].ToString());
        abattoir_ticket = short.Parse(item["abattoir_ticket"].ToString());
        arena_ticket = short.Parse(item["arena_ticket"].ToString());
        skill_limit = short.Parse(item["skill_limit"].ToString());
        retroactive_limit = short.Parse(item["retroactive_limit"].ToString());
        friend_limit = short.Parse(item["friend_limit"].ToString());
        arena_frequency = short.Parse(item["arena_frequency"].ToString());
    }
}
