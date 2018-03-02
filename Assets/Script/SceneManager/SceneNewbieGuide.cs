using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using UnityEngine.SceneManagement;

public class SceneNewbieGuide : SceneBaseManager
{

    public new static SceneNewbieGuide instance;

    GameObject YinDao_GuangQuan;
    GameObject insYinDao;
    GameObject yd_guangq;
    GameObject insGuangQuan;
    GameObject promptArrow1;
    GameObject promptArrow2;
    GameObject guangquan;
    Transform center;

    FightTouch fightTouchIns;
    EasyTouchMove touchMove;
    NextGuidePanel guidePanel;
    GuideNode guideNode;

    TOUCH_KEY defKey;

    BoxCollider touchCollider;
    EffectBlock block;

    float distance;
    float p2airDis;
    int indexSpawn = 0;
    int count = 1;
    int summonIndex = 0;
    int guideIndex = 0;
    float rotateTimer = 0;

    bool Summon1 = true;
    bool isHideArrow = true;
    bool isForce = false;
    bool setSummon = true;
    bool touchRotate = false;
    bool isOverGuide = false;

    List<Monster_AI> elite = new List<Monster_AI>();

    GameObject mask;

    public override void StartCD()
    {
        if (!GameLibrary.isNetworkVersion)
            Globe.isFightGuide = true;
    }

    public override void InitScene()
    {
        instance = this;

        Globe.fightHero = new int[] { (int)GameLibrary.player, 201000300, 201001900, 201001100, 0, 0 };

        Resource.CreatPrefabs("HeroPosEmbattle", null, new Vector3(10, 1000, 0));

        guidePanel = NextGuidePanel.Single();
        guidePanel.transform.parent = SceneUIManager.instance.transform;
        guidePanel.transform.localScale = Vector3.one;

        insGuangQuan = Resources.Load(GameLibrary.Effect_UI + "yd_guangq") as GameObject;
        yd_guangq = NGUITools.AddChild(SceneUIManager.instance.gameObject, insGuangQuan);
        yd_guangq.SetActive(false);
        if (yd_guangq.transform.Find("guangquan"))
            guangquan = yd_guangq.transform.Find("guangquan").gameObject;

        YinDao_GuangQuan = transform.Find("BullockCarts").gameObject;
        insYinDao = Resource.CreatPrefabs("UI_YinDao_GuangQuan_01", YinDao_GuangQuan, Vector3.zero, GameLibrary.Effect_UI);

        promptArrow1 = Resource.CreatPrefabs("UI_YinDao_XiangQian_01", gameObject, Vector3.zero, GameLibrary.Effect_UI);
        promptArrow2 = Resource.CreatPrefabs("UI_YinDao_XiangQian_01", gameObject, Vector3.zero, GameLibrary.Effect_UI);
        SetArrowState(promptArrow1);
        SetArrowState(promptArrow2);

        fightTouchIns = FightTouch._instance;
        fightTouchIns.HideAllFightBtn();
        fightTouchIns.OnTouchBtn += HideGuide;
        fightTouchIns.OnBtnTargetNil += SummonTargetNil;

        touchMove = SceneUIManager.instance.moveTouch;
        touchMove.OnMove += TouchMove;

        touchCollider = SceneUIManager.instance.moveTouch.GetComponent<BoxCollider>();

        EffectBlock[] eb = GetComponentsInChildren<EffectBlock>();
        for (int i = 0; i < eb.Length; i++)
        {
            eb[i].OnCloseWall += (int num) =>
            {
                if (num > 2)
                    SetArrowState(promptArrow2, true, airWallPos.transform.position);
            };

            if (null == block && eb[i].transform.childCount == 2)
                block = eb[i];
        }

        for (int i = 0; i < Globe.fightHero.Length; i++)
        {
            if (Globe.fightHero[i] != 0)
                playerData.GetInstance().RefreshHeroToList(Globe.fightHero[i], 1, i == 0 ? 1 : 20);
        }

        CreateMainHero();

        for (int i = 0; i < Globe.Heros().Length; i++)
        {
            if (null != Globe.Heros()[i] && Globe.Heros()[i].id != 0)
            {
                Globe.Heros()[i].useServerAttr = false;
                Globe.Heros()[i].RefreshAttr();
            }
        }

        defKey = TOUCH_KEY.Run;
        ShowGuide(touchMove.transform, true);
        ShowGuidePanel(1);
        StartSpawn();
        SetArrowState(promptArrow1, true, player.transform.position);

        for (int i = 0; i < spwanList.Count; i++)
        {
            spwanList[i].isKM = false;
            if (spwanList[i].tag == Tag.boss)
            {
                spwanList[i].OnCreatMonster += (GameObject go, CharacterData cd) =>
                {
                    bossCs = go.GetComponent<CharacterState>();
                    TaskBossBlood();
                    go.GetComponent<CharacterState>().OnDead += (CharacterState cs) => { BossDead(); };
                };
            }
        }

        ReadTask(500);
        EnterDungensTask();

        ThirdCamera.instance._flatAngle = FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(GameLibrary.dungeonId).flat_angle;

        mask = SceneUIManager.instance.transform.Find("GuideMask").gameObject;
    }

