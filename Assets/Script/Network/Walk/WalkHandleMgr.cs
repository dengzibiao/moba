using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WalkHandleMgr
{
    public WalkHandleMgr ()
    {
        //m_Handle = new Handle[(int)D2CMessage.ECount];
        m_byteDataList = new byte [ DataDefine.MAX_PACKET_SIZE ];
        m_Singleton = this;
        m_nbyteDataLength = 0;
        m_sPacketMinLength = ( short ) ( sizeof( short ));
        m_HandleDiction = new Dictionary<UInt32 , Handle>();

        m_HandleList = new ArrayList();
        //m_WalkHandle = new WalkHandle( m_Singleton );//测试
    }
    public static WalkHandleMgr GetSingle ()
    {
        if ( m_Singleton == null )
            new WalkHandleMgr();
        return m_Singleton;
    }

    public delegate bool Handle ( CReadPacket packet );

    //Regist All Handle
    public void RegistAllHandle ()
    {
        for ( int i = 0 ; i < m_HandleList.Count ; i++ )
        {
            WalkHandleBase handleBase = ( WalkHandleBase ) m_HandleList [ i ];
            if ( handleBase != null )
                handleBase.RegistAllHandle();
        }
    }

    public void AddHandleBase ( WalkHandleBase base1 )
    {
        m_HandleList.Add( base1 );
    }

    //Get One Handle
    public Handle GetHandle ( UInt32 mID )
    {
        Debug.Log( mID );
        if ( m_HandleDiction.ContainsKey( mID ) )
            return m_HandleDiction [ mID ];//m_Handle[eMessage-D2CMessage.EStart];
        return null;
    }

    public void ReceiveByte ( byte [] bData , int nLength )
    {
        if ( bData != null && nLength > 0 )
        {
            //GameLibrary.isSendPackage = false;
            //Debug.Log( "收到服务器回复消息更改状态     " + GameLibrary.isSendPackage );
            //if ( Application.loadedLevelName == "UI_MajorCity" )
            //{
            //    Control.HideGUI( GameLibrary.UIWaitForSever );
            //}
            if ( ( m_nbyteDataLength + nLength ) > DataDefine.MAX_PACKET_SIZE )
            {
                Debug.Log( "error! the packet is too large,more than " + DataDefine.MAX_PACKET_SIZE );
                Debug.Log( "m_nbyteDataLength:" + m_nbyteDataLength );
                Debug.Log( "nReceiveLen:" + nLength );
                //Array.Clear(m_byteDataList, 0, DataDefine.MAX_PACKET_SIZE);
                //m_nbyteDataLength = 0;
                Debug.Log("ReadPacket's length too large! ");

                return;
            }
            
            Array.Copy( bData , 0 , m_byteDataList , m_nbyteDataLength , nLength );
            m_nbyteDataLength += nLength;

            while ( true )
            {
                if (m_nbyteDataLength < m_sPacketMinLength)
                {
                   
                    Array.Clear(m_byteDataList, 0, DataDefine.MAX_PACKET_SIZE);
                    m_nbyteDataLength = 0;
                    Debug.Log("ReadPacket's length ");
                    return;
                }

                //				Debug.Log("m_nbyteDataLength:"+m_nbyteDataLength);
                //				Debug.Log("nReceiveLen:"+nLength);
                //handle
                CReadPacket readPacket = new CReadPacket( m_byteDataList );
                if ( m_nbyteDataLength >= readPacket.GetPacketLen() )
                {
                   
                    Debug.Log( "Receive Handle msgid:" + readPacket.GetMessageID() + "lens:" + readPacket.GetPacketLen() );
                    WalkHandleMgr.Handle hHandle = GetHandle( readPacket.GetMessageID() );
                    Debug.Log( hHandle );
                    if ( hHandle != null )
                        hHandle( readPacket );
                    else
                        Debug.Log( "No handle attach:" + readPacket.GetMessageID() );
                    //clear
                    int len = readPacket.GetPacketLen();
                    int nNewLen = m_nbyteDataLength - readPacket.GetPacketLen() ;
                    if ( nNewLen > 0 )
                    {
                        byte [] bTempData = new byte [ nNewLen ];
                        Array.Copy( m_byteDataList , readPacket.GetPacketLen() , bTempData , 0 , nNewLen );
                        Array.Clear( m_byteDataList , 0 , DataDefine.MAX_PACKET_SIZE );
                        Array.Copy( bTempData , 0 , m_byteDataList , 0 , nNewLen );
                    }
                    else
                        Array.Clear( m_byteDataList , 0 , DataDefine.MAX_PACKET_SIZE );
                    m_nbyteDataLength = nNewLen;
                }
                else
                    break;
            }
        }
    }

    //Regist Handle
    public void RegistHandle (UInt32 mID , Handle hHandle )
    {
        Handle oldHandle;
        if ( m_HandleDiction.TryGetValue( mID , out oldHandle ) )
        {
            m_HandleDiction [ mID ] += hHandle;
        }
        else
        {
            m_HandleDiction.Add( mID , hHandle );//m_Handle[eMessage-D2CMessage.EStart] = hHandle;
        }
    }

    //private data
    private Dictionary<UInt32, Handle> m_HandleDiction;

    private byte [] m_byteDataList;
    private int m_nbyteDataLength;
    private short m_sPacketMinLength;

    private ArrayList m_HandleList;
    //public WalkHandle m_WalkHandle; //测试
    private static WalkHandleMgr m_Singleton;
}
