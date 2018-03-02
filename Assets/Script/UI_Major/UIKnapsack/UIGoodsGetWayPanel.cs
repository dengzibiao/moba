using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;

public class UIGoodsGetWayPanel : GUIBase
{
    public UISprite goods;
    public UISprite iconS;
    public GUISingleSprite icon;
    public UILabel count;
    public UILabel goodsName;
    public UILabel des;
    public GUISingleMultList multList;
    public GUISingleButton closeBtn;
    public UIScrollView scrollView;
    private ItemData itemdata;
    private long itemID = 0;
    GradeType gradeType = GradeType.Blue;
    private Transform debris;
    public GameObject backObj;
    private List<string> fubenIDList = new List<string>();
    private static UIGoodsGetWayPanel instance;
    public static UIGoodsGetWayPanel Instance { get { return instance; } set { instance = value; } }

    private object[] obj = new object[5];
    public UIGoodsGetWayPanel()
    {
        instance = this;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIGoodsGetWayPanel;
    }
    protected override void SetUI(params object[] uiParams)
    {
        itemID = (long)uiParams[0];
        base.SetUI();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        if (CheckMapData())
        {
            Singleton<Notification>.Instance.RegistMessageID(MessageID.pve_worldmap_list_ret, UIPanleID.UIGoodsGetWayPanel);
            Singleton<Notification>.Instance.RegistMessageID(MessageID.pve_dungeon_list_ret, UIPanleID.UIGoodsGetWayPanel);
            Singleton<Notification>.Instance.Send(MessageID.pve_worldmap_list_req, C2SMessageType.ActiveWait);
        }
        else
        {
            this.State = EnumObjectState.Ready;
            Show();
        }
        
    }
    bool CheckMapData()
    {
        return null == playerData.GetInstance().worldMap || playerData.GetInstance().worldMap.Count <= 0;
    }
    bool isShow = false;
    public override void ReceiveData(uint messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            case MessageID.pve_worldmap_list_ret:
                Dictionary<string, object> newpacket1 = new Dictionary<string, object>();
                newpacket1.Add("arg1", playerData.GetInstance().worldMap);
                newpacket1.Add("arg2", 1);
                Singleton<Notification>.Instance.Send(MessageID.pve_dungeon_list_req, newpacket1, C2SMessageType.ActiveWait);
                Dictionary<string, object> newpacket2 = new Dictionary<string, object>();
                newpacket2.Add("arg1", playerData.GetInstance().worldMap);
                newpacket2.Add("arg2", 2);
                Singleton<Notification>.Instance.Send(MessageID.pve_dungeon_list_req, newpacket2, C2SMessageType.ActiveWait);
                break;
            case MessageID.pve_dungeon_list_ret:
                if (isShow)
                {
                    Show();
                    isShow = false;
                }
                else
                {
                    isShow = true;
                }
                break;
        }
    }
    protected override void Init()
    {
        backObj = transform.Find("Back").gameObject;
        goods = transform.Find("Goods").GetComponent<UISprite>();
        iconS = transform.Find("Icon").GetComponent<UISprite>();
        icon = transform.Find("Icon").GetComponent<GUISingleSprite>();
        count = transform.Find("Count").GetComponent<UILabel>();
        goodsName = transform.Find("GoodsName").GetComponent<UILabel>();
        des = transform.Find("Des").GetComponent<UILabel>();
        debris = transform.Find("Debris");
        scrollView = transform.Find("ScrollView").GetComponent<UIScrollView>();
        multList = transform.Find("ScrollView/MultList").GetComponent<GUISingleMultList>();
        UIEventListener.Get(backObj).onClick += OnCloseClick;
        closeBtn.onClick = OnBackClick;
    }

    private void OnBackClick()
    {
        Control.HideGUI(this.GetUIKey());
        //Hide();
    }

    private void OnCloseClick(GameObject go)
    {
        Control.HideGUI(this.GetUIKey());
        //Hide();
    }

    protected override void ShowHandler()
    {
        ShowItem();
    }
    public void SetData(ItemData data)
    {
        itemdata = data;
    }
    public void SetID(long id)
    {
        itemID = id;
    }
    private void ShowItem()
    {
        ItemNodeState itemnodestate = null;
        if (GameLibrary.Instance().ItemStateList.ContainsKey(itemID))
        {
            itemnodestate = GameLibrary.Instance().ItemStateList[itemID];
            if (itemnodestate.types == 6|| itemnodestate.types == 3)
            {
                debris.gameObject.SetActive(true); 
            }
            else
            {
                debris.gameObject.SetActive(false);
                icon.uiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
            }
            if (itemnodestate.types == 3)
            {
                icon.uiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                debris.GetComponent<UISprite>().spriteName = "materialdebris";
                debris.GetComponent<UISprite>().MakePixelPerfect();
            }
            else if (itemnodestate.types == 6)
            {
                icon.uiAtlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
                debris.GetComponent<UISprite>().spriteName = "linghunshi";
                debris.GetComponent<UISprite>().MakePixelPerfect();
            }
            iconS.spriteName = itemnodestate.icon_name;
            //count.text = itemnodestate + "";
            switch (itemnodestate.grade)
            {
                case 1:
                    gradeType = GradeType.Gray;
                    break;
                case 2:
                    gradeType = GradeType.Green;
                    break;
                case 4:
                    gradeType = GradeType.Blue;
                    break;
                case 7:
                    gradeType = GradeType.Purple;
                    break;
                case 11:
                    gradeType = GradeType.Orange;
                    break;
                case 16:
                    gradeType = GradeType.Red;
                    break;
            }
            goods.spriteName = ItemData.GetFrameByGradeType(gradeType);
            goodsName.text = GoodsDataOperation.GetInstance().JointNameColour(itemnodestate.name, gradeType);
            if (GameLibrary.Instance().ItemStateList.ContainsKey(itemID))
            {
                //  [副本id,副本id], 0：暂不能通过副本获得；1：通过副本和活动获得；2：通过合成获得；3：通过祭坛获得；4：角斗场商店；5：竞技场商店；6：公会商店；
                if (GameLibrary.Instance().ItemStateList[itemID].drop_fb== null)
                {
                    scrollView.gameObject.SetActive(false);
                    des.gameObject.SetActive(true);
                    des.text = getWayDes(0);
                    return;
                }
                if (GameLibrary.Instance().ItemStateList[itemID].drop_fb.Length > 0 && GameLibrary.Instance().ItemStateList[itemID].drop_fb[0] <= 2)
                {
                    scrollView.gameObject.SetActive(false);
                    des.gameObject.SetActive(true);
                    des.text = getWayDes(GameLibrary.Instance().ItemStateList[itemID].drop_fb[0]);
                }
                else
                {
                    des.gameObject.SetActive(false);
                    scrollView.gameObject.SetActive(true);
                    fubenIDList.Clear();
                    if (GameLibrary.Instance().ItemStateList[itemID].drop_fb.Length > 0)
                    {
                        for (int i = 0; i < GameLibrary.Instance().ItemStateList[itemID].drop_fb.Length; i++)
                        {
                            if (FSDataNodeTable<SceneNode>.GetSingleton().DataNodeList.ContainsKey(GameLibrary.Instance().ItemStateList[itemID].drop_fb[i]))
                            {
                                //要判断版本中 是否开启该关卡
                                if (FSDataNodeTable<SceneNode>.GetSingleton().DataNodeList[GameLibrary.Instance().ItemStateList[itemID].drop_fb[i]].released == 1)
                                {
                                    fubenIDList.Add(GameLibrary.Instance().ItemStateList[itemID].drop_fb[i].ToString());
                                }

                            }
                            else if(GameLibrary.Instance().ItemStateList[itemID].drop_fb[i]>=3&& GameLibrary.Instance().ItemStateList[itemID].drop_fb[i]<=6)//祭坛不用判断
                            {
                                fubenIDList.Add(GameLibrary.Instance().ItemStateList[itemID].drop_fb[i].ToString());
                            }
                                
                        }
                        multList.InSize(fubenIDList.Count, 1);
                        multList.Info(fubenIDList.ToArray());
                        scrollView.ResetPosition();
                    }
                }
            }
        }
    }

    string getWayDes(int dropfb)
    {
        string str = "";
        switch (dropfb)
        {
            case 0:
                str = "该道具尚不明确获取途径！";
                break;
            case 1:
                str = "通过副本和活动获得";
                break;
            case 2:
                str = "通过合成获得";
                break;
            case 3:
                str = "通过祭坛获得";
                break;
        }
        return str;
    }
}