    public override void WinCondition(bool isWin, bool isBackMajor = false)
    {

    }

    void BossDead()
    {
        Time.timeScale = 0.2f;
        SceneUIManager.instance.SetMaskPanel(true);
        Invoke("ShowCreateNamePanel", 1f);
    }

    void ShowCreateNamePanel()
    {
        Time.timeScale = 1;
        SceneUIManager.instance.SetMaskPanel(false);
        Control.ShowGUI(UIPanleID.UIFubenTaskDialogue, EnumOpenUIType.DefaultUIOrSecond, false, 72);
        UIFubenTaskDialogue.instance.DialogEnd += DialogEnd;
        //Control.ShowGUI(GameLibrary.UICreateName);
    }

    void DialogEnd()
    {
        if (isOverGuide)
        {
            Globe.isFightGuide = false;
            Globe.isFB = false;
            GameLibrary.dungeonId = 0;
            playerData.GetInstance().herodataList.Clear();
            GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
            StartLandingShuJu.GetInstance().GetLoadingData(string.IsNullOrEmpty(Globe.FightGuideSceneName) ? GameLibrary.UI_Major : Globe.FightGuideSceneName, 3);
            SceneManager.LoadScene("Loding");
        }
        else
        {

            Control.ShowGUI(UIPanleID.UICreateName, EnumOpenUIType.DefaultUIOrSecond);
        }
        if (null != UIFubenTaskDialogue.instance)
            UIFubenTaskDialogue.instance.DialogEnd -= DialogEnd;
    }

    public void CreateNameOver()
    {
        isOverGuide = true;
        Control.HideGUI(UIPanleID.UICreateName);
        Control.ShowGUI(UIPanleID.UIFubenTaskDialogue, EnumOpenUIType.DefaultUIOrSecond, false, 73);
        UIFubenTaskDialogue.instance.DialogEnd += DialogEnd;
    }

    void FixedUpdate()
    {
        if (null != insYinDao && insYinDao.activeSelf)
        {
            distance = Vector3.Distance(player.transform.position, insYinDao.transform.position);
            if (distance <= 1f)
            {
                insYinDao.gameObject.SetActive(false);
                ShowGuidePanel(2);
                SetTouchState(false);
                Invoke("GuideAttack", 1);
                Invoke("RefreshSpawn", 1f);
            }
        }

        if (enemy.size <= 0 && count <= 0)
        {
            count = 1;
            Invoke("RefreshSpawn", 0.3f);
        }

        if (player.currentHp != 0)
        {
            if (player.currentHp < player.maxHp * 0.7f)
            {
                player.DamagePercent = 0;
                SetMonsterAttack();
            }
            else
                player.DamagePercent = 1;
        }

        if (!Summon1 && player.currentHp != 0 && player.currentHp <= player.maxHp * 0.70f)
        {
            SummonHero();
        }

        if (GameLibrary.isBossChuChang && isHideArrow)
        {
            isHideArrow = false;
            SetArrowState(promptArrow1);
            SetArrowState(promptArrow2);
        }

        if (isForce)
        {
            ForceToMove();
        }

        if (touchRotate && guideIndex == 1)
        {
            RotateAround();
        }
    }

