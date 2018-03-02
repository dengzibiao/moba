﻿/*
 *
 *  作者：尛 陽
 *  版本：
 *  时间：
 *  主要功能：
 * 
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 道具管理
/// </summary>
public class GoodsDataManager 
{
    private static GoodsDataManager instance;

    public static GoodsDataManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GoodsDataManager();

            }
            return instance;
        }
    }

    private List<GoodsData> lisGoodsData = new List<GoodsData>();

    public GoodsDataManager ()
    {
        for(int i = 0; i < 6; i++)
        {
            GoodsData data = new GoodsData();
            data.Name = "5012经验药膏";
            data.Count = 100 + i;
            data.ExpNumber = 100 + i * 100;
            lisGoodsData.Add(data);
        }
    }

    public List<GoodsData> GetGoodsList()
    {
        return lisGoodsData;
    }

}


public class GoodsData
{
    public string Name      { get; set; }
    public int Count        { get; set; }
    public int ExpNumber    { get; set; }
}


/// <summary>
/// 英雄信息
/// </summary>
public class HeroDataManager
{
    private static HeroDataManager instance;

    public static HeroDataManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new HeroDataManager();

            }
            return instance;
        }
    }

    private Dictionary<int, HeroData1> DicPlayerData = new Dictionary<int, HeroData1>();

    public HeroDataManager ()
    {

    }

    /// <summary>
    /// 增加英雄信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    public void AddHeroData ( int id, HeroData1 data )
    {
        if( !DicPlayerData.ContainsKey(id) )
        {
            DicPlayerData.Add(id,data);
        }
        else
        {
            DicPlayerData[id] = data;
        }
    }


    /// <summary>
    /// 移除英雄数据
    /// </summary>
    /// <param name="id"></param>
    public void RemoveHeroData ( int id )
    {
        if(DicPlayerData.ContainsKey(id))
        {
            DicPlayerData.Remove(id);
        }
    }
}


public class HeroData1
{
    public string Name { get; set; }
    public string SprName { get; set; }
    public int Lv { get; set; }
    public int ExpMax { get; set; }
    public int ExpNow { get; set; }
}