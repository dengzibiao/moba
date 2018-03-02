using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 飞行技能攻击处理
/// </summary>
public class FlyEffect : MonoBehaviour
{

    private List<GameObject> mCurMonsters = new List<GameObject>();
    private List<GameObject> mAllMonsters = new List<GameObject>();
    public float range = 0.5f;//半径
    [HideInInspector]
    public bool isPlay = false;
    public bool isPierce = false;   //是否穿透
    public delegate void isHit(List<GameObject> go);

    public static FlyEffect instance;
    public event isHit isHitEvent;
    [HideInInspector]
    public Transform ct;            //施法者
    Collider col;
    private int layer
    {
        get
        {
            return GameLibrary.GetAllLayer();
        }
    }
    private int groupIndex
    {
        get
        {
            return trackBase.groupIndex;
        }
    }
    public FlyPropUnit trackBase;

    private SkillNode mCurSkillNode
    {
        get
        {
            if (trackBase != null)
            {
                return trackBase.mCurSkillNode;
            }
            return null;
        }
    }


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        col = GetComponent<Collider>();
    } 

    void Update()
    {
        if (isPlay)
        {
            if (col == null)
            {
                GetDamageRange();
                HitAction();
            }
        }
    }

    private void HitAction()
    {
        if (mCurMonsters.Count > 0 && enabled)
        {
            if (mCurSkillNode.range_type == rangeType.spurting)
            {
                trackBase.Bomb(transform);
            }
            if (isHitEvent != null)
            {
                isHitEvent(mCurMonsters);
            }
            if (!isPierce)
            {
                enabled = false;
                DestoryChild(transform);
            }
        }
    }

    private void DestoryChild(Transform parent)
    {
        if (parent.childCount > 0)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        HitTarget(col);
    }

    private void HitTarget(Collider col)
    {
        if (((1 << col.gameObject.layer) & GameLibrary.GetAllLayer()) != 0)
        {
            CharacterState mCurTargetCs = col.GetComponent<CharacterState>();
            if (mCurTargetCs != null && CheckHitCondition(mCurTargetCs))
            {
                mCurMonsters.Add(col.gameObject);
                HitAction();
            }
        }
    }

    void GetDamageRange()
    {
        mCurMonsters.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, layer);
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterState mCurTargetCs = colliders[i].GetComponent<CharacterState>();
            if (mCurTargetCs != null && CheckHitCondition(mCurTargetCs))
            {
                if (!mAllMonsters.Contains(colliders[i].gameObject))
                {
                    mCurMonsters.Add(colliders[i].gameObject);
                    mAllMonsters.Add(colliders[i].gameObject);
                }
            }
        }
    }

    public virtual bool CheckHitCondition(CharacterState cs)
    {
        return GameLibrary.Instance().CheckHitCondition(mCurSkillNode, trackBase.attackerCs, cs);
    }
}
