using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class UIAbattiorPro : MonoBehaviour
{

    public GameObject Mask;
    public UILabel PromLabel;
    public UILabel DiamondLabel;
    public UIButton CancelBtn;
    public UIButton DefineBtn;
    public GameObject CDorNumber;
    public GameObject Later;
    public UILabel laterLabel;
    public UIButton LaterDefineBtn;

    //UIAbattiorList uiAbattior;

    bool isRef = false;

    int diamond;

    void Start()
    {
        //uiAbattior = GetComponentInParent<UIAbattiorList>();
        UIEventListener.Get(Mask).onClick = HidePanel;
        EventDelegate.Set(CancelBtn.onClick, OnCancelBtnClick);
        EventDelegate.Set(DefineBtn.onClick, OnDefineBtnClick);
        EventDelegate.Set(LaterDefineBtn.onClick, OnCancelBtnClick);
    }

    public void RefreshPrompt(bool isRef)
    {
        CDorNumber.SetActive(true);
        Later.SetActive(false);
        gameObject.SetActive(true);

        this.isRef = isRef;

        if (isRef)
        {
            diamond = 50;
            PromLabel.text = "立即开始挑战";
        }
        else
        {
            //VIP表
            ResetLaterNode node = FSDataNodeTable<ResetLaterNode>.GetSingleton().FindDataByType(1);
            if (Convert.ToInt32(GameLibrary.ArenaNumber[2]) >= node.buy_abattoir.Length - 1)
                diamond = node.buy_abattoir[node.buy_abattoir.Length - 1];
            else
                diamond = node.buy_abattoir[Convert.ToInt32(GameLibrary.ArenaNumber[2])];
            PromLabel.text = "获得挑战次数";
        }
        
        DiamondLabel.text = diamond + "";

    }

    void OnDefineBtnClick()
    {
        if (playerData.GetInstance().baginfo.diamond >= diamond)
        {
            //if (isRef)
            //{
            //    uiAbattior.ClearOnCD();
            //}
            //else
            //{
            //    uiAbattior.AddDareNum();
            //}
            //ClientSendDataMgr.GetSingle().GetBattleSend().SendArenaReloadCD(isRef ? 1 : 2);

            Dictionary<string, object> newpacket = new Dictionary<string, object>();
            newpacket.Add("arg1", isRef ? 1 : 2);
            Singleton<Notification>.Instance.Send(MessageID.pve_arena_reload_cd_req, newpacket, C2SMessageType.ActiveWait);

            OnCancelBtnClick();
        }
        else
        {
            //UIBuyEnergyVitality.Instance.SetInfo(ActionPointType.Energy);
            //Control.ShowGUI(GameLibrary.UIBuyEnergyVitality);
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "当前钻石不足");
            gameObject.SetActive(false);
        }
    }

    public void RefresLaterPrompt(string prompt)
    {
        gameObject.SetActive(true);
        CDorNumber.SetActive(false);
        Later.SetActive(true);
        laterLabel.text = prompt;
    }

    void OnCancelBtnClick()
    {
        HidePanel(gameObject);
    }

    void HidePanel(GameObject go)
    {
        gameObject.SetActive(false);
    }
}
