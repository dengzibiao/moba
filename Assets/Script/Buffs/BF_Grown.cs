using UnityEngine;

public class BF_Grown : SkillBuff
{
    float mDeltaTime = 0.33f;
    public BF_Grown(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        if (mCurValue != 0)
        {
            cs.StartCoroutine(cs.GrownAndSmaller(mCurValue, mDeltaTime, mCurValue == baseValue ? cs.mOriginLocalScale : cs.transform.localScale));
        }
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        if (mCurValue != 0)
        {
            cs.StartCoroutine(cs.GrownAndSmaller(-mCurValue, mDeltaTime, cs.transform.localScale));
        }
    }
}
