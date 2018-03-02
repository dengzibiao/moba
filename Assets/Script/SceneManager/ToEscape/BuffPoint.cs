using UnityEngine;
using System.Collections;

public enum ResultType
{
    SpeedUp,
    SpeedDown,
    Dizziness,
    AddBlood,
    SmallPurse,
    BigPurse,
    ActivitySmallPurse,
    ActivityBigPurse,
    ActivityExp
}

public class BuffPoint : MonoBehaviour
{
    public ResultType resultType;
    public int SpeedUp = 20;
    public int SpeedDown = 20;
    public float hpRate = 10f;

    GameObject effectEat;
    CharacterState player { get { return CharacterManager.playerCS; } }

    void Start ()
    {
        if (resultType == ResultType.SmallPurse || resultType == ResultType.BigPurse || resultType == ResultType.ActivitySmallPurse || resultType == ResultType.ActivityBigPurse)
        {
            effectEat = Resources.Load("Effect/Prefabs/Item/jinbi_eat") as GameObject;
        }
        else if (resultType == ResultType.ActivityExp)
        {
            effectEat = Resources.Load("Effect/Prefabs/Item/yaoping_eat") as GameObject;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (null == player) return;
            SetBuff();
            gameObject.SetActive(false);
        }
    }

    void SetBuff()
    {
        switch (resultType)
        {
            case ResultType.SpeedUp:
                ResultSpeedUp();
                break;
            case ResultType.SpeedDown:
                ResultSpeedDown();
                break;
            case ResultType.Dizziness:
                ResultDizziness();
                break;
            case ResultType.AddBlood:
                ResultAddBlood();
                break;
            case ResultType.SmallPurse:
                ResultSmallPurse();
                break;
            case ResultType.BigPurse:
                ResultBigPurse();
                break;
            case ResultType.ActivitySmallPurse:
            case ResultType.ActivityBigPurse:
                ActivityGoldPruse();
                break;
            case ResultType.ActivityExp:
                ActivityEXPPruse();
                break;
        }
    }

    void ResultSpeedUp()
    {
        object obj = new object[2] { SkillBuffType.Fast, 4};
        SkillBuffManager.GetInst().AddBuffs(SpeedUp, obj, player, null);
    }

    void ResultSpeedDown()
    {
        object obj = new object[2] { SkillBuffType.Slow, 4 };
        SkillBuffManager.GetInst().AddBuffs(SpeedDown, obj, player, null);
    }   

    void ResultDizziness()
    {
        object obj = new object[2] { SkillBuffType.Dizzy, 3 };
        SkillBuffManager.GetInst().AddBuffs(20, obj, player, null);
    }

    void ResultAddBlood()
    {
        player.Hp(-(int)(player.maxHp * (hpRate * 0.01f)));
        Debug.Log("CurrentHP:" + player.currentHp);
    }

    void ResultSmallPurse()
    {
        InstanceEffect();
        SceneToEscape.instance.CollectItems(1);
        //SceneToEscape.instance.OnGoldBuffPoint();
    }

    void ResultBigPurse()
    {
        InstanceEffect();
        SceneToEscape.instance.CollectItems(1);
        //SceneToEscape.instance.OnGoldBuffPoint();
    }

    void ActivityGoldPruse()
    {
        InstanceEffect();
        SceneEscort.instance.CollectItems(1);
        
    }

    void ActivityEXPPruse()
    {
        InstanceEffect();
        SceneEscort.instance.CollectItems(1);
    }

    void InstanceEffect(float desTime = 1)
    {
        if (null == effectEat) return;
        GameObject go = Instantiate(effectEat) as GameObject;
        go.transform.position = transform.position;
        Destroy(go, desTime);
    }

}