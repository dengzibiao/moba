/*
文件名（File Name）:   NewBehaviourScript.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(GUIDynamicMultList))]
[AddComponentMenu("NGUI/Interaction/GUIDynamicMultList")]
public class GUIDynamicMultList : GUIComponentBase
{
    public delegate void OnInitializeItem(GameObject go, int wrapIndex, int realIndex);
    public delegate void VoidMultList(int index);
    public VoidMultList onClick;

    public int itemSize = 0;//item的宽度或高度
    public int dataMinIndex = 0;//数据的起始位置
    public int dataMaxIndex = 0;//数据的终止位置
    public int showItemCount;
    private bool climp = false;//限制item超出数据个数限制
    private bool upOrDown = false;//向上为true
    private int countIndex = 0;//记录数据的初始和结束后不更新数据
    public bool cullContent = true;//是都隐藏超出边界的Item
    private float slipView = 0.5f;
    private List<GUIDynamicItemList> list = new List<GUIDynamicItemList>();//item的列表
    protected Transform mTrans;
    protected UIPanel mPanel;
    [HideInInspector]
    public int minIndex = 0;
    [HideInInspector]
    public int maxIndex = 0;
    protected UIScrollView mScroll;
    private float mScrollDrag;//滑动的时候存储变化的ScrollView的值
    private float mScrollStartDrag;
    protected bool mHorizontal = false;// 竖向滑动
    protected bool mFirstTime = true;
    public bool hideInactive = false;//隐藏的
    protected List<Transform> mChildren = new List<Transform>();
    private Transform A;
  //  private Transform B;
  //  private Transform C;
    private float rowSpace;//横向上一个transform和下一个的中心点间距
    private float colSpace;//纵向
    Dictionary<int, GUIDynamicItemList> itemlist = new Dictionary<int, GUIDynamicItemList>();

    /// <summary>
    /// Callback that will be called every time an item needs to have its content updated.
    /// The 'wrapIndex' is the index within the child list, and 'realIndex' is the index using position logic.
    /// </summary>

    public OnInitializeItem onInitializeItem;
    protected override void Init()
    {
        if (!CacheScrollView()) return;
        mScrollStartDrag = mScroll.transform.localPosition.y;
        mScrollDrag = mScrollStartDrag;
        A = this.transform.Find("A");
      //  B = this.transform.Find("B");
       // C = this.transform.Find("C");
        //如果没有在编辑界面设置，那么就自动获取当前视图操作的A的大小
        if (mHorizontal && itemSize == 0) itemSize = A.GetComponent<UIWidget>().width;
        else if(itemSize == 0) itemSize = A.GetComponent<UIWidget>().height;
        if (A.GetComponent<GUIDynamicItemList>() == null)
        {

            GUIDynamicItemList  item =A.gameObject.AddComponent<GUIDynamicItemList>();
            itemlist.Add(itemlist.Count, item);
        }
        rowSpace = itemSize;
        colSpace = itemSize;
      //  rowSpace = B.localPosition.x - A.localPosition.x;
      // colSpace = C.localPosition.y - A.localPosition.y;

        //NGUITools.Destroy(B);
        //  NGUITools.Destroy(C);
        //  B = null;
        // C = null;
        A.gameObject.SetActive(false);
    }
    protected override void OnStart()
    {
        base.OnStart();
        if (mScroll != null) mScroll.GetComponent<UIPanel>().onClipMove = OnMove;
        mFirstTime = false;
    }

    private void DragFinished()
    {
        if (climp && !upOrDown)
        {
            mScroll.transform.localPosition = new Vector3(0, mScrollDrag, 0);
        }
        if (!climp && !upOrDown)
        {
            mScroll.transform.localPosition = new Vector3(0, mScrollStartDrag, 0);
        }
    }

    /// <summary>
    /// 将所有内容,根据需要重新定位所有的子集。
    /// </summary>
    public virtual void WrapContent()
    {
        float extents = itemSize * mChildren.Count * 0.5f;

        Vector3[] corners = mPanel.worldCorners;

        for (int i = 0; i < 4; ++i)
        {
            Vector3 v = corners[i];
            v = mTrans.InverseTransformPoint(v);
            corners[i] = v;
        }

        Vector3 center = Vector3.Lerp(corners[0], corners[2], 0.5f);
        bool allWithinRange = true;
        float ext2 = extents * 2f;

        #region 左右滑动

        if (mHorizontal)
        {
            float min = corners[0].x - itemSize;
            float max = corners[2].x + itemSize;

            for (int i = 0, imax = mChildren.Count; i < imax; ++i)
            {
                Transform t = mChildren[i];
                float distance = t.localPosition.x - center.x;

                if (distance < -extents)
                {
                    Vector3 pos = t.localPosition;
                    pos.x += ext2;
                    distance = pos.x - center.x;
                    int realIndex = Mathf.RoundToInt(pos.x / itemSize);

                    if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                    {
                        t.localPosition = pos;
                        UpdateItem(t, i);
                    }
                    else allWithinRange = false;
                }
                else if (distance > extents)
                {
                    Vector3 pos = t.localPosition;
                    pos.x -= ext2;
                    distance = pos.x - center.x;
                    int realIndex = Mathf.RoundToInt(pos.x / itemSize);

                    if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                    {
                        t.localPosition = pos;
                        UpdateItem(t, i);
                    }
                    else allWithinRange = false;
                }
                else if (mFirstTime) UpdateItem(t, i);

                if (cullContent)
                {
                    distance += mPanel.clipOffset.x - mTrans.localPosition.x;
                    if (!UICamera.IsPressed(t.gameObject))
                        NGUITools.SetActive(t.gameObject, (distance > min && distance < max), false);
                }
            }
        }
        #endregion
        #region 上下滑动

        else
        {
            float a = mScroll.transform.localPosition.y ;

            float min = corners[0].y - itemSize;
            float max = corners[2].y + itemSize;
            if (a- itemSize> mScrollStartDrag)
            {
                mScrollDrag = a;
                   countIndex = Mathf.RoundToInt((a - mScrollStartDrag)/itemSize);
                print(countIndex + " 向上 ");
            }
            if (mScrollStartDrag - itemSize>a)
            {
                mScrollDrag = a;
                countIndex = Mathf.RoundToInt((mScrollStartDrag - a) / itemSize);
                print(countIndex + " 向下 ");
            }
            for (int i = 0, imax = mChildren.Count; i < imax; ++i)
            {
                Transform t = mChildren[i];
                float distance = t.localPosition.y - center.y;
                //向下滑
                if (distance < -extents)
                {
                   
                    Vector3 pos = t.localPosition;
                    pos.y += ext2;
                    distance = pos.y - center.y;
                    int realIndex = Mathf.RoundToInt(pos.y / itemSize);

                    if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                    {               
                        t.localPosition = pos;
                        UpdateItem(t, i);
                    }
                    else allWithinRange = false;
                } //向上滑
                else if (distance > extents)
                {
                    countIndex++;
                    print(countIndex + " 向上 ");
                    Vector3 pos = t.localPosition;
                    pos.y -= ext2;
                    distance = pos.y - center.y;
                    int realIndex = Mathf.RoundToInt(pos.y / itemSize);
                    if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                    {
                        t.localPosition = pos;
                        UpdateItem(t, i);
                    }
                    else allWithinRange = false;
                }
                else if (mFirstTime) UpdateItem(t, i);

                if (cullContent)
                {
                    distance += mPanel.clipOffset.y - mTrans.localPosition.y;
                    if (!UICamera.IsPressed(t.gameObject))
                        NGUITools.SetActive(t.gameObject, (distance > min && distance < max), false);
                }
            }
        }
        #endregion

        mScroll.restrictWithinPanel = !allWithinRange; //拖动是否会被限制在滚动视图的范围内
        mScroll.InvalidateBounds();
    }

    /// <summary>
    /// ScrollView滑动的时候调用
    /// </summary>
    /// <param name="panel"></param>
    protected virtual void OnMove(UIPanel panel) { WrapContent(); }
    /// <summary>
    /// 初始化组件mScroll不为空返回true
    /// </summary>
    /// <returns></returns>
    protected bool CacheScrollView()
    {
        mTrans = transform;
        mPanel = NGUITools.FindInParents<UIPanel>(gameObject);
        mScroll = mPanel.GetComponent<UIScrollView>();
        if (mScroll == null) return false;
        if (mScroll.movement == UIScrollView.Movement.Horizontal) mHorizontal = true;
        else if (mScroll.movement == UIScrollView.Movement.Vertical) mHorizontal = false;
        else return false;
        return true;
    }

    /// <param name="row">行数</param>
    /// <param name="col">列数</param
    /// <param name="count">最后一行数量</param>
    public void InSize(int len, int col)
    {
        int row = len / col;
        int count = len % col;
        if (count != 0) row++;
        GameObject go = null;
        float xOffset = 0;
        float yOffset = 0;
        for (int i = 0; i < list.Count; i++)
        {
            Destroy(list[i].gameObject);
        }
        list.Clear();
        mChildren.Clear();
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

                list.Add(go.GetComponent<GUIDynamicItemList>());
                if (hideInactive && !go.activeInHierarchy) continue;
                mChildren.Add(go.transform);
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            list[i].index = i;
            list[i].onClick = OnComponentClick;
        }
    }
    private void OnComponentClick(int index)
    {
        if (onClick != null) onClick(index);
    }
    public List<GUIDynamicItemList> ItemList()
    {
        return list;
    }
    public GUIDynamicItemList GetItemByIndex(int index)
    {
        return list[index];
    }
    /// <summary>
    /// Want to update the content of items as they are scrolled? Override this function.
    /// </summary>
    /// <summary>
    /// Sanity checks.
    /// </summary>

    void OnValidate()
    {
        if (maxIndex < minIndex)
            maxIndex = minIndex;
        if (minIndex > maxIndex)
            maxIndex = minIndex;
    }

    protected virtual void UpdateItem(Transform item, int index)
    {
        if (onInitializeItem != null)
        {
            int realIndex = (mScroll.movement == UIScrollView.Movement.Vertical) ?
                Mathf.RoundToInt(item.localPosition.y / itemSize) :
                Mathf.RoundToInt(item.localPosition.x / itemSize);
            onInitializeItem(item.gameObject, index, realIndex);
        }
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

        WrapContent();
    }
}


