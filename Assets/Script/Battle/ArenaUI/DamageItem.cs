using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageItem : MonoBehaviour
{

    public UISprite border;
    public UISprite icon;
    public UISlider slider;
    public UILabel damageLabel;
    public UIGrid grid;
    public List<UISprite> star = new List<UISprite>();
    

    int damage;
    //int maxDamage;
    float maxValue;

    int currentDam = 0;
    string groupName = "";

    public void RefreshUI(HeroData hd, int damage, int maxDamage, bool isPlayer = false)
    {
        this.damage = damage;
        //this.maxDamage = maxDamage;

        damageLabel.text = groupName + currentDam;
        maxValue = (float)damage / maxDamage;
        slider.value = 0;
        if (null != hd)
        {
            icon.spriteName = hd.node.icon_name;
            border.spriteName = playerData.GetInstance().GetHeroGrade(hd.grade);
            for (int i = 0; i < star.Count; i++)
            {
                star[i].gameObject.SetActive(i < hd.star);
            }
            grid.Reposition();
        }
        if (isPlayer)
        {
            slider.value = maxValue;
            damageLabel.text = damage + "";
        }
        else
        {
            InvokeRepeating("ShowSlider", 0, 0.05f);
        }
    }

    void ShowSlider()
    {
        slider.value += maxValue * 0.05f;

        currentDam += (int)(damage * 0.05f);
        
        damageLabel.text = groupName + currentDam;

        if (currentDam >= damage)
        {
            slider.value = maxValue;
            damageLabel.text = groupName + damage;
            CancelInvoke("ShowSlider");
        }
    }

}
