using UnityEngine;
using Tianyu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum AttackType
{
    action = -1,//表演
    Monster = 0,//打怪
    OtherPlayer,//打其他玩家
    Player,//打的是自己
}

public class CreatePeople : MonoBehaviour
{
	public Dictionary<long,GameObject> MonsterDic = new Dictionary<long, GameObject>();//怪物列表
    public Dictionary<long, GameObject> OtherplayerDic = new Dictionary<long, GameObject>();//其他玩家列表
    public Dictionary<long, GameObject> NpcDic = new Dictionary<long, GameObject>();//npc列表
    public Dictionary<long, GameObject> ResurgenceDic = new Dictionary<long, GameObject>();
    public Dictionary<long, GameObject> MineDic = new Dictionary<long, GameObject>();
    public Dictionary<long, GameObject> TransferDic = new Dictionary<long, GameObject>();//传送点列表
    public Dictionary<long, GameObject> CollectDic = new Dictionary<long, GameObject>();//采集物列表
    private static CreatePeople instance;
    public delegate void OverMine();
    public OverMine myOverMine;
    UnityEngine.AI.NavMeshAgent NMA;
    Vector3 oldPos;
    GameObject Monster;
    public GameObject OhterPlayer;
    GameObject Mine;
    GameObject Npc;
    GameObject Resurgence;
    GameObject Transfer;
    GameObject Collect;
    int index;
    uint targetMineKeyId;

    public static CreatePeople GetInstance ()
    {
       // if (!GameLibrary.Instance().CheckSceneNeedSync()) return null;
        if (instance == null)
        {
            GameObject obj = new GameObject();
            if (CharacterManager.instance != null&&CharacterManager.instance.transform.parent!=null)
            {
                //obj.transform.parent = CharacterManager.instance.transform; 
                obj.transform.localPosition = CharacterManager.instance.transform.parent.position;
            }
            obj.name = "CreatePeople";
            obj.AddComponent<CreatePeople>();
           
        }
        return instance;
    }

    void Awake ()
    {
        instance = this;
        Monster = SetInfo( "MonsterParent" );
        OhterPlayer = SetInfo( "OtherParent" );
        Mine = SetInfo( "MineParent" );
        Npc = SetInfo( "NpcParent" );
        Resurgence = SetInfo( "ResurgenceParent" );
        Transfer = SetInfo( "TransferParent" );
        Collect = SetInfo("CollectParent");
    }

    GameObject SetInfo (string name)
    {
        GameObject obj = new GameObject();
        obj.transform.parent = this.transform;
        obj.transform.localPosition = Vector3.zero;
        obj.name = name;
        return obj;
    }

    public void OtherPlayerAttack ( uint attackKeyId , uint targetKeyId , AttackType attackType ,int skillIndex, Vector3 attackpos, Vector3 attackrot,Vector3 targetpos)
    {
        if ( OtherplayerDic.ContainsKey( attackKeyId ) )
        {
            GameObject tempObj = null;
            tempObj =  OtherplayerDic[ attackKeyId] ;
            CharacterState tempCs = tempObj != null ? tempObj.GetComponent<CharacterState>() : null;
            GameObject targetObj = null;
            if(attackType == AttackType.Monster)//打怪
            {
                MonsterDic.TryGetValue(targetKeyId, out targetObj);
            }
            else if(attackType == AttackType.OtherPlayer)//打人
            {
                OtherplayerDic.TryGetValue(targetKeyId, out targetObj);
            } else if(attackType == AttackType.Player)//攻击自己
            {
                targetObj = CharacterManager.player;
            }
            else//表演
            {
            }
            CharacterState targetCs = targetObj != null ? targetObj.GetComponent<CharacterState>() : null;
            DoAttack(tempCs, CharacterManager.playerCS, skillIndex);
        }
    }

    void DoAttack ( CharacterState attacker , CharacterState target, int sid)
    {
        if(attacker != null && target != null){
            if(target != null)
            {
                attacker.transform.LookAt(new Vector3(target.transform.position.x, attacker.transform.position.y, target.transform.position.z));
                attacker.SetAttackTargetTo(target);
            }
            if(sid < 4)
            {
                attacker.pm.ContinuousAttack();
            }
            else
            {
                attacker.pm.Skill(sid - 3);
            }
        }
    }

