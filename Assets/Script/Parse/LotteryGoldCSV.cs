using System.Collections.Generic;

public class LotteryGoldCSV : CSVParse
{

    private static LotteryGoldCSV instance;
    public static LotteryGoldCSV Instance()
    {
        if (instance == null) instance = new LotteryGoldCSV();
        return instance;
    }

    private Dictionary<int, LotteryGoldVO> dic = new Dictionary<int, LotteryGoldVO>();

    private LotteryGoldVO outVO;

    protected override void Parse(string data)
    {
        base.Parse(data);

        string[] textColumn = data.Split("\n".ToCharArray());

        for (int i = 1; i < textColumn.Length; i++)
        {

            string[] textRow = textColumn[i].Split(" ".ToCharArray());

            LotteryGoldVO vo = new LotteryGoldVO();

            for (int j = 0; j < textRow.Length; j++)
            {

                vo.id = int.Parse(textRow[0]);

                vo.name = textRow[1];

                vo.iconid = int.Parse(textRow[2]);

                vo.conut = int.Parse(textRow[3]);

                vo.qualitytype = int.Parse(textRow[4]);

                vo.proptype = int.Parse(textRow[5]);

            }

            dic.Add(vo.id, vo);

        }

    }

    public LotteryGoldVO GetVO(int id)
    {

        dic.TryGetValue(id, out outVO);

        return outVO;

    }

}

public struct LotteryGoldVO
{

    public int id;

    public string name;

    public int iconid;

    public int conut;

    public int qualitytype;

    public int proptype;

}