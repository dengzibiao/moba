using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ItemRaids : MonoBehaviour
{

    public UILabel RaidsCount;
    public UILabel Vitality;
    public UILabel Glod;
    public UILabel Exp;
    public UIGrid itenGrid;
    public GameObject[] GoodItem;


    ItemNodeState itemNode;

    public void RefreshUI(object item, int exp, int raidsCount)
    {
        object[] itemGood = null;
        if (raidsCount != 0)
        {
            Dictionary<string, object> data = item as Dictionary<string, object>;
            RaidsCount.text = "第" + raidsCount + "次扫荡";
            Vitality.text = "-" + exp;
            Glod.text = "+" + int.Parse(data["gold"].ToString());
            Exp.text = "+" + exp;
            itemGood = data["item"] as object[];
        }
        else
        {
            transform.Find("Other").gameObject.SetActive(false);
            transform.Find("BG").transform.localPosition += new Vector3(0, 40, 0);
            RaidsCount.text = "额外奖励";
            Vitality.text = "";
            Glod.text = "";
            Exp.text = "";
            itemGood = item as object[];
        }

        Dictionary<int, int> itemList = new Dictionary<int, int>();
        Dictionary<string, object> itemData = new Dictionary<string, object>();
        int id = 0;
        int count = 0;

        if (null == itemGood)
        {
            for (int i = 0; i < GoodItem.Length; i++)
            {
                GoodItem[i].SetActive(false);
            }
            return;
        }

        for (int i = 0; i < itemGood.Length; i++)
        {
            itemData = itemGood[i] as Dictionary<string, object>;
            id = (int)itemData["id"];
            count = (int)itemData["at"];
            if (itemList.ContainsKey(id))
                itemList[(int)itemData["id"]] += count;
            else
                itemList.Add(id, count);
        }

        List<int> key = new List<int>(itemList.Keys);
        for (int i = 0; i < GoodItem.Length; i++)
        {
            if (i < itemList.Count)
            {
                GoodItem[i].SetActive(true);
                RefreshItem(GoodItem[i], key[i], itemList[key[i]]);
            }
            else
            {
                GoodItem[i].SetActive(false);
            }
        }
        itenGrid.Reposition();
    }

    void RefreshItem(GameObject item, long id, int count)
    {
        if (!GameLibrary.Instance().ItemStateList.ContainsKey(id))
            return;
        itemNode = GameLibrary.Instance().ItemStateList[id];
        if (itemNode.types == 6 || itemNode.types == 7)
        {
            item.transform.Find("Icon").GetComponent<UISprite>().atlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
        }
        else
        {
            item.transform.Find("Icon").GetComponent<UISprite>().atlas = ResourceManager.Instance().GetUIAtlas("Prop");
        }
        ItemData.SetAngleMarking(item.transform.Find("Subscript").GetComponent<UISprite>(), itemNode.types);
        item.GetComponent<UISprite>().spriteName = ItemData.GetFrameByGradeType((GradeType)itemNode.grade);
        item.transform.Find("Icon").GetComponent<UISprite>().spriteName = itemNode.icon_name;
        item.transform.Find("Count").GetComponent<UILabel>().text = "" + count;
    }


}
