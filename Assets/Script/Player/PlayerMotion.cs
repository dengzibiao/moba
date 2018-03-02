using UnityEngine;
using UnityEngine.SceneManagement;
using Tianyu;

public class PlayerMotion : PlayerAnimator
{
    public bool isAutoMode = false;
    public int hp;
    UnityEngine.AI.NavMeshPath path = null;

    public override void Awake()
    {
        base.Awake();
        base.Init();
        path = new UnityEngine.AI.NavMeshPath();
    }
    /// <summary>
    /// 朝向某个位置
    /// </summary>
    /// <param name="v"></param>
    public void Approaching(Vector3 targetPos, float stopDistance = 0f)
    {
        if (CanMoveState())
        {
            nav.stoppingDistance = stopDistance > 0 ? stopDistance : nav.radius;
            Move(targetPos);
        }
        else
        {
            Stop();
        }
    }
    //上下坐骑
    public void Ride(bool b, long mountId = 0, bool showEffect = true)
    {
        if(cs.pm.isRiding == b && mountedId == mountId)
            return;
        if(cs.CharData is HeroData) {
            if(!UIRole.CheckCanRide((HeroData)cs.CharData))
                return;
        }
        else
        {
            return;
        }
        isCountRandom = isRiding = b;
        ani.SetBool("isRiding", b);
        if (b)
        {
            // mountId = MountAndPetNodeData.Instance().goMountID;
            if(mountId <= 0)
                return;
            mountedId = mountId;
            UIMountNode mountNode = FSDataNodeTable<UIMountNode>.GetSingleton().DataNodeList[mountId];
            mount = Resource.CreatPrefabs(mountNode.icon_name, gameObject, new Vector3(0f, -mountNode.ride_y, 0f), GameLibrary.Mount_URL);
            mountAni = mount.GetComponent<Animator>();
            cs.mShadowTrans.localPosition = new Vector3(0f, -mountNode.ride_y, 0f);
            cs.mShadowTrans.localScale = 1.5f * Vector3.one;
            cs.mHitPoint.transform.localPosition = new Vector3(cs.mOriginHitPos.x, cs.mOriginHitPos.y - mountNode.ride_y, cs.mOriginHitPos.z);
            nav.baseOffset = mountNode.ride_y;
            cs.moveSpeed = mountNode.movement_speed + cs.moveInitSpeed;
            if(showEffect)
            {
                if(mountEffect == null)
                    mountEffect = Resource.CreatPrefabs("ShangMa_01", gameObject, new Vector3(0f, 0f, 0f), "Effect/Prefabs/Item/");
                else
                    BattleUtil.PlayParticleSystems(mountEffect);
            }
            if(cs == CharacterManager.playerCS && ThirdCamera.instance != null)
                ThirdCamera.instance._heightOfSet = 0.18f - mountNode.ride_y;
            if(cs.cc != null)
                cs.cc.center = new Vector3(cs.cc.center.x, 0.3f - mountNode.ride_y, cs.cc.center.z);
        }
        else
        {
            if(cs == CharacterManager.playerCS && ThirdCamera.instance != null)
                ThirdCamera.instance._heightOfSet = 0.18f;
            cs.mShadowTrans.localPosition = Vector3.zero;
            cs.mShadowTrans.localScale = Vector3.one;
            nav.baseOffset = 0f;
            cs.mHitPoint.transform.localPosition = cs.mOriginHitPos;
            cs.moveSpeed = cs.moveInitSpeed;
            if (mount != null)
            {
                mountedId = 0;
                Destroy(mount.gameObject);
            }
            if(cs.cc != null)
                cs.cc.center = new Vector3(cs.cc.center.x, 0.3f, cs.cc.center.z);
        }
        if (transform.GetComponent<SetMainHeroName>() != null)
        {
            transform.GetComponent<SetMainHeroName>().SetHeadBuffPos(b);
        }
        else if (transform.GetComponent<OtherPlayer>()!=null)
        {
            transform.GetComponent<OtherPlayer>().SetHeadBuffPos(b);
        }
        InvokeRepeating("CheckRidingState", Time.fixedDeltaTime, Time.fixedDeltaTime);
    }

    void CheckRidingState ()
    {
        if((isRiding && GetCurrentHash() == Animator.StringToHash("Base.Ride")) ||
            ( !isRiding && GetCurrentHash() != Animator.StringToHash("Base.Ride") ))
        {
            //Debug.LogError("check isWaitingRideMsg, before is " + isWaitingRideMsg);
            isWaitingRideMsg = false;
            //Debug.LogError("check isWaitingRideMsg, after is " + isWaitingRideMsg);
            CancelInvoke("CheckRidingState");
        }
    }

    public void RotateTo(Vector3 v)
    {
        if (CanMoveState())
        {
            transform.LookAt(v);
        }
    }

