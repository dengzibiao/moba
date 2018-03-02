using Tianyu;
using System.Collections.Generic;
using UnityEngine;

public class GuideNode : FSDataNodeBase
{
    private Dictionary<string, object> item;
    public int guide_id;
    public int next_guide;
    public int guide_genre;
    public int step;
    public int ui_id;
    public int element_id;
    public int skip;
    public int window_type;
    public int orientations;
    public int resource;
    public int aspect;
    public int shade;
    public int condition;
    public int parameter;

    object deviation1;
    object deviation2;
    public Vector2 modelPos;
    public Vector2 dialogPos;
    public Vector2 effectpos;

    public string voice;
    public string guide_content;

    public override void parseJson(object jd)
    {
        item = jd as Dictionary<string, object>;
        guide_id = int.Parse(item["guide_id"].ToString());
        window_type = int.Parse(item["window_type"].ToString());
        condition = int.Parse(item["condition"].ToString());
        parameter = int.Parse(item["parameter"].ToString());
        guide_genre = int.Parse(item["guide_genre"].ToString());
        skip = int.Parse(item["skip"].ToString());
        element_id = int.Parse(item["element_id"].ToString());
        step = int.Parse(item["step"].ToString());
        ui_id = int.Parse(item["ui_id"].ToString());
        orientations = int.Parse(item["orientations"].ToString());
        resource = int.Parse(item["resource"].ToString());
        aspect = int.Parse(item["aspect"].ToString());
        shade = int.Parse(item["shade"].ToString());

        object[] nodeCond = (object[])item["deviation1"];
        deviation1 = nodeCond[0];
        LoadPos(deviation1, ref dialogPos);
        deviation1 = nodeCond[1];
        LoadPos(deviation1, ref effectpos);

        object[] nodeCond2 = (object[])item["deviation2"];
        deviation2 = nodeCond2[0];
        LoadPos(deviation2, ref modelPos);
        if(item["voice"].ToString()!=null)
        voice = item["voice"].ToString();
        guide_content = item["guide_content"].ToString();
    }

    void LoadPos(object items, ref Vector2 vector)
    {

        double[] nodepos = items as double[];
        if (nodepos != null)

        {
            // pos = ParseToFloatArray(items);
            if ( nodepos.Length > 0)
            {
                vector = new Vector2(float.Parse(nodepos[0].ToString()), float.Parse(nodepos[1].ToString()));
                // pos = null;
            }
        }
        else
        {
            int[] itempos = items as int[];
            if(itempos!=null)
            {
                if (itempos.Length > 0)
                {
                    vector = new Vector2(float.Parse(itempos[0].ToString()), float.Parse(itempos[1].ToString()));
                    // pos = null;
                }
            }
            else
            {
                object[] objs = items as object[];
                if (objs.Length > 0)
                {
                    vector = new Vector2(float.Parse(objs[0].ToString()), float.Parse(objs[1].ToString()));
                    // pos = null;
                }
            }

        }


    }
}
