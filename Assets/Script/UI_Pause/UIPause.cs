using UnityEngine;
using System.Collections;
using System;
using Tianyu;
using UnityEngine.SceneManagement;

public class UIPause : GUIBase
{
    public static UIPause instance;
    public GUISingleCheckBox musicCheckBox;
    public GUISingleCheckBox soundEffectCheckBox;
    public GUISingleButton quitBattleBtn;
    public GUISingleButton continueBattleBtn;

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIPause;
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }

    protected override void Init()
    {
        instance = this;
        musicCheckBox = transform.FindComponent<GUISingleCheckBox>("MusicCheckBox");
        soundEffectCheckBox = transform.FindComponent<GUISingleCheckBox>("SoundEffectCheckBox");
        quitBattleBtn = transform.FindComponent<GUISingleButton>("QuitBattleBtn");
        continueBattleBtn = transform.FindComponent<GUISingleButton>("ContinueBattleBtn");

        musicCheckBox.onClick = OnMusicCheckBoxClick;
        soundEffectCheckBox.onClick = OnSoundCheckBoxClick;
        //quitBattleBtn.onClick = OnQuitBattleBtnClick;
        continueBattleBtn.onButtonPress = OnContinueBattleBtnClick;
        quitBattleBtn.onButtonPress = OnQuitBattleBtnClick;

        if (SceneManager.GetActiveScene().name == GameLibrary.LGhuangyuan)
        {
            quitBattleBtn.text = "退出野外";
        }
        else
        {
            quitBattleBtn.text = "退出关卡";
        }

    }
    
    /// <summary>
    /// 继续战斗按钮事件
    /// </summary>
    private void OnContinueBattleBtnClick(bool isClick)
    {
        //cd继续
        CDTimer.GetInstance().CDRunOrStop(true);
        //时间缩放设置为1
        Time.timeScale = 1;
        //隐藏暂停面板
        Control.HideGUI(UIPanleID.UIPause);
    }

    /// <summary>
    /// 退出战斗按钮事件
    /// </summary>
    private void OnQuitBattleBtnClick(bool isClick)
    {
        //Globe.isHeroPlay = false;
        //Globe.isUpdate = false;
        if (SceneManager.GetActiveScene().name == "LGhuangyuan"|| SceneManager.GetActiveScene().name ==GameLibrary.PVP_1V1)
        {
            ClientSendDataMgr.GetSingle().GetLoginSend().SendChengeScene(playerData.GetInstance().selfData.mapID, 20000, 2);
            Globe.isC = false;
            Globe.isRefresh = false;
            Time.timeScale = 1;

            if (GameLibrary.isMoba) GameLibrary.isMoba = false;
            if (GameLibrary.isPVP3) GameLibrary.isPVP3 = false;
            if (Globe.isFB)
            {
                Globe.isFB = false;
            }
            Globe.isLoadOutCity = true;
            Control.HideGUI(UIPanleID.UIPause);
        }
        else
        {
            Time.timeScale = 1;

            if (!Globe.isFB && GameLibrary.isMoba)
            {
                ClientSendDataMgr.GetSingle().GetMobaSend().SendMobaResult(2);
                GameLibrary.isMoba = false;
            }

            if (Globe.isFB)
            {
                SceneBaseManager.instance.WinCondition(false, true);
                SceneBaseManager.instance.StopBaseAllCoroutinesAndInvok();
                GameLibrary.dungeonId = 0;
                Globe.isFB = false;
            }

            Globe.isC = false;
            Globe.isRefresh = false;
            
            if (GameLibrary.isPVP3) GameLibrary.isPVP3 = false;

            Control.HideGUI(UIPanleID.UIPause);
            Singleton<SceneManage>.Instance.Current = EnumSceneID.UI_MajorCity01;
            //UI_Loading.LoadScene(GameLibrary.UI_Major, 3);
            GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
            StartLandingShuJu.GetInstance().GetLoadingData(GameLibrary.UI_Major, 3);
            SceneManager.LoadScene("Loding");
        }
    }

    private void OnSoundCheckBoxClick(bool state)
    {

    }

    private void OnMusicCheckBoxClick(bool state)
    {

    }
}
