using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TaskEffectManager : MonoBehaviour
{
    public static TaskEffectManager instance;
    void Awake()
    {
        instance = this;


        //接收任务特效
        string renwjstxpath = "Effect/Prefabs/UI/renwjstx";
        GameObject renwjstx = Instantiate(Resources.Load(renwjstxpath)) as GameObject;
        renwjstx.transform.parent = transform;
        renwjstx.transform.position = Vector3.one;
        renwjstx.transform.localScale = Vector3.one;
        renwjstx.name = "renwjstx";
        renwjstx.SetActive(false);
        CreateEffectObj("renwjstx",renwjstx);

        //完成任务特效
        string renwwctxpath = "Effect/Prefabs/UI/renwwctx";
        GameObject renwwctx = Instantiate(Resources.Load(renwwctxpath)) as GameObject;
        renwwctx.transform.parent = transform;
        renwwctx.transform.position = Vector3.one;
        renwwctx.transform.localScale = Vector3.one;
        renwwctx.name = "renwwctx";
        renwwctx.SetActive(false);
        CreateEffectObj("renwwctx", renwwctx);

        //到达指定区域特效
        string wanctexpath = "Effect/Prefabs/UI/wanctex";
        GameObject wanctex = Instantiate(Resources.Load(wanctexpath)) as GameObject;
        wanctex.transform.parent = transform;
        wanctex.transform.position = Vector3.one;
        wanctex.transform.localScale = Vector3.one;
        wanctex.name = "wanctex";
        wanctex.SetActive(false);
        CreateEffectObj("wanctex", wanctex);
    }
    public Dictionary<string, GameObject> TaskEffcctObjDic = new Dictionary<string, GameObject>();
	
    public void CreateEffectObj(string name, GameObject go)
    {
        if (!TaskEffcctObjDic.ContainsKey(name) || TaskEffcctObjDic[name] == null)
        {
            if (TaskEffcctObjDic.ContainsKey(name))
            {
                TaskEffcctObjDic[name] = go;
            }
            else
            {
                TaskEffcctObjDic.Add(name, go);
            }
        }
    }

    public GameObject GetEffectObj(string name)
    {
        if (TaskEffcctObjDic.ContainsKey(name))
        {
            return TaskEffcctObjDic[name];
        }
        Debug.Log("this Npc Model Not Create npcid:" + name + " Please Check Secence Setting");
        return null;
    }

    public void ShowTaskEffect(TaskEffectType type)
    {
        GameObject obj = null;
        if (type == TaskEffectType.AcceptTask)
        {
            obj = GetEffectObj("renwjstx");
            if (obj != null)
            {
                PlayTaskEffect(obj);
            }
        }
        else if (type == TaskEffectType.SucceedTask)//任务完成会伴随任务奖励界面，目前方案是先出现任务完成特效 0.5s后显示再显示奖励面板
        {
            obj = GetEffectObj("renwwctx");
            UIDialogue.instance.ShowTaskHidePanel();
            if (obj != null)
            {
                PlayTaskEffect(obj);
                StartCoroutine(PlayTaskReward());
            }
        }
        else if (type == TaskEffectType.ArriveTarget)
        {
            obj = GetEffectObj("wanctex");
            if (obj != null)
            {
                PlayTaskEffect(obj);
            }
        }
    }

    IEnumerator PlayTaskReward()
    {
        yield return new WaitForSeconds(0.3f);

        Control.ShowGUI(UIPanleID.UITaskRewardPanel, EnumOpenUIType.DefaultUIOrSecond);
        yield return 0;
    }

    private void PlayTaskEffect(GameObject obj)
    {
        Vector3 tempV = CharacterManager.player.transform.position;
        if (CharacterManager.playerCS.pm.isRiding)
        {
            tempV.y += 0.7f;
        }
        else
        {
            tempV.y += 1.1f;
        }
       
        obj.transform.position = tempV;
        obj.SetActive(false);
        obj.SetActive(true);
        TaskManager.Single().isAcceptTask = false;
    }
}
