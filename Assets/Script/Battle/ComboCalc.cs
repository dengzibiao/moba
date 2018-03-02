using UnityEngine;
using System.Collections;

public class ComboCalc : MonoBehaviour
{

    public static ComboCalc _instance;

    private UILabel txt;
    private int count = 0;
    private float timer = 2f;
    private TweenAlpha ta;
    private TweenScale ts;

    void Awake()
    {
        _instance = this;
        if(transform.FindChild("txt"))
        txt = transform.FindComponent<UILabel>("txt");
        ta = this.GetComponent<TweenAlpha>();
        ts = transform.FindComponent<TweenScale>("txt");
    }

    void Update()
    {
        if(count != 0)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                count = 0;
                txt.text = "";
                timer = 2f;
            }
        }
    }

    public void ChangeCombo(int digit = 1)
    {
        timer = 3f;
        count += digit;
        ta.ResetToBeginning();
        ts.ResetToBeginning();

        if(count == 0)
        {
            txt.text = "";
        }
        else
        {
            txt.text = count + "";
            ta.PlayForward();
            ts.PlayForward();
        }
    }
}
