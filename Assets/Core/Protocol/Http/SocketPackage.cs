using UnityEngine;
using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

public class SocketPackage : SocketBase
{
    private HandlerManager handlerManager = HandlerManager.Instance;
    private Socket socket;

    public void connect()
    {
        this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ipAddress = IPAddress.Parse(IP);
        IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, Port);
        IAsyncResult result = socket.BeginConnect(ipEndpoint, new AsyncCallback(connectCallback), socket);

        if(!result.AsyncWaitHandle.WaitOne(connectTimeOut, true))
        {
            //超时
            this.Closed();
        //    Debug.Log("connect Time Out");
        }
        else
        {
            //与socket建立连接成功，开启线程接受服务端数据。
            Thread thread = new Thread(ReceiveSocket);
            thread.IsBackground = true;
            thread.Start();
        }
    }

    private void connectCallback(IAsyncResult asyncConnect)
    {
        if(asyncConnect.IsCompleted)
        {
          //  Debug.Log("connectSuccess");
        }
    }

    private void ReceiveSocket()
    {
        while(true)
        {

            if(!socket.Connected)
            {
                socket.Close();
                break;
            }

            if(socket.Available > 0)
            {
                try
                {
                    //设立缓冲区，最大容量4096字节
                    byte[] bytes = new byte[4096];

                    socket.ReceiveTimeout = receiveTimeOut;
                    if(socket.Receive(bytes, SocketFlags.None) <= 0)
                    {
                        socket.Close();
                        break;
                    }
                    else
                    {
                        VerifySocket(bytes);
                    }

                }
                catch(SocketException e)
                {
                   // Debug.Log(e.Message + ":" + e.ErrorCode);
                    socket.Close();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 校验数据
    /// </summary>
    private void VerifySocket(byte[] bytes)
    {
        if(bytes.Length < 1)
        {
          //  Debug.Log("length is  <  1");
            return;
        }
        else
        {
            //*3
            //$7
            //message
            //$6
            //chat_1
            //$24
            //我靠这回终于全了

            StreamReader read = new StreamReader(new MemoryStream(bytes));
            string data = "";

            while(true)
            {
                try
                {
                    string s = read.ReadLine();
                    if(s.IndexOf("*") != -1)
                    {
                        //数据开始
                    }
                    else if(s.IndexOf("$") != -1)
                    {
                        string len = s.Split('$')[1];
                        string line = read.ReadLine();
                        //国家标准GB2312： 一个汉字＝2个字节
                        //UTF－8：一个汉字＝3个字节
                        if(int.Parse(len) == StringUtil.GetStringLength(line))
                        {
                            data += " " + line;
                        }
                    }
                }
                catch(Exception)
                {
                    break;
                }

                data.Trim();
            }
        }
    }
    /// <summary>
    /// 校验数据
    /// </summary>
    //private void VerifySocket(byte[] bytes)
    //{
    //    if(bytes.Length < 6)
    //    {
    //        Debug.Log("length is  <  6");
    //        return;
    //    }
    //    else
    //    {
    //        BinaryReader reader = new BinaryReader(new MemoryStream(bytes));

    //        ushort size = reader.ReadUInt16();
    //        uint id = reader.ReadUInt32();

    //        int count = size - 6;
    //        if(bytes.Length < count)
    //        {
    //            Debug.Log("length is  <  " + count);
    //            return;
    //        }

    //        handlerManager.ReceiveSocket(id, bytes);
    //    }
    //}

    private void SplitPackage(byte[] bytes, int index)
    {

        while(true)
        {
            //包头是2个字节
            byte[] head = new byte[2];
            Array.Copy(bytes, index, head, 0, 2);
            //计算包体的长度
            short length = BitConverter.ToInt16(head, 0);
            if(length > 0)
            {
                byte[] data = new byte[length];
                Array.Copy(bytes, index + 2, data, 0, length);
                this.BytesToObject(data);
                index += length;
            }
            else
            {
                break;
            }
        }
    }

// 发送字符串
    public void Flush(string message)
    {
        byte[] msg = Encoding.UTF8.GetBytes(message);

        if(!socket.Connected)
        {
            socket.Close();
            return;
        }

        try
        {
            IAsyncResult asyncSend = socket.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(sendCallback), socket);
            if(!asyncSend.AsyncWaitHandle.WaitOne(connectTimeOut, true))
            {
                socket.Close();
               // Debug.Log("Failed to SendMessage server.");
            }
        }
        catch(SocketException e)
        {
          //  Debug.Log(e.Message + ":" + e.ErrorCode);
            socket.Close();
        }
    }
// 发送数据包
    public void Flush(object obj)
    {

        if(!socket.Connected)
        {
            socket.Close();
            return;
        }

        try
        {

            //先得到数据包的长度
            short size = (short)Marshal.SizeOf(obj);
            byte[] head = BitConverter.GetBytes(size);

            //把结构体对象转换成数据包，也就是字节数组
            byte[] data = this.ObjectToBytes(obj);

            byte[] newByte = new byte[head.Length + data.Length];
            Array.Copy(head, 0, newByte, 0, head.Length);
            Array.Copy(data, 0, newByte, head.Length, data.Length);

            //向服务端异步发送这个字节数组
            IAsyncResult asyncSend = socket.BeginSend(newByte, 0, newByte.Length, SocketFlags.None, new AsyncCallback(sendCallback), socket);
            if(!asyncSend.AsyncWaitHandle.WaitOne(connectTimeOut, true))
            {
                socket.Close();
              //  Debug.Log("Time Out !");
            }

        }
        catch(SocketException e)
        {
            //Debug.Log(e.Message + ":" + e.ErrorCode);
            socket.Close();
        }
    }

    public void Flush()
    {
        if(!socket.Connected)
        {
            socket.Close();
            return;
        }

        try
        {
            MemoryStream ms = GetStream() as MemoryStream;
            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, (int)ms.Length);

            //向服务端异步发送这个字节数组
            IAsyncResult asyncSend = socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(sendCallback), socket);
            if(!asyncSend.AsyncWaitHandle.WaitOne(connectTimeOut, true))
            {
                socket.Close();
             //   Debug.Log("Time Out !");
            }
        }
        catch(SocketException e)
        {
          //  Debug.Log(e.Message + ":" + e.ErrorCode);
            socket.Close();
            throw;
        }
    }


    public byte[] ObjectToBytes(object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);
        return ms.GetBuffer();
    }

    public object BytesToObject(byte[] bytes)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(bytes);
        return bf.Deserialize(ms);
    }

    private void sendCallback(IAsyncResult asyncSend)
    {
        Clear();
    }

//关闭Socket
    public void Closed()
    {

        if(socket != null && socket.Connected)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
        socket = null;
    }

}