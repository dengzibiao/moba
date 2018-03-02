using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIHeroDetList : MonoBehaviour
{
    public UIButton SwitchBtn;
    public BoxCollider leftBtn;
    public BoxCollider rightBtn;
    public UIGrid Grid;

    Dictionary<long, ItemHeroIcon> heroIcon = new Dictionary<long, ItemHeroIcon>();

    List<HeroData> selectHero = new List<HeroData>();

    UI_HeroDetail herodetail;
    GameObject ItemHeroDetail;
    ItemHeroIcon icon;

    bool isBreath = false;

    int switchHero = 0;
    int heroIndex = 0;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        selectHero = GetHeroAttribute(0);
        ItemHeroDetail = Resources.Load("Prefab/UIPanel/ItemHeroDetail") as GameObject;
        herodetail = GetComponentInParent<UI_HeroDetail>();
        EventDelegate.Set(SwitchBtn.onClick, OnSwitchBtnClick);
        UIEventListener.Get(leftBtn.gameObject).onClick += OnLeftBtnClick;
        UIEventListener.Get(rightBtn.gameObject).onClick += OnRightBtnClick;
        RefreshHeroIcon();
    }

    public void RefreshHeroIcon()
    {
        if (Grid.transform.childCount > 0 && Grid.transform.childCount < playerData.GetInstance().herodataList.Count)
        {
            for (int i = Grid.transform.childCount - 1; i >= 0; i--)
            {
                if (Grid.GetChild(i) != null)
                    DestroyImmediate(Grid.GetChild(i).gameObject);
            }
        }

        GameObject item = null;

        for (int i = 0; i < playerData.GetInstance().herodataList.Count; i++)
        {
            item = NGUITools.AddChild(Grid.gameObject, ItemHeroDetail);
            if (item.GetComponent<ItemHeroIcon>())
            {
                icon = item.GetComponent<ItemHeroIcon>();
                if (heroIcon.ContainsKey(playerData.GetInstance().herodataList[i].id))
                    heroIcon.Remove(playerData.GetInstance().herodataList[i].id);
                heroIcon.Add(playerData.GetInstance().herodataList[i].id, icon);

                icon.RefreshInfo(playerData.GetInstance().herodataList[i]);
            }
        }
    }

    void RefreshGrid(int type)
    {

        ItemHeroIcon icon = null;

        for (int i = 0; i < playerData.GetInstance().herodataList.Count; i++)
        {
            if (type == 0)
            {
                if (heroIcon.TryGetValue(playerData.GetInstance().herodataList[i].id, out icon))
                {
                    if (!icon.gameObject.activeSelf)
                        icon.gameObject.SetActive(true);
                }
            }
            else
            {
                if (!heroIcon.TryGetValue(playerData.GetInstance().herodataList[i].id, out icon)) continue;

                if (playerData.GetInstance().herodataList[i].node.attribute != type)
                {
                    if (icon.gameObject.activeSelf)
                        icon.gameObject.SetActive(false);
                }
                else
                {
                    if (!icon.gameObject.activeSelf)
                        icon.gameObject.SetActive(true);
                }

            }
        }
        Grid.repositionNow = true;
        Grid.Reposition();
        Grid.transform.parent.GetComponent<UIScrollView>().ResetPosition();
    }

    void OnSwitchBtnClick()
    {
        switchHero++;

        SwitchHeroType();

        if (switchHero > 3)
        {
            switchHero = 0;
        }

        switch (switchHero)
        {
            case 0:
                SwitchBtn.GetComponentInChildren<UILabel>().text = "全部";
                RefreshGrid(0);
                break;
            case 1:
                SwitchBtn.GetComponentInChildren<UILabel>().text = "力量";
                RefreshGrid(1);
                break;
            case 2:
                SwitchBtn.GetComponentInChildren<UILabel>().text = "智力";
                RefreshGrid(2);
                break;
            case 3:
                SwitchBtn.GetComponentInChildren<UILabel>().text = "敏捷";
                RefreshGrid(3);
                break;

        }

        for (int i = 0; i < selectHero.Count; i++)
        {
            if (UI_HeroDetail.hd.id != selectHero[i].id)
            {
                Globe.selectHero = (HeroNode)selectHero[0].node;
                herodetail.InsHero(selectHero[0], false);
            }
        }

        Grid.transform.parent.GetComponent<UIScrollView>().ResetPosition();

    }

    void SwitchHeroType()
    {
        selectHero = GetHeroAttribute(switchHero);
        if (selectHero.Count <= 0)
        {
            switchHero++;
        }
    }

    List<HeroData> GetHeroAttribute(int type)
    {
        List<HeroData> hero = new List<HeroData>();
        if (type == 0)
        {
            hero = playerData.GetInstance().herodataList;
        }
        else
        {
            for (int i = 0; i < playerData.GetInstance().herodataList.Count; i++)
            {
                if (playerData.GetInstance().herodataList[i].node.attribute == type)
                {
                    hero.Add(playerData.GetInstance().herodataList[i]);
                }
            }
        }
        return hero;
    }

    /// <summary>
    /// 切换英雄左按钮
    /// </summary>
    private void OnLeftBtnClick(GameObject go)
    {
        if (playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id) == null) return;
        if (selectHero.Count <= 1)
        {
            return;
        }

        for (int i = 0; i < selectHero.Count; i++)
        {
            if (selectHero[i].id == UI_HeroDetail.hd.id)
            {
                heroIndex = i;
                break;
            }
        }

        heroIndex--;

        if (heroIndex < 0)
        {
            heroIndex = selectHero.Count - 1;
        }

        herodetail.SendDrugUpgrade();

        Globe.selectHero = selectHero[heroIndex].node;
        UI_HeroDetail.hd = selectHero[heroIndex];
        herodetail.InsHero(selectHero[heroIndex], false);

        if (HeroAndEquipNodeData.TanNUm != 5)
        {
            HeroAndEquipNodeData.TabType = 0;
            UI_HeroDetail.equipItemState = 3;
            HeroAndEquipNodeData.DetailsTabState = true;
            //Control.ShowGUI(GameLibrary.UI_HeroDetail);
            //Control.ShowGUI(UIPanleID.UIHeroDetail, EnumOpenUIType.OpenNewCloseOld, false, true);
            UI_HeroDetail.instance.SwitchHero();
        }
    }

    /// <summary>
    /// 切换英雄右按钮
    /// </summary>
    private void OnRightBtnClick(GameObject go)
    {
        if (playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id) == null) return;
        if (selectHero.Count <= 1)
        {
            return;
        }

        for (int i = 0; i < selectHero.Count; i++)
        {
            if (selectHero[i].id == UI_HeroDetail.hd.id)
            {
                heroIndex = i;
                break;
            }
        }

        heroIndex++;

        if (heroIndex > selectHero.Count - 1)
        {
            heroIndex = 0;
        }

        herodetail.SendDrugUpgrade();

        Globe.selectHero = selectHero[heroIndex].node;
        UI_HeroDetail.hd = selectHero[heroIndex];
        herodetail.InsHero(selectHero[heroIndex], false);

        if (HeroAndEquipNodeData.TanNUm != 5)
        {
            HeroAndEquipNodeData.TabType = 0;
            UI_HeroDetail.equipItemState = 3;
            HeroAndEquipNodeData.DetailsTabState = true;
            //Control.ShowGUI(GameLibrary.UI_HeroDetail);
            //Control.ShowGUI(UIPanleID.UIHeroDetail, EnumOpenUIType.OpenNewCloseOld, false, true);
            UI_HeroDetail.instance.SwitchHero();
        }
    }

}
