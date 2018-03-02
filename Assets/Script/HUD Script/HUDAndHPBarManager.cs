using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//HUD显示类型 暴击，治疗，地方受击，玩家受击
public enum HUDType
{
    Crit,               //暴击
    Cure,               //治疗
    Miss,               //miss
    DamageEnemy,        //敌军伤害
    DamagePlayer,       //英雄伤害
    Bleeding,           //流血
    Immune,             //免疫
    SuckBlood,          //吸血
    Absorb              //吸收
}

public class HUDAndHPBarManager : MonoBehaviour
{
    public static HUDAndHPBarManager instance;

    GameObject HUDTextPrefab, mHudPanel;
    Dictionary<GameObject, HUDText> hudDic = new Dictionary<GameObject, HUDText>();
    //List<GameObject> mHudAndHp = new List<GameObject>();
    //List<Transform> mHpTransform = new List<Transform>();
    Transform mTowerPanel, mPlayerPanel, mMonsterPanel;

    void Awake()
    {
        instance = this;
        HUDTextPrefab = Resources.Load("HUD/HUD Text") as GameObject;
        mHudPanel = NGUITools.AddChild(gameObject);
        mHudPanel.name = "HudPanel";
        mHudPanel.AddComponent<UIPanel>().depth = 2;
    }
    
    public void HUD(GameObject target, int count, HUDType hType = HUDType.DamageEnemy, float time = 0f)
    {
        string str = ( count != 0 ) ? "" + Mathf.Abs(count) : "";
        if (hudDic.ContainsKey(target))
        {
            hudDic[target].Add(str, time, hType);
        }
        else
        {
            HUDText hudTxt = NGUITools.AddChild(mHudPanel, HUDTextPrefab).GetComponent<HUDText>();
            GameObject go = Instantiate(Resources.Load("HUD/HUD Point"), Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.parent = target.transform;
            Transform mHitPoint = target.GetComponent<CharacterState>().mHitPoint;
            go.transform.localPosition = mHitPoint == null ? Vector3.up * 0.5f : mHitPoint.localPosition;
            hudTxt.GetComponent<UIFollowTarget>().target = go.transform;
            hudTxt.Add(str, time, hType);
            hudDic.Add(target, hudTxt);
        }
    }

    public void AddHUDAndHP(GameObject target, Modestatus state)
    {
        switch (state)
        {
            case Modestatus.Tower:
                if (mTowerPanel == null)
                {
                    mTowerPanel = NGUITools.AddChild(gameObject).transform;
                    mTowerPanel.gameObject.AddComponent<UIPanel>();
                    mTowerPanel.name = "Tower";
                    mTowerPanel.GetComponent<UIPanel>().depth = 3;
                    target.transform.parent = mTowerPanel;
                }
                else
                {
                    target.transform.parent = mTowerPanel;
                }
                break;
            case Modestatus.NpcPlayer:
            case Modestatus.Player:
                if (mPlayerPanel == null)
                {
                    mPlayerPanel = NGUITools.AddChild(gameObject).transform;
                    mPlayerPanel.gameObject.AddComponent<UIPanel>();
                    mPlayerPanel.name = "Player";
                    mPlayerPanel.GetComponent<UIPanel>().depth = 4;
                    target.transform.parent = mPlayerPanel;
                }
                else
                {
                    target.transform.parent = mPlayerPanel;
                }
                break;
            case Modestatus.Monster:
                if (mMonsterPanel == null)
                {
                    mMonsterPanel = NGUITools.AddChild(gameObject).transform;
                    mMonsterPanel.gameObject.AddComponent<UIPanel>();
                    mMonsterPanel.name = "monster";
                    mMonsterPanel.GetComponent<UIPanel>().depth = 5;
                    target.transform.parent = mMonsterPanel.transform;
                }
                else
                {
                    target.transform.parent = mMonsterPanel;
                }
                break;
            default:
                break;
        }
    }

    public void AddPrompt(Transform target)
    {
        if (null == target) return;
        GameObject go = Instantiate(Resources.Load("HUD/ESPrompt")) as GameObject;
        go.transform.parent = mPlayerPanel;
        go.GetComponent<UIFollowTarget>().target = target;
        go.transform.localScale = Vector3.one;
        TweenScale ts = go.GetComponent<TweenScale>();
        CDTimer.GetInstance().AddCD(5, (int count, long cid) => { ts.enabled = true;
            CDTimer.GetInstance().AddCD(2, (int counts, long cids) => { ts.enabled = false; });
        }, 30);
    }

    public void DestroyHUD(GameObject target, float time = 0)
    {
        HUDText t = null;
        if (hudDic.ContainsKey(target))
        {
            t = hudDic[target];
            StartCoroutine(DestroyH(t, time));
            hudDic.Remove(target);
        }
    }

    IEnumerator DestroyH(HUDText target, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(target.gameObject);
    }

    public void DestroyAll()
    {
        foreach (GameObject item in hudDic.Keys)
        {
            Destroy(hudDic[item].gameObject);
        }

        hudDic.Clear();
    }
}