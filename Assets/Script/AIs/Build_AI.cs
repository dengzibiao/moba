using UnityEngine;
using System.Collections;

public class Build_AI : MonoBehaviour
{

    private GameObject effect_Idle;
    private GameObject effect_Die;
    private GameObject effect_Gold;
    private GameObject effect_HpBall;
    private GameObject effect_AngerBall;
    private GameObject go;

    public int type = 0; // 0:Gold 1:生命球 2:怒气球

    void Awake()
    {
        effect_Idle = transform.Find("Idle").gameObject;
        effect_Die = transform.Find("Die").gameObject;
        effect_Gold = Resources.Load(GameLibrary.Effect_Build + "Gold") as GameObject;
        effect_HpBall = Resources.Load(GameLibrary.Effect_Build + "HpBall") as GameObject;
        effect_AngerBall = Resources.Load(GameLibrary.Effect_Build + "AngerBall") as GameObject;
        go = transform.Find("Go").gameObject;
    }

    /// <summary>
    /// 1:宝箱 2:木桶
    /// </summary>
    /// <param name="index"></param>
    public void DieAction(int index = 1)
    {
        if(index == 1)
        {
            effect_Idle.SetActive(false);
        }
        else
        {
            go.SetActive(false);
        }

        effect_Die.SetActive(true);
        Fall();
    }

    /// <summary>
    /// 掉落
    /// </summary>
    private void Fall()
    {

        if(type == 0)
        {
            Instantiate(effect_Gold, transform.position, Quaternion.identity);
        }
        else if(type == 1)
        {
            Instantiate(effect_HpBall, transform.position, Quaternion.identity);
        }
        else if(type == 2)
        {
            Instantiate(effect_AngerBall, transform.position, Quaternion.identity);
        }
    }

    private Vector3 RandomVector3()
    {
        return new Vector3(Random.Range(-0.3f, 0.3f), 0.2f, Random.Range(-0.3f, 0.3f));
    }
}
