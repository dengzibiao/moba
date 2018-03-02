using System;
using UnityEngine;
using Pathfinding.Serialization.JsonFx;
using System.Collections.Generic;

public class CReadPacket : CPacket
{
    public Dictionary<string, object> data;

    public CReadPacket (byte[] bData)
		: base(0, 0 )
	{
		if ( bData != null )
		{
			m_bDataList = bData;
            m_nPacketLen =(short)( ReadShort());
           // m_nPacketID = (ushort)ReadShort();
            string tempstr = ReadString();
            if(tempstr.Length!= m_nPacketLen )
            {
                int exit = 0;
                int npos = tempstr.IndexOf( '{' );
                if (npos==-1)
                {

                     Debug.Log(tempstr);
                    Debug.Log("json串不全");
                    m_nPacketID = 0;
                    return;

                }
                string subLeft = tempstr;
                string subRight = tempstr;
                while ( npos!=-1 )
                {
                    npos = subLeft.IndexOf( '{' );
                    subLeft = subLeft.Substring( npos + 1 );
                    if ( npos != -1 )
                    {
                        exit += 1;
                    }
                    npos = subRight.IndexOf( "}" );
                    subRight = subRight.Substring( npos + 1 );
                    if ( npos != -1 )
                    {
                        exit -= 1;
                    }
                }

                if (exit !=0)
                {
                   // Debug.LogError("json串不全");
                    m_nPacketID = 0;
                    return;
                }
            }

            if (tempstr == "" || tempstr == "\n" || tempstr == "\r\n" || tempstr == "\r")
            {
                Debug.LogError("消息为空");
                return ;
            }
            //JsonReader reader = new JsonReader(tempstr);
            object obj = Jsontext.ReadeData(tempstr);

            data = obj as Dictionary<string,object>;
            m_nPacketID = ushort.Parse(data["msgid"].ToString());
          //  m = uint.Parse(data["msgid"].ToString());

            // if(DataDefine.isLogMsgDetail && DataDefine.filterWalkMsg(mID))
            //  Debug.Log("Recieve msgDetail: " + tempstr);
        }
	}
	
	public UInt32 GetMessageID()
	{
		return (UInt32)GetPacketID();
	}

    public string GetLogMessageID ()
    {
        return "|0x" + Convert.ToString(GetPacketID(), 16);
    }

    //restart Read
    public	void		ReStartRead()	{m_nPos=0;}
    //json 转化成相应类型的值
	public int GetInt(string key)
    {
        int valu = -1;
        if (data.ContainsKey(key))
            valu =  int.Parse(data[key].ToString());
        else
        {
            Debug.Log(key + " 键值不存在。。。。。");
        }
        return valu;
                
    }
    public short GetShort(string key)
    {
        short valu = -1;
        if (data.ContainsKey(key))
            valu = short.Parse(data[key].ToString());
        else
        {
            Debug.Log(key + " 键值不存在。。。。。");
        }
        return valu;

    }
    public UInt32 GetUint32 ( string key )
    {
        UInt32 valu = 0;
        if ( data.ContainsKey( key ) )
            valu = UInt32.Parse( data [ key ].ToString() );
        else
        {
            Debug.Log( key + " 键值不存在。。。。。" );
        }
        return valu;

    }
    public float GetFloat(string key)
    {
        float valu = -1;
        if (data.ContainsKey(key))
            valu = float.Parse(data[key].ToString());
        else
        {
            Debug.Log(key + " 键值不存在。。。。。");
        }
        return valu;
    }
    public string GetString(string key)
    {
        string valu = "";
        if (data.ContainsKey(key))
            valu = data[key].ToString();
        else
        {
            Debug.Log(key + " 键值不存在。。。。。");
        }
        return valu;
    }
    public double GetDouble(string key)
    {
        double valu = -1;
        if (data.ContainsKey(key))
            valu = double.Parse(data[key].ToString());
        else
        {
            Debug.Log(key + " 键值不存在。。。。。");
        }
        return valu;

    }
    public long GetLong(string key)
    {
        long valu = -1;
        if (data.ContainsKey(key))
            valu = long.Parse(data[key].ToString());
        else
        {
            Debug.Log(key + " 键值不存在。。。。。");
        }
        return valu;
    }
    public byte ReadByte(string key)
    {
        byte valu = 255;
        if (data.ContainsKey(key))
            valu = byte.Parse(data[key].ToString());
        else
        {
            Debug.Log(key + " 键值不存在。。。。。");
        }
        return valu;
    }
    //read short
    public	short		ReadShort()
	{
		if ( m_nPos >=0  )
		{
			int nSize = sizeof(short);
            //if ((m_nPos + nSize) > GetPacketLen())
            //{
            //    Debug.Log("GetShort error!");
            //    return 0;
            //}
            return ReadShortUnlimite();
		}
		return 0;
	}
   
	
	//read int
	public	int			ReadInt()
	{
		if ( m_nPos >=0 && m_nPos < GetPacketLen() )
		{
			int nSize = sizeof(int);
			if ( (m_nPos+nSize) > GetPacketLen() )
			{
				Debug.Log( "GetInt error!" );
				return 0;
			}
			int nValue = BitConverter.ToInt32( m_bDataList, m_nPos );
			m_nPos += nSize;
			//if ( BitConverter.IsLittleEndian )
			return System.Net.IPAddress.NetworkToHostOrder(nValue);
		}
		return 0;
	}
    	
