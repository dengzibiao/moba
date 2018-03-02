using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArrowSetting : MonoBehaviour
{
    private bool isTrue = false;
    private Transform target;
    private GameObject arrow;

    Transform monster;
    float mindis;
    float dis;

    void Awake()
    {
        arrow = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Effect/Prefabs/UI/zhiyin"));
        arrow.SetActive(false);
    }

    void Update()
    {
        //if(Time.frameCount%100 != 0)
        //    return;
        AutoTest();
        if (isTrue)
        {
            if ( target != null )
            {
                Vector3 dis = target.position - CharacterManager.player.transform.position;
                arrow.transform.LookAt( target );
                arrow.transform.position = CharacterManager.player.transform.position + dis.normalized + new Vector3( 0f , 0.3f , 0f );
            }
            else
            {
                HideArrow();
            }
        }
    }

    public void ShowArrow(Transform target)
    {
        this.target = target;
        arrow.SetActive(true);
        isTrue = true;
    }

    public void HideArrow()
    {
        arrow.SetActive(false);
        isTrue = false;
    }

    bool hideArrowScene ()
    {
        return GameLibrary.SceneType(SceneType.MB1) || GameLibrary.SceneType(SceneType.Dungeons_MB1);
    }
    //判断屏幕内是否有怪
    void AutoTest()
    {
        GameObject player = CharacterManager.player;
        if(player == null || hideArrowScene())
        {
            HideArrow();
            return;
        }
        if (ClientNetMgr.GetSingle().IsConnect())//如果有网络连接
        {
            if (SceneManager.GetActiveScene().name == GameLibrary.Moba1V1_TP1)
            {
                //GameObject[] ats = CharacterManager.instance.autos;
                //if (ats.Length > 0 && Vector3.Distance(player.transform.position, ats[ats.Length - 1].transform.position) > 3f)
                //{
                //    ShowArrow(ats[ats.Length - 1].transform);
                //}
            } 
            else
            {
                if (SceneBaseManager.instance != null)
                {
                    List<SpawnMonster> spawns = SceneBaseManager.instance.spwanList;
                    BetterList<CharacterState> enemyList = SceneBaseManager.instance.enemy;
                    BetterList<Transform> targetTrans = new BetterList<Transform>();
                    for (int m = 0; m < spawns.Count; m++)
                    {
                        if (!spawns[m].spawnOver)
                            targetTrans.Add(spawns[m].transform);
                    }
                    for (int m = 0; m < enemyList.size; m++)
                    {
                        if(enemyList[m]!=null)
                        targetTrans.Add(enemyList[m].transform);
                    }

                    if (targetTrans.size <= 0)
                        return;
                    for (int i = 0; i < targetTrans.size; i++)
                    {

                        Vector2 screenPos = Camera.main.WorldToScreenPoint(targetTrans[i].position);
                        if ((screenPos.x > 0 && screenPos.x < Screen.width) && (screenPos.y > 0 && screenPos.y < Screen.height))
                        {
                            HideArrow();
                            break;
                        }
                        else
                        {
                            mindis = Vector3.Distance(player.transform.position, targetTrans[0].position);
                            for (int j = 0; j < targetTrans.size; j++)
                            {
                                dis = Vector3.Distance(player.transform.position, targetTrans[j].position);
                                if (mindis >= dis)
                                {
                                    mindis = dis;
                                    monster = targetTrans[j];
                                }
                            }
                            ShowArrow(monster);
                        }
                    }
                }
            }
        }
    }
}
