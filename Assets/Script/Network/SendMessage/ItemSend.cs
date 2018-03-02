using System.Collections.Generic;

public class CItemSend : CSendBase
{
	public CItemSend ( ClientSendDataMgr mgr) : base(mgr)
	{
		
	}

    public void SendGetBackPackList(C2SMessageType c2sType)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        PackNormalKvpAndSend(MessageID.common_backpack_list_req, newpacket, c2sType);
    }

    public void SendSellItem(List<object> obj)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", obj);
        PackNormalKvpAndSend(MessageID.common_sell_item_req, newpacket, C2SMessageType.PASVWait);
    }


    public void SendUseItem(long itemid, string uuid, int count)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", itemid);
        newpacket.Add("arg2", uuid);
        newpacket.Add("arg3", count);
        PackNormalKvpAndSend(MessageID.common_use_item_req, newpacket, C2SMessageType.PASVWait);
    }
    //send Tech update
    //public void SendGetItemInfo(long lCastleID, long lItemID)
    //{
    //	CWritePacket packet = new CWritePacket(MessageID.Item_C2D_GetItemInfo);
    //	packet.WriteInt64( lCastleID );
    //	packet.WriteInt64( lItemID );
    //	SendPacket( packet );
    //}


    //public void SendBuyItem(long lCastleID, int nTypeID , int nCount)
    //{
    //	CWritePacket packet = new CWritePacket(MessageID.Item_C2D_BuyItem);
    //	packet.WriteInt64( lCastleID );
    //	packet.WriteInt( nTypeID );	
    //	packet.WriteInt( nCount );		
    //	SendPacket( packet );
    //}

    //public void SendSellItem(long lCastleID, long lItemID , int nCount)
    //{
    //	CWritePacket packet = new CWritePacket(MessageID.Item_C2D_SellItem);
    //	packet.WriteInt64( lCastleID );
    //	packet.WriteInt64( lItemID );	
    //	packet.WriteInt( nCount );		
    //	SendPacket( packet );
    //}


    //public void SendDropItem(long lCastleID, long lItemID , int nCount)
    //{
    //	CWritePacket packet = new CWritePacket(MessageID.Item_C2D_DropItem);
    //	packet.WriteInt64( lCastleID );
    //	packet.WriteInt64( lItemID );	
    //	packet.WriteInt( nCount );		
    //	SendPacket( packet );
    //}

    //public void SendUseItem(long lCastleID, long lItemID , int nCount)
    //{
    //	CWritePacket packet = new CWritePacket(MessageID.Item_C2D_UseItem);
    //	packet.WriteInt64( lCastleID );
    //	packet.WriteInt64( lItemID );	
    //	packet.WriteInt( nCount );		
    //	SendPacket( packet );
    //}		

    //public void SendStrengthenItem(long lCastleID, long lItemID )
    //{
    //	CWritePacket packet = new CWritePacket(MessageID.Item_C2D_StrengthenItem);
    //	packet.WriteInt64( lCastleID );
    //	packet.WriteInt64( lItemID );	
    //	SendPacket( packet );
    //}	
    //public void SendDecomposeItem(long lCastleID, long lItemID )
    //{
    //	CWritePacket packet = new CWritePacket(MessageID.Item_C2D_DecomposeItem);
    //	packet.WriteInt64( lCastleID );
    //	packet.WriteInt64( lItemID );	
    //	SendPacket( packet );
    //}		
    //public void SendEmbedGemToItem(long lCastleID, long lItemID , byte bPos , int nGemType)
    //{
    //	CWritePacket packet = new CWritePacket(MessageID.Item_C2D_EmbedGemToItem);
    //	packet.WriteInt64( lCastleID );
    //	packet.WriteInt64( lItemID );	
    //	packet.WriteByte ( bPos ) ;
    //	packet.WriteInt  ( nGemType ) ;
    //	SendPacket( packet );
    //}		
    //public void SendRemoveGemFromItem(long lCastleID, long lItemID , byte bPos )
    //{
    //	CWritePacket packet = new CWritePacket(MessageID.Item_C2D_RemoveGemFromItem);
    //	packet.WriteInt64( lCastleID );
    //	packet.WriteInt64( lItemID );	
    //	packet.WriteByte ( bPos ) ;
    //	SendPacket( packet );
    //}	
    //public void SendSynthToEquipment(long lCastleID, int nSynthID,ArrayList ItemIDList )
    //{
    //	CWritePacket packet = new CWritePacket(MessageID.Item_C2D_SynthToEquipmentItem);
    //	packet.WriteInt64( lCastleID );
    //	packet.WriteInt64( nSynthID );	
    //	packet.WriteByte((byte)ItemIDList.Count);
    //	foreach(long lItemID in ItemIDList)
    //	{
    //		packet.WriteInt64(lItemID);
    //	}
    //	SendPacket( packet );
    //}

    //public void SendSynthToGem(long lCastleID, int nSynthID , int nCount )
    //{
    //	CWritePacket packet = new CWritePacket(MessageID.Item_C2D_SynthToGemItem);
    //	packet.WriteInt64( lCastleID );
    //	packet.WriteInt( nSynthID );	
    //	packet.WriteInt ( nCount ) ;
    //	SendPacket( packet );
    //}

    //public void SendUpgradeGemFromEquipment(long lCastleID, long lItemID, int nSynthID , byte bHole )
    //{
    //	CWritePacket packet = new CWritePacket(MessageID.Item_C2D_UpgradeGemFromEquipment);
    //	packet.WriteInt64( lCastleID );
    //	packet.WriteInt64( lItemID );
    //	packet.WriteInt( nSynthID );	
    //	packet.WriteByte ( bHole ) ;
    //	SendPacket( packet );
    //}
}

