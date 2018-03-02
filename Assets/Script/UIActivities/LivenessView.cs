/*
文件名（File Name）:   LivenessView.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-10-25 15:25:43
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class LivenessView : GUIBase
{
    public static LivenessView instance;
    public GUISingleLabel count;
    public GUISingleButton boxOneBtn;
    public GUISingleButton boxTwoBtn;
    public GUISingleButton boxThreeBtn;
    public GUISingleButton boxFourBtn;
    public GUISingleSprite eXE;
    public GameObject[] effect;
    private List<ItemData> dataList = new List<ItemData>(); //存储数据列表

    public LivenessView()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    protected override void Init()
    {
        boxOneBtn.onButtonPress = BoxOneBtnOnClick;
        boxTwoBtn.onButtonPress = BoxTwoBtnOnClick;
        boxThreeBtn.onButtonPress = BoxThreeBtnOnClick;
        boxFourBtn.onButtonPress = BoxFourBtnOnClick;
    }

    private void BoxOneBtnOnClick(bool state)
    {
        if (state)
        {
            ClickHandle(playerData.GetInstance().taskDataList.box1State, boxOneBtn, 20);
        }
        else
        {
            //Control.HideGUI(GameLibrary.UIBoxGoodsTip);
            Control.HideGUI(UIPanleID.UIBoxGoodsTip);
        }

    }
    private void BoxTwoBtnOnClick(bool state)
    {
        if (state)
        {
            ClickHandle(playerData.GetInstance().taskDataList.box2State, boxTwoBtn, 40);
        }
        else
        {
            //Control.HideGUI(GameLibrary.UIBoxGoodsTip);
            Control.HideGUI(UIPanleID.UIBoxGoodsTip);
        }

    }
    private void BoxThreeBtnOnClick(bool state)
    {
        if (state)
        {
            ClickHandle(playerData.GetInstance().taskDataList.box3State, boxThreeBtn, 60);
        }
        else
        {
            //Control.HideGUI(GameLibrary.UIBoxGoodsTip);
            Control.HideGUI(UIPanleID.UIBoxGoodsTip);
        }

    }
    private void BoxFourBtnOnClick(bool state)
    {
        if (state)
        {
            ClickHandle(playerData.GetInstance().taskDataList.box4State, boxFourBtn, 80);
        }
        else
        {
            //Control.HideGUI(GameLibrary.UIBoxGoodsTip);
            Control.HideGUI(UIPanleID.UIBoxGoodsTip);
        }

    }
    protected override void ShowHandler()
    {
        InitUI();
    }

    public void InitUI()
    {

        count.text = playerData.GetInstance().taskDataList.ActiveIndex + "/" +
                     "[acd5ff]" + playerData.GetInstance().taskDataList.ActiveAll + "[-]";
        if (playerData.GetInstance().taskDataList != null && playerData.GetInstance().taskDataList.dailyTasks != null)
        {
            boxOneBtn.text = playerData.GetInstance().taskDataList.dailyTasks.Active_value1.ToString();
            boxTwoBtn.text = playerData.GetInstance().taskDataList.dailyTasks.Active_value2.ToString();
            boxThreeBtn.text = playerData.GetInstance().taskDataList.dailyTasks.Active_value3.ToString();
            boxFourBtn.text = playerData.GetInstance().taskDataList.dailyTasks.Active_value4.ToString();
        }
        eXE.fillAmount = (float)playerData.GetInstance().taskDataList.ActiveIndex /
                         playerData.GetInstance().taskDataList.ActiveAll;
        GetTypeHandle(playerData.GetInstance().taskDataList.box1State, boxOneBtn,1);
        GetTypeHandle(playerData.GetInstance().taskDataList.box2State, boxTwoBtn,2);
        GetTypeHandle(playerData.GetInstance().taskDataList.box3State, boxThreeBtn,3);
        GetTypeHandle(playerData.GetInstance().taskDataList.box4State, boxFourBtn,4);
    }

    /// <summary>
    /// 点击时根据状态处理
    /// </summary>
    /// <param name="type"></param>
    void ClickHandle(int type, GUISingleButton botton, int boxIndex)
    {
        switch ((TaskProgress)type)
        {
            case TaskProgress.CantAccept:

                switch (boxIndex)
                {
                    case 20:
                        GetBoxItemList(boxIndex);
                        //UIBoxGoodsTip.instance.SetData(dataList);
                        break;
                    case 40:
                        GetBoxItemList(boxIndex);
                        //UIBoxGoodsTip.instance.SetData(dataList);
                        break;
                    case 60:
                        GetBoxItemList(boxIndex);
                        //UIBoxGoodsTip.instance.SetData(dataList);
                        break;
                    case 80:
                        GetBoxItemList(boxIndex);
                        //UIBoxGoodsTip.instance.SetData(dataList);
                        break;
                }
                if(dataList.Count >0)
                    //Control.ShowGUI(GameLibrary.UIBoxGoodsTip);
                    Control.ShowGUI(UIPanleID.UIBoxGoodsTip, EnumOpenUIType.DefaultUIOrSecond, false, dataList);
                break;
            case TaskProgress.Complete:
                ClientSendDataMgr.GetSingle()
                    .GetEveryDailyTaskSend()
                    .EveryDailyRequest(playerData.GetInstance().taskDataList.Id, (int)(GetTaskItemType.Box), boxIndex);
                switch (boxIndex)
                {
                    case 20:
                        botton.spriteName = "baox1_open";
                        effect[0].gameObject.SetActive(false);
                        break;
                    case 40:
                        botton.spriteName = "baox2_open";
                        effect[1].gameObject.SetActive(false);
                        break;
                    case 60:
                        botton.spriteName = "baox3_open";
                        effect[2].gameObject.SetActive(false);
                        break;
                    case 80:
                        botton.spriteName = "baox4_open";
                        effect[3].gameObject.SetActive(false);
                        break;
                }
                break;
            case TaskProgress.Reward:

                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "奖励已经领取");
                break;
        }
    }

    /// <summary>
    /// 根据状态处理
    /// </summary>
    /// <param name="type"></param>
    void GetTypeHandle(int type, GUISingleButton botton, int boxIndex)
    {
        switch ((TaskProgress)type)
        {
            case TaskProgress.CantAccept: botton.delayTime = 0; break;
            case TaskProgress.Complete:
                switch (boxIndex)
                {
                    case 1:
                        effect[0].gameObject.SetActive(true);
                        break;
                    case 2:
                        effect[1].gameObject.SetActive(true);
                        break;
                    case 3:
                        effect[2].gameObject.SetActive(true);
                        break;
                    case 4:
                        effect[3].gameObject.SetActive(true);
                        break;
                }
                break;
            case TaskProgress.Reward:
                switch (boxIndex)
                {
                    case 1:
                        botton.spriteName = "baox1_open";
                        break;
                    case 2:
                        botton.spriteName = "baox2_open";
                        break;
                    case 3:
                        botton.spriteName = "baox3_open";
                        break;
                    case 4:
                        botton.spriteName = "baox4_open";
                        break;
                }
                break;
        }
    }

    void GetBoxItemList(int index)
    {
        long[,] data = null;
        dataList.Clear();
        switch (index)
        {
            case 20:
                if(playerData.GetInstance().taskDataList.DailyTasks!=null)
                data = playerData.GetInstance().taskDataList.DailyTasks.active_prop1;
                break;
            case 40:
                if (playerData.GetInstance().taskDataList.DailyTasks != null)
                    data = playerData.GetInstance().taskDataList.DailyTasks.active_prop2;
                break;
            case 60:
                if (playerData.GetInstance().taskDataList.DailyTasks != null)
                    data = playerData.GetInstance().taskDataList.DailyTasks.active_prop3;
                break;
            case 80:
                if (playerData.GetInstance().taskDataList.DailyTasks != null)
                    data = playerData.GetInstance().taskDataList.DailyTasks.active_prop4;
                break;
        }
        if (data!=null)
        {
            for (int i = 0; i < data.Length / 2; i++)
            {
                ItemData tempData = new ItemData();
                tempData.GoodsType = MailGoodsType.ItemType;

                tempData.Id = data[i, 0];
                tempData.Count = (int)(data[i, 1]);
                if (int.Parse(StringUtil.SubString(tempData.Id.ToString(), 3)) == 106)
                {
                    tempData.UiAtlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
                    tempData.Types = 6;
                }
                else
                {
                    tempData.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                }

                if (tempData.Id == (int)MailGoodsType.GoldType)
                {
                    tempData.GradeTYPE = GradeType.Blue;
                    tempData.Gold = tempData.Count;
                    tempData.IconName = "jinbi";
                    tempData.Name = "金币";
                }
                else if (tempData.Id == (int)MailGoodsType.ExE)
                {
                    tempData.GradeTYPE = GradeType.Blue;
                    tempData.Exe = tempData.Count;
                    tempData.IconName = "zhandui-exp";
                    tempData.Name = "战队经验";
                }
                else if (tempData.Id == (int)MailGoodsType.DiamomdType)
                {
                    tempData.GradeTYPE = GradeType.Blue;
                    tempData.Diamond = tempData.Count;
                    tempData.IconName = "zuanshi";
                    tempData.Name = "钻石";
                }
                else
                {
                    if (GameLibrary.Instance().ItemStateList.ContainsKey(tempData.Id))
                    {
                        tempData.IconName = GameLibrary.Instance().ItemStateList[tempData.Id].icon_name;
                        tempData.Name = GameLibrary.Instance().ItemStateList[tempData.Id].name;
                        //tempData.Types = vo.type;
                        switch (GameLibrary.Instance().ItemStateList[tempData.Id].grade)
                        {
                            case 1:
                                tempData.GradeTYPE = GradeType.Gray;
                                break;
                            case 2:
                                tempData.GradeTYPE = GradeType.Green;
                                break;
                            case 4:
                                tempData.GradeTYPE = GradeType.Blue;
                                break;
                            case 7:
                                tempData.GradeTYPE = GradeType.Purple;
                                break;
                            case 11:
                                tempData.GradeTYPE = GradeType.Orange;
                                break;
                            case 16:
                                tempData.GradeTYPE = GradeType.Red;
                                break;
                        }

                    }

                }
                dataList.Add(tempData);
            }
     
        }
    }
}
