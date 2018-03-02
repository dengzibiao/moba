using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectToEnemy : EffectTrackBase
{


    private List<GameObject> monsters = new List<GameObject>();
    public EffectType effectType     = EffectType.Single;
    private string id;
    public float castDistance = 1.2f;   //施法距离
    public float range = 0.2f;          //作用半径

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        Play();
    }

    //3秒后特效销毁
    void Start()
    {
        Destroy(this.gameObject, destoryTime);
    }

    //播放特效            
    public void Play()
    {

        this.gameObject.SetActive(true);

        if (null != hit)
        {
            this.transform.position = new Vector3(hit.position.x,attackerTrans.position.y, hit.position.z);
            this.transform.rotation = hit.rotation;
        }
        else
        {
            this.transform.position = attackerTrans.position + transform.forward * castDistance;
        }

        GetDamageRange();

        if (effectType == EffectType.Single)
        {
            CallBack();
        }
        else
        {
            RunTimeUtil.Instance.InAction(CallBack, 1000, 3, RunState.IMMEDIATELY);
        }
    }


    //回调
    void CallBack()
    {
        if (result != null)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                
                //if (GameLibrary.player == "Shengqi")
                //{
                //    ct.GetComponentInChildren<EffectEmission>().Emission(effect_id, monsters[i], 3);
                //    return;
                //}
                //monsters[i].transform.Find("Hit").GetComponent<HitManager>().Hit(id, result, attackerTrans);
            }
        }
    }


    //获取攻击到的敌人
    List<GameObject> GetDamageRange()
    {
        int layMask = GameLibrary.GetAllLayer();

        Collider[] colliders = Physics.OverlapSphere(this.transform.position, range, layMask);

        CharacterState mcs = attackerTrans.GetComponent<CharacterState>();
        if (colliders != null)
        {
            monsters.Clear();

            foreach (Collider item in colliders)
            {
                CharacterState cs = item.GetComponent<CharacterState>();
                if(cs != null && mcs != null && cs.groupIndex != mcs.groupIndex)
                    monsters.Add(item.gameObject);//攻击范围内的敌人
            }
        }

        return monsters;
    }
}
