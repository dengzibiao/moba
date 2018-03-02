using UnityEngine;
using Tianyu;
using System.Collections;
using System.Collections.Generic;


public class SceneMapNode : FSDataNodeBase
{
    public int ID;
    public int type;//1、怪物= monsterAttr
//2、NPC= npc 
//3、传送点=TransferTable
//4、复活点  
//5、藏宝点  
//6、天灾降临随机点  
//7、采集物=Collect
    public long type_id;
    public Vector3 pos;
    public float rotation;

    public override void parseJson ( object jd )
    {
        Dictionary<string , object> item = ( Dictionary<string , object> ) jd;
        ID = int.Parse( item [ "key" ].ToString() );
        type = int.Parse( item [ "type" ].ToString());
        type_id = long.Parse( item [ "type_id" ].ToString() );
        pos.x = float.Parse( item [ "x" ].ToString() );
        pos.y = float.Parse( item [ "y" ].ToString() );
        pos.z = float.Parse( item [ "z" ].ToString() );
        if (item.ContainsKey("rotation"))
        {
            rotation = float.Parse(item["rotation"].ToString());
        }
    }
}
