using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class TeamIcon : MonoBehaviour
{

    public UISprite boder;
    public UISprite icon;
    public UIGrid starGrid;
    public List<UISprite> star = new List<UISprite>();
    public UILabel lvl;
    public UISlider hpSlider;

    public void AssignedInfo(CharacterState cs)
    {
        HeroData hd = (HeroData)cs.CharData;
        if (null == hd) return;
        gameObject.SetActive(true);
        boder.spriteName = playerData.GetInstance().GetHeroGrade(hd.grade);
        icon.spriteName = hd.attrNode.icon_name;
        lvl.text = hd.lvl + "";
        for (int i = 0; i < star.Count; i++)
        {
            star[i].gameObject.SetActive(i < hd.star);
        }
        starGrid.Reposition();
        hpSlider.value = 1f;
    }

    public void RefreshHPBar(CharacterState mCs)
    {
        hpSlider.value = (float)mCs.currentHp / mCs.maxHp;
    }

}
