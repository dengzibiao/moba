
public class BF_KnockBack : SkillBuff
{
    private BaseAction baseAction;
    public BF_KnockBack(float baseVal, object p) : base(baseVal, p)
    {
    }
    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        baseAction = cs.gameObject.AddMissingComponent<HitRepelAction>();
        baseAction.Init(cs, last, target.transform.position - attacker.transform.position, baseValue);
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        baseAction.SetTargetEnable(true);
    }
}