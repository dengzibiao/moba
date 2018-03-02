using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIHeroNameArea : MonoBehaviour
{


    public UILabel NameLabel;
    public List<UISprite> star = new List<UISprite>();
    public UILabel LvLabel;
    public UISlider ExpBar;
    public UILabel Figting;
    public UISprite HeroType;
    private Transform shengJiEffect;
    public static UIHeroNameArea Instance;
    public void Awake()
    {
        Instance = this;
        shengJiEffect = transform.Find("UI_YXShengJi_01");
    }
    public void RefreshUI(HeroData hd)
    {
        NameLabel.text = "["+GoodsDataOperation.GetInstance().GetNameColourByHeroGrade(hd.grade)+"]"+ hd.node.name+"[-]";
        //NameLabel.color = ChangeNameColor(hd.grade);

        for (int i = 0; i < star.Count; i++)
        {
            star[i].spriteName = i < hd.star ? "xing" : "xing-hui";
        }

        LvLabel.text = hd.lvl + "级";
        
        ExpBar.value = hd.exps / (float)hd.maxExps;
        ExpBar.transform.Find("Label").GetComponent<UILabel>().text = hd.exps + "/" + hd.maxExps;
        HeroType.spriteName =UISign_in.Instance().GetHeroType(hd.node.attribute);
        hd.RefreshAttr();
        Figting.text = hd.fc.ToString();// hd.fc + "";
        if (shengJiEffect!=null)
        {
            if (shengJiEffect.gameObject.activeSelf)
            {
                shengJiEffect.gameObject.SetActive(false);
            }
        }
    }

    public void ChangeExpBar(float value, int currentEXPs, int maxEXP, int level = 0)
    {
        ExpBar.value = value;
        ExpBar.transform.Find("Label").GetComponent<UILabel>().text = currentEXPs + "/" + maxEXP;
        if (level != 0)
        {
            LvLabel.text = level + "级";
        }
    }

    Color ChangeNameColor(int grade)
    {
        switch (grade)
        {
            case 1://白色
                return Color.white;
            case 2://绿色
                return Color.green;
            case 3://蓝色
                return Color.blue;
            case 4://紫色
                return new Color(255, 0, 250);
            case 5://橙色
                return new Color(255, 97, 0);
            case 6://红色
                return Color.red;
            default: return Color.white;
        }
    }
    public void PlayShengjiEffect()
    {
        shengJiEffect.gameObject.SetActive(true);
        StartCoroutine(HideEffect());
    }
    IEnumerator HideEffect()
    {
        yield return new WaitForSeconds(1f);
        shengJiEffect.gameObject.SetActive(false);
    }
}
