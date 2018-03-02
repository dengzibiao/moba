using System.Collections.Generic;
using Tianyu;

public class HeroNode : FSDataNodeBase
{
    public long hero_id;        // 英雄ID
    public int types;           //英雄类型 1：英雄，2:小怪，3：普通怪物，4：精英，5：BOSS，6：建筑

    public string name;         //英雄名字
    public string describe;     //英雄描述
    public int[] dingwei;       //定位
    public string info;         //英雄信息
    public string icon_atlas;   //头像资源路径
    public string icon_name;    //头像名称
    public int[] mount_types;   //可骑乘类型
    // public string icon_name_y;

    public int model;                //模型
    public string original_painting;    //原画
    public int[] skill_order;           //出手顺序
    public int attribute;               //角色属性 1:力量，2：智力，3：敏捷
    public int released;                //当前版本是否开放 0 : No，1：Yes
    public int is_icon;
    public int sex;                     // 性别 1：男，2：女，
    public int init_star;               //初始星级
    public int[] characteristic;

    // 1-5星成长系数：[力量, 智力, 敏捷]
    public float[] rate1;
    public float[] rate2;
    public float[] rate3;
    public float[] rate4;
    public float[] rate5;

    public int soul_gem;             //灵魂石id
    public long[] skill_id;      //技能id
    public bool isHas;
    public int dlgAmount;

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        hero_id = long.Parse(item["hero_id"].ToString());
        types = int.Parse(item["types"].ToString());
        name = item["name"].ToString();
        describe = item["describe"].ToString();
        dingwei = item["describe"] as int[];
        info = item["info"].ToString();
        icon_atlas = item["icon_atlas"].ToString();
        icon_name = item["icon_name"].ToString();
        // icon_name_y = item["icon_name_y"].ToString();
        model = int.Parse(item["model"].ToString());
        original_painting = item["original_painting"].ToString();

        skill_order = item["skill_order"] as int[];
        mount_types = item["mount_types"] as int[];

        attribute = int.Parse(item["attribute"].ToString());
        released = int.Parse(item["released"].ToString());
        is_icon = int.Parse(item["is_icon"].ToString());
        sex = int.Parse(item["sex"].ToString());
        init_star = int.Parse(item["init_star"].ToString());

        if (item.ContainsKey("characteristic"))
            characteristic = item["characteristic"] as int[];

        rate1 = FSDataNodeTable<HeroNode>.GetSingleton().ParseToFloatArray(item["rate1"]);
        rate2 = FSDataNodeTable<HeroNode>.GetSingleton().ParseToFloatArray(item["rate2"]);
        rate3 = FSDataNodeTable<HeroNode>.GetSingleton().ParseToFloatArray(item["rate3"]);
        rate4 = FSDataNodeTable<HeroNode>.GetSingleton().ParseToFloatArray(item["rate4"]);
        rate5 = FSDataNodeTable<HeroNode>.GetSingleton().ParseToFloatArray(item["rate5"]);

        soul_gem = int.Parse(item["soul_gem"].ToString());
        // skill_id = (List<long>)item["skill_id"];
        int[] nodeIntarr = (int[])item["skill_id"];
        // nodelist = (int[])item["skill_id"];
        skill_id = new long[nodeIntarr.Length];

        for (int m = 0; m < nodeIntarr.Length; m++)
        {
            skill_id[m] = nodeIntarr[m];
        }
        dlgAmount = int.Parse(item["dlgAmount"].ToString());


    }
    /// <summary>
    /// 获取星级成长系数
    /// </summary>
    /// <param name="attrIndex">属性索引【力量，智力，敏捷】</param>
    /// <param name="star">星级</param>
    /// <returns></returns>
    public float GetStarGrowUpRate(int attrIndex, int star)
    {
        switch (star)
        {
            case 1:
                return (float)rate1[attrIndex];
            case 2:
                return (float)rate2[attrIndex];
            case 3:
                return (float)rate3[attrIndex];
            case 4:
                return (float)rate4[attrIndex];
            case 5:
                return (float)rate5[attrIndex];
            default:
                return 0f;
        }
    }
}

