using System.Collections.Generic;

public class BackpackEquipCSV : CSVParse
{

    private static BackpackEquipCSV instance;
    public static BackpackEquipCSV Instance()
    {
        if (instance == null) instance = new BackpackEquipCSV();
        return instance;
    }

    private Dictionary<int, BackpackEquipVO> dic = new Dictionary<int, BackpackEquipVO>();

    private BackpackEquipVO outVO;

    private object[] obj;

    protected override void Parse(string data)
    {
        base.Parse(data);

        string[] textColumn = data.Split("\n".ToCharArray());

        obj = new object[textColumn.Length];

        for (int i = 0; i < textColumn.Length; i++)
        {

            string[] textRow = textColumn[i].Split(" ".ToCharArray());

            BackpackEquipVO vo = new BackpackEquipVO();

            for (int j = 0; j < textRow.Length; j++)
            {

                vo.id = int.Parse(textRow[0]);

                vo.name = textRow[1];

                vo.iconid = int.Parse(textRow[2]);

                vo.conut = int.Parse(textRow[3]);

                vo.qualitytype = int.Parse(textRow[4]);

                vo.proptype = int.Parse(textRow[5]);

                vo.saleprice = int.Parse(textRow[6]);

                vo.ceiling = int.Parse(textRow[7]);

                vo.itemdescription = textRow[8];

                vo.enchantenergy = int.Parse(textRow[9]);

            }

            obj[i] = vo;


            dic.Add(vo.id, vo);

        }

    }

    public BackpackEquipVO GetVO(int id)
    {

        dic.TryGetValue(id, out outVO);

        return outVO;

    }

    public object[] GetVoList()
    {
        return obj;
    }

}

public struct BackpackEquipVO
{

    public int id;

    public string name;

    public int iconid;

    public int conut;

    /// <summary>
    /// 品质类型
    /// </summary>
	public int qualitytype;

    /// <summary>
    /// 类型 1.基础装备 2.合成装备 3.消耗品 4.灵魂石 5.卷轴
    /// </summary>
	public int proptype;

    /// <summary>
    /// 出售价格
    /// </summary>
	public int saleprice;

    /// <summary>
    /// 最大堆叠
    /// </summary>
	public int ceiling;

    /// <summary>
    /// 装备介绍
    /// </summary>
	public string itemdescription;

    /// <summary>
    /// 附魔点数
    /// </summary>
	public int enchantenergy;

}