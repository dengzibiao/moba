using UnityEngine;

public class BF_Blur : SkillBuff
{
    private SkinnedMeshRenderer[] skinned;
    private CDTimer.CD skinedAlphaCd;
    public BF_Blur(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Init()
    {
        base.Init();
        skinned = target.GetComponentsInChildren<SkinnedMeshRenderer>();
        SetSkinedAlpha(0.3f);
        skinedAlphaCd = CDTimer.GetInstance().AddCD(last, (a, b) => SetSkinedAlpha(1f));        
    }

    private void SetSkinedAlpha(float alpha)
    {
        if (skinned != null)
        {
            for (int i = 0; i < skinned.Length; i++)
            {
                if (skinned[i] != null)
                {
                    skinned[i].material.SetFloat("_Alpha", alpha);
                }
            }
        }
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        Formula.AddAttrWith(ref cs.CharData.buffAttrs, AttrType.dodge, mCurValue);
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        Formula.AddAttrWith(ref cs.CharData.buffAttrs, AttrType.dodge, -1f * mCurValue);
    }
}