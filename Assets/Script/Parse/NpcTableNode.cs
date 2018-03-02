using UnityEngine;
using Tianyu;
using System.Collections;
using System.Collections.Generic;

public class NpcTableNode : FSDataNodeBase
{
    public int key;
    public string model;
    public override void parseJson ( object jd )
    {
        Dictionary<string , object> item = ( Dictionary<string , object> ) jd;
        key = int.Parse( item [ "key" ].ToString() );
        model = item [ "modelid" ].ToString();
    }
}
