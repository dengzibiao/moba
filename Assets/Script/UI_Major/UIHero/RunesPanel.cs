using UnityEngine;
using System.Collections;
using System;
using Tianyu;
using System.Text;

public class RunesPanel : MonoBehaviour
{

    public static RunesPanel instance;



    #region

    UILabel runeslvl;
    UISprite border;
    UISprite icon;
    UILabel count;
    UILabel powerL;
    UILabel addPowerL;
    UILabel hitL;
    UILabel addHitL;
    UIButton maskBtn;
    UIButton wearBtn;
    GUISingleCheckBoxGroup detailsTab;
    UILabel gold;

    #endregion

    ItemNodeState rune;
    bool isLeft;
    bool isOneClick = false;
    bool isSyn = false;

    int site;

    void Awake()
    {
        instance = this;
        runeslvl = transform.Find("Lvl").GetComponent<UILabel>();
        border = transform.Find("Border").GetComponent<UISprite>();
        icon = transform.Find("Icon").GetComponent<UISprite>();
        count = transform.Find("CountL").GetComponent<UILabel>();
        powerL = transform.Find("PowerL").GetComponent<UILabel>();
        addPowerL = transform.Find("AddPowerL").GetComponent<UILabel>();
        hitL = transform.Find("HitL").GetComponent<UILabel>();
        addHitL = transform.Find("AddHitL").GetComponent<UILabel>();
        maskBtn = transform.Find("MaskBtn").GetComponent<UIButton>();
        wearBtn = transform.Find("WearBtn").GetComponent<UIButton>();
        detailsTab = transform.Find("DetailsTab").GetComponent<GUISingleCheckBoxGroup>();
        gold = transform.Find("Gold").GetComponent<UILabel>();
    }

    void OnEnable()
    {
        if (!isOneClick) return;

        OnDetailsTabClick(0, true);

        detailsTab.DefauleIndex = 0;
    }

    void Start()
    {
        detailsTab.onClick = OnDetailsTabClick;
        UIEventListener.Get(maskBtn.gameObject).onClick += OnMaskBtnClick;
        EventDelegate.Set(wearBtn.onClick, OnWearBtnClick);
        count.enabled = false;
        gold.gameObject.SetActive(false);
        isOneClick = true;
    }

    public void ShowPanel(ItemNodeState runeVO, bool isLeft, int site = -1)
    {
        this.rune = runeVO;
        this.isLeft = isLeft;
        this.site = site;

        if (isLeft)
        {
            detailsTab.gameObject.SetActive(false);
            wearBtn.transform.Find("Label").GetComponent<UILabel>().text = "卸下";
        }
        else
        {
            detailsTab.gameObject.SetActive(true);
            wearBtn.transform.Find("Label").GetComponent<UILabel>().text = "佩戴";
        }

        runeslvl.text = runeVO.name;
        icon.spriteName = runeVO.icon_name;
        count.text = playerData.GetInstance().GetItemCountById(runeVO.props_id).ToString();
            //playerData.GetInstance().runesDic[runeVO.props_id].ToString();

    }

    void OnWearBtnClick()
    {
        if (isSyn)
        {
            print("合成");

            SynRune();

            return;
        }

        HeroData hd = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);

