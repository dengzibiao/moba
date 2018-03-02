using UnityEngine;
using System.Collections;

public class UITowerDefence : MonoBehaviour
{

    public UILabel CampHP;
    public UILabel Wave;
    public UILabel Wavetime;
    public TweenPosition[] BossWarn;
    public TweenScale bgScale;
    public UISprite BossWave;
    public UISprite WaveSprite;
    public GameObject WaveEffect;

    bool isTowerDefence = false;

    float timeWave = 0;
    int currentWave = 1;
    int maxWave = 1;

    void Start()
    {
        Wavetime.enabled = false;
    }

    public void ShowUI(bool isTowerDefence)
    {
        this.isTowerDefence = isTowerDefence;
        gameObject.SetActive(true);
        CampHP.gameObject.SetActive(isTowerDefence);
    }

    public void RefreshCampHP(CharacterState cCs)
    {
        CampHP.text = (int)((float)cCs.currentHp / cCs.maxHp * 100) + "%";
    }

    public void RefreshWave(float time, int currentWave, int maxWave)
    {
        timeWave = time;
        this.currentWave = currentWave;
        this.maxWave = maxWave;

        if (!isTowerDefence)
        {
            currentWave -= 1;
            if (currentWave <= 0) currentWave = 1;
        }

        Wave.text = currentWave + "/" + maxWave;

        if (currentWave == maxWave && isTowerDefence)
        {
            //SceneUIManager.instance.bossWarning.ShowWarning(timeWave);
            ShowWarning(BossWave.gameObject);
            Invoke("ShowBossHPBar", timeWave);
        }
        else
        {
            if (currentWave > 1 && isTowerDefence)
                ShowWarning(bgScale.gameObject, currentWave);
        }

        if (timeWave > 0)
        {
            RefreshWavetime(timeWave);
        }
    }

    void ShowBossHPBar()
    {
        //GameLibrary.bossBlood.IsShow(true);
    }

    void RefreshWavetime(float time)
    {
        //timeWave = time;
        Wavetime.enabled = true;

        InvokeRepeating("UpdateTime", 0, 1);

    }

    void UpdateTime()
    {
        int minute = (int)(timeWave / 60);
        int second = (int)(timeWave % 60);

        if (minute < 0) minute = 0;
        if (second < 0) second = 0;

        Wavetime.text = (minute > 9 ? minute.ToString("0") : 0 + "" + minute) + ":" + (second > 9 ? second.ToString("0") : 0 + "" + second);
        timeWave--;

        if (timeWave == 0)
        {

            //WarningPanel.gameObject.SetActive(true);
            //WarningPanel.transform.FindChild("Label").GetComponent<UILabel>().text = "第" + currentWave + "波 亡灵来袭";
            //WarningPanel.PlayForward();
            //WarningPanel.ResetToBeginning();

        }
        else if (timeWave < 0)
        {
            if (!isTowerDefence)
            {
                if (currentWave == maxWave)
                {
                    ShowWarning(BossWave.gameObject);
                    ShowBossHPBar();
                }
                else
                {
                    ShowWarning(bgScale.gameObject, currentWave);
                }
                
            }
            Wave.text = currentWave + "/" + maxWave;
            Wavetime.enabled = false;
            CancelInvoke("UpdateTime");
        }

    }

    void ShowWarning(GameObject warning, int wave = -1)
    {

        if (wave == -1)
        {
            warning.SetActive(true);
            for (int i = 0; i < BossWarn.Length; i++)
            {
                BossWarn[i].enabled = true;
            }
        }
        else
        {
            bgScale.gameObject.SetActive(true);
            WaveEffect.SetActive(true);
            Invoke("ShowWaveSprite", 0f);
        }

        StartCoroutine(HideWarning(warning));
    }

    void ShowWaveSprite()
    {
        WaveSprite.spriteName = "di" + currentWave + "bo";
        bgScale.ResetToBeginning();
        bgScale.PlayForward();
    }

    IEnumerator HideWarning(GameObject warning)
    {
        if (bgScale.gameObject.activeSelf)
        {
            yield return new WaitForSeconds(1f);
            bgScale.PlayReverse();
            WaveEffect.SetActive(false);
        }
        else
        {
            for (int i = 0; i < BossWarn.Length; i++)
            {
                BossWarn[i].ResetToBeginning();
            }
        }
        
        yield return new WaitForSeconds(1f);
        warning.SetActive(false);
    }


}
