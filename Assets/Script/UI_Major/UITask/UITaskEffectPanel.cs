using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITaskEffectPanel : GUIBase
{
    public static UITaskEffectPanel instance;
    private Transform zdxlTrs;//自动寻路特效
    private GameObject renwjstx;//接收任务特效
    private GameObject renwwctx;//完成任务特效
    private GameObject wanctex;//到达指定区域特效
    bool isXunlu;
    TaskEffectType type;
    public UITaskEffectPanel()
    {
        instance = this;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UITaskEffectPanel;
    }
    //protected override void SetUI(params object[] uiParams)
    //{
    //    type = (TaskEffectType)uiParams[0];
    //    isXunlu = (bool)uiParams[1];
    //    base.SetUI();
    //}
    protected override void OnLoadData()
    {
        base.OnLoadData();
        this.State = EnumObjectState.Ready;
        Show();
    }
    public override void ReceiveData(uint messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            //case MessageID.common_offer_reward_mission_list_ret:
            //  Show(); break;
        }
    }
    protected override void Init()
    {
        instance = this;
        zdxlTrs = transform.Find("zidongxunlu");
        renwjstx = transform.Find("renwjstx").gameObject;
        renwwctx = transform.Find("renwwctx").gameObject;
        wanctex = transform.Find("wanctex").gameObject;
        renwjstx.SetActive(false);
        CreateEffectObj("renwjstx", renwjstx);
        renwwctx.SetActive(false);
        CreateEffectObj("renwwctx", renwwctx);
        wanctex.SetActive(false);
        CreateEffectObj("wanctex", wanctex);
    }
    public void SetZDXLEffect(bool isXunlu)
    {
        zdxlTrs.gameObject.SetActive(isXunlu);
        ResetAutoFightEffect(zdxlTrs);
    }
    void ResetAutoFightEffect(Transform trans)
    {
        TweenPosition[] tps = trans.GetComponentsInChildren<TweenPosition>();
        for (int i = 0; i < tps.Length; i++)
        {
            tps[i].ResetToBeginning();
        }
    }
    IEnumerator OpenEffect()
    {
        yield return new WaitForSeconds(1f);
        zdxlTrs.gameObject.SetActive(true);
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
        Debug.Log("未找到:" + name + " 特效");
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
            //UIDialogue.instance.ShowTaskHidePanel();
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
        //Vector3 tempV = CharacterManager.player.transform.position;
        //if (CharacterManager.playerCS.pm.isRiding)
        //{
        //    tempV.y += 0.7f;
        //}
        //else
        //{
        //    tempV.y += 1.1f;
        //}
        //obj.transform.position = tempV;
        obj.SetActive(false);
        obj.SetActive(true);
        TaskManager.Single().isAcceptTask = false;
    }

}
public enum TaskEffectType
{
    AcceptTask,
    SucceedTask,
    ArriveTarget,
    AutoXunlu,
}