    float tempTime = 0f;
    GameObject autoMovePoint;
    public void Move(Vector3 v)
    {
        if (GameLibrary.isBossChuChang || cs.isDie || GetCurrentHash() == init_Hash || !ani.HasState(0, run_Hash) || isWaitingRideMsg) return;
        if (CanMoveState())
        {
            if (!nav.enabled) nav.enabled = true;
            nav.areaMask = moveAreaMask;
            Run();
            if (nav.isOnNavMesh)
            {
                if (isAutoMode)
                {
                    nav.Resume();
                    nav.speed = cs.moveSpeed;
                    nav.SetDestination(v);

                    //    ClientSendDataMgr.GetSingle().GetWalkSend().SendSelfPos( v );
                    //    ClientSendDataMgr.GetSingle().GetWalkSend().SendOrientation( cs.transform.rotation.eulerAngles );
                    if (cs == CharacterManager.playerCS && UIRole.instance != null)
                    {
                        if (UIRole.instance.riding)
                        {
                            UIRole.instance.CancelRide();
                        }
                    }
                    // ShowAutoMoveTargetPoint(v);
                }
                else
                {
                    nav.Move(v.normalized * Time.deltaTime * cs.moveSpeed);
                }
                tempTime += Time.deltaTime;
                if(tempTime > 0.2f)
                {
                    tempTime = 0f;
                    if(CharacterManager.playerCS != null && cs == CharacterManager.playerCS)
                        ClientSendDataMgr.GetSingle().GetWalkSend().SendSelfPos();
                }
            }
        }
        else
        {
            Stop();
        }
    }

    UnityEngine.AI.NavMeshHit hit;
    public void FastMove(Vector3 v, bool ignoreTerrain = true)
    {
        if (cs.isDie) return;
        nav.enabled = true;
        if (nav.isOnNavMesh)
        {
            if (ignoreTerrain)
            {
                nav.areaMask = skillAreaMask;
                if (cs.emission != null && cs.emission.et != null && cs.emission.et.mCurSkillNode.target == TargetState.Need)
                {
                    nav.CalculatePath(v, path);
                    if (path.status == UnityEngine.AI.NavMeshPathStatus.PathPartial || path.status == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
                    {
                        nav.enabled = false;
                        transform.position += v;
                    }
                    else
                    {
                        NormalFastMove(v);
                    }
                }
                else
                {
                    NormalFastMove(v);
                }
            }
            else
            {
                nav.areaMask = moveAreaMask;
                nav.Move(v);
            }
        }
    }

    private void NormalFastMove(Vector3 v)
    {
        Vector3 pos = transform.position + v;
        if (Physics.CheckSphere(pos, nav.radius / 2, 1 << (int)GameLayer.Obstacle) && nav.Raycast(pos, out hit))
        {
            nav.enabled = false;
            transform.position += v;
        }
        else
        {
            nav.Move(v);
        }
    }

    public void ShowAutoMoveTargetPoint(Vector3 pos)
    {
        if (cs == CharacterManager.playerCS)
        {
            if (autoMovePoint == null)
            {
                autoMovePoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                autoMovePoint.transform.localScale = 0.1f * Vector3.one;
            }
            autoMovePoint.transform.position = pos;
        }
    }

    public void Rotation(Vector3 v)
    {
        transform.localRotation = Quaternion.Euler(v);
    }

    public void Stop()
    {
        PlayRunMusic(false);
        if (nav.enabled)
        {
            if (nav.isOnNavMesh)
            {
                if (nav.hasPath)
                {
                    nav.enabled = false;
                    nav.enabled = true;
                }
                else
                {
                    nav.Stop();
                }
            }
        }
        Prepare();
    }

    public void ContinuousAttack()
    {
        if (!CanAttackState())
            return;
        GameLibrary.Instance().BrokenInvisibility(cs);
        CharacterState mCs = GetComponent<CharacterState>();
        if (mCs.CharData.attrNode is MonsterAttrNode && ((MonsterAttrNode)mCs.CharData.attrNode).types == 4)
        {
            StarEliteAttack();
        }
        else
        {
            StartAttack();
        }
    }

    public void ResetState()
    {
        //attackCount = 0;
        canMove = true;
        canAttack = true;
        //ani.SetInteger("Attack", 0);
    }

    public override void Hit()
    {
        if ((!cs.isDie && GameLibrary.Instance().CanControlSwitch(this) && CheckCanHitTag(transform) && !GameLibrary.isMoba))
        {
            base.Hit();
            Stop();
        }
    }

    private bool CheckCanHitTag(Transform transform)
    {
        return (transform.CompareTag(Tag.monster) && cs.CharData.id != 209006301) || transform.CompareTag(Tag.cart);
    }
    /// <summary>
    /// 绕着目标点一定角度巡逻运动
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target"></param>
    /// <param name="angle"></param>
    public void Patrol(CharacterState self, CharacterState target)
    {

        if (target != null && self != null)
        {
            Vector3 selfPos = self.transform.position;
            Vector3 targetPos = target.transform.position;
            Vector3 pos = selfPos - targetPos;
            Vector3 pos2 = target.transform.TransformPoint(pos.normalized * 3);
            this.Move(pos2);
        }
    }

}
