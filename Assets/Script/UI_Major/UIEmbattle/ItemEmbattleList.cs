using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemEmbattleList : MonoBehaviour
{

    public delegate void OnClickItemEmbattle(ItemEmbattleList item, bool isPlay);
    public OnClickItemEmbattle OnClickItem;

    public UISprite border;
    public UIButton icon;
    public UILabel heroName;
    public UILabel lvl;
    public UISprite play;
    public UISprite site;
    public UIGrid grid;
    public List<GameObject> star = new List<GameObject>();
    public SpinWithMouse DragSprite;

    public HeroData heroData;

    UIEmbattle uiEmbattle;

    void Start()
    {
        if (null == icon) return;
        uiEmbattle = transform.GetComponentInParent<UIEmbattle>();
        EventDelegate.Set(icon.onClick, OnIconClick);
    }

    public void RefreshItemUI(HeroData hd)
    {
        if (null == hd) return;

        heroData = hd;

        if (heroData.isPlay)
            SetPlay(true);

        border.spriteName = playerData.GetInstance().GetHeroGrade(hd.grade);
        if (hd.node != null)
        {
            icon.normalSprite = hd.node.icon_name;
            lvl.text = hd.lvl + "级";
        }

        for (int i = 0; i < hd.star; i++)
        {
            star[i].SetActive(true);
        }
        for (int i = hd.star; i < star.Count; i++)
        {
            star[i].SetActive(false);
        }
        grid.Reposition();
    }

    void OnIconClick()
    {
        if (!uiEmbattle.IsAllowPlay(heroData) && !heroData.isPlay)
        {
            uiEmbattle.ShowTips();
            return;
        }
        if (null != OnClickItem)
            OnClickItem(this, !heroData.isPlay);
        SetPlay(!heroData.isPlay);
    }

    public void SetPlay(bool boo)
    {
        play.enabled = boo;
        heroData.isPlay = boo;
    }

    public void RefreshHeroModul(HeroData hd, int index, bool isFour = false, bool isThreeState = false)
    {

        heroData = hd;

        if (null == hd)
        {

            if (!isFour)
            {
                if (!isThreeState)
                {
                    if (index > 1)
                    {
                        index -= 2;
                    }
                    else
                    {
                        index += 3;
                    }
                }
                HeroPosEmbattle.instance.DestroyModel((PosType)index);
            }
            else if (index == 0)
                HeroPosEmbattle.instance.DestroyModel(PosType.EmbattlePos);
            grid.gameObject.SetActive(false);
            heroName.enabled = false;
            lvl.enabled = false;
            //if (null != site)
            //    site.enabled = false;
            return;
        }

        grid.gameObject.SetActive(true);
        heroName.enabled = true;
        lvl.enabled = true;
        if (hd.node != null)
        {
            heroName.text = hd.node.name;
        }
        else
        {
            Debug.Log(hd.id + " is null!!!!!!!!!!!!");
        }
        lvl.text = hd.lvl + "级";

        bool isShowStar = false;
        for (int i = 0; i < star.Count; i++)
        {
            isShowStar = i < hd.star ? true : false;
            star[i].SetActive(isShowStar);
        }
        grid.Reposition();
        if (!isThreeState)
        {
            if (index > 1)
            {
                index -= 2;
            }
            else
            {
                index += 3;
            }
        }
        //if (!isFour)
        //{
        //    site.enabled = true;
        //    site.spriteName = GetSiteIndex(index > 2 ? index - 3 : index);
        //}
        if (hd.node != null)
        {
            HeroPosEmbattle.instance.CreatModel(hd.node.icon_name + "_show", isFour ? PosType.EmbattlePos : (PosType)index, DragSprite);
        }

    }

    string GetSiteIndex(int index)
    {
        switch (index)
        {
            case 1:
                return "erhao";
            case 2:
                return "sanhao";
            default:
                return "yihao";
        }
    }

}
