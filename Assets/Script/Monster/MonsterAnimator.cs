using UnityEngine;
using System.Collections;

public class MonsterAnimator : MonoBehaviour
{
    public static string CURRENT_ANIMATION = "ANIMATION_STATE";
    public Animator ani;
    CharacterState _cs;
    public CharacterState cs {
        get { if(_cs == null) _cs = GetComponent<CharacterState>(); return _cs; } }
    UnityEngine.AI.NavMeshAgent _nav;
    public UnityEngine.AI.NavMeshAgent nav
    {
        get { if(_nav == null) _nav = GetComponent<UnityEngine.AI.NavMeshAgent>(); return _nav; }
    }

    public int moveAreaMask = 1;
    public int skillAreaMask = -1;
    public bool isRiding;
    public bool isWaitingRideMsg;
    public Animator mountAni;
    public GameObject mount;
    public long mountedId;
    public Transform mountPoint;
    public GameObject mountEffect;

    protected int attack_Hash;
    protected int run_Hash;
    protected int prepare_Hash;
    protected int init_Hash;
    protected int fly_Hash, enter_Hash;

    public bool canMove = true;
    public bool canAttack = true;
    public bool canSkill = true;

    public int canMoveSwitch = 0;
    public int canAttackSwitch = 0;
    public int canSkillSwitch = 0;
    public int canControlSwitch = 0;
        
    public int mCurSkillTime = 0, mCurAttackTime = 0;
    public int mAmountAttackTime = 3;
    public bool isMove = true, isTriggerPassive = false;
    public float mMoveIntervalTime = 0.5f;
    public float mMoveTime = 0f;
    protected float mRunDlgRatio = 0.5f, mAttackDlgRatio = 0.3f, mSkillDlgRatio = 0.1f;
    protected float mRandomDlgInterval = 20f, mRandomDlgTime = 0f;
    public bool mIsHangIn = false;

    public virtual void Awake()
    {
        ani = GetComponent<Animator>();
        attack_Hash = Animator.StringToHash("Base.Attack");
        run_Hash = Animator.StringToHash("Base.Run");
        prepare_Hash = Animator.StringToHash("Base.Prepare");
        init_Hash= Animator.StringToHash("Base.Init");
        fly_Hash = Animator.StringToHash("Base.Fly");
        enter_Hash = Animator.StringToHash("Base.Enter");

        canMove = true;
        mMoveTime = mMoveIntervalTime;
        mRandomDlgTime = mRandomDlgInterval;
    }

    protected int GetCurrentHash()
    {
        return ani.GetCurrentAnimatorStateInfo(0).fullPathHash;
    }

    public virtual void Run()
    {
        if (GetCurrentHash() == prepare_Hash)
        {
            ani.SetInteger("Prepare", 0);
        }
        if(isRiding && mountAni != null)
        {
            mountAni.SetBool("isRun", true);
            isCountRandom = false;
        }
        ani.speed = cs.animSpeed;
        ani.SetInteger("Attack", 0);
        ani.SetInteger("Speed",1);
    }

    public virtual void DieToPrepare()
    {
        if(ani!=null)
        {
            ani.SetInteger( "Speed" , 0 );
            ani.SetInteger( "Attack" , 0 );
            ani.SetInteger( "Prepare" , 1 );
            ani.SetTrigger( "reborn" );
        }
        else
        {
            Debug.Log( "MonsterAnimator ani is null! " );
        }
    }

    protected bool isCountRandom;
    float RandomMountIdleTimer = 5f;
    public virtual void Prepare()
    {
        if (ani != null)
        {
            ani.SetInteger("Speed", 0);
            ani.SetInteger("Attack", 0);
            ani.SetInteger("Prepare", GameLibrary.Instance().CheckPlayIdel() ? 0 : 1);
            if(isRiding && mountAni != null)
            {
                mountAni.SetBool("isRun", false);
                isCountRandom = true;
            }
        }
        else
        {
            Debug.Log("MonsterAnimator ani is null! ");
        }
    }

    public void RandomPlayDialogMusic(float mRandom)
    {
        if (mRandomDlgTime <= 0 && CharacterManager.player != null && CharacterManager.playerCS == cs)
        {
            if (Random.value < mRandom)
            {
                int mDialogIndex = Random.Range(0, cs.mAmountDlg);
                AudioController.Instance.PlayEffectSound(cs.GetMobaName() + "/dlg" + (mDialogIndex + 1), cs);
                mRandomDlgTime = mRandomDlgInterval;
            }
        }
    }

    void PlayRandomMountIdle ()
    {
        int r = 0;
        if(mountAni.HasState(mountAni.GetLayerIndex("BaseCw"), Animator.StringToHash("BaseCw.Idle02")))
            r = Random.Range(0, 2);
        mountAni.SetTrigger(r == 0 ? "Idle": "Idle02");
    }

