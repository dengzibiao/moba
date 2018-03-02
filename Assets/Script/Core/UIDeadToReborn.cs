using UnityEngine;
using System.Collections;
using Tianyu;
public class UIDeadToReborn : GUIBase {

    public Transform btnPlaceReborn;//原地复活按钮
    public Transform btnSafeReborn;//安全区域复活按钮
    public UILabel placeRebornCore;//原地复活花费
    public UILabel safeRebornCore;//安全区域复活花费
    public UILabel timesafeReborn;//安全区域复活倒计时
    public Transform content;
    private uint mStartTime = 0;
    private uint mcount = 1;
    private uint needcd = 0;
    private static UIDeadToReborn m_Single;
    public static UIDeadToReborn GetInstance()
    {
        return m_Single;
    }
    private bool isCanReborn = false;
    // Use this for initialization
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIDeadToReborn;
    }

    void Start () {
        m_Single = this;
        Init();


    }
    public void Init()
    {
        if (content == null)
        {
            content = transform.FindChild("content");
        }
        if (btnPlaceReborn == null)
        {
            btnPlaceReborn = content.FindChild("placeReborn");
        }
        if(btnSafeReborn == null)
        {
            btnSafeReborn = content.FindChild("safeReborn");
        }
        if (placeRebornCore == null)
        {
            Transform obj = btnPlaceReborn.FindChild("count");

            if(obj!=null)
            placeRebornCore = obj.GetComponent<UILabel>();
        }
        if (safeRebornCore == null)
        {
            Transform obj = btnSafeReborn.FindChild("count");

            if(obj!=null)
            safeRebornCore = obj.GetComponent<UILabel>();
        }
        if(timesafeReborn!=null)
        {
            Transform obj = btnSafeReborn.FindChild("time");

            if (obj != null)
                timesafeReborn = obj.GetComponent<UILabel>();
        }
        if(content!=null&&content.gameObject.activeSelf)
        {
            content.gameObject.SetActive(false);
        }

    }
    public void OnSafeReborn()
    {
        if (content != null && content.gameObject.activeSelf)
        {
          //  uint keyid = playerData.GetInstance().selfData.keyId;
          if(isCanReborn)
            {
                ClientSendDataMgr.GetSingle().GetWalkSend().SendPlayerRevive(0, 0, 1, playerData.GetInstance().selfData.hp);

            }
            else
            {
                ClientSendDataMgr.GetSingle().GetWalkSend().SendPlayerRevive(0, 0, 2, playerData.GetInstance().selfData.hp);

            }
            content.gameObject.SetActive(false);
        }
    }
    public void OnPlaceReborn()
    {
        if (content != null && content.gameObject.activeSelf)
        {
          //  uint keyid = playerData.GetInstance().selfData.keyId;
           // ClientSendDataMgr.GetSingle().GetWalkSend().SendPlayerRevive(0, 0, 3, playerData.GetInstance().selfData.hp);
            content.gameObject.SetActive(false);
        }
    }
    public void show()
    {
        if(content!=null)
        {
            content.gameObject.SetActive(true);
        }
        mStartTime = Auxiliary.GetNowTime();
        mcount = 1;
        isCanReborn = false;
    }
    public void ResetData()
    {
        mStartTime = Auxiliary.GetNowTime();
        mcount = 1;
        isCanReborn = false;
        PlayerLevelUpNode levelupnode = FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList[playerData.GetInstance().selfData.level];
        if(levelupnode!=null)
        {
            needcd = (uint)(levelupnode.resurrection_cd);
        }
    }
    int timecount = 0;
    // Update is called once per frame
    void Update () {
        if(content!=null&&content.gameObject.activeSelf&& !isCanReborn)
        {
            timecount = Auxiliary.ShiftTimeSecond((long)(Auxiliary.GetNowTime() - mStartTime));
            if(timecount> mcount)
            {
                mcount += 1;
                if (timesafeReborn != null)
                    timesafeReborn.text = (needcd - mcount).ToString() + "秒后可复活";
                if(mcount >= needcd)
                {
                    // uint keyid = playerData.GetInstance().selfData.keyId;
                    //  ClientSendDataMgr.GetSingle().GetWalkSend().SendPlayerRevive(0, 0, 1, playerData.GetInstance().selfData.hp);
                    // content.gameObject.SetActive(false);
                    if (timesafeReborn != null)
                        timesafeReborn.text =  "马上复活";
                    isCanReborn = true;               
                  
                }
            }

        }
	
	}
}
