using System;
using UnityEngine;
using System.Collections.Generic;

public class FlyPropUnit : EffectTrackBase
{
    private FlyEffect[] flyEffects;

    void Play(List<GameObject> go)
    {
        mCurMonsters.Clear();
        if (mCurSkillNode.range_type == rangeType.canBlock)
        {
            go.ForEach(a =>
            {
                if (!mAllMonsters.Contains(a))
                {
                    mCurMonsters.Add(a);
                    mAllMonsters.Add(a);
                }
            });
        }
        else
        {
            mCurMonsters.AddRange(go);
        }
        Hit(mCurMonsters);
    }

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        flyEffects = transform.GetComponentsInChildren<FlyEffect>(true);
        for (int i = 0; i < flyEffects.Length; i++)
        {
            flyEffects[i].isHitEvent += Play;
            flyEffects[i].isPlay = true;
            flyEffects[i].ct = thisTrans;
            flyEffects[i].range = distance;
            flyEffects[i].trackBase = this;
            flyEffects[i].isPierce = skillNode.isPierce;
        }
        Destroy(this.gameObject, destoryTime);
    }

    void Update()
    {
        if (mCurSkillNode.effect_time != 0 && attackerCs != null && (attackerCs.isDie || !GameLibrary.Instance().CanControlSwitch(attackerCs.pm)))
        {
            Destroy(gameObject);
        }
    }
}
