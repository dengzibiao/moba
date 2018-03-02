using UnityEngine;
using System.Collections;

public class Effect_LifeCycle : MonoBehaviour
{

    public delegate void OnDestroyEff();
    public OnDestroyEff OnDesEff;

    public bool isDestroy = false;
    public float cycle = 1f;
    private float timer;
    public int type = 0;//1：boss出场

    void OnEnable()
    {
        isShow = true;
        timer = cycle;
    }

    void Update()
    {
        if(isShow)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                isShow = false;
                timer = cycle;
                this.gameObject.SetActive(false);
                if (isDestroy)
                {
                    if (null != OnDesEff)
                        OnDesEff();
                    Destroy(this.gameObject);
                }

                RenderSettings.fog = true;
                if(type == 1)
                {
                    //如果是boss出场动画
                    GameLibrary.isBossChuChang = false;
                }

            }
        }
    }

    private bool isShow { set; get; }

}