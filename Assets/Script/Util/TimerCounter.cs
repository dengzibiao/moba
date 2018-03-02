using UnityEngine;
using System.Collections;

public class TimerCounter : MonoBehaviour {

    public delegate void timeDelegate();
    float time = 0;
   public  int result = 0;  
    static TimerCounter instance;
   
    void FixedUpdate()
    {
       
        time += Time.deltaTime;
        if (time>1)
        {
            result++;
            time = 0;
        }
	}

    public static TimerCounter GetInstance()
    {
        if (instance==null)
        {
            instance = Instantiate<GameObject>(new GameObject()).AddComponent<TimerCounter>();
            instance.name = "TimerCounter";
        }
        return instance;
    }

    public void Show(UILabel lab,string sec)
    {
        lab.text = sec.ToString();
    }

   
}
