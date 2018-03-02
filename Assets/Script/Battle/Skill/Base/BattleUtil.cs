using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

public class BattleUtil
{
    public static uint BattleTimeSinceStart;
    public static uint BattleTimeStep = 1;
    public static bool useUnityUpdate = true;
    static System.Timers.Timer BattleTimer;

    public static void StartBattleTimer()
    {
        if (BattleTimer == null)
        {
            BattleTimer = new System.Timers.Timer(100);
            BattleTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => BattleTimeSinceStart++;
            BattleTimer.AutoReset = true;
            BattleTimer.Enabled = true;
        }
    }

    public static void AddBattleTimerHandler(System.Timers.ElapsedEventHandler handler)
    {
        BattleTimer.Elapsed += handler;
    }

    public static void RunCircle(object obj, string FuncName, object[] paras, uint interval = 1)
    {
        BattleTimeSinceStart += interval;
        Type t = obj.GetType();
        MethodInfo mi = t.GetMethod(FuncName);
        if (mi != null)
            mi.Invoke(obj, paras);
    }

    public static void DoWait(uint waitTime, object obj, string FuncName, object[] paras)
    {
        if (useUnityUpdate)
        {
            CDTimer.GetInstance().AddCD(waitTime, (int count, long id) => RunCircle(obj, FuncName, paras));
        }
        else
        {
            BattleTimeSinceStart += waitTime;
            RunCircle(obj, FuncName, paras);
        }
    }

    public static void CancelWait(object obj, string FuncName, object[] paras)
    {

    }

    public static Vector3 CheckBound(Transform[] bounds, Transform self, Vector3 intendMovement)
    {

        self.position = self.position + intendMovement;
        return intendMovement;
    }

    public static bool ReachPos(Vector3 selfPos, Vector3 targertPos, float reachDis)
    {
        return V3ToV2Dis(selfPos, targertPos) < reachDis;
    }
    public static bool ReachPos(CharacterState self, CharacterState target, float reachDis)
    {
        return V3ToV2Dis(self.transform.position, target.transform.position) < reachDis;
    }

    public static float V3ToV2Dis(Vector3 selfPos, Vector3 targertPos)
    {
        return Vector2.Distance(new Vector2(selfPos.x, selfPos.z), new Vector2(targertPos.x, targertPos.z));
    }

    public static bool IsTargetedIncludeInvisible(CharacterState attacker, CharacterState target, float radius)
    {
        if (CantBeAttack(attacker, target) || target.Invincible)
            return false;
        return ReachPos(attacker.transform.position, target.transform.position, radius);
    }

    public static bool IsTargeted(CharacterState attacker, CharacterState target, float radius)
    {
        if (CantBeAttack(attacker, target) || target.Invincible || target.Invisible)
            return false;
        return ReachPos(attacker, target, radius);
    }

    static bool CantBeAttack(CharacterState attacker, CharacterState target)
    {
        return target == null || attacker.groupIndex == target.groupIndex || target.isDie || attacker == target;
    }

    public static bool IsHeroTarget(CharacterData targetData)
    {
        return targetData.state == Modestatus.Player || targetData.state == Modestatus.NpcPlayer;
    }

    public static bool IsHeroTarget(CharacterState targetCs)
    {
        return null != targetCs && (targetCs.state == Modestatus.Player || targetCs.state == Modestatus.NpcPlayer);
    }

    public static CharacterState AddMoveComponents(GameObject go, ModelNode modelNode, bool isAuto = true)
    {
        if (go == null)
            return null;
        CharacterController cc = UnityUtil.AddComponetIfNull<CharacterController>(go);
        cc.skinWidth = 0.02f;
        cc.center = new Vector3(0f, 0.2f, 0f);
        cc.radius = modelNode.colliderRadius;
        float scaleRate = Mathf.Max(go.transform.lossyScale.x, go.transform.lossyScale.z) / go.transform.lossyScale.y;
        cc.height = 2f * (modelNode.colliderRadius * scaleRate + 0.2f);

        UnityEngine.AI.NavMeshAgent nav = UnityUtil.AddComponetIfNull<UnityEngine.AI.NavMeshAgent>(go);
        nav.radius = modelNode.navRadius;
        nav.height = 0.6f;
        nav.angularSpeed = 360;

        UnityUtil.AddComponetIfNull<PlayerMotion>(go).isAutoMode = isAuto;
        CharacterState cs = UnityUtil.AddComponetIfNull<CharacterState>(go);
        AddShadowTo(go);
        return cs;
    }

    public static void AddPetComponents(GameObject pet, float radius = 0.05f)
    {
        if (null == pet) return;
        UnityEngine.AI.NavMeshAgent nav = UnityUtil.AddComponetIfNull<UnityEngine.AI.NavMeshAgent>(pet);
        nav.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
        nav.avoidancePriority = 55;
        nav.height = 0.6f;
        nav.radius = radius;
        nav.angularSpeed = 360;
        UnityUtil.AddComponetIfNull<Pet_AI>(pet);
    }

