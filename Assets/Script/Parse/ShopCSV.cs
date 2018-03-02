using System.Collections.Generic;
using UnityEngine;

public class ShopCSV : CSVParse
{

	private static ShopCSV instance;
	public static ShopCSV Instance()
	{
		 if (instance == null) instance = new ShopCSV();
		 return instance;
	}

	private Dictionary<int, ShopVO> dic = new Dictionary<int, ShopVO>();

	private ShopVO outVO;

	private object[] obj;

	protected override void Parse(string data)
	{
		base.Parse(data);

		string[] textColumn = data.Split("\n".ToCharArray());

		obj = new object[textColumn.Length];

		for (int i = 0; i < textColumn.Length; i++)
		{

			string[] textRow = textColumn[i].Split(" ".ToCharArray());

			ShopVO vo = new ShopVO();

            for (int j = 0; j < textRow.Length; j++)
			{

				vo.id = int.Parse(textRow[0]);
			    
				vo.goods_num = int.Parse(textRow[1]);

				vo.soul_num = textRow[2];

				vo.soul_stone = textRow[3];

				vo.exp_num = textRow[4];

				vo.exp_material = textRow[5];

				vo.prop_num = textRow[6];

				vo.prop = textRow[7];

				vo.prop_piece_num = textRow[8];

				vo.prop_piece = textRow[9];

				vo.diamond_prop_num = textRow[10];

				vo.diamond_prop = textRow[11];

				vo.gold_prop = textRow[12];

				vo.refresh_cost = textRow[13];

				vo.refresh_point = textRow[14];

			}

			obj[i] = vo;


			dic.Add(vo.id, vo);

		}

	}

	public ShopVO GetVO(int id)
	{

		dic.TryGetValue(id, out outVO);

		return outVO;

	}

	public object[] GetVoList()
	{
		return obj;
	}

}

public struct ShopVO
{

	public int id;

	public int goods_num;

	public string soul_num;

	public string soul_stone;

	public string exp_num;

	public string exp_material;

	public string prop_num;

	public string prop;

	public string prop_piece_num;

	public string prop_piece;

	public string diamond_prop_num;

	public string diamond_prop;

	public string gold_prop;

	public string refresh_cost;

	public string refresh_point;

}