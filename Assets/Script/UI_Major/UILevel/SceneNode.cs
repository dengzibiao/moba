using System;
using System.Collections.Generic;
using Tianyu;

public class SceneNode : FSDataNodeBase
{
    public int SceneId;
    public string SceneName;
    public string MapName;
    public string Config;
    public string Info;
    public int Type;
    public int game_type;
    public int bigmap_id;
    public int released;
    public int exp;
    public int power_cost;
    public int pass_cost;
    public int drop_double;
    public float time_limit;
    public int pass_lv;
    public int fighting_capacity;
    public long boss;
    public float model_size;
    public string icon_name;
    public object[] drop;
    public int gold;
    public int diamond;
    public int elite_id;
    public double[] treasure_box;
    public string star_describe1;
    public double[] star_parameter1;
    public string star_describe2;
    public double[] star_parameter2;
    public string star_describe3;
    public double[] star_parameter3;
    public bool isOpened;

    public string BossName;
    public string BossImg;

    public int AdvisePower;
    public int CostVigor;
    public int NeedLevel;
    public int MaxBattleTimes;

    public int StarsGot;
    public int TokenNum;
    public int RemainTimes;

    public float flat_angle;

    public string animationName;//boss动画名称
    public int needAnimation;//是否需要bos动画,0:不需要;1：需要;2:特殊相机动画


    public List<string> star_describe = new List<string>();

    public override void parseJson(object jd)
    {
        Dictionary<string, object> items = (Dictionary<string, object>)jd;
        SceneId = Convert.ToInt32(items["map_id"]);
        SceneName = Convert.ToString(items["name"]);
        MapName = Convert.ToString(items["map_name"]);
        Config = Convert.ToString(items["config"]);
        Info = Convert.ToString(items["info"]);
        Type = Convert.ToInt32(items["type"]);
        game_type = Convert.ToInt32(items["game_type"]);
        bigmap_id = Convert.ToInt32(items["bigmap_id"]);
        released = Convert.ToInt32(items["released"]);
        exp = Convert.ToInt32(items["exp"]);
        power_cost = Convert.ToInt32(items["power_cost"]);
        pass_cost = Convert.ToInt32(items["pass_cost"]);
        drop_double = Convert.ToInt32(items["drop_double"]);
        time_limit = Convert.ToInt32(items["time_limit"]);
        pass_lv = Convert.ToInt32(items["pass_lv"]);
        fighting_capacity = Convert.ToInt32(items["fighting_capacity"]);
        boss = long.Parse(items["boss_show"].ToString());
        model_size = float.Parse(items["model_size"].ToString());
        if (items.ContainsKey("icon_name"))
            icon_name = items["icon_name"].ToString();

        if (null != items["drop"])
        {
            if (items["drop"] is int[][])
            {
                object[] node = items["drop"] as object[];
                drop = new object[node.Length];
                for (int i = 0; i < node.Length; i++)
                {
                    drop[i] = node[i];
                }
            }
            else
            {
                drop = new object[1];
                drop[0] = items["drop"] as object;
            }
        }

        gold = Convert.ToInt32(items["gold"]);
        diamond = Convert.ToInt32(items["diamond"]);
        elite_id = Convert.ToInt32(items["elite_id"]);


        if (items["treasure_box"] is int[])
        {
            int[] node = (int[])items["treasure_box"];
            treasure_box = new double[node.Length];
            for (int i = 0; i < node.Length; i++)
            {
                treasure_box[i] = node[i];
            }
        }

        star_describe1 = Convert.ToString(items["1star_describe"]);
        star_describe.Add(star_describe1);

        if (items["1star_parameter"] is int[])
        {
            int[] node = (int[])items["1star_parameter"];
            star_parameter1 = new double[node.Length];
            for (int i = 0; i < node.Length; i++)
            {
                star_parameter1[i] = node[i];
            }
        }

        star_describe2 = Convert.ToString(items["2star_describe"]);
        star_describe.Add(star_describe2);

        if (items["2star_parameter"] is int[])
        {
            int[] node = (int[])items["2star_parameter"];
            star_parameter2 = new double[node.Length];
            for (int i = 0; i < node.Length; i++)
            {
                star_parameter2[i] = node[i];
            }
        }

        star_describe3 = Convert.ToString(items["3star_describe"]);
        star_describe.Add(star_describe3);

        if (items["3star_parameter"] is int[])
        {
            int[] node = (int[])items["3star_parameter"];
            star_parameter3 = new double[node.Length];
            for (int i = 0; i < node.Length; i++)
            {
                star_parameter3[i] = node[i];
            }
        }

        if (items.ContainsKey("flat_angle"))
            flat_angle = float.Parse(items["flat_angle"].ToString());

        animationName = items["animation_name"].ToString();
        needAnimation = int.Parse(items["boss_animation"].ToString());
    }
}