using UnityEngine;
public class BF_BlowUp : SkillBuff
{
    private BaseAction baseAction;

    public BF_BlowUp(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        baseAction = cs.gameObject.AddMissingComponent<HithangAction>();
        baseAction.Init(cs, last, cs.transform.up);
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        baseAction.SetTargetEnable(true);
    }
}