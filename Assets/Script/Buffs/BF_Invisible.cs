using System;
using UnityEngine;

public class BF_Invisible : SkillBuff
{
    private SkinnedMeshRenderer[] skinned;
    public bool CanReveal;

    public BF_Invisible ( float baseVal, object p) : base(baseVal, p)
    {

    }

    public override void Init()
    {
        base.Init();
        skinned = target.GetComponentsInChildren<SkinnedMeshRenderer>();
        CanReveal = false;
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        SetInvisible(cs, true);
        CDTimer.GetInstance().AddCD(1.0f, (a, b) => CanReveal = true);
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        SetInvisible(cs, false);
    }

    private void SetInvisible(CharacterState cs, bool b)
    {
        if (cs.groupIndex == 1)
        {
            float mAlpha = b ? 0f : 1f;
            for (int i = 0; i < skinned.Length; i++)
            {
                if (skinned[i] != null)
                {
                    skinned[i].material.SetFloat("_Alpha", mAlpha);
                }
            }
        }
        else
        {
            GameLibrary.Instance().SetCsInvisible(cs, b, skinned);
        }
        cs.Invisible = b;
    }
}
