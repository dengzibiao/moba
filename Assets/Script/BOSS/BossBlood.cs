using UnityEngine;
using System.Collections.Generic;
using Tianyu;


public class BossBlood : MonoBehaviour
{
    #region gameObject
    //private List<UISprite> LisSpr = new List<UISprite>();
    private UILabel LabCount;
    private GameObject goRoot;
    private UILabel namelvl;

    public UISprite FgShow;
    public UISprite BgShow;
    public UISprite SpTrack;
    public UISprite SpTrackBg;

    int mBarCount;
    float mMaxHp;
    float mCurrentHp;
    int currentBarCount;
    #endregion

    void Awake()
    {
        GameLibrary.bossBlood = this;
        goRoot = UnityUtil.FindObjectRecursively(this.gameObject, "root");
        namelvl = UnityUtil.FindCtrl<UILabel>(goRoot, "namelvl");
        LabCount = UnityUtil.FindCtrl<UILabel>(goRoot, "number");
    }

    bool inited = false;
    float trackSpeedRatio = 0.05f;
    float trackStep = 0f;
    float trackStepFaster = 0f;
    float trackFrom = 0f;
    float trackTo = 0f;
    CharacterState currentBoss;

    public void RefreshBlood ( float cur)
    {
        float hpPerBar = mMaxHp / mBarCount; //每条血的值
        mCurrentHp = cur;
        trackTo = mBarCount* mCurrentHp / mMaxHp;
        currentBarCount = Mathf.CeilToInt(mCurrentHp / hpPerBar);
        float hpCurrentBar = mCurrentHp - ( currentBarCount - 1 ) * hpPerBar;
        if(currentBarCount <= 0)
        {
            BgShow.fillAmount = 0f;
            FgShow.fillAmount = 0f;
            return;
        }
        LabCount.text = "x" + currentBarCount;
        FgShow.fillAmount = Mathf.Clamp(hpCurrentBar / hpPerBar, 0f, 1f);
        int colorId = mBarCount - currentBarCount;
        FgShow.spriteName = "bossxuetiao-" + barColors[barColorIDs[colorId]];
        if(currentBarCount > 1)
        {
            BgShow.fillAmount = 1f;
            BgShow.spriteName = "bossxuetiao-" + barColors[barColorIDs[colorId + 1]];
        }
        else
        {
            BgShow.fillAmount = 0f;
        }
    }

    void SetTrackSp ()
    {
        int colorId = mBarCount - Mathf.CeilToInt(trackFrom);
        if(trackFrom > 0)
        {
            SpTrack.spriteName = "bossxuetiao-" + barColors[barColorIDs[colorId]];
            SpTrackBg.fillAmount = SpTrack.fillAmount = trackFrom - Mathf.FloorToInt(trackFrom);
        }
        else
        {
            SpTrackBg.fillAmount = SpTrack.fillAmount = 0f;
        }
        SpTrack.depth = trackFrom > currentBarCount ? FgShow.depth + 2 : FgShow.depth - 1;
        SpTrackBg.depth = trackFrom > currentBarCount ? FgShow.depth + 1 : FgShow.depth - 2;
    }

    void FixedUpdate ()
    {
        if(trackStep == 0)
        {
            if(trackFrom > trackTo)
            {
                trackStep = trackSpeedRatio * ( trackFrom - trackTo );
            }
            else
            {
                trackFrom = trackTo;
                SetTrackSp();
                trackStep = 0;
            }
        }
        else
        {
            trackFrom -= trackStep;
            SetTrackSp();
            if(trackFrom <= trackTo)
            {
                trackFrom = trackTo;
                trackStep = 0;
            } else {
                trackStepFaster = trackSpeedRatio * ( trackFrom - trackTo );
                if(trackStepFaster > trackStep)
                    trackStep = trackStepFaster;
            }
        }
    }

    List<int> barColorIDs = new List<int>();
    string[] barColors = {"hong","huang","lv","lan","zi"};
    void InitBarColorArray ()
    {
        if(mBarCount > 2)
        {
            for(int i = mBarCount; i > 2; i--)
            {
                int colorIndex = 4 - ( mBarCount - i ) % 3;
                barColorIDs.Add(colorIndex);
            }
        }
        if(mBarCount > 1)
        {
            barColorIDs.Add(1);
        }
        barColorIDs.Add(0);
    }


    /// <summary>
    /// BOSS血量（接口）
    /// </summary>
    /// <param name="max">boss血量上限</param>
    /// <param name="now">boss当前血量</param>
    public void ShowBlood(CharacterState cs)
    {
        if(!inited || cs != currentBoss)
        {
            currentBoss = cs;
            // mBarCount = cs.CharData.attrNode.hpBar_count;
            mBarCount = GetHpBarCount(cs.maxHp);
            trackFrom = mBarCount;
            mMaxHp = cs.maxHp;
            SceneNode sceneNode = null;
            if (GameLibrary.dungeonId != 0)
                sceneNode = FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(GameLibrary.dungeonId);
            namelvl.text = null != sceneNode ? sceneNode.animationName : cs.CharData.attrNode.name;
            // + " 等级[ffc937]" + (((MonsterData)cs.CharData).lvlRate != 0 ? Mathf.Ceil(((MonsterData)cs.CharData).lvlRate) : cs.CharData.lvl) + "[-]";
            InitBarColorArray();
            inited = true;
            goRoot.SetActive(true);
        }
        RefreshBlood(cs.currentHp);

        if (cs.currentHp <= 0)
            Invoke("BossDead", 1f);
    }


    int GetHpBarCount (int mHp)
    {
        if(mHp > 80000)
            return 15;
        if(mHp > 50000)
            return 14;
        if(mHp > 35000)
            return 13;
        if(mHp > 25000)
            return 12;
        if(mHp > 18000)
            return 11;
        if(mHp > 12000)
            return 10;
        if(mHp > 8000)
            return 9;
        if(mHp > 5000)
            return 8;
        if(mHp > 3000)
			return 7;
        return 6;
    }

    void BossDead()
    {
        goRoot.SetActive(false);
    }

    void OnDestroy()
    {
        LabCount = null;
        goRoot = null;
    }
}