    void RefreshSpawn()
    {
        indexSpawn++;
        count = 0;
        if (indexSpawn == 2)
        {
            defKey = TOUCH_KEY.Skill4;
            ShowGuidePanel(4);
            ShowGuide(fightTouchIns.ShowFightBtn(TOUCH_KEY.Skill4));
        }
        for (int i = 0; i < spwanList.Count; i++)
        {
            if (spwanList[i].distance == indexSpawn && spwanList[i].tag != Tag.boss)
            {
                count++;
                spwanList[i].OnCreatMonster += (GameObject go, CharacterData cd) =>
                {
                    if (indexSpawn == 3)
                    {
                        if (setSummon)
                        {
                            setSummon = false;
                            Summon1 = false;
                            //Invoke("SummonHero", 5f);
                        }
                        elite.Add(go.GetComponent<Monster_AI>());
                    }
                    go.GetComponent<CharacterState>().OnDead += (CharacterState cs) => { count--; };
                };
                spwanList[i].StartSpawn();
            }
        }
        if (elite.Count > 0 && player.currentHp <= player.maxHp * 0.7f)
        {
            SetMonsterAttack();
        }
    }

    void SummonHero()
    {
        CancelInvoke("SummonHero");
        fightTouchIns.ClearAttackBtn();
        block.ChangeCount();
        Summon1 = true;
        defKey = TOUCH_KEY.Summon1;
        summonIndex = 5;
        //Invoke("InvokeSummon", 5f);
        ShowGuidePanel(5);
        fightTouchIns.ShowFightBtn(TOUCH_KEY.Summon1);
        ForciblySummonGuide(defKey);
    }

    void GuideAttack()
    {
        SetTouchState(true);
        defKey = TOUCH_KEY.Attack;
        ShowGuidePanel(3);
        ShowGuide(FightTouch._instance.ShowFightBtn(TOUCH_KEY.Attack));
    }

    void ShowGuide(Transform pos, bool isRatete = false)
    {
        if (null == pos) return;
        center = pos;
        yd_guangq.transform.position = pos.transform.position;
        if (null != guangquan)
        {
            guangquan.SetActive(true);
            if (guangquan.activeSelf)
                yd_guangq.GetComponent<RenderQueueModifier>().GetRender();
        }
        yd_guangq.transform.GetComponentInChildren<Animator>().enabled = true;
        yd_guangq.GetComponent<RenderQueueModifier>().m_target = pos.GetComponent<UISprite>();
        yd_guangq.SetActive(true);

        if (isRatete)
        {
            CDTimer.GetInstance().AddCD(1f, (int count, long cid) =>
            {
                yd_guangq.transform.GetComponentInChildren<Animator>().enabled = false;
                yd_guangq.transform.GetComponentInChildren<Animator>().transform.localPosition = new Vector3(40.3f, -36.7f, 0);
                guangquan.SetActive(false);
                yd_guangq.transform.position = center.position + new Vector3(-0.15f, 0, 0);
                touchRotate = true;
            });
        }
    }

