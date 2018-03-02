using UnityEngine;
using System.Collections;

public class CountDown : MonoBehaviour
{
    private UILabel label;
    private float total = 180;
    private float autoTimer = 10f;//无操作10秒后自动开始战斗

    void Awake()
    {
        label = this.GetComponent<UILabel>();
    }

    void Update()
    {
        if(total <= 0)
        {
            return;
        }

        total -= Time.deltaTime;
        ShowCount(total);

        if(!GameLibrary.autoMode && !GameLibrary.isMoba)
        {
            autoTimer -= Time.deltaTime;

            //开始计时
            if(autoTimer <= 0)
            {
                //开始自动战斗
                autoTimer = 10f;
                FightTouch._instance.OnAutoClick();
                //CharacterManager.instance.StartAutoMode(true);
            }
        }
    }


    private void ShowCount(float t)
    {
        label.text = (int)t / 60 + ":" + string.Format("{0:D2}", (int)t % 60);
    }

}
