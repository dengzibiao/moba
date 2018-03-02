using UnityEngine;
using System.Collections;

public class BloodScreen : MonoBehaviour
{

    private static BloodScreen _instance;
    public static BloodScreen Instance
    {
        get { return _instance; }
    }
    //private UISprite sprite;
    private TweenAlpha tweenAlpha;

    void Awake()
    {
        _instance = this;
        //sprite = this.GetComponent<UISprite>();
        tweenAlpha = this.GetComponent<TweenAlpha>();
    }

    void Update()
    {
        Show();
    }
    public void Show()
    {
        //sprite.alpha = 1;

        tweenAlpha.PlayForward();
        tweenAlpha.ResetToBeginning();
    }

}