    void HideGuide(TOUCH_KEY key)
    {
        if (yd_guangq.activeSelf && defKey == key)
            yd_guangq.SetActive(false);

        if (key == TOUCH_KEY.Run)
        {
            touchRotate = false;
        }
        else if (key == TOUCH_KEY.Attack)
        {
            if (guideIndex == 3)
            {
                CloseGuidePanel();
                guideIndex = 0;
            }
        }
        else if (key == TOUCH_KEY.Skill4)
        {
            if (guideIndex == 4)
            {
                CloseGuidePanel();
                guideIndex = 0;
            }
        }
        else if (key == TOUCH_KEY.Summon1)
        {
            HideForcibly();

            //CancelInvoke("InvokeSummon");
            for (int i = 0; i < elite.Count; i++)
            {
                elite[i].StartEscape(airWallPos);
            }
            PlotLinesNode plot = FSDataNodeTable<PlotLinesNode>.GetSingleton().FindDataByType(71);
            SceneUIManager.instance.InsDialogBubble(elite[0].gameObject, plot.Content, plot.Intervaltime);
            RetuenAttr();

            CharacterState cs = null;
            for (int i = 0; i < elite.Count; i++)
            {
                //elite[i].GetComponent<CharacterState>().InitHp(1000);
                cs = elite[i].GetComponent<CharacterState>();
                if (null != cs)
                    cs.Hp(cs.currentHp > 1000 ? cs.currentHp - 1000 : 1000 - cs.currentHp);
            }

            CDTimer.GetInstance().AddCD(2, (int count, long cid) =>
            {
                defKey = TOUCH_KEY.Summon2;
                summonIndex = 6;
                //Invoke("InvokeSummon", 5f);
                ShowGuidePanel(6);
                fightTouchIns.ShowFightBtn(TOUCH_KEY.Summon2);
                ForciblySummonGuide(defKey);
            });
        }
        else if (key == TOUCH_KEY.Summon2)
        {
            HideForcibly();
            //CancelInvoke("InvokeSummon");
            CDTimer.GetInstance().AddCD(2, (int count, long cid) =>
            {
                //Invoke("InvokeSummon", 5f);
                defKey = TOUCH_KEY.Summon3;
                summonIndex = 7;
                ShowGuidePanel(7);
                fightTouchIns.ShowFightBtn(TOUCH_KEY.Summon3);
                ForciblySummonGuide(defKey);
            });
        }
        else if (key == TOUCH_KEY.Summon3)
        {
            //CancelInvoke("InvokeSummon");
            HideForcibly(true);
            //Globe.isFightGuide = false;
            fightTouchIns.OnBtnTargetNil -= SummonTargetNil;
            fightTouchIns.OnTouchBtn -= HideGuide;
            CloseGuidePanel();
        }
        FightTouch._instance.HideSummonHero(key);
    }

    void SummonTargetNil(TOUCH_KEY key)
    {
        switch (key)
        {
            case TOUCH_KEY.Summon1:
                summonIndex = 5;
                break;
            case TOUCH_KEY.Summon2:
                summonIndex = 6;
                break;
            case TOUCH_KEY.Summon3:
                summonIndex = 7;
                break;
        }
        isForce = true;
    }

    void InvokeSummon()
    {
        if (null == player.attackTarget)
        {
            isForce = true;
        }
        else
        {
            fightTouchIns.InvokeSummon(summonIndex);
        }
    }

    void TouchMove(float timeLast)
    {
        touchMove.OnMove -= TouchMove;
        CloseGuidePanel();
        HideGuide(TOUCH_KEY.Run);
    }

    void SetArrowState(GameObject arrow, bool isShow = false, Vector3 pos = default(Vector3))
    {
        if (isShow)
        {
            arrow.transform.rotation = Quaternion.Euler(0, -50, 0);
            arrow.transform.position = pos;
            arrow.SetActive(true);
        }
        else
        {
            arrow.SetActive(false);
        }
    }

    void SetTouchState(bool isEnable)
    {
        touchCollider.enabled = isEnable;
    }

    void ForceToMove()
    {
        p2airDis = Vector3.Distance(player.transform.position, elite[0].transform.position);
        if (p2airDis < 2 || null != player.attackTarget)
        {
            player.pm.isAutoMode = false;
            player.pm.Stop();
            player.SetAttackTargetTo(elite[0].GetComponent<CharacterState>());
            InvokeSummon();
            fightTouchIns.SetFightBtnStatus(true);
            SetTouchState(true);
            isForce = false;
        }
        else
        {
            if (touchCollider.enabled)
            {
                fightTouchIns.SetFightBtnStatus(false);
                SetTouchState(false);
            }
            player.pm.isAutoMode = true;
            if (player.moveSpeed == 0)
                player.moveSpeed = player.moveInitSpeed;
            player.pm.Approaching(elite[0].transform.position);
        }
    }

    void ShowGuidePanel(int guideid)
    {
        guideNode = FSDataNodeTable<GuideNode>.GetSingleton().FindDataByType(guideid);
        guideIndex = guideid;
        guidePanel.Init(guideIndex);

        AudioController.Instance.PlayUISound(GameLibrary.Resource_GuideSound + guideNode.voice, true);
    }

