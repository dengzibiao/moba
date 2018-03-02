using System.Collections.Generic;

public class PlayerUpgradeCSV : CSVParse
{

	private static PlayerUpgradeCSV instance;
	public static PlayerUpgradeCSV Instance()
	{
		 if (instance == null) instance = new PlayerUpgradeCSV();
		 return instance;
	}

	private Dictionary<int, PlayerUpgradeVO> dic = new Dictionary<int, PlayerUpgradeVO>();

	private PlayerUpgradeVO outVO;

	private object[] obj;

	protected override void Parse(string data)
	{
		base.Parse(data);

		string[] textColumn = data.Split("\n".ToCharArray());

		obj = new object[textColumn.Length];

		for (int i = 1; i < textColumn.Length; i++)
		{

			string[] textRow = textColumn[i].Split(" ".ToCharArray());

			PlayerUpgradeVO vo = new PlayerUpgradeVO();

			for (int j = 0; j < textRow.Length; j++)
			{

				vo.id = int.Parse(textRow[0]);

				vo.exp = int.Parse(textRow[1]);

				vo.hero_lv_limit = int.Parse(textRow[2]);

				vo.power_reward = int.Parse(textRow[3]);

				vo.max_power = int.Parse(textRow[4]);

				vo.max_friend = int.Parse(textRow[5]);

				vo.max_vitality = int.Parse(textRow[6]);

			}

			obj[i] = vo;


			dic.Add(vo.id, vo);

		}

	}

	public PlayerUpgradeVO GetVO(int id)
	{

		dic.TryGetValue(id, out outVO);

		return outVO;

	}

	public object[] GetVoList()
	{
		return obj;
	}

}

public struct PlayerUpgradeVO
{

	public int id;

	public int exp;

	public int hero_lv_limit;

	public int power_reward;

	public int max_power;

	public int max_friend;

	public int max_vitality;

}