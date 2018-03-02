using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ShowType
{
    player = 1,
    otherPlayer,
    teammate,
    npc
}
public class MiniMap : MonoBehaviour
{
    public static MiniMap instance;
    ScenceElementFileIndexTableNode miniMapInfo;
    List<GameObject> targetL = new List<GameObject>();
    List<GameObject> miniTargetL = new List<GameObject>();
    GameObject miniMapShow;

    void Awake()
    {
        instance = this;
        GameObject go = GameObject.Find("miniMapT");
        if (go == null)
        {
            GameObject objT = Instantiate(Resources.Load("Prefab/miniMap/miniMapT")) as GameObject;
            objT.GetComponent<UIWidget>().SetAnchor(GameObject.Find("UI Root(Clone)"));
        }

    }

    public void CreateTargetPos (GameObject target, ShowType type)
    {
        if ( target != null)
        {
            bool isHave = false;
            for (int i=0 ;i< targetL.Count ;i++ )
            {
                if ( targetL [ i ] == null)
                {
                    Debug.LogError("列表是空的~~~~~~~~~"+i.ToString());
                }
                if ( target == null )
                {
                    Debug.LogError( "目标是空的~~~~~~~~~" );
                }
                if ( targetL [ i ] != null )
                {
                    if ( targetL [ i ].name == target.name )
                    {
                        isHave = true;
                        break;
                    }
                } 
            }
            if ( !isHave )
            {
                GameObject obj = Instantiate( Resources.Load( "Prefab/miniMap/pos" ) ) as GameObject;
                obj.transform.parent = target.transform;
                obj.transform.position = new Vector3( target.transform.position.x, miniMapInfo.pos.y + 2 , target.transform.position.z );
                obj.name = target.name + "pos";
                switch ( type )
                {
                    case ShowType.player:
                        {
                            Material [] tempM = new Material [ obj.GetComponent<MeshRenderer>().materials.Length ];
                            tempM [ 0 ] = Instantiate( Resources.Load( "Prefab/miniMap/player" ) ) as Material;
                            obj.GetComponent<MeshRenderer>().materials = tempM;
                        }
                        break;
                    case ShowType.otherPlayer:
                        {
                            Material [] tempM = new Material [ obj.GetComponent<MeshRenderer>().materials.Length ];
                            tempM [ 0 ] = Instantiate( Resources.Load( "Prefab/miniMap/otherplayer" ) ) as Material;
                            obj.GetComponent<MeshRenderer>().materials = tempM;
                        }
                        break;
                    case ShowType.teammate:
                        {
                            Material [] tempM = new Material [ obj.GetComponent<MeshRenderer>().materials.Length ];
                            tempM [ 0 ] = Instantiate( Resources.Load( "Prefab/miniMap/teammate" ) ) as Material;
                            obj.GetComponent<MeshRenderer>().materials = tempM;
                        }
                        break;
                    case ShowType.npc:
                        {
                            Material [] tempM = new Material [ obj.GetComponent<MeshRenderer>().materials.Length ];
                            tempM [ 0 ] = Instantiate( Resources.Load( "Prefab/miniMap/npc" ) ) as Material;
                            obj.GetComponent<MeshRenderer>().materials = tempM;
                        }
                        break;
                }
                miniTargetL.Add( obj );
                targetL.Add( target );
            }
        }
    }

    void Update ()
    {

       for(int i=0 ;i< targetL.Count ;i++ )
        {
            if ( targetL[i] == null )
            {
                targetL.RemoveAt( i );
                miniTargetL.RemoveAt( i );
            }
            else
            {
                if (miniMapInfo != null)
                {
                    if (CharacterManager.player != null)
                    {
                        Vector3 tempPos = CharacterManager.player.transform.position;
                        tempPos.x = Mathf.Clamp(tempPos.x, miniMapInfo.borderMin.x, miniMapInfo.borderMax.x);
                        tempPos.z = Mathf.Clamp(tempPos.z, miniMapInfo.borderMin.y, miniMapInfo.borderMax.y);
                        tempPos.y = miniMapInfo.pos.y + 2;
                        transform.position = tempPos;
                    }
                    Vector3 tempMiniPos = targetL[i].transform.position;
                    tempMiniPos.y = miniMapInfo.pos.y + 2;
                    miniTargetL[i].transform.position = tempMiniPos;
                }
            }
        }
    }

    public static void  Create ( ScenceElementFileIndexTableNode miniMapInfo)
    {
        GameObject obj = Instantiate( Resources.Load( "Prefab/miniMap/miniMapCamera" ) ) as GameObject;
        GameObject objShow = Instantiate( Resources.Load( "Prefab/miniMap/miniMapShow" ) ) as GameObject;
        objShow.transform.position = miniMapInfo.pos;
        objShow.transform.localRotation = Quaternion.Euler( miniMapInfo.rot );
        objShow.transform.localScale = miniMapInfo.scale;
        objShow.name = "miniMapShow";
        Material [] tempM = new Material [ objShow.GetComponent<MeshRenderer>().materials.Length ];
        Texture tempT = Instantiate( Resources.Load( "Prefab/miniMap/" + miniMapInfo.MiniMapId ) ) as Texture;
        objShow.GetComponent<MeshRenderer>().materials [ 0 ].mainTexture = tempT;
        obj.AddComponent<MiniMap>();
        obj.GetComponent<MiniMap>().miniMapInfo = miniMapInfo;
    }
}