    public static GameObject AddEffectTo(string url, Transform parentTrans, string id = null, SkillNode skillNode = null, CharacterState cs = null)
    {
        try
        {
            if (cs != null && cs.state == Modestatus.Boss && cs.mCurMobalId != MobaObjectID.None && skillNode.skill_type == SkillCastType.MeleeEffect && id.StartsWith("attack"))
            {
                return null;
            }
            GameObject prefab = Resources.Load(url) as GameObject;
            GameObject effect = GameObject.Instantiate(prefab, parentTrans.position, parentTrans.rotation) as GameObject;
            effect.transform.localScale = Vector3.one;
            effect.SetActive(false);
            return effect;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static GameObject AddShadowTo(GameObject parent)
    {
        GameObject shadowGo = Resource.CreatPrefabs("Shadow", parent, Vector3.zero, "Prefab/Character/");
        shadowGo.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        CharacterState cs = parent.GetComponent<CharacterState>();
        if (cs != null)
        {
            cs.OnDead += (a) =>
            {
                if (shadowGo != null) shadowGo.gameObject.SetActive(false);
            };
        }
        return shadowGo;
    }

    public static List<long> GetRandomTeam(int count, List<long> exceptIds = null)
    {
        List<int> randmSeeds = new List<int>();
        List<int> alreadyHave = new List<int>();
        if(exceptIds != null)
        {
            for(int m = 0; m< exceptIds.Count; m++)
            {
                int rSeed = (int)( exceptIds[m] - 201000000 ) / 100;
                if(rSeed == 30) rSeed = 13;
                if(rSeed == 33) rSeed = 15;
                alreadyHave.Add(rSeed);
            }
        }

        for (int i = 0; i < count; i++)
        {
            int seed = RandomSeedNotRepeat(alreadyHave);
            randmSeeds.Add(seed);
            //Debug.LogError("add " + seed + " alreadyHave " + alreadyHave.Count);
        }
        List<long> heroids = new List<long>();
        for (int j = 0; j < randmSeeds.Count; j++)
        {
            if(randmSeeds[j] == 13) randmSeeds[j] = 30;
            if(randmSeeds[j] == 15) randmSeeds[j] = 33;
            heroids.Add(201000000 + randmSeeds[j] * 100);
        }
        return heroids;
    }

    static int RandomSeedNotRepeat(List<int> alreadyHave)
    {
        int randomSeed = UnityEngine.Random.Range(1, 23);
        if (alreadyHave.Contains(randomSeed))
        {
            randomSeed = RandomSeedNotRepeat(alreadyHave);
        }
        else
        {
            alreadyHave.Add(randomSeed);
        }
        return randomSeed;
    }

    public static void SetPSspeed(GameObject go, float speed)
    {
        ParticleSystem[] pss = go.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in pss)
        {
            ps.playbackSpeed = speed;
        }
    }

    public static void PlayParticleSystems(GameObject go)
    {
        ParticleSystem[] pss = go.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in pss)
        {
            ps.Play();
            //ps.Stop();
            //ps.Simulate(0f, true);
        }
    }

    public static Vector3 GetNearestNavPos(GameObject go, float radius = 0f)
    {
        UnityEngine.AI.NavMeshHit nmh;
        if (UnityEngine.AI.NavMesh.SamplePosition(go.transform.position, out nmh, radius, UnityEngine.AI.NavMesh.AllAreas))
        {
            return nmh.position;
        }
        else
        {
            return GetNearestNavPos(go, radius + 1f);
        }
        //NavMeshAgent nma = go.GetComponent<NavMeshAgent>();
        //if(!nma.isOnNavMesh)
        //{

        //}
        //else
        //{
        //    return go.transform.position;
        //}
    }

    public static Vector3 GetRandomAvoidPos(Vector3 selfPos, Vector3 targetPos, float dis)
    {
        Vector3 pos = selfPos + Quaternion.Euler(0f, 1f * UnityEngine.Random.Range(90, 270), 0f) * (dis * (selfPos - targetPos).normalized);
        UnityEngine.AI.NavMeshHit nmh;
        if (!UnityEngine.AI.NavMesh.SamplePosition(pos, out nmh, 0.01f, UnityEngine.AI.NavMesh.AllAreas))
        {
            pos = GetRandomAvoidPos(selfPos, targetPos, dis);
        }
        return pos;
    }

    public static Vector3 GetCanNavPos(UnityEngine.AI.NavMeshAgent nma, Vector3 targetPos)
    {
        UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
        if (nma.enabled && nma.isOnNavMesh && nma.CalculatePath(targetPos, nmp))
        {
            if(nmp.corners.Length > 1)
                return nmp.corners[nmp.corners.Length - 1];
        }
        return Vector3.zero;
    }

    public static bool IsBoss(CharacterData cdata)
    {
        return cdata != null && cdata.attrNode.types == 5;
    }

    public static bool IsElite(CharacterData cdata)
    {
        return cdata != null && cdata.attrNode.types == 4;
    }

    public static void GetRadiusRandomPos(Transform tran, Transform target, float minVal, float maxVal)
    {
        tran.position = target.transform.position;
        tran.Rotate(new Vector3(0, UnityEngine.Random.Range(0, 360), 0));
        tran.position = tran.position + tran.forward * UnityEngine.Random.Range(minVal, maxVal);
        tran.position = BattleUtil.GetNearestNavPos(tran.gameObject);
    }

    public static Vector3 WorldToScreenPoint(Vector3 target)
    {
        Vector3 tempPos = Camera.main.WorldToScreenPoint(target);
        tempPos.z = 0;
        return UICamera.mainCamera.ScreenToWorldPoint(tempPos);
    }

}