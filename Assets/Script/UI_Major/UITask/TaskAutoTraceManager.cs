using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using UnityEngine.SceneManagement;

public enum TaskMoveTarget
{
    Null,
    MoveToNpc,
    MoveToMonsterPos,
    MoveToCollectPos,
    MoveToMonsterDropPis,
}
/// <summary>
///  任务自动追踪控制（移动到npc 采集点 怪物点等）
/// </summary>
public class TaskAutoTraceManager : MonoBehaviour
{
    public static TaskAutoTraceManager _instance;
    //主摄像机对象
    public float ratospeed = 1000f;
    public bool isTaskOperation;
    private GameObject selfplayer;
    private bool navmeshenable = false;
    public Vector3 targetPosition;
    private Vector3 transferPos = new Vector3(0.18f,11.7f,13.05f);//穿送点位置  暂时测试
    private TaskItem taskItem;
    private long npcID;
    public bool isTaskAutoPathfinding = false;//是否是任务自动寻路，要给后端同步位置
    private float tempTime;
    //public bool isTaskAutoTraceToTransfer = false;//任务自动寻到到传送门
    //public List<Vector3> posList = new List<Vector3>();
    #region GetSet
    public bool IsTaskOperation
    {
        get
        {
            return isTaskOperation;
        }

        set
        {
            isTaskOperation = value;
        }
    }

    public GameObject Selfplayer
    {
        get
        {
            return selfplayer;
        }

        set
        {
            selfplayer = value;
        }
    }

    public bool Navmeshenable
    {
        get
        {
            return navmeshenable;
        }

        set
        {
            navmeshenable = value;
        }
    }
    #endregion
    void Awake()
    {
        _instance = this;
    }
    void Start()
    {
        Selfplayer = CharacterManager.player;
        if (TaskManager.Single().isTaskAutoTraceToTransfer && TaskManager.Single().taskMoveType != TaskMoveTarget.Null && TaskManager.Single().taskAutoTraceID != 0)
        {
            TaskManager.Single().isTaskAutoTraceToTransfer = false;
            //MoveToTargetPosition(TaskManager.Single().taskAutoTraceID, TaskManager.Single().taskMoveType);
            StartCoroutine(Test());
        }
    }
    IEnumerator Test()
    {
        yield return new WaitForSeconds(0.2f);
        MoveToTargetPosition(TaskManager.Single().taskAutoTraceID, TaskManager.Single().taskMoveType,TaskManager.Single().maiID);
        //Debug.LogError(TaskManager.Single().taskAutoTraceID + "..." + TaskManager.Single().taskMoveType);
    }
    void Update()
    {
        if (selfplayer == null)
        {
            Selfplayer = CharacterManager.player;
        }
        if (isTaskOperation)
        {
            //在追踪任务的过程中，移动遥感 暂停寻路，并将隐藏的面板重新显示
            if (Mathf.Abs(EasyTouch.instance.upPosition.x) > 0.1f || Mathf.Abs(EasyTouch.instance.upPosition.y) > 0.1f)
            {
                TaskManager.Single().isTaskAutoTraceToTransfer = false;
                //Selfplayer.GetComponent<NavMeshAgent>().enabled = Navmeshenable;
                isTaskOperation = false;
                CharacterManager.instance.SwitchAutoMode(false);
                isTaskAutoPathfinding = false;
                PlayerAutoPathfindingStop();
            }
        }
    }

