using System.Collections.Generic;

public class StarUpgradeCSV : CSVParse
{

	private static StarUpgradeCSV instance;
	public static StarUpgradeCSV Instance()
	{
		 if (instance == null) instance = new StarUpgradeCSV();
		 return instance;
	}

	private Dictionary<int, StarUpgradeVO> dic = new Dictionary<int, StarUpgradeVO>();

	private StarUpgradeVO outVO;

	private object[] obj;

	protected override void Parse(string data)
	{
		base.Parse(data);

		string[] textColumn = data.Split("\n".ToCharArray());

		obj = new object[textColumn.Length];

        for (int i = 0; i < textColumn.Length; i++)
		{

            string[] textRow = textColumn[i].Split(" ".ToCharArray());

			StarUpgradeVO vo = new StarUpgradeVO();

			for (int j = 0; j < textRow.Length; j++)
			{

                vo.star = int.Parse(textRow[0]);

				vo.call_stone_num = int.Parse(textRow[1]);

				vo.evolve_cost = int.Parse(textRow[2]);

				vo.evolve_stone_num = int.Parse(textRow[3]);

				vo.convert_stone_num = int.Parse(textRow[4]);

			}

			obj[i] = vo;


			dic.Add(vo.star, vo);

		}

	}

	public StarUpgradeVO GetVO(int id)
	{

		dic.TryGetValue(id, out outVO);

		return outVO;

	}

	public object[] GetVoList()
	{
		return obj;
	}

}

public struct StarUpgradeVO
{

	public int star;

	public int call_stone_num;

	public int evolve_cost;

	public int evolve_stone_num;

	public int convert_stone_num;

}