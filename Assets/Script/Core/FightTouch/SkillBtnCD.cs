using System;
using UnityEngine;

public class SkillBtnCD : MonoBehaviour
{
    public GameObject LockGo;
    public SkillNode skillNode;

    UISprite _cdSprite;
    public UISprite CantUseSprite;
    public UISprite IconSprite;
    public GameObject cdOverEffect;
    Animator[] cdOverAnims;
    public int index;
    public delegate void OnPressDele (int id, bool b);
    public OnPressDele onPressed;

    void Awake ()
    {
        IconSprite = GetComponent<UISprite>();
        _cdSprite = transform.FindComponent<UISprite>("CD");
        if(cdOverEffect != null)
            cdOverAnims = cdOverEffect.GetComponentsInChildren<Animator>();
    }

    void Update ()
    {
        if(cd>0)
        {
            cd -= Time.deltaTime;
            if(cd < 0)
            {
                cd = 0;
                if(cdOverEffect != null && isShowEffect)
                {
                    cdOverEffect.SetActive(true);
                    if(cdOverAnims != null && cdOverAnims.Length > 0)
                    {
                        for(int i = 0; i < cdOverAnims.Length; i++)
                        {
                            cdOverAnims[i].Play(cdOverAnims[i].GetCurrentAnimatorStateInfo(0).fullPathHash);
                        }
                    }
                }
            }
            _cdSprite.fillAmount = cd / cdConfig;
        }
    }

    void OnPress (bool b)
    {
        TweenScale.Begin(gameObject, 0.06f, (b ? 1.2f : 1f) * Vector3.one);
        if(!_isEnabled)
            return;
        if(onPressed != null)
            onPressed(index, b);
    }

    public void SetCd (float cdCfg)
    {
        cdConfig = cdCfg;
    }

    public void StartCd ()
    {
        if(!isCD)
        {
            _cdSprite.fillAmount = 1;
            cd = cdConfig;
            if(cdOverEffect != null) cdOverEffect.SetActive(false);
        }
    }

    float cd;
    float cdConfig;
    public bool isShowEffect = true;
    public bool isCD { get { return cd > 0; } }
    public float CD { get { return cd; } }

    bool _isEnabled = true;
    public bool IsEnabled {
        get { return _isEnabled; }
        set {
            _isEnabled = value;
            ChangeColorGray.Instance.ChangeSpriteColor(IconSprite, _isEnabled);
        }
    }

}