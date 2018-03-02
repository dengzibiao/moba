using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class UIRunes : MonoBehaviour
{

    public GUISingleMultList RuneList;
    public UIButton SynthesisBtn;
    public RunesPanel RunesPanel;

    Dictionary<long, int> upGradeDic = new Dictionary<long, int>();
    Dictionary<long, int> runesDic = new Dictionary<long, int>();
    List<long> runesKey = new List<long>();
    StringBuilder runeSB = new StringBuilder();

    int runesGlod;
    int indexK = 0;

    void Start()
    {
        EventDelegate.Set(SynthesisBtn.onClick, OnSynthesisBtn);
    }

    public void RefreshRunes()
    {
        List<ItemData> runesData = GoodsDataOperation.GetInstance().GetItemListByItmeType(ItemType.Rune);
        object[] rune = new object[12];
        ItemNodeState runeNode;

        for (int i = 0; i < runesData.Count; i++)
        {
            if (GameLibrary.Instance().ItemStateList.TryGetValue(runesData[i].Id, out runeNode))
            {
                rune[i] = runeNode;
            }
        }

        RuneList.InSize(12, 3);
        RuneList.Info(rune);
    }

    public void ShowRunesPanel(ItemNodeState rune, bool isLeft, int site)
    {
        RunesPanel.gameObject.SetActive(true);
        RunesPanel.ShowPanel(rune, isLeft, site);
    }

    void OnSynthesisBtn()
    {
        print("一键合成");

        runesGlod = 0;

        List<ItemData> runesData = GoodsDataOperation.GetInstance().GetItemListByItmeType(ItemType.Rune);

        for (int i = 0; i < runesData.Count; i++)
        {
            if (!runesDic.ContainsKey(runesData[i].Id))
                runesDic.Add(runesData[i].Id, runesData[i].Count);
        }

        //runesDic = playerData.GetInstance().runesDic;

        indexK = 0;

        RunesRecursion();

        //playerData.GetInstance().runesDic = runesDic;

        foreach (long id in upGradeDic.Keys)
        {
            ClientSendDataMgr.GetSingle().GetHeroSend().SendRunesCompounes(id, upGradeDic[id]);
        }

        RefreshRunes();

    }

    void RunesRecursion()
    {

        if (indexK >= 5) return;

        foreach (long item in runesDic.Keys)
        {
            if (!runesKey.Contains(item))
                runesKey.Add(item);
        }

        ItemNodeState rune;

        List<long> needUpGrade = new List<long>();

        for (int i = 0; i < runesKey.Count; i++)
        {
            if (runesKey[i] % 10 == indexK)
                needUpGrade.Add(runesKey[i]);
        }

        for (int i = 0; i < needUpGrade.Count; i++)
        {
            rune = GameLibrary.Instance().ItemStateList[(int)needUpGrade[i]];

            if (null == rune.syn_condition || rune.syn_condition.Length <= 0) break;

            for (int j = 0; j < rune.syn_condition.Length; j += 2)
            {
                for (int l = 0; l < 2; l++)
                {
                    runeSB.Append(rune.syn_condition[j, l] + ",");
                }
            }

            string[] synID = runeSB.ToString().Split(',');

            runeSB.Length = 0;

            int count = 0;

            if (runesDic.ContainsKey(long.Parse(synID[0])))
            {

                count = runesDic[long.Parse(synID[0])] / int.Parse(synID[1]);

                if (playerData.GetInstance().baginfo.gold < (rune.syn_cost * count))
                {
                    Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
                    return;
                }

                runesGlod += (rune.syn_cost * count);

                if (count > 0)
                {
                    if (runesDic.ContainsKey(rune.props_id))
                    {
                        runesDic[rune.props_id] += count;
                    }
                    else
                    {
                        runesDic.Add(rune.props_id, count);
                    }

                    GoodsDataOperation.GetInstance().UseGoods(int.Parse(synID[0]), int.Parse(synID[1]) * count);
                    GoodsDataOperation.GetInstance().AddGoods((int)rune.props_id, count);

                    runesDic[long.Parse(synID[0])] -= (int.Parse(synID[1]) * count);

                }
            }

            if (count > 0)
            {
                if (upGradeDic.ContainsKey(int.Parse(synID[0])))
                {
                    upGradeDic[int.Parse(synID[0])] += count;
                }
                else
                {
                    upGradeDic.Add(int.Parse(synID[0]), count);
                }
            }
        }

        indexK++;
        RunesRecursion();
    }

    
}
