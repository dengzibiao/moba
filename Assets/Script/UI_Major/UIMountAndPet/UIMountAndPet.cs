using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;
//这个是坐骑和宠物的枚举
public enum MountAndPet
{
    Null = 0,//无
    Mount = 1,//坐骑
    Pet = 2,//宠物
}
//这个是入口枚举
public enum EntranceType
{
    Shop = 1, //商店
    Hero = 2,//英雄详情
    Main = 3,//主城
}
public class UIMountAndPet : GUIBase
{
   
    //这个是宠物和坐骑的MultList
    public GUISingleMultList mountAndPetList;
    //这个是英雄列表的MultList
    public GUISingleMultList heroList;
    public UILabel TITletext;
    public GUISingleButton backBtn;              //返回按钮
    public GUISingleButton buyBtn;               //领取
    public GUISingleButton useBtn;               //使用
    public GUISingleCheckBoxGroup checkBoxs;
    public static MountAndPet mountAndPets;
    static EntranceType entranceTypes;
    public MountAndPet currentOperation = MountAndPet.Null;
    private UIMountNode mountData;
    private UIPetNode petData;
    public UILabel morPName;
    public UILabel des;
    public UILabel levellimit;
    public UILabel canRide;
    public UILabel getWayLabel;
    GameObject HeroPosEmb = null;
    private GameObject heroObj;
    public List<UIMountNode> mplist = new List<UIMountNode>();
    public List<UIPetNode> petlist = new List<UIPetNode>();
    public object[] mountObjs;//宠物
    public object[] petObjs;//坐骑
    /// <summary>
    /// 单例
    /// </summary>
    public static UIMountAndPet Instance;
    public UIMountAndPet()
    {
        Instance = this;
    }
    protected override void Init()
    {
        HeroPosEmb = GameObject.Find("HeroPosEmbattle");
        mountAndPetList = transform.Find("MountAndPet/MountAndPetList").GetComponent<GUISingleMultList>();
        heroList = transform.Find("HeroView/HeroList").GetComponent<GUISingleMultList>();
        morPName = transform.Find("MorPName").GetComponent<UILabel>();
        des = transform.Find("Des").GetComponent<UILabel>();
        levellimit = transform.Find("Levellimit").GetComponent<UILabel>();
        canRide = transform.Find("CanRide").GetComponent<UILabel>();
        getWayLabel = transform.Find("GetWayLabel").GetComponent<UILabel>();
        //InitData();
        backBtn.onClick = OnBackBtnClick;
        useBtn.onClick = OnUseBtnClick;
        buyBtn.onClick = OnBuyBtnClick;
        checkBoxs.onClick = OnCheckClick;

    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIMountAndPet;
    }
    private void InitData()
    {
        //mountObjs = MountAndPetNodeData.Instance().GetMPlist().ToArray();
        //petObjs = MountAndPetNodeData.Instance().Getpetlist().ToArray();
        mplist.Clear();
        petlist.Clear();
        //排列顺序  使用 拥有 为拥有   
        List<UIMountNode> allMountList = new List<UIMountNode>();//临时的用于插入
        mplist = MountAndPetNodeData.Instance().GetMPlist();
        foreach (UIMountNode val in mplist)
        {
            if (MountAndPetNodeData.Instance().currentMountID == val.mount_id)
            {

            }
            if (MountAndPetNodeData.Instance().IsHaveThisMount(val.mount_id))
            {
                allMountList.Insert(0, val);
            }
            else
            {
                allMountList.Add(val);
            }

        }
        foreach (UIMountNode val in allMountList)
        {
            if (val.mount_id == MountAndPetNodeData.Instance().currentMountID)
            {
                allMountList.Remove(val);
                allMountList.Insert(0, val);
                break;
            }
        }


        List<UIPetNode> allPetList = new List<UIPetNode>();//临时的用于插入
        petlist = MountAndPetNodeData.Instance().Getpetlist();
        foreach (UIPetNode val in petlist)
        {
            if (MountAndPetNodeData.Instance().currentPetID == val.pet_id)
            {

            }
            if (MountAndPetNodeData.Instance().IsHaveThisPet(val.pet_id))
            {
                allPetList.Insert(0, val);
            }
            else
            {
                allPetList.Add(val);
            }

        }
        foreach (UIPetNode val in allPetList)
        {
            if (val.pet_id == MountAndPetNodeData.Instance().currentPetID)
            {
                allPetList.Remove(val);
                allPetList.Insert(0, val);
                break;
            }
        }


        mountObjs = allMountList.ToArray();
        petObjs = allPetList.ToArray();

        allMountList = null;
        allPetList = null;
    }
    protected override void ShowHandler()
    {
        if (MountAndPet.Mount == mountAndPets)
        {
            checkBoxs.DefauleIndex = 0;
        }
        else
        {
            checkBoxs.DefauleIndex = 1;
        }
    }
    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams.Length>0)
        {
            mountAndPets = (MountAndPet)uiParams[0];
        }
        base.SetUI();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.pet_query_list_ret, UIPanleID.UIMountAndPet);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.pet_set_defend_status_ret, UIPanleID.UIMountAndPet);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.pet_update_mounts_list_ret, UIPanleID.UIMountAndPet);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.pet_update_pet_list_ret, UIPanleID.UIMountAndPet);
        Show();
    }
    public override void ReceiveData(UInt32 messageID)
    {
        switch (messageID)
        {
            case MessageID.pet_query_list_ret:
            case MessageID.pet_set_defend_status_ret:
                if (currentOperation == MountAndPet.Mount)
                {
                    ShowMount();
                }
                else if (currentOperation == MountAndPet.Pet)
                {
                    ShowPet();
                }
                break;
            case MessageID.pet_update_mounts_list_ret:
                ShowMount();
                break;
            case MessageID.pet_update_pet_list_ret:
                ShowPet();
                break;
        }
        base.ReceiveData(messageID);
    }
    private void OnCheckClick(int index, bool boo)
    {
        if (boo)
        {
            switch (index)
            {
                case 0:
                    currentOperation = MountAndPet.Mount;
                    MountAndPetNodeData.Instance().ShowType = true;
                    canRide.gameObject.SetActive(true);
                    heroList.gameObject.SetActive(true);
                    //获取一下玩家拥有的坐骑
                    Dictionary<string, object> newpacket = new Dictionary<string, object>();
                    newpacket.Add("arg1", 2);//1宠物 2坐骑
                    Singleton<Notification>.Instance.Send(MessageID.pet_query_list_req, newpacket, C2SMessageType.ActiveWait);
                    //ClientSendDataMgr.GetSingle().GetPetSend().SendGetHaveMountOrPetList(C2SMessageType.ActiveWait, 2);
                    ShowMount();
                    break;
                case 1:
                    currentOperation = MountAndPet.Pet;
                    MountAndPetNodeData.Instance().ShowType = false;
                    canRide.gameObject.SetActive(false);
                    heroList.gameObject.SetActive(false);
                    //获取一下玩家拥有的宠物
                    Dictionary<string, object> newpacket1 = new Dictionary<string, object>();
                    newpacket1.Add("arg1", 1);//1宠物 2坐骑
                    Singleton<Notification>.Instance.Send(MessageID.pet_query_list_req, newpacket1, C2SMessageType.ActiveWait);
                    //ClientSendDataMgr.GetSingle().GetPetSend().SendGetHaveMountOrPetList(C2SMessageType.ActiveWait, 1);
                    ShowPet();
                    break;
            }

        }
    }
    private void OnBuyBtnClick()
    {
       
        if (currentOperation == MountAndPet.Mount)
        {
            Debug.Log("领取坐骑" + currentOperation + "," + mountData.name);
            OnBackBtnClick();
            
            //打开福利界面
            if (mountData.mount_id == 601000100)//亡灵马打开签到
            {
                UIWelfare._instance.ExternalOpenWelfare(4, true);

            }
            if (mountData.mount_id == 601000200)//雷刃豹打开开服回馈
            {
                UIWelfare._instance.ExternalOpenWelfare(4, true);
            }
            if (mountData.mount_id == 601000300)//嗜血狂狼打开升级大礼
            {
                UIWelfare._instance.ExternalOpenWelfare(4, true);
            }
            ClientSendDataMgr.GetSingle().GetUISign_inSend().SendGetUISign_inList(C2SMessageType.Active, 2);//发送每日签到列表
        }
        else if (currentOperation == MountAndPet.Pet)
        {
            Debug.Log("领取宠物" + currentOperation + "," + petData.name);
            //打开福利界面
            OnBackBtnClick();
           
            if (petData.pet_id == 701000100)//蛋蛋升级大礼
            {
                UIWelfare._instance.ExternalOpenWelfare(4, true);
            }
            if (petData.pet_id == 701000200)//咬咬在线
            {
                UIWelfare._instance.ExternalOpenWelfare(4, true);
            }
            if (petData.pet_id == 701000300)//吼叫开服回馈
            {
                UIWelfare._instance.ExternalOpenWelfare(4, true);
            }
            ClientSendDataMgr.GetSingle().GetUISign_inSend().SendGetUISign_inList(C2SMessageType.Active, 2);//发送每日签到列表
        }
    }

    private void OnUseBtnClick()
    {
        
        if (currentOperation == MountAndPet.Mount)
        {
            Debug.Log("使用坐骑" + currentOperation + "," + mountData.name);
            if (mountData!=null)
            {
                ClientSendDataMgr.GetSingle().GetPetSend().SendUseMountOrPet(C2SMessageType.ActiveWait, 2, mountData.mount_id);// 1宠物 2坐骑
            }
           
        }
        else if (currentOperation == MountAndPet.Pet)
        {
            Debug.Log("使用宠物" + currentOperation + "," + petData.name);
            if (petData!=null)
            {
                ClientSendDataMgr.GetSingle().GetPetSend().SendUseMountOrPet(C2SMessageType.ActiveWait,1, petData.pet_id);// 1宠物 2坐骑
            }
        }

    }

    
    /// <summary>
    /// 显示坐骑
    /// </summary>
    public void ShowMount()
    {
        InitData();
        MountAndPetNodeData.Instance().seletIndex = 0;
        if (mountObjs!=null)
        {
            mountAndPetList.InSize(mountObjs.Length, 1);
            mountAndPetList.Info(mountObjs);
            if (mountObjs.Length>0)
            {
                SetInfo(mountObjs[0], MountAndPet.Mount);
            }
        }
       
        
    }
    /// <summary>
    /// 设置宠物或者坐骑的详细信息
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="type"></param>
    public void SetInfo(object obj, MountAndPet type)
    {
        currentOperation = type;
        if (type == MountAndPet.Mount)
        {
            mountData = (UIMountNode)obj;
            //MountHeroViewItem.Instance().refreshUI(MountAndPetNodeData.Instance().GetHerolist(mountData.mount_types));
            heroList.InSize(MountAndPetNodeData.Instance().GetHerolist(mountData.mount_types).Count, 3);
            heroList.Info(MountAndPetNodeData.Instance().GetHerolist(mountData.mount_types).ToArray());
            morPName.text = GoodsDataOperation.GetInstance().JointNameColour(mountData.name, GradeType.Purple); ;
            des.text = mountData.describe;
            levellimit.text = mountData.need_lv + "";
            // 先判断是否拥有
            //再判断是否使用
            if (MountAndPetNodeData.Instance().IsHaveThisMount(mountData.mount_id))
            {
                buyBtn.gameObject.SetActive(false);
                getWayLabel.gameObject.SetActive(false);
                if (MountAndPetNodeData.Instance().currentMountID == mountData.mount_id)
                {
                    useBtn.gameObject.SetActive(false);
                }
                else
                {
                    useBtn.gameObject.SetActive(true);
                }
            }
            else
            {
                useBtn.gameObject.SetActive(false);
                buyBtn.gameObject.SetActive(true);
                getWayLabel.gameObject.SetActive(true);
                getWayLabel.text = GetWayDetail(mountData.mount_id);
            }
            InsHero(int.Parse(mountData.model_id),MountAndPet.Mount);


        }
        else if (type == MountAndPet.Pet)
        {
            petData = (UIPetNode)obj;
            morPName.text = GoodsDataOperation.GetInstance().JointNameColour(petData.name, GradeType.Purple); ;
            des.text = petData.describe;
            levellimit.text = petData.need_lv + "";
            if (MountAndPetNodeData.Instance().IsHaveThisPet(petData.pet_id))
            {
                buyBtn.gameObject.SetActive(false);
                getWayLabel.gameObject.SetActive(false);
                if (MountAndPetNodeData.Instance().currentPetID == petData.pet_id)
                {
                    useBtn.gameObject.SetActive(false);
                }
                else
                {
                    useBtn.gameObject.SetActive(true);
                }
            }
            else
            {
                useBtn.gameObject.SetActive(false);
                buyBtn.gameObject.SetActive(true);
                getWayLabel.gameObject.SetActive(true);
                getWayLabel.text = GetWayDetail(petData.pet_id);
            }
            InsHero(int.Parse(petData.model_id),MountAndPet.Pet);
        }
    }
    private string GetWayDetail(long id)
    {
        string str = "";
        switch (id)
        {
            case 601000100://亡灵马
                str = "请前往登录礼包领取奖励";
                break;
            case 601000200://雷刃豹
                str = "请前往登录礼包领取奖励";
                break;
            case 601000300://嗜血狂狼
                str = "请前往登录礼包领取奖励";
                break;
            case 701000100://蛋蛋
                str = "请前往登录礼包领取奖励";
                break;
            case 701000200://咬咬
                str = "请前往登录礼包领取奖励";
                break;
            case 701000300://吼叫
                str = "请前往登录礼包领取奖励";
                break;
        }
        return str;
    }
    /// <summary>
    /// 显示宠物
    /// </summary>
    public void ShowPet()
    {
        InitData();
        MountAndPetNodeData.Instance().seletIndex = 0;
        if (petObjs !=null)
        {
            mountAndPetList.InSize(petObjs.Length, 1);
            mountAndPetList.Info(petObjs);
            if(petObjs.Length > 0)
            {
                SetInfo(petObjs[0], MountAndPet.Pet);
            }
        }

    }
    //这里是根据传进来的类型加载不同数据显示的
    public void SetShowType(MountAndPet mountAndPet, EntranceType entranceType)
    {
        mountAndPets = mountAndPet;
        entranceTypes = entranceType;
    }
    /// <summary>
    /// 返回按钮
    /// </summary>
    private void OnBackBtnClick()
    {
        //MountHeroViewItem.cishu = 0;
        HeroPosEmb.transform.Find("TitlePos").gameObject.SetActive(false);
        if (heroObj != null)
        {
            Destroy(heroObj);
        }
        Control.HideGUI();
        //Control.HideGUI(GameLibrary.UIMountAndPet);
        AudioController.Instance.StopUISound();
        if (entranceTypes == EntranceType.Hero)
        {
            //Control.ShowGUI(UIPanleID.UIHeroDetail);
        }
        else
        {
            Control.PlayBGmByClose(this.GetUIKey());
        }
       
       
    }
    /// <summary>
    /// 实例化英雄展示模型
    /// </summary>
   public void InsHero(int modelID,MountAndPet type)
    {
        heroObj = HeroPosEmbattle.instance.CreatModelByModelID(modelID, PosType.TitlePos, transform.Find("HeroTexture").GetComponent<SpinWithMouse>(),type);
    }
}
