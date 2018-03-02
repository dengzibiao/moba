using UnityEngine;
public class BF_DelayEffective : SkillBuff
{
    private GameObject prefab;
    public BF_DelayEffective(float baseVal, object p) : base(baseVal, p)
    {
        prefab = Resources.Load(GameLibrary.Effect_Buff + "delayHit") as GameObject;
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        if (cs != null && !cs.isDie)
        {
            for (int i = 0; i < talentBuff.Count; i++)
            {
                SkillBuffManager.GetInst().AddBuffs(talentBuff[i].baseValue,
                    new object[2] { talentBuff[i].id, talentBuff[i].last }, cs, attacker, talentBuff[i].skillNode, talentBuff[i].lvl);
                if (talentBuff[i].baseValue != 0)
                {
                    int hpResult = GameLibrary.Instance().CalcSkillDamage(talentBuff[i].baseValue, node.damageType, talentBuff[i].lvl, cs, attacker);
                    cs.Hp(hpResult, cs.state == Modestatus.Player ? HUDType.DamagePlayer : HUDType.DamageEnemy, showHud);
                }
                if (prefab != null)
                {
                    GameObject hitGo = NGUITools.AddChild(cs.mHitPoint == null ? cs.gameObject : cs.mHitPoint.gameObject, prefab);
                    GameObject.Destroy(hitGo, 1.5f);
                }
            }
        }
    }
}