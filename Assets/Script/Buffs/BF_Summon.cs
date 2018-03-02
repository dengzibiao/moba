using System;
using System.Collections.Generic;

public class BF_Summon : SkillBuff
{
    public List<CharacterState> mCurSummonCs = new List<CharacterState>();
    private long mSummonId;
    public BF_Summon(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        for (int i = 0; i < skillNode.specialBuffs.Length; i++)
        {
            if (skillNode.specialBuffs[i] is Array)
            {
                Array o = (Array)skillNode.specialBuffs[i];
                if (id == long.Parse(o.GetValue(0).ToString()))
                {
                    mSummonId = long.Parse(o.GetValue(2).ToString());
                    break;
                }
            }
        }
        CreateSummonMonster(mSummonId);
    }

    private void CreateSummonMonster(long monsterId)
    {
        EffectBlock eb = target.GetComponentInParent<EffectBlock>();
        for (int i = 0; i < skillNode.target_ceiling; i++)
        {
            MonsterData monsterData = new MonsterData(monsterId);
            monsterData.groupIndex = target.groupIndex;
            monsterData.state = Modestatus.Monster;
            monsterData.lvl = BattleUtil.IsHeroTarget(target) ? target.CharData.skill [skillId] : target.CharData.lvl;
            CharacterState mCs = SceneBaseManager.instance.CreateBattleMonster(monsterData, target.transform.parent.gameObject);
            mCs.transform.position = target.transform.position;
            mCs.gameObject.AddMissingComponent<Monster_AI>();
            mCs.transform.position += ((i / 2) % 2 == 0 ? target.transform.forward : target.transform.right) * 0.1f * (i % 2 == 0 ? 1 : -1);
            if (eb != null)
            {
                Resource.CreatPrefabs(((MonsterAttrNode)monsterData.attrNode).effect_sign, target.transform.parent.gameObject, mCs.transform.localPosition, GameLibrary.Effect_Spawn);
                eb.AddMonsterCount(1);
                mCs.OnDead += (mcs) => eb.ChangeCount();
            }
            mCurSummonCs.Add(mCs);
        }
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        if (last != 0)
        {
            for (int i = 0; i < mCurSummonCs.Count; i++)
            {
                if (mCurSummonCs[i] != null)
                {
                    mCurSummonCs[i].HandleDieLogic();
                }
            }
        }
    }
}
