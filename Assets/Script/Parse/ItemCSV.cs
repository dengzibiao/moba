using System.Collections.Generic;

public class ItemCSV : CSVParse
{

	private static ItemCSV instance;
	public static ItemCSV Instance()
	{
		 if (instance == null) instance = new ItemCSV();
		 return instance;
	}

	private Dictionary<long, ItemVO> dic = new Dictionary<long, ItemVO>();

	private ItemVO outVO;

	private object[] obj;

	protected override void Parse(string data)
	{
		base.Parse(data);

		string[] textColumn = data.Split("\n".ToCharArray());

		obj = new object[textColumn.Length];

		for (int i = 0; i < textColumn.Length; i++)
		{

			string[] textRow = textColumn[i].Split(" ".ToCharArray());

			ItemVO vo = new ItemVO();

			for (int j = 0; j < textRow.Length; j++)
			{

				vo.props_id = int.Parse(textRow[0]);

				vo.name = textRow[1];

				vo.types = int.Parse(textRow[2]);

				vo.describe = textRow[3];

				vo.grade = int.Parse(textRow[4]);

				vo.next_grade = int.Parse(textRow[5]);

				vo.cprice = textRow[6];

				vo.sprice = int.Parse(textRow[7]);

				vo.power = int.Parse(textRow[8]);

				vo.intelligence = int.Parse(textRow[9]);

				vo.agility = int.Parse(textRow[10]);

				vo.hp = int.Parse(textRow[11]);

				vo.physical_attack = int.Parse(textRow[12]);

				vo.magic_attack = int.Parse(textRow[13]);

				vo.armor = int.Parse(textRow[14]);

				vo.magic_resist = int.Parse(textRow[15]);

				vo.physical_critical = int.Parse(textRow[16]);

				vo.dodge = int.Parse(textRow[17]);

				vo.hit_ratio = int.Parse(textRow[18]);

				vo.armor_penetration = int.Parse(textRow[19]);

				vo.magic_penetration = int.Parse(textRow[20]);

				vo.suck_blood = int.Parse(textRow[21]);

				vo.spell_vamp = int.Parse(textRow[22]);

				vo.tenacity = int.Parse(textRow[23]);

				vo.movement_speed = int.Parse(textRow[24]);

				vo.attack_speed = int.Parse(textRow[25]);

				vo.striking_distance = int.Parse(textRow[26]);

				vo.skill_point = int.Parse(textRow[27]);

				vo.exp_gain = int.Parse(textRow[28]);

				vo.be_equip = textRow[29];

				vo.be_synth = textRow[30];

				vo.syn_condition = textRow[31];

				vo.syn_cost = int.Parse(textRow[32]);

				vo.drop_fb = int.Parse(textRow[33]);

				vo.icon_atlas = textRow[34];

				vo.icon_name = textRow[35];

				vo.released = int.Parse(textRow[36]);

				vo.lv_limit = int.Parse(textRow[37]);

				vo.piles = int.Parse(textRow[38]);

			}

			obj[i] = vo;


			dic.Add(vo.props_id, vo);

		}

	}

	public ItemVO GetVO(long id)
	{

		dic.TryGetValue(id, out outVO);

		return outVO;

	}

	public object[] GetVoList()
	{
		return obj;
	}

}

public struct ItemVO
{

	public long props_id;

	public string name;

	public int types;

	public string describe;

	public int grade;

	public int next_grade;

	public string cprice;

	public int sprice;

	public int power;

	public int intelligence;

	public int agility;

	public int hp;

	public int physical_attack;

	public int magic_attack;

	public int armor;

	public int magic_resist;

	public int physical_critical;

	public int dodge;

	public int hit_ratio;

	public int armor_penetration;

	public int magic_penetration;

	public int suck_blood;

	public int spell_vamp;

	public int tenacity;

	public int movement_speed;

	public int attack_speed;

	public int striking_distance;

	public int skill_point;

	public int exp_gain;

	public string be_equip;

	public string be_synth;

	public string syn_condition;

	public int syn_cost;

	public int drop_fb;

	public string icon_atlas;

	public string icon_name;

	public int released;

	public int lv_limit;

	public int piles;

}