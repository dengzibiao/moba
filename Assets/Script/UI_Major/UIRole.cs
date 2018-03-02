using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;
public class UIRole : GUIBase
{
    public GUISingleButton roleIcon;
    public GUISingleButton rideIcon;
    public GUISingleButton petIcon;
    public UIProgressBar rideBar;
    public GUISingleSprite roleIconBorder;
    public GUISingleLabel nameTxt;
    public GUISingleLabel levelTxt;//战队等级
    public GUISingleLabel heroLv;//英雄等级
    public UILabel FightTxt;
    public UISlider hpSlider;
    public UISprite hpSpliter;
    public UISprite SpSheild;
    public UISprite SpSheildFull;

    public static UIRole instance;

    GameObject changeName;
    GameObject uiRoleInfo;
    GameObject changeIcon;
    GameObject changeIconBorder;
    // HeroData hero;
    private RoleIconAttrNode item;

    //public List<HeroData> herodataList = new List<HeroData>();
    //object[] objAll;                             //所有英雄
    //object[] objHave;

    public int fcSum = 0;

    public static UIRole Instance { get { return instance; } }

    public UIRole()
    {
        instance = this;
    }

    protected override void Init()
    {
        base.Init();
         
        if (FightTxt != null)
        {
            FightTxt.text = fcSum.ToString();

            roleIcon.onClick = OnRoleIconClick;
            //changeName = transform.Find("ChangeName").gameObject;///5
            //uiRoleInfo = transform.Find("UIRoleInfo").gameObject;///5
            //changeIcon = transform.Find("ChangeIcon").gameObject;
            //changeIconBorder = transform.Find("ChangeIconBorder").gameObject;
            //Control.ShowGUI(GameLibrary.UIKnapsack);
        }
        rideIcon.onClick = OnRide;
        petIcon.onClick = OnPetIcon;
        //if (transform.Find("Mask"))
        //{
        //    mask = transform.Find("Mask").GetComponent<UISprite>();
        //    UIEventListener.Get(mask.gameObject).onClick += OnMaskClick;
        //}
        playerData.GetInstance().ChangelvAndExpEvent += UpdataLV;
        if(hpSpliter != null)
            hpSpliter.gameObject.SetActive(false);

        if (SpSheild != null)
            SpSheild.alpha = 0f;

        if (SpSheildFull!=null)
        SpSheildFull.alpha = 0f;
    }

    void OnRide()
    {
        if(TaskAutoTraceManager._instance != null)
            TaskAutoTraceManager._instance.StopTaskAutoFindWay();
        if (!CheckCanRide((HeroData)CharacterManager.playerCS.CharData))
        {
            CharacterManager.instance.ShowTip("该英雄无法骑乘");
            return;
        }
        if (MountAndPetNodeData.Instance().currentMountID <= 0)
        {
            //MountAndPetNodeData.Instance().currentMountID = 601000100;
            // CharacterManager.instance.ShowTip("未指定坐骑");
            //UIMountAndPet.mountAndPets = MountAndPet.Mount;
            //Control.ShowGUI(GameLibrary.UIMountAndPet);
            Control.ShowGUI(UIPanleID.UIMountAndPet, EnumOpenUIType.OpenNewCloseOld, false, MountAndPet.Mount);
            return;
        }
        if (!CharacterManager.playerCS.pm.isRiding)
        {
            if (riding)
            {
                CancelRide();
            }
            else
            {
                rideBar.gameObject.SetActive(true);
                rideBar.value = 0f;
                InvokeRepeating("StartRide", 0f, 0.02f);
            }
        }
        else
        {
            CharacterManager.playerCS.pm.isWaitingRideMsg = true;
            ClientSendDataMgr.GetSingle().GetPetSend().SendChangeMountOrPetState(C2SMessageType.ActiveWait, 2, MountAndPetNodeData.Instance().currentMountID, 0);
            //CharacterManager.playerCS.pm.Ride(false);
        }
    }
    //判断该英雄能否骑乘
    public static bool CheckCanRide( HeroData hd )
    {
        for (int i = 0; i < hd.node.mount_types.Length; i++)
        {
            if (hd.node.mount_types[i] == 1)
                return true;
        }
        return false;
    }

    public bool riding;
    void StartRide()
    {
        riding = true;
        rideBar.value += 0.01f;
        if (rideBar.value >= 0.99f)
        {
            long mountId = MountAndPetNodeData.Instance().currentMountID;
            ClientSendDataMgr.GetSingle().GetPetSend().SendChangeMountOrPetState(C2SMessageType.ActiveWait, 2, MountAndPetNodeData.Instance().currentMountID, 1);
            CharacterManager.playerCS.pm.isWaitingRideMsg = true;
            CancelRide();
        }
    }

