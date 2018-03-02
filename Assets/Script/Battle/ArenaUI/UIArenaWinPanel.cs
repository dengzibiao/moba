using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;
using UnityEngine.SceneManagement;

public class UIArenaWinPanel : MonoBehaviour
{

    public BoxCollider maskBtn;
    public BoxCollider damageMaskBtn;
    public UISprite winSprite;
    public GameObject Settlement;
    public ArenaItem mHero;
    public ArenaItem dHero;
    public UIButton ConfirmBtn;
    public UILabel HistoryRank;
    public UILabel receive;
    public UILabel highest;
    public GameObject winGo;
    public GameObject loseGo;
    public DamageItem mDamageItem;
    public DamageItem dDamageItem;
    public List<DamageItem> mitem = new List<DamageItem>();
    public List<DamageItem> ditem = new List<DamageItem>();

    List<ArenaPlayer> damageHeroList = new List<ArenaPlayer>();

    Transform damagePanel;

    bool isWin = false;

    void Start()
    {
        UIEventListener.Get(maskBtn.gameObject).onClick = OnMaskBtnClick;
        UIEventListener.Get(damageMaskBtn.gameObject).onClick = OnDamageBtnClick;

        EventDelegate.Set(ConfirmBtn.onClick, OnConfirmBtnClick);
    }

    public void RefreshUI(bool isWin, List<ArenaPlayer> damageHeroList)
    {
        gameObject.SetActive(true);
        this.damageHeroList = damageHeroList;
        this.isWin = isWin;
        winSprite.gameObject.SetActive(true);
        if (isWin)
        {
            winSprite.spriteName = "shenglibiao";
            winSprite.transform.FindComponent<UISprite>("Sprite").enabled = true;
        }
        else
        {
            winSprite.spriteName = "shibaibiao";
            winSprite.transform.FindComponent<UISprite>("Sprite").enabled = false;
        }
    }

    void OnMaskBtnClick(GameObject go)
    {
        winSprite.gameObject.SetActive(false);
        if (isWin)
        {
            damagePanel = transform.Find("InjuryStatistics");
            if (null != damagePanel)
            {
                damagePanel.gameObject.SetActive(true);
                RefreshDamage();
            }
        }
        else
        {
            OnDamageBtnClick(gameObject);
        }
    }

    void OnDamageBtnClick(GameObject go)
    {
        if (null != damagePanel)
            damagePanel.gameObject.SetActive(false);
        Settlement.SetActive(true);
        HistoryRank.gameObject.SetActive(true);
        if (isWin)
        {
            winGo.SetActive(true);
            if (Globe.maxRk > Globe.mrk)
            {
                highest.enabled = true;
            }
            HistoryRank.text = Globe.maxRk + "名";
            receive.text = Globe.diamond + "";
            mHero.transform.localPosition = new Vector3(48.6f, 162, 0);
        }
        else
        {
            loseGo.SetActive(true);
            mHero.transform.localPosition = new Vector3(48.6f, 90, 0);
        }
        mHero.RefreshUI(playerData.GetInstance().iconData.icon_name, playerData.GetInstance().selfData.level, playerData.GetInstance().selfData.playeName, Globe.mrk + "", isWin);
        if (isWin && null != Globe.arenahero)
        {
            RoleIconAttrNode icon = FSDataNodeTable<RoleIconAttrNode>.GetSingleton().FindDataByType(Globe.dPhoto);
            dHero.RefreshUI(null != icon ? icon.icon_name : "yx_00" + UnityEngine.Random.Range(1, 10), Globe.arenahero.lvl, Globe.arenahero.nm, "" + Globe.drk);
        }
        ConfirmBtn.gameObject.SetActive(true);
    }

    void OnConfirmBtnClick()
    {
        StartLandingShuJu.GetInstance().GetLoadingData(GameLibrary.UI_Major, 2);
        SceneManager.LoadScene("Loding");
        //UI_Loading.LoadScene(GameLibrary.UI_Major, 2, () =>
        //{
        //    //if (SceneBaseManager.instance.sceneType == SceneType.PVP3 && null != PropertyManager.Instance)
        //    //    PropertyManager.Instance.StarTiming();
        //});
        if (!Globe.isFB)
            Globe.isFB = false;
        Globe.mrk = 0;
        Globe.drk = 0;
        Globe.dPhoto = 0;
        Globe.maxRk = 0;
        Globe.diamond = 0;
        Globe.arenaHero.Clear();
    }

    List<ArenaPlayer> mCs = new List<ArenaPlayer>();
    List<ArenaPlayer> dCs = new List<ArenaPlayer>();
    float mDamage = 0;
    float dDamage = 0;

    void RefreshDamage()
    {
        for (int i = 0; i < damageHeroList.Count; i++)
        {
            if (damageHeroList[i].groupIndx == 1)
                mCs.Add(damageHeroList[i]);
            else
                dCs.Add(damageHeroList[i]);
        }

        mCs.Sort(new ArenaPlayerDamageSort());
        dCs.Sort(new ArenaPlayerDamageSort());

        for (int i = 0; i < mCs.Count; i++)
        {
            mDamage += mCs[i].damage;
        }
        for (int i = 0; i < dCs.Count; i++)
        {
            dDamage += dCs[i].damage;
        }

        mDamageItem.RefreshUI(null, (int)mDamage, (int)(mDamage + dDamage), dDamage != 0);
        dDamageItem.RefreshUI(null, (int)dDamage, (int)(mDamage + dDamage), mDamage != 0);

        for (int i = 0; i < mitem.Count; i++)
        {
            if (i >= mCs.Count)
                mitem[i].gameObject.SetActive(false);
            else
                mitem[i].RefreshUI(mCs[i].playerData, (int)mCs[i].damage, (int)mDamage);
        }
        for (int i = 0; i < ditem.Count; i++)
        {
            if (i >= dCs.Count)
                ditem[i].gameObject.SetActive(false);
            else
                ditem[i].RefreshUI(dCs[i].playerData, (int)dCs[i].damage, (int)dDamage);
        }
    }


}

public class ArenaPlayerDamageSort : IComparer<ArenaPlayer>
{
    public int Compare(ArenaPlayer x, ArenaPlayer y)
    {
        if (x.damage > y.damage)
        {
            return -1;
        }
        else if (x.damage < y.damage)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
