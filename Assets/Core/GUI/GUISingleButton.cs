using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUISingleButton : GUIComponentBase
{
    public enum State
    {
        Normal,
        Disabled,
    }
    #region Init
    public delegate void VoidSingleButton();

    public delegate void VoidSingleButtonClick(int index);
    public delegate void VoidSingleButtonPress(bool state);
    public delegate void VoidSingleItemPress(int index, bool state);
    public VoidSingleItemPress onItemPress;
    public VoidSingleButtonClick onItemClick;
    public VoidSingleItemPress OnItemClicks;
    /// <summary>
    /// 按钮点击事件
    /// </summary>
    public VoidSingleButton onClick;
    public void SetOnclick(VoidSingleButton func)
    {
        onClick = func;
    }
    /// <summary>
    /// 按钮长按触发事件,点击也可以执行
    /// </summary>
    public VoidSingleButtonPress onButtonPress;
    private UILabel _label;
    private UISprite _sprite;
    private UIWidget _widget;

    public int sceneID;

    public bool isDisplay = false;

    #endregion

    protected override void Init()
    {
        if (_sprite != null) return;
        _sprite = GetComponent<UISprite>();
        _widget = GetComponent<UIWidget>();
        if (GetComponentInChildren<UILabel>() != null)
            _label = GetComponentInChildren<UILabel>();
        isDragMove = false;
        // this.onComponentHover = OnComponentHover;
        //his.onComponentPress = OnComponentPress;//这个会跟scroll view的滚动事件同时触发 换成OnClick就可以了 拖是拖 点击是点击    ----完美
        this.onComponentClick = OnButtonClick;                           //this.onComponentClick = OnComponentClick;
        this.onComponentPress += OnButtonPress;
        this.AddBoxCollider();

    }
    /// <summary>
    /// 点击处理
    /// </summary>
    private void OnButtonClick()
    {

        if (state == State.Disabled) return;
        if (!isEnabled) return;
        // if (isDragMove) return;

        if (Input.touchCount > 1)
        {
            return;
        }
        if (!Singleton<GUIManager>.Instance.CheckButtonOnClick())
        {  // Debug.LogError("<color=#FFc937>连续点击了</color>");
            return;
        }
        //if (!Singleton<GUIManager>.Instance.CheckButtonOnClieck()) return;
        if (playerData.GetInstance().guideData.uId != 0)
            GuideManager.Single().SetObject(this.gameObject);
        if (onItemClick != null)
            this.onItemClick(index);
        if (null != OnItemClicks)
            OnItemClicks(sceneID, isDisplay);
        if (onClick != null)
        {
     
            onClick();
            if (target != null)
            {
                Send();
            }
        }
    }

    public void Setatlas(UIAtlas Atlas)
    {
        _sprite.atlas = Atlas;
    }

    private Vector3 _pos;
    private void OnButtonPress(bool isPress)
    {
        if (isPress)
        {
            _pos = this.transform.localPosition;

            if (state == State.Disabled) return;
            //  if (isDragMove) return;
            if (Input.touchCount > 1)
            {
                return;
            }
            a = Random.Range(0, 1000);

            this.transform.localPosition = new Vector3(_pos.x + 1.5f, _pos.y - 1.5f);
            //  this.transform.localPosition = new Vector3(_pos.x + 1.5f, _pos.y - 1.5f);
            StartCoroutine(ContinuePress(a));
        }
        else
        {
            a = -1;
            this.transform.localPosition = _pos;
            //  this.transform.localPosition = _pos;
            if (onButtonPress != null) onButtonPress(false);
            if (onItemPress != null) onItemPress(index, false);
        }
    }
    IEnumerator ContinuePress(int key)
    {
        yield return new WaitForSeconds(mDelayTime);
        int b = key;
        if (b == a)
        {
            if (onButtonPress != null) onButtonPress(true);
            if (onItemPress != null) onItemPress(index, true);
        }

    }
    void OnDragStart()
    {
        isDragMove = true;
    }

    void OnDragEnd()
    {
        Invoke("IsDragEnd", 0.5f);
    }

    void IsDragEnd()
    {
        isDragMove = false;
    }
    void Send()
    {
        if (string.IsNullOrEmpty(functionName)) return;
        if (Target == null) return;

        if (includeChildren)
        {
            Transform[] transforms = Target.GetComponentsInChildren<Transform>();

            for (int i = 0, imax = transforms.Length; i < imax; ++i)
            {
                Transform t = transforms[i];
                t.gameObject.SendMessage(functionName, target, SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            Target.SendMessage(functionName, Target, SendMessageOptions.DontRequireReceiver);
        }
    }
    /// <summary>
    /// 按钮是否不可用
    /// </summary>
    public bool isEnabled
    {
        set
        {
            if (this.enabled != value)
            {
                this.enabled = value;
            }
        }

        get { return this.enabled; }
    }
    /// <summary>
    /// 更改BUtton的状态，禁用的时候会变灰
    /// </summary>
    /// <param name="state"></param>
    public void SetState(State state)
    {
        if (_sprite != null && state != mState)
        {
            mState = state;
            switch (state)
            {
                case State.Normal:
                    ChangeColor(true); break;
                case State.Disabled:
                    ChangeColor(false); break;
            }
        }
    }
    public void ChangeColor(bool isNormal)
    {
        ChangeColorGray.Instance.ChangeSpriteColor(_sprite, isNormal);
    }
    public UIAtlas AtlasName
    {
        set
        {
            if (_sprite != null)
                _sprite.atlas = value;
        }
        get
        {
            if (_sprite != null)
                return _sprite.atlas;
            else
                return null;
        }
    }
    /// <summary>
    /// 更新按钮图片
    /// </summary>
    public string spriteName
    {
        set { if (_sprite != null) this._sprite.spriteName = value; }
        get { if (_sprite != null) return this._sprite.spriteName; else return null; }
    }
    /// <summary>
    /// 长按按钮的延迟时间
    /// </summary>
    public float delayTime { set { mDelayTime = value; } get { return mDelayTime; } }
    /// <summary>
    /// 按钮文本
    /// </summary>
    public string text
    {
        set { if (_label != null) this._label.text = value; }
        get { if (_label != null) return this._label.text; else return null; }
    }
    public State state { get { return mState; } set { if (value != mState) SetState(value); } }
    /// <summary>
    /// 增加声音
    /// </summary>
    /// <param name="clip"></param>
    public void Sound(AudioClip clip)
    {
        UIPlaySound sound = NGUITools.AddMissingComponent<UIPlaySound>(this.gameObject);
        sound.audioClip = clip;
    }

    public void InitUI()
    {
        Init();
    }
    public int index { set; get; }
    private bool isDragMove { set; get; }

    public GameObject Target
    {
        get { return target; }
        set { if (value != null) target = value; }
    }
    private int a = -1;//作为按钮长按的key
    private long mTime = 0;//作为按钮点击间隔时间
    private float mDelayTime = 0f;//长按的延迟时间
    private GameObject target = null;
    public string functionName;//执行的方法
    public bool includeChildren = false;
    private State mState = State.Normal;
}

