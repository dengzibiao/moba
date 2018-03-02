using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TrigerTest : MonoBehaviour
{


    public uint _groupIndex;
    public uint GroupIndex { get { return _groupIndex; } set { _groupIndex = value; } }
    public int enemyMonsterCnt = 0;//TODO bug
    public TrigerTest instance;
    public bool haveMonster = false;
    void Awake()
    {
        instance = this;
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider != null)
        {
            CharacterState cs = collider.gameObject.GetComponent<CharacterState>();
            var ai = collider.gameObject.GetComponent<AIPve3V3>();
            if (cs != null)
            {
                if (cs.state == Modestatus.Player || cs.state == Modestatus.NpcPlayer)
                {
                    if (this.GroupIndex != cs.groupIndex && ai != null)
                    {
                        //ai.isInTowerAtkRange = true;
                        //Debug.LogError(collider.name + ": enter");
                    }

                }
                else if (cs.state == Modestatus.Monster)
                {
                    if (this.GroupIndex != cs.groupIndex)
                    {
                        
                    }
                }

            }

        }

    }

    void OnTriggerExit(Collider collider)
    {
        CharacterState cs = collider.gameObject.GetComponent<CharacterState>();
        var ai = collider.gameObject.GetComponent<AIPve3V3>();
        if (cs != null)
        {
            if (cs.state == Modestatus.Player || cs.state == Modestatus.NpcPlayer)
            {
                if (this.GroupIndex != cs.groupIndex && ai != null)
                {
                    //ai.isInTowerAtkRange = false;
                    //Debug.LogError(collider.name + ": exit");
                }

            }
            else if (cs.state == Modestatus.Monster)
            {
                if (this.GroupIndex != cs.groupIndex)
                {
                    
                }
            }

        }
    }
    List<CharacterState> monsterInTower = new List<CharacterState>();
    bool canCaculate = true;
    void OnTriggerStay(Collider collider)
    {
        CharacterState cs = collider.gameObject.GetComponent<CharacterState>();
        var enemy = SceneBaseManager.instance.enemy;
        if (cs!=null)
        {
            if (cs.state == Modestatus.Monster && cs.groupIndex != this.GroupIndex)
            {
                haveMonster = true;
            }
            else
                haveMonster = false;

        }

    }

  public  void Minus ()
    {
        this.enemyMonsterCnt -= 1;
    }
}