using System.Collections.Generic;

using UnityEngine;

public class HeroCSV : CSVParse
{

	private static HeroCSV instance;
	public static HeroCSV Instance()
	{
		 if (instance == null) instance = new HeroCSV();
		 return instance;
	}

	private Dictionary<long, HeroVO> dic = new Dictionary<long, HeroVO>();

	private HeroVO outVO;

	private object[] obj;

	protected override void Parse(string data)
	{
		base.Parse(data);

		string[] textColumn = data.Split("\n".ToCharArray());

		obj = new object[textColumn.Length];

		for (int i = 0; i < textColumn.Length; i++)
		{
            string [] textRow = textColumn[i].Split(" ".ToCharArray());

			HeroVO vo = new HeroVO();

			for (int j = 0; j < textRow.Length; j++)
			{
                vo.hero_id = int.Parse(textRow[0]);

				vo.types = int.Parse(textRow[1]);

				vo.name = textRow[2];

				vo.describe = textRow[3];

				vo.info = textRow[4];

				vo.icon_atlas = textRow[5];

				vo.icon_name = textRow[6];

				vo.icon_name_y = textRow[7];

				vo.model = textRow[8];

                vo.original_painting = textRow[9];

                vo.attribute = int.Parse(textRow[10]);

				vo.released = int.Parse(textRow[11]);

				vo.is_icon = int.Parse(textRow[12]);

				vo.sex = int.Parse(textRow[13]);

				vo.init_star = int.Parse(textRow[14]);

				vo.rate1 = textRow[15];

				vo.rate2 = textRow[16];

				vo.rate3 = textRow[17];

				vo.rate4 = textRow[18];

				vo.rate5 = textRow[19];

				vo.soul_gem = int.Parse(textRow[20]);

				vo.skill_id = textRow[21];

			    vo.isHas = false;


			}

			obj[i] = vo;


			dic.Add(vo.hero_id, vo);

		}

	}

	public HeroVO GetVO(long id)
	{

		dic.TryGetValue(id, out outVO);

		return outVO;

	}

	public object[] GetVoList()
	{
		return obj;
	}

}

public struct HeroVO
{

	public long hero_id;

	public int types;

	public string name;

	public string describe;

	public string info;

	public string icon_atlas;

	public string icon_name;

	public string icon_name_y;

	public string model;

	public string original_painting;

	public int attribute;

	public int released;

	public int is_icon;

	public int sex;

	public int init_star;

	public string rate1;

	public string rate2;

	public string rate3;

	public string rate4;

	public string rate5;

	public int soul_gem;

	public string skill_id;

    public bool isHas;

}