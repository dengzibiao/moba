using UnityEngine;
using System.Collections;
using System;

public class ItemRunesList : GUISingleItemList
{

    UIButton mailBtn;
    UILabel count;
    ItemNodeState rune;
    UI_HeroDetail heroDet;

    float timer = 0;
    bool isState = false;

    /// <summary>
    /// item初始化函数
    /// </summary>
    protected override void InitItem()
    {
        heroDet = UI_HeroDetail.instance;
        //初始化
        count = transform.Find("Count").GetComponent<UILabel>();
        mailBtn = transform.Find("MailBtn").GetComponent<UIButton>();
        UIEventListener.Get(mailBtn.gameObject).onClick += OnBtnClick;
        UIEventListener.Get(mailBtn.gameObject).onPress += OnBtnPress;
    }


    public override void Info(object obj)
    {
        if (null == obj)
        {
            mailBtn.gameObject.SetActive(false);
            count.text = "";
        }
        else
        {
            rune = (ItemNodeState)obj;
            mailBtn.normalSprite = rune.icon_name;
            count.text = playerData.GetInstance().GetItemCountById(rune.props_id).ToString();

            //if (playerData.GetInstance().runesDic.ContainsKey(rune.props_id))
            //    count.text = playerData.GetInstance().runesDic[rune.props_id].ToString();
        }

    }

    private void OnBtnClick(GameObject go)
    {
        long[] runes = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id).runes;
        int site = 0;

        for (int i = 0; i < runes.Length; i++)
        {
            if (runes[i] == 0)
            {
                site = i;
                break;
            }
        }

        heroDet.Runes.ShowRunesPanel(rune, false, 1 + site);
    }

    private void OnBtnPress(GameObject go, bool state)
    {
        isState = state;
        if (!state)
        {
            timer = 0;
            UI_HeroDetail.instance.skillDes.SetActive(state);
        }

    }

    void FixedUpdate()
    {
        if (isState)
        {
            timer += Time.deltaTime;

            if (timer > 0.5f)
            {
                UI_HeroDetail.instance.skillDes.SetActive(isState);
                UI_HeroDetail.instance.skillDes.GetComponent<SkillDesPanel>().SetData(rune.name, 0, rune.describe);
            }
        }
    }

}