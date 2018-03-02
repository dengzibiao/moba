using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// VO管理类
/// </summary>
public class VOManager
{

    private static VOManager instance;
    public static VOManager Instance ()
    {
        if ( instance == null ) instance = new VOManager();
        return instance;
    }

    public Dictionary<string , CSVParse> dic = new Dictionary<string , CSVParse>();

    /// <summary>
    /// 初始化字典
    /// </summary>
    public void Init ()
    {

        dic.Add( "RoleData" , new RoleDataCSV() );

        dic.Add( "HeroTable" , new HeroTableCSV() );

        dic.Add( "Login" , new LoginCSV() );

        dic.Add( "Skill" , new SkillCSV() );

        dic.Add("Item",new ItemCSV());

        dic.Add("Shop", new ShopCSV());

        dic.Add("StarUpgrade", new StarUpgradeCSV());

        dic.Add("HeroData", new HeroDataCSV());

        dic.Add("PlayerUpgrade", new PlayerUpgradeCSV());

        dic.Add("EquipUpgrade", new EquipUpgradeCSV());

        dic.Add("HeroUpgrade", new HeroUpgradeCSV());
        dic.Add("ResetLater",new ResetLaterCSV());
        dic.Add("GoldenHand", new GoldenHandCSV());
    }

    /// <summary>
    /// 读取需要更新装备名字的文本
    /// </summary>
    public void LoadTxt ()
    {

        TextAsset txt = Resources.Load( "Csv/TxT" ) as TextAsset;

        string [] txts = txt.text.Split( ",".ToCharArray() );



        for ( int i = 0 ; i < txts.Length ; i++ )
        {

            CSVParse v;

            if(dic.TryGetValue(txts[i], out v))
            {
                v.ParseCSV( txts [ i ] );
            } else
            {
                Debug.LogError("cant find CSV file " + txts[i]);
            }

        }

    }

    /// <summary>
    /// 获取表
    /// </summary>
    /// <typeparam name="T">表的类型</typeparam>
    /// <param name="s">表的名字</param>
    /// <returns>返回类型</returns>
    public T GetCSV<T> ( string s ) where T : CSVParse
    {
        CSVParse v;
        dic.TryGetValue( s , out v );
        return v as T;
    }

}