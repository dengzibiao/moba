using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneUIManager : MonoBehaviour
{
    public static SceneUIManager instance;

    public EasyTouchMove moveTouch;
    public FightTouch fightTouch;

    public UIRole uiRole;
    public GameObject autoMove;
    public GameObject autoFight;
    public BattleCDandScore scoreCD;

    public BossWarning bossWarning;
    public BossBlood bossBlood;
    public ArrowSetting guideArrow;
    public GameObject bubble;

    public UITowerDefence TowerDefence;
    public UITowerPoint TowerPoint;

    public Delay startBattleDelay;
    public UIArenaPanel arenaPanel;

    public MobaMiniMap mobaMiniMap;
    public RebornPanel pnReborn;
    public UILabel laChubingCD;
    public MobaKOInfoPanel MobaKoInfo;
    public UIGrid heroRebornThumb;
    GameObject startGamePrompt;
    public GamePrompt gamePrompt;

    [HideInInspector]
    public FlopPanel FlopCardPanel;
    [HideInInspector]
    public GameObject MobaStatInfoGo;
    [HideInInspector]
    public UIArenaWinPanel arenaWinPanel;
    public GameObject ACTCounter;

    public UIPanel MaskPanel;

    GameObject bloodScreen;
    GameObject taskBubble;
    GameObject ArenaDragCam;


    int chubingCd = 10;

    void Awake()
    {
        instance = this;
        if (SceneManager.GetActiveScene().name == "Loding")
        {
            return;
        }
        else {

            if (transform.Find("ArenaDragCam") != null)
            {
                ArenaDragCam = transform.Find("ArenaDragCam").gameObject;
                if (Camera.main.GetComponentInChildren<UIDraggableCamera>() != null)
                {
                    UIDraggableCamera dc = ArenaDragCam.GetComponentInChildren<UIDragCamera>().draggableCamera = Camera.main.GetComponentInChildren<UIDraggableCamera>();
                    if (GameLibrary.SceneType(SceneType.PVP3))
                        dc.SetDragBounds(8.25f, 2, 2, 0.78f, -90);
                    else
                        dc.SetDragBounds(20, -20, 20, -20);
                }
            }

            bloodScreen = transform.FindChild("PanelUIEffects/GamePromptPanel/UIBloodScreen").gameObject;
            // ACTCounter = transform.FindChild("ACTCounter").gameObject;

            if (transform.FindChild("TaskBubble") != null)
                taskBubble = transform.FindChild("TaskBubble").gameObject;

            SwitchBloodScreen(false);

            tas = laChubingCD.GetComponents<TweenAlpha>();

            FlopCardPanel = AddUIGo(GameLibrary.UIFlopCardPanel).GetComponent<FlopPanel>();
            MobaStatInfoGo = AddUIGo(GameLibrary.UIMobaStatInfo);
            arenaWinPanel = AddUIGo(GameLibrary.UIArenaWinPanel).GetComponent<UIArenaWinPanel>();
            startGamePrompt = AddUIGo(GameLibrary.UIGamePromptPanel);

            if (Globe.isFightGuide)
            {
                AddUIGo("GuideMask");
            }

            if (null != SceneBaseManager.instance)
            {
                if (SceneBaseManager.instance.sceneType == SceneType.FD_LG)
                {
                    Control.ShowGUI(UIPanleID.UITaskTracker, EnumOpenUIType.DefaultUIOrSecond);
                    Control.ShowGUI(UIPanleID.UIDeadToReborn, EnumOpenUIType.DefaultUIOrSecond);
                    Control.ShowGUI(UIPanleID.UITaskEffectPanel, EnumOpenUIType.DefaultUIOrSecond);
                }
                ShowUIByScene(SceneBaseManager.instance.sceneType);
            }
        }

    }

    GameObject AddUIGo(string goPath, string goName = "")
    {
        GameObject uiGo = NGUITools.AddChild(gameObject, Resources.Load<GameObject>(GameLibrary.PATH_UIPrefab + goPath));
        uiGo.name = string.IsNullOrEmpty(goName) ? goPath : goName;
        uiGo.SetActive(false);
        return uiGo;
    }

    void ShowUIByScene(SceneType type)
    {
        bool isMoba = type == SceneType.MB1 || type == SceneType.MB3 || type == SceneType.MB5 || type == SceneType.Dungeons_MB1;
        bool isTPorMoba = type == SceneType.TP || isMoba;
        bool isNotDungeon = isMoba || type == SceneType.PVP3 || type == SceneType.FD_LG;
        bool isACT = type == SceneType.ACT_AGILE || type == SceneType.ACT_POWER || type == SceneType.ACT_INTEL || type == SceneType.ACT_GOLD || type == SceneType.ACT_EXP;

        uiRole.rideIcon.gameObject.SetActive(type == SceneType.FD_LG);
        uiRole.petIcon.gameObject.SetActive(type == SceneType.FD_LG);
        uiRole.gameObject.SetActive(!isTPorMoba && type != SceneType.PVP3);
        if (mobaMiniMap != null)
        {
            mobaMiniMap.gameObject.SetActive(isTPorMoba);
        }
        fightTouch.tpBtn.gameObject.SetActive(isMoba);
        fightTouch.progress.gameObject.SetActive(isMoba);
        fightTouch.mobaStatic.gameObject.SetActive(isMoba);
        Vector3 pos = fightTouch.pauseBtn.transform.localPosition;
        if (type == SceneType.FD_LG)
            fightTouch.pauseBtn.transform.localPosition = new Vector3(85, 95f, pos.z);
        else if (type == SceneType.TP || type == SceneType.Dungeons_MB1)
        {
            fightTouch.pauseBtn.transform.localPosition = new Vector3(0, -30, pos.z);
            fightTouch.getStarDisBtn.transform.localPosition = new Vector3(0, -110, pos.z);
            fightTouch.overBtn.transform.localPosition = new Vector3(90, -30, pos.z);
        }
        else if (isTPorMoba)
            fightTouch.pauseBtn.transform.localPosition = new Vector3(pos.x, -30f, pos.z);
        else
            fightTouch.pauseBtn.transform.localPosition = new Vector3(pos.x, 50f, pos.z);

        startBattleDelay.gameObject.SetActive(type == SceneType.PVP3);
        arenaPanel.gameObject.SetActive(type == SceneType.PVP3);
        ArenaDragCam.gameObject.SetActive(type == SceneType.PVP3);

        scoreCD.gameObject.SetActive(isACT);
        moveTouch.gameObject.SetActive(type != SceneType.PVP3);
        fightTouch.GetComponent<UIRect>().alpha = type == SceneType.PVP3 ? 0 : 1;
        fightTouch.getStarDisBtn.gameObject.SetActive(type == SceneType.Dungeons_MB1 || !isNotDungeon);
        fightTouch.overBtn.gameObject.SetActive(!((type == SceneType.BO || (type != SceneType.Dungeons_MB1 && isNotDungeon)) || !Globe.isOverBtn));
        guideArrow.gameObject.SetActive(!isNotDungeon);

        TowerPoint.gameObject.SetActive(type == SceneType.TP);
        TowerDefence.gameObject.SetActive(false);
        bossBlood.transform.FindChild("root").gameObject.SetActive(false);

        if (type == SceneType.TD)
            TowerDefence.ShowUI(true);
        if (type == SceneType.ACT_AGILE || type == SceneType.ACT_POWER || type == SceneType.ACT_INTEL)
            TowerDefence.ShowUI(false);
        ACTCounter.SetActive(type == SceneType.ACT_GOLD || type == SceneType.ACT_EXP);

        heroRebornThumb.gameObject.SetActive(type == SceneType.MB3);
        if (isTPorMoba)
        {
            chubingCd = 5;
            InvokeRepeating("RefreshChubingCD", 5f, 1f);
        }
    }

    public void SwitchBloodScreen(bool b)
    {
        if (SceneBaseManager.instance.sceneType == SceneType.PVP3)
            return;
        if (bloodScreen.activeSelf != b)
            bloodScreen.SetActive(b);
        bloodScreen.GetComponent<TweenAlpha>().ResetToBeginning();
        foreach (Transform item in bloodScreen.transform)
            item.gameObject.SetActive(false);
    }

    void RefreshChubingCD()
    {
        chubingCd--;
        if (chubingCd == 5)
            FadeChubingCDLabel(string.Format(Localization.Get("ChubingCD"), chubingCd));
        if (chubingCd < 1)
        {
            FadeChubingCDLabel(Localization.Get("ChubingCDOver"));
            CancelInvoke("RefreshChubingCD");
        }
    }

    TweenAlpha[] tas;
    void FadeChubingCDLabel(string str)
    {
        laChubingCD.gameObject.SetActive(true);
        laChubingCD.text = str;
        for (int i = 0; i < tas.Length; i++)
        {
            tas[i].enabled = true;
            if (tas[i].from == 0)
                tas[i].ResetToBeginning();
            tas[i].PlayForward();
        }
    }

    public void HideUI()
    {
        uiRole.gameObject.SetActive(false);
        mobaMiniMap.gameObject.SetActive(false);
        arenaPanel.gameObject.SetActive(false);
        fightTouch.gameObject.SetActive(false);
        moveTouch.gameObject.SetActive(false);
    }

    public void InsDialogBubble(GameObject target, string text, float time)
    {
        GameObject go = NGUITools.AddChild(taskBubble, bubble);
        if (go.GetComponent<TaskBubbleItem>())
            go.GetComponent<TaskBubbleItem>().DialogBubble(target, text, time);
    }

    public void RefreshACTCounter(int count)
    {
        if (null == ACTCounter && !ACTCounter.gameObject.activeSelf) return;
        ACTCounter.transform.Find("Label").GetComponent<UILabel>().text = "x " + count;
    }

    public void SetMaskPanel(bool isEnable)
    {
        MaskPanel.gameObject.SetActive(isEnable);
        if (isEnable)
            MaskPanel.depth = UIPanel.nextUnusedDepth;
    }

}