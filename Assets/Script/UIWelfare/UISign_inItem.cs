using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;

public class UISign_inItem : GUISingleItemList
{
    public UIAtlas Propatlas;
    public UIAtlas UIHeroHeadatlas;
    private UISprite Sign_inListOK;
    private UISprite OK;
    private UISprite VIP;
    public UISprite Sprite;//灵魂石
    public UISprite sprite;//边框
    public GUISingleSprite Icons;
    public GUISingleSprite icon;
    public GUISingleLabel count;
    public UILabel VIPNum;
    public Transform xuanZhongIcon;
    private UISign_inItem uiSign_inItem;
    private object item;
    private GradeType gradeTYPE = GradeType.Gray;
    DateTime time;
    public UIGrid grid;
    private int mStar;
    public List<GameObject> star = new List<GameObject>();
    protected override void InitItem()
    {
        sprite = transform.GetComponent<UISprite>();
        Sign_inListOK = transform.Find("Sign_inListOK").GetComponent<UISprite>();
        OK = transform.Find("OK").GetComponent<UISprite>();
        VIP = transform.Find("VIP").GetComponent<UISprite>();
        count = transform.Find("Count").GetComponent<GUISingleLabel>();
        xuanZhongIcon = transform.Find("XuanZhongIcon");
        time = TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime());
        if (index == 0)
        {
            transform.gameObject.SetActive(true);
        }
        icon.onClick = OnIconClick;


    }
    //点击领取签到物品
    private void OnIconClick()
    {
        for (int i = 0; i < UISign_inData.Instance().GoodsNumList.Length; i++)
        {
            if (index == i)
            {
                if (UISign_inData.Instance().GoodsNumList[0] != null)
                {
                    UISign_inData.Instance().DoodsID = Convert.ToInt32(UISign_inData.Instance().GoodsNumList[0]);
                    UISign_inData.Instance().DoodsNum = Convert.ToInt32(UISign_inData.Instance().GoodsNumList[1]);
                }
            }
        }
        for (int j = 0; j < UISign_in.itemRankList.Count; j++)
        {
            if (index == j)
            {
                if (UISign_in.itemRankList[j].reward_prop != null)
                {
                    UISign_inData.Instance().ItemNode = GameLibrary.Instance().ItemStateList[UISign_in.itemRankList[j].reward_prop[0]];
                    UISign_inData.Instance().ID = UISign_in.itemRankList[j].reward_prop[0];
                    UISign_inData.Instance().goodsType = true;
                }
                else
                {
                    UISign_inData.Instance().goodsType = false;
                }
            }
            if (int.Parse(playerData.GetInstance().singnData.Signed.Substring(4, 2)) < time.Day && index == int.Parse(playerData.GetInstance().singnData.Signed.Substring(6, 2)))
            {
                ClientSendDataMgr.GetSingle().GetUISign_inSend().SendGetUISign_in(C2SMessageType.PASVWait);//发送每日签到列表
                List<ItemData> datalist = new List<ItemData>();
                ItemData data = new ItemData();
                if (((UISign_inNode)item).reward_prop != null)
                {
                    if (int.Parse(((UISign_inNode)item).reward_prop[0].ToString().Substring(0, 3)) == 107 || int.Parse(((UISign_inNode)item).reward_prop[0].ToString().Substring(0, 3)) == 106)
                    {
                        data.UiAtlas = UIHeroHeadatlas;
                        data.IconName = GameLibrary.Instance().ItemStateList[((UISign_inNode)item).reward_prop[0]].icon_name;
                    }
                    else
                    {
                        data.UiAtlas = Propatlas;
                        data.IconName = GameLibrary.Instance().ItemStateList[((UISign_inNode)item).reward_prop[0]].icon_name.ToString();
                    }
                    data.Name = GameLibrary.Instance().ItemStateList[((UISign_inNode)item).reward_prop[0]].name;
                    data.Count = int.Parse(((UISign_inNode)item).reward_prop[1].ToString());
                    data.GradeTYPE = gradeTYPE;
                }
                else
                {
                    data.UiAtlas = Propatlas;
                    data.IconName = "zuanshi";
                    data.Name = "钻石";
                    data.Count = ((UISign_inNode)item).reward_money;
                    data.GradeTYPE = GradeType.Orange;
                }
                datalist.Add(data);
                TaskManager.Single().itemlist = datalist;
                UISign_inData.Instance()._index = index;
            }
            else
            {
                if (UISign_inData.Instance().goodsType != false)
                {
                    UIgoodstips.Instances.Setgoods(UISign_inData.Instance().ItemNode, UISign_inData.Instance().ID);
                    Control.Show(UIPanleID.UIgoodstips);
                }
                else
                {
                    //钻石操作
                    UIgoodstips.Instances.SetjewelImg(UIgoodstips.goodsType.jewel);
                    Control.Show(UIPanleID.UIgoodstips);
                }

            }
        }
    }
    public override void Info(object obj)
    {
        //Prop
        //    UIHeroHead
        item = obj;
        if (((UISign_inNode)obj).reward_prop != null)
        {
            UISign_inData.Instance().GoodsNumList = ((UISign_inNode)obj).reward_prop;
            string goodsID = ((UISign_inNode)obj).reward_prop[0].ToString();
            int heroId = int.Parse(201 + StringUtil.SubString(goodsID, 6, 3));
            int goodsIDNum = int.Parse(goodsID.Substring(0, 3));
            if (goodsIDNum == 107 || goodsIDNum == 106)
            {
                Icons.uiAtlas = UIHeroHeadatlas;
                Icons.spriteName = GameLibrary.Instance().ItemStateList[((UISign_inNode)obj).reward_prop[0]].icon_name;
            }
            else
            {
                Icons.uiAtlas = Propatlas;
                Icons.spriteName = GameLibrary.Instance().ItemStateList[((UISign_inNode)obj).reward_prop[0]].icon_name.ToString();
            }
            count.text = ((UISign_inNode)obj).reward_prop[1].ToString();
            if (goodsIDNum == 107)
            {
                count.text = "";
                if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(heroId))
                {
                    mStar = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].init_star;
                    for (int i = 0; i < mStar; i++)
                    {
                        star[i].SetActive(true);
                    }
                    for (int i = mStar; i < star.Count; i++)
                    {
                        star[i].SetActive(false);
                    }
                    grid.Reposition();
                }
            }
            if (goodsIDNum == 106)
            {
                Sprite.transform.gameObject.SetActive(true);
            }
            else
            {
                Sprite.transform.gameObject.SetActive(false);
            }

            switch (GameLibrary.Instance().ItemStateList[((UISign_inNode)obj).reward_prop[0]].grade)
            {
                case 1:
                    sprite.spriteName = "hui";
                    gradeTYPE = GradeType.Gray;
                    break;
                case 2:
                    sprite.spriteName = "lv";
                    gradeTYPE = GradeType.Green;
                    break;
                case 4:
                    sprite.spriteName = "lan";
                    gradeTYPE = GradeType.Blue;
                    break;
                case 7:
                    sprite.spriteName = "zi";
                    gradeTYPE = GradeType.Purple;
                    break;
                case 11:
                    sprite.spriteName = "cheng";
                    gradeTYPE = GradeType.Orange;
                    break;
                case 16:
                    sprite.spriteName = "hong";
                    gradeTYPE = GradeType.Red;
                    break;
                default:
                    break;
            }
        }
        else
        {
            //钻石图集
            //jewel.transform.gameObject.SetActive(true);
            Icons.uiAtlas = Propatlas;
            Icons.spriteName = "zuanshi";
            sprite.spriteName = "cheng";//钻石默认边框
            count.text = ((UISign_inNode)obj).reward_money.ToString();
        }

        if (index == int.Parse(playerData.GetInstance().singnData.Signed.Substring(6, 2)))
        {
            if (int.Parse(playerData.GetInstance().singnData.Signed.Substring(4, 2)) < time.Day)
            {
                OK.transform.gameObject.SetActive(true);
            }
        }
        if (index < int.Parse(playerData.GetInstance().singnData.Signed.Substring(6, 2)))
        {
            Sign_inListOK.transform.gameObject.SetActive(true);
        }
        if (((UISign_inNode)obj).vip_limit != 0)
        {
            VIP.transform.gameObject.SetActive(true);
            VIPNum.text = "双倍\n会员" + ((UISign_inNode)obj).vip_limit.ToString();
        }

    }
}