    /// <summary>
    /// 打开界面或者是在野外点击技能 如果在任务自动寻路就暂停自动寻路
    /// </summary>
    /// <returns></returns>
    public void StopTaskAutoFindWay()
    {
        if (isTaskAutoPathfinding)
        {
            TaskManager.Single().isTaskAutoTraceToTransfer = false;
            //Selfplayer.GetComponent<NavMeshAgent>().enabled = Navmeshenable;
            isTaskOperation = false;
            CharacterManager.instance.SwitchAutoMode(false);
            isTaskAutoPathfinding = false;
            PlayerAutoPathfindingStop();
        }
       
    }
    /// <summary>
    /// 自动寻路停止告诉服务器
    /// </summary>
    public void PlayerAutoPathfindingStop()
    {
        //   if (CharacterManager.player != null && !CharacterManager.instance.shouldMove)
        //  {
        //玩家停下来要同步给服务器位置
        // ClientSendDataMgr.GetSingle().GetWalkSend().SendSelfPos(CharacterManager.player.transform.localPosition);
        //  CharacterManager.instance.PlayerStop();
        //  }//点击对话框的时候不用发同步信息
        tempTime = 0;
        UITaskEffectPanel.instance.SetZDXLEffect(false);
    }
    void FixedUpdate()
    {
        //任务自动寻路过程中 0.3s给服务器同步一下位置信息
        if (isTaskAutoPathfinding)
        {
            CheckMapInfo();
            tempTime += Time.deltaTime;
            if (tempTime > 0.3f)
            {
                tempTime = 0f;

                 ClientSendDataMgr.GetSingle().GetWalkSend().SendSelfPos();

            }
        }   
        if (isTaskOperation && Selfplayer!=null)
        {
            if (Vector2.Distance(new Vector2(Selfplayer.transform.position.x, Selfplayer.transform.position.z), new Vector2(targetPosition.x, targetPosition.z)) < 1f
                && !TaskManager.Single().isTaskAutoTraceToTransfer)
            {
                //Selfplayer.GetComponent<NavMeshAgent>().enabled = Navmeshenable;
                isTaskOperation = false;
                if (TaskManager.Single().reachplaceNpcEvent != null)
                {
                    TaskManager.Single().reachplaceNpcEvent(npcID);
                    //移动地目标是npc,移动到位置让npc朝向玩家
                    GameObject npcObj = NPCManager.GetSingle().GetNpcObj(npcID);
                    if (npcObj != null)
                    {
                        Vector3 tempVector3 = Selfplayer.transform.position;
                        tempVector3.y = npcObj.transform.position.y;
                        npcObj.transform.LookAt(tempVector3);
                        TaskNPC taskNpc = npcObj.GetComponent<TaskNPC>();
                        if (taskNpc!=null)
                        {
                            if (taskNpc.quanEffect!=null)
                            {
                                taskNpc.quanEffect.gameObject.SetActive(true);
                                taskNpc.SetNpcStateEffectRotation();
                            }
                        }
                    }
                    TaskManager.Single().taskAutoTraceID = 0;
                    TaskManager.Single().taskMoveType = TaskMoveTarget.Null;
                    TaskManager.Single().maiID = 0;
                    TaskManager.Single().reachplaceNpcEvent = null;
                }
                //if (TaskManager.Single().reachplaceCollectPointEvent!=null)
                //{
                //    TaskManager.Single().reachplaceCollectPointEvent();
                //    TaskManager.Single().taskAutoTraceID = 0;
                //    TaskManager.Single().taskMoveType = TaskMoveTarget.Null;
                //    TaskManager.Single().maiID = 0;
                //    TaskManager.Single().reachplaceCollectPointEvent = null;
                //}
                //if (TaskManager.Single().reachplaceMonsterDropEvent !=null)
                //{
                //    TaskManager.Single().reachplaceMonsterDropEvent();
                //    TaskManager.Single().taskAutoTraceID = 0;
                //    TaskManager.Single().taskMoveType = TaskMoveTarget.Null;
                //    TaskManager.Single().maiID = 0;
                //    TaskManager.Single().reachplaceCollectPointEvent = null;
                //}
                CharacterManager.instance.SwitchAutoMode(false);
                isTaskAutoPathfinding = false;
                PlayerAutoPathfindingStop();
            }
            else if(Vector2.Distance(new Vector2(Selfplayer.transform.position.x, Selfplayer.transform.position.z), new Vector2(targetPosition.x, targetPosition.z)) < 0.5f
                && TaskManager.Single().isTaskAutoTraceToTransfer)
            {
                //Selfplayer.GetComponent<NavMeshAgent>().enabled = Navmeshenable;
                isTaskOperation = false;
                CharacterManager.instance.SwitchAutoMode(false);
                isTaskAutoPathfinding = false;
                PlayerAutoPathfindingStop();
                if (TaskManager.Single().reachplaceTransferEvent != null)
                {
                    //TaskManager.Single().reachplaceTransferEvent();
                }
                TaskManager.Single().reachplaceTransferEvent = null;
            }
            else
            {
                Selfplayer.GetComponent<PlayerMotion>().Move(targetPosition);
            }
        }
    }

