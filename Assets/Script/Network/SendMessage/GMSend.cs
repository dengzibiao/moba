using System;
public class CGMSend : CSendBase
{
	public CGMSend ( ClientSendDataMgr mgr) : base(mgr)
	{
		
	}
	
	// send add value
	//public void SendAddValue(MessageID packetId, long lCastleID, int nValue)
	//{
	//	CWritePacket packet = new CWritePacket(packetId);
	//	packet.WriteInt64( lCastleID );
	//	packet.WriteInt( nValue );
	//	SendPacket( packet );
	//}
	
	
	// send add food
	//public void SendAddFood(long lCastleID, int nValue)
	//{
	//	SendAddValue(MessageID.GM_C2D_AddFood, lCastleID, nValue);
	//}
	
	//// send add wood
	//public void SendAddWood(long lCastleID, int nValue)
	//{
	//	SendAddValue(MessageID.GM_C2D_AddWood, lCastleID, nValue);
	//}
	
	//public void SendAddRecruits(long lCastleID, int nValue)
	//{
	//	SendAddValue(MessageID.GM_C2D_AddWood, lCastleID, nValue);
	//}
	
	//// send add stone
	//public void SendAddStone(long lCastleID, int nValue)
	//{
	//	SendAddValue(MessageID.GM_C2D_AddStone, lCastleID, nValue);
	//}
	
	//// send add bronze
	//public void SendAddBronze(long lCastleID, int nValue)
	//{
	//	SendAddValue(MessageID.GM_C2D_AddBronze, lCastleID, nValue);
	//}
	
	//// send add money
	//public void SendAddMoney(long lCastleID, int nValue)
	//{
	//	SendAddValue(MessageID.GM_C2D_AddMoney, lCastleID, nValue);
	//}
	
	//// send add idle populace
	//public void SendAddIdlePopulace(long lCastleID, int nValue)
	//{
	//	SendAddValue(MessageID.GM_C2D_AddIdlePopulace, lCastleID, nValue);
	//}
	
	//// send add castle exp
	//public void SendAddCastleExp(long lCastleID, int nValue)
	//{
	//	SendAddValue(MessageID.GM_C2D_AddCastleExp, lCastleID, nValue);
	//}
	
	//// send add castle exp max
	//public void SendAddCastleMaxExp(long lCastleID, int nValue)
	//{
	//	SendAddValue(MessageID.GM_C2D_AddCastleMaxExp, lCastleID, nValue);
	//}
	
	//// send add silver
	//public void SendAddSilver(long lCastleID, int nValue)
	//{
	//	SendAddValue(MessageID.GM_C2D_AddSilver, lCastleID, nValue);
	//}
	
	//// send add gold
	//public void SendAddGold(long lCastleID, int nValue)
	//{
	//	SendAddValue(MessageID.GM_C2D_AddGold, lCastleID, nValue);
	//}
	
	//public void SendAddMonarchAction(long lCastleID, int nValue)
	//{
	//	SendAddValue(MessageID.GM_C2D_AddMonarchAction, lCastleID, nValue);
	//}
	
	//public void SendAddMonarchExp(long lCastleID, int nValue)
	//{
	//	SendAddValue(MessageID.GM_C2D_AddMonarchExp, lCastleID, nValue);
	//}
	
	//public void SendAddMonarchMilitaryExploit(long lCastleID, int nValue)
	//{
	//	SendAddValue(MessageID.GM_C2D_AddMonarchMilitaryExploit, lCastleID, nValue);
	//}
	
	//public void SendAddMonarchReputation(long lCastleID, int nValue)
	//{
	//	SendAddValue(MessageID.GM_C2D_AddMonarchReputation, lCastleID, nValue);
	//}
	
	//public void SendAddMonarchStrategy(long lCastleID, int nValue)
	//{
	//	SendAddValue(MessageID.GM_C2D_AddMonarchStrategy, lCastleID, nValue);
	//}
	
	//public void SendExecuteAction(long lCastleID, int ActionID ,int nValue1 ,int nValue2,int nValue3,int nValue4)
	//{
	//	CWritePacket packet = new CWritePacket(MessageID.GM_C2D_ExecuteAction);
	//	packet.WriteInt64( lCastleID );
	//	packet.WriteInt( ActionID );
	//	packet.WriteInt( nValue1 );
	//	packet.WriteInt( nValue2 );
	//	packet.WriteInt( nValue3 );
	//	packet.WriteInt( nValue4 );
	//	SendPacket( packet );
	//}
	
	//public void SendAddItem(long lCastleID, int nTypeID , int nCount)
	//{
	//	CWritePacket packet = new CWritePacket(MessageID.GM_C2D_AddItem);
	//	packet.WriteInt64( lCastleID );
	//	packet.WriteInt( nTypeID );
	//	packet.WriteInt( nCount );
	//	SendPacket( packet );
	//}
}