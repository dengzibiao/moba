using System.Collections.Generic;

public class HeroUpgradeCSV : CSVParse
{

	private static HeroUpgradeCSV instance;
	public static HeroUpgradeCSV Instance()
	{
		 if (instance == null) instance = new HeroUpgradeCSV();
		 return instance;
	}

	private Dictionary<int, HeroUpgradeVO> dic = new Dictionary<int, HeroUpgradeVO>();

	private HeroUpgradeVO outVO;

	private object[] obj;

	protected override void Parse(string data)
	{
		base.Parse(data);

		string[] textColumn = data.Split("\n".ToCharArray());

		obj = new object[textColumn.Length];

		for (int i = 0; i < textColumn.Length; i++)
		{

			string[] textRow = textColumn[i].Split(" ".ToCharArray());

			HeroUpgradeVO vo = new HeroUpgradeVO();

			for (int j = 0; j < textRow.Length; j++)
			{

				vo.id = int.Parse(textRow[0]);

				vo.exp = int.Parse(textRow[1]);

				vo.skill_limit = int.Parse(textRow[2]);

				vo.skill_ceiling = int.Parse(textRow[3]);

				vo.grade = int.Parse(textRow[4]);

			}

			obj[i] = vo;


			dic.Add(vo.id, vo);

		}

	}

	public HeroUpgradeVO GetVO(int id)
	{

		dic.TryGetValue(id, out outVO);

		return outVO;

	}

	public object[] GetVoList()
	{
		return obj;
	}

}

public struct HeroUpgradeVO
{

	public int id;

	public int exp;

	public int skill_limit;

	public int skill_ceiling;

	public int grade;

}