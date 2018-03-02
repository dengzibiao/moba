using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class ItemRunes : MonoBehaviour
{

    UISprite runeSpr;
    UIButton runeBtn;

    ItemNodeState rune;

    int site;

    void Awake()
    {
        runeSpr = GetComponent<UISprite>();
        runeBtn = GetComponent<UIButton>();
    }

    void Start()
    {
        //UI_HeroDetail.instance.runesDel += (ItemVO rItem, bool isLeft) => RunesPanel.instance.ShowPanel(rItem, isLeft);
        EventDelegate.Set(runeBtn.onClick, OnRunesClick);
    }

    void OnRunesClick()
    {
        UI_HeroDetail.instance.Runes.ShowRunesPanel(rune, true, site);
    }

    public void ChangeRunes(long id, int site)
    {
        this.site = 1 + site;
        if (id == 0)
        {
            runeSpr.enabled = false;
            runeBtn.enabled = false;
        }
        else
        {

            if (GameLibrary.Instance().ItemStateList.TryGetValue((int)id, out rune))
            {
                runeSpr.enabled = true;
                runeBtn.enabled = true;
                runeBtn.normalSprite = rune.icon_name;
            }
            //rune = FSDataNodeTable<RuneNode>.GetSingleton().FindDataByType((int)id);
                //VOManager.Instance().GetCSV<ItemCSV>("Item").GetVO(id);
            
        }
    }

}