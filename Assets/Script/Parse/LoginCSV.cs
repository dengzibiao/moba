using System.Collections.Generic;

public class LoginCSV : CSVParse
{

	private static LoginCSV instance;
	public static LoginCSV Instance()
	{
		 if (instance == null) instance = new LoginCSV();
		 return instance;
	}

	private Dictionary<long, LoginVO> dic = new Dictionary<long, LoginVO>();

	private LoginVO outVO;

	private object[] obj;

	protected override void Parse(string data)
	{
		base.Parse(data);

		string[] textColumn = data.Split("\n".ToCharArray());

		obj = new object[textColumn.Length];

		for (int i = 1; i < textColumn.Length; i++)
		{

			string[] textRow = textColumn[i].Split(" ".ToCharArray());

			LoginVO vo = new LoginVO();

			for (int j = 0; j < textRow.Length; j++)
			{

				vo.card_id = int.Parse(textRow[0]);

				vo.card_atlas = textRow[1];

				vo.card_name = textRow[2];

			}

			obj[i] = vo;

			dic.Add(vo.card_id, vo);

		}

	}

	public LoginVO GetVO(int id)
	{

		dic.TryGetValue(id, out outVO);

		return outVO;

	}

	public object[] GetVoList()
	{
		return obj;
	}

}

public struct LoginVO
{

	public long card_id;

	public string card_atlas;

	public string card_name;

}