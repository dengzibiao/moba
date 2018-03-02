using UnityEngine;
using System.Collections.Generic;
using Tianyu;

public class Boss_AI : BasePlayerAI
{
    public float striking = 10;
    public int SummonMonsterID = 202000500;
    public bool isSummonMonster = true;

    //List<float> hoRate = new List<float>() { 0.8f, 0.2f };
    //int SummonMonsterCount = 0;
    float randomRot = 0;
    Vector3 pos;
    //GameObject Thunder;

    List<CharacterState> sumMonster = new List<CharacterState>();

    void Start()
    {
        List<long> skills = thisCs.GetSkills();
        List<float> skillCDs = new List<float>();
        skills.ForEach((long sid) => skillCDs.Add(AISkillHandler.GetNode(sid).cooling));
        aiSkillHandler.SetSkills(thisCs.GetSkills(), skillCDs);
        GameLibrary.bossBlood.ShowBlood(thisCs);
        //Thunder = Resources.Load("Thunder") as GameObject;

        //if (null != Thunder)
        //    InvokeRepeating("BeginThunder", 5, 5);

        if (GameLibrary.SceneType(SceneType.TD) || GameLibrary.SceneType(SceneType.ACT_GOLD) || GameLibrary.SceneType(SceneType.ACT_EXP))
        {
            thisCs.OnBeAttack += (c) =>
            {
                if (c.state == Modestatus.Player && thisCs.attackTarget != CharacterManager.playerCS)
                {
                    targetCs = CharacterManager.playerCS;
                    thisCs.SetAttackTargetTo(CharacterManager.playerCS);
                }
            };
        }

        thisCs.OnDead += (c) =>
        {
            for (int i = sumMonster.Count - 1; i >= 0; i--)
            {
                if (null != sumMonster[i])
                {
                    if (sumMonster[i].GetComponent<Monster_AI>())
                        sumMonster[i].GetComponent<Monster_AI>().StopMonsterAI();
                }
            }
        };
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if (null != targetCs && targetCs.isDie)
            targetCs = null;

        if (null == targetCs)
            targetCs = GetAttackTarget(thisCs.TargetRange * striking);

        if (null != targetCs)
        {

            if (BattleUtil.ReachPos(thisCs.transform.position, targetCs.transform.position, thisCs.TargetRange))
            {
                thisCs.SetAttackTargetTo(targetCs);
                if (!aiSkillHandler.NormalAISkill())
                    aiSkillHandler.NormalAttack();
            }
            else
            {
                thisCs.pm.Approaching(targetCs.transform.position);
            }
        }
        else
        {
            thisCs.pm.Stop();
        }

        //if (isSummonMonster && thisCs.currentHp <= thisCs.maxHp * hoRate[SummonMonsterCount])
        //{
        //    SummonMonster();
        //    SummonMonsterCount++;
        //    if (SummonMonsterCount >= hoRate.Count) isSummonMonster = false;
        //}

    }

    public override CharacterState GetAttackTarget(float radius = 2f)
    {
        CharacterState result = null;

        float minDis = float.MaxValue;
        for (int i = 0; i < SceneBaseManager.instance.agents.size; i++)
        {
            CharacterState chs = SceneBaseManager.instance.agents[i];
            float dis = Vector3.Distance(thisCs.transform.position, chs.transform.position);
            if (BattleUtil.IsTargeted(thisCs, chs, radius))
            {
                if (dis < minDis)
                {
                    minDis = dis;
                    result = chs;
                }
            }
        }

        return result;
    }

    List<SpawnMonster> _smlist = new List<SpawnMonster>();
    int sumIndex = 0;

    void SummonMonster()
    {

        _smlist.Clear();

        for (int i = 0; i < 3; i++)
        {
            GameObject monster = new GameObject();
            monster.name = "SummonMonster";
            monster.transform.parent = transform.parent;
            SpawnMonster _sm = UnityUtil.AddComponetIfNull<SpawnMonster>(monster);
            _smlist.Add(_sm);
        }



        InvokeRepeating("CreatMonster", 0, 0.2f);

    }

    void CreatMonster()
    {
        _smlist[sumIndex].transform.position = RandomPos(_smlist[sumIndex].transform);
        _smlist[sumIndex].OnCreatMonster += (GameObject go, CharacterData cd) =>
        {
            thisCs.SetAttackTargetTo(targetCs);
            sumMonster.Add(thisCs);
            thisCs.OnDead += (CharacterState mCs) =>
            {
                if (sumMonster.Contains(mCs))
                    sumMonster.Remove(mCs);
                if (go.transform.parent.name.Contains("SummonMonster"))
                    Destroy(go.transform.parent.gameObject, 2);
            };

        };

        _smlist[sumIndex].PropCreatMonster(SummonMonsterID, 1, 1, 1, "shuaxin");

        sumIndex++;

        if (sumIndex > _smlist.Count - 1)
        {
            sumIndex = 0;
            CancelInvoke("CreatMonster");
        }
    }

    Vector3 RandomPos(Transform sm)
    {

        randomRot = Random.Range(0, 360);
        sm.position = thisCs.transform.position;
        sm.Rotate(new Vector3(0, randomRot, 0));
        pos = thisCs.transform.position + sm.forward * 1;
        sm.position = pos;

        for (int i = 0; i < _smlist.Count; i++)
        {
            if (sm == _smlist[i].transform) continue;
            if (Vector3.Distance(sm.position, _smlist[i].transform.position) < 0.5f)
            {
                RandomPos(sm);
            }
        }

        return pos;

    }

    //void BeginThunder()
    //{
    //    GameObject go = Instantiate(Thunder) as GameObject;
    //    go.transform.parent = GameObject.Find("EffectNip").transform;
    //    go.GetComponent<DungeonsThunder>().targetCs = targetCs;
    //}

}
