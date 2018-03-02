using UnityEngine;
using Tianyu;
using System.Collections;
using System.Collections.Generic;
using System;

public class ScenceNode : FSDataNodeBase
{
     
    public long key;
    public byte elementype;//1、怪物= monsterAttr 2、NPC= npc  3、传送点=TransferTable 4、复活点  5、藏宝点  6、天灾降临随机点  7、采集物=Collect
    public UInt32 typeid;
    public Vector3 pos;
    public override void parseJson ( object jd )
    {
        Dictionary<string , object> item = ( Dictionary<string , object> ) jd;
        key = long.Parse( item [ "key" ].ToString() );
        elementype = byte.Parse( item [ "type" ].ToString() );
        pos.x = float.Parse( item [ "x" ].ToString() );
        pos.y = float.Parse( item [ "y" ].ToString() );
        pos.z = float.Parse( item [ "z" ].ToString() );
    }
}
