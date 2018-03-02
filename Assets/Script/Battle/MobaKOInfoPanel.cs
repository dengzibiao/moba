using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobaKOInfoPanel : MonoBehaviour 
{
    public UISprite SpKillerBg;
    public UISprite SpKillerHead;
    public UISprite SpKillerBorder;
    public UISprite SpDeadBorder;
    public UISprite SpDeadHead;
    public UISprite SpKillEffect;
    public UISprite[] SpAidHeads;
    public UILabel LaKill;
    public UISprite SpKillText;
    public UIWidget WdAid;
    public GameObject FirstBloodEffect;
    public GameObject KillEffect;

    public List<MobaKOInfo> KoInfosToPlay = new List<MobaKOInfo>();
    bool playing = false;

    void ShowOneInfo ( MobaKOInfo info)
    {
        LaKill.text = info.killDesc;
        SpKillText.spriteName = info.killSpName;
        SpKillText.MakePixelPerfect();

        if(info.type == MobaKOInfoType.firstKill)
        {
            SpKillerBg.spriteName = "yixuedi";
        } else if(info.type == MobaKOInfoType.multiKill || (info.type == MobaKOInfoType.serialKill && info.SerialKillCount>2) || info.type == MobaKOInfoType.endKill)
        {
            SpKillerBg.spriteName = info.killer.groupIndex == 0 ? "shuangshahongdi" : "shuangshalandi";
        } else if(info.type == MobaKOInfoType.allKill)
        {
            SpKillerBg.spriteName = info.killer.groupIndex == 1 ? "shuangshahongdi" : "shuangshalandi";
        }
        else
        {
            SpKillerBg.spriteName = info.killer.groupIndex == 0 ? "hongdi" : "landi";
        }

        SpKillerBorder.spriteName = info.killer.groupIndex == 0 ? "hongkuang" : "lankuang";
        SpDeadBorder.spriteName = info.decedent.groupIndex == 0 ? "hongkuang" : "lankuang";
        SpKillerBg.MakePixelPerfect();
        string containGroupIndex = BattleUtil.IsHeroTarget(info.killer) ? "" : "" + info.killer.groupIndex;
        SpKillerHead.spriteName = info.killer.attrNode.icon_name + containGroupIndex + "_head";
        SpDeadHead.spriteName = info.decedent.attrNode.icon_name + "_head";
        for(int i = 0; i< SpAidHeads.Length; i++)
        {
            SpAidHeads[i].gameObject.SetActive(false);
        }
        for(int j = 0; j < info.aids.Count; j++)
        {
            SpAidHeads[j].spriteName = info.aids[j].attrNode.icon_name + "_head";
            SpAidHeads[j].gameObject.SetActive(true);
        }

        bool showDead = info.type != MobaKOInfoType.allKill && info.type != MobaKOInfoType.towerKill && info.type != MobaKOInfoType.chaoshenKill;
        SpKillerHead.gameObject.SetActive(info.type != MobaKOInfoType.allKill);
        SpDeadHead.gameObject.SetActive(showDead);
        SpKillEffect.gameObject.SetActive(showDead);
        WdAid.gameObject.SetActive(info.ShowAid);

        SpKillEffect.fillAmount = 0f;

        TweenAlpha ta = TweenAlpha.Begin(gameObject, 0.7f, 1f);
        EventDelegate.Add(ta.onFinished, ()=>FadeOut(info), true);

        SpDeadHead.transform.localPosition = new Vector3(240f, 0f, 0f);
        SpKillerHead.transform.localPosition = new Vector3(-240f, 0f, 0f);
        TweenPosition.Begin(SpDeadHead.gameObject, 0f, new Vector3(240f, 0f, 0f));
        TweenPosition.Begin(SpKillerHead.gameObject, 0f, new Vector3(-240f, 0f, 0f));
        float pos = info.type == MobaKOInfoType.towerKill ? 180f : 140f;
        TweenPosition.Begin(SpDeadHead.gameObject, 0.4f, new Vector3(pos, 0f, 0f)).delay = 0.1f;
        TweenPosition.Begin(SpKillerHead.gameObject, 0.4f, new Vector3(-pos, 0f, 0f)).delay = 0.1f;
    }

    void ShowKillEffect (GameObject effectGo)
    {
        effectGo.SetActive(true);
        BattleUtil.PlayParticleSystems(effectGo);
    }

    void FadeOut ( MobaKOInfo info)
    {
        if(info.type == MobaKOInfoType.firstKill)
            ShowKillEffect(FirstBloodEffect);
        else if(info.type == MobaKOInfoType.serialKill || info.type == MobaKOInfoType.multiKill)
            ShowKillEffect(KillEffect);
        InvokeRepeating("SplashKillEffect", 0f, 0.02f);
        TweenAlpha ta = TweenAlpha.Begin(gameObject, 0.8f, 0f);
        ta.delay = 1.5f;
        // ta.SetOnFinished(() => playing = false);
        EventDelegate.Add(ta.onFinished, () => playing = false, true);
    }

    float splashScale = 1f;
    void SplashKillEffect ()
    {
        if(SpKillEffect.fillAmount < 1f)
        {
            SpKillEffect.fillAmount += 0.125f;

        }
        else
        {
            splashScale *= 1.2f;
            SpKillEffect.transform.localScale = splashScale * Vector3.one;
            if(splashScale > 3f)
            {
                splashScale = 1f;
                SpKillEffect.transform.localScale = Vector3.one;
                CancelInvoke("SplashKillEffect");
            }
        }
    }

    void Update()
	{
        if(KoInfosToPlay.Count > 0)
        {
            if(!playing)
            {
                playing = true;
                ShowOneInfo(KoInfosToPlay[0]);
                KoInfosToPlay.RemoveAt(0);
            }
        }
	}
}

public class MobaKOInfo
{
    public MobaKOInfoType type;
    public CharacterData decedent;
    public CharacterData killer;
    public List<CharacterData> aids = new List<CharacterData>();
    public string killDesc;
    public string killSpName;
    public int SerialKillCount;
    public bool ShowAid = true;
}

public enum MobaKOInfoType
{
    serialKill,
    multiKill,
    firstKill,
    endKill,
    allKill,
    towerKill,
    chaoshenKill
}