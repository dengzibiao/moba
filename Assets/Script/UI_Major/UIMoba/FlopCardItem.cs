using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlopCardItem : MonoBehaviour 
{
    public UISprite Icon;
    public UISprite Frame;
    public UISprite SpLight;
    public UISprite SpTypeIcon;
    public UILabel Name;
    public UILabel Num;
    public UISprite PriceIcon;
    public UILabel Price;
    public bool flopped = false;
    public delegate void FlopEvent ( FlopCardItem flopCardItem );
    public FlopEvent OnFlop;
    public FlopEvent OnFlopped;
    public Animator Anim1;
    public Animator Anim2;
    public GameObject PressEffect;
    public UIEventListener UnFlopCard;

    void Start ()
    {
        UnFlopCard.onClick += UnFlopCardClicked;
    }

    public void DoFlop (FlopItem flopItem )
    {
        Time.timeScale = 1;
        PriceIcon.gameObject.SetActive(false);
        Price.gameObject.SetActive(false);
        AudioController.Instance.PlayUISound("UI_FanPai");
        StartCoroutine(FlopAnim(flopItem));
    }

    public void RefreshPrice ( int cost )
    {
        Price.text = cost == 0 ? Localization.Get("FlopFree") : "" + cost;
    }

    void UnFlopCardClicked ( GameObject go )
    {
        if(OnFlop != null && !flopped)
            OnFlop(this);
    }

    IEnumerator FlopAnim ( FlopItem flopItem )
    {
        yield return new WaitForSeconds(0.1f);
        PressEffect.SetActive(true);
        BattleUtil.PlayParticleSystems(PressEffect);
        yield return new WaitForSeconds(1f);
        Anim1.enabled = true;
        SpLight.gameObject.SetActive(true);
        if (GameLibrary.Instance().ItemStateList.ContainsKey(flopItem.itemId))
        {
            ItemNodeState item = GameLibrary.Instance().ItemStateList[flopItem.itemId];

            Num.text = flopItem.num.ToString();
            Name.text = item.name;
            Icon.spriteName = item.icon_name;
            if (item.types == 6)
            {
                Icon.atlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
                SpTypeIcon.alpha = 1;
            }
            else
            {
                Icon.atlas = ResourceManager.Instance().GetUIAtlas("Prop");
                SpTypeIcon.alpha = 0;
            }
            Frame.spriteName = ItemData.GetFrameByGradeType((GradeType)item.grade);
        }
    }

    public void FlopOver ()
    {
        flopped = true;
        Anim2.gameObject.SetActive(true);
        if(OnFlopped != null)
            OnFlopped(this);
    }
}

public struct FlopItem
{
    public int index;
    public int num;
    public int cost;
    public int itemId;
}