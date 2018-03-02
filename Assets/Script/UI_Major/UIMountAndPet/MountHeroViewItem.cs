using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class MountHeroViewItem : GUISingleItemList
{
    public UISprite Icon;
    public UISprite Frame;
    public static List<long> herolist;
    public static int cishu = 0;
    private long heroID;
    /// <summary>
    /// 单例
    /// </summary>
    private static MountHeroViewItem mSingleton;
    public static MountHeroViewItem Instance()
    {
        if (mSingleton == null)
            mSingleton = new MountHeroViewItem();
        return mSingleton;
    }
    protected override void InitItem()
    {

    }
    protected override void ShowHandler()
    {
        SetHerolistdata();
    }
    void SetHerolistdata()
    {

    }
    public override void Info(object obj)
    {
        if (obj!=null)
        {
            heroID = long.Parse(obj.ToString());
            if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(heroID))
            {
                Icon.spriteName = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroID].icon_name;
                for (int i = 0; i < playerData.GetInstance().herodataList.Count; i++)
                {
                    if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroID].hero_id == playerData.GetInstance().herodataList[i].id)
                    {
                        Icon.color = new Color(1, 1, 1);
                        break;
                    }
                }
            }
            
        }
        //if (cishu < herolist.Count)
        //{
        //    if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(herolist[cishu]))
        //    {
        //        Icon.spriteName = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[herolist[cishu]].icon_name;
        //    }
            
        //}
        //cishu += 1;
    }
    public void refreshUI(List<long> list)
    {
        herolist = new List<long>();

        herolist = list;
    }
}
