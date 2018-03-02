using UnityEngine;

public class MobaStatic : MonoBehaviour 
{
    //public GUISingleButton BtnStatic;
    public UILabel LaBlueScore;
    public UILabel LaRedScore;
    public UILabel LaTime;
    public UILabel LaKill;
    public UILabel LaAid;
    public UILabel LaDeath;
    public UILabel LaMorale;

    CDTimer.CD cd;

    public void Refresh ( CharacterData cd, int blueScore, int redScore)
    {
        LaBlueScore.text = blueScore.ToString();
        LaRedScore.text = redScore.ToString();
        LaKill.text = "" + cd.mobaKillCount;
        LaAid.text = "" + cd.mobaAidCount;
        LaDeath.text = "" + cd.mobaDeathCount;
        LaMorale.text = cd.mobaMorale + "%";
    }

    void Start ()
    {
        StartCD(int.MaxValue);
    }

    void OnClick ()
    {
        SceneUIManager.instance.MobaStatInfoGo.SetActive(true);
    }

    public void StartCD ( int total )
    {
        cd = CDTimer.GetInstance().AddCD(1f, RefreshCDLabel, total);
        cd.IsCountDown = false;
    }

    public void StopCD()
    {
        if (null != cd)
            cd.OnCd = null;
    }

    void RefreshCDLabel ( int countElapsed, long id )
    {
        LaTime.text = CDTimer.FormatToMMSS(countElapsed);
    }
}