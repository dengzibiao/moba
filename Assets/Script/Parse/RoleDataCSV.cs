using System.Collections.Generic;

public class RoleDataCSV : CSVParse
{

	private static RoleDataCSV instance;
	public static RoleDataCSV Instance()
	{
		 if (instance == null) instance = new RoleDataCSV();
		 return instance;
	}

	private Dictionary<int, RoleDataVO> dic = new Dictionary<int, RoleDataVO>();

	private RoleDataVO outVO;

	protected override void Parse(string data)
	{
		base.Parse(data);

		string[] textColumn = data.Split("\n".ToCharArray());

		for (int i = 1; i < textColumn.Length; i++)
		{

			string[] textRow = textColumn[i].Split(" ".ToCharArray());

			RoleDataVO vo = new RoleDataVO();

			for (int j = 0; j < textRow.Length; j++)
			{

				vo.id = int.Parse(textRow[0]);

				vo.name = textRow[1];

				vo.combateffectiveness = int.Parse(textRow[2]);

				vo.iconid = int.Parse(textRow[3]);

				vo.level = int.Parse(textRow[4]);

				vo.experience = int.Parse(textRow[5]);

				vo.gold = int.Parse(textRow[6]);

				vo.diamond = int.Parse(textRow[7]);

				vo.vit = int.Parse(textRow[8]);

			}

			dic.Add(vo.id, vo);

		}

	}

	public RoleDataVO GetVO(int id)
	{

		dic.TryGetValue(id, out outVO);

		return outVO;

	}

}

public struct RoleDataVO
{

	public int id;

	public string name;

	public int combateffectiveness;

	public int iconid;

	public int level;

	public int experience;

	public int gold;

	public int diamond;

	public int vit;

}