using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class CHandleMgr
{
    public CHandleMgr()
    {
        //m_Handle = new Handle[(int)D2CMessage.ECount];
        m_byteDataList = new byte[DataDefine.MAX_PACKET_SIZE];
        m_Singleton = this;
        m_nbyteDataLength = 0;
        m_sPacketMinLength = (short)(sizeof(short));
        m_HandleDiction = new Dictionary<UInt32, Handle>();

        m_HandleList = new ArrayList();
        m_LoginHandle = new CLoginHandle(m_Singleton);
        m_ResHandle = new CResourceHandle(m_Singleton);
        m_ItemHandle = new CItemHandle(m_Singleton);
        m_ShopHandle = new CShopHandle(m_Singleton);
        m_HeroSkillHandle = new CHeroSkillHandle(m_Singleton);
        m_HeroHandle = new CHeroHandle(m_Singleton);
        m_RoleHandle = new CRoleHandle(m_Singleton);
        m_LotteryHandle = new CLotteryHandle(m_Singleton);
        m_ActionPointHandle = new CActionPointHandle(m_Singleton);
        m_LotteryHandle = new CLotteryHandle(m_Singleton);
        m_GoldHandHandle = new CGoldHandHandle(m_Singleton);
        m_LotterHotHandle = new CLotterHotHandle(m_Singleton);
        m_MailHandle = new CMailHandle(m_Singleton);
        m_ChatHandle = new CChatHandle(m_Singleton);
        m_BattleHandle = new CBattleHandle(m_Singleton);
        m_MobaHandle = new CMobaHandle(m_Singleton);
        m_RankList = new CRankListHandle(m_Singleton);
        m_FriendHandle = new CFriendHandle(m_Singleton);
        m_TaskHandle = new TaskHandle(m_Singleton);
        m_MarqueeHandle = new CMarqueeHandle(m_Singleton);
        m_UISign_inHandle = new UISign_inHandle(m_Singleton);
        m_GetEnergyHandle = new CGetEnergyHandle(m_Singleton);
        m_TitleHandle = new CTitleHandle(m_Singleton);
        m_NewplayerReward = new CNewplayerRewardHandle(m_Singleton);
        m_WalkHandle = new WalkHandle(m_Singleton);
        m_CGuideFuncOpenHandle = new CGuideFuncOpenHandle(m_Singleton);
        m_SocietyHandle = new CSocietyHandle(m_Singleton);
        m_FixedDataHandle = new CFixedDataHandle(m_Singleton);
        m_GuideHandle = new CGuideHandle(m_Singleton);
        m_PetHandle = new CPetHandel(m_Singleton);
    }
    public static CHandleMgr GetSingle()
    {
        if (m_Singleton == null)
            new CHandleMgr();
        return m_Singleton;
    }

    public delegate bool Handle(CReadPacket packet);

    //Regist All Handle
    public void RegistAllHandle()
    {
        for (int i = 0; i < m_HandleList.Count; i++)
        {
            CHandleBase handleBase = (CHandleBase)m_HandleList[i];
            if (handleBase != null)
                handleBase.RegistAllHandle();
        }
    }

    public void AddHandleBase(CHandleBase base1)
    {
        m_HandleList.Add(base1);
    }

    //Get One Handle
    public Handle GetHandle(UInt32 mID)
    {
        if (m_HandleDiction.ContainsKey(mID))
            return m_HandleDiction[mID];//m_Handle[eMessage-D2CMessage.EStart];

        return null;
    }
    //private bool mlock = false;
    //void Lock()
    //{
    //    mlock = true;
    //}
    //void UnLock()
    //{
    //    mlock = false;
    //}
    //public bool GetLock()
    //{
    //    return mlock;
    //}

    public void ReceiveByte(byte[] bData, int nLength)
    {
        // if (!GetLock())
        {

            if (bData != null && nLength > 0)
            {
                GameLibrary.isSendPackage = false;
                // Debug.Log("收到服务器回复消息更改状态     " + GameLibrary.isSendPackage + "  " + nLength);
                if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01 || Globe.isFightGuide)
                {
                    if (Control.GetUIObject(UIPanleID.UIWaitForSever) != null)
                        Control.HideGUI(UIPanleID.UIWaitForSever);
                }
                if ((m_nbyteDataLength + nLength) > DataDefine.MAX_PACKET_SIZE)
                {
                    Debug.Log("error! the packet is too large,more than " + DataDefine.MAX_PACKET_SIZE);
                    Debug.Log("m_nbyteDataLength:" + m_nbyteDataLength);
                    Debug.Log("nReceiveLen:" + m_nbyteDataLength + "---" + nLength);
                    //Array.Clear(m_byteDataList, 0, DataDefine.MAX_PACKET_SIZE);
                    //m_nbyteDataLength = 0;
                    Debug.Log("ReadPacket's length too large! ");
                    return;
                }

                Array.Copy(bData, 0, m_byteDataList, m_nbyteDataLength, nLength);
                m_nbyteDataLength += nLength;

                while (true)
                {
                    // Lock();
                    if (m_nbyteDataLength == 0)
                    {
                        //Array.Clear(m_byteDataList, 0, DataDefine.MAX_PACKET_SIZE);
                        //m_nbyteDataLength = 0;
                        //Debug.Log("ReadPacket's length ");
                        // UnLock();
                        return;
                    }
                    else if (m_nbyteDataLength < m_sPacketMinLength)
                    {
                        Array.Clear(m_byteDataList, 0, DataDefine.MAX_PACKET_SIZE);
                        m_nbyteDataLength = 0;
                        Debug.Log("ReadPacket's length ");
                        // UnLock();
                        return;
                    }

                    //				Debug.Log("m_nbyteDataLength:"+m_nbyteDataLength);
                    //				Debug.Log("nReceiveLen:"+nLength);
                    //handle
                    CReadPacket readPacket = new CReadPacket(m_byteDataList);
                    if (m_nbyteDataLength >= readPacket.GetPacketLen())
                    {
                        //  Profiler.maxNumberOfSamplesPerFrame = 
                        // Debug.Log(Profiler.maxNumberOfSamplesPerFrame);
                        CHandleMgr.Handle hHandle = GetHandle(readPacket.GetMessageID());

                        string logString = "Receive msgid:" + readPacket.GetMessageID() + readPacket.GetLogMessageID() + " Lens:" + readPacket.GetPacketLen();
                        // Debug.Log(hHandle);
                        if (hHandle != null)
                        {

                            GameLibrary.Instance().PackedCount = 0;
                            if (msgDishandled.Count == 0 || !msgDishandled.Contains(readPacket.GetMessageID()))
                            {
                                //Dictionary<string, object> data = readPacket.data;
                                //if (data.ContainsKey("ret"))
                                //{
                                //    int resolt = int.Parse(data["ret"].ToString());
                                //    if (resolt == 0)
                                //    {
                                bool isTrue = hHandle(readPacket);
                                if (isTrue)
                                {
                                    Singleton<Notification>.Instance.ReceiveMessageList(readPacket.GetMessageID());
                                }
                                //    }
                                //    else
                                //    {
                                //        if (data.ContainsKey("desc"))
                                //        {
                                //            Debug.Log(data["desc"].ToString());
                                //        }

                                //    }
                                //}
                                //else
                                //{
                                //    hHandle(readPacket);
                                //}




                                logString += " Handle:" + hHandle.Target.GetType() + "." + hHandle.Method.Name + " Time:" + Time.realtimeSinceStartup;
                            }
                        }
                        else
                            logString += " No handle attach";
                        if (DataDefine.isLogMsg && DataDefine.filterWalkMsg(readPacket.GetMessageID()))
                        {
                            Debug.Log(logString);
                        }

                        //clear
                        int nNewLen = m_nbyteDataLength - readPacket.GetPacketLen();
                        if (nNewLen > 0)
                        {
                            byte[] bTempData = new byte[nNewLen];
                            Array.Copy(m_byteDataList, readPacket.GetPacketLen(), bTempData, 0, nNewLen);
                            Array.Clear(m_byteDataList, 0, DataDefine.MAX_PACKET_SIZE);
                            Array.Copy(bTempData, 0, m_byteDataList, 0, nNewLen);
                        }
                        else
                            Array.Clear(m_byteDataList, 0, DataDefine.MAX_PACKET_SIZE);
                        m_nbyteDataLength = nNewLen;

                    }
                    else
                    {
                        // UnLock();
                        break;
                    }
                    // UnLock();
                }
            }
        }
    }

    bool IsWalkMsg(uint msgId)
    {
        return msgId == 261 || msgId == 260 || msgId == 259;
    }

    public void Clear()
    {
        if (m_byteDataList != null)
        {
            Array.Clear(m_byteDataList, 0, DataDefine.MAX_PACKET_SIZE);
            m_byteDataList = null;
        }

    }
    //Regist Handle
    public void RegistHandle(UInt32 mID, Handle hHandle)
    {
        Handle oldHandle;
        if (m_HandleDiction.TryGetValue(mID, out oldHandle))
        {
            m_HandleDiction[mID] = hHandle;
        }
        else
        {
            m_HandleDiction.Add(mID, hHandle);//m_Handle[eMessage-D2CMessage.EStart] = hHandle;
        }
    }

    public void DisableHandle(UInt32 mID)
    {
        if (!msgDishandled.Contains(mID))
            msgDishandled.Add(mID);
    }
    //private data
    public List<UInt32> msgDishandled = new List<UInt32>();

    //private data
    private Dictionary<UInt32, Handle> m_HandleDiction;

    private byte[] m_byteDataList;
    private int m_nbyteDataLength;
    private short m_sPacketMinLength;

    private ArrayList m_HandleList;
    private CLoginHandle m_LoginHandle;
    private CResourceHandle m_ResHandle;
    private CItemHandle m_ItemHandle;
    private CShopHandle m_ShopHandle;
    private CHeroSkillHandle m_HeroSkillHandle;
    private CHeroHandle m_HeroHandle;
    private CRoleHandle m_RoleHandle;
    private CLotteryHandle m_LotteryHandle;
    private CActionPointHandle m_ActionPointHandle;
    private CGoldHandHandle m_GoldHandHandle;
    private CLotterHotHandle m_LotterHotHandle;
    private CMailHandle m_MailHandle;
    private CChatHandle m_ChatHandle;
    private CBattleHandle m_BattleHandle;
    private CMobaHandle m_MobaHandle;
    private CRankListHandle m_RankList;
    private CFriendHandle m_FriendHandle;
    private CMarqueeHandle m_MarqueeHandle;
    private TaskHandle m_TaskHandle;
    private CGetEnergyHandle m_GetEnergyHandle;
    private UISign_inHandle m_UISign_inHandle;
    private CTitleHandle m_TitleHandle;
    private CNewplayerRewardHandle m_NewplayerReward;
    private WalkHandle m_WalkHandle;
    private CGuideFuncOpenHandle m_CGuideFuncOpenHandle;
    private CSocietyHandle m_SocietyHandle;
    private CFixedDataHandle m_FixedDataHandle;
    private CGuideHandle m_GuideHandle;
    private CPetHandel m_PetHandle;
    private static CHandleMgr m_Singleton;
}