    public virtual void Update()
    {
        if (isMove)
        {
            mMoveTime -= Time.deltaTime;
        }

        if(isRiding && isCountRandom)
        {
            RandomMountIdleTimer -= Time.deltaTime;
            if(RandomMountIdleTimer < 0f)
            {
                RandomMountIdleTimer = Random.Range(12f, 30f);
                Invoke("PlayRandomMountIdle", 0f);
            }
        }

        //随机台词间隔时间
        mRandomDlgTime -= Time.deltaTime;
    }

    public void PlayRunMusic(bool b)
    {
        if (b)
        {
            isMove = true;
            if (mMoveTime <= 0 && CharacterManager.player != null && cs.gameObject == CharacterManager.player)
            {
                AudioController.Instance.PlayEffectSound(cs.GetMobaName() + (GameLibrary.Instance().CheckInCopy() ? "/run1" : "/run"), cs);
                mMoveTime = mMoveIntervalTime;
            }
        }
        else
        {
            isMove = false;
            mMoveTime = mMoveIntervalTime;
        }
    }

    public void MonsterAttack()
    {
        if (!CanAttackState()) return;
        if (ani != null)
        {

            if (cs != null)
            {
                ani.speed = cs.animSpeed;
            }
            ani.SetInteger("Speed", 0);
            ani.SetInteger("Prepare", 0);
            ani.SetInteger("Attack", 1);

        }
        else
        {
            Debug.Log("MonsterAnimator ani is null! ");
        }
    }

    public virtual void Die()
    {
        ani.ResetTrigger("Hit");
        ani.speed = cs.animSpeed;
        ani.SetTrigger("Die");
        if (cs.CharData is HeroData)
        {
            AudioController.Instance.PlayEffectSound(((HeroData)cs.CharData).node.sex == 1 ?  "die" : "die1", cs);
        }
        cs.emission.PlayDieEffect();
    }

    public virtual void Hit()
    {
        ani.speed = cs.animSpeed;
        ani.SetTrigger("Hit");
    }

    public virtual void Init()
    {
        if (cs != null && ani != null)
        {
            ani.speed = cs.animSpeed;
        }
        if (ani.HasState(0, init_Hash))
        {
            ani.SetTrigger("Init");
        }
    }

    public void KnockDown()
    {
        ani.speed = cs.animSpeed;
        ani.SetTrigger("Down");
        //this.transform.Translate(Vector3.back * 0.5f);
    }

    public virtual void HangIn()
    {
        ani.speed = cs.animSpeed;
        ani.SetTrigger("HangIn");
    }

    public virtual void CanNotControl()
    {
        SetControlSwitch(true);
        if (ani != null)
        {
            ani.SetInteger("Control", 1);
            ani.ResetTrigger("Hit");
        }
    }

    public virtual void CanControl()
    {
        SetControlSwitch(false);
        if (ani != null)
        {
            ani.ResetTrigger("Hit");
            ani.SetInteger("Control", 0);
        }
    }

    public void SetControlSwitch(bool b)
    {
        if (b)
            canControlSwitch++;
        else
            canControlSwitch--;
        if (cs == CharacterManager.playerCS)
        {
            SkillBuff mExistBuff = null;
            if (!b)
            {
                mExistBuff = SkillBuffManager.GetInst().GetSkillBuffListByCs(cs).Find(a => (SkillBuffType)a.id == SkillBuffType.Silence);
            }
            FightTouch._instance.ChangeAllCDTo(b ? b: !(mExistBuff == null && GameLibrary.Instance().CanControlSwitch(this)));
        }
    }

    public virtual void HitFly()
    {
        if (ani != null)
        {
            ani.ResetTrigger("Hit");
            if (ani.GetInteger("Control") == 1)
            {
                ani.SetInteger("Control", 0);
            }
            if (ani.HasState(0, fly_Hash))
            {
                ani.SetTrigger("Fly");
            }
        }
    }

    public virtual void Enter()
    {
        if (ani != null)
        {
            if (ani.HasState(0, enter_Hash))
            {
                ani.SetTrigger("Enter");
            }
        }
    }

    public virtual void ForceChangePrepare()
    {
        if (ani != null)
        {
            ani.SetInteger("Control", 1);
        }
        StartCoroutine(ChangeToPrepare());
    }

    public virtual void ReachTarget(bool b)
    {
        if (ani != null)
        {
            ani.SetBool("ReachTarget", b);
        }
    }

    IEnumerator ChangeToPrepare()
    {
        yield return new WaitForEndOfFrame();
        if (ani != null)
        {
            ani.SetInteger("Control", 0);
        }
        yield break;
    }

    public bool CanMoveState ()
    {
        return canMove && canMoveSwitch <= 0 && GameLibrary.Instance().CanControlSwitch(this);
    }

    public bool CanAttackState ()
    {
        return canAttack && canAttackSwitch <= 0 && GameLibrary.Instance().CanControlSwitch(this);
    }

    public bool CanSkillState ()
    {
        return canSkill && canSkillSwitch <= 0 && GameLibrary.Instance().CanControlSwitch(this);
    }

    public bool NormalState ()
    {
        return CanMoveState() && CanAttackState() && CanSkillState();
    }
}
