using System.Collections.Generic;

public class HeroDataCSV : CSVParse
{

	private static HeroDataCSV instance;
	public static HeroDataCSV Instance()
	{
		 if (instance == null) instance = new HeroDataCSV();
		 return instance;
	}

	private Dictionary<long, HeroDataVO> dic = new Dictionary<long, HeroDataVO>();

	private HeroDataVO outVO;

	private object[] obj;

	protected override void Parse(string data)
	{
		base.Parse(data);

		string[] textColumn = data.Split("\n".ToCharArray());

		obj = new object[textColumn.Length];

		for (int i = 0; i < textColumn.Length; i++)
		{

			string[] textRow = textColumn[i].Split(" ".ToCharArray());

			HeroDataVO vo = new HeroDataVO();

			for (int j = 0; j < textRow.Length; j++)
			{

				vo.heros_id = int.Parse(textRow[0]);

				vo.name = textRow[1];

				vo.hero_id = int.Parse(textRow[2]);

				vo.grade = int.Parse(textRow[3]);

				vo.next_grade = int.Parse(textRow[4]);

				vo.anger = int.Parse(textRow[5]);

				vo.equipment = textRow[6];

				vo.power = float.Parse(textRow[7]);

				vo.intelligence = float.Parse(textRow[8]);

				vo.agility = float.Parse(textRow[9]);

				vo.hp = int.Parse(textRow[10]);

				vo.physical_attack = float.Parse(textRow[11]);

				vo.magic_attack = float.Parse(textRow[12]);

				vo.armor = float.Parse(textRow[13]);

				vo.magic_resist = float.Parse(textRow[14]);

				vo.physical_critical = float.Parse(textRow[15]);

				vo.dodge = int.Parse(textRow[16]);

				vo.hit_ratio = int.Parse(textRow[17]);

				vo.armor_penetration = int.Parse(textRow[18]);

				vo.magic_penetration = int.Parse(textRow[19]);

				vo.suck_blood = int.Parse(textRow[20]);

				vo.spell_vamp = int.Parse(textRow[21]);

				vo.tenacity = int.Parse(textRow[22]);

				vo.movement_speed = int.Parse(textRow[23]);

				vo.attack_speed = int.Parse(textRow[24]);

				vo.striking_distance = int.Parse(textRow[25]);

			}

			obj[i] = vo;


			dic.Add(vo.heros_id, vo);

		}

	}

	public HeroDataVO GetVO(int id)
	{

		dic.TryGetValue(id, out outVO);

		return outVO;

	}

	public object[] GetVoList()
	{
		return obj;
	}

}

public struct HeroDataVO
{

	public long heros_id;

	public string name;

	public int hero_id;

	public int grade;

	public int next_grade;

	public int anger;

	public string equipment;

	public float power;

	public float intelligence;

	public float agility;

	public int hp;

	public float physical_attack;

	public float magic_attack;

	public float armor;

	public float magic_resist;

	public float physical_critical;

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

}