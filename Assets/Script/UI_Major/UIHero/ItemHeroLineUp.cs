using UnityEngine;
using System.Collections.Generic;

public class ItemHeroLineUp : MonoBehaviour
{

    public delegate void OnClickItemHeroLineUp(HeroData hd);
    public OnClickItemHeroLineUp OnClickItemLineUp;

    public UISprite mainBorder;
    public UIButton icon;
    public UISprite _lock;
    public UILabel lvlLabel;
    public UIGrid grid;
    public List<UISprite> stars = new List<UISprite>();
    public bool isShow { get; set; }
    
    public HeroData heroData;

    void Start()
    {
        EventDelegate.Set(icon.onClick, OnIconBtn);
    }

    public void RefreshUI(HeroData hn = null, bool enableClick = true)
    {
        heroData = hn;
        SetCollider(null == heroData || !enableClick ? false : true);
        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].gameObject.SetActive(false);
        }
        if (null == hn || hn.id == 0)
        {
            isShow = false;
            icon.gameObject.SetActive(false);
            mainBorder.spriteName = "hui";
            return;
        }
        isShow = true;
        icon.gameObject.SetActive(true);
        if (hn.node != null)
            icon.normalSprite = hn.node.icon_name;
        lvlLabel.text = hn.lvl + "级";
        for (int i = 0; i < hn.star; i++)
        {
            stars[i].gameObject.SetActive(true);
        }

        mainBorder.spriteName = playerData.GetInstance().GetHeroGrade(hn.grade);

        if (grid != null)
            grid.Reposition();
    }

    public void SetLock(bool isShow = false)
    {
        if (null != _lock)
        {
            _lock.enabled = isShow;
        }
    }

    void SetCollider(bool isClick = false)
    {
        icon.GetComponent<BoxCollider>().enabled = isClick;
    }

    void OnIconBtn()
    {
        if (null != OnClickItemLineUp)
            OnClickItemLineUp(heroData);
        if (null == heroData)
            RefreshUI(null);
    }

}
