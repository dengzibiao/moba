using UnityEngine;

public class UIFollowTarget : MonoBehaviour
{

    public Transform target;

    public Camera gameCamera;

    public Camera uiCamera;

    public bool disableIfInvisible = true;

    Transform mTrans;

    bool mIsVisible = false;

    private HUDText mHudText;
    private float mxExtend, myExtend;
    public bool b = false;
    private Vector3 mLastPos, mCurrentPos;

    void Awake()
    {
        mTrans = transform;
        mHudText = GetComponent<HUDText>();
        mxExtend = 0.1172f;
        myExtend = 0.1806f;
    }

    void Start()
    {
        if (target != null)
        {
            GetCamera();
            //SetVisible(false);
            if (uiCamera != null)
            {
                mTrans.position = uiCamera.ViewportToWorldPoint(new Vector3(-mxExtend, -myExtend, 0));
            }
        }
        else
        {
            // Debug.LogError("Expected to have 'target' set to a valid transform", this);
            enabled = false;
        }
    }

    private void GetCamera()
    {
        if (gameCamera == null) gameCamera = NGUITools.FindCameraForLayer(target.gameObject.layer);
        if (uiCamera == null) uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
    }

    void SetVisible(bool val)
    {
        mIsVisible = val;

        for (int i = 0, imax = mTrans.childCount; i < imax; ++i)
        {
            NGUITools.SetActive(mTrans.GetChild(i).gameObject, val);
        }
    }

    void SetNameInvisible(bool b)
    {
        if (mHudText != null) return;
        if ((b && !mIsVisible) || (!b && mIsVisible))
        {
            SetVisible(b);
        }
    }

    //void FixedUpdate()
    void LateUpdate()
    {
        SetNameInvisible(!GameLibrary.isBossChuChang);
        if (GameLibrary.isBossChuChang) return;

        if (target == null)
        {
            return;
        }

        if (gameCamera == null || uiCamera == null)
        {
            GetCamera();
            if (gameCamera == null || uiCamera == null)
            {
                return;
            }
        }

        Vector3 pos = gameCamera.WorldToViewportPoint(target.position);
        //修改 pos.z <= 0f 为加入项
        //bool isVisible = (gameCamera.orthographic || pos.z > 0f || pos.z <= 0f) && (!disableIfInvisible || (pos.x > 0f && pos.x < 1f && pos.y > 0f && pos.y < 1f));

        //if (mIsVisible != isVisible) SetVisible(isVisible);
        bool isVisible = !(pos.x < -mxExtend || pos.x > (1 + mxExtend) || pos.y < -myExtend || pos.y > (1 + myExtend));
        if (isVisible)
        {
            b = true;
            mCurrentPos = uiCamera.ViewportToWorldPoint(pos);
            if (mLastPos != mCurrentPos)
            {
                mTrans.position = mCurrentPos;
                mLastPos = mCurrentPos;
            }
        }
        else
        {
            if (b)
            {
                b = false;
                mCurrentPos = uiCamera.ViewportToWorldPoint(pos);
                if (mLastPos != mCurrentPos)
                {
                    mTrans.position = mCurrentPos;
                    mLastPos = mCurrentPos;
                }
            }
        }
        OnUpdate(isVisible);

    }

    protected virtual void OnUpdate(bool isVisible) { }
}
