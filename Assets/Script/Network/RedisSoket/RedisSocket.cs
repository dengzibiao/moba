using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;
using System.Text;
using System.Threading;

public class StateObject
{
    public StateObject(RedisSocket _redis_socket, Socket _socket)
    {
        redis_socket = _redis_socket;
        socket = _socket;
    }
    public RedisSocket redis_socket;
    public Socket socket = null;

    // Size of receive buffer.
    public const int BufferSize = 256;
    public byte[] buffer = new byte[BufferSize];
}


/**
		JsonReader jr = new JsonReader(file_get_contents("config.json"));
		Dictionary<string, object> config = (Dictionary<string, object>)jr.Deserialize();
		RedisSocket c1 = new RedisSocket ();
		RedisSocket c2 = new RedisSocket ();
		c1.cfg ((string)config["c1_host"], (int)config["c1_port"]);
		c2.cfg ((string)config["c2_host"], (int)config["c2_port"]);

		string[] cmd_set = new string[]{ "subscribe", "h","g","b" };
		c1.send (cmd_set);


		//file_put_contents ("hello.txt", redis_cmd (cmd_set));
		Debug.Log ("--------");
        mp mp1 = new mp();
        mp1.set_method("C2S_玩家登陆");
        mp1.set_param( "gid", "1_3");
		c2.game_call (mp1);


 */
public delegate void On_cmd_set_callback(RedisSocket redis_socket,string [] cmd_set);
public class RedisSocket
{
	public On_cmd_set_callback On_cmd_set_callback;

	public string ip="127.0.0.1";
	public int port=6379;

	public Socket sock=null;
	public System.Text.UTF8Encoding convert = new System.Text.UTF8Encoding ();

	public RedisSocket ()
	{
		On_cmd_set_callback=this.default_On_cmd_set_callback;
	}

	public void cfg(string _ip,int _port)
	{
		ip = _ip;
		port = _port;
		connect (ip, port);
	}
    //连接socket
	public Socket connect_socket(string ip,int port)
	{
		try
		{
			Debug.Log(string.Format("connect to {0}:{1} . . .",ip,port));
			Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			sock.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
			Debug.Log(string.Format("connect to {0}:{1} done.",ip,port));
			return sock;
		}
		catch (ArgumentNullException e)
		{
			Debug.Log(string.Format("ArgumentNullException: {0}", e));
			return null;
		}
		catch (SocketException e)
		{
			Debug.Log(string.Format("SocketException: {0}", e));
			return null;
		}

	}
	bool connect(string ip,int port)
	{
		if (sock != null)
			return true;
		sock = connect_socket (ip, port);
		if (sock != null) {
			init_data ();
			StateObject state = new StateObject(this,sock);
			sock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
		}

		return sock != null;
	}
	public static void ReadCallback(IAsyncResult ar)
	{
		StateObject state = (StateObject)ar.AsyncState;
		RedisSocket redis_socket = state.redis_socket;
		Socket sock = state.socket;
		int bytesRead = sock.EndReceive(ar);
		if (bytesRead > 0) {
			redis_socket.on_data (state.buffer, bytesRead);
			sock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
		}
	}

	MemoryStream line = new MemoryStream ();//单行字节流数据
	int ii=0,num = 1;// ii是读取的参数的个数，num是要读取的参数的数量
	int idx=0,len=0;//idx是当前读取的字节数，len是要读取的字节数
	List<string> cmd_set=new List<string>();//是string list
	private void init_data()
	{
		line.Seek (0, SeekOrigin.Begin);
		line.SetLength(0);
		ii = 0;num = 1;
		idx = 0;len = 0; 
		cmd_set.Clear ();
	}

