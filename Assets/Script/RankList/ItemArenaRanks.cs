using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;
public class ItemArenaRanks : GUISingleItemList
{
    public List<UISprite> stars=new List<UISprite>();//星星
    public GUISingleLabel levelLab;//等级
    public GUISingleSprite icon;//头像
    public GUISingleSprite iconFrame;//头像框
    public UIGrid grid;
    private HeroNode item = null;
    protected override void InitItem()
    {
    }
    public override void Info(object obj)
    {
        base.Info(obj);
        if (obj == null) return;
            //stars.gameObject.SetActive(true);//暂时不显示
            //levelLab.gameObject.SetActive(true);
            //icon.gameObject.SetActive(true);
            //iconFrame.gameObject.SetActive(true);

            Dictionary<long, HeroNode> mAllIconNode = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList;
            long id1 = ((RankHeroList)obj).iconId1;
            mAllIconNode.TryGetValue(id1, out item);
            if (item != null)
            {
                icon.uiAtlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
                icon.spriteName = item.icon_name;
            }

            levelLab.text = ((RankHeroList)obj).lv.ToString();
            iconFrame.spriteName = GoodsDataOperation.GetInstance().GetSmallHeroGrameByHeroGrade(((RankHeroList)obj).iconIdFrame);

        for(int i=0;i<stars.Count;i++)
        {
            stars[i].gameObject.SetActive(false);
        }
        for(int j=0;j<((RankHeroList)obj).star;j++)
        {
            stars[j].gameObject.SetActive(true);
        }
        grid.Reposition();
    }
}
