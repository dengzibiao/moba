using UnityEngine;
using System.Collections;
using System;
using Tianyu;
using System.Collections.Generic;

public class GoodsGetWayItem : GUISingleItemList
{
    public UILabel wayName;
    public UILabel isOpen;
    public GUISingleButton btn;
    public UISprite daguanSprite;
    public UISprite bossIcon;
    private int id;
    private bool isOpened = false;
    private MapNode mapNode;
    private SceneNode sceneNode;
    private Transform openSprite;
    protected override void Init()
    {
        wayName = transform.Find("WayName").GetComponent<UILabel>();
        isOpen = transform.Find("IsOpen").GetComponent<UILabel>();
        btn = transform.Find("Btn").GetComponent<GUISingleButton>();
        daguanSprite = transform.Find("DaguanSprite").GetComponent<UISprite>();
        bossIcon = transform.Find("BossIcon").GetComponent<UISprite>();
        openSprite = transform.Find("OpenSprite");
        btn.onClick = OnBtnClick;
        
    }

    private void OnBtnClick()
    {
        if (99 < id && id < 30000)
        {
            Debug.Log("章节" + id);
            if (isOpened)
            {
                //Control.HideGUI(GameLibrary.UIGoodsGetWayPanel);
                Control.HideGUI(UIPanleID.UIGoodsGetWayPanel);
                //if (Control.GetGUI(GameLibrary.UI_HeroDetail).gameObject.activeSelf)
                //{
                //    Control.HideGUI(GameLibrary.UI_HeroDetail);
                //    HeroPosEmbattle.instance.HideModel();
                //    UI_HeroDetail.instance.HeroID = 0;
                //}
                object[] openParams = new object[] { OpenLevelType.ByIDOpen, id };
                Control.ShowGUI(UIPanleID.UILevel, EnumOpenUIType.OpenNewCloseOld, false, openParams);
            }
            else
            {
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "功能尚未解锁，无法前往");
                //UIPromptBox.Instance.ShowLabel("功能尚未解锁，无法前往");
            }
        }
        if (30000 < id)//试练
        {
            if (isOpened)
            {
                Debug.Log("试练" + id);
                Control.HideGUI(UIPanleID.UIGoodsGetWayPanel);
                //Control.HideGUI(GameLibrary.UIGoodsGetWayPanel);
                //if (Control.GetGUI(GameLibrary.UI_HeroDetail).gameObject.activeSelf)
                //{
                //    Control.HideGUI(GameLibrary.UI_HeroDetail);
                //    HeroPosEmbattle.instance.HideModel();
                //    UI_HeroDetail.instance.HeroID = 0;
                //}
                //Control.ShowGUI(GameLibrary.UIActivity);
                Control.ShowGUI(UIPanleID.UIActivity, EnumOpenUIType.OpenNewCloseOld, false);
            }
            else
            {
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "功能尚未解锁，无法前往");
                //UIPromptBox.Instance.ShowLabel("功能尚未解锁，无法前往");
            }
        }
        if (id == 3)//打开祭坛
        {
            Control.HideGUI(UIPanleID.UIGoodsGetWayPanel);
            Control.ShowGUI(UIPanleID.UILottery, EnumOpenUIType.OpenNewCloseOld);
            //UITooltips.Instance.SetBlackerBottom_Text("暂时无法跳转到祭坛");
            //if (Control.GetGUI(GameLibrary.UI_HeroDetail).gameObject.activeSelf)
            //{
            //    Control.HideGUI(GameLibrary.UI_HeroDetail);
            //}
            //Control.ShowGUI(GameLibrary.UILottery);
            //Debug.Log("打开祭坛");
            //Control.HideGUI(GameLibrary.UIGoodsGetWayPanel);
        }
        if (id == 4)//决斗场商店
        {
            //UIShop.Instance.IsShop(1);
            //Control.ShowGUI(GameLibrary.UIShop);
            object[] obj = new object[2] { 0, 1 };
            Control.ShowGUI(UIPanleID.UIShopPanel, EnumOpenUIType.OpenNewCloseOld, false, obj);
            Debug.Log("打开决斗场商店");
            //Control.HideGUI(GameLibrary.UIGoodsGetWayPanel);
            Control.HideGUI(UIPanleID.UIGoodsGetWayPanel);
        }
        if (id == 5)//竞技场商店
        {
            //UIShop.Instance.IsShop(2);
            //Control.ShowGUI(GameLibrary.UIShop);
            object[] obj = new object[2] { 0, 2 };
            Control.ShowGUI(UIPanleID.UIShopPanel, EnumOpenUIType.OpenNewCloseOld, false, obj);
            Debug.Log("打开竞技场商店");
            //Control.HideGUI(GameLibrary.UIGoodsGetWayPanel);
            Control.HideGUI(UIPanleID.UIGoodsGetWayPanel);
        }

        if (id == 6)//公会商店
        {
            //UIShop.Instance.IsShop(0);
            //Control.ShowGUI(GameLibrary.UIShop);
            object[] obj = new object[2] { 0, 0 };
            Control.ShowGUI(UIPanleID.UIShopPanel, EnumOpenUIType.OpenNewCloseOld, false, obj);
            Debug.Log("打开公会商店");
            //Control.HideGUI(GameLibrary.UIGoodsGetWayPanel);
            Control.HideGUI(UIPanleID.UIGoodsGetWayPanel);
        }
        
    }
    public string SceneIcon(int type)
    {
        switch (type)
        {
            case 1: return "KM";
            case 2: return "MOBA";
            case 3: return "TP";
            case 4: return "KV";
            case 5: return "TD";
            case 6: return "ES";
            default:
                return "BOSS";
        }
    }
    public override void Info(object obj)
    {
        if (obj != null) 
        {
            id = int.Parse(obj.ToString());
            bossIcon.gameObject.SetActive(false);
            if ( 99< id && id < 30000)//副本
            {
                if (FSDataNodeTable<SceneNode>.GetSingleton().DataNodeList.ContainsKey(id))
                {
                    sceneNode = FSDataNodeTable<SceneNode>.GetSingleton().DataNodeList[id];

                    
                    if (id % 2 == 0)
                    {
                        wayName.text = sceneNode.SceneName + "(精英)";
                    }
                    else
                    {
                        wayName.text = sceneNode.SceneName;
                    }
                    //daguanSprite.spriteName = SceneIcon(sceneNode.game_type);
                    //if (null != sceneNode.icon_name && sceneNode.icon_name != "0")
                    //{
                    //    bossIcon.gameObject.SetActive(true);
                    //    bossIcon.spriteName = sceneNode.icon_name;
                    //}
                    //else
                    //{
                    //    bossIcon.gameObject.SetActive(false);
                    //}
                    daguanSprite.spriteName = "maoxian";
                    //获得关卡是够已经开启
                    int bigmapID = sceneNode.bigmap_id;//大关id
                    if (GameLibrary.mapOrdinary.ContainsKey(bigmapID))
                    {
                        if (GameLibrary.mapOrdinary[bigmapID].ContainsKey(id))
                        {
                            if (FunctionOpenMng.GetInstance().GetFunctionOpen(bigmapID / 100))
                            {
                                if (Globe.GetStar(GameLibrary.mapOrdinary[bigmapID][id]) < 0)//通过小关星级判断是否开启 -1不开启
                                {

                                    isOpened = false;

                                }
                                else
                                {
                                    isOpened = true;
                                }
                            }
                            else
                            {
                                isOpened = false;
                            }
                        }
                    }
                    
                }
            }
            if (30000 < id)//试练
            {
                if (FSDataNodeTable<SceneNode>.GetSingleton().DataNodeList.ContainsKey(id))
                {
                    sceneNode = FSDataNodeTable<SceneNode>.GetSingleton().DataNodeList[id];
                    wayName.text = sceneNode.SceneName;
                    int bigmapID = sceneNode.bigmap_id;//大关id
                    //daguanSprite.spriteName = SceneIcon(sceneNode.game_type);
                    daguanSprite.spriteName = "shijian";
                    if (GameLibrary.eventsList.ContainsKey(bigmapID))
                    {
                        List<int[]> temarr = GameLibrary.eventsList[bigmapID];
                        string temStr = id.ToString();
                        int temcount = int.Parse(temStr.Substring(temStr.Length - 1, 1));//截取试练id的最后一位 然后去取相对应的星级
                        if (temcount > 1&& temarr.Count>=temcount)
                        {
                            if (Globe.GetStar(temarr[temcount - 1]) < 0)
                            {
                                isOpened = false;
                            }
                            else
                            {
                                isOpened = true;
                            }
                        }
                    }
                }
            }

            if (!isOpened)
            {
                isOpen.gameObject.SetActive(true);
                openSprite.gameObject.SetActive(false);
                isOpen.text = "[ff0000]未开放[-]";
            }
            else
            {
                isOpen.gameObject.SetActive(false);
                openSprite.gameObject.SetActive(true);
            }
            //打开面板
            if (id == 3)//祭坛
            {
                wayName.text = "祭坛";
                daguanSprite.spriteName = "jitan";
                HidePng();
            }
            //打开面板
            if (id == 4)//远征商店
            {
                wayName.text = "角斗场商店";
                daguanSprite.spriteName = "shangdian";
                HidePng();
            }
            if (id == 5)//竞技场商店
            {
                wayName.text = "竞技场商店";
                daguanSprite.spriteName = "shangdian";
                HidePng();
            }

            if (id == 6)//公会商店
            {
                wayName.text = "公会商店";
                daguanSprite.spriteName = "shangdian";
                HidePng();
            }
        }
    }

    private void HidePng()
    {
        isOpen.gameObject.SetActive(false);
        openSprite.gameObject.SetActive(false);
    }
}