    void CheckMapInfo()
    {
        if (playerData.GetInstance().selfData.oldMapID != playerData.GetInstance().selfData.mapID)
        {
            Dictionary<long, MapInfoNode> tempMapInfo = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList;
            if (tempMapInfo != null)
            {
                foreach (MapInfoNode min in tempMapInfo.Values)
                {
                    playerData.GetInstance().selfData.oldMapID = (int)min.key;
                    //  playerData.GetInstance().selfData.mapID = min.key;

                    if (min.MapName == SceneManager.GetActiveScene().name)
                    {
                        RoleInfo tempRI = new RoleInfo();
                        Vector3 pos = CharacterManager.player.transform.position;
                        tempRI.mapID = min.key;
                        tempRI.keyID = playerData.GetInstance().selfData.accountId;
                        tempRI.accID = playerData.GetInstance().selfData.accountId;
                        tempRI.playID = playerData.GetInstance().selfData.playerId;
                        tempRI.roleID = playerData.GetInstance().selfData.heroId;
                        tempRI.posX = pos.x;
                        tempRI.posY = pos.y;
                        tempRI.posZ = pos.z;
                        tempRI.name = playerData.GetInstance().selfData.playeName;
                        if (CharacterManager.player.transform.GetComponent<SetMainHeroName>() == null)
                            CharacterManager.player.transform.gameObject.AddComponent<SetMainHeroName>();
                        ClientSendDataMgr.GetSingle().GetWalkSend().SendInitializePosInfo(tempRI);
                        return;

                    }
                }
            }
        }
    }

    //设置人物移动到NPC后执行事件
    public void SetReachplaceNpcEvent(ReachplaceNpcEvent _reachplaceNpcEvent)
    {
        //MoveToNpc();
        TaskManager.Single().reachplaceNpcEvent = _reachplaceNpcEvent;
    }
    //设置任务移动到传送点后执行事件
    public void SetReachplaceTransferEvent(ReachplaceTransferEvent _reachplaceTransferEvent)
    {
        TaskManager.Single().reachplaceTransferEvent = _reachplaceTransferEvent;
    }
    ////设置任务移动到采集点后执行事件
    //public void SetReachplaceCollectPointEvent(ReachplaceCollectPointEvent _reachplaceCollectPointEvent)
    //{
    //    TaskManager.Single().reachplaceCollectPointEvent = _reachplaceCollectPointEvent;
    //}
    ////设置任务移动到怪物点后执行事件
    //public void SetReachplaceMonsterEvent(ReachplaceMonsterEvent _reachplaceMonsterEvent)
    //{
    //    TaskManager.Single().reachplaceMonsterEvent = _reachplaceMonsterEvent;
    //}
    ////设置任务移动到怪物掉落物后执行事件
    //public void SetReachplaceMonsterDropEvent(ReachplaceMonsterDropEvent _reachplaceMonsterDropEvent)
    //{
    //    TaskManager.Single().reachplaceMonsterDropEvent = _reachplaceMonsterDropEvent;
    //}
    public void StartMove()
    {
        if (isTaskOperation) return;//避免重复点击
        //寻路至NPC
        isTaskOperation = true;
        selfplayer = CharacterManager.player;
        selfplayer.GetComponent<UnityEngine.AI.NavMeshAgent>().acceleration = 15f;
        selfplayer.GetComponent<UnityEngine.AI.NavMeshAgent>().angularSpeed = ratospeed;
        navmeshenable = selfplayer.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled;
        selfplayer.GetComponent<PlayerMotion>().isAutoMode = true;
        if (targetPosition != null)
        {
            selfplayer.transform.forward = targetPosition - selfplayer.transform.position;
            selfplayer.transform.localEulerAngles = new Vector3(0, selfplayer.transform.localEulerAngles.y, 0);
        }
        isTaskAutoPathfinding = true;
        if (Vector2.Distance(new Vector2(Selfplayer.transform.position.x, Selfplayer.transform.position.z), new Vector2(targetPosition.x, targetPosition.z)) > 1f)
        {
            UITaskEffectPanel.instance.SetZDXLEffect(true);
        }
    }

    private void MoveToTransferEvent()
    {
        if (TaskManager.Single().isTaskAutoTraceToTransfer)
        {
            ClientSendDataMgr.GetSingle().GetBattleSend().SendGetHerosBattleAttr(Globe.fightHero);
            if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
            {
                ClientSendDataMgr.GetSingle().GetLoginSend().SendChengeScene(20000, 20100);
            }
            else if (Singleton<SceneManage>.Instance.Current == EnumSceneID.LGhuangyuan)
            {
                ClientSendDataMgr.GetSingle().GetLoginSend().SendChengeScene(20100,20000);
            }
            
        }
    }
    #region 查找追踪点变量信息
    NPCNode npcNode;
    MapInfoNode mapInfoNode;
    ScenceElementFileIndexTableNode sEFIndexNode;
    CollectNode collectNode;
    int mapId;//所去地点场景id
    string mapName;
    int mapInfo;
    string fileName = "";