    void CloseGuidePanel()
    {
        CDTimer.GetInstance().AddCD(0.5f, (int count, long id) => { guidePanel.Close(); });
    }

    void RotateAround()
    {
        if (null == center)
        {
            touchRotate = false;
            return;
        }

        rotateTimer += Time.deltaTime;

        if (rotateTimer >= 2)
        {
            touchRotate = false;
            ShowGuide(center, true);
            rotateTimer = 0;
            return;
        }

        yd_guangq.transform.RotateAround(center.position, new Vector3(0, 0, 1), 200 * Time.deltaTime);
        yd_guangq.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    Dictionary<CharacterAttrNode, float> attrNode = new Dictionary<CharacterAttrNode, float>();
    bool setAttr = false;
    void SetMonsterAttack()
    {
        if (setAttr) return;
        setAttr = true;
        float attack = 0f;
        for (int i = 0; i < elite.Count; i++)
        {
            attack = Formula.GetAttr(elite[i].GetComponent<CharacterState>().CharData.attrNode.base_Propers, AttrType.attack);
            if (!attrNode.ContainsKey(elite[i].GetComponent<CharacterState>().CharData.attrNode))
                attrNode.Add(elite[i].GetComponent<CharacterState>().CharData.attrNode, attack);
            Formula.SetAttrTo(ref elite[i].GetComponent<CharacterState>().CharData.attrNode.base_Propers, AttrType.attack, 0);
        }
    }

    void RetuenAttr()
    {
        List<CharacterAttrNode> attr = new List<CharacterAttrNode>(attrNode.Keys);
        for (int i = 0; i < attrNode.Count; i++)
        {
            Formula.SetAttrTo(ref attr[i].base_Propers, AttrType.attack, attrNode[attr[i]]);
        }
    }

    #region 强制召唤英雄引导

    GameObject copyBtn = null;
    void ForciblySummonGuide(TOUCH_KEY key)
    {
        SkillBtnCD summonBtn = null;
        switch (key)
        {
            case TOUCH_KEY.Summon1:
                summonBtn = fightTouchIns.summon1;
                break;
            case TOUCH_KEY.Summon2:
                summonBtn = fightTouchIns.summon2;
                break;
            case TOUCH_KEY.Summon3:
                summonBtn = fightTouchIns.summon3;
                break;
        }

        if (null == summonBtn)
        {
            Debug.Log("summonBtn is null");
            return;
        }

        copyBtn = Instantiate(summonBtn.gameObject) as GameObject;

        if (null == copyBtn)
        {
            Debug.Log("copyBtn is null");
            return;
        }
        copyBtn.transform.parent = mask.transform;
        copyBtn.transform.localScale = Vector3.one;
        copyBtn.transform.position = summonBtn.transform.position;
        copyBtn.transform.FindComponent<UISprite>("CD").enabled = false;
        ShowGuide(copyBtn.transform);

        if (mask.activeSelf)
        {
            mask.transform.Find("Sprite").GetComponent<UISprite>().alpha = 1f;
        }
        else
        {
            mask.SetActive(true);
        }

        fightTouchIns.CopySummonBtn(copyBtn.GetComponent<SkillBtnCD>(), key);

        for (int i = 0; i < elite.Count; i++)
        {
            elite[i].GetComponent<CharacterState>().moveSpeed = 0;
        }

        player.moveSpeed = 0;
    }

    void HideForcibly(bool hideMaks = false)
    {
        if (hideMaks)
            mask.SetActive(false);
        else
            mask.transform.Find("Sprite").GetComponent<UISprite>().alpha = 0.1f;

        Destroy(copyBtn.gameObject);
        CharacterState cs = null;
        for (int i = 0; i < elite.Count; i++)
        {
            cs = elite[i].GetComponent<CharacterState>();
            if (null != cs)
            {
                cs.moveSpeed = cs.moveInitSpeed;
            }
        }
        player.moveSpeed = player.moveInitSpeed;
    }

    #endregion

}