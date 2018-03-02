using UnityEngine;
using System.Collections.Generic;

public class HeroTableCSV : CSVParse
{

    private static HeroTableCSV instance;
    public static HeroTableCSV Instance()
    {
        if (instance == null) instance = new HeroTableCSV();
        return instance;
    }

    private Dictionary<int, HeroTableVO> dic = new Dictionary<int, HeroTableVO>();

    private HeroTableVO outVO;

    private object[] obj;

    protected override void Parse(string data)
    {
        base.Parse(data);

        string[] textColumn = data.Split("\n".ToCharArray());

        obj = new object[textColumn.Length];

        for (int i = 0; i < textColumn.Length; i++)
        {

            string[] textRow = textColumn[i].Split(" ".ToCharArray());

            HeroTableVO vo = new HeroTableVO();

            for (int j = 0; j < textRow.Length; j++)
            {

                vo.id = int.Parse(textRow[0]);

                vo.name = textRow[1];

                vo.icon = textRow[2];

                vo.model = textRow[3];

                vo.image = textRow[4];

                vo.desc = textRow[5];

                vo.main_prop = int.Parse(textRow[6]);

                vo.sex = int.Parse(textRow[7]);

                vo.unlock_action = textRow[8];

                vo.init_star = int.Parse(textRow[9]);

                vo.init_str = float.Parse(textRow[10]);

                vo.init_int = float.Parse(textRow[11]);

                vo.init_agi = float.Parse(textRow[12]);

                vo.init_hp = int.Parse(textRow[13]);

                vo.init_patk = float.Parse(textRow[14]);

                vo.init_matk = float.Parse(textRow[15]);

                vo.init_pdef = float.Parse(textRow[16]);

                vo.init_mdef = float.Parse(textRow[17]);

                vo.init_pcrit = float.Parse(textRow[18]);

                vo.init_mcrit = float.Parse(textRow[19]);

                vo.init_dodge = int.Parse(textRow[20]);

                vo.init_hit = int.Parse(textRow[21]);

                vo.str_rate = float.Parse(textRow[22]);

                vo.str_rate_grow = float.Parse(textRow[23]);

                vo.agi_rate = float.Parse(textRow[24]);

                vo.agi_rate_grow = float.Parse(textRow[25]);

                vo.int_rate = float.Parse(textRow[26]);

                vo.int_rate_grow = float.Parse(textRow[27]);

                vo.stone_id = int.Parse(textRow[28]);

                vo.isHas = false;
            }


            obj[i] = vo;

            dic.Add(vo.id, vo);

        }

    }

    public HeroTableVO GetVO(int id)
    {

        dic.TryGetValue(id, out outVO);

        return outVO;

    }

    public object[] GetVoList()
    {
        return obj;
    }

}

public class HeroTableVO
{

    public int id;

    public string name;

    public string icon;

    public string model;

    public string image;

    public string desc;

    public int main_prop;

    public int sex;

    public string unlock_action;

    public int init_star;

    public float init_str;

    public float init_int;

    public float init_agi;

    public int init_hp;

    public float init_patk;

    public float init_matk;

    public float init_pdef;

    public float init_mdef;

    public float init_pcrit;

    public float init_mcrit;

    public int init_dodge;

    public int init_hit;

    public float str_rate;

    public float str_rate_grow;

    public float agi_rate;

    public float agi_rate_grow;

    public float int_rate;

    public float int_rate_grow;

    public int stone_id;

    public bool isHas;

}