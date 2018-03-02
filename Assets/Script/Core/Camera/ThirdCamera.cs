using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Tianyu;

public class ThirdCamera : MonoBehaviour
{
    /// <summary>
    /// 位置旋转等相关
    /// </summary>
    private static byte ConfigurationLV = 0;//屏幕旋转等级
    public byte GetConfigurationLV()
    {
        return ConfigurationLV;
    }
    public static ThirdCamera instance;

    public float _distanceToPlayer = 2.0f;//初始化相机与玩家的距离
    public float _angle = -10f;//仰俯角度
    public float _flatAngle = 270f;//水平角度
    public Transform _MainPlayer = null;//主角
    public float _heightOfSet = 0.1f;//人物在视野内屏幕中的位置设置
    public float sensitivity1 = 5f;//灵敏度

    public float _maxDistance = 3.0f;//人与摄像机的最大距离
    public float _maxAngle = -10.0f;//摄像机的最高角度
    public float _minDistance = 3.0f;//人与摄像机的最小距离
    public float _minAngle = -10.0f;//摄像机的最低角度界限


    public bool shaking;

    public bool bzoom1 = false;
    public bool bzoom2 = false;

    public float shakeTime = 0.15f;
    public float shakeIntensity = 2;
    public float rateX = 2;
    public float rateY = 0;
    public float rateZ = 0;
    public float rateW = 0;
    public bool ifRolling = true;//滚轮是否
    public float timer = 1.0f;
    public AnimationCurve curve;

    public float _fiatAngleSet = 90.0f;
    private float decay = 0;
    private float intensity = 0;
    private Vector3 originalPos;
    private Quaternion originalRot;

    public bool act = true;
    RoleInfo tempRI = new RoleInfo();



    //private bool IsRotateBack = true;

    private float slowAction = 0;
    public float slowActionValue = -10.0f;//摄像机的初始角度

    public void StartAct()
    {
        act = true;
    }
    public void StopAct()
    {
        act = false;
    }

    void Awake()
    {
        instance = this;

        gameObject.GetComponent<AudioListener>().enabled = false;
        //Object.DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        shaking = false;
        if (CharacterManager.player != null && !GameLibrary.SceneType(SceneType.PVP3))
            _MainPlayer = CharacterManager.player.transform;

       // GetComponent<Camera>().renderingPath = RenderingPath.Forward;

        if (_MainPlayer == null)
            return;
        if (FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList != null)
            return;

        //主城以外移除同步消息
        //if (FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList!=null&&FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.ContainsKey(playerData.GetInstance().selfData.mapID))
        //{
        //    MapInfoNode tempMN = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList[playerData.GetInstance().selfData.mapID];
        //    if (tempMN != null)
        //    {
        //        if (SceneManager.GetActiveScene().name != tempMN.MapName)
        //        {
        //            ClientSendDataMgr.GetSingle().GetWalkSend().SendQuit();
        //            //WalkSendMgr.GetSingle().GetWalkSend().SendQuit();//测试
        //        }
        //    }
        //}
        //TODO:生成小地图摄像机
        // CreatMapCamera();
        if (playerData.GetInstance().selfData.oldMapID != playerData.GetInstance().selfData.mapID)
        {
            Dictionary<long, MapInfoNode> tempMapInfo = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList;
            if (tempMapInfo != null)
            {
                foreach (MapInfoNode min in tempMapInfo.Values)
                {
                    if (min.MapName == Application.loadedLevelName)
                    {
                        playerData.GetInstance().selfData.oldMapID = (int)min.key;
                        // playerData.GetInstance().selfData.mapID = min.key;

                        tempRI.accID = playerData.GetInstance().selfData.accountId;

                        if (ClientSendDataMgr.GetSingle() != null)
                            ClientSendDataMgr.GetSingle().GetWalkSend().SendInitializePosInfo(tempRI);
                        break;
                    }
                }
            }
        }
        //if (GetComponent<Camera>().renderingPath != RenderingPath.Forward)
        //{
        //    GetComponent<Camera>().renderingPath = RenderingPath.Forward;
        //}
        //main.SetCamera (this);

        //tempRI.accID = playerData.GetInstance().selfData.accountId;

        //if (ClientSendDataMgr.GetSingle() != null)
        //    ClientSendDataMgr.GetSingle().GetWalkSend().SendInitializePosInfo();
    }

    void Update ()
    {
        //输入控制
        if(slowActionValue != _angle)
        {
            _angle = Mathf.Lerp(_angle, slowActionValue, Time.deltaTime * 5);

            _distanceToPlayer = Mathf.Sqrt(( _angle - _minAngle ) / CurLineSensi()) + _minDistance;

            _distanceToPlayer = Mathf.Clamp(_distanceToPlayer, _minDistance, _maxDistance);
        }



    }

    static Vector3 _vTmp = new Vector3();

