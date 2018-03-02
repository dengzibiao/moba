using UnityEngine;
using System.Collections;

public class BF_Petrified : BF_Dizzy
{
    public Renderer[] skinned;
    public Material[] mOriginalMateria;
    private Material mPetrifiedMateria;
    public BF_Petrified(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Init()
    {
        base.Init();
        mPetrifiedMateria = Resources.Load(GameLibrary.Effect_Buff + "shihua") as Material;
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        skinned = cs.GetSkins();
        mOriginalMateria = cs.GetMaterials();
        SetSkinedPetrified(true);
        cs.pm.ani.speed = 0f;
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        SkillBuff mPetrifiedBuff = SkillBuffManager.GetInst().GetSkillBuffListByCs(target).Find(a => a != this && a.id == id);
        if (mPetrifiedBuff == null)
        {
            SetSkinedPetrified(false);
            cs.pm.ani.speed = cs.animSpeed;
        }
    }


    private void SetSkinedPetrified(bool isPetrified)
    {
        if (skinned != null && mPetrifiedMateria != null)
        {
            for (int i = 0; i < skinned.Length; i++)
            {
                if (skinned[i] != null)
                {
                    Color col = skinned[i].material.GetColor("_MainColor");
                    float alpha = skinned[i].material.GetFloat("_Alpha");
                    //float blink = skinned[i].material.GetFloat("_Blink");
                    skinned[i].material = isPetrified ? mPetrifiedMateria : mOriginalMateria[i];
                    skinned[i].material.SetColor("_MainColor", col);
                    skinned[i].material.SetFloat("_Alpha", alpha);
                    //skinned[i].material.SetFloat("_Blink", blink);
                }
            }
        }
    }
}
