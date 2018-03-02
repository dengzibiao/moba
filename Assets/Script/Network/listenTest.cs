using System;
using System.Net;
using System.Threading;
using UnityEngine;

public class ClistenTest
{
	public ClistenTest ()
	{
		m_Singleton = this;
		m_byteDataList = new byte[DataDefine.MAX_PACKET_SIZE];
	}
	public static ClistenTest GetSingle(){
		if(m_Singleton==null)
			new ClistenTest();
		return m_Singleton;
	}

	private void Listen()
	{
	 try
	 {
			  bool done = false;
			  TcpListen = new System.Net.Sockets.TcpListener(6811);
			  Debug.Log( TcpListen );
			  TcpListen.Start();
			
				System.Net.Sockets.TcpClient client = TcpListen.AcceptTcpClient();
				
				Debug.Log( client );
				if ( client != null )
				{
					m_NetworkStream = client.GetStream();
					System.Net.Sockets.NetworkStream ns = m_NetworkStream;
				
					//listen Receive
					StartReceiveData();
				
				 	while(!done)
					{

						if ( ns != null )
						{
						continue;//pause one test/**/
						
							Debug.Log( ns );                                           
							//CWritePacket packet = new CWritePacket((int)D2CMessage.EBuilding_Update);
							//packet.WriteInt(345);
							//packet.WriteString("keyinishihuaihaizi");
							//packet.WriteShort(15);
							//packet.WriteByte(10);
							try
							{
								//ns.Write( packet.GetPacketByte(), 0, packet.GetPacketLen() );
								//ns.Close();
								//client.Close();
							}
							catch ( System.Exception ex )
							{
								Debug.Log("listen error!"+ex.Message);
							}
						}
					}
				}
	  }
		catch (System.Exception ex )
		{
			Debug.Log("listen error111!"+ex.Message);
		}
	}
	

	public void StartListen()
	{
		Debug.Log( "listen start" );
		thThreadRead = new Thread(new ThreadStart(Listen));
		Debug.Log( "listen start11" );
		thThreadRead.Start();
	}
	
	
	
	private void    StartReceiveData()
	{
		if ( m_NetworkStream!=null )
		{
			m_NetworkStream.BeginRead( m_byteDataList, 0, DataDefine.MAX_PACKET_SIZE,
						                ReceiveMessage, null );
			Debug.Log( "Listen:::StartReceiveData" );
		}
		else
			Debug.Log( "Listen:no ! StartReceiveData" );
	}
	
	//receive
	private void ReceiveMessage(System.IAsyncResult ar)
	{
		try
		{
			if ( m_NetworkStream!=null )
			{
				lock( m_NetworkStream )
				{
					int nRecLength =m_NetworkStream.EndRead(ar);
					if ( nRecLength <= 0 )
						return;
					else
					{
						Debug.Log( "c-dReceive message!!" );
						 
						Debug.Log(nRecLength  );
						
						//receive one message
						byte[] bReceiveData = new byte[nRecLength];
						Array.Copy( m_byteDataList, 0, bReceiveData, 0, nRecLength );
						Array.Clear( m_byteDataList, 0, DataDefine.MAX_PACKET_SIZE );
						
						Debug.Log( bReceiveData );
						//int nlll = BitConverter.ToInt32(bReceiveData,0);
						//Debug.Log( "Receive message number=" + nlll );
						//String str = BitConverter.ToString(bReceiveData);
						//Debug.Log( "Receive string:" + str);
						
						
						//handle
						CReadPacket packet = new CReadPacket( bReceiveData );
						//Debug.Log( "Receive data packet length" + 
						//	          nRecLength+"/" + packet.GetPacketLen());
						
						//int nValue = packet.ReadInt();
						//String str = packet.ReadString();
						//short short11 = packet.ReadShort();
						//byte bTT = packet.ReadByte();
						//long lV = packet.ReadInt64();
						//Debug.Log( "c-d:read para:::"+ nValue+"str:"+ str );
						//Debug.Log("c-d:short11::"+short11 + "byte:"+ bTT + "long:"+lV);
					}
					
					if ( m_NetworkStream!= null )
					{
						m_NetworkStream.BeginRead( m_byteDataList, 0, DataDefine.MAX_PACKET_SIZE,
						                          new System.AsyncCallback(ReceiveMessage), m_NetworkStream );
					}
				}
			}
		}
		catch ( System.Exception ex )
		{
			Debug.Log( ex.Message );
		}
	}
	
	private Thread thThreadRead;
	private System.Net.Sockets.TcpListener TcpListen;
	
	private static ClistenTest				m_Singleton;
	private System.Net.Sockets.NetworkStream m_NetworkStream;
	private byte[]							m_byteDataList;
}


