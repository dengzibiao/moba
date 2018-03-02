using UnityEngine;
using System.Collections;

public class UIActivity : GUIBase
{

    public static UIActivity instance;

    public UIActModeSelect ActModeSelect;
    public UIActDifficSelect ActDifficSelect;

    [HideInInspector]
    public OpenSourceType type;


    //30100	黄金商路
    //30200	药剂商路
    //30300	暮色峡谷
    //30400	冰封雪域
    //30500	沼泽湿地

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIActivity;
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();

        Singleton<Notification>.Instance.RegistMessageID(MessageID.pve_eventdungeon_list_ret, this.GetUIKey());
        Singleton<Notification>.Instance.RegistMessageID(MessageID.pve_eventdungeon_list_ret, UIPanleID.UIActivity);
        Singleton<Notification>.Instance.Send(MessageID.pve_eventdungeon_list_req, C2SMessageType.ActiveWait);
    }
    public override void ReceiveData(uint messageID)
    {
        base.ReceiveData(messageID);

        Show();
        Control.PlayBgmWithUI(UIPanleID.UIActivity);

    }
    protected override void Init()
    {
        instance = this;

        if (null != Globe.backPanelParameter && (UIPanleID)Globe.backPanelParameter[0] == UIPanleID.UIActivity)
        {
            if (Globe.backPanelParameter.Length > 1)
                OpenModeSelect((int)Globe.backPanelParameter[1]);
            Globe.backPanelParameter = null;
        }
        else if (Globe.openSceenID != 0)
        {
            int modelid = Globe.openSceenID / 100;
            modelid *= 100;
            OpenModeSelect(modelid);
            Globe.openSceenID = 0;
        }
    }

    public string ModulIcon(int sceneID)
    {
        switch (sceneID)
        {
            case 30100: return "huangjinshanglu";
            case 30200: return "yaojishanglu";
            case 30300: return "musexiagu";
            case 30400: return "bingfengxuedi";
            default: return "zhaozeshidi";
        }
    }

    void LoadSceneEntry()
    {
        GameObject go = Instantiate(Resources.Load("Prefab/UIPanel/SceneEntry")) as GameObject;
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        ActDifficSelect.sceneEnter = go.GetComponent<UISceneEntry>();
        UISceneEntry sceneEntry = Control.Show(UIPanleID.SceneEntry) as UISceneEntry;
        ActDifficSelect.sceneEnter = sceneEntry;
    }

    public void OpenModeSelect(int id)
    {
        if (id == 0 || null == ActModeSelect) return;
        ActModeSelect.OnItemClick(id);
    }

    public void HidePanel()
    {
        //Hide();
        Control.HideGUI();
        Control.PlayBGmByClose(UIPanleID.UIActivity);
    }

}
