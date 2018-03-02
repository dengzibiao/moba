using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CHeroSkillHandle : CHandleBase
{
    public CHeroSkillHandle ( CHandleMgr mgr ) : base( mgr )
    {
    }
    public override void RegistAllHandle ()
    {
        //RegistHandle( MessageID.HERO_S2C_HeroSkillList , HeroSkillListResultHandle );
        //RegistHandle( MessageID.UpGrade_S2C_HeroSkill , HeroSkillUpGradeResultHandle );
    }

    public bool HeroSkillListResultHandle ( CReadPacket packet )
    {
        return true;

        //Debug.Log( "SkillList result" );

        //Dictionary<string , object> data = packet.data;

        //int resolt = int.Parse( data [ "ret" ].ToString() );
        //if ( resolt == 0 )
        //{
        //    Dictionary<string , object> skillListDic = data [ "item" ] as Dictionary<string , object>;
        //    if (skillListDic.Count != 0)
        //    {

        //        HeroData hd = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);

        //        foreach (KeyValuePair<string, object> skill in skillListDic)
        //        {
        //            if (hd.skill.ContainsKey(long.Parse(skill.Key)))
        //                hd.skill[long.Parse(skill.Key)] = (int)skill.Value;
        //            else
        //                hd.skill.Add(long.Parse(skill.Key), (int)skill.Value);
        //        }

        //        //foreach (KeyValuePair<string, object> skill in skillListDic)
        //        //{
        //        //    GameLibrary.skillLevel[int.Parse(skill.Key)] = (int)skill.Value;
        //        //}
        //        UI_HeroDetail.instance.InitSkillData(Globe.selectHero.hero_id);
        //    }
        //    else
        //    {
        //        Debug.Log("Herolist count is 0!!");
        //    }

        //}
        //else
        //{
        //    Debug.Log( string.Format( "获取英雄技能列表失败：{0}" , data [ "desc" ].ToString() ) );
        //}
        //return true;
    }

    public bool HeroSkillUpGradeResultHandle ( CReadPacket packet )
    {
        //Debug.Log( "SkillList result" );
        //Dictionary<string , object> data = packet.data;

        //int resolt = int.Parse( data [ "ret" ].ToString() );
        //if ( resolt == 0 )
        //{
        //    Debug.Log("升级成功");
        //    int playerMoney = int.Parse( data [ "vv" ].ToString() );
        //    int moneyType = int.Parse( data [ "vt" ].ToString() );

        //    playerData.GetInstance().RoleMoneyHadler((MoneyType)moneyType, playerMoney);

        //}
        //else
        //{
        //    Debug.Log( string.Format( "技能升级失败：{0}" , data [ "desc" ].ToString() ) );
        //}
        return true;
    }
}
