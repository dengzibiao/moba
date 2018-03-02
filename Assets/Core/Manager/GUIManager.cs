using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
/// <summary>
/// 路径定义。
/// </summary>
public static class UIResourcesPath
{
    /// <summary>
    /// UI预设。
    /// </summary>
    public const string UI_PREFAB = "Prefab/UIPanel/";
}
public class GUIManager
{
    /// <summary>
    /// 打开的互斥UI集合。
    /// </summary>
    private List<UIPanleID> mutexUIList = null;
    /// <summary>
    /// 所有打开的ui集合。
    /// </summary>
    private Dictionary<UIPanleID, GameObject> allOpenUIDic = null;
    /// <summary>
    /// 要打开的UI集合栈。先进后出这个需要注意，如果打开界面是一个列表，要注意想要显示的顺序
    /// </summary>
    private Stack<UIInfoData> stackOpenUI = null;
    private Dictionary<string, GUIBase> dict = null;
    public Dictionary<int, GameObject> dic = null;
    private GameObject ui = null;
    private string uiName = null;
    private GameObject go = null;
    private Transform _root = null;
    /// <summary>
    /// 全屏显示的ID集合
    /// </summary>
    private UIPanleID[] mFullScreenPanelIds = new UIPanleID[]
    {
        UIPanleID.UIActivities, UIPanleID.UIWelfare, UIPanleID.UIFriends, UIPanleID.UIMailPanel, UIPanleID.UILevel,
        UIPanleID.UIActivityPanel, UIPanleID.UIPvP, UIPanleID.UIKnapsack, UIPanleID.UIShopPanel,
        UIPanleID.UIRankList, UIPanleID.UIHeroList, UIPanleID.UILottery, UIPanleID.UINotJoinSocietyPanel,
        UIPanleID.UIMountAndPet, UIPanleID.EquipDevelop, UIPanleID.UIHeroDetail, UIPanleID.UIAbattiorList,
        UIPanleID.UIEmbattle, UIPanleID.UIArenaModePanel, UIPanleID.UIArenaModePanel
    };
    public GUIManager()
    {
        mutexUIList = new List<UIPanleID>();//存储打开界面的集合
        stackOpenUI = new Stack<UIInfoData>();//要打开的界面集合
        allOpenUIDic = new Dictionary<UIPanleID, GameObject>();//所有UI界面的集合
        dict = new Dictionary<string, GUIBase>();//存储所有UI逻辑对象
        dic = new Dictionary<int, GameObject>();//界面加控件名字为键，控件的集合,目前主要用于新手引导
    }

    public List<UIPanleID> GetFullScreenUI()
    {
        return mutexUIList;
    }

    public void AddOrDeletFullScreenUI(UIPanleID uiKey, bool isAdd)
    {
        if (isAdd)
        {
            for (int i = 0; i < mutexUIList.Count; i++)
            {
                if (mutexUIList[i] == uiKey)
                {
                    mutexUIList.RemoveAt(i);
                }
            }
            mutexUIList.Add(uiKey);
        }
        else
        {
            for (int i = 0; i < mutexUIList.Count; i++)
            {
                if (mutexUIList[i] == uiKey)
                {
                    mutexUIList.RemoveAt(i);
                }
            }
        }
    }
    /// <summary>
    /// 注册控件
    /// </summary>
    /// <param name="id"></param>
    /// <param name="go"></param>
    public void RegisterComponent(int id, GameObject go)
    {
        if (!dic.ContainsKey(id))
        {
            //  Debug.Log("不包含  直接添加进来!!");
            // if(go!=null)
            // Debug.Log(go.transform.name);
            dic.Add(id, go);
        }
        else
        {
            // if (dic[id] == null)
            {
                //  Debug.Log("已经包含  直接赋值!!");
                // if (go != null) ;
                //  Debug.Log(go.transform.name);
                //  dic.Remove(id);
                dic[id] = go;
            }

        }
    }
    public bool isContainsKey(int id)
    {
        return dic.ContainsKey(id);

    }
    public void RemoveComponentID(int id)
    {
        if (dic.ContainsKey(id))
        {
            dic.Remove(id);
        }
    }

