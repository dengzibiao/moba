using System;
using System.Threading;
using UnityEngine;
using System.Collections;
using System.Net;

public class CSendData
{
    public CSendData(byte[] bList, short nLength, ushort nMessageID)
    {
        //m_bDataList = bList;

        m_bDataList = new byte[nLength];

        Array.Copy(bList, m_bDataList, nLength);
        m_nLength = nLength;
        m_nMessageID = nMessageID;
    }

    public byte[] m_bDataList;
    public short m_nLength;
    public ushort m_nMessageID;
}

public class ClientSendDataMgr
{
    public ClientSendDataMgr()
    {
        m_SendThread = null;
        m_SendDataList = new ArrayList();
        m_Singleton = this;

        m_LoginSend = new CLoginSend(m_Singleton);
        m_GmSend = new CGMSend(m_Singleton);
        m_ItemSend = new CItemSend(m_Singleton);
        m_BuffSend = new CBuffSend(m_Singleton);
        m_MapSend = new CMapSend(m_Singleton);
        m_MailSend = new CMailSend(m_Singleton);
        m_ShopSend = new CShopSend(m_Singleton);
        m_HeroSkillSend = new CHeroSkillSend(m_Singleton);
        //   m_RedisSend = new CRedisSend(m_Singleton);
        m_HeroSend = new CHeroSend(m_Singleton);
        m_RoleSend = new CRoleSend(m_Singleton);
        m_LotterySend = new CLotterySend(m_Singleton);
        m_AactionPointSend = new CActionPointSend(m_Singleton);
        m_GoldHandSend = new CGoldHandSend(m_Singleton);
        m_LotterHotSend = new CLotterHotSend(m_Singleton);
        m_ChatSend = new CChatSend(m_Singleton);
        m_BattleSend = new CBattleSend(m_Singleton);
        m_MobaSend = new CMobaSend(m_Singleton);
        m_RankLisrSend = new CRankListSend(m_Singleton);
        m_FriendSend = new CFriendSend(m_Singleton);
        m_TaskSend = new CTaskSend(m_Singleton);
        m_GetEnergySend = new CGetEnergySend(m_Singleton);
        m_UISign_inSend = new UISign_inSend(m_Singleton);
        m_TitleSend = new CTitleSend(m_Singleton);
        m_EveryDailyTaskSend = new CEveryDailyTaskSend(m_Singleton);
        m_NewplayerRewardSend = new CNewplayerRewardSend(m_Singleton);
        m_WalkSend = new WalkSend( m_Singleton );
        m_SocietySend = new CSocietySend(m_Singleton);
        m_GuideSend = new CGuideSend(m_Singleton);
        m_PetSend = new CPetSend(m_Singleton);
        m_SendMessage = new CSendMessage(m_Singleton);

    }

    public static ClientSendDataMgr GetSingle()
    {
        if (m_Singleton == null)
            new ClientSendDataMgr();
        return m_Singleton;
    }

    public CLoginSend GetLoginSend() { return m_LoginSend; }
    public CGMSend GetGMSend() { return m_GmSend; }
    public CItemSend GetItemSend() { return m_ItemSend; }
    public CBuffSend GetBuffSend() { return m_BuffSend; }
    public CMapSend GetMapSend() { return m_MapSend; }
    public CMailSend GetMailSend() { return m_MailSend; }
    public CShopSend GetCShopSend() { return m_ShopSend; }
    public CHeroSkillSend GetHeroSkillSend() { return m_HeroSkillSend; }
    public CHeroSend GetHeroSend() { return m_HeroSend; }
    public CLotterySend GetLotterySend() { return m_LotterySend; }
    public CGoldHandSend GetGoldHandSend() { return m_GoldHandSend; }
    public CLotterHotSend GetLotterHotSend() { return m_LotterHotSend; }
    public CBattleSend GetBattleSend() { return m_BattleSend; }
    public CMobaSend GetMobaSend () { return m_MobaSend; }
    public CFriendSend GetFriendSend() { return m_FriendSend; }
    public CTaskSend GetTaskSend() { return m_TaskSend; }
    public CRankListSend GetRankListSend() { return m_RankLisrSend; }
    public CEveryDailyTaskSend GetEveryDailyTaskSend() { return m_EveryDailyTaskSend; }
    
    //  public CRedisSend       GetRedisSend() { return m_RedisSend; }

