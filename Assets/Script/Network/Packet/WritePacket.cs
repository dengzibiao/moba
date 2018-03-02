using System;
using UnityEngine;
using System.Net;
public class CWritePacket : CPacket
{
	public CWritePacket ( UInt32 mID )//short nMessageID)
		: base( (ushort)mID, 0 )
	{
		m_bDataList = new byte[DataDefine.MAX_PACKET_SIZE];
		
	
	}
	

	
	//Wite short
	public	bool			WriteShort(short nValue)
	{
		short nSize = sizeof(short);
		if ( (GetPacketLen()+nSize) > DataDefine.MAX_PACKET_SIZE )
		{
			Debug.Log( "WriteInt error!too Large!" );
			return false;
		}

       // nValue = System.Net.IPAddress.HostToNetworkOrder(nValue);
        nValue = System.Net.IPAddress.NetworkToHostOrder(nValue);
        byte[] bDataListT = BitConverter.GetBytes( nValue );
		Array.Copy( bDataListT, 0, m_bDataList, m_nPos, nSize );
		m_nPos += nSize;
		ChangeLength( nSize );
     //   ReWriteLength();
        return true;
	}
	
	//Wite int
	public	bool			WriteInt(int nValue)
	{
		short nSize = sizeof(int);
		if ( (GetPacketLen()+nSize) > DataDefine.MAX_PACKET_SIZE )
		{
			Debug.Log( "WriteInt error!too Large!" );
			return false;
		}

        //nValue = System.Net.IPAddress.HostToNetworkOrder(nValue);
        nValue = System.Net.IPAddress.NetworkToHostOrder(nValue);
		byte[] bDataListT = BitConverter.GetBytes( nValue );
		Array.Copy( bDataListT, 0, m_bDataList, m_nPos, nSize );
		//Debug.Log("writeint,Pos:::"+m_nPos);
		m_nPos += nSize;
		ChangeLength( nSize );
		return true;
	}
	
	//Wite int64

	
	//Write string
	public bool		WriteString(String strValue)
	{
		if ( strValue.Length <= 0 ) return false;
		
		//short nSize = sizeof(char);
		//short nAllSize = (short)((strValue.Length+2));
     //   WriteShort(nAllSize);
       // WriteShort((short)m_nPacketID);
		//int nPosTemp =0;
		byte[] bWriteString = System.Text.Encoding.UTF8.GetBytes(strValue);
        //Debug.Log( "size to jisuan:"+nAllSize+"len:"+bWriteString.Length);
		return WriteBlob( bWriteString, (short)bWriteString.Length);
	}
	
	//Write blob
	public bool		WriteBlob( byte[] bBlobList, short nBlobLength )
	{
		if ( nBlobLength <= 0 || bBlobList==null ) return false;
        // short tempshort =  IPAddress.HostToNetworkOrder(nBlobLength);
        //first add length
        int length = nBlobLength +4;
       
        WriteShort((short)length);
        WriteShort((short)m_nPacketID);

        //second add byte[]
        short nAllSize = (short)(nBlobLength*sizeof(byte));
		if ( (GetPacketLen()+nAllSize) > DataDefine.MAX_PACKET_SIZE )
		{
			Debug.Log( "WriteBlob error!too Large!" );
			return false;
		}
		
		Array.Copy(bBlobList, 0,m_bDataList, m_nPos, nAllSize );
       // string temp = System.Text.Encoding.UTF8.GetString(bBlobList);
		m_nPos += nAllSize;
		ChangeLength( nAllSize );
		return true;
	}
   
    public  string Compress(string strContent, string key)
    {
      //  key = "bloodgod20160516";
         byte[] mBuffer = new byte[strContent.Length];
        byte[] pkey = System.Text.Encoding.ASCII.GetBytes(key);//System.Text.Encoding.UTF8.GetBytes(key);
        int len = pkey.Length;
        byte[] pIn = System.Text.Encoding.ASCII.GetBytes(strContent);  //System.Text.Encoding.UTF8.GetBytes(strContent);
        for (int i = 0; i < strContent.Length; i++)
        {
            mBuffer[i] = (byte)(((int)pIn[i]) ^ ((int)pkey[i % len]));
        }
        // Array.Copy(key, 0, pkey, 0, key.Length);
        return System.Text.Encoding.ASCII.GetString(mBuffer); 
    }
    private void	ChangeLength( short nChange )
	{
		m_nPacketLen += nChange;
      //  ReWriteLength();

    }
	
	//ReWrite Length
	//private void	ReWriteLength()
	//{
	//	int nSize = sizeof(short);
	//	short nValue = System.Net.IPAddress.HostToNetworkOrder((short)m_nPacketLen);
	//	byte[] bDataListT = BitConverter.GetBytes( nValue );
	//	int nWriteStart = 0+nSize;		//id's size
	//	Array.Copy( bDataListT, 0, m_bDataList, nWriteStart, nSize );
	//}
}

