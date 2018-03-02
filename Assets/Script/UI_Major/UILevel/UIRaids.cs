using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;

public class UIRaids : MonoBehaviour
{

    public GUISingleButton backBtn;
    public GUISingleButton raidsBtn;
    public GUISingleButton defineBtn;
    public UIScrollView scrollView;
    public UIGrid itemGrid;

    GameObject itemRaids;
    GameObject go;

    UISceneEntry SceneEntry;
    SceneNode sceneNode;

    int raidsCount = 0;
    int itemIndex = 0;
    Vector3 itemPos;

    object[] item;
    object[] other;

    void Start()
    {
        backBtn.onClick = OnDefineBtnClick;
        raidsBtn.onClick = OnRaidsBtnClick;
        defineBtn.onClick = OnDefineBtnClick;
    }

    public void AddRaidsItem(object[] item, object[] other, SceneNode sn)
    {
        this.other = null;
        this.item = new object[null != other ? item.Length + 1 : item.Length];
        for (int i = 0; i < item.Length; i++)
        {
            this.item[i] = item[i];
        }

        if (null != other)
        {
            this.item[this.item.Length - 1] = other;
            this.other = other;
        }

        itemIndex = 0;
        raidsCount = 0;

        scrollView.ResetPosition();
        ClearItemGrid();
        itemPos = new Vector3(-21, 0, 0);
        InvokeRepeating("ShowItemRaids", 0, 0.5f);

        sceneNode = sn;
        if (null == SceneEntry)
            SceneEntry = transform.parent.GetComponent<UISceneEntry>();
        raidsBtn.SetState(GUISingleButton.State.Normal);
        backBtn.isEnabled = false;
        SetBtnStateCollider(false);

        if (SceneEntry.type != OpenSourceType.Dungeons)
        {
            GameLibrary.eventdList[(sn.bigmap_id - 30000) / 100] -= SceneEntry.CleanoutCount;
            //SceneEntry.RefreshUI(sn);
            Control.ShowGUI(UIPanleID.SceneEntry, EnumOpenUIType.DefaultUIOrSecond, false, sn);
            raidsBtn.isEnabled = GameLibrary.eventdList[(sceneNode.bigmap_id - 30000) / 100] <= 0 ? false : true;
        }
    }


    void ShowItemRaids()
    {
        raidsCount++;

        if (null == itemRaids)
            itemRaids = Resources.Load("Prefab/UIPanel/RaidsItem") as GameObject;

        go = NGUITools.AddChild(itemGrid.gameObject, itemRaids);

        go.transform.parent = itemGrid.transform;
        ItemRaids itemRai = go.GetComponent<ItemRaids>();
        if (null != other && raidsCount == item.Length)
        {
            itemRai.RefreshUI(item[itemIndex], sceneNode.power_cost, 0);
        }
        else
        {
            itemRai.RefreshUI(item[itemIndex], sceneNode.power_cost, raidsCount);
        }

        itemIndex++;

        if (itemIndex > item.Length - 1)
        {
            //scrollView.ResetPosition();
            CancelInvoke("ShowItemRaids");
            SetBtnStateCollider(true);
            backBtn.isEnabled = true;
            if (SceneEntry.type != OpenSourceType.Dungeons)
            {
                if (GameLibrary.eventdList[(sceneNode.bigmap_id - 30000) / 100] <= 0)
                {
                    raidsBtn.SetState(GUISingleButton.State.Disabled);
                }
                else
                {
                    raidsBtn.SetState(GUISingleButton.State.Normal);
                }
            }
            else
            {
                raidsBtn.isEnabled = true;
            }
        }

        itemGrid.Reposition();
        if (raidsCount > 2)
        {
            itemPos += new Vector3(0, 216, 0);
        }

        SpringPanel.Begin(scrollView.gameObject, itemPos, 8);
    }

    public void ClearItemGrid()
    {
        for (int i = itemGrid.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(itemGrid.GetChild(i).gameObject);
        }
    }

    void OnRaidsBtnClick()
    {
        SceneEntry.BuySweepVoucher();
        SceneEntry.CleanoutDungeons(SceneEntry.CleanoutCount);
    }

    void OnDefineBtnClick()
    {
        raidsCount = 0;
        CancelInvoke("ShowItemRaids");
        SceneEntry.RefreshHeroPlay();
        SceneEntry.BuySweepVoucher();
        gameObject.SetActive(false);
    }

    void SetBtnStateCollider(bool isClick)
    {
        raidsBtn.isEnabled = isClick;
        defineBtn.isEnabled = isClick;
    }

}