    public CRoleSend GetRoleSend() { return m_RoleSend; }
    public CActionPointSend GetActionPointSend() { return m_AactionPointSend; }
    public CChatSend GetChatSend() { return m_ChatSend; }
    public CGetEnergySend GetEnergySend() { return m_GetEnergySend; }
    public UISign_inSend GetUISign_inSend() { return m_UISign_inSend; }
    public CNewplayerRewardSend GetNewplayerReward() { return m_NewplayerRewardSend; }
    public CTitleSend GetTitleSend() { return m_TitleSend; }
    public WalkSend GetWalkSend () { return m_WalkSend; }
    public CSocietySend GetSocietySend() { return m_SocietySend; }
    public CGuideSend GetGuideSend() { return m_GuideSend; }
    public CPetSend GetPetSend() { return m_PetSend; }
    public CSendMessage GetMessageSend() { return m_SendMessage; }
    public void SendPacket(CWritePacket packet)
    {
        if (packet != null)
        {
            SendData(packet.GetPacketByte(), packet.GetPacketLen(), packet.GetPacketID());
        }
    }

    //the true send data
    private void SendData(byte[] bDataList, short nLength, ushort nMessageID)
    {
        if (bDataList != null && nLength > 0)
        {
            lock (m_SendDataList)
            {
                //Debug.Log( "SendData: addData" );
                m_SendDataList.Add(new CSendData(bDataList, nLength, nMessageID));
            }
        }
    }

    //Create Send Thread
    public bool CreateSendThread()
    {
        CloseSendThread();

        m_SendThread = new Thread(new ThreadStart(SendDataFromList));
        m_SendThread.Start();
		if (m_SendThread != null && m_SendThread.ThreadState == ThreadState.Running) {
			
			return true;
		}
        return false;
    }

    //close send thread
    public void CloseSendThread()
    {
        if (m_SendThread != null)
            m_SendThread.Abort();
    }

    //send data from list
    private void SendDataFromList()
    {
        try
        {
            while (true)
            {
                if (ClientNetMgr.GetSingle().IsConnect())
                {
                    if (m_SendDataList.Count <= 0)
                    {
                        Thread.Sleep(30);
                        continue;
                    }
                    //Debug.Log("SendData:Send Count!"+m_SendDataList.Count);
                    System.Net.Sockets.NetworkStream netStream = ClientNetMgr.GetSingle().GetNetworkStream();
                    lock (netStream)
                    {
                        if (netStream != null && netStream.CanWrite)
                        {
                            lock (m_SendDataList)
                            {
                                CSendData data = (CSendData)m_SendDataList[0];
                                netStream.Write(data.m_bDataList, 0, data.m_nLength);
                                m_SendDataList.RemoveAt(0);
                                //Debug.Log( "<color=#FFc900>Send packet:</color>"+ data.m_nMessageID);


                                //Debug.Log("Send packet:" + data.m_nMessageID);
                            }
                        }
                    }
                }
                else
                {
                    //CloseSendThread();
                    break;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    //private data
    private Thread m_SendThread;
    //send data
    private ArrayList m_SendDataList;

    private CLoginSend m_LoginSend;
    private CItemSend m_ItemSend;
    private CBuffSend m_BuffSend;
    private CGMSend m_GmSend;
    private CMapSend m_MapSend;
    private CMailSend m_MailSend;
    private CShopSend m_ShopSend;
    private CHeroSkillSend m_HeroSkillSend;
    // private CRedisSend                      m_RedisSend;
    private CHeroSend m_HeroSend;
    private CLotterySend m_LotterySend;
    private CRoleSend m_RoleSend;
    private CActionPointSend m_AactionPointSend;
    private CGoldHandSend m_GoldHandSend;
    private CLotterHotSend m_LotterHotSend;
    private CChatSend m_ChatSend;
    private CBattleSend m_BattleSend;
    private CMobaSend m_MobaSend;
    private CRankListSend m_RankLisrSend;
    private CFriendSend m_FriendSend;
    private CTaskSend m_TaskSend;
    private CGetEnergySend m_GetEnergySend;
    private UISign_inSend m_UISign_inSend;
    private CTitleSend m_TitleSend;
    private CNewplayerRewardSend m_NewplayerRewardSend;
    private WalkSend m_WalkSend;
    private static ClientSendDataMgr m_Singleton;
    private CEveryDailyTaskSend m_EveryDailyTaskSend;
    private CSocietySend m_SocietySend;
    private CGuideSend m_GuideSend;
    private CPetSend m_PetSend;
    private CSendMessage m_SendMessage;
}

