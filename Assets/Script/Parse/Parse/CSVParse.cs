using UnityEngine;
using System.Collections;

/// <summary>
/// CSV解析器
/// </summary>
public class CSVParse
{

    public TextAsset txt;

    public void ParseCSV(string url)
    {
        txt = Resources.Load("CSV/" + url) as TextAsset;

        Parse(txt.text);
    }

    protected virtual void Parse(string data)
    {

    }

}
