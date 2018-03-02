using System;
using System.Collections;
using System.Collections.Generic;
public class CMapSend : CSendBase
{
	public CMapSend ( ClientSendDataMgr mgr) : base(mgr)
	{
		
	}
	
	//send Tech update
	//public void SendGetMapCastleList(short sMinX, short sMaxX , short sMinY ,short sMaxY)
	//{
	//	CWritePacket packet = new CWritePacket(MessageID.Map_C2D_GetMapCastleList);
	//	packet.WriteShort( sMinX );
	//	packet.WriteShort( sMaxX );
	//	packet.WriteShort( sMinY );
	//	packet.WriteShort( sMaxY );
	//	SendPacket( packet );
	//}
}

