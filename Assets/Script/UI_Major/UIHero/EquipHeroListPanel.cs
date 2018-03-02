using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EquipHeroListPanel : GUIBase {

    // Use this for initialization
    //void Start () {

    //}
    public GUISingleMultList allofMultList;             //所有英雄
   // public object[] obj;                                //所有英雄
    Dictionary<long, object> heroList;                  //所有英雄字典

    List<long> count = new List<long>();                //所有英雄的键值


    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    protected override void Init()
    {
        base.Init();
    }
    protected override void ShowHandler()
    {
        InitListData();
    }
    public void InitListData()
    {
        //全部已拥有英雄的字典
        heroList = new Dictionary<long, object>();
        int index = 0;
        object[] objs = new object[playerData.GetInstance().herodataList.Count];
        object obje = null;
        //已经拥有的英雄
        for (int i = 0; i < playerData.GetInstance().herodataList.Count; i++)
        {

            if (heroList.TryGetValue(playerData.GetInstance().herodataList[i].id, out obje))
            {
                objs[index] = obje;
                count.Remove(playerData.GetInstance().herodataList[i].id);
                index++;
                heroList.Remove(playerData.GetInstance().herodataList[i].id);
            }
        }
        //进行显示
        InitHeroList(objs);
    
    }

    /// <summary>
    /// 英雄列表数据显示
    /// </summary>
    /// <param name="allHero"></param>
    void InitHeroList(object[] allHero)
    {
       
       

        //全部英雄
        allofMultList.InSize(allHero.Length, 5);
        allofMultList.Info(allHero);

    }
    // Update is called once per frame
    void Update () {
	
	}
}
