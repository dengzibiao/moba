using UnityEngine;

public class BF_Restore : SkillBuff
{
    public CDTimer.CD restoreCd;
    public BF_Restore(float baseVal, object p) : base(baseVal, p)
    {
    }


    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        restoreCd = CDTimer.GetInstance().AddCD(1f, DoEffect, Mathf.FloorToInt(last));
    }

    void DoEffect(int c, long cid)
    {
        if (target != null && !target.isDie)
            target.Hp(Mathf.FloorToInt(-baseValue), HUDType.Cure, showHud);
    }
}