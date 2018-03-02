using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectBlock : MonoBehaviour
{

    public delegate void OnClose(int totalNum);
    public OnClose OnCloseWall;

    public delegate void OnCanMove(bool canMove, EffectBlock eb);
    public OnCanMove OnCanMoveCarts;

    public GameObject particle;

    [HideInInspector]
    public bool isTriggerBoss = false;

    //List<SpawnMonster> spawnList = new List<SpawnMonster>();
    SpawnMonster[] sm;
    float downpos;
    int count = 0;
    int totalNum = 0;

    public void LoadSpawnMonster()
    {
        foreach (Transform item in transform)
        {
            if (item.GetComponent<SpawnMonster>())
                count += item.GetComponent<SpawnMonster>().monsterCount;
        }

        totalNum = count;
        sm = GetComponentsInChildren<SpawnMonster>();
        if (null == particle)
            particle = transform.Find("Particle System").gameObject;
    }

    public void AddMonsterCount(int addCount)
    {
        count += addCount;
    }

    public void ChangeCount()
    {
        if (count == 0) return;
        count -= 1;
        if (count == 0)
        {
            if (null != particle)
                InvokeRepeating("ParticleMoveDownward", 0f, 0.01f);
            if (null != OnCloseWall)
                OnCloseWall(totalNum);
            //if (null != OnCanMoveCarts)
            //    OnCanMoveCarts(true, this);
            if (isTriggerBoss)
                TriggerBoss();
        }
    }
    
    void ParticleMoveDownward()
    {
        downpos += Time.deltaTime;
        particle.transform.Translate(Vector3.down * Time.deltaTime);
        if (downpos > 0.7f)
        {
            CancelInvoke("ParticleMoveDownward");
            CloseWall();
        }
    }

    void CloseWall()
    {
        particle.gameObject.SetActive(false);
        if (GetComponent<BoxCollider>())
        {
            GetComponent<BoxCollider>().isTrigger = true;
            GetComponent<BoxCollider>().enabled = false;
        }
        if (GetComponent<UnityEngine.AI.NavMeshObstacle>())
        {
            GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = false;
        }

        enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Cart")
        {
            if (sm != null)
            {
                for (int i = 0; i < sm.Length; i++)
                {
                    if (sm[i].name != "Boss")
                        sm[i].StartSpawn();
                }
            }

            if (SceneBaseManager.instance.enemy.size > 0)
                OnCartMove();
        }

    }

    void OnCartMove()
    {
        if (null != OnCanMoveCarts)
            OnCanMoveCarts(false, this);
    }

    void TriggerBoss()
    {
        BossChuChang bossCC = GameObject.FindObjectOfType<BossChuChang>();
        if (null == bossCC)
        {
            GameObject bossChuChang = GameObject.FindGameObjectWithTag(Tag.bossWarning);
            if (null != bossChuChang && bossChuChang.GetComponent<BossChuChang>())
                bossCC = bossChuChang.GetComponent<BossChuChang>();
        }
        if (null == bossCC) return;
        bossCC.TriggerBoss();
    }

}