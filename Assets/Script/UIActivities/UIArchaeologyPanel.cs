using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
/// <summary>
/// 考古面板
/// </summary>
public class UIArchaeologyPanel : GUIBase 
{
    public GUISingleCheckBoxGroup checkBoxs;
    public GUISingleMultList multList;
    public static UIArchaeologyPanel instance;
    public static UIArchaeologyPanel Instance { get { return instance; } set { instance = value; } }
    public UILabel[] OpenRuleLabs;
    public List<ItemData> archaeologyRewardList = new List<ItemData>();//储存奖励物品列表
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }


    protected override void Init()
    {
        base.Init();
        multList = transform.Find("MultList").GetComponent<GUISingleMultList>();           
        checkBoxs.onClick = OnCheckClick;
        checkBoxs.DefauleIndex = 0;
    }
    protected override void ShowHandler()
    {
        base.ShowHandler();
    }
    private void OnCheckClick(int index, bool boo)
    {
        if(boo)
        {
           switch(index)
           {
               case 0:
                   //按等级要求对按钮置灰以及提示
                   //if (playerData.GetInstance().selfData.level<=FSDataNodeTable<Archaeology_rewardNode>.GetSingleton().DataNodeList[1].grade)
                   //{
                   //    checkBoxs.GetComponentInChildren<GUISingleCheckBox>()._mark.color = new Color(0,0,0);
                   //    checkBoxs.GetComponentInChildren<GUISingleCheckBox>()._mark.GetComponent<BoxCollider>().enabled = false;
                   //    OpenRuleLabs[0].text=FSDataNodeTable<Archaeology_rewardNode>.GetSingleton().DataNodeList[1].grade+"级开放";
                   //}
                   //else { }
                   for (int i = 0; i < FSDataNodeTable<Archaeology_rewardNode>.GetSingleton().DataNodeList[1].rewardItem.Length/3; i++)
                   {
                      ItemData itemdata = new ItemData();
                      itemdata.GoodsType = MailGoodsType.ItemType;
                      itemdata.Id = FSDataNodeTable<Archaeology_rewardNode>.GetSingleton().DataNodeList[1].rewardItem[i, 0];
                      archaeologyRewardList.Add(itemdata);
                   }
                   multList.InSize(archaeologyRewardList.Count, 4);
                   multList.Info(archaeologyRewardList.ToArray());
                   archaeologyRewardList.Clear();
                   break;
               case 1:
                   //if (playerData.GetInstance().selfData.level <= FSDataNodeTable<Archaeology_rewardNode>.GetSingleton().DataNodeList[2].grade)
                   //{
                   //    checkBoxs.GetComponentInChildren<GUISingleCheckBox>()._mark.color = new Color(0, 0, 0);
                   //    checkBoxs.GetComponentInChildren<GUISingleCheckBox>()._mark.GetComponent<BoxCollider>().enabled = false;
                   //    OpenRuleLabs[0].text = FSDataNodeTable<Archaeology_rewardNode>.GetSingleton().DataNodeList[1].grade + "级开放";
                   //}
                   for (int i = 0; i < FSDataNodeTable<Archaeology_rewardNode>.GetSingleton().DataNodeList[2].rewardItem.Length / 3; i++)
                   {
                      ItemData itemdata = new ItemData();
                      itemdata.GoodsType = MailGoodsType.ItemType;
                      itemdata.Id = FSDataNodeTable<Archaeology_rewardNode>.GetSingleton().DataNodeList[2].rewardItem[i, 0];
                      archaeologyRewardList.Add(itemdata);
                   }
                   multList.InSize(archaeologyRewardList.Count, 4);
                   multList.Info(archaeologyRewardList.ToArray());
                   archaeologyRewardList.Clear();
                  // checkBoxs.GetComponentInChildren<GUISingleCheckBox>()._label.text = archaeologyRewardList[1].map_name;
                   break;
               case 2:
                   for (int i = 0; i < FSDataNodeTable<Archaeology_rewardNode>.GetSingleton().DataNodeList[3].rewardItem.Length / 3; i++)
                   {
                      ItemData itemdata = new ItemData();
                      itemdata.GoodsType = MailGoodsType.ItemType;
                      itemdata.Id = FSDataNodeTable<Archaeology_rewardNode>.GetSingleton().DataNodeList[3].rewardItem[i, 0];
                      archaeologyRewardList.Add(itemdata);
                   }
                   multList.InSize(archaeologyRewardList.Count, 4);
                   multList.Info(archaeologyRewardList.ToArray());
                   archaeologyRewardList.Clear();
                 //  checkBoxs.GetComponentInChildren<GUISingleCheckBox>()._label.text = archaeologyRewardList[2].map_name;
                   break;
               case 3:
                   for (int i = 0; i < FSDataNodeTable<Archaeology_rewardNode>.GetSingleton().DataNodeList[4].rewardItem.Length / 3; i++)
                   {
                      ItemData itemdata = new ItemData();
                      itemdata.GoodsType = MailGoodsType.ItemType;
                      itemdata.Id = FSDataNodeTable<Archaeology_rewardNode>.GetSingleton().DataNodeList[4].rewardItem[i, 0];
                      archaeologyRewardList.Add(itemdata);
                   }
                   multList.InSize(archaeologyRewardList.Count, 4);
                   multList.Info(archaeologyRewardList.ToArray());
                   archaeologyRewardList.Clear();
                 //  checkBoxs.GetComponentInChildren<GUISingleCheckBox>()._label.text = archaeologyRewardList[3].map_name;
                   break;
               case 4:
                   for (int i = 0; i < FSDataNodeTable<Archaeology_rewardNode>.GetSingleton().DataNodeList[5].rewardItem.Length / 3; i++)
                   {
                      ItemData itemdata = new ItemData();
                      itemdata.GoodsType = MailGoodsType.ItemType;
                      itemdata.Id = FSDataNodeTable<Archaeology_rewardNode>.GetSingleton().DataNodeList[5].rewardItem[i, 0];
                      archaeologyRewardList.Add(itemdata);
                   }
                   multList.InSize(archaeologyRewardList.Count, 4);
                   multList.Info(archaeologyRewardList.ToArray());
                   archaeologyRewardList.Clear();
                 //  checkBoxs.GetComponentInChildren<GUISingleCheckBox>()._label.text = archaeologyRewardList[4].map_name;
                   break;
               default:
                   break;
           }
        }
    }
}
