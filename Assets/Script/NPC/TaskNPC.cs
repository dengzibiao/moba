/*
文件名（File Name）:   TaskNPC.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;
using UnityEngine.SceneManagement;

public class TaskNPC : MonoBehaviour
{
    //NPCID
    public long NPCID;
    public long NpcKey;
    float npcHeight;
    //主摄像机对象
    public float ratospeed = 1000f;
    private Camera camera;
    public Transform quanEffect;//脚底选中光圈特效
    Transform taskCanAccept;//头顶可接受任务特效
    Transform taskCanSucceed;//头顶可完成任务特效
    public GameObject headtipobj;
    public GameObject headQipaoObj;
    bool isShowQiPao = false;
    void Awake()
    {
        camera = Camera.main;
        //注解1
        Vector3 vec = GetComponentInChildren<SkinnedMeshRenderer>().bounds.size;
        transform.gameObject.AddComponent<BoxCollider>();
        transform.GetComponent<BoxCollider>().size = new Vector3(0.3f,vec.y,0.3f);
        transform.GetComponent<BoxCollider>().center = new Vector3(0, vec.y / 2, 0); ;
        //得到模型原始高度
        //float size_y = transform.GetComponent<BoxCollider>().bounds.size.y;
        float size_y = 0.8f;
        //得到模型缩放比例
        float scal_y = transform.localScale.y;
        //它们的乘积就是高度
        npcHeight = (size_y * scal_y);

        headtipobj = Resources.Load("Prefab/UIPanel/NpcHeadTip") as GameObject;
        headQipaoObj = Resources.Load("Prefab/UIPanel/NpcHeadQipao") as GameObject;
        if (npcHeadQipaoObj == null)
        {
            if (headQipaoObj != null)
            {
                npcHeadQipaoObj = Instantiate(headQipaoObj) as GameObject;
            }
            npcHeadQipaoObj.name = name + "_HeadTip";
            npcHeadQipaoObj.transform.parent = CharacterManager.instance.UIControl;
            npcHeadQipaoObj.SetActive(false);
            npcHeadQipaoObj.transform.localScale = Vector3.one;
        }
        if (npcNameObj == null)
        {
            if (headtipobj != null)
            {
                npcNameObj = GameObject.Instantiate(headtipobj) as GameObject;
            }
            npcNameObj.name = name + "_Name";
            npcNameObj.transform.parent = CharacterManager.instance.UIControl;
            npcNameObj.gameObject.SetActive(false);
            npcNameObj.transform.localScale = Vector3.one;
            NPCManager.GetSingle().AddNpcNameModel(NpcKey,npcNameObj);
        }
    }
    public  List<string> contonts = new List<string>();
    void Start()
    {
        //NPC创建时设置任务状态
        TaskManager.Single().ChangeNpcState(NPCID);
        NPCNode npcNode = FSDataNodeTable<NPCNode>.GetSingleton().FindDataByType(NPCID);
        npcName = npcNode.npcname;//npc名字
        quanEffect = transform.Find("jiaodgq");
        taskCanAccept = transform.Find("toudjstb");
        taskCanSucceed = transform.Find("toudwctb");
        selfplayer = CharacterManager.player;

        for (int i = 0; i < npcNode.info.Length; i++)
        {
            if (string.IsNullOrEmpty(npcNode.info[i]))
            {
                continue;
            }
            contonts.Add(npcNode.info[i]);
        }
        if (contonts.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, contonts.Count);
            strheadtip = contonts[index];
        }
    }

    Vector3 getNPCHeadPos()
    {
        //NPC对象 头顶上的任务状态图标
        //Vector2 headpos;
        //得到NPC头顶在3D世界中的坐标
        //默认NPC坐标点在脚底下，所以这里加上npcHeight它模型的高度即可
        Vector3 worldPosition = new Vector3(transform.position.x, transform.position.y + npcHeight + offset, transform.position.z);
        ////根据NPC头顶的3D坐标换算成它在2D屏幕中的坐标
        //headpos = camera.WorldToScreenPoint(worldPosition);
        ////得到真实NPC头顶的2D坐标
        //headpos = UICamera.mainCamera.ScreenToWorldPoint(headpos);

        return BattleUtil.WorldToScreenPoint(worldPosition);
    }

    public float rotateSpeed = 10f;

    void FixedUpdate()
    {
        ShowNpcState();
        ShowHeadTip();
        ShowNpcName();
        if (CharacterManager.player!=null&& quanEffect !=null&& Vector3.Distance(CharacterManager.player.transform.position, transform.position) > 1f && quanEffect.gameObject.activeSelf)
        {
            quanEffect.gameObject.SetActive(false);
        }
        if (CharacterManager.player != null  && Vector3.Distance(CharacterManager.player.transform.position, transform.position) < 2f )
        {
            isShowQiPao = true;
        }
        else
        {
            isShowQiPao = false;
        }
    }

    

    //操作任务
    private bool isTaskOperation;
    private GameObject selfplayer;
    private bool navmeshenable = false;
    //public delegate void ReachplaceNpcEvent(int npcid);
    //private ReachplaceNpcEvent reachplaceNpcEvent;
    //点击NPC触发
    void OnMouseDown()
    {

        if ((UICamera.selectedObject != null && UICamera.selectedObject.transform.name ==  "UI Root" ) || (UICamera.hoveredObject != null && UICamera.hoveredObject.transform.name ==  "UI Root")|| (UICamera.selectedObject != null && UICamera.selectedObject.transform.name == "UI Root(Clone)")|| (UICamera.hoveredObject != null && UICamera.hoveredObject.transform.name == "UI Root(Clone)"))
        {
            if (!UICamera.isOverUI)
            {
                //点击npc显示光圈
                if (quanEffect!=null)
                {
                    quanEffect.gameObject.SetActive(true);
                    Vector3 tempV = transform.position;
                    quanEffect.position = tempV;
                }

                //如果当前npc上没有任务 就进行闲聊 不触发点击npc协议
                if (!TaskManager.NpcTaskStateDic.ContainsKey(NPCID))
                {
                    TaskOperation.Single().MoveToNpc(NPCID, TaskOperation.MoveToNpcType.Smalltalk);
                    return;
                }
                //设置当前操作npc
                TaskOperation.Single().taskNpc = this;
                TaskOperation.Single().currentTaskNpcID = NPCID;
                if (TaskManager.NpcTaskListDic.ContainsKey(NPCID))
                {
                    //暂时一个npc下有一个可接收的任务 或者 可完成的任务
                    int taskID = 0;
                    foreach (int key in TaskManager.NpcTaskListDic[NPCID].Keys)
                    {
                        taskID = key;
                    }
                    //npc上有任务 但是任务状态是未完成 也进行闲聊
                    if (TaskManager.NpcTaskListDic[NPCID][taskID].taskProgress == TaskProgress.Accept)
                    {
                        TaskOperation.Single().MoveToNpc(NPCID, TaskOperation.MoveToNpcType.Smalltalk);
                        return;
                    }
                    if (FSDataNodeTable<TaskDataNode>.GetSingleton().DataNodeList.ContainsKey(taskID))
                    {
                        if (FSDataNodeTable<TaskDataNode>.GetSingleton().DataNodeList[taskID].Level>playerData.GetInstance().selfData.level)
                        {
                         Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond,false, "需战队等级达到" + FSDataNodeTable<TaskDataNode>.GetSingleton().DataNodeList[taskID].Level + "级才可领取");
                            return;
                        }
                    }
                    TaskOperation.Single().MoveToNpc(NPCID,TaskOperation.MoveToNpcType.JuQingTaskZhuiZhong);
                    //ClientSendDataMgr.GetSingle().GetTaskSend().ClickNpc(NPCID, taskID, TaskManager.NpcTaskListDic[NPCID][taskID].parm0);
                }

                //在此判定是否该NPC身上是否有任务 如果没有不进行任何操作
                //List<TaskItem> tasklist = TaskManager.Single().GetNPCTaskList(NPCID);
                //if (tasklist.Count > 0)
                //{
                //    TaskOperation.Single().MoveToNpc(NPCID, TaskOperation.MoveToNpcType.OpenTaskList);
                //}
                //else//闲聊
                //{
                //    TaskOperation.Single().MoveToNpc(NPCID, TaskOperation.MoveToNpcType.Smalltalk);
                //}
            }
        }
    }

    private bool stateActive = false;

    private UISprite statesprite;
    public GameObject npcHeadQipaoObj;
    public GameObject npcNameObj;
    private float offset = 0.08f;

    //设置当前NPC任务显示状态 当前只有可接 可完成 两种状态
    public void SetTaskState(TaskProgress taskProgress)
    {
        string taskstatesprite;

        switch (taskProgress)
        {
            case TaskProgress.NoAccept:
                taskstatesprite = "tanhao-jin";
                break;
            case TaskProgress.Complete:
                taskstatesprite = "wenhao-jin";
                break;
            case TaskProgress.Accept:
            case TaskProgress.CantAccept:
            case TaskProgress.Reward:
                taskstatesprite = "";
                break;
            default:
                taskstatesprite = "";
                break;
        }
        //if (statesprite == null)
        //{
        //    statesprite = GameObject.Instantiate(CharacterManager.instance.stateobj).GetComponent<UISprite>();
        //    statesprite.name = name + "_State";
        //    statesprite.transform.parent = CharacterManager.instance.UIControl;
        //}

        //stateActive = !string.IsNullOrEmpty(taskstatesprite);
        //statesprite.gameObject.SetActive(stateActive);
        //statesprite.spriteName = taskstatesprite;
        //statesprite.MakePixelPerfect();

        if (taskProgress == TaskProgress.NoAccept)
        {
            transform.Find("toudjstb").gameObject.SetActive(true);
            transform.Find("toudwctb").gameObject.SetActive(false);
            Vector3 tempV = transform.position;
            tempV.y += 1f;
            transform.Find("toudjstb").position = tempV;
        }
        else if (taskProgress == TaskProgress.Complete)
        {
            transform.Find("toudwctb").gameObject.SetActive(true);
            transform.Find("toudjstb").gameObject.SetActive(false);
            Vector3 tempV = transform.position;
            tempV.y += 1f;
            transform.Find("toudwctb").position = tempV;
        }
        else
        {
            transform.Find("toudjstb").gameObject.SetActive(false);
            transform.Find("toudwctb").gameObject.SetActive(false);
        }
        SetNpcStateEffectRotation();
    }
    //使npc状态特效始终正面朝向屏幕
    public void SetNpcStateEffectRotation()
    {
        transform.Find("toudjstb").rotation = Quaternion.Euler(0f, transform.rotation.y + 180f, 0);
        transform.Find("toudwctb").rotation = Quaternion.Euler(0f, transform.rotation.y + 180f, 0);
    }
    private string npcName = "";
    private string strheadtip = "";
    private float showtime = 5f;
    //NPC头部显示文本
    public void ShowHeadTip()
    {
        
        showtime += Time.deltaTime;
        if (showtime > 7)
        {
            if(isShowQiPao) npcHeadQipaoObj.SetActive(true);
            npcHeadQipaoObj.transform.FindComponent<UILabel>("Content").text = strheadtip;
            //根据文字长度 动态修改气泡的偏移位置
            Vector3 temPos = npcHeadQipaoObj.transform.FindComponent<UILabel>("Content").transform.localPosition;
            temPos.x = -(npcHeadQipaoObj.transform.FindComponent<UILabel>("Content").width / 2.0f);
            npcHeadQipaoObj.transform.FindComponent<UILabel>("Content").transform.localPosition = temPos;
            //随机文本 显示动画
            Vector3 tempV = getNPCHeadPos();
            tempV.y += 0.15f;
            npcHeadQipaoObj.transform.position = tempV;
        }

        //显示两秒隐藏
        if (showtime > 9)
        {
            if (contonts.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, contonts.Count);
                strheadtip = contonts[index];
            }
            npcHeadQipaoObj.SetActive(false);
            showtime = 0;
        }
    }
    private void ShowNpcName()
    {
        npcNameObj.gameObject.SetActive(true);
        npcNameObj.GetComponent<UILabel>().text = npcName;
        //npcNameObj.color = new Color(0, 100, 0);
        Vector3 tempV = getNPCHeadPos();
        tempV.y += 0.01f;
        npcNameObj.transform.position = tempV;
    }
    public void ShowNpcState()
    {
        if (stateActive)
        {
            statesprite.transform.position = getNPCHeadPos();
        }
    }
    void onDestroy()
    {
        if (npcNameObj != null)
        {
            Destroy(npcNameObj);
        }
    }
}
