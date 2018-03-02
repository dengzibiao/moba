using UnityEngine;

public class SummonHeroAI : MonoBehaviour
{
    public CharacterState target;
    bool isDestroy = false;
    PlayerMotion pm;
    CharacterState cs;

    void Awake ()
    {
        pm = GetComponent<PlayerMotion>();
        cs = GetComponent<CharacterState>();
        PlayerEffect();
        if (cs.mCurMobalId == MobaObjectID.HeroJiansheng)
        {
            Invoke("DestoryMe", 2.7f);
        }
    }

    void PlayerEffect()
    {
        GameObject effecGo = Instantiate(Resources.Load(GameLibrary.Effect_Hero + "Summon/Summon"), transform.position, transform.rotation) as GameObject;
        effecGo.transform.parent = transform;
        pm.Summon();
    }

    void FixedUpdate()
    {
        if (pm != null)
        {
            if(isDestroy)
            {
                transform.Translate(Vector3.up * 20 * Time.deltaTime);
            }
            else
            {
                if(cs.mCurMobalId == MobaObjectID.HeroJiansheng)
                {
                    //cs.attackTarget = CharacterManager.instance.GetAttackTarget(3f);
                    if (cs.attackTarget == null || cs.attackTarget.tag == Tag.tower)
                    {
                        DestoryMe();
                    }
                }
                else if(cs.mCurMobalId == MobaObjectID.HeroHuonv && target != null)
                {
                    if(cs.emission.et != null && cs.emission.et is EffectAOECenter)
                    //{
                        ( (EffectAOECenter)cs.emission.et ).mForceForward = target.transform.position - transform.position;
                    //}
                    //transform.LookAt(target.transform);
                }
            }
            AnimatorStateInfo mStateInfo = pm.ani.GetCurrentAnimatorStateInfo(0);
            if ((mStateInfo.IsName("Skill4") && mStateInfo.normalizedTime >= 0.95f) || mStateInfo.IsName("Prepare")) {
                if (cs.emission.et == null && cs.mCurMobalId != MobaObjectID.HeroJiansheng)
                {
                    DestoryMe();
                }
            }
        }
    }


    void DestoryMe()
    {
        if (!isDestroy)
        {
            isDestroy = true;
            pm.Stop();
            pm.nav.enabled = false;
            Destroy(gameObject, 0.5f);
        }
    }
}