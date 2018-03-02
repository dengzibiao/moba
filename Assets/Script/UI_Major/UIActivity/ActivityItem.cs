using UnityEngine;
using System.Collections;
using System.Text;
using Tianyu;

public class ActivityItem : GUIBase
{


    //public delegate void OnActivityItemClick(int mapID);
    //public OnActivityItemClick OnItemClick;

    public UILabel NameLabel;
    public UILabel DesLabel;
    public UISprite Icon;
    public Transform redPoint;
    private MapNode mapNode;//记录当前的map信息

    StringBuilder stringB = new StringBuilder();

    public void RefreshUI(MapNode map)
    {
        mapNode = map;
        NameLabel.text = "" + map.MapName;

        stringB.Append("每周");

        for (int i = 0; i < map.is_opened.Length; i++)
        {
            if (map.is_opened[i] == 1)
            {
                stringB.Append(IsOpened(i));

                if (i != map.is_opened.Length - 1)
                    stringB.Append("、");
            }
        }

        stringB.Append("\n" + "开启");

        DesLabel.text = stringB.ToString();
        stringB.Length = 0;
        Icon.spriteName = UIActivity.instance.ModulIcon(map.MapId);
    }

    string IsOpened(int id)
    {
        switch (id)
        {
            case 0: return "一";
            case 1: return "二";
            case 2: return "三";
            case 3: return "四";
            case 4: return "五";
            case 5: return "六";
            default: return "日";
        }
    }

    public void SetBtnState(bool isOpen, int count)
    {
        if (isOpen)
        {
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<UISprite>().color = new Color(1, 1, 1);
            Icon.color = new Color(1, 1, 1);
            DesLabel.text = "剩余次数：" + count + "次";
            //红点的添加和移除
            if (count > 0)
            {
                if (mapNode != null && mapNode.ordinary.Length > 0)
                {
                    if (FSDataNodeTable<SceneNode>.GetSingleton().DataNodeList.ContainsKey(mapNode.ordinary[0]))
                    {
                        if (FSDataNodeTable<SceneNode>.GetSingleton().DataNodeList[mapNode.ordinary[0]].pass_lv <= playerData.GetInstance().selfData.level)
                        {
                            //redPoint.gameObject.SetActive(true);
                            if (NameLabel != null)
                                OperateRedFlag(NameLabel.text, true);
                        }
                        else
                        {
                            if (NameLabel != null)
                                OperateRedFlag(NameLabel.text, false);
                        }
                    }
                }

            }
            else
            {
                //redPoint.gameObject.SetActive(false);
                if (NameLabel != null)
                    OperateRedFlag(NameLabel.text, false);
            }
        }
        else
        {
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<UISprite>().color = new Color(0, 0, 0);
            Icon.color = new Color(0, 0, 0);
            //redPoint.gameObject.SetActive(false);
            if (NameLabel != null)
                OperateRedFlag(NameLabel.text, false);
        }
    }
    private void OperateRedFlag(string name, bool isAdd)
    {
        if (name == "黄金商路")
        {
            if (isAdd)
            {
                Singleton<RedPointManager>.Instance.AddChildFlag(EnumRedPoint.RP_EVENT_DUNGEON, 1);
            }
            else
            {
                Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RP_EVENT_DUNGEON, 1);
            }
        }
        if (name == "药剂商路")
        {
            if (isAdd)
            {
                Singleton<RedPointManager>.Instance.AddChildFlag(EnumRedPoint.RP_EVENT_DUNGEON, 2);
            }
            else
            {
                Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RP_EVENT_DUNGEON, 2);
            }
        }
        if (name == "暮色峡谷")
        {
            if (isAdd)
            {
                Singleton<RedPointManager>.Instance.AddChildFlag(EnumRedPoint.RP_EVENT_DUNGEON, 3);
            }
            else
            {
                Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RP_EVENT_DUNGEON, 3);
            }
        }
        if (name == "冰封雪域")
        {
            if (isAdd)
            {
                Singleton<RedPointManager>.Instance.AddChildFlag(EnumRedPoint.RP_EVENT_DUNGEON, 4);
            }
            else
            {
                Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RP_EVENT_DUNGEON, 4);
            }
        }
        if (name == "沼泽湿地")
        {
            if (isAdd)
            {
                Singleton<RedPointManager>.Instance.AddChildFlag(EnumRedPoint.RP_EVENT_DUNGEON, 5);
            }
            else
            {
                Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RP_EVENT_DUNGEON, 5);
            }
        }
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

}