    void OnPetIcon()
    {
        if (null == CharacterManager.playerCS.pet || !CharacterManager.playerCS.pet.activeSelf)
        {
            if(MountAndPetNodeData.Instance().currentPetID == 0)
            {
                // CharacterManager.instance.ShowTip("未指定宠物");
                //UIMountAndPet.mountAndPets = MountAndPet.Pet;
                //Control.ShowGUI(GameLibrary.UIMountAndPet);
                Control.ShowGUI(UIPanleID.UIMountAndPet, EnumOpenUIType.OpenNewCloseOld, false, MountAndPet.Pet);
                return;
            }
            ClientSendDataMgr.GetSingle().GetPetSend().SendChangeMountOrPetState(C2SMessageType.ActiveWait, 1, MountAndPetNodeData.Instance().currentPetID, 1);
        }
        else
        {
            ClientSendDataMgr.GetSingle().GetPetSend().SendChangeMountOrPetState(C2SMessageType.ActiveWait, 1, MountAndPetNodeData.Instance().currentPetID, 0);
        }
    }

    public void CancelRide()
    {
        riding = false;
        if (rideBar != null)
            rideBar.gameObject.SetActive(false);
        CancelInvoke("StartRide");
    }
    private void OnRoleIconClick()
    {
        Control.ShowGUI(UIPanleID.UIRoleInfo, EnumOpenUIType.DefaultUIOrSecond);
        //uiRoleInfo.SetActive(true);

    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.login_change_name_ret, UIPanleID.UIRole);//购买是否成功

