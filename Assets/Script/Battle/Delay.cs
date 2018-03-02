using UnityEngine;

public class Delay : MonoBehaviour
{
    public static Delay instance;

    TweenAlpha ta;
    TweenScale ts;
    UILabel lb;

    public int Count = 3;

    [HideInInspector]
    public bool isChange = false;

    public delegate void DelayEvent();
    public DelayEvent OnDelayDone;

    void Awake()
    {
        instance = this;
        isChange = false;
        ta = this.GetComponent<TweenAlpha>();
        ts = this.GetComponent<TweenScale>();
        lb = this.GetComponent<UILabel>();
    }

    public void Do (int count, float interval = 1f)
    {
        Count = count;
        gameObject.SetActive(true);
        InvokeRepeating("Show", 0, interval);
    }

    void Show()
    {
        if(Count == 0)
        {
            if(OnDelayDone != null)
            {
                OnDelayDone();
            }
            isChange = true;
            gameObject.SetActive(false);
            CancelInvoke("Show");
            return;
        }
        lb.alpha = 1;
        lb.text = Count.ToString();
        ta.PlayForward();
        ta.ResetToBeginning();
        ts.PlayForward();
        ts.ResetToBeginning();
        Count--;
    }
}