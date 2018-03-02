using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class TaskCollectPoint : MonoBehaviour
{

    public long collectID;//采集物id  给服务器发的id
    float collectHeight;
    private Camera camera;
    public GameObject collectNameObj;
    private float offset = 0.08f;
    private string collectName = "";
    public GameObject headtipobj;
    public bool rewardCanCollect = false;//悬赏可以采集
    public bool mainTaskCanCollect = false;//主线可以采集 只要满足一个就可以采集
    Transform nameTrs;
    void Awake()
    {
        camera = Camera.main;
        //注解1
        //Vector3 vec = GetComponentInChildren<SkinnedMeshRenderer>().bounds.size;
        transform.gameObject.AddComponent<BoxCollider>();
        transform.GetComponent<BoxCollider>().size = new Vector3(0.5f, 0.5f, 0.5f);
        transform.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0.1f);
        nameTrs = transform.Find("name");
        //得到模型原始高度
        float size_y = transform.GetComponent<BoxCollider>().bounds.size.y;
        //得到模型缩放比例
        float scal_y = transform.localScale.y;
        //它们的乘积就是高度
        collectHeight = (size_y * scal_y);
        headtipobj = Resources.Load("Prefab/UIPanel/NpcHeadTip") as GameObject;
        if (collectNameObj == null)
        {
            if (headtipobj != null)
            {
                collectNameObj = GameObject.Instantiate(headtipobj) as GameObject;
            }
            collectNameObj.name = name + "_Name";
            collectNameObj.transform.parent = CharacterManager.instance.UIControl;
            collectNameObj.gameObject.SetActive(false);
            collectNameObj.transform.localScale = Vector3.one;
        }
    }
    // Use this for initialization
    void Start()
    {
        if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList.ContainsKey(collectID))
        {
            collectName = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[collectID].resourcename;
        }
    }
    void FixedUpdate()
    {
        ShowCollectName();

    }
    // Update is called once per frame
    void Update()
    {

    }

    Vector3 getCollectHeadPos()
    {
        //采集物对象 头顶上的任务状态图标
        Vector2 headpos;
        //得到采集物头顶在3D世界中的坐标
        //默认采集物坐标点在脚底下，所以这里加上collectHeight它模型的高度即可
        Vector3 worldPosition = new Vector3(transform.position.x, transform.position.y + collectHeight + offset, transform.position.z);
        if (nameTrs!=null)
        {
            worldPosition = nameTrs.position;
        }
        //根据采集物头顶的3D坐标换算成它在2D屏幕中的坐标
        headpos = camera.WorldToScreenPoint(worldPosition);
        //得到真实采集物头顶的2D坐标
        headpos = UICamera.mainCamera.ScreenToWorldPoint(headpos);
        return headpos;
    }
    //点击采集物
    void OnMouseDown()
    {
        //if ((UICamera.selectedObject != null && UICamera.selectedObject.transform.name == "UI Root") || (UICamera.hoveredObject != null && UICamera.hoveredObject.transform.name == "UI Root") || (UICamera.selectedObject != null && UICamera.selectedObject.transform.name == "UI Root(Clone)") || (UICamera.hoveredObject != null && UICamera.hoveredObject.transform.name == "UI Root(Clone)"))
        //{
        if(UICamera.hoveredObject ==null|| UICamera.hoveredObject&& UICamera.hoveredObject.gameObject.name== "UI Root"|| UICamera.hoveredObject && UICamera.hoveredObject.gameObject.name == "UI Root(Clone)")
        {
            EveryTaskData data = playerData.GetInstance().taskDataList.itList.Find(x => (TaskProgress)x.state == TaskProgress.Accept);
            if (data != null)
            {
                if (data.type != (int)TaskType.Collect)
                {
                    data = null;
                }
            }
            if (TaskManager.Single().TaskToCaijiDic.Count > 0 || data != null)
            {
               
                if (TaskManager.Single().isCollecting)
                {
                    Debug.Log("正在采集请稍等");
                    return;
                }
                if (!TaskManager.Single().isCollecting)
                {
                    TaskManager.Single().isCollecting = true;
                }
                if (data != null)
                {
                    if (data.countIndex >= data.count)
                    {
                        Debug.Log("有悬赏采集任务 采集数量已满足 ");
                        TaskManager.Single().isCollecting = false;
                        rewardCanCollect = false;
                    }
                    else
                    {
                        rewardCanCollect = true;
                    }
                }
                if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList.ContainsKey(collectID))
                {
                    //通过采集源id 得到采集物的id 用于发给服务器
                    long[,] collectid = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[collectID].collectid;

                    foreach (int id in TaskManager.Single().TaskToCaijiDic.Keys)
                    {
                        if (TaskManager.Single().TaskToCaijiDic[id].opt4 == collectid[0, 0])
                        {
                            if (TaskManager.Single().TaskItemCountsDic.ContainsKey(collectid[0, 0]))
                            {
                                //有采集任务，但是采集数量已经够了 也不进行采集
                                if (TaskManager.Single().TaskItemCountsDic[collectid[0, 0]] >= TaskManager.Single().TaskToCaijiDic[id].opt5)
                                {
                                    Debug.Log("有采集任务 采集数量已满足 ");
                                    TaskManager.Single().isCollecting = false;
                                    mainTaskCanCollect = false; //数量够了不能采集
                                }
                                else
                                {
                                    mainTaskCanCollect = true;
                                }
                            }
                            else//数量中没有 肯定可以采
                            {
                                mainTaskCanCollect = true;
                            }
                        }
                        if (TaskManager.Single().TaskToCaijiDic[id].opt6 == collectid[0, 0])
                        {
                            if (TaskManager.Single().TaskItemCountsDic.ContainsKey(collectid[0, 0]))
                            {
                                //有采集任务，但是采集数量已经够了 也不进行采集
                                if (TaskManager.Single().TaskItemCountsDic[collectid[0, 0]] >= TaskManager.Single().TaskToCaijiDic[id].opt7)
                                {
                                    Debug.Log("有采集任务 采集数量已满足");
                                    TaskManager.Single().isCollecting = false;
                                    mainTaskCanCollect = false;//数量够了不能采集
                                }
                                else
                                {
                                    mainTaskCanCollect = true;
                                }
                            }
                            else {
                                mainTaskCanCollect = true;
                            }
                        }
                    }
                    if (TaskManager.MainTask != null)
                    {
                        if (TaskManager.MainTask.tasknode.Level > playerData.GetInstance().selfData.level)
                        {
                            Debug.Log("等级不足");
                            TaskManager.Single().isCollecting = false;
                            mainTaskCanCollect = false;//数量够了不能采集
                        }
                    }
                    
                    //UITaskCollectPanel.Instance.SetData(FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[collectID].note, 2f, collectid[0, 0], TaskProgressBarType.Collect, null,TaskClass.Main);
                }
                if (mainTaskCanCollect || rewardCanCollect)
                {
                    Debug.Log("有采集任务 点击采集" + collectID);
                    if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList.ContainsKey(collectID))
                    {
                        //通过采集源id 得到采集物的id 用于发给服务器
                        long[,] collectidarr = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[collectID].collectid;
                        object[] tempObj = new object[] { FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[collectID].note, 2f, collectidarr[0, 0], TaskProgressBarType.Collect, null, TaskClass.Main };
                        Control.ShowGUI(UIPanleID.UITaskCollectPanel, EnumOpenUIType.DefaultUIOrSecond, false, tempObj);
                    }
                    //Control.ShowGUI(GameLibrary.UITaskCollectPanel);
                }
                else
                {
                    TaskManager.Single().isCollecting = false;
                }


            }
            else
            {
                Debug.Log("无采集任务 不采集 ");
            }


        }
    }
    private void ShowCollectName()
    {
        collectNameObj.GetComponent<UILabel>().text = collectName;
        //npcNameObj.color = new Color(0, 100, 0);
        Vector3 tempV = getCollectHeadPos();
        tempV.y += 0.07f;
        collectNameObj.transform.position = tempV;
        collectNameObj.gameObject.SetActive(true);
    }
}
