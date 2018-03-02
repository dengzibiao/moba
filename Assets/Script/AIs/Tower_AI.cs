using System.Collections.Generic;
using UnityEngine;

public class Tower_AI : MonoBehaviour
{
    private EffectEmission emission;
    private float timer = 0f;
    public CharacterState cs;
    public GameObject AttackEffect;
    public GameObject DeathEffect;
    public GameObject NormalState;
    public GameObject AttackCircle;
    Material circleMaterial;

    [HideInInspector]
    public float baseAttack;
    [HideInInspector]
    public float plusAttack;
    private UnityEngine.AI.NavMeshObstacle navOb;
    private CharacterController cc;
    private bool changeRed;
    ParticleSystem[] pss;

    public CharacterState enemyHeroAttackingMyHero;
    bool inited;

    public void InitTowerAI ()
    {
        emission = GetComponentInChildren<EffectEmission>();
        circleMaterial = AttackCircle.GetComponentInChildren<MeshRenderer>().material;
        cs = GetComponent<CharacterState>();
        cc = GetComponent<CharacterController>();
        navOb = GetComponent<UnityEngine.AI.NavMeshObstacle>();

        cs.OnDead += ShowDeathEffect;
        AttackCircle.transform.localScale = 2f * Vector3.one * GetAtkRange() / AttackCircle.transform.lossyScale.x;
        AttackCircle.gameObject.SetActive(false);
        inited = true;
        changeRed = false;
        pss = AttackEffect.GetComponentsInChildren<ParticleSystem>();
    }

    void ShowDeathEffect (CharacterState mcs)
    {
        DeathEffect.gameObject.SetActive(false);
        DeathEffect.gameObject.SetActive(true);
        NormalState.gameObject.SetActive(false);
        if (cc != null) cc.enabled = false;
        if (navOb != null) navOb.enabled = false;

        // cs.redCircle.gameObject.SetActive(false);
        MobaAddLife mal = GetComponentInChildren<MobaAddLife>();
        if(mal != null) mal.gameObject.SetActive(false);
        AttackCircle.gameObject.SetActive(false);
        mcs.OnDead -= ShowDeathEffect;
    }

    void FixedUpdate()
    {
        if(cs != null && cs.attackTarget != null)
        {
            if(cs.attackTarget.isDie || !BattleUtil.ReachPos(cs, cs.attackTarget, GetAtkRange()))
                cs.SetAttackTargetTo(null);
        }
        timer -= Time.deltaTime;
        if (Time.frameCount % GameLibrary.mTowerDelay != 0) return;
        if (!inited || cs.groupIndex == 2) return;

        AttackCircle.gameObject.SetActive(cs.groupIndex == 0 && cs != null && !cs.isDie && CharacterManager.playerCS != null && !CharacterManager.playerCS.isDie && BattleUtil.ReachPos(transform.position, CharacterManager.playerCS.transform.position, 1.2f * GetAtkRange()));
        if(cs != null && !cs.isDie)
        {
            if(enemyHeroAttackingMyHero != null)
            {
                if(enemyHeroAttackingMyHero.isDie || !BattleUtil.ReachPos(cs, enemyHeroAttackingMyHero, GetAtkRange()))
                {
                    enemyHeroAttackingMyHero = null;
                }
            }

            if(enemyHeroAttackingMyHero != null)
            {
                cs.SetAttackTargetTo(enemyHeroAttackingMyHero);
            }

            if(cs.attackTarget == null)
            {
                cs.SetAttackTargetTo(GetAttackTarget(GetAtkRange()));
            }
            if(cs.attackTarget != cs.LastAttack)
            {
                plusAttack = 0;
				Formula.SetAttrTo(ref cs.CharData.buffAttrs, AttrType.attack, 0);
            }
            ChangeCircleColor(cs.attackTarget == CharacterManager.playerCS);
            AttackAction(cs.attackTarget);
        }
    }

    float GetAtkRange ()
    {
        return cs.AttackRange + cs.cc.radius * cs.transform.lossyScale.z ;
    }

    CharacterState GetAttackTarget ( float radius = 2f )
    {
        CharacterState ifHero = null;
        for(int i = 0; i < SceneBaseManager.instance.agents.size; i++)
        {
            CharacterState chs = SceneBaseManager.instance.agents[i];
            if(BattleUtil.IsTargetedIncludeInvisible(cs, chs, radius))
            {
                if(BattleUtil.IsHeroTarget(chs))
                {
                    ifHero = chs;
                    continue;
                }
                else
                {
                    return chs;
                }
            }
        }
        return ifHero;
    }

    void ChangeCircleColor(bool b)
    {
        if (b && !changeRed)
        {
            circleMaterial.SetColor("_TintColor", new Color(1, 0, 0, 80f / 255));
            changeRed = true;
        }
        else if (!b && changeRed)
        {
            circleMaterial.SetColor("_TintColor", new Color(39f / 255, 122f / 255, 1f, 160f / 255));
            changeRed = false;
        }
    }

    void AttackAction ( CharacterState target )
    {
        // Debug.LogError(target == null ? " change to null " + Time.realtimeSinceStartup : target.name);
        if(target == null)
        {
            cs.LastAttack = null;
            return;
        }
        if(timer <= 0)
        {
            if(enabled)
            {
                if(target == cs.LastAttack && BattleUtil.IsHeroTarget(target))
                {
                    plusAttack += ( plusAttack > baseAttack * 0.7f ) ? 0 : baseAttack * 0.2f;
					Formula.SetAttrTo(ref cs.CharData.buffAttrs, AttrType.attack, plusAttack);
                }
                for(int i = 0; i < pss.Length; i++)
                {
                    pss[i].Play();
                }
                emission.PlayAttackEffect("attack" + ( cs.groupIndex == 0 ? "" : "0" ), target.gameObject);
            }
            cs.LastAttack = target;
            timer = 1f / cs.CharData.attrNode.attack_speed;
        }
    }

}
