using System.Collections.Generic;
using UnityEngine;
public class GoldenHandCSV : CSVParse
{

	private static GoldenHandCSV instance;
	public static GoldenHandCSV Instance()
	{
		 if (instance == null) instance = new GoldenHandCSV();
		 return instance;
	}

	private Dictionary<int, GoldenHandVO> dic = new Dictionary<int, GoldenHandVO>();

	private GoldenHandVO outVO;

	private object[] obj;

	protected override void Parse(string data)
	{
		base.Parse(data);

		string[] textColumn = data.Split("\n".ToCharArray());

		obj = new object[textColumn.Length];

		for (int i = 0; i < textColumn.Length; i++)
		{

			string[] textRow = textColumn[i].Split(" ".ToCharArray());

			GoldenHandVO vo = new GoldenHandVO();

			for (int j = 0; j < textRow.Length; j++)
			{

				vo.id = int.Parse(textRow[0]);

				vo.diamond_cost = int.Parse(textRow[1]);

				vo.basic_rate = float.Parse(textRow[2]);

                vo.common_difference = float.Parse(textRow[3]);

				vo.midas2 = int.Parse(textRow[4]);

				vo.midas5 = int.Parse(textRow[5]);

				vo.midas10 = int.Parse(textRow[6]);

			}

			obj[i] = vo;


			dic.Add(vo.id, vo);

		}

	}

	public GoldenHandVO GetVO(int id)
	{

		dic.TryGetValue(id, out outVO);

		return outVO;

	}

	public object[] GetVoList()
	{
		return obj;
	}

}

public struct GoldenHandVO
{
    /// <summary>
    /// id同时也是点金次数
    /// </summary>
	public int id;
    /// <summary>
    /// 钻石消耗
    /// </summary>
	public int diamond_cost;
    /// <summary>
    /// 基础倍率
    /// </summary>
	public float basic_rate;
    /// <summary>
    /// 递增公差
    /// </summary>
	public float common_difference;

	public int midas2;

	public int midas5;

	public int midas10;

}