using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class UIPopLottery : GUIBase
{
    public GUISingleSprite sprite;
    public GUISingleLabel label;
    public GUISingleButton ensureBtn;
    private static UIPopLottery instance;
    private ItemNodeState vo;
    private ItemData voo;
    private int count = 0;
    private bool isShow = false;
    private int _index = 0;
    private LotteryType _type;//区分魂匣和普通抽奖只要不是魂匣都是普通抽奖
    private List<long> showList = new List<long>();

    public UIPopLottery()
    {
        instance = this;
    }

    protected override void Init()
    {
        ensureBtn.onClick = OnEnsureBtn;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIPopLottery;
    }
    private void OnEnsureBtn()
    {
        //  GameLibrary.ItemTime = 0;
        showList.RemoveAt(0);
        isShow = false;
        if (_index == GameLibrary.lotteryCount)
        {
            _index = 0;
            Hide();
            Control.HideGUI(this.GetUIKey());
            return;
        }
        else
        {
            if (_type == LotteryType.GoldLottery) UIResultLottery.instance.IsShowDebris(_index + 1);
            else if (_type == LotteryType.None)
            {

            }
            _index = 0;
            Hide();
            Control.HideGUI(this.GetUIKey());
        }

    }
    public static UIPopLottery Instance
    {
        get { return instance; }
        set { instance = value; }
    }

    public void InitShow(long ID, int index, LotteryType type)
    {
        if (index != -1)
            _index = index;
        _type = type;
        showList.Add(ID);

    }

    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams[0] != null)
        {
            showList.Add(long.Parse(uiParams[0].ToString()));
        }
        if (uiParams[1] != null&& int.Parse(uiParams[1].ToString())!=-1)
        {
            _index = int.Parse(uiParams[1].ToString());
        }
        if (uiParams[2] != null)
            _type = (LotteryType)uiParams[2];

        base.SetUI(uiParams);
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        if (showList.Count > 0)Show();
    }

    protected override void ShowHandler()
    {
        if (showList.Count > 0)
        {
            isShow = true;
            int heroId = int.Parse(201 + StringUtil.SubString(showList[0].ToString(), 6, 3));
            if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(heroId))
            {
                int startId = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].init_star;
                count = FSDataNodeTable<StarUpGradeNode>.GetSingleton().DataNodeList[startId].convert_stone_num;
            }
            string a = StringUtil.SubString(showList[0].ToString(), 6, 3);
            int id = int.Parse(106 + StringUtil.SubString(showList[0].ToString(), 6, 3));
            vo = GameLibrary.Instance().ItemStateList[id];
            label.text = "您已经拥有此英雄,故赠送您" + count + "个" + vo.name;
            //  Debug.Log(label.text + "      ");
            sprite.spriteName = vo.icon_name;
            // StartCoroutine(DelayTime());
            //GoodsDataOperation.GetInstance().AddGoods(id,count);
        }
        else
        {
            Hide();
            Control.HideGUI(this.GetUIKey());
        }
    }



    private IEnumerator DelayTime()
    {
        yield return new WaitForSeconds(0.5f);
    }
}

