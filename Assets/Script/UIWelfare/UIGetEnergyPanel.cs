using System.Collections.Generic;
using System.Collections;
using UnityEngine;
/// <summary>
/// 福利-获取体力
/// </summary>
public class UIGetEnergyPanel : GUIBase
{

    public GUISingleLabel tipsLab;
    public GUISingleButton eatBtn;
    public GUISingleLabel lunchName;
    public GUISingleLabel lunchTime;
    public GUISingleLabel dinnerName;
    public GUISingleLabel dinnerTime;
    public GUISingleLabel nightSnackName;
    public GUISingleLabel nightSnackTime;
    public GUISingleLabel addPower;
    public Transform turkeyImg;
    public Transform noTurkeyImg;
    public static UIGetEnergyPanel _instance;

    private bool boo;
    private string str;
    private bool isPower;
    private bool turkey;
    //Dictionary<long, MealAttrNode> nodeDic;
    // List<MealAttrNode> mealList = new List<MealAttrNode>();
    private MealAttrNode mealAttrNode;
    List<MealAttrNode> mealList = null;
    Transform tiliEffect;
    public UIGetEnergyPanel()
    {
        _instance = this;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    protected override void Init()
    {
        base.Init();

        lunchName.text = playerData.GetInstance().getEnergyData.mealList[0].name;
        lunchTime.text = playerData.GetInstance().getEnergyData.mealList[0].startTime.ToString() + ":00" + "-" + playerData.GetInstance().getEnergyData.mealList[0].endTime.ToString() + ":00";

        dinnerName.text = playerData.GetInstance().getEnergyData.mealList[1].name;
        dinnerTime.text = playerData.GetInstance().getEnergyData.mealList[1].startTime.ToString() + ":00" + "-" + playerData.GetInstance().getEnergyData.mealList[1].endTime.ToString() + ":00";

        nightSnackName.text = playerData.GetInstance().getEnergyData.mealList[2].name;
        nightSnackTime.text = playerData.GetInstance().getEnergyData.mealList[2].startTime.ToString() + ":00" + "-" + playerData.GetInstance().getEnergyData.mealList[2].endTime.ToString() + ":00";

        eatBtn.onClick = OnEatClick;
        tiliEffect = transform.Find("UI_TiLiHuoQu_01");

    }
    protected override void ShowHandler()
    {
       // Refarece();
        ShowData();
        tiliEffect.gameObject.SetActive(false);
    }
    //public void Refarece()
    //{
    //    ShowData();

    //}
    //实时刷新吃鸡的动态
    void Update()
    {
        ShowData();
    }
    private void OnEatClick()
    {
        //EditorUtility.DisplayDialog("title", "content","ok");
        //鸡腿文字消失
        ClientSendDataMgr.GetSingle().GetEnergySend().SendGetEnergy();
    }
    public void OpenTiliEffect()
    {
        if (tiliEffect!=null)
        {
            tiliEffect.gameObject.SetActive(false);
            tiliEffect.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 初始化数据0--1是否显示进餐按钮2显示的字符串3鸡显示4增加多少体力
    /// </summary>
    /// <param name="boo"></param>
    /// <param name="str"></param>
    public void InitData(bool boo, string str,bool turkey, bool isPower)
    {
        this.boo = boo;
        this.str = str;
        this.turkey = turkey;
        this.isPower = isPower;
    }
    //显示数据
    private void ShowData()
    {
        //turkeyImg.gameObject.SetActive(this.boo);
        
        eatBtn.gameObject.SetActive(this.boo);
        tipsLab.text = str;
        addPower.gameObject.SetActive(this.isPower);
        if (this.turkey)//在时间内
        {
            turkeyImg.gameObject.SetActive(true);
            noTurkeyImg.gameObject.SetActive(false);
        }
        else
        {
            turkeyImg.gameObject.SetActive(false);
            noTurkeyImg.gameObject.SetActive(true);
        }
    }

}