        if (isLeft)
        {
            playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id).runes[site - 1] = 0;
            //if (playerData.GetInstance().runesDic.ContainsKey(rune.props_id))
            //    playerData.GetInstance().runesDic[rune.props_id] += 1;
            //else
            //    playerData.GetInstance().runesDic.Add(rune.props_id, 1);
            GoodsDataOperation.GetInstance().AddGoods((int)rune.props_id, 1);
            ClientSendDataMgr.GetSingle().GetHeroSend().SendEquipRunes(Globe.selectHero.hero_id, site, rune.props_id, 2);
        }
        else
        {

            if (hd.grade < rune.grade)
            {
                Debug.Log("请提升英雄品阶");
                PromptPanel.instance.ShowPrompt("请提升英雄品阶");
                return;
            }

            bool isFull = false;
            for (int i = 0; i < hd.runes.Length; i++)
            {
                if (hd.runes[i] == 0)
                {
                    isFull = true;
                }
            }

            if (!isFull)
            {
                Debug.Log("符石位满");
                PromptPanel.instance.ShowPrompt("符石位满");
                return;
            }

            for (int i = 0; i < hd.runes.Length; i++)
            {
                if (hd.runes[i] == 0)
                {
                    hd.runes[i] = rune.props_id;
                    break;
                }
            }

            //playerData.GetInstance().runesDic[rune.props_id] -= 1;

            GoodsDataOperation.GetInstance().UseGoods((int)rune.props_id, 1);

            ClientSendDataMgr.GetSingle().GetHeroSend().SendEquipRunes(Globe.selectHero.hero_id, site, rune.props_id, 1);

        }

        UI_HeroDetail.instance.WearRunes();

        UI_HeroDetail.instance.Runes.RefreshRunes();

        OnMaskBtnClick(gameObject);

    }

    void SynRune()
    {

        if (rune.next_grade == 0)
        {
            PromptPanel.instance.ShowPrompt("已达最高等级");
            return;
        }

        ItemNodeState runeSyn = GameLibrary.Instance().ItemStateList[rune.next_grade];

        StringBuilder synSB = new StringBuilder();
        for (int i = 0; i < runeSyn.syn_condition.Length; i += 2)
        {
            for (int j = 0; j < 2; j++)
            {
                synSB.Append(runeSyn.syn_condition[i, j] + ",");
            }
        }

        string synStr = synSB.ToString();
        synStr = synStr.Replace("[", "");
        synStr = synStr.Replace("]", "");

        synSB.Length = 0;

        string[] synID = synStr.Split(',');

        //Debug.Log(synID[0] + "-" + synID[1]);

        int runeBagCount = playerData.GetInstance().GetItemCountById(int.Parse(synID[0]));

        if (runeBagCount > 0)
        {
            if (runeBagCount >= int.Parse(synID[1]))
            {

                if (playerData.GetInstance().baginfo.gold < rune.syn_cost)
                {
                    Debug.Log("金币不足，弹出点金手");
                    Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
                    return;
                }

                Debug.Log("合成成功");

                //if (playerData.GetInstance().runesDic.ContainsKey((int)rune.be_synth[0]))
                //{
                //    playerData.GetInstance().runesDic[(int)rune.be_synth[0]] += 1;
                //}
                //else
                //{
                //    playerData.GetInstance().runesDic.Add((int)rune.be_synth[0], 1);
                //}
                //playerData.GetInstance().runesDic[long.Parse(synID[0])] -= int.Parse(synID[1]);

                GoodsDataOperation.GetInstance().AddGoods((int)runeSyn.props_id, 1);
                GoodsDataOperation.GetInstance().UseGoods(int.Parse(synID[0]), int.Parse(synID[1]));

                UI_HeroDetail.instance.Runes.RefreshRunes();
                count.text = playerData.GetInstance().GetItemCountById(rune.props_id).ToString();

                UI_HeroDetail.instance.runesGlod = rune.syn_cost;

                ClientSendDataMgr.GetSingle().GetHeroSend().SendRunesCompounes(rune.next_grade, 1);

            }
            else
            {
                Debug.Log("符石不足，弹出获取途径");
                Control.Show(UIPanleID.UIGetWayPanel);
            }
        }
        else
        {
            Debug.Log("无符石");
        }
    }

    private void OnDetailsTabClick(int index, bool boo)
    {
        if (boo == false) return;
        switch (index)
        {
            case 0:
                wearBtn.transform.Find("Label").GetComponent<UILabel>().text = "佩戴";
                count.enabled = false;
                gold.gameObject.SetActive(false);
                isSyn = false;
                break;
            case 1:
                wearBtn.transform.Find("Label").GetComponent<UILabel>().text = "合成";
                count.enabled = true;
                gold.gameObject.SetActive(true);
                gold.text = rune.syn_cost.ToString();
                isSyn = true;
                break;
            default:
                break;
        }
    }

    private void OnMaskBtnClick(GameObject go)
    {
        gameObject.SetActive(false);
    }

}