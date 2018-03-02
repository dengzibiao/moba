using UnityEngine;
using System.Collections;
using Tianyu;


public class UIBuySweepVoucher : MonoBehaviour
{
    public GUISingleButton buyBtn;
    public GUISingleButton cancelBtn;
    public GUISingleLabel des;
    public GameObject mask;

    int buyCount = 10;
    long moneyCount = 0;
    int needCount = 0;

    PromptType type;

    SceneNode sn;
    int resetDiamond;

    void Start()
    {
        buyBtn.onClick = OnBuyBtnClick;
        cancelBtn.onClick = OnCancelBtnClick;
        UIEventListener.Get(mask).onClick = OnMaskClick;
    }

    public void RefreshBuyUI(PromptType type, SceneNode sn)
    {
        gameObject.SetActive(true);
        this.type = type;
        this.sn = sn;
        moneyCount = playerData.GetInstance().GetMyMoneyByType(MoneyType.Diamond);
        if (type == PromptType.Buy)
        {
            ItemNodeState item = GameLibrary.Instance().ItemStateList[110000100];
            for (int i = 0; i < item.cprice.Length; i++)
            {
                if (item.cprice[i] != 0)
                {
                    needCount = item.cprice[i];
                    break;
                }
            }
            des.text = "花费" + (buyCount * needCount) + "钻石购买" + buyCount + "个扫荡卷";
            UnityUtil.SetBtnState(buyBtn.gameObject, true);
        }
        else if (type == PromptType.WarTimes)
        {
            ResetLaterNode node = FSDataNodeTable<ResetLaterNode>.GetSingleton().FindDataByType(1);//GameLibrary.mapElite[sn.SceneId][1]
            int reset = GameLibrary.mapElite[sn.SceneId][1] >= node.resetStage.Length - 1 ? node.resetStage.Length - 1 : GameLibrary.mapElite[sn.SceneId][1];
            resetDiamond = node.resetStage[reset];
            des.text = "花费" + resetDiamond + "钻石购买" + 3 + "次征战";
            UnityUtil.SetBtnState(buyBtn.gameObject, true);
        }
        else
        {
            des.text = "钻石不足，去充值";
            UnityUtil.SetBtnState(buyBtn.gameObject, false);
        }
    }

    void OnBuyBtnClick()
    {
        if (type == PromptType.Buy)
        {
            if (moneyCount >= buyCount * needCount)
                ClientSendDataMgr.GetSingle().GetBattleSend().SendBuySomeone(110000100, buyCount);
            else
                RefreshBuyUI(PromptType.Diamond, sn);
        }
        else if (type == PromptType.WarTimes)
        {
            if (moneyCount >= resetDiamond)
                ClientSendDataMgr.GetSingle().GetBattleSend().SendResetEliteDungeon(sn.bigmap_id, sn.SceneId);
            else
                RefreshBuyUI(PromptType.Diamond, sn);
        }
        else
        {
            //充值界面跳转
            //暂无充值界面
        }
        buyBtn.isEnabled = false;
        CDTimer.GetInstance().AddCD(0.5f, (int count, long id) => { buyBtn.isEnabled = true; });
    }

    void OnCancelBtnClick()
    {
        gameObject.SetActive(false);
    }

    void OnMaskClick(GameObject go)
    {
        gameObject.SetActive(false);
    }



}
