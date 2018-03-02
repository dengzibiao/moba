using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class GUISingleMultList : GUIComponentBase
{
    #region Init
    public delegate void VoidMultList(int index);
    /// <summary>
    /// (int index)
    /// </summary>
    public VoidMultList onClick;
    private List<GUISingleItemList> list = new List<GUISingleItemList>();

    private Transform A;
    private Transform B;
    private Transform C;

    private float rowSpace;
    private float colSpace;

    private UIScrollView scrollView;

    protected override void Init()
    {
        //print(this.transform.name);
        A = this.transform.Find("A");
        B = this.transform.Find("B");
        C = this.transform.Find("C");

        if (A.GetComponent<GUISingleItemList>() == null) A.gameObject.AddComponent<GUISingleItemList>();

        rowSpace = B.localPosition.x - A.localPosition.x;
        colSpace = C.localPosition.y - A.localPosition.y;

        NGUITools.Destroy(B);
        NGUITools.Destroy(C);
        B = null;
        C = null;
        //print(A);

        A.gameObject.SetActive(false);
    }
    /// <summary>
    /// 设置list样式,count=0表示最后一行全满
    /// </summary>
    /// <param name="row">行数</param>
    /// <param name="col">列数</param
    /// <param name="count">最后一行数量</param>
    public void InSize(int row, int col, int count, int max)
    {
        GameObject go = null;
        float xOffset = 0;
        float yOffset = 0;
        if (list.Count>0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Destroy(list[i].gameObject);
            }
        }
       

        list.Clear();
        for (int i = 0; i < row; i++)
        {
            if (i == row - 1) col = count == 0 ? col : count;

            for (int j = 0; j < col; j++)
            {
                //Debug.Log(A+":"+this.gameObject.name);
                go = NGUITools.AddChild(this.gameObject, A.gameObject);
                go.transform.localPosition = A.localPosition;
                go.gameObject.SetActive(true);

                xOffset = rowSpace * j;
                yOffset = colSpace * i;

                go.transform.localPosition = new Vector3(go.transform.localPosition.x + xOffset, go.transform.localPosition.y + yOffset, go.transform.localPosition.z);

                list.Add(go.GetComponent<GUISingleItemList>());
                //UIGuidePanel.Single().SetObject(go);
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            list[i].index = i;
            list[i].onClick = OnComponentClick;
        }
        if (list.Count>0)
        {
            //当前transform的UIWidget会根据物品的多少而变化，用于背景大小的动态变化
            transform.GetComponent<UIWidget>().width =(int)list[list.Count - 1].transform.localPosition.x+ (int)rowSpace /2; 
        }
    }

    /// <summary>
    /// len:数据长度，max：每行最大个数
    /// </summary>
    public void InSize(int len, int max)
    {
        if (max == 0) return;
        int row = len / max;
        int col = len % max;
        if (col != 0) row++;
        InSize(row, max, col, max);
    }

    private void OnComponentClick(int index)
    {
        if (onClick != null) onClick(index);
        for (int i = 0; i < list.Count; i++)
        {
            (list[i]).A_Sprite.color = Color.white;
        }

        //选中效果演示,正式使用要更改
        // list[index].A_Sprite.color = Color.green;
    }
    /// <summary>
    /// 返回List列表
    /// </summary>
    /// <returns></returns>
    public List<GUISingleItemList> ItemList()
    {
        return list;
    }
    public GUISingleItemList GetItemByIndex(int index)
    {
        return list[index];
    }
    public void Info(params object[] infos)
    {
        object[] info = infos as object[];
        for (int i = 0; i < list.Count; i++)
        {
            if (i + 1 <= info.Length)
            {
                list[i].Info(info[i]);
            }
            else
            {
                list[i].Info(null);
            }
        }
    }

    /// <summary>
    /// 增加ScrollView
    /// </summary>
    public Transform ScrollView
    {
        set
        {
            scrollView = value.GetComponent<UIScrollView>();
            this.transform.parent = scrollView.transform;
            //此处不要加下面代码
            //this.AddBoxCollider();
            // NGUITools.AddMissingComponent<UIDragScrollView>(this.gameObject);    
        }
    }
    #endregion
    /// <summary>
    /// 释放Item
    /// </summary>
    public void Release()
    {
        for (int i = 0; i < list.Count; i++)
        {
            Destroy(list[i].gameObject);
        }
        list.Clear();
    }

}