using System;
using UnityEngine;

public class CPacket
{
	public CPacket (ushort nID, short nLen)
	{
		m_nPacketID = nID;
		m_nPacketLen = nLen;
		
		m_nPos=0;
	}
	
	public  ushort			GetPacketID()
	{
		return m_nPacketID;
	}

    public string GetLogPacketID ()
    {
        return "|0x" + Convert.ToString(GetPacketID(), 16);
    }

    public  short			GetPacketLen()
	{
		return m_nPacketLen;
	}
	
	public byte[]			GetPacketByte()
	{
		return m_bDataList;
	}
	
	public	int			GetPos()		{return m_nPos; }
	//public	void`		SetPos(int nPos){m_nPos = nPos;}
	
	 
	//big to little endian
	protected short BigToLittleEndian(short nValue)
	{	
		 						 return 					(short)( ((((short)(nValue) & 0xff00) >> 8) | 
		             										(((short)(nValue) & 0x00ff) << 8)) );
	}
	
	//big to little endian
	protected int BigToLittleEndian(int nValue)
	{
			 					return						(int) ( ((((int)(nValue) & 0xff000000) >> 24) | 
	                                                         (((int)(nValue) & 0x00ff0000) >> 8) | 
	                                                          (((int)(nValue) & 0x0000ff00) << 8) | 
	                                                          (((int)(nValue) & 0x000000ff) << 24)) );
	}
	
	
	protected ushort		m_nPacketID;
	protected short		m_nPacketLen;
	protected byte[]		m_bDataList;
	protected int			m_nPos;
}

