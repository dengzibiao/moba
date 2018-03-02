using UnityEngine;

public class MobaAddLife : MonoBehaviour
{
    public float refreshTime = 0f;
    public GameObject showEffect;

    Collider effectCol;
    public bool isShowing;

    void Start()
    {
        if (!GameLibrary.SceneType(SceneType.TD))
            Invoke("Init", refreshTime);
    }

    void Init()
    {
        Show(true);
    }

    void Show(bool isShow)
    {
        isShowing = isShow;
        showEffect.SetActive(isShow);
        GetComponent<Collider>().enabled = isShow;
    }

    void OnTriggerStay(Collider other)
    {
        CharacterState cs = other.GetComponent<CharacterState>();
        if (cs == null || !isShowing)
            return;
        if (cs.currentHp < cs.maxHp && BattleUtil.IsHeroTarget(cs))
        {
            cs.Hp(-Mathf.CeilToInt(0.2f * cs.maxHp), HUDType.Cure, cs.state == Modestatus.Player);
            Show(false);
            Invoke("Init", refreshTime);
        }
    }
}
