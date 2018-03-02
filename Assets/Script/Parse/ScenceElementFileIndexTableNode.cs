using UnityEngine;
using Tianyu;
using System.Collections;
using System.Collections.Generic;

public class ScenceElementFileIndexTableNode : FSDataNodeBase
{
    public int sn;
    public int key;
    public string filename;
    public string MiniMapId;
    public int isHave;
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 scale;
    public Vector2 borderMin;
    public Vector2 borderMax;

    public override void parseJson ( object jd )
    {
        Dictionary<string , object> item = ( Dictionary<string , object> ) jd;
        sn = int.Parse( item [ "sn" ].ToString() );
        key = int.Parse( item [ "key" ].ToString() );
        filename = item [ "filename" ].ToString();
        MiniMapId = item [ "MiniMapId" ].ToString();
        isHave = int.Parse( item [ "isHave" ].ToString() );
        pos = new Vector3( float.Parse( item [ "posX" ].ToString()) , 
            float.Parse( item [ "posY" ].ToString()),
            float.Parse( item [ "posZ" ].ToString()));
        rot = new Vector3( float.Parse( item [ "rotX" ].ToString() ) ,
            float.Parse( item [ "rotY" ].ToString() ) ,
            float.Parse( item [ "rotZ" ].ToString() ) );
        scale = new Vector3( float.Parse( item [ "scaX" ].ToString() ) ,
            float.Parse( item [ "scaY" ].ToString() ) ,
            float.Parse( item [ "scaZ" ].ToString() ) );
        borderMin = new Vector2( float.Parse( item [ "borderMinX" ].ToString() ),
            float.Parse( item [ "borderMinY" ].ToString() ) );
        borderMax = new Vector2( float.Parse( item [ "borderMaxX" ].ToString() ) ,
            float.Parse( item [ "borderMaxY" ].ToString() ) );
    }
}
