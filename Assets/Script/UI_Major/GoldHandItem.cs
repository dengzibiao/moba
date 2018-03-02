using UnityEngine;
using System.Collections;

public class GoldHandItem : GUISingleItemList
{
    public UILabel jewelCount;
    public UILabel goldCount;
    public UILabel critCount;
    public GameObject cirtObj;
    private Transform cirt2;
    private Transform cirt5;
    private Transform cirt10;
    private GameObject target;
    GoldHandItemData item;
    public Transform towBaojiEffect;
    protected override void InitItem()
    {
        base.InitItem();
        jewelCount = transform.Find("JewelCount").GetComponent<UILabel>();
        goldCount = transform.Find("GoldCount").GetComponent<UILabel>();
        critCount = transform.Find("CritCount").GetComponent<UILabel>();
        cirtObj = transform.Find("Cirt").gameObject;
        target = GameObject.Find("UI Root/Camera");
        towBaojiEffect = transform.Find("dianjin_baoji");
        cirt2 = transform.Find("Cirt2");
        cirt5 = transform.Find("Cirt5");
        cirt10 = transform.Find("Cirt10");
        towBaojiEffect.gameObject.SetActive(false);
    }

    public override void Info(object obj)
    {
        base.Info(obj);
        item = (GoldHandItemData)obj;
        jewelCount.text = item.jewelCount + "";
        goldCount.text = item.goldCount + "";
        if (item.isCrit)
        {
            cirtObj.SetActive(true);
            //critCount.gameObject.SetActive(true);
            //critCount.text = "[dc2be5]" + "X"+item.critCount + ""+"[-]";
            if (item.critCount == 2) cirt2.gameObject.SetActive(true);
            if (item.critCount == 5) cirt5.gameObject.SetActive(true);
            if (item.critCount == 10) cirt10.gameObject.SetActive(true);
            if (index >= UIGoldHand.Instance.lastDataCount&& item.critCount == 2)
            {

                UIGoldHand.Instance.towBaojiEffect.gameObject.SetActive(false);
                UIGoldHand.Instance.towBaojiEffect.gameObject.SetActive(true);
            }
            else if(index >= UIGoldHand.Instance.lastDataCount && item.critCount == 5)
            {
                UIGoldHand.Instance.fiveBaojiEffect.gameObject.SetActive(false);
                UIGoldHand.Instance.fiveBaojiEffect.gameObject.SetActive(true);
            }
            else if (index >= UIGoldHand.Instance.lastDataCount && item.critCount == 10)
            {
                UIGoldHand.Instance.tenBaojiEffect.gameObject.SetActive(false);
                UIGoldHand.Instance.tenBaojiEffect.gameObject.SetActive(true);
            }
        }
        else
        {
            cirtObj.SetActive(false);
            critCount.gameObject.SetActive(false);
        }
    }
}
