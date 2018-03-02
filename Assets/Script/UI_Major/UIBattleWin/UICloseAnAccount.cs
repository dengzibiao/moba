using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;

public class UICloseAnAccount : MonoBehaviour
{

    public UILabel playerlvl;
    public UILabel playerName;
    public UILabel addGold;
    public UILabel addEXP;
    public UISlider playerEXP;
    public List<GameObject> effect = new List<GameObject>();

    public UIGrid HeroGrid;
    public UIGrid ItemGrid;
    GameObject HeroGo;
    GameObject ItemGo;

    SceneNode sceneNode;

    GameObject shengjitx;

    void Start()
    {
        HeroGo = Resources.Load("Prefab/UIPanel/AccountHero") as GameObject;
        ItemGo = Resources.Load("Prefab/UIPanel/AccountItem") as GameObject;
        shengjitx = Resources.Load("Effect/Prefabs/UI/shengjitx") as GameObject;
        shengjitx.layer = 5;
        UnityUtil.AddComponetIfNull<RenderQueueModifier>(shengjitx).m_target = HeroGo.transform.Find("Exp/Sprite").GetComponent<UIWidget>();

        sceneNode = FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(GameLibrary.dungeonId);
        ShowPlayerInfo();
        ShowHeroInfo();
        ShowItemInfo();
        Invoke("ShowEffect", 0.3f);
    }


    void ShowPlayerInfo()
    {
        playerlvl.text = playerData.GetInstance().selfData.level + "级";
        playerName.text = playerData.GetInstance().selfData.playeName;
        addGold.text = "+" + GameLibrary.receiveGolds;
        if(addEXP!=null&& sceneNode!=null)
        addEXP.text = "+" + sceneNode.power_cost;
        PlayerLevelUpNode playerNode = FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().FindDataByType(playerData.GetInstance().selfData.level);
        playerEXP.value = (float)playerData.GetInstance().selfData.exprience / playerNode.exp;
    }

    void ShowHeroInfo()
    {
        if (null == Globe.Heros()) return;
        GameObject go = null;
        HeroData heroData = null;
        for (int i = 0; i < 4; i++)
        {
            if (null == Globe.Heros()[i] || Globe.Heros()[i].id == 0) continue;
            go = NGUITools.AddChild(HeroGrid.gameObject, HeroGo);
            if (Globe.Heros()[i] != null && Globe.Heros()[i].id != 0)
            {
                heroData = playerData.GetInstance().GetHeroDataByID(Globe.Heros()[i].id);
                if (null == heroData) continue;
                heroData.exps += sceneNode.exp;
                UpGradeHero(heroData);
                go.GetComponent<AccountItem>().RefreshUIHero(Globe.Heros()[i].id, sceneNode.exp);
            }
        }
        HeroGrid.Reposition();
    }

    void UpGradeHero(HeroData heroData)
    {
        UpGrade(ref heroData.exps, ref heroData.maxExps, ref heroData.lvl, false);
    }

    void UpGradePlayer()
    {
        UpGrade(ref playerData.GetInstance().selfData.exprience, ref playerData.GetInstance().selfData.maxExprience, ref playerData.GetInstance().selfData.level, true);
    }

    void UpGrade(ref int current, ref int max, ref int lvl ,bool isPlayer)
    {
        if (current >= max)
        {
            PlayerLevelUpNode playerNode = FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().FindDataByType(!isPlayer ? playerData.GetInstance().selfData.level : lvl);
            if (null == playerNode) return;
            if (!isPlayer)
            {
                if (lvl >= playerNode.heroLvLimit)
                {
                    current = max;
                    return;
                }
            }
            current -= max;
            lvl++;
            if (isPlayer)
            {
                max = playerNode.exp;
            }
            else
            {
                HeroUpGradeNode node = FSDataNodeTable<HeroUpGradeNode>.GetSingleton().FindDataByType(lvl);
                if (null == node) return;
                max = node.exp;
            }
            if (current >= max)
                UpGrade(ref current, ref max, ref lvl, isPlayer);
            else
                return;
        }
    }

    void ShowItemInfo()
    {
        if (null == GameLibrary.receiveGoods) return;
        GameObject go = null;
        List<long> keys = new List<long>(GameLibrary.receiveGoods.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            go = NGUITools.AddChild(ItemGrid.gameObject, ItemGo);
            go.GetComponent<AccountItem>().RefreshUIItem(keys[i], GameLibrary.receiveGoods[keys[i]]);
        }
        ItemGrid.Reposition();
    }

    void ShowEffect()
    {
        for (int i = 0; i < effect.Count; i++)
        {
            effect[i].gameObject.SetActive(true);
        }
    }


}
