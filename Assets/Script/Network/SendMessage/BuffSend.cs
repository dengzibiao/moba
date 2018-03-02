using System;
using System.Collections;
using System.Collections.Generic;
public class CBuffSend : CSendBase
{
	public CBuffSend ( ClientSendDataMgr mgr) : base(mgr)
	{
		
	}
	
	//send Tech update
	//public void SendGetBuffInfo(long lCastleID, long lBuffID)
	//{
	//	CWritePacket packet = new CWritePacket(MessageID.Buff_C2D_GetBuffInfo);
	//	packet.WriteInt64( lCastleID );
	//	packet.WriteInt64( lBuffID );
	//	SendPacket( packet );
	//}
}

