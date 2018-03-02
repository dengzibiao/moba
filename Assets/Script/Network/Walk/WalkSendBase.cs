using System;
using UnityEngine;

public class WalkSendBase
{
    public WalkSendBase ( WalkSendMgr mgr )
    {
        m_Parent = mgr;
    }

    public void SendPacket ( CWritePacket packet )
    {
        //Debug.Log( "True服务器正在接收其他消息     " + GameLibrary.isSendPackage );
        //if ( GameLibrary.isSendPackage && ( !GameLibrary.isActiveSendPackahe ) )
        //{
        //    return;
        //}
        //GameLibrary.isSendPackage = true;
        //if ( Application.loadedLevelName == "UI_MajorCity" )
        //{
        //    Control.ShowGUI( GameLibrary.UIWaitForSever );
        //}
        //Debug.Log( "True代表服务器正在接收本次消息     " + GameLibrary.isSendPackage );
        if ( m_Parent != null )
            m_Parent.SendPacket( packet );
    }

    private WalkSendMgr m_Parent;
}
