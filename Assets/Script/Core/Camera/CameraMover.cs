using UnityEngine;

public class CameraMover : MonoBehaviour
{

    public float m_fTotalTime = 2.0f;    //运动总时长
    public float m_fRuntime = 0.5f;      //运动时间
    public float m_fHight = 2.0f;       //高度上的增量
    public float m_fAccelerate = 0;     //加速度

    public float m_fEndRunTime = 1f;

    public bool act = true;
    bool m_bIsRunning = false;
    Vector3 _origPos = Vector3.zero;            //起始坐标
    Quaternion _origRot = Quaternion.identity;  //起始角度
    Vector3 _targetPos = Vector3.zero;          //终止坐标
    float m_fInterval = 0f;
    float m_fStartTime = 0f;

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
        //Object.DontDestroyOnLoad(gameObject);
        //if (this.GetComponent<ShowFps>()==null)
        //{
        //    this.gameObject.AddComponent<ShowFps>();
        //}
    }

    void Start()
    {
           
    }

    public bool IsMoving
    {
        get { return m_bIsRunning; }
    }

    void Update()
    {
        if (!act)
            return;

        if (m_bIsRunning == false)
            return;

        m_fInterval = Time.time - m_fStartTime;
        if (m_fInterval <= m_fRuntime)
        {
            float percent = m_fInterval / m_fRuntime;
            DoRunAction(percent);
        }
        else if (m_fInterval <= m_fTotalTime)
        {

        }
        else if (m_fInterval <= (m_fTotalTime + m_fEndRunTime))
        {
            float endPer = (m_fInterval- m_fTotalTime) / m_fEndRunTime;
            DoEndRunAction(endPer);
        }
        else
        {
            DoEndRunAction(2.0f);
        }
    }

    public void DoMove(float hight, float runtime, float allTime,float a = 0)
    {
        //CameraMgr cameraMgr = transform.GetComponent<CameraMgr>();
        //if (cameraMgr != null)
        //{
        //    cameraMgr.ActiveCamera((int)enumCameraControlType.moveCamera);
        //}
        _origPos = transform.localPosition;
        _origRot = transform.localRotation;//transform.localRotation.eulerAngles;
        m_fTotalTime = allTime;
        m_fHight = hight;
        m_fRuntime = runtime;
        m_fAccelerate = a;

        m_fInterval = 0;
        m_fStartTime = Time.time;
        m_bIsRunning = true;
    }

    Vector3 CalculateAddPosByLine(float addHight, Vector3 vecAngel)
    {
        Vector3 pos = Vector3.zero;
        vecAngel.Normalize();
      //  Debug.LogWarning(vecAngel.x +"  " + vecAngel.y + "  "+ vecAngel.z);
        return pos;

    }

    void DoRunAction(float percent)
    {
        if (percent <= 1.0)
        {
            float dwAddHight = m_fHight * percent;
            Vector3 temp = _origPos + _origRot * (new Vector3(0,0,dwAddHight));
            transform.localPosition = temp;
        }
        else
        {
            Vector3 temp = _origPos + _origRot * (new Vector3(0, 0, m_fHight));
            transform.localPosition = temp;
        }
    }

    void DoEndRunAction(float percent)
    {
        if (percent < 1.0)
        {
            float dwAddHight = m_fHight * (1 - percent);
            Vector3 temp = _origPos + _origRot * (new Vector3(0, 0, dwAddHight));               
            transform.localPosition = temp;
        }
        else
        {
            m_bIsRunning = false;
            transform.localPosition = _origPos;
            transform.localRotation = _origRot;
            m_fInterval = 0f;

            //CameraMgr cameraMgr = transform.GetComponent<CameraMgr>();
            //if (cameraMgr != null)
            //{
            //    cameraMgr.ActiveCamera((int)enumCameraControlType.thirdCamera);
            //}

        }
    }

}


