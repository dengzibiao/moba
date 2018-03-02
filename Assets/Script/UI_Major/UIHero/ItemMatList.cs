using UnityEngine;
using System.Collections;

public class ItemMatList : GUISingleItemList
{

    #region 字段


    GUISingleButton matBtn;         //装备
    ItemNodeState itemNode;
    ItemData item;

    int needMat = 0;

    #endregion

    #region 属性

    public bool isEnough { get; set; }      //是否足够
    //public int comCount  { get; set; }      //可合成个数

    #endregion

    /// <summary>
    /// item初始化函数
    /// </summary>
    protected override void InitItem()
    {
        //初始化
        matBtn = transform.Find("MatBtn").GetComponent<GUISingleButton>();
        matBtn.onClick = OnBtnClick;
    }

    /// <summary>
    /// 信息赋值
    /// </summary>
    /// <param name="obj"></param>
    public override void Info(object obj)
    {

        itemNode = (ItemNodeState)obj;

        if (obj == null)
        {
            
        }
        else
        {

            EquipInfoPanel.instance.needMatDic.TryGetValue(itemNode.props_id, out needMat);

            matBtn.spriteName = itemNode.icon_name;

            //获取背包中所需材料的个数
            item = playerData.GetInstance().GetItemDatatByID(itemNode.props_id);

            //背包中是否有该材料
            if (null == item)
            {
                matBtn.text = 0 + "/" + needMat;
                isEnough = false;
                matBtn.transform.Find("Label").GetComponent<UILabel>().color = Color.red;
            }
            else
            {
                //计算可合成数量
                //comCount = needMat / item.numb;

                matBtn.text = item.Count + "/" + needMat;

                if (item.Count >= needMat)
                {
                    isEnough = true;
                    matBtn.transform.Find("Label").GetComponent<UILabel>().color = Color.white;
                }
                else
                {
                    isEnough = false;
                    matBtn.transform.Find("Label").GetComponent<UILabel>().color = Color.red;
                }

            }

            if (EquipInfoPanel.instance.itemmatl.Contains(this))
            {

                EquipInfoPanel.instance.itemmatl.Remove(this);
            }

            EquipInfoPanel.instance.itemmatl.Add(this);

        }

    }

    /// <summary>
    /// 更新药品数量
    /// </summary>
    public void UpdateCount()
    {

        item = playerData.GetInstance().GetItemDatatByID(itemNode.props_id);

        if (null == item)
        {
            matBtn.text = 0 + "/" + needMat;
            matBtn.transform.Find("Label").GetComponent<UILabel>().color = Color.red;
        }
        else
        {

            matBtn.text = item.Count + "/" + needMat;

            if (item.Count >= needMat)
            {
                isEnough = true;
                matBtn.transform.Find("Label").GetComponent<UILabel>().color = Color.white;
            }
            else
            {
                isEnough = false;
                matBtn.transform.Find("Label").GetComponent<UILabel>().color = Color.red;
            }

        }
    }

    /// <summary>
    /// 点击材料按钮
    /// </summary>
    private void OnBtnClick()
    {

        //无可合成材料，打开获取途径面板
        if (null == itemNode.syn_condition || itemNode.syn_condition.Length <= 0)
        {
            Control.Show(UIPanleID.UIGetWayPanel);
        }
        else
        {
            //隐藏装备信息，显示材料信息
            EquipInfoPanel.instance.HideEquipInfo(true, itemNode, needMat);

            EquipInfoPanel.instance.lastItem = itemNode;
        }

    }

}