    public void CamaraRotateLeft()
    {
        if (_flatAngle > _fiatAngleSet - 360)
            _flatAngle -= sensitivity1;
    }

    public void CamaraRotateRight()
    {
        if (_flatAngle < _fiatAngleSet + 360)
            _flatAngle += sensitivity1;
    }

    void RotateBack()
    {
        if (_flatAngle > _fiatAngleSet + sensitivity1)
        {
            _flatAngle -= sensitivity1;
            return;
        }
        else if (_flatAngle + sensitivity1 < _fiatAngleSet)
        {
            _flatAngle += sensitivity1;
            return;
        }
        else
            _flatAngle = _fiatAngleSet;
        //IsRotateBack = true;
        return;
    }

    void LateUpdate()
    {
        if (_MainPlayer == null)
        {
            return;
        }
        if (!act)
        {
            return;
        }
        if (ifRolling)
        {

            slowAction = Input.GetAxis("Mouse ScrollWheel");
            if (slowAction != 0)
            {
                slowActionValue = _angle - slowAction * 2 * (_maxAngle - _minAngle);
                slowActionValue = Mathf.Clamp(slowActionValue, _minAngle, _maxAngle);
            }
        }
        if (UIInput.selection == null)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _flatAngle -= sensitivity1 * Time.deltaTime * 3;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                _flatAngle += sensitivity1 * Time.deltaTime * 3;
            }

        }
        if (Input.GetKey(KeyCode.M))
        {
            if (SceneManager.GetActiveScene().name == GameLibrary.UI_Major)
            {

                //UIPromptBox.Instance.ShowLabel("想去野外吗？来吧！");
                //Control.ShowGUI(UIPanleID.UIPromptBox);
                Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "想去野外吗？来吧！");
            }
        }
        if (Input.GetKey(KeyCode.Y))
        {
            int hp = 0;
            CharacterState cs = null;

            if (CharacterManager.player != null)
            {
                cs = CharacterManager.player.GetComponent<CharacterState>();
            }
            if (cs != null)
            {
                hp = cs.maxHp;
            }
            ClientSendDataMgr.GetSingle().GetWalkSend().SendPlayerRevive(0, 0, 1, hp);
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {

            UnityEngine.AI.NavMeshHit hit;
            bool blocked = false;
            blocked = UnityEngine.AI.NavMesh.Raycast(_MainPlayer.position, new Vector3(_MainPlayer.position.x, _MainPlayer.position.y - 1, _MainPlayer.position.z), out hit, UnityEngine.AI.NavMesh.AllAreas);
            Debug.DrawLine(transform.position, _MainPlayer.position, blocked ? Color.red : Color.green);
            if (blocked)
                Debug.DrawRay(hit.position, Vector3.up, Color.red);
            //NavMeshAgent nav =  _MainPlayer.GetComponent<NavMeshAgent>();
            // NavMeshHit navhit;
            //bool ishas =  nav.Raycast(new Vector3(nav.transform.position.x, nav.transform.position.y - 100, nav.transform.position.z), out navhit);
            //if(ishas)
            // {
            //     string str = navhit.position.x.ToString();
            // }
            //else
            // {
            //     Debug.Log("not find");
            // }


            //    public Transform target;
            //private NavMeshHit hit;
            //private bool blocked = false;
            //void Update()
            //{
            //    blocked = NavMesh.Raycast(transform.position, target.position, out hit, NavMesh.AllAreas);
            //    Debug.DrawLine(transform.position, target.position, blocked ? Color.red : Color.green);
            //    if (blocked)
            //        Debug.DrawRay(hit.position, Vector3.up, Color.red);
            //}

            //IsRotateBack = false;
        }



        if (Input.GetKeyUp(KeyCode.RightArrow))
        {

            // IsRotateBack = false;
        }

        //bool isMoving = false;

        //if (transform.GetComponent<CameraMover>() != null)
        //    isMoving = transform.GetComponent<CameraMover>().IsMoving;

        //if (!isMoving)
        //{
        float upRidus = Mathf.Deg2Rad * _angle;
        float flatRidus = Mathf.Deg2Rad * _flatAngle;

        float x = _distanceToPlayer * Mathf.Cos(upRidus) * Mathf.Cos(flatRidus);
        float z = _distanceToPlayer * Mathf.Cos(upRidus) * Mathf.Sin(flatRidus);
        float y = _distanceToPlayer * Mathf.Sin(upRidus);

        transform.position = Vector3.zero;
        _vTmp.Set(x, y + 2.6f, z);
        _vTmp = _vTmp + _MainPlayer.position;
        transform.position = _vTmp;
        _vTmp.Set(_MainPlayer.position.x, _MainPlayer.position.y + _heightOfSet, _MainPlayer.position.z);
        //}

        transform.LookAt(_vTmp);

        shakeUpdate();
    }


    public void shakeUpdate()
    {
        if (Time.deltaTime == 0) return;
        if (intensity > 0)
        {
            transform.position = originalPos + Random.insideUnitSphere * intensity * 0.005f;
            transform.rotation = new Quaternion(originalRot.x + Random.Range(-intensity, intensity) * .001f * rateX,
                                                originalRot.y + Random.Range(-intensity, intensity) * .001f * rateY,
                                                originalRot.z + Random.Range(-intensity, intensity) * .001f * rateZ,
                                                originalRot.w + Random.Range(-intensity, intensity) * .001f * rateW);

            intensity -= decay * Time.deltaTime;
        }
        else if (shaking)
        {
            shaking = false;
            transform.position = originalPos;
            transform.rotation = originalRot;
        }
    }

    public float CurLineSensi()
    {
        float dis = _maxDistance - _minDistance;
        float angChange = _maxAngle - _minAngle;
        float sensitivityLine = angChange / (dis * dis);
        return sensitivityLine;


    }

    public void DoShake1(float intesityArg, float lastTime)
    {
        if (!shaking)
        {
            originalPos = transform.position;
            originalRot = transform.rotation;
        }

        intensity = intesityArg;
        decay = intesityArg / lastTime;
        shaking = true;
    }

    IEnumerator WaitShake(float intesityArg, float lastTime, float fDelay)
    {
        yield return new WaitForSeconds(fDelay);
        DoShake1(intesityArg, lastTime);
    }
    /// <summary>
    /// 抖动
    /// </summary>
    /// <param name="type">幅度类型逐渐增大</param>
    /// <param name="fDelay">几秒后播放抖动</param>
    public void DoShake(int type, float fDelay)
    {

        if (type == 1)
        {
            StartCoroutine(WaitShake(1f, 0.15f, fDelay));
        }
        else if (type == 2)
        {
            StartCoroutine(WaitShake(2f, 0.20f, fDelay));
        }
        else if (type == 3)
        {
            StartCoroutine(WaitShake(3f, 0.22f, fDelay));
        }
        else if (type == 4)
        {
            StartCoroutine(WaitShake(4f, 0.24f, fDelay));
        }
        else if (type == 5)
        {
            StartCoroutine(WaitShake(5f, 0.26f, fDelay));

        }

    }

    ///// <summary>
    ///// 屏幕震动
    ///// </summary>
    ///// <param name="fDelay"></param>
    ///// <param name="fIntesityArg"></param>
    ///// <param name="fLastTime"></param>
    //public void DoCameraShake(float fDelay=0f, float fIntesityArg=5f, float fLastTime=0.5f)
    //{
    //    StartCoroutine(WaitShake(fIntesityArg, fLastTime, fDelay));
    //}
    //public void angleChange()
    //{

    //    if (timer > 1.0f)
    //    {

    //        CancelInvoke();
    //        timer = 1.0f;
    //        return;
    //    }
    //    if (timer == 1)
    //    {
    //        bzoom1 = false;
    //    }
    //        timer += 0.01f;

    //        float timerTrans = curve.Evaluate(timer);
    //        _angle = _minAngle + timerTrans * (_maxAngle - _minAngle);
    //        _angle = Mathf.Clamp(_angle, _minAngle, _maxAngle);

    //        _distanceToPlayer = Mathf.Sqrt((_angle - _minAngle) / CurLineSensi()) + _minDistance;
    //        _distanceToPlayer = Mathf.Clamp(_distanceToPlayer, _minDistance, _maxDistance);

    //}

    //public void revertAngleChang()
    //{

    //    if (timer < 0.0f)
    //    {
    //        CancelInvoke();
    //        timer = 0.0f;
    //        return;
    //    }

    //    if (timer == 0)
    //    {
    //        bzoom2 = false;
    //    }

    //    timer -= 0.01f;

    //    float timerTrans = curve.Evaluate(timer);
    //    _angle = _minAngle + timerTrans * (_maxAngle - _minAngle);
    //    _angle = Mathf.Clamp(_angle, _minAngle, _maxAngle);

    //    _distanceToPlayer = Mathf.Sqrt((_angle - _minAngle) / CurLineSensi()) + _minDistance;
    //    _distanceToPlayer = Mathf.Clamp(_distanceToPlayer, _minDistance, _maxDistance);

    //}

    //public void CreatMapCamera()
    //{
    //    foreach (MapInfoNode min in FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.Values)
    //    {
    //        if (min.MapName == Application.loadedLevelName)
    //        {
    //            if (FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList.ContainsKey(min.map_info))
    //            {
    //                if (FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList[min.map_info].isHave == 1)
    //                {
    //                    MiniMap.Create(FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList[min.map_info]);
    //                    MiniMap.instance.CreateTargetPos(CharacterManager.player, ShowType.player);
    //                }
    //            }
    //        }
    //    }
    //}
}



