using UnityEngine;
using System.Collections;
using System.Security.Principal;
/// <summary>
/// 播放序列帧特效
/// </summary>
public class PlayEffect : MonoBehaviour
{
    public static PlayEffect instance;
   /// <summary>
   /// 1:数组中的索引，2：界面的名字,方便接受到的界面单独处理需求
   /// </summary>
   /// <param _name="index"></param>
   /// <param _name="name"></param>
    public delegate void Active(int index,params string [] name);

    public event Active ActiveEvent;
    private UISprite effectName;
    private string _name;
    public int mFPS = 30;
    public int maxCount;
    private float mDelta = 0f;
    private int index = 0;
    public bool isLoop = true;
    public static bool mActive = true;

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public bool IsActive
    {
        get { return mActive; }
        set { mActive = value; }
    }

    void Awake()
    {
        effectName = GetComponent<UISprite>();
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (mActive && !string.IsNullOrEmpty(_name) && Application.isPlaying&& maxCount > 0 && mFPS > 0)
        {
            mDelta += Mathf.Min(1f, RealTime.deltaTime);
            float rate = 1f / mFPS;
            while (rate < mDelta)
            {
                mDelta = (rate > 0f) ? mDelta - rate : 0f;
                if (index > maxCount)
                {
                    index = 0;
                    mActive = isLoop;
                    if (mActive == false)
                    {
                        if (ActiveEvent != null)
                        {
                            ActiveEvent(0, GameLibrary.UILottryEffect);
                        }
                    }
                }
                if (index <= 9)
                {
                    effectName.spriteName = _name + "0" + index;
                }               
                if (index <= maxCount&& index>9)
                {
                    effectName.spriteName = _name + index;
                }
                index++;
            }
        }
    }
}