    string currentSceneName = "";//当前场景名字
    int currentSceneMapid = 0;//当前场景的场景id
    long toSceneTransID = 0;//当前场景到目的场景的传送id
    #endregion
  
    /// <summary>
    /// 任务移动接口
    /// </summary>
    /// <param name="id">npcID 或 怪物id 或采集点id 或传送门id</param>
    /// <param name="moveType"></param>
    /// <param name="mapID"></param>
    public void MoveToTargetPosition(long id,TaskMoveTarget moveType,int mapID = 0)
    {
        Clear();
        TaskManager.Single().taskAutoTraceID = id;
        TaskManager.Single().taskMoveType = moveType;
        TaskManager.Single().maiID = mapID;
        //通过这个id，查表 一步一步找到目标位置
        if (moveType == TaskMoveTarget.MoveToNpc) //npc
        {

            //先跳转到npc场景，然后在寻找npc位置，然后移动到npc位置
            if (FSDataNodeTable<NPCNode>.GetSingleton().DataNodeList.ContainsKey(id))
            {
                npcID = id;
                npcNode = FSDataNodeTable<NPCNode>.GetSingleton().DataNodeList[id];
                mapId = npcNode.mapid;
                if (FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.ContainsKey(mapId))
                {
                    mapInfoNode = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList[mapId];//场景属性表
                    mapName = mapInfoNode.MapName;

                    mapInfo = mapInfoNode.map_info;
                    if (FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList.ContainsKey(mapInfo))
                    {
                        sEFIndexNode = FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList[mapInfo];//场景元素索引表
                        fileName = sEFIndexNode.filename;
                    }
                    //直接遍历场景元素表 去找位置了
                    TaskManager.Single().posList.Clear();
                    if (fileName == "")
                    {
                        return;
                    }
                    foreach (SceneMapNode mapNode in FSDataNodeTable<SceneMapNode>.GetSingleton().DataNodeLists[fileName].Values)
                    {
                        if (mapNode.type_id == (long)id)
                        {
                            TaskManager.Single().posList.Add(mapNode.pos);
                        }
                    }
                    FindTargetPosition();

                }
            }
        }
        else if (moveType == TaskMoveTarget.MoveToCollectPos)//采集点
        {
            //先跳转到采集物所在场景，然后在再寻找采集物位置，然后移动到采集物位置
            if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList.ContainsKey(id))
            {
                collectNode = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[id];
                mapId = collectNode.mapid;
                if (FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.ContainsKey(mapId))
                {
                    mapInfoNode = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList[mapId];//场景属性表
                    mapName = mapInfoNode.MapName;

                    mapInfo = mapInfoNode.map_info;
                    if (FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList.ContainsKey(mapInfo))
                    {
                        sEFIndexNode = FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList[mapInfo];//场景元素索引表
                        fileName = sEFIndexNode.filename;
                    }
                    //直接遍历场景元素表 去找位置了
                    TaskManager.Single().posList.Clear();
                    if (fileName == "")
                    {
                        return;
                    }
                    foreach (SceneMapNode mapNode in FSDataNodeTable<SceneMapNode>.GetSingleton().DataNodeLists[fileName].Values)
                    {
                        if (mapNode.type_id == (long)id)
                        {
                            TaskManager.Single().posList.Add(mapNode.pos);
                        }
                    }
                    FindTargetPosition();

                }
            }
        }
        else if (moveType == TaskMoveTarget.MoveToMonsterDropPis)
        {
            //怪物掉落物 传过来的采采集表的id，然后通过采集表找到怪物id 和 mapId ---确定怪物的位置
            //先跳转到怪物掉落物地点，然后在寻找怪物掉落点位置，然后移动到该位置
            if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList.ContainsKey(id))
            {
                collectNode = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[id];
                long monsterID = collectNode.monsterid;
                mapId = collectNode.mapid;
                if (FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.ContainsKey(mapId))
                {
                    mapInfoNode = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList[mapId];//场景属性表
                    mapName = mapInfoNode.MapName;

                    mapInfo = mapInfoNode.map_info;
                    if (FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList.ContainsKey(mapInfo))
                    {
                        sEFIndexNode = FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList[mapInfo];//场景元素索引表
                        fileName = sEFIndexNode.filename;
                    }
                    //直接遍历场景元素表 去找位置了
                    TaskManager.Single().posList.Clear();
                    if (fileName == "")
                    {
                        return;
                    }
                    foreach (SceneMapNode mapNode in FSDataNodeTable<SceneMapNode>.GetSingleton().DataNodeLists[fileName].Values)
                    {
                        if (mapNode.type_id == monsterID)//怪物掉落物 传进来的id是采集表id，通过采集表id在采集表中得到怪物id，然后通过怪物id在相应的元素表中找到怪物位置
                        {
                            TaskManager.Single().posList.Add(mapNode.pos);
                        }
                    }
                    FindTargetPosition();
                }
            }
        }
        else if (moveType == TaskMoveTarget.MoveToMonsterPos)
        {
            //先跳转到怪物地点，然后在寻找怪物位置，然后移动到该位置
            long monsterID = id;
            mapId = mapID;// 去野外杀怪  告诉我野外mapid
            if (FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.ContainsKey(mapId))
            {
                mapInfoNode = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList[mapId];//场景属性表
                mapName = mapInfoNode.MapName;

                mapInfo = mapInfoNode.map_info;
                if (FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList.ContainsKey(mapInfo))
                {
                    sEFIndexNode = FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList[mapInfo];//场景元素索引表
                    fileName = sEFIndexNode.filename;
                }
                //直接遍历场景元素表 去找位置了
                TaskManager.Single().posList.Clear();
                if (fileName == "")
                {
                    return;
                }
                foreach (SceneMapNode mapNode in FSDataNodeTable<SceneMapNode>.GetSingleton().DataNodeLists[fileName].Values)
                {
                    if (mapNode.type_id == monsterID)//怪物掉落物 传进来的id是采集表id，通过采集表id在采集表中得到怪物id，然后通过怪物id在相应的元素表中找到怪物位置
                    {
                        TaskManager.Single().posList.Add(mapNode.pos);
                    }
                }
                FindTargetPosition();
            }
        }

        StartMove();
    }

    private void FindTargetPosition()
    {
        //不在npc所在场景要用传送门跳转
        if (SceneManager.GetActiveScene().name != mapName)
        {

            currentSceneName = SceneManager.GetActiveScene().name;
            currentSceneMapid = 0;//当前场景的场景id
            toSceneTransID = 0;//当前场景到目的场景的传送id
            foreach (MapInfoNode mapInfo1 in FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.Values)
            {
                if (mapInfo1.MapName == currentSceneName)
                {
                    currentSceneMapid = (int)mapInfo1.key;
                    break;
                }
            }

            foreach (RouteNode transferNode in FSDataNodeTable<RouteNode>.GetSingleton().DataNodeList.Values)
            {
                if (transferNode.from_mapid == currentSceneMapid && transferNode.to_mapid == mapId)
                {
                    toSceneTransID = transferNode.from;
                }
            }

            if (FSDataNodeTable<TransferTableNode>.GetSingleton().DataNodeList.ContainsKey(toSceneTransID))
            {
                if (FSDataNodeTable<TransferTableNode>.GetSingleton().DataNodeList[toSceneTransID].scene == currentSceneMapid)//再验证一下
                {
                    transferPos = FSDataNodeTable<TransferTableNode>.GetSingleton().DataNodeList[toSceneTransID].pos;
                }
            }

            //自动跑到传送门位置进行传送 然后自动跑到相应位置
            SetReachplaceTransferEvent(MoveToTransferEvent);
            targetPosition = transferPos;
            TaskManager.Single().isTaskAutoTraceToTransfer = true;
        }
        else
        {
            if (TaskManager.Single().posList.Count > 1)
            {
                int index = Random.Range(0, TaskManager.Single().posList.Count);
                targetPosition = TaskManager.Single().posList[index];
            }
            else if (TaskManager.Single().posList.Count == 1)
            {
                targetPosition = TaskManager.Single().posList[0];
            }
            TaskManager.Single().isTaskAutoTraceToTransfer = false;
        }
    }
    void Clear()
    {
        npcNode = null;
        mapInfoNode = null;
        sEFIndexNode = null;
        collectNode = null;
        mapId = 0;
        mapName = "";
        fileName = "";
        currentSceneName = "";
        currentSceneMapid = 0;
        toSceneTransID = 0;
    }
    /// <summary>
    /// 跳转场景后重新设置任务追踪点
    /// </summary> 
    public void SetTargetPosition()
    {
        if (TaskManager.Single().posList.Count > 1)
        {
            int index = Random.Range(0, TaskManager.Single().posList.Count);
            targetPosition = TaskManager.Single().posList[index];
        }
        else if (TaskManager.Single().posList.Count == 1)
        {
            targetPosition = TaskManager.Single().posList[0];
        }
    }
}
