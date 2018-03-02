using UnityEngine;
using System.Collections;

public class ThrowEffect : EffectTrackBase
{

    bool isThrow = false;     //是否初始化
    private Vector3 startPoint;  //起始点
    private Vector3 endPoint;//结束点
    private float startTime;//开始时间

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        CreatePoint();
        Play();
    }
    void CreatePoint()
    {
        Transform startPoint = attackerTrans.Find("startPoint");
        if (startPoint == null)
        {
            startPoint = new GameObject("startPoint").transform;
            startPoint.parent = attackerTrans;
            startPoint.localPosition = new Vector3(0, 0.4f, -0.5f);
        }
        transform.position = startPoint.transform.position;
        this.startPoint = startPoint.transform.position;
    }
    void Play()
    {
        isThrow = true;
        if (null != hit)
        {
            endPoint = hit.position;
        }
        else
        {
            Transform endPoint = attackerTrans.transform.Find("endPoint");
            if (endPoint == null)
            {
                endPoint = new GameObject("endPoint").transform;
                endPoint.transform.parent = attackerTrans;
            }
            endPoint.transform.localPosition = new Vector3(0, 0f, mCurSkillNode.max_fly / attackerTrans.transform.localScale.z);
            this.endPoint = endPoint.transform.position;
        }
        destoryTime = Vector3.Distance(this.endPoint, this.startPoint) / mCurSkillNode.flight_speed;
    }
    void Awake()
    {
        FindPosTrans(transform);
        startTime = Time.time;
    }

    void GetDamageRange()
    {
        GetCommonDamageRange(pos);
    }

    void Update()
    {
        if (!isThrow) return;
        Vector3 center = (startPoint + endPoint) * 0.5F;
        center -= new Vector3(0, 1, 0);
        Vector3 riseRelCenter = startPoint - center;
        Vector3 setRelCenter = endPoint - center;
        float fracComplete = (Time.time - startTime) / destoryTime;
        transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
        transform.position += center;
        if (transform.position == endPoint)
        {
            Bomb(hit == null ? transform : mHitTargetCs.transform);
            GetDamageRange();
            Hit(mCurMonsters);
            Destroy(gameObject);
        }
    }
}