	//read string
	public String		ReadString()
	{
		if ( m_nPos >=0  )
		{
			short nBlobLen = (short)(m_nPacketLen-4);

            // nBlobLen -= 2;
            m_nPacketID = (ushort)ReadShort();
            byte[] bDataTemp = ReadBlob( nBlobLen);

            if ( nBlobLen > 0)
            {
                string tempstr = "";
                // tempstr = System.Text.Encoding.ASCII.GetString( bDataTemp , 0 , nBlobLen );
                //  tempstr = Decompress(tempstr, DataDefine.datakey);

                //char[] tempstr = System.Text.Encoding.ASCII.GetChars( System.Text.Encoding.ASCII.GetBytes("6ICB6buR5ZOm77yM6ZmI6Zu344CC") );

                //byte[] arrtemp =  Convert.FromBase64CharArray( tempstr , 0,tempstr.Length);
                //string content = System.Text.Encoding.UTF8.GetString( arrtemp );
                tempstr = Decompress( bDataTemp , nBlobLen , DataDefine.datakey );
                return tempstr;
              //  return Decompress(tempstr, DataDefine.datakey);
            }
		}
		return "";
	}
    // 解密
 public   string Decompress (byte[] pIn, int pInlength, string key)
        
    {
        if (DataDefine.isEFS == 1)
        {
            //  key = "bloodgod20160516";
            byte[] mBuffer = new byte[pInlength];
            byte[] pkey = System.Text.Encoding.ASCII.GetBytes(key);//System.Text.Encoding.UTF8.GetBytes(key);
            int len = pkey.Length;
            //  byte[] pIn = System.Text.Encoding.ASCII.GetBytes(strContent);  //System.Text.Encoding.UTF8.GetBytes(strContent);
            for (int i = 0; i < pInlength; i++)
            {
                mBuffer[i] = (byte)(((int)pIn[i]) ^ ((int)pkey[i % len]));
            }
            // Array.Copy(key, 0, pkey, 0, key.Length);
            return System.Text.Encoding.UTF8.GetString(mBuffer);
        }
        else
        {
             return System.Text.Encoding.UTF8.GetString( pIn );
            //string tempstr = "6ICB6buR5ZOm77yM6ZmI6Zu344CC";

           // return Convert.FromBase64CharArray(tempstr);
        }
    }

	//read blob
	public byte[]		ReadBlob( short nBlobLength )
	{
		//if ( m_nPos >=0  )
		//{
			//first read bloglength
			//short nBlobLenT = ReadShort();
			//if ( (m_nPos+nBlobLenT) > GetPacketLen() )
			//{
			//	Debug.Log( "Read Blob Very Large!" );
			//	nBlobLength=0;
			//	return null;
			//}
			
			//send read byte[]
			if (nBlobLength > 0 )
			{
				//nBlobLength = nBlobLenT;
				
				byte[] bList = new byte[nBlobLength];
				Array.Copy( m_bDataList, m_nPos, bList, 0, nBlobLength );
				                        
				m_nPos += nBlobLength;
				return bList;
			}
		//}
		//nBlobLength=0;
		return null;
	}
	
	
	//read short Unlimite
	private short		ReadShortUnlimite()
	{
		int nSize = sizeof(short);
		//Debug.Log("nPos:"+m_nPos);
		short nValue = BitConverter.ToInt16( m_bDataList, m_nPos );
		m_nPos += nSize;
		//Debug.Log("Short:"+nValue);
		return System.Net.IPAddress.NetworkToHostOrder(nValue);
	}
}

