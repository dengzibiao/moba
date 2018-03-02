using UnityEngine;
using System.Collections.Generic;

public class RewardButton : MonoBehaviour
{
    public UISprite Sprite;
    public UILabel Label;

    public int[] info;
    public int index;
    public int boxIndex;
    public bool IsScaleSprite;
    public GameObject boxEffect;

    public delegate void VoidDelegate(GameObject go);
    public VoidDelegate OnClickGo;

    public delegate void OnPressDelegate(GameObject go, bool isPress, int[] info, int index);
    public OnPressDelegate OnPressClick;

    [HideInInspector]
    public List<EventDelegate> OnClickDele = new List<EventDelegate>();

    void OnClick()
    {
        if (OnClickGo != null)
            OnClickGo(gameObject);
        EventDelegate.Execute(OnClickDele);
    }

    bool isPress = false;

    void OnPress(bool b)
    {
        if (b && IsScaleSprite)
        {
            TweenScale.Begin(Sprite.gameObject, 0.2f, 1.05f * Vector3.one);
        }
        else
        {
            TweenScale.Begin(Sprite.gameObject, 0.1f, Vector3.one);
        }
        if (null != OnPressClick)
            OnPressClick(gameObject, b, info, index);
    }

    public void SetEffectActive(bool isEnable)
    {
        if (null != boxEffect)
        {
            boxEffect.SetActive(isEnable);
        }
    }

}