using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;

public class HeroPreviewList : GUISingleItemList
{
    public UILabel propType;
    public UILabel FrontNum;
    public UILabel QueenNum;
    public List<HeroAttrNode> itemRankList = new List<HeroAttrNode>();
    protected override void InitItem()
    {
        propType = transform.Find("propType").GetComponent<UILabel>();
        FrontNum = transform.Find("propFrontNum").GetComponent<UILabel>();
        QueenNum = transform.Find("propQueenNum").GetComponent<UILabel>();
    }

    public override void Info(object obj)
    {
        itemRankList = HeroPreview.itemRankList;
        for (int i = 0; i < UI_HeroDetail.instance.propTypeName.Length; i++)
        {
            if (index == i)
            {
                propType.text = UI_HeroDetail.instance.propTypeName[i];
            }
        }
        for (int i = 0; i < itemRankList.Count; i++)
        {
            if (itemRankList[i].grade == UI_HeroDetail.hd.grade)
            {
                switch (index)
                {
                    case 0:
                        FrontNum.text = itemRankList[i].power.ToString();
                        break;
                    case 1:
                        FrontNum.text = itemRankList[i].intelligence.ToString();
                        break;
                    case 2:
                        FrontNum.text = itemRankList[i].agility.ToString();
                        break;
                    case 3:
                        FrontNum.text = itemRankList[i].hp.ToString();
                        break;
                    case 4:
                        FrontNum.text = itemRankList[i].attack.ToString();
                        break;
                    case 5:
                        FrontNum.text = itemRankList[i].armor.ToString();
                        break;
                    case 6:
                        FrontNum.text = itemRankList[i].magic_resist.ToString();
                        break;
                    case 7:
                        FrontNum.text = itemRankList[i].critical.ToString();
                        break;
                    case 8:
                        FrontNum.text = itemRankList[i].dodge.ToString();
                        break;
                    case 9:
                        FrontNum.text = itemRankList[i].hit_ratio.ToString();
                        break;
                    case 10:
                        FrontNum.text = itemRankList[i].armor_penetration.ToString();
                        break;
                    case 11:
                        FrontNum.text = itemRankList[i].magic_penetration.ToString();
                        break;
                    case 12:
                        FrontNum.text = itemRankList[i].suck_blood.ToString();
                        break;
                    case 13:
                        FrontNum.text = itemRankList[i].tenacity.ToString();
                        break;
                    case 14:
                        FrontNum.text = itemRankList[i].movement_speed.ToString();
                        break;
                    case 15:
                        FrontNum.text = itemRankList[i].attack_speed.ToString();
                        break;
                    case 16:
                        FrontNum.text = itemRankList[i].field_distance.ToString();
                        break;
                    case 17:
                        FrontNum.text = itemRankList[i].hp_regain.ToString();
                        break;
                }
            }
            if (itemRankList[i].grade == UI_HeroDetail.hd.grade + 1)
            {
                switch (index)
                {
                    case 0:
                        QueenNum.text = itemRankList[i].power.ToString();
                        break;
                    case 1:
                        QueenNum.text = itemRankList[i].intelligence.ToString();
                        break;
                    case 2:
                        QueenNum.text = itemRankList[i].agility.ToString();
                        break;
                    case 3:
                        QueenNum.text = itemRankList[i].hp.ToString();
                        break;
                    case 4:
                        QueenNum.text = itemRankList[i].attack.ToString();
                        break;
                    case 5:
                        QueenNum.text = itemRankList[i].armor.ToString();
                        break;
                    case 6:
                        QueenNum.text = itemRankList[i].magic_resist.ToString();
                        break;
                    case 7:
                        QueenNum.text = itemRankList[i].critical.ToString();
                        break;
                    case 8:
                        QueenNum.text = itemRankList[i].dodge.ToString();
                        break;
                    case 9:
                        QueenNum.text = itemRankList[i].hit_ratio.ToString();
                        break;
                    case 10:
                        QueenNum.text = itemRankList[i].armor_penetration.ToString();
                        break;
                    case 11:
                        QueenNum.text = itemRankList[i].magic_penetration.ToString();
                        break;
                    case 12:
                        QueenNum.text = itemRankList[i].suck_blood.ToString();
                        break;
                    case 13:
                        QueenNum.text = itemRankList[i].tenacity.ToString();
                        break;
                    case 14:
                        QueenNum.text = itemRankList[i].movement_speed.ToString();
                        break;
                    case 15:
                        QueenNum.text = itemRankList[i].attack_speed.ToString();
                        break;
                    case 16:
                        QueenNum.text = itemRankList[i].field_distance.ToString();
                        break;
                    case 17:
                        QueenNum.text = itemRankList[i].hp_regain.ToString();
                        break;
                }
            }
        }
    }
}