    public void OtherPlayerDamage ( long keyId ,int damageHp , int skillID , long attackKeyId , AttackType attackType ,bool isShowBuff = false,float baseVal = 0f)
    {
        //if ( OtherplayerDic.ContainsKey( keyId ) )
        //{
        //    GameObject tempObj = null;
        //    MonsterDic.TryGetValue( keyId , out tempObj );
        //    SkillNode skillNode = FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList [ skillID ];
        //    if ( damageHp > 0 )
        //    {
        //        GameObject tempObj_M_O = null;
        //        if ( attackType == AttackType.Monster )//怪物
        //        {
        //            if ( MonsterDic.ContainsKey( attackKeyId ) )
        //            {
        //                MonsterDic.TryGetValue( attackKeyId , out tempObj_M_O );
        //            }
        //        }
        //        else if ( attackType == AttackType.OtherPlayer )//人物
        //        {
        //            if ( OtherplayerDic.ContainsKey( attackKeyId ) )
        //            {
        //                OtherplayerDic.TryGetValue( attackKeyId , out tempObj_M_O );
        //            }
        //        }
        //        if ( tempObj != null)
        //        {
        //            tempObj.GetComponent<CharacterState>().Hp( damageHp , HUDType.DamageEnemy , true );
        //            tempObj.GetComponent<PlayerMotion>().Hit();
        //            if ( tempObj_M_O != null)
        //            {
        //                tempObj.GetComponent<CharacterState>().emission.PlayHitEffect( skillNode , tempObj , tempObj_M_O.GetComponent<CharacterState>() );
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if ( tempObj != null )
        //        {
        //            tempObj.GetComponent<CharacterState>().Hp( 0 , HUDType.Miss , true );
        //        }
        //    }
            

        //    if ( isShowBuff )
        //    {
        //        GameObject tempObj_M_O = null;
        //        if ( attackType == AttackType.Monster )//怪物
        //        {
        //            if ( MonsterDic.ContainsKey( attackKeyId ) )
        //            {
        //                MonsterDic.TryGetValue( attackKeyId , out tempObj_M_O );
        //            }
        //        }
        //        else if ( attackType == AttackType.OtherPlayer )//人物
        //        {
        //            if ( OtherplayerDic.ContainsKey( attackKeyId ) )
        //            {
        //                OtherplayerDic.TryGetValue( attackKeyId , out tempObj_M_O );
        //            }
        //        }

                
        //        if ( skillNode.add_state != null )
        //        {
        //            for ( int i = 0 ; i < skillNode.add_state.Length ; i++ )
        //            {
        //                object o = skillNode.add_state [ i ];
        //                if ( o != null && o is System.Array && ( ( System.Array ) o ).Length > 0 )
        //                {
        //                    if ( tempObj != null && tempObj_M_O != null )
        //                    {
        //                        SkillBuffManager.GetInst().AddBuffs( baseVal , skillNode.add_state [ i ] , tempObj.GetComponent<CharacterState>() , tempObj_M_O.GetComponent<CharacterState>() );
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }
    /// <summary>
    /// 创建生命建筑物
    /// </summary>
    /// <param name="SMN"></param>
    /// <param name="ri"></param>
    /// <param name="isTeam"></param>
    public void CreateBuild(SceneMapNode SMN, ref RoleInfo ri, bool isTeam)
    {
        MonsterData monsterData = new MonsterData(SMN.type_id);
        if (monsterData == null && monsterData.attrNode == null)
        {
            Debug.Log(SMN.type_id);
            return;
        }

        //MonsterAttrNode MAN = FSDataNodeTable<MonsterAttrNode>.GetSingleton().FindDataByType( SMN.type_id );
        if (monsterData.attrNode != null)
        {
            GameObject obj = Resource.CreateCharacter(monsterData.attrNode, Monster);
            obj.transform.localPosition = new Vector3(ri.posX, GetMapY(ri.posX, ri.posY, ri.posZ), ri.posZ);

            //if (!obj.activeSelf)
            //{
            //    obj.SetActive(true);
            //}

            ri.RoleObj = obj;
          
            

        
        }
    }
    /// <summary>
    /// 创建怪物和活动怪
    /// </summary>
    /// <param name="SMN"></param>
    /// <param name="ri"></param>
    /// <param name="isTeam"></param>
    public void CreateMonster(ref RoleInfo ri, bool isTeam)
    {
        // MonsterData monsterData = new MonsterData(SMN.type_id);
        //if (monsterData == null&& monsterData.attrNode==null)
        //{
        //    Debug.Log(SMN.type_id);
        //    return;
        //}
        MonsterData monsterData = new MonsterData(ri.typeid);
        if (monsterData == null && monsterData.attrNode == null)
        {
            Debug.Log(ri.typeid);
            return;
        }

        //MonsterAttrNode MAN = FSDataNodeTable<MonsterAttrNode>.GetSingleton().FindDataByType( SMN.type_id );
        if (monsterData.attrNode != null)
        {
            GameObject obj = Resource.CreateCharacter(monsterData.attrNode, Monster);
            obj.transform.localPosition = new Vector3(ri.posX, GetMapY(ri.posX, ri.posY, ri.posZ), ri.posZ);

            //if (!obj.activeSelf)
            //{
            //    obj.SetActive(true);
            //}

            ri.RoleObj = obj;
            CharacterController cc =  UnityUtil.AddComponetIfNull<CharacterController>(obj) ;
            if (cc != null)
            {
                cc.radius = 0.1f;
                cc.height = 0.6f;
                cc.center = new Vector3(0,0.3f,0);
            }
            PlayerMotion mm = UnityUtil.AddComponetIfNull<PlayerMotion>(obj);
            mm.ani = obj.GetComponent<Animator>();
            UnityUtil.AddComponetIfNull<UnityEngine.AI.NavMeshAgent>(obj);
            mm.nav.radius = 0.1f;
            mm.nav.height = 0.6f;
            mm.nav.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.LowQualityObstacleAvoidance;
            mm.nav.avoidancePriority = 49;
            mm.nav.stoppingDistance = 0.3f;
            mm.nav.speed = monsterData.attrNode.movement_speed;
            mm.hp = ri.hp;

            CharacterState mCs = UnityUtil.AddComponetIfNull<CharacterState>(obj);

            if (mCs != null)
            {
                mCs.keyId = ri.keyID;
                mCs.rc = ri.rc;
                mCs.InitHp(ri.hp);
                //mCs.maxHp = ri.hp;
                if (!mCs.isNetworking)
                    mCs.isNetworking = true;

                mCs.InitData(monsterData);

                if (obj.tag == "Boss")
                {
                    obj.transform.localScale = Vector3.one * 1.3f;

                    if (mCs != null && mCs.CharData != null)
                        mCs.CharData.state = Modestatus.Boss;

                    obj.name = "Boss" + ri.keyID.ToString() + "_" + index.ToString();
                }
                else
                {
                    if (mCs != null && mCs.CharData != null)
                        mCs.CharData.state = Modestatus.Monster;

                    obj.name = "Monster" + ri.keyID.ToString() + "_" + index.ToString();
                    mCs.AddHpBar();
                }

                //mCs.OnDead += Die;
                if (monsterData.attrNode.types == 4)
                {
                    if (mCs.CharData != null)
                        mCs.CharData.state = Modestatus.Boss;
                    obj.name = "Elite" + ri.keyID.ToString() + "_" + index.ToString();
                }

                obj.AddComponent<OtherPlayer>();
                // if (SceneManager.GetActiveScene().name != GameLibrary.LGhuangyuan)


                if (mCs.CharData != null)
                    mCs.CharData.groupIndex = 0;
                if (!MonsterDic.ContainsKey(ri.keyID))
                    MonsterDic.Add(ri.keyID, obj);

                if (SceneBaseManager.instance != null)
                {
                    SceneBaseManager.instance.AddCs(mCs);
                    mCs.OnDead += ( c ) => SceneBaseManager.instance.RemoveCs(mCs);
                }
            }
            else
            {
                Debug.Log(obj.transform.name);
            }
        }
    }
    /// <summary>
    /// 创建npc
    /// </summary>
    /// <param name="SMN"></param>
    /// <param name="ri"></param>
    /// <param name="isTeam"></param>
    /// <param name="min"></param>
    public void CreateNpc(SceneMapNode SMN,ref RoleInfo ri, bool isTeam, MapInfoNode min)
    {
        if (FSDataNodeTable<NPCNode>.GetSingleton().DataNodeList.ContainsKey(SMN.type_id))
        {
            int model = FSDataNodeTable<NPCNode>.GetSingleton().DataNodeList[SMN.type_id].modelid;
            if (FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList.ContainsKey(model))
            {
                GameObject obj = Instantiate(Resources.Load(FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList[model].respath)) as GameObject;
                obj.transform.parent = Npc.transform;
                obj.transform.position = new Vector3(ri.posX, GetMapY(ri.posX, ri.posY, ri.posZ), ri.posZ);
                if (ri.type != 8 || ri.type != 9)
                {
                    obj.transform.rotation = Quaternion.Euler(0f, SMN.rotation, 0f);
                }
                
                ri.RoleObj = obj;
                obj.name = "Npc" + ri.keyID.ToString() + "_" + index.ToString(); ;
                if(!NpcDic.ContainsKey(ri.keyID))
                NpcDic.Add(ri.keyID, obj);

                //创建npc后 给npc添加TaskNPC脚本 并且给npc添加状态特效
                obj.AddComponent<TaskNPC>();
                obj.GetComponent<TaskNPC>().NPCID = FSDataNodeTable<NPCNode>.GetSingleton().DataNodeList[SMN.type_id].npcid;
                obj.GetComponent<TaskNPC>().NpcKey = ri.keyID;
                if (SceneManager.GetActiveScene().name == GameLibrary.UI_Major)
                BattleUtil.AddShadowTo(obj);
                UnityUtil.AddComponetIfNull<CharacterState>(obj);
                UnityUtil.AddComponetIfNull<NPCMotion>(obj);
                NPCManager.GetSingle().CreateNpcModel(FSDataNodeTable<NPCNode>.GetSingleton().DataNodeList[SMN.type_id].npcid, obj);

                string footEffctpath = "Effect/Prefabs/UI/jiaodgq";
                GameObject footgo = Instantiate(Resources.Load(footEffctpath)) as GameObject;
                footgo.transform.parent = obj.transform;
                footgo.transform.position = Vector3.one;
                footgo.transform.localScale = Vector3.one;
                footgo.name = "jiaodgq";
                footgo.SetActive(false);

                string toudjstbpath = "Effect/Prefabs/UI/toudjstb";
                GameObject toudjstb = Instantiate(Resources.Load(toudjstbpath)) as GameObject;
                toudjstb.transform.parent = obj.transform;
                toudjstb.transform.position = Vector3.one;// 位置设在头顶
                toudjstb.transform.localScale = Vector3.one;
                toudjstb.name = "toudjstb";
                toudjstb.SetActive(false);

                string toudwctbpath = "Effect/Prefabs/UI/toudwctb";
                GameObject toudwctb = Instantiate(Resources.Load(toudwctbpath)) as GameObject;
                toudwctb.transform.parent = obj.transform;
                toudwctb.transform.position = Vector3.one;// 位置设在头顶
                toudwctb.transform.localScale = Vector3.one;
                toudwctb.name = "toudwctb";
                toudwctb.SetActive(false);


                //if (FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList[min.map_info].isHave == 1)
                //{
                //    if (MiniMap.instance != null)
                //    {
                //        MiniMap.instance.CreateTargetPos(obj, ShowType.npc);
                //    }
                //}
            }
            else
            {
                Debug.Log(model + "  模型不存在");
            }
        }
        else
        {
            Debug.Log(SMN.type_id + " npc不存在");
        }
    }
    /// <summary>
    /// 创建传送门
    /// </summary>
    /// <param name="SMN"></param>
    /// <param name="ri"></param>
    /// <param name="isTeam"></param>
    public void CreateTransferTable(SceneMapNode SMN,ref RoleInfo ri, bool isTeam)
    {
        if (FSDataNodeTable<TransferTableNode>.GetSingleton().DataNodeList.ContainsKey(SMN.type_id))
        {
            string model = FSDataNodeTable<TransferTableNode>.GetSingleton().DataNodeList[SMN.type_id].model;
            GameObject obj = Instantiate(Resources.Load("Effect/Prefabs/Build/" + model)) as GameObject;
            obj.transform.parent = Transfer.transform;
            obj.transform.position = new Vector3(ri.posX, GetMapY(ri.posX, ri.posY, ri.posZ), ri.posZ); ;
            obj.transform.LookAt(playerData.GetInstance().selfData.GetPos());//策划说传送门应该朝向玩家出生点
            obj.name = "Transfer" + ri.keyID.ToString() + "_" + index.ToString();
            ri.RoleObj = obj;
            if(!TransferDic.ContainsKey(ri.keyID))
            TransferDic.Add(ri.keyID, obj);
        }
        else
        {
            Debug.Log(SMN.type_id);
        }
    }
    /// <summary>
    /// 创建复活点
    /// </summary>
    /// <param name="SMN"></param>
    /// <param name="ri"></param>
    /// <param name="isTeam"></param>
    public void CreateRevioPos(SceneMapNode SMN,ref RoleInfo ri, bool isTeam)
    {
        GameObject obj = new GameObject();
        obj.transform.parent = Resurgence.transform;
        obj.transform.position = new Vector3(ri.posX, GetMapY(ri.posX, ri.posY, ri.posZ), ri.posZ); ;
        obj.name = "Resurgence" + ri.keyID.ToString() + "_" + index.ToString();
        ri.RoleObj = obj;
        if(!ResurgenceDic.ContainsKey(ri.keyID))
        ResurgenceDic.Add(ri.keyID, obj);
    }
    /// <summary>
    /// 创建藏宝点
    /// </summary>
    /// <param name="SMN"></param>
    /// <param name="ri"></param>
    /// <param name="isTeam"></param>
    public void CreateMinePos(SceneMapNode SMN,ref RoleInfo ri, bool isTeam)
    {
        GameObject obj = new GameObject();
        obj.transform.parent = Mine.transform;
        obj.transform.position = new Vector3(ri.posX, GetMapY(ri.posX, ri.posY, ri.posZ), ri.posZ); ;
        obj.name = "Mine" + ri.keyID.ToString() + "_" + index.ToString();
        ri.RoleObj = obj;
        if(!MineDic.ContainsKey(ri.keyID))
        MineDic.Add(ri.keyID, obj);
    }
    /// <summary>
    /// 创建采集物
    /// </summary>
    /// <param name="SMN"></param>
    /// <param name="ri"></param>
    /// <param name="isTeam"></param>
    public void CreateCollect(SceneMapNode SMN, ref RoleInfo ri, bool isTeam)
    {
        int model = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[SMN.type_id].model;
        if (FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList.ContainsKey(model))
        {
            string respath = FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList[model].respath;
            if (respath != "")
            {

                Object resobj = Resources.Load(respath);
                if (resobj != null)
                {
                    GameObject obj = Instantiate(resobj) as GameObject;
                    obj.transform.parent = Collect.transform;
                    obj.transform.position = new Vector3(ri.posX, GetMapY(ri.posX, ri.posY, ri.posZ), ri.posZ);
                    ri.RoleObj = obj;
                    obj.name = "Collect" + ri.keyID.ToString() + "_" + index.ToString();
                    ri.RoleObj = obj;
                    if(CollectDic!=null&&!CollectDic.ContainsKey(ri.keyID))
                    CollectDic.Add(ri.keyID, obj);

                    //创建采集物后 给采集物添加TaskCollectPoint脚本 
                    obj.AddComponent<TaskCollectPoint>();
                    obj.GetComponent<TaskCollectPoint>().collectID = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[SMN.type_id].id;
                }
            }
        }
        else
        {
            Debug.Log(model + "  模型不存在");
        }
    }
    void  CreateTower(SceneMapNode SMN, ref RoleInfo ri, bool isTeam)
    {

    }
   void CreatElement(ref RoleInfo ri, bool isTeam = false)
    {
       // Debug.Log(ri.keyID);
        if (1 == ri.type || 6 == ri.type)
        {
            CreateMonster(ref ri, isTeam);
        }
    }
    void  CreateMapElement(ref RoleInfo ri,  MapInfoNode min, bool isTeam = false)
    {
       if(1==ri.type|| 6 == ri.type)
        {
            CreateMonster(ref ri, isTeam);
        }
      
        if (min.MapName == Application.loadedLevelName)//场景为加载场景时处理
        {
            if (FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList.ContainsKey(min.map_info))
            {
               string fileName = FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList[min.map_info].filename;
                if (FSDataNodeTable<SceneMapNode>.GetSingleton().DataNodeLists[fileName].ContainsKey((long)ri.keyID))
                {
                    SceneMapNode SMN = FSDataNodeTable<SceneMapNode>.GetSingleton().DataNodeLists[fileName][(long)ri.keyID];

                    switch (ri.type)
                    {
                        ////怪物，活动怪
                        //case 1:
                        //case 6:
                        //    {
                        //        // CreateMonster(SMN, ref ri, isTeam);
                        //        CreateMonster(ref ri, isTeam);
                        //    }
                        //    break;
                        //npc
                        case 2:
                            {
                                CreateNpc(SMN, ref ri, isTeam, min);
                            }
                            break;
                        case 3:
                            //传送点
                            {
                                CreateTransferTable(SMN, ref ri, isTeam);
                            }
                            break;
                        case 4:
                            //复活点
                            {
                                CreateRevioPos(SMN, ref ri, isTeam);
                            }
                            break;
                        case 5:
                            //藏宝点
                            {
                                CreateMinePos(SMN, ref ri, isTeam);
                            }
                            break;
                        case 7:
                            //采集物
                            {
                                CreateCollect(SMN, ref ri, isTeam);
                            }
                            break;
                        case 8:
                        case 9:
                            //塔
                            {
                                CreateBuild(SMN, ref ri, isTeam);
                            }
                            break;
                            //case 9:
                            //    //基地
                            //    {
                            //        CreateMonster(SMN, ref ri, isTeam);
                            //    }
                            //    break;
                    }
                    index++;
                }
                else
                {
                    Debug.Log(ri.keyID + "  不存在！！！");
                }
            }
        }
    }
    /// <summary>
    /// 创建场景元素
    /// </summary>
    /// <param name="ri"></param>
    /// <param name="isTeam"></param>
    public void CreateScenceElement(ref RoleInfo ri, bool isTeam = false)
    {
        MapInfoNode min;
        if (FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.TryGetValue(ri.mapID, out min))
        {
			Debug.Log ("创建场景元素信息");
            CreateMapElement(ref ri,min,isTeam);
           
        }
        else
        {
            CreatElement(ref ri,isTeam);
        }
    }
    /// <summary>
    /// 创建附近物件
    /// </summary>
    /// <param name="ri"></param>
    /// <param name="isTeam"></param>
    public void CreatOtherObject (ref RoleInfo ri , bool isTeam = false )
    {
        if (SceneManager.GetActiveScene().name != GameLibrary.UI_Major
           && SceneManager.GetActiveScene().name != GameLibrary.LGhuangyuan
           && SceneManager.GetActiveScene().name != GameLibrary.PVP_1V1
          )
        {
            Debug.Log("创建其他玩家返回" + ri.name);
            return;
        }
            if ( CharacterManager.player != null )
        {//战斗设置为联网
            if(!CharacterManager.player.GetComponent<CharacterState>().isNetworking)
                CharacterManager.player.GetComponent<CharacterState>().isNetworking = true;
        }
        if ( ri.accID == 0 )//账号id为0时
        {
            CreateScenceElement(ref ri, isTeam);
        }
        else//账号id不为0的处理
        {
            if (ri.keyID == playerData.GetInstance().selfData.keyId)
            {
				Debug.Log (" CreateOther init player");
                if (CharacterManager.player != null)
                {
                    if (!CharacterManager.instance.isInit)
                    {
						CharacterManager.player.transform.parent = CreatePeople.instance.OhterPlayer.transform;
						CharacterManager.player.transform.localPosition = playerData.GetInstance ().selfData.GetPos ();
						CharacterManager.instance.isInit = true;
					}
				} 
			//	Debug.Log ("创建其他玩加");
				
			}
			else {
				Debug.Log ("CreateOtherPlayer"+ri.keyID);
				CreateOtherPlayer (ref ri, isTeam);
			}

           
        }
    }
    //创建其他玩家
    public void CreateOtherPlayer(ref RoleInfo ri, bool isTeam)
    {
        if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(ri.roleID))//包含英雄id时
        {
			Debug.Log (ri.roleID);
            HeroData heroDara = new HeroData(ri.roleID);
            //heroDara.groupIndex = ri.keyID;
            heroDara.groupIndex = CharacterManager.playerGroupIndex;
            heroDara.state = Modestatus.NpcPlayer;

            GameObject obj = Resource.CreateCharacter(heroDara.node.icon_name, OhterPlayer);
            CharacterState otherCs = BattleUtil.AddMoveComponents(obj, heroDara.attrNode.modelNode);
            obj.name = ri.name;
            otherCs.InitData(heroDara);
            otherCs.isNetworking = true;
            otherCs.keyId = ri.keyID;
            otherCs.rc = ri.rc;
            otherCs.pm.nav.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
            otherCs.pm.nav.speed = otherCs.CharData.attrNode.movement_speed;
            ri.RoleObj = obj;
            //System.Random rd = new System.Random();
            //float x = rd.Next(0, 360);
            //float z = rd.Next(0, 360);

            OtherPlayer otherPlayer = obj.AddComponent<OtherPlayer>();
            otherPlayer.rType = RoleType.otherPlayer;
            otherPlayer.PlayPrepare();
            otherPlayer.SetStartPosAndRot(new Vector3(ri.posX, ri.posY, ri.posZ), new Vector3(ri.orientX, ri.posY, ri.orientZ), ri.name,ri.title,ri.unionName);
            if (ri.mount > 0)
                otherCs.pm.Ride(true, ri.mount, false);

            if (!OtherplayerDic.ContainsKey(ri.keyID))
            OtherplayerDic.Add(ri.keyID, obj);

            //foreach (MapInfoNode min in FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.Values)
            //{
            //    if (min.MapName == Application.loadedLevelName)
            //    {
            //        if (FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList.ContainsKey(min.map_info))
            //        {
            //            //if (FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList[min.map_info].isHave == 1)
            //            //{
            //                //if (MiniMap.instance != null)
            //                //{
            //                //    if (!isTeam)
            //                //    {
            //                //        MiniMap.instance.CreateTargetPos(obj, ShowType.otherPlayer);
            //                //    }
            //                //    else
            //                //    {
            //                //        MiniMap.instance.CreateTargetPos(obj, ShowType.teammate);
            //                //    }
            //                //}
            //            //}
            //        }
            //    }
            //}
            if(SceneBaseManager.instance != null)
            {
                SceneBaseManager.instance.AddCs(otherCs);
            }
        }
        else//不包含此英雄id时
        {
            Debug.Log("角色ID不对");
        }
    }

    public void SetOtherPlayerOrientation ( long keyId ,float rotX,float rotY,float rotZ)
    {
        if ( OtherplayerDic.ContainsKey( keyId ) )
        {
            GameObject tempObj = null;
            if ( OtherplayerDic.TryGetValue( keyId , out tempObj ) )
            {
                if ( tempObj.GetComponent<OtherPlayer>() == null )
                {
                    tempObj.AddComponent<OtherPlayer>();
                }
                tempObj.GetComponent<OtherPlayer>().SetRot( new Vector3( rotX,rotY,rotZ));
            }
        }
    }
    
    public static  float GetMapY ( float x , float y,float z )
    {
        float Y =y;

        RaycastHit [] hits;
        Ray ray = new Ray( new Vector3( x ,y+100 ,z ) , Vector3.down );

        hits = Physics.RaycastAll( ray , 1000 , 1 << LayerMask.NameToLayer( "Ground" ) );

        for ( int i = 0 ; i < hits.Length ; i++ )
        {
            Vector3 startPos = new Vector3(x, hits[i].point.y, z);
            UnityEngine.AI.NavMeshHit hit1;
            UnityEngine.AI.NavMesh.SamplePosition(startPos, out hit1, 0.2f, UnityEngine.AI.NavMesh.AllAreas);
            //return hits [ i ].point.y;
            return hit1.position.y;
        }

        if ( hits.Length == 0 )
        {
            Debug.Log( "服务器目标位置不在地图中" );
        }

        return Y;
    }

    public void GetOtherObjectByKeyId(long keyId,ref GameObject tempObj)
    {
        if(playerData.GetInstance().NearRIarr.ContainsKey(keyId))
        {
            tempObj = playerData.GetInstance().NearRIarr[keyId].RoleObj;
        }        
    }

    public void UpdateOtherPlayerInfo ( long keyId, UpdatePlayerInfoType infoType)
    {
        GameObject tempObj = null;
        GetOtherObjectByKeyId(keyId, ref tempObj);
        if(tempObj != null)
        {
            RoleInfo rInfo = playerData.GetInstance().NearRIarr[keyId];
            CharacterState otherCs = tempObj.GetComponent<CharacterState>();
            if(otherCs == null || rInfo == null)
                return;
            switch(infoType)
            {
                case UpdatePlayerInfoType.Mount:
                    otherCs.pm.nav.enabled = true;
                    otherCs.pm.Ride(rInfo.mount > 0, rInfo.mount, false);
                    break;
                case UpdatePlayerInfoType.Pet:
                    if(rInfo.petid > 0)
                        otherCs.CreatePet(rInfo.petid);
                    else
                        otherCs.HidePet();
                    break;
                default:
                    break;
            }
        }
    }

    public void ChangeOtherHp(long keyId, int hp)
    {
        GameObject tempObj = null;
        GetOtherObjectByKeyId(keyId, ref tempObj);
        if ( tempObj != null && keyId != playerData.GetInstance().selfData.keyId)
        {
            if ( tempObj.GetComponent<OtherPlayer>() == null )
            {
                tempObj.AddComponent<OtherPlayer>();
            }
            if ( tempObj.GetComponent<CharacterState>() != null && tempObj.GetComponent<CharacterState>().pm != null )
            {
                tempObj.GetComponent<CharacterState>().ChangeHp( hp );
            }
            //else
            //{
            //    Debug.Log("MoveOtherObject 没有赋值 ======>" + rc);
            //}
            //Vector3 targetPos = new Vector3(x, GetMapY(x, y, z), z);
            //NavMeshHit hit;
            //NavMesh.SamplePosition(targetPos, out hit, 1.0f, NavMesh.AllAreas);
            //tempObj.GetComponent<OtherPlayer>().SetTargetPos(hit.position);
            //Debug.Log(rc + " MoveOtherObject!!!");


        }
        else
        {
            if( CharacterManager.player !=null && keyId == playerData.GetInstance().selfData.keyId)
                CharacterManager.player.GetComponent<CharacterState>().ChangeHp( hp,HUDType.DamagePlayer,true );
            else
            {
                Debug.Log(keyId + "   不存在");
            }
        }
    }

    public void MoveOtherObject(long keyId, float x, float y, float z, int rc)
    {
        GameObject tempObj = null;
        GetOtherObjectByKeyId(keyId, ref tempObj);
        if (tempObj != null)
        {
            if (tempObj.GetComponent<OtherPlayer>() == null)
            {
                tempObj.AddComponent<OtherPlayer>();
            }
            if (tempObj.GetComponent<CharacterState>() != null)
            {
                tempObj.GetComponent<CharacterState>().rc = rc;
            }
            else
            {
                Debug.LogError("MoveOtherObject 没有赋值 ======>" + rc);
            }
            Vector3 targetPos = new Vector3(x, GetMapY(x, y, z), z);
            UnityEngine.AI.NavMeshHit hit;
            UnityEngine.AI.NavMesh.SamplePosition(targetPos, out hit, 0.2f, UnityEngine.AI.NavMesh.AllAreas);
            if(tempObj.GetComponent<OtherPlayer>()!=null)
            tempObj.GetComponent<OtherPlayer>().SetTargetPos(targetPos);
            // Debug.Log(rc + " MoveOtherObject!!!");
        }
        //else
        //{
        //    if (keyId == playerData.GetInstance().selfData.keyId)
        //    {
        //        playerData.GetInstance().selfData.SetPos( new Vector3(x, y, z));
        //    }
       // }
    }

    public GameObject GetTargert(uint keyId)
    {
        GameObject tempObj = null;
        if (playerData.GetInstance().NearRIarr.ContainsKey(keyId))
        {
            RoleInfo rinfo = playerData.GetInstance().NearRIarr[keyId];
            if (rinfo != null)
            {
                tempObj = rinfo.RoleObj;
            }
        }        
        //if (MonsterDic.ContainsKey(keyId))
        //{
        //    MonsterDic.TryGetValue(keyId, out tempObj);
        //}
        //else if (NpcDic.ContainsKey(keyId))
        //{
        //    NpcDic.TryGetValue(keyId, out tempObj);
        //}
        //else if (CollectDic.ContainsKey(keyId))
        //{
        //    CollectDic.TryGetValue(keyId, out tempObj);
        //}
        //else if (OtherplayerDic.ContainsKey(keyId))
        //{
        //    OtherplayerDic.TryGetValue(keyId, out tempObj);
        //}
            return tempObj;
    }
    /// <summary>
    /// 删除其他玩家
    /// </summary>
    /// <param name="tempObj"></param>
    /// <param name="ri"></param>
    /// <param name="isDelete"></param>
     void  DeleteOtherPlayer(GameObject tempObj,RoleInfo ri,bool isDelete = false)
    {
        if (tempObj != null)
        {
            CharacterState otherCs = tempObj.GetComponent<CharacterState>();
            if (otherCs != null)
            {
                if (!otherCs.isDie)
                {
                    SkillBuffManager.GetInst().ClearBuffsFrom(otherCs);
                    if (otherCs.OnDead != null) otherCs.OnDead(otherCs);
                    otherCs.DeleteHpAndHud();
                    Destroy(tempObj);
                    OtherplayerDic.Remove(ri.keyID);
                }
                else
                {
                    if (isDelete)
                    {
                        SkillBuffManager.GetInst().ClearBuffsFrom(otherCs);
                        if (otherCs.OnDead != null) otherCs.OnDead(otherCs);
                        otherCs.DeleteHpAndHud();
                        Destroy(tempObj);
                        OtherplayerDic.Remove(ri.keyID);
                    }
                }
            }
        }
        else
        {
            OtherplayerDic.Remove(ri.keyID);
        }
    }
    //删除场景里的活动建筑物
    void DeletBuild(GameObject tempObj, RoleInfo ri, bool isDelete = false)
    {
        if (tempObj != null)
        {
            Destroy(tempObj);
              
        }
    }
    /// <summary>
    /// 删除场景里的怪物
    /// </summary>
    /// <param name="tempObj"></param>
    /// <param name="ri"></param>
    /// <param name="isDelete"></param>
    void DeletMonst(GameObject tempObj, RoleInfo ri, bool isDelete = false)
    {
        if (tempObj != null)
        {
            CharacterState cs = tempObj.GetComponent<CharacterState>();
            if (cs != null)
            {
                if (!cs.isDie)
                {
                    if (SceneBaseManager.instance != null)
                    {
                        SceneBaseManager.instance.RemoveCs(cs);
                        if (cs.OnDead != null) cs.OnDead(cs);
                        cs.DeleteHpAndHud();
                        Destroy(tempObj);
                        MonsterDic.Remove(ri.keyID);
                    }
                }
                else
                {
                    if (isDelete)
                    {
                        SceneBaseManager.instance.RemoveCs(cs);
                        if (cs.OnDead != null) cs.OnDead(cs);
                        cs.DeleteHpAndHud();
                        Destroy(tempObj);
                        MonsterDic.Remove(ri.keyID);
                    }
                }
            }
        }
        else
        {
            MonsterDic.Remove(ri.keyID);
        }
    }
    /// <summary>
    /// 删除场景里的采集物
    /// </summary>
    /// <param name="tempObj"></param>
    /// <param name="ri"></param>
    /// <param name="isDelete"></param>
    void DeletCollect(GameObject tempObj, RoleInfo ri)
    {
        if (tempObj != null)
        {
            //删除采集物对象时候 删除采集物名字
            if (tempObj.GetComponent<TaskCollectPoint>() != null)
            {
                Destroy(tempObj.GetComponent<TaskCollectPoint>().collectNameObj);
            }
            Destroy(tempObj);
        }
        CollectDic.Remove(ri.keyID);
    }
    /// <summary>
    /// 删除Npc
    /// </summary>
    /// <param name="tempObj"></param>
    /// <param name="ri"></param>
    void DeletNpc(GameObject tempObj, RoleInfo ri)
    {
        if (tempObj != null)
        {
            //删除npc对象时候 删除npc名字 和 npc头顶气泡
            if (tempObj.GetComponent<TaskNPC>() != null)
            {
                Destroy(tempObj.GetComponent<TaskNPC>().npcNameObj);
                Destroy(tempObj.GetComponent<TaskNPC>().npcHeadQipaoObj);
            }
            Destroy(tempObj);
        }
        NpcDic.Remove(ri.keyID);
    }
    /// <summary>
    /// 删除复活点
    /// </summary>
    /// <param name="tempObj"></param>
    /// <param name="ri"></param>
    void DeleteRevioPos(GameObject tempObj, RoleInfo ri)
    {
        if (tempObj != null)
        {
            Destroy(tempObj);
        }
        ResurgenceDic.Remove(ri.keyID);
    }
    /// <summary>
    /// 删除传送点
    /// </summary>
    /// <param name="tempObj"></param>
    /// <param name="ri"></param>
    void DeleteTransfer(GameObject tempObj, RoleInfo ri)
    {
        if (tempObj != null)
        {
            Destroy(tempObj);
        }
        TransferDic.Remove(ri.keyID);
    }
    void DeletMinePos(GameObject tempObj, RoleInfo ri)
    {
        if (tempObj != null)
        {
            Destroy(tempObj);
        }
        MineDic.Remove(ri.keyID);
    }
  public  void DeleteNearElement(GameObject tempObj, RoleInfo ri, bool isDelete = false)
    {
        switch(ri.type)
        {

            //怪物，活动怪
            case 1:
            case 6:
                {
                    DeletMonst(tempObj, ri, isDelete );
                }
                break;
            //npc
            case 2:
                {
                    DeletNpc(tempObj,ri);
                }
                break;
            case 3:
                //传送点
                {
                    DeleteTransfer(tempObj, ri);
                }
                break;
            case 4:
                //复活点
                {
                    DeleteRevioPos(tempObj,ri);
                }
                break;
            case 5:
                //藏宝点
                {
                    DeletMinePos(tempObj, ri);
                }
                break;
            case 7:
                //采集物
                {
                    DeletCollect(tempObj, ri);
                }
                break;
            case 8:
            case 9:
                DeletBuild(tempObj, ri, isDelete);
                break;
        }
    }
    /// <summary>
    /// 删除附近物体 
    /// </summary>
    /// <param name="keyId"></param>
    /// <param name="isDelete"></param>
    public void DeleteOtherObject ( uint keyId , bool isDelete = false )
    {
        GameObject tempObj = null;
        if (playerData.GetInstance().NearRIarr.ContainsKey(keyId))
        {
            RoleInfo ri = playerData.GetInstance().NearRIarr[keyId];
            tempObj = ri.RoleObj;
            if(ri.accID == 0)//场景里除了其他玩家的其他元素
            {
                DeleteNearElement(tempObj, ri, isDelete);
            }
            else//其他玩家
            {
                DeleteOtherPlayer(tempObj, ri, isDelete);

            }
            playerData.GetInstance().NearRIarr.Remove(ri.keyID);
            ri = null;

        }
    }

    public void OtherObjectDamage ( uint attackKeyId,uint targetKeyId , int damageHp ,int skillID = -1 ,float baseVal = 0.0f )
    {
        GameObject targetObj = null;
        if ( MonsterDic.ContainsKey( targetKeyId ) )
        {
            MonsterDic.TryGetValue( targetKeyId , out targetObj );
            
        }
        else if (OtherplayerDic.ContainsKey(targetKeyId))
        {
            OtherplayerDic.TryGetValue( targetKeyId , out targetObj );
        }

        GameObject attackObj = null;

        if ( MonsterDic.ContainsKey( attackKeyId ) )
        {
            MonsterDic.TryGetValue( attackKeyId , out attackObj );

        }
        else if ( OtherplayerDic.ContainsKey( attackKeyId ) )
        {
            OtherplayerDic.TryGetValue( attackKeyId , out attackObj );
        }

        CharacterState tempCS = null;

        if ( targetObj != null )
        {
            tempCS = targetObj.GetComponent<CharacterState>();
        }

        if ( tempCS != null )
        {
            CharacterState attackerCS = null;
            if(attackObj != null) attackerCS = attackObj.GetComponent<CharacterState>();
            if ( damageHp > 0 )
            {
                tempCS.ChangeHp( damageHp , HUDType.DamageEnemy , true);
            }
            else
            {
                if ( attackKeyId > 0 && skillID == 0)
                {
                    tempCS.ChangeHp( 0 , HUDType.Miss , true);
                }
            }
        }

        if ( FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList.ContainsKey( skillID ) )
        {
            SkillNode skillNode = FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList [ skillID ];
          //  if ( attackObj != null )
            {
              //  if ( tempCS != null )
                {
                    if (attackObj != null)
                    {
                        CharacterState mAttackerCs = attackObj.GetComponent<CharacterState>();
                        tempCS.emission.PlayHitEffect(skillNode, tempCS, mAttackerCs);
                        tempCS.AddBuffManager(skillNode, mAttackerCs, mAttackerCs.CharData);
                    }
                    else
                    {
                        tempCS.AddNetSkillManager(skillNode);
                    }
                }
            }
        }
    }

    public void OtherObjectDead (uint keyId)
    {
        GameObject tempObj = null;
        RoleInfo ri = null;
        if (playerData.GetInstance().NearRIarr.ContainsKey(keyId))
        {
            ri = playerData.GetInstance().NearRIarr[keyId];
        }
        if(ri!=null)
        {
            tempObj = ri.RoleObj;
            if(ri.accID>0)
            {
               // playerData.GetInstance().NearRIarr.Remove(keyId);
             //   OtherplayerDic.Remove(keyId);
                CharacterState tempCS;
                if (tempObj != null)
                {
                    tempCS = tempObj.GetComponent<CharacterState>();
                    if (tempCS != null)
                    {
                        tempCS.isDie = true;
                        tempCS.pm.Die();
                       // tempCS.DeleteMonster();
                        // Destroy(tempObj);
                        // tempObj.GetComponent<CharacterState>().DeleteMonster();
                    }
                }
            }
            else
            {
               if( ri.type == 1||ri.type ==6)
                {
                    playerData.GetInstance().NearRIarr.Remove(keyId);
                    MonsterDic.Remove(keyId);
                    CharacterState tempCS;
                    if (tempObj != null)
                    {
                        tempCS = tempObj.GetComponent<CharacterState>();

                        if (tempCS != null)
                        {
                            if (tempCS.currentHp != 0)
                            {
                                //怪物死亡，血量先置0
                                tempCS.ChangeHp(0, HUDType.DamagePlayer, true);
                               
                            }
                            tempCS.isDie = true;
                            tempCS.pm.Die();
                            tempCS.DeleteMonster();
                        }
                    }
                }
            }
        }

       
    }

    public void MonsterAttack (uint attackKeyId,uint targetKeyId,int skillId)
    {
        if( MonsterDic.ContainsKey( attackKeyId ) )
        {
            Debug.Log(attackKeyId + "攻击");
            GameObject tempObj = null;
            if ( MonsterDic.TryGetValue( attackKeyId , out tempObj )&&tempObj!=null )
            {
                OtherPlayer monsterOP = tempObj.GetComponent<OtherPlayer>();
                if ( monsterOP != null )
                {
                    if ( monsterOP.navMA.enabled == false )
                    {
                        if ( tempObj.GetComponent<NetworkMonster>() == null )
                        {
                            tempObj.AddComponent<NetworkMonster>();
                        }

                        if ( OtherplayerDic.ContainsKey( targetKeyId ) )
                        {
                            GameObject targetObj = null;
                            if ( OtherplayerDic.TryGetValue( targetKeyId , out targetObj ) )
                            {
                                tempObj.GetComponent<NetworkMonster>().AttackObject( targetObj );
                            }
                        }
                        else if ( targetKeyId == playerData.GetInstance().selfData.keyId ||targetKeyId == 0)
                        {
                            tempObj.GetComponent<NetworkMonster>().AttackObject( CharacterManager.player );
                        }
                    }
                    else
                    {
                        Debug.LogError(attackKeyId + "没有停下来");
                       
                    }
                }
            }

        }
    }

    void RemoveMine ( uint keyId )
    {
        if ( MineDic.ContainsKey( keyId ) )
        {
            GameObject tempObj = null;
            if ( MineDic.TryGetValue( keyId , out tempObj ) )
            {
                Destroy( tempObj );
            }
            MineDic.Remove( keyId );
            if ( myOverMine != null )
            {
                myOverMine();
            }
        }
    }

    public void StartMine ( uint keyId )
    {
        NMA = CharacterManager.player.GetComponent<UnityEngine.AI.NavMeshAgent>();
        NMA.enabled = true;
        NMA.ResetPath();
        NMA.stoppingDistance = 0.1f;
        targetMineKeyId = keyId;
        NMA.SetDestination( MineDic [ keyId ].transform.position);
    }

    void LateUpdate ()
    {
        if (NMA != null)
        {
            if ( NMA.enabled )
            {
                if ( !NMA.pathPending )
                {
                    if ( Mathf.Abs( NMA.remainingDistance ) < 0.1f )
                    {
                        if ( oldPos == transform.position )
                        {
                            PlayPrepare();
                            RemoveMine( targetMineKeyId );
                            NMA.enabled = false;
                        }
                    }
                    else
                    {
                        transform.LookAt( NMA.steeringTarget );
                        CharacterManager.playerCS.pm.Run();
                    }
                }
            }

            oldPos = transform.position;
        }
    }

    void PlayPrepare ()
    {
        CharacterManager.playerCS.pm.ani.SetInteger( "Prepare" , 1 );
        CharacterManager.playerCS.pm.ani.SetInteger( "Speed" , 0 );
    }

    public void Clear ()
    {
        ClearDict(MonsterDic);
        ClearDict(OtherplayerDic);
        ClearDict(NpcDic);
        ClearDict(ResurgenceDic);
        ClearDict(MineDic);
        ClearDict(TransferDic);
    }

    void ClearDict ( Dictionary<long, GameObject> dict)
    {
        foreach(GameObject obj in dict.Values)
        {
            Destroy(obj);
        }

        dict.Clear();
    }

    void OnDestroy ()
    {
        if ( CharacterManager.player != null )
        {
            if(CharacterManager.player.GetComponent<CharacterState>().isNetworking)
            CharacterManager.player.GetComponent<CharacterState>().isNetworking = false;
        }
        
        instance = null;
    }
}