        Singleton<Notification>.Instance.RegistMessageID(MessageID.login_change_name_req, UIPanleID.UIRole);//购买是否成功
    }
    public override void ReceiveData(uint messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            case MessageID.login_change_name_ret:
                 Show();
                 Control.ShowGUI(UIPanleID.UIRoleInfo, EnumOpenUIType.DefaultUIOrSecond);
                 playerData.GetInstance().selfData.playeName = ChangeName._instance.nicknameInput.value;
                 UIRole.Instance.nameTxt.text = ChangeName._instance.nicknameInput.value;
                 if (null != CharacterManager.playerCS.pet)//改名成功后同步宠物的名字
                 {
                    CharacterManager.playerCS.pet.GetComponent<Pet_AI>().ChangePetName();
                 }
                 playerData.GetInstance().selfData.changeCount = 1;
              
                break;
        }
    }
    protected override void ShowHandler()
    {

        nameTxt.text = playerData.GetInstance().selfData.playeName.ToString();
        if (levelTxt != null)
            levelTxt.text = playerData.GetInstance().selfData.level.ToString();

        SetMainHeroLevel();

        if (playerData.GetInstance().iconFrameData!=null&&playerData.GetInstance().iconFrameData.iconFrame_id!=0&&FSDataNodeTable<RoleIconAttrNode>.GetSingleton().DataNodeList.ContainsKey(playerData.GetInstance().iconFrameData.iconFrame_id))
            roleIconBorder.spriteName = FSDataNodeTable<RoleIconAttrNode>.GetSingleton().DataNodeList[playerData.GetInstance().iconFrameData.iconFrame_id].icon_name;
      
        if (Globe.isFB || GameLibrary.isPVP3)
        {
            roleIcon.enabled = false;
        }
        else
        {
            roleIcon.enabled = true;
        }

        fcSum = 0;
        for (int i = 0; i < playerData.GetInstance().herodataList.Count; i++)
        {
            fcSum += playerData.GetInstance().herodataList[i].fc;
        }
        if (FightTxt != null && FightTxt.text != null)
        {
            playerData.GetInstance().selfData.FightLv = fcSum;
            FightTxt.text = playerData.GetInstance().selfData.FightLv.ToString();
        }

        // FightTxt.text = fcSum.ToString();
    }
    public void RefreshIconId(int id)
    {
        // this._id = id;
        if (id == 0) return;
        playerData.GetInstance().iconData.icon_id = id;
        HeroData hd = playerData.GetInstance().GetHeroDataByID(id);
        if (null != hd)
            heroLv.text = hd.lvl.ToString();
        ShowIcon();
    }
    public void SetMainHeroLevel()
    {
        if (!GameLibrary.isNetworkVersion)
        {
            HeroData hd = playerData.GetInstance().GetHeroDataByID(GameLibrary.player);
            // ShowLv(null == hd ? 1 : hd.lvl);
            RefreshIconId(GameLibrary.SceneType(SceneType.PVP3) ? (int)Globe.challengeTeam[0].id : (int)GameLibrary.player);//初始化显示英雄头像
        }
        else
        {
            if (null == SceneBaseManager.instance)
            {
                //  UIRole.instance.RefreshLv(playerData.GetInstance().selfData.level);//经验值
            }
            else
            {
                //  ShowLv(Globe.Heros()[0].lvl);
            }
            RefreshIconId(null == SceneBaseManager.instance ? (int)GameLibrary.player : (int)Globe.Heros()[0].id);//初始化显示英雄头像
        }
    }
    private void ShowIcon()
    {
        if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList != null && this != null && FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(playerData.GetInstance().iconData.icon_id))
        {
            roleIcon.spriteName = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[playerData.GetInstance().iconData.icon_id].icon_name + "_head";

          //  ClientSendDataMgr.GetSingle().GetRoleSend().SendIcon();
        }
    }
    /// <summary>
    /// 当前等级、当前经验、最大经验
    /// </summary>
    /// <param name="nextLv"></param>
    /// <param name="exp"></param>
    /// <param name="maxExp"></param>
    //public void RefreshLv(int nextLv)
    //{
    //    playerData.GetInstance().selfData.level = nextLv;
    //    ShowLv(playerData.GetInstance().selfData.level, true);
    //}
    //private void ShowLv(int lvl, bool isMajor = false)
    //{
    //    if (isMajor)
    //    {
    //        //UIExpBar.GetInstance().RefreshExpBar(exp);
    //    }
    //    if (levelTxt != null)
    //        levelTxt.text = lvl.ToString();

    //}
    private void UpdataLV(int lv, int exp = 0)
    {

        if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)//在主城时刷新战队等级、经验
        {
            if (levelTxt != null)
            {
                levelTxt.text = lv.ToString();
            }
            playerData.GetInstance().ChangeActionPointCeilingHandler();
            if (FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList.ContainsKey(lv))
            {
                UIExpBar.GetInstance().expBar.InValue(exp, FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList[lv].exp);
            }
        }

    }

    /// <summary>
    /// 打开改名面板
    /// </summary>
    public void OpenChangeNamePanel(bool isOpen)
    {
        if (isOpen)
        {
            Control.ShowGUI(UIPanleID.ChangeName, EnumOpenUIType.DefaultUIOrSecond);
        }
        else
        {
            Control.HideGUI(UIPanleID.UIRoleInfo);
            //  Control.ShowGUI(GameLibrary.UIRoleInfo);
        }
        // changeName.SetActive(isOpen);
        // uiRoleInfo.SetActive(!isOpen);
    }
    /// <summary>
    /// 打开更换头像面板
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenChangeIconPanel(bool isOpen)
    {
        //if (isOpen)
        //{
        //    Control.ShowGUI(GameLibrary.ChangeIcon);
        //}
        //else {
        //    Control.HideGUI(GameLibrary.ChangeIcon);
        //    Control.ShowGUI(GameLibrary.UIRoleInfo);
        //}
        // changeIcon.SetActive(isOpen);
        // uiRoleInfo.SetActive(!isOpen);
    }
    /// <summary>
    /// 打开更换头像框面板
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenChangeIconBorderPanel(bool isOpen)
    {
        //if (isOpen)
        //{
        //    Control.ShowGUI(GameLibrary.ChangeIconBorder);
        //}
        //else
        //{
        //    Control.HideGUI(GameLibrary.ChangeIconBorder);
        //    Control.ShowGUI(GameLibrary.UIRoleInfo);
        //}
        //changeIconBorder.SetActive(isOpen);
        //uiRoleInfo.SetActive(!isOpen);
    }
    public void ChangeHeroHeadIcon()
    {
        HeroNode heroNode = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[GameLibrary.player];
        roleIcon.spriteName = heroNode.icon_name + "_head";
        nameTxt.text = heroNode.name;
    }

    public void OnHpBarChange ( float count, float max, float shield )
    {
        hpSlider.backgroundWidget.gameObject.SetActive(false);
        if(max <= 0)
            return;

        float hpPercent = count / max;
        float shieldPercent = shield / max;
        float totalPercent = hpPercent + shieldPercent;

        if(shield > 0)
        {
            SpSheild.alpha = 1f;
            if(totalPercent > 1f)
            {
                SpSheild.fillAmount = shieldPercent;
                SpSheild.depth = hpSlider.foregroundWidget.depth + 1;
                SpSheild.invert = true;
                SpSheildFull.alpha = 1f;
            }
            else
            {
                SpSheild.fillAmount = shieldPercent + hpPercent;
                SpSheild.depth = hpSlider.foregroundWidget.depth -1;
                SpSheild.invert = false;
                SpSheildFull.alpha = 0f;
            }
        }
        else
        {
            SpSheild.alpha = SpSheildFull.alpha = 0f;
        }

        hpSlider.value = hpPercent;
        hpSpliter.gameObject.SetActive(totalPercent < 1f);
        if(totalPercent > 1f)
            return;
        float lessVal = 1f - totalPercent;
        if(totalPercent < 0.96f)
        {
            hpSpliter.transform.localScale = Vector3.one;
            hpSpliter.transform.localPosition = new Vector3(163f - 210f * lessVal, 0f, 0f);
        }
        else
        {
            hpSpliter.transform.localScale = new Vector3(0.5f + 10f * lessVal, 0.5f + 10f * lessVal, 1f);
            hpSpliter.transform.localPosition = new Vector3(164f - 100f * (2f + 10f * lessVal) * lessVal, 8f - 2f * 100f * lessVal, 0f);
        }

    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIRole;
    }

}