    /// 判断是否有全屏UI显示
    /// </summary>
    /// <returns></returns>
    public bool HasFullScreenUIActive()
    {
        bool result = false;
        if (ScenesManage.Instance != null)
        {
            GUIBase mBase;
            for (int i = 0; i < mFullScreenPanelIds.Length; i++)
            {
                mBase = GetUI<GUIBase>(mFullScreenPanelIds[i]);
                if (mBase != null)
                {

                    if (mBase.IsShow())
                    {
                        result = true;
                        break;
                    }

                }
            }
        }
        return result;
    }

    //获取控件
    /// </summary>
    /// <param name="mid"></param>
    /// <returns></returns>
    public GameObject GetComponent(int mid)
    {
        go = null;
        if (!dic.TryGetValue(mid, out go))
        {
            return null;
        }
        return go;
    }
    public void RegisterGUI(string key, GUIBase ui)
    {
        //Debug.Log(ui + ":" + key);
        if (_root == null) _root = ui.transform.parent;

        if (!dict.ContainsKey(key))
        {
            dict.Add(key, ui);
        }
    }
    /// <summary>
    /// ui，是否排斥其他界面isRepel=true，关闭所有打开界面，界面isRepel=false关闭界面
    /// </summary>
    public GUIBase Show(UIPanleID uiKey)
    {
        ui = null;
        //Debug.Log(dict.TryGetValue(key, out ui));
        if (allOpenUIDic.ContainsKey(uiKey ))
        {
            if (GetUI<GUIBase>(uiKey) != null)
            {
                GetUI<GUIBase>(uiKey).Show();

            }
        }
        return GetUI<GUIBase>(uiKey);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="isRepel"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public GUIBase Hide(UIPanleID uiKey)
    {
        ui = null;
        if (allOpenUIDic.TryGetValue(uiKey, out ui))
        {
            if (GetUI<GUIBase>(uiKey) != null)
            {
                GetUI<GUIBase>(uiKey).Hide();
            }
        }
        return GetUI<GUIBase>(uiKey);
    }
    public void Clear()
    {
        allOpenUIDic.Clear();
        dict.Clear();
    }
    #region 置顶界面方法
    /// <summary>
    /// 置顶界面
    /// </summary>
    /// <param name="gameObject"></param>
    public void Set2Top(GameObject gameObject)
    {
        UIPanel go = gameObject.GetComponent<UIPanel>();
        UIPanel[] myPanels = gameObject.GetComponentsInChildren<UIPanel>();

        List<UIPanel> myPanelList = new List<UIPanel>(myPanels);

        UIPanel[] panels = UnityEngine.Object.FindObjectsOfType<UIPanel>();

        int max = 0;

        for (int i = 0; i < panels.Length; i++)
        {
            if (!myPanelList.Contains(panels[i]))
            {
                if (panels[i].depth > max)
                {
                    max = panels[i].depth;
                }
            }
        }

        int min = 0;

        for (int i = 0; i < myPanels.Length; i++)
        {
            if (min > myPanels[i].depth)
            {
                min = myPanels[i].depth;
            }
        }
        go.depth = (max - min + 2);
        for (int i = 0; i < myPanels.Length; i++)
        {
            if (myPanels[i] != go)
                myPanels[i].depth = (go.depth + 2);
        }

    }
    #endregion

    #region 关闭界面的方法
    /// <summary>
    /// 关闭当前界面显示上一个打开的来源界面
    /// </summary>
    public void CloseMutexUI(bool isCloseAllUI = false)
    {
        if (isCloseAllUI)
        {
            CloseAllUI();
            return;
        }
        if (mutexUIList.Count == 0)
        {
            return;
        }
        CloseUI(mutexUIList[mutexUIList.Count - 1]);
        mutexUIList.RemoveAt(mutexUIList.Count - 1);
        if (mutexUIList.Count > 0)
        {
            OpenUI(mutexUIList[mutexUIList.Count - 1], false);
        }
    }
    public void CloseAllUI()
    {
        List<UIPanleID> keyList = new List<UIPanleID>(allOpenUIDic.Keys);
        foreach (UIPanleID uiKey in keyList)
        {
            GameObject obj = allOpenUIDic[uiKey];
            GUIBase baseUI = obj.GetComponent<GUIBase>();
            if (baseUI != null)
            {
                baseUI.StateChanged += CloseHandler;
                baseUI.Release();
            }
            else
            {
                GameObject.Destroy(obj);
                allOpenUIDic.Remove(uiKey);
            }
        }
        allOpenUIDic.Clear();
        mutexUIList.Clear();
    }
    /// <summary>
    /// 关闭事件。
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="newState">New state.</param>
    /// <param name="oldState">Old state.</param>
    private void CloseHandler(object sender, EnumObjectState newState, EnumObjectState oldState)
    {
        if (newState == EnumObjectState.Closing)
        {
            GUIBase ui = sender as GUIBase;
            allOpenUIDic.Remove(ui.GetUIKey());
            ui.StateChanged -= CloseHandler;
        }
    }
    /// <summary>
    /// 关闭界面。
    /// </summary>
    /// <param name="uiKey"></param>
    public void CloseUI(UIPanleID uiKey)
    {
        GameObject uiObject = null;
        if (!allOpenUIDic.TryGetValue(uiKey, out uiObject))
        {
            return;
        }

        if (uiObject == null)
        {
            if (allOpenUIDic.ContainsKey(uiKey))
                allOpenUIDic.Remove(uiKey);
        }
        else
        {
            GUIBase baseUI = uiObject.GetComponent<GUIBase>();
            if (baseUI != null)
            {
                baseUI.StateChanged += CloseHandler;
                baseUI.Release();
            }
            else
            {
                GameObject.Destroy(uiObject);
                allOpenUIDic.Remove(uiKey);
            }
        }
    }
    #endregion
    #region 获取界面对象。
    /// <summary>
    /// 获取界面对象。
    /// </summary>
    /// <param name="uiKey"></param>
    /// <returns>GameObject</returns>
    public GameObject GetUIObject(UIPanleID uiKey)
    {
        GameObject uiObject = null;
        if (allOpenUIDic.TryGetValue(uiKey, out uiObject))
        {
            return uiObject;
        }
        return null;
    }
    #endregion
    /// <summary>
    /// 获取界面对象上的Script
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="uiKey"></param>
    /// <returns></returns>
    public T GetUI<T>(UIPanleID uiKey) where T : GUIBase
    {
        GameObject uiObject = this.GetUIObject(uiKey);
        if (uiObject != null)
        {
            return uiObject.GetComponent<T>();
        }

        return null;
    }
    #region 打开UI界面的方法

    /// <summary>
    /// 打开界面集合。
    /// </summary>
    /// <param name="isCloseOthers">是否关闭其他所有界面，返回时不会打开来源界面</param>
    /// <param name="uiKeys"></param>
    public void OpenMutexUI(bool isCloseOthers, UIPanleID[] uiKeys, params object[] uiObjParams)
    {
        if (isCloseOthers) CloseMutexUI();
        if (mutexUIList.Count > 0)
        {
            CloseUI(mutexUIList[mutexUIList.Count - 1]);
        }
        for (int i = 0; i < mutexUIList.Count; i++)
        {
            for (int j = 0; j < uiKeys.Length; j++)
            {
                if (mutexUIList[i] == uiKeys[j])
                {
                    mutexUIList.RemoveAt(i);
                }
            }

        }
        for (int i = 0; i < uiKeys.Length; i++)
        {
            OpenMutexUI(uiKeys[i], uiObjParams);
        }
    }
    /// <summary>
    /// 打开单一界面
    /// </summary>
    /// <param name="isCloseOthers">是否关闭其他所有界面，返回时不会打开来源界面</param>
    /// <param name="uiKey"></param>
    /// <param name="uiObjParams"></param>
    public void OpenMutexUI(bool isCloseOthers, UIPanleID uiKey, params object[] uiObjParams)
    {
        if (isCloseOthers) CloseMutexUI();
        if (mutexUIList.Count > 0)
        {
            CloseUI(mutexUIList[mutexUIList.Count - 1]);
        }
        for (int i = 0; i < mutexUIList.Count; i++)
        {
            if (mutexUIList[i] == uiKey)
            {
                mutexUIList.RemoveAt(i);
            }
        }
        OpenMutexUI(uiKey, uiObjParams);
    }

    /// <summary>
    /// 打开界面。默认的界面或者二级界面用这个方法打开
    /// </summary>
    /// <param name="uiKey"></param>
    public void OpenUI(UIPanleID[] uiKey, bool isCloseAllUI)
    {
        this.OpenUI(isCloseAllUI, uiKey, null);
    }
    /// <summary>
    ///  打开界面。默认的界面或者二级界面用这个方法打开
    /// </summary>
    /// <param name="uiKey"></param>
    /// <param name="uiObjParams"></param>
    public void OpenUI(UIPanleID uiKey, bool isCloseAllUI, params object[] uiObjParams)
    {
        UIPanleID[] uiKeys = new UIPanleID[1];
        uiKeys[0] = uiKey;
        this.OpenUI(isCloseAllUI, uiKeys, uiObjParams);
    }
    #endregion
    #region 核心方法
    private void OpenMutexUI(UIPanleID uiKey, object[] uiObjParams)
    {
        if (!mutexUIList.Contains(uiKey))
        {
            UIPanleID[] uiKeys = new UIPanleID[1];
            uiKeys[0] = uiKey;
            OpenUI(false, uiKeys, uiObjParams);
            mutexUIList.Add(uiKey);
        }
    }
    /// <summary>
    /// 打开界面的核心方法
    /// </summary>
    /// <param name="isCloseAll"></param>
    /// <param name="uiKey"></param>
    /// <param name="uiObjParams"></param>
    private void OpenUI(bool isCloseAll, UIPanleID[] uiKey, params object[] uiObjParams)
    {
        ///关闭所有UI。
        if (isCloseAll)
        {
            this.CloseAllUI();
        }
        //把要打开的UI压入栈中。
        for (int i = 0; i < uiKey.Length; i++)
        {
            UIPanleID uiId = uiKey[i];
            if (!allOpenUIDic.ContainsKey(uiId))
            {
                string path = this.GetPrefabPathFromKey(uiId);
                stackOpenUI.Push(new UIInfoData(uiId, path, uiObjParams));
            }
        }
        //协同打开UI
        if (Game.Instance != null)
            Game.Instance.StartCoroutine(AyncCallbackOpenUI());
    }
    /// <summary>
    /// 协同打开UI。
    /// </summary>
    /// <returns>The callback open UI.</returns>
    private IEnumerator<int> AyncCallbackOpenUI()
    {
        UIInfoData uiInfoData = null;
        UnityEngine.Object prefab = null;
        GameObject uiObject = null;
        if (stackOpenUI != null && stackOpenUI.Count > 0)
        {
            do
            {
                uiInfoData = stackOpenUI.Pop();
                prefab = Singleton<ResourceManager>.Instance.LoadPrefab(uiInfoData.Path);
                if (prefab != null)
                {
                    uiObject = NGUITools.AddChild(Game.Instance.UiRoot, prefab as GameObject);
                    GUIBase baseUi = uiObject.GetComponent<GUIBase>();
                    if (baseUi != null)
                    {
                        baseUi.SetUIWhenOpening(uiInfoData.UIObjParams);
                    }
                    allOpenUIDic.Add(uiInfoData.UIKey, uiObject);
                }
            } while (stackOpenUI.Count > 0);
        }
        yield return 0;
    }
    #endregion
    /// <summary>
    /// 根据界面ID获取预设。
    /// </summary>
    /// <param name="uiType">User interface type.</param>
    private string GetPrefabPathFromKey(UIPanleID uiKey)
    {
        string path = string.Empty;
        switch (uiKey)
        {
            case UIPanleID.UILogin:
                path = UIResourcesPath.UI_PREFAB + "UILogin";
                break;
            case UIPanleID.UIRegister:
                path = UIResourcesPath.UI_PREFAB + "UIRegister";
                break;
            case UIPanleID.UI_SelectServer:
                path = UIResourcesPath.UI_PREFAB + "UISelectServer";
                break;
            case UIPanleID.UI_ServerList:
                path = UIResourcesPath.UI_PREFAB + "UIServerList";
                break;
            case UIPanleID.UIGameAffiche:
                path = UIResourcesPath.UI_PREFAB + "UIGameAffiche";
                break;
            case UIPanleID.UI_CreateRole:
                path = UIResourcesPath.UI_PREFAB + "UICreateRole";
                break;
            case UIPanleID.UILoading:
                path = UIResourcesPath.UI_PREFAB + "UILoading";
                break;
            case UIPanleID.UIPromptBox:
                path = UIResourcesPath.UI_PREFAB + "UIPromptBox";
                break;
            case UIPanleID.UIRole:
                path = UIResourcesPath.UI_PREFAB + "UIRole";
                break;
            case UIPanleID.UIPause:
                path = UIResourcesPath.UI_PREFAB + "UIPause";
                break;
            case UIPanleID.ChangeIcon:
                path = UIResourcesPath.UI_PREFAB + "ChangeIcon";
                break;
            case UIPanleID.UIRoleInfo:
                path = UIResourcesPath.UI_PREFAB + "UIRoleInfo";
                break;
            case UIPanleID.ChangeName:
                path = UIResourcesPath.UI_PREFAB + "ChangeName";
                break;
            case UIPanleID.Upgrade:
                path = UIResourcesPath.UI_PREFAB + "Upgrade";
                break;
            case UIPanleID.UIMoney:
                path = UIResourcesPath.UI_PREFAB + "UIMoney";
                break;
            case UIPanleID.UIChat:
                path = UIResourcesPath.UI_PREFAB + "UIChat";
                break;
            case UIPanleID.UISetting:
                path = UIResourcesPath.UI_PREFAB + "UISetting";
                break;
            //case UIPanleID.UIPopRefresh:
            //    path = UIResourcesPath.UI_PREFAB + "UIPopRefresh";
            //    break;
            case UIPanleID.UIPopBuy:
                path = UIResourcesPath.UI_PREFAB + "UIPopBuy";
                break;
            case UIPanleID.UIMask:
                path = UIResourcesPath.UI_PREFAB + "UIMask";
                break;
            case UIPanleID.UIShopPanel:
                path = UIResourcesPath.UI_PREFAB + "UIShopPanel";
                break;
            case UIPanleID.UILottery:
                path = UIResourcesPath.UI_PREFAB + "UILottery";
                break;
            case UIPanleID.UIResultLottery:
                path = UIResourcesPath.UI_PREFAB + "UIResultLottery";
                break;
            case UIPanleID.UIPopLottery:
                path = UIResourcesPath.UI_PREFAB + "UIPopLottery";
                break;
            case UIPanleID.UIGetWayPanel:
                path = UIResourcesPath.UI_PREFAB + "UIGetWayPanel";
                break;
            case UIPanleID.UILevel:
                path = UIResourcesPath.UI_PREFAB + "UILevel";
                break;
            case UIPanleID.UIHeroList:
                path = UIResourcesPath.UI_PREFAB + "UIHeroList";
                break;
            case UIPanleID.UIHeroDetail:
                path = UIResourcesPath.UI_PREFAB + "UIHeroDetail";
                break;
            case UIPanleID.UIKnapsack:
                path = UIResourcesPath.UI_PREFAB + "UIKnapsack";
                break;
            case UIPanleID.UISkillAndGoldHintPanel:
                path = UIResourcesPath.UI_PREFAB + "UISkillAndGoldHintPanel";
                break;
            case UIPanleID.UIEmbattle:
                path = UIResourcesPath.UI_PREFAB + "UIEmbattle";
                break;
            case UIPanleID.UISalePanel:
                path = UIResourcesPath.UI_PREFAB + "UISalePanel";
                break;
            case UIPanleID.UIGoldHand:
                path = UIResourcesPath.UI_PREFAB + "UIGoldHand";
                break;
            case UIPanleID.UITaskList:
                path = UIResourcesPath.UI_PREFAB + "UITaskList";
                break;
            case UIPanleID.UICountdownPanel:
                path = UIResourcesPath.UI_PREFAB + "UICountdownPanel";
                break;
            case UIPanleID.UIHeroUseExp:
                path = UIResourcesPath.UI_PREFAB + "UIHeroUseExp";
                break;
            case UIPanleID.UIBuyEnergyVitality:
                path = UIResourcesPath.UI_PREFAB + "UIBuyEnergyVitality";
                break;
            case UIPanleID.UIShopSell:
                path = UIResourcesPath.UI_PREFAB + "UIShopSell";
                break;
            case UIPanleID.UIDialogue:
                path = UIResourcesPath.UI_PREFAB + "UIDialogue";
                break;
            case UIPanleID.UIRankList:
                path = UIResourcesPath.UI_PREFAB + "UIRankList";
                break;
            case UIPanleID.UIMailPanel:
                path = UIResourcesPath.UI_PREFAB + "UIMailPanel";
                break;
            case UIPanleID.UIChatPanel:
                path = UIResourcesPath.UI_PREFAB + "UIChatPanel";
                break;
            case UIPanleID.UIPlayerInteractionPort:
                path = UIResourcesPath.UI_PREFAB + "UIPlayerInteractionPort";
                break;
            case UIPanleID.UIWaitForSever:
                path = UIResourcesPath.UI_PREFAB + "UIWaitForSever";
                break;
            case UIPanleID.UITaskInfoPanel:
                path = UIResourcesPath.UI_PREFAB + "UITaskInfoPanel";
                break;
            case UIPanleID.UITaskRewardPanel:
                path = UIResourcesPath.UI_PREFAB + "UITaskRewardPanel";
                break;
            case UIPanleID.UIFriends:
                path = UIResourcesPath.UI_PREFAB + "UIFriends";
                break;
            case UIPanleID.UIPlayerTitlePanel:
                path = UIResourcesPath.UI_PREFAB + "UIPlayerTitlePanel";
                break;
            case UIPanleID.UIMarquee:
                path = UIResourcesPath.UI_PREFAB + "UIMarquee";
                break;
            case UIPanleID.UIWelfare:
                path = UIResourcesPath.UI_PREFAB + "UIWelfare";
                break;
            case UIPanleID.UISign_intBox:
                path = UIResourcesPath.UI_PREFAB + "UISign_intBox";
                break;
            case UIPanleID.UITooltips:
                path = UIResourcesPath.UI_PREFAB + "UITooltips";
                break;
            case UIPanleID.UITaskCollectPanel:
                path = UIResourcesPath.UI_PREFAB + "UITaskCollectPanel";
                break;
            case UIPanleID.UIActivities:
                path = UIResourcesPath.UI_PREFAB + "UIActivities";
                break;

            case UIPanleID.UIPopUp:
                path = UIResourcesPath.UI_PREFAB + "UIPopUp";
                break;

            case UIPanleID.UIGoodsGetWayPanel:
                path = UIResourcesPath.UI_PREFAB + "UIGoodsGetWayPanel";
                break;
            case UIPanleID.UITaskUseItemPanel:
                path = UIResourcesPath.UI_PREFAB + "UITaskUseItemPanel";
                break;
            case UIPanleID.UIFriendTip:
                path = UIResourcesPath.UI_PREFAB + "UIFriendTip";
                break;
            case UIPanleID.UIAbattiorList:
                path = UIResourcesPath.UI_PREFAB + "UIAbattiorList";
                break;
            case UIPanleID.SceneEntry:
                path = UIResourcesPath.UI_PREFAB + "SceneEntry";
                break;

            case UIPanleID.UITaskTracker:
                path = UIResourcesPath.UI_PREFAB + "UITaskTracker";
                break;
            case UIPanleID.UIExpBar:
                path = UIResourcesPath.UI_PREFAB + "UIExpBar";
                break;
            case UIPanleID.UIBoxGoodsTip:
                path = UIResourcesPath.UI_PREFAB + "UIBoxGoodsTip";
                break;
            case UIPanleID.UIArenaModePanel:
                path = UIResourcesPath.UI_PREFAB + "UIArenaModePanel";
                break;
            case UIPanleID.UIExptips:
                path = UIResourcesPath.UI_PREFAB + "UIExptips";
                break;
            case UIPanleID.UIUseExpVialPanel:
                path = UIResourcesPath.UI_PREFAB + "UIUseExpVialPanel";
                break;
            case UIPanleID.UINotJoinSocietyPanel:
                path = UIResourcesPath.UI_PREFAB + "UINotJoinSocietyPanel";
                break;
            case UIPanleID.UIHaveJoinSocietyPanel:
                path = UIResourcesPath.UI_PREFAB + "UIHaveJoinSocietyPanel";
                break;
            case UIPanleID.UISocietyInfoPanel:
                path = UIResourcesPath.UI_PREFAB + "UISocietyInfoPanel";
                break;
            case UIPanleID.UISocietyInteractionPort:
                path = UIResourcesPath.UI_PREFAB + "UISocietyInteractionPort";
                break;
            case UIPanleID.UIMountAndPet:
                path = UIResourcesPath.UI_PREFAB + "UIMountAndPet";
                break;
            case UIPanleID.UILottryEffect:
                path = UIResourcesPath.UI_PREFAB + "UILottryEffect";
                break;
            case UIPanleID.UILottryHeroEffect:
                path = UIResourcesPath.UI_PREFAB + "UILottryHeroEffect";
                break;
            case UIPanleID.UIPvP:
                path = UIResourcesPath.UI_PREFAB + "UIPVP";
                break;
            case UIPanleID.UIUseEnergyItemPanel:
                path = UIResourcesPath.UI_PREFAB + "UIUseEnergyItemPanel";
                break;
            case UIPanleID.UISocietyIconPanel:
                path = UIResourcesPath.UI_PREFAB + "UISocietyIconPanel";
                break;
            case UIPanleID.EquipDevelop:
                path = UIResourcesPath.UI_PREFAB + "EquipDevelop";
                break;
            case UIPanleID.UIEquipDetailPanel:
                path = UIResourcesPath.UI_PREFAB + "UIEquipDetailPanel";
                break;

            case UIPanleID.UIExpPropPanel:
                path = UIResourcesPath.UI_PREFAB + "UIExpPropPanel";
                break;
            case UIPanleID.UIDeadToReborn:
                path = UIResourcesPath.UI_PREFAB + "UIDeadToReborn";
                break;

            case UIPanleID.ArenaWinPanel:
                path = UIResourcesPath.UI_PREFAB + "ArenaWinPanel";
                break;
            case UIPanleID.FlopPanel:
                path = UIResourcesPath.UI_PREFAB + "FlopPanel";
                break;
            case UIPanleID.HeroSelectTipsPanel:
                path = UIResourcesPath.UI_PREFAB + "HeroSelectTipsPanel";
                break;
            case UIPanleID.MobaMap:
                path = UIResourcesPath.UI_PREFAB + "MobaMap";
                break;
            case UIPanleID.MobaStatInfo:
                path = UIResourcesPath.UI_PREFAB + "MobaStatInfo";
                break;
            case UIPanleID.UIActivity:
                path = UIResourcesPath.UI_PREFAB + "UIActivity";
                break;
            case UIPanleID.UICreateName:
                path = UIResourcesPath.UI_PREFAB + "UICreateName";
                break;
            case UIPanleID.UITaskEffectPanel:
                path = UIResourcesPath.UI_PREFAB + "UITaskEffectPanel";
                break;
            case UIPanleID.UITheBattlePanel:
                path = UIResourcesPath.UI_PREFAB + "TheBattlePanel";
                break;
            case UIPanleID.UIFubenTaskDialogue:
                path = UIResourcesPath.UI_PREFAB + "UIFubenTaskDialogue";
                break;
            default:
                path = string.Empty;
                break;
        }
        return path;
    }
    /// <summary>
    /// 判断按钮是否可点击
    /// </summary>
    //public float buttonClick = 0.0f;
    private float lastButtonClick = 0.0f;
    public bool CheckButtonOnClick()
    {
        if (Time.realtimeSinceStartup - lastButtonClick >= 0.3f)
        {

            lastButtonClick = Time.realtimeSinceStartup;
            return true;
        }
        else
        {

            return false;
        }
    }

}
/// <summary>
/// 存在的界面信息。
/// </summary>
class UIInfoData
{
    public UIPanleID UIKey { get; private set; }
    public string Path { get; private set; }
    public object[] UIObjParams { get; private set; }
    public UIInfoData(UIPanleID uiKey, string path, object[] uiObjParams)
    {
        UIKey = uiKey;
        Path = path;
        UIObjParams = uiObjParams;
    }
}

