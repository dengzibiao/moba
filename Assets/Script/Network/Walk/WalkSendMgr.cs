using System;
using System.Threading;
using UnityEngine;
using System.Collections;
using System.Net;

public class WalkSendMgr
{

    public WalkSendMgr ()
    {
        m_SendThread = null;
        m_SendDataList = new ArrayList();
        m_Singleton = this;

        //m_WalkSend = new WalkSend( m_Singleton );//测试
    }

    public static WalkSendMgr GetSingle ()
    {
        if ( m_Singleton == null )
            new WalkSendMgr();
        return m_Singleton;
    }

    //public WalkSend GetWalkSend () { return m_WalkSend; } //测试

    public void SendPacket ( CWritePacket packet )
    {
        if ( packet != null )
        {
            //Debug.Log("messageid"+packet.GetPacketID());
            SendData( packet.GetPacketByte() , packet.GetPacketLen() , packet.GetPacketID() );
        }
    }

    //the true send data
    private void SendData ( byte [] bDataList , short nLength , ushort nMessageID )
    {
        if ( bDataList != null && nLength > 0 )
        {
            lock ( m_SendDataList )
            {
                //Debug.Log( "SendData: addData" );
                m_SendDataList.Add( new CSendData( bDataList , nLength , nMessageID ) );
            }
        }
    }

    //Create Send Thread
    public bool CreateSendThread ()
    {
        CloseSendThread();

        m_SendThread = new Thread( new ThreadStart( SendDataFromList ) );
        m_SendThread.Start();
        if ( m_SendThread != null && m_SendThread.ThreadState == ThreadState.Running )
            return true;
        return false;
    }

    //close send thread
    public void CloseSendThread ()
    {
        if ( m_SendThread != null )
            m_SendThread.Abort();
    }

    //send data from list
    private void SendDataFromList ()
    {
        try
        {
            while ( true )
            {
                if ( WalkNetMgr.GetSingle().IsConnect() )
                {
                    if ( m_SendDataList.Count <= 0 )
                    {
                        Thread.Sleep( 30 );
                        continue;
                    }
                    //Debug.Log("SendData:Send Count!"+m_SendDataList.Count);
                    System.Net.Sockets.NetworkStream netStream = WalkNetMgr.GetSingle().GetNetworkStream();
                    lock ( netStream )
                    {
                        if ( netStream != null && netStream.CanWrite )
                        {
                            lock ( m_SendDataList )
                            {
                                CSendData data = ( CSendData ) m_SendDataList [ 0 ];
                                netStream.Write( data.m_bDataList , 0 , data.m_nLength );
                                m_SendDataList.RemoveAt( 0 );
                                //Debug.Log("SendData:Send One!");
                                Debug.Log( "Send packet:" + data.m_nMessageID );
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
        catch ( System.Exception ex )
        {
            Debug.Log( ex.Message );
        }
    }

    //private data
    private Thread m_SendThread;
    //send data
    private ArrayList m_SendDataList;

    //private WalkSend m_WalkSend;//测试
    private static WalkSendMgr m_Singleton;
}
