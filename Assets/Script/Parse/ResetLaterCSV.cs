using System.Collections.Generic;

public class ResetLaterCSV : CSVParse
{

	private static ResetLaterCSV instance;
	public static ResetLaterCSV Instance()
	{
		 if (instance == null) instance = new ResetLaterCSV();
		 return instance;
	}

	private Dictionary<int, ResetLaterVO> dic = new Dictionary<int, ResetLaterVO>();

	private ResetLaterVO outVO;

	private object[] obj;

	protected override void Parse(string data)
	{
		base.Parse(data);

		string[] textColumn = data.Split("\n".ToCharArray());

		obj = new object[textColumn.Length];

		for (int i = 0; i < textColumn.Length; i++)
		{

			string[] textRow = textColumn[i].Split(" ".ToCharArray());

			ResetLaterVO vo = new ResetLaterVO();

			for (int j = 0; j < textRow.Length; j++)
			{

				vo.id = int.Parse(textRow[0]);

				vo.power_buy = textRow[1];

				vo.reset_stage = textRow[2];

				vo.reset_arena_cd = int.Parse(textRow[3]);

				vo.buy_vitality = textRow[4];

				vo.buy_skill = textRow[5];

				vo.general_shop = textRow[6];

			}

			obj[i] = vo;


			dic.Add(vo.id, vo);

		}

	}

	public ResetLaterVO GetVO(int id)
	{

		dic.TryGetValue(id, out outVO);

		return outVO;

	}

	public object[] GetVoList()
	{
		return obj;
	}

}

public struct ResetLaterVO
{

	public int id;

	public string power_buy;

	public string reset_stage;

	public int reset_arena_cd;

	public string buy_vitality;

	public string buy_skill;

	public string general_shop;

}