using System.Collections.Generic;

public class EquipUpgradeCSV : CSVParse
{

	private static EquipUpgradeCSV instance;
	public static EquipUpgradeCSV Instance()
	{
		 if (instance == null) instance = new EquipUpgradeCSV();
		 return instance;
	}

	private Dictionary<int, EquipUpgradeVO> dic = new Dictionary<int, EquipUpgradeVO>();

	private EquipUpgradeVO outVO;

	private object[] obj;

	protected override void Parse(string data)
	{
		base.Parse(data);

		string[] textColumn = data.Split("\n".ToCharArray());

		obj = new object[textColumn.Length];

		for (int i = 1; i < textColumn.Length; i++)
		{

			string[] textRow = textColumn[i].Split(" ".ToCharArray());

			EquipUpgradeVO vo = new EquipUpgradeVO();

			for (int j = 0; j < textRow.Length; j++)
			{

				vo.id = int.Parse(textRow[0]);

				vo.consume = int.Parse(textRow[1]);

			}

			obj[i] = vo;


			dic.Add(vo.id, vo);

		}

	}

	public EquipUpgradeVO GetVO(int id)
	{

		dic.TryGetValue(id, out outVO);

		return outVO;

	}

	public object[] GetVoList()
	{
		return obj;
	}

}

public struct EquipUpgradeVO
{

	public int id;

	public int consume;

}