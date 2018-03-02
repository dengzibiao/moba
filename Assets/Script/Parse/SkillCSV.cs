using System.Collections.Generic;
using UnityEngine;

public class SkillCSV : CSVParse
{

	private static SkillCSV instance;
	public static SkillCSV Instance()
	{
		 if (instance == null) instance = new SkillCSV();
		 return instance;
	}

	private Dictionary<int, SkillVO> dic = new Dictionary<int, SkillVO>();

	private SkillVO outVO;

	private object[] obj;

	protected override void Parse(string data)
	{
		base.Parse(data);

		string[] textColumn = data.Split("\n".ToCharArray());

		obj = new object[textColumn.Length];

		for (int i = 1; i < textColumn.Length; i++)
		{

			string[] textRow = textColumn[i].Split(" ".ToCharArray());

			SkillVO vo = new SkillVO();

			for (int j = 0; j < textRow.Length; j++)
			{

				vo.skill_id = int.Parse(textRow[0]);
                
                vo.name = textRow[1];

				vo.hero_id = int.Parse(textRow[2]);

				vo.skill_name = textRow[3];

				vo.des = textRow[4];

				vo.skill_icon = textRow[5];

				vo.skill_motion = textRow[6];

                vo.hit = int.Parse(textRow[7]);

				vo.buff = int.Parse(textRow[8]);

				vo.weapon = int.Parse(textRow[9]);

				vo.sound = textRow[10];

				vo.hit_sound = textRow[11];

				vo.types = int.Parse(textRow[12]);

				vo.dist = float.Parse(textRow[13]);

				vo.aoe = int.Parse(textRow[14]);

				vo.aoe_type = int.Parse(textRow[15]);

				vo.aoe_long = float.Parse(textRow[16]);

				vo.aoe_wide = int.Parse(textRow[17]);

				vo.angle = int.Parse(textRow[18]);

				vo.reading = int.Parse(textRow[19]);

				vo.shake = int.Parse(textRow[20]);

				vo.shake_size = textRow[21];

				vo.cass1 = int.Parse(textRow[22]);

				vo.cass1v = textRow[23];

				vo.cass2 = int.Parse(textRow[24]);

				vo.cass2v =textRow[25];

				vo.cass3 = int.Parse(textRow[26]);

				vo.cass3v = textRow[27];

				vo.cass4 = int.Parse(textRow[28]);

				vo.cass4v = int.Parse(textRow[29]);

				vo.cass5 = int.Parse(textRow[30]);

				vo.cass5v = int.Parse(textRow[31]);

				vo.cooling = int.Parse(textRow[32]);

				vo.site = int.Parse(textRow[33]);

				vo.energy = int.Parse(textRow[34]);

				vo.use_type = int.Parse(textRow[35]);

				vo.skill_value = int.Parse(textRow[36]);

				vo.coefficient_add = int.Parse(textRow[37]);

				vo.coefficient_da = float.Parse(textRow[38]);

				vo.amend = int.Parse(textRow[39]);

				vo.or_break = int.Parse(textRow[40]);

				vo.use_limit = int.Parse(textRow[41]);

				vo.nullity_type = int.Parse(textRow[42]);

				vo.hp_limit = int.Parse(textRow[43]);

			}

			obj[i] = vo;


            dic.Add(vo.skill_id, vo);

		}

	}

	public SkillVO GetVO(int id)
	{

		dic.TryGetValue(id, out outVO);

		return outVO;

	}

	public object[] GetVoList()
	{
		return obj;
	}

}

public struct SkillVO
{

	public int skill_id;

	public string name;

	public int hero_id;

	public string skill_name;

	public string des;

	public string skill_icon;

	public string skill_motion;

	public int hit;

	public int buff;

	public int weapon;

	public string sound;

	public string hit_sound;

	public int types;

	public float dist;

	public int aoe;

	public int aoe_type;

	public float aoe_long;

	public int aoe_wide;

	public int angle;

	public int reading;

	public int shake;

	public string shake_size;

	public int cass1;

	public string cass1v;

	public int cass2;

	public string cass2v;

	public int cass3;

	public string cass3v;

	public int cass4;

	public int cass4v;

	public int cass5;

	public int cass5v;

	public int cooling;

	public int site;

	public int energy;

	public int use_type;

	public int skill_value;

	public int coefficient_add;

	public float coefficient_da;

	public int amend;

	public int or_break;

	public int use_limit;

	public int nullity_type;

	public int hp_limit;

}