using UnityEngine;
using System.Collections;

public class UIPVPPlayer : MonoBehaviour
{

    int pid;
    public UILabel playerName;
    public UILabel rankL;
    public UILabel lvlL;
    public UILabel fcL;
    public GUISingleButton dearBtn;

    public ItemHeroLineUp[] HeroList;
    public Transform[] heroPos;

    [HideInInspector]
    public bool isSelf = false;

    UIEmbattle embattle;

    HeroData[] heroData = new HeroData[6];

    UIAbattiorList uiabattior;

    ArenaHero hero;

    void Start()
    {
        uiabattior = GetComponentInParent<UIAbattiorList>();
        //EventDelegate.Set(dearBtn.onClick, OnAdjustBtnClick);
        dearBtn.onClick += OnAdjustBtnClick;
    }

    public void RefreshUI(HeroData[] playHeroList)
    {
        if (null == playHeroList)
            return;

        SetHeroPos(Globe.adFightHero[5]);

        for (int i = 0; i < playHeroList.Length - 1; i++)
        {
            heroData[i] = playHeroList[i];
        }

        playerName.text = "防守阵容";
        rankL.text = "[5eaeff]排名：[-]" + playerData.GetInstance().selfData.rank;
        lvlL.text = "[5eaeff]等级：[-]" + playerData.GetInstance().selfData.level;
        int sumFC = 0;

        for (int i = 0; i < heroData.Length; i++)
        {
            if (null != heroData[i] && heroData[i].id != 0)
                sumFC += heroData[i].fc;
        }

        fcL.text = "[5eaeff]战力：[-]" + sumFC;

        RefreshData(Globe.adFightHero[5], heroData);
    }

    public void RefreshUID(ArenaHero hero = null)
    {
        if (null == hero)
        {
            playerName.text = "";
            rankL.text = "排名：";
            lvlL.text = "等级：";
            fcL.text = "战力：";
            SetHeroPos(1);
            for (int i = 0; i < HeroList.Length; i++)
            {
                RefreshIcon(HeroList[i], null);
            }
            dearBtn.isEnabled = false;
            dearBtn.SetState(GUISingleButton.State.Disabled);
            //gameObject.SetActive(false);
            return;
        }
        //gameObject.SetActive(true);
        this.hero = hero;
        pid = hero.pid;
        playerName.text = hero.nm;
        rankL.text = "排名：" + hero.rk;
        lvlL.text = "等级：" + hero.lvl;
        fcL.text = "战力：" + hero.fc;
        //Globe.InitHeroSort(hero.herolist);
        SetHeroPos(hero.heroState);
        RefreshData(hero.heroState, hero.herolist);
        dearBtn.isEnabled = true;
        dearBtn.SetState(GUISingleButton.State.Normal);
    }

    void RefreshIcon(ItemHeroLineUp icon, HeroData hd)
    {
        if (null != hd && hd.id != 0)
            icon.RefreshUI(hd, false);
        else
            icon.RefreshUI(null, false);
    }

    void OnAdjustBtnClick()
    {

        if (isSelf)
        {
            ShowEmbattlePanel(OpenSourceType.ArenaDefen);
        }
        else
        {
            if (!uiabattior.CanDare()) return;

            ShowEmbattlePanel(OpenSourceType.Arena);

            //UIEmbattle.sourceType = OpenSourceType.Arena;
            if (null == embattle)
                embattle = UIEmbattle.instance;
            ClearCallBack();
            embattle.OnConfirm += OnConfirmLineup;

            //Control.HideGUI(GameLibrary.UIAbattiorLis);
        }

    }

    void ShowEmbattlePanel(OpenSourceType type)
    {
        Control.HideGUI(UIPanleID.UIAbattiorList);
        Control.ShowGUI(UIPanleID.UIEmbattle, EnumOpenUIType.OpenNewCloseOld, false, type);
        if (null == embattle)
            embattle = UIEmbattle.instance;
        ClearCallBack();
        //embattle.OnConfirm += OnConfirm;
    }

    void OnConfirm(bool isDefine)
    {
        //Control.ShowGUI(GameLibrary.UIAbattiorLis);

        RefreshUI(Globe.defendTeam);
    }

    void OnConfirmLineup(bool isDefine)
    {
        if (isDefine)
        {

            for (int i = 0; i < heroData.Length; i++)
            {
                if (null != heroData[i])
                    Globe.ArenaEnemy[i] = heroData[i];
            }

            Globe.arenahero = hero;
            ClientSendDataMgr.GetSingle().GetBattleSend().SendInitArenaFighting(pid, playerData.GetInstance().GetHeroFormationIds(Globe.challengeTeam, true));
        }
        else
        {
            ClearCallBack();
            //Control.ShowGUI(GameLibrary.UIAbattiorLis);
        }
    }

    public void ClearCallBack()
    {
        if (null != embattle && null != embattle.OnConfirm)
        {
            embattle.OnConfirm = null;
        }
    }

    void SetHeroPos(int index)
    {
        if (null == heroPos) return;
        if ((index == 1 && heroPos[0].localPosition.y < heroPos[1].localPosition.y) || (index == 2 && heroPos[0].localPosition.y > heroPos[1].localPosition.y))
        {
            Vector3 pos = default(Vector3);
            pos = heroPos[0].localPosition;
            heroPos[0].localPosition = heroPos[1].localPosition;
            heroPos[1].localPosition = pos;
        }
    }

    void RefreshData(int site, HeroData[] heroData)
    {
        if (site == 1)
        {
            int index = 2;
            for (int i = 0; i < 3; i++)
            {
                RefreshIcon(HeroList[index], heroData[i]);
                index++;
            }
            index = 0;
            for (int i = 3; i < HeroList.Length; i++)
            {
                RefreshIcon(HeroList[index], heroData[i]);
                index++;
            }
        }
        else
        {
            for (int i = 0; i < HeroList.Length; i++)
            {
                RefreshIcon(HeroList[i], heroData[i]);
            }
        }
    }

}
