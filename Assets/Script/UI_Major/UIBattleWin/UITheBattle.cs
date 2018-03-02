using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Tianyu;
using UnityEngine.SceneManagement;

public class UITheBattle : GUIBase
{

    public static UITheBattle instance;
    UISprite Result;
    UISprite resultFB;
    TweenAlpha promptLabel;
    UISprite sprite;
    UISprite star_0;
    UISprite star_1;
    UISprite star_2;
    Transform effStar_0;
    Transform effStar_1;
    Transform effStar_2;
    Transform effWinBg_1;
    Transform effWinBg_2;
    Transform effDefeat;
    List<UISprite> bannerList = new List<UISprite>();
    SceneNode sceneNode;

    Transform FailedPanel;

    public GUISingleButton leaveBtn;
    public GUISingleButton fightingBtn;
    public Transform ResultPanel;
    public UICloseAnAccount closeAnAccount;
    public GUISingleButton heroBtn;
    public GUISingleButton equipBtn;
    public GUISingleButton shopBtn;
    public GUISingleButton altarBtn;
    public UITexture bg;
    public UISprite mobaBG;


    public bool isWin = false;
    public bool isMoba = false;
    public UITheBattle()
    {
        instance = this;
    }

    float intervalTime = 2f;

    List<TweenPosition> starlist = new List<TweenPosition>();


    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UITheBattlePanel;
    }
    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams.Length > 0)
        {
            isMoba = (bool)uiParams[0];
            isWin = (bool)uiParams[1];
        }
        base.SetUI(uiParams);
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }
    protected override void Init()
    {
        base.Init();

        Result = ResultPanel.Find("Result").GetComponent<UISprite>();
        sprite = Result.transform.Find("Sprite").GetComponent<UISprite>();
        promptLabel = transform.Find("PromptLabel").GetComponent<TweenAlpha>();

        effDefeat = ResultPanel.Find("UI_JieSuan_BG_Lose");
        if (ResultPanel.Find("ResultFB") != null)
            resultFB = ResultPanel.Find("ResultFB").GetComponent<UISprite>();

        if (isWin)
        {
            star_0 = ResultPanel.Find("ResultFB/Star_0").GetComponent<UISprite>();
            star_1 = ResultPanel.Find("ResultFB/Star_1").GetComponent<UISprite>();
            star_2 = ResultPanel.Find("ResultFB/Star_2").GetComponent<UISprite>();
            effStar_0 = ResultPanel.Find("ResultFB/UI_JieSuan_Star_00");
            effStar_1 = ResultPanel.Find("ResultFB/UI_JieSuan_Star_01");
            effStar_2 = ResultPanel.Find("ResultFB/UI_JieSuan_Star_02");
            effWinBg_1 = ResultPanel.Find("ResultFB/UI_JieSuan_BG_01");
            effWinBg_2 = ResultPanel.Find("ResultFB/UI_JieSuan_BG_02");
            for (int i = 0; i < 3; i++)
            {
                bannerList.Add(resultFB.transform.Find("Banner_" + i).GetComponent<UISprite>());
                starlist.Add(resultFB.transform.Find("Banner_" + i).transform.Find("Tween").GetComponent<TweenPosition>());
            }
        }
        else
        {
            FailedPanel = ResultPanel.Find("FailedPanel");
            heroBtn.onClick = OnHeroBtnClick;
            equipBtn.onClick = OnEquipBtnClick;
            shopBtn.onClick = OnShopBtnClick;
            altarBtn.onClick = OnAltarBtnClick;
        }

        //leaveBtn.onClick = OnLeaveBtnClick;
        //fightingBtn.onClick = OnFightingBtnClick;

        if (!isMoba && isWin)
        {
            Invoke("ShowCloseAnAccount", intervalTime);
        }
        else
        {
            ShowPromptLabel();
        }

    }

    protected override void ShowHandler()
    {
        if (this.isMoba)
        {
            bg.enabled = false;
            mobaBG.enabled = true;
            if (isWin)
            {
                Result.spriteName = "shenglibiao";
                resultFB.gameObject.SetActive(false);
                sprite.gameObject.SetActive(true);
            }
            else
            {
                Result.spriteName = "shibaibiao";
                resultFB.gameObject.SetActive(false);
                sprite.gameObject.SetActive(false);
            }
        }
        else
        {
            bg.enabled = true;
            mobaBG.enabled = false;

            sceneNode = FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(GameLibrary.dungeonId);

            if (this.isWin)
            {
                Result.spriteName = "shenglibiao";
                resultFB.gameObject.SetActive(true);
                sprite.gameObject.SetActive(false);
                effWinBg_1.gameObject.SetActive(true);
                effWinBg_2.gameObject.SetActive(true);
                int star = Globe.GetStar(GameLibrary.star);
                StartCoroutine(Delays(star));
                StartCoroutine(DelayStar());

                int index = 0;
                foreach (bool item in GameLibrary.meetStar.Values)
                {
                    if (item)
                    {
                        bannerList[index].spriteName = "hongtiao";
                        bannerList[index].transform.Find("Tween/Star").GetComponent<UISprite>().spriteName = "xingxing";
                    }
                    else
                    {
                        bannerList[index].spriteName = "heitiao";
                        bannerList[index].transform.Find("Tween/Star").GetComponent<UISprite>().spriteName = "huisexingxing";
                    }
                    if (bannerList.Count > index && bannerList[index] != null && bannerList[index].transform.Find("Tween/Label") != null)
                    {
                        bannerList[index].transform.Find("Tween/Label").GetComponent<UILabel>().text = sceneNode.star_describe[index];
                    }
                    index++;
                }
            }
            else
            {
                Result.spriteName = "shibaibiao";
                sprite.gameObject.SetActive(false);
                effDefeat.gameObject.SetActive(true);
                FailedPanel.gameObject.SetActive(true);
            }
        }
        //弹出失败or成功面板 关闭副本对话框（避免穿透）
        Control.Hide(UIPanleID.UIFubenTaskDialogue);
    }

    private void OnBGClick(GameObject go)
    {
        if (!Globe.isFB && GameLibrary.isMoba)
        {
            ClientSendDataMgr.GetSingle().GetMobaSend().SendMobaResult(isWin ? 1 : 2);
            if (isWin) return;
        }
        OnLeaveBtnClick();
        if (null != sceneNode && sceneNode.Type > 2)
            SetBackName(UIPanleID.UIActivity);
    }

    void LoadDungeonsData()
    {
        //if (SceneBaseManager.instance.sceneType == SceneType.PVP3)//SceneManager.GetActiveScene().name == GameLibrary.PVP_Zuidui
        //{
        //    PropertyManager.Instance.StarTiming();
        //}
        if (!Globe.isFB)
            return;
        //  if (sceneNode.Type == 1 || sceneNode.Type == 2)
        //  ClientSendDataMgr.GetSingle().GetBattleSend().SendQueryDungeonList(new List<int> { sceneNode.bigmap_id }, sceneNode.Type);
        ClientSendDataMgr.GetSingle().GetHeroSend().SendGetHero(C2SMessageType.Active);
        Globe.isFB = false;
    }

    void ShowCloseAnAccount()
    {
        closeAnAccount.gameObject.SetActive(true);
        ResultPanel.GetComponent<TweenPosition>().enabled = true;

        if (Globe.isDungeonsUpgrade)
        {
            Invoke("ShowPlayerUpGradePanel", intervalTime);
        }
        else
        {
            Invoke("ShowPromptLabel", 0.5f);
        }
    }

    void ShowPlayerUpGradePanel()
    {
        Debug.Log("战斗奖励升级  " + playerData.GetInstance().beforePlayerLevel + "=====>" + playerData.GetInstance().selfData.level);
        Control.ShowGUI(UIPanleID.Upgrade, EnumOpenUIType.DefaultUIOrSecond);
        ShowPromptLabel();
        Globe.isDungeonsUpgrade = false;
    }

    void ShowPromptLabel()
    {
        //if (isMoba)
        //{
        if (null != promptLabel)
        {
            promptLabel.enabled = true;
            promptLabel.PlayForward();
        }
        UIEventListener.Get(isMoba ? mobaBG.gameObject : bg.gameObject).onClick += OnBGClick;

        //}
        //else
        //{
        //    leaveBtn.GetComponent<TweenAlpha>().PlayForward();
        //    fightingBtn.GetComponent<TweenAlpha>().PlayForward();
        //}
    }

    public IEnumerator Delays(int starNum)
    {

        if (starNum == 1)
        {//1行特效、腥腥的显示
            effStar_0.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            star_0.gameObject.SetActive(true);
        }//2/3星
        else if (starNum == 2)
        {
            effStar_0.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            star_0.gameObject.SetActive(true);
            //yield return new WaitForSeconds(0.6f);
            effStar_1.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            star_1.gameObject.SetActive(true);
        }
        else if (starNum == 3)
        {
            effStar_0.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            star_0.gameObject.SetActive(true);
            //yield return new WaitForSeconds(0.6f);
            effStar_1.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            star_1.gameObject.SetActive(true);
            // yield return new WaitForSeconds(0.6f);
            effStar_2.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            star_2.gameObject.SetActive(true);
        }

    }

    IEnumerator DelayStar()
    {
        starlist[0].enabled = true;
        yield return new WaitForSeconds(0.3f);
        starlist[1].enabled = true;
        yield return new WaitForSeconds(0.3f);
        starlist[2].enabled = true;
    }

    void OnLeaveBtnClick()
    {
        Singleton<SceneManage>.Instance.Current = EnumSceneID.UI_MajorCity01;
        //UI_Loading.LoadScene(GameLibrary.UI_Major, 2, LoadDungeonsData);
        GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
        StartLandingShuJu.GetInstance().GetLoadingData(GameLibrary.UI_Major, 2, LoadDungeonsData);
        SceneManager.LoadScene("Loding");
        GameLibrary.dungeonId = 0;
        GameLibrary.isPVP3 = false;
        //GameLibrary.isMoba = false;
        Debug.Log("The End !");
    }

    void OnFightingBtnClick()
    {
        if (sceneNode.Type > 2)
            SetBackName(UIPanleID.UIActivity);
        else
            SetBackName(UIPanleID.UILevel, sceneNode.bigmap_id, sceneNode.Type);
    }

    void OnHeroBtnClick()
    {
        SetBackName(UIPanleID.UIHeroList);
    }

    void OnEquipBtnClick()
    {
        SetBackName(UIPanleID.EquipDevelop);
    }

    void OnShopBtnClick()
    {
        SetBackName(UIPanleID.UIShopPanel);
    }

    void OnAltarBtnClick()
    {
        SetBackName(UIPanleID.UILottery);
    }

    void SetBackName(params object[] parms)
    {
        //Control.AddOrDeletFullScreenUI();
        Globe.backPanelParameter = parms;
        OnLeaveBtnClick();
    }



}