	public void on_cmd_set(List<string> _cmd_set)
	{
		on_cmd_set (_cmd_set.ToArray ());
	}
	public void on_cmd_set(string [] cmd_set)
	{
		if (On_cmd_set_callback == null)
			return;
		On_cmd_set_callback (this, cmd_set);
		/*
		for (int i = 0; i < cmd_set.Length; i++) {
			Debug.Log(string.Format("on_cmd_set:{0}/{1} {2}[{3}]",i,cmd_set.Length,cmd_set[i].Length,cmd_set[i]));
		}*/
	}
    //默认回调函数
	public void default_On_cmd_set_callback(RedisSocket redis_socket,string[] cmd_set)
	{
		for (int i = 0; i < cmd_set.Length; i++) {
			Debug.Log(string.Format("default_On_cmd_set_callback:{0}/{1} {2}[{3}]",i,cmd_set.Length,cmd_set[i].Length,cmd_set[i]));
		}
	}
	private void on_a_cmd_set(MemoryStream line)
	{
		string text=System.Text.Encoding.UTF8.GetString(line.GetBuffer(),0,(int)line.Length);
		//Debug.Log (string.Format ("on_a_cmd_set: {0}/[{1}]", text.Length, text));
		line.Seek (0, SeekOrigin.Begin);
		line.SetLength (0);
		cmd_set.Add(text);
		idx = 0; len = 0;
		ii++;
		if (ii >= num) {
			on_cmd_set(cmd_set);
			init_data ();
		}
	}
	public void on_data(string cmdline)
	{
		init_data ();
		byte[] data = System.Text.Encoding.UTF8.GetBytes (cmdline+"\r\n");
		on_data (data,data.Length);
	}
	public void on_data(byte[] buf,int length)
	{
		for (int i = 0; i < length; i++) {
			if (idx < len) {
				line.WriteByte(buf [i]);
				idx++;
				if (idx >= len) {
					on_a_cmd_set(line);
				}
			} else {
				if (buf [i] == 13) continue;
				if (buf [i] != 10) {
					line.WriteByte(buf [i]); continue;
				}
				if (line.Length == 0) continue;
				byte[] bInput = line.GetBuffer ();
				string input = System.Text.Encoding.UTF8.GetString (bInput,0,(int)line.Length);
				char chr = (char)bInput [0];
				if (chr == '*') {
					ii = 0; num = int.Parse ((string)input.Replace ("*", ""));
					idx = 0; len = 0;
					cmd_set.Clear ();
					if(num<=0) {
						//if(i+1<length && buf[i+1]==13) i++;
						//if(i+1<length && buf[i+1]==10) i++;
					}
				} else if (chr == '$') {
					len = int.Parse (input.Replace ("$", ""));
					idx = 0;
					if(len<=0) {
						if(i+1<length && buf[i+1]==13) i++;
						if(i+1<length && buf[i+1]==10) i++;
                        line.Seek(0, SeekOrigin.Begin); line.SetLength(0); on_a_cmd_set(line);
					}
				} else if (chr == '+' || chr == '-' || chr == ':') {
					on_a_cmd_set(line);
				} else {
					on_cmd_set (input.Split (new char[]{' '}));
				}
				line.Seek (0, SeekOrigin.Begin); line.SetLength (0);
			}
		}
	}

	int do_send(string s)
	{
		if (sock == null)
			connect (ip, port);
		byte[] data = convert.GetBytes (s);
		if (sock != null) {
			try {
				Debug.Log (string.Format ("1.do_send: {0}", s));
				int size=sock.Send (data);
				return size;
			} catch(Exception e) {
				Debug.Log (string.Format ("1.do_send({0}) error({1}!", s,e.ToString()));
				sock.Close ();
				connect (ip, port);
				return 0;
			}
		} else {
			Debug.Log (string.Format ("0.do_send: {0}", s));
			return 0;
		}
	}
  
	public bool send(string[] cmd_set)
	{
		return do_send(redis_cmd (cmd_set))!=0;
	}
	public bool send(string input)
	{
		string [] cmd_set=input.Split (new char[]{' '});
		return do_send (redis_cmd (cmd_set)) != 0;
	}

	public bool game_call(ChanelMp mp)
	{
		string[] cmd_set = new string[]{ "GAME_CALL", mp.to_json () };
		return do_send (redis_cmd (cmd_set))!=0;
	}


	static string redis_cmd(string[] cmd_set)
	{
		System.Text.UTF8Encoding c=new System.Text.UTF8Encoding();
		string data = string.Format ("*{0}\r\n", cmd_set.Length);
		for(int i=0;i<cmd_set.Length;i++)
		{
			byte[] d = c.GetBytes (cmd_set [i]);// System.Convert.FromBase64String(cmd_set[i]);
			data += string.Format ("${0}\r\n{1}\r\n", d.Length, cmd_set [i]);
		}
		data += "\r\n";
		return data;
	}

}
