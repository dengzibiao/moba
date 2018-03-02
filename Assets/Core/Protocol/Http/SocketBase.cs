using UnityEngine;
using System.Collections;
using System.IO;

public class SocketBase : Singleton<SocketBase>
{
    public const string IP = "192.168.3.10";
    public const int Port = 2000;
    public const int connectTimeOut = 5000;//连接超时
    public const int receiveTimeOut = 5000;//接受消息超时

    private BinaryWriter bw = new BinaryWriter(new MemoryStream());

    public void Write(byte u)
    {
        bw.Write(u);
    }

    public void Write(sbyte u)
    {
        bw.Write(u);
    }

    public void Write(ushort u)
    {
        bw.Write(u);
    }

    public void Write(short u)
    {
        bw.Write(u);
    }

    public void Write(uint u)
    {
        bw.Write(u);
    }

    public void Write(int u)
    {
        bw.Write(u);
    }

    public void Write(ulong u)
    {
        bw.Write(u);
    }

    public void Write(long u)
    {
        bw.Write(u);
    }
    public void Write(string u)
    {
        bw.Write(u);
    }

    public void Write(byte[] u)
    {
        bw.Write(u);
    }

    public void Write(Stream u)
    {
        byte[] bytes = new byte[u.Length];
        bw.Write(u.Read(bytes, 0, (int)u.Length));
    }

    public Stream GetStream()
    {
        return bw.BaseStream;
    }

    public void Clear()
    {
        bw.BaseStream.Dispose();
        bw.BaseStream.Close();
        bw.Close();
        //bw = new BinaryWriter(new MemoryStream());
    }
}
