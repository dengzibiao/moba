using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseHome : MonoBehaviour
{
    public int groupIndex = 0;//0红 1蓝
    private float time = 1f;
    GameObject restoreEffect;
    Dictionary<Collider, CharacterState> cureDict = new Dictionary<Collider, CharacterState>();

    void Start ()
    {
        GetComponent<Collider>().isTrigger = true;
        restoreEffect = Resources.Load<GameObject>("Effect/Prefabs/Buff/cure");
    }

    void OnTriggerStay(Collider other)
    {
        CharacterState cs = GetCsFromDict(other);
        // CharacterState cs = other.GetComponent<CharacterState>();
        if(cs == null)
            return;

        if(cs.maxHp == cs.currentHp)
        {
            Transform effectTrans = cs.transform.FindChild("cureEffect");
            if(effectTrans != null)
                Destroy(effectTrans.gameObject);
            return;
        }

        time += Time.deltaTime;
        if(time >= 1f)
        {
            if(cs.groupIndex == groupIndex)
            {
                Transform effectTrans = cs.transform.FindChild("cureEffect");
                if(effectTrans == null)
                {
                    GameObject effectGo = Resource.CreatPrefabs("cure", cs.gameObject, Vector3.zero, "Effect/Prefabs/Buff/");
                    effectGo.name = "cureEffect";
                }
            }
            //加血
            if(cs.groupIndex == groupIndex)
                cs.Hp(-Mathf.CeilToInt(0.2f * cs.maxHp), HUDType.Cure, cs.state == Modestatus.Player);
            else
                cs.Hp(Mathf.CeilToInt(0.2f * cs.maxHp), HUDType.DamageEnemy, cs.state == Modestatus.Player);
            //cs.Hp(2, HUDType.Cure, cs.state == Modestatus.Player);
            time = 0f;
        }
    }

    void OnTriggerExit ( Collider other )
    {
        Transform effectTrans = other.transform.FindChild("cureEffect");
        if(effectTrans != null)
            Destroy(effectTrans.gameObject, 0.5f);
    }

    CharacterState GetCsFromDict ( Collider other )
    {
        CharacterState cs = null;
        if(cureDict.ContainsKey(other))
        {
            cs = cureDict[other];
        }
        else
        {
            cs = other.GetComponent<CharacterState>();
            if(cs != null && BattleUtil.IsHeroTarget(cs))
                cureDict.Add(other, cs);
            else
               return null;
        }
        return cs;
    }
}
