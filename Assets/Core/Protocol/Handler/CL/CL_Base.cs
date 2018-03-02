using UnityEngine;
using System.IO;

public class CL_Base
{
    public ushort hc = 6;
    public SocketPackage socketPackage = (SocketPackage)SocketPackage.Instance;
    public void Write(byte u)
    {
        socketPackage.Write(u);
    }
    public void Write(sbyte u)
    {
        socketPackage.Write(u);
    }
    public void Write(ushort u)
    {
        socketPackage.Write(u);
    }
    public void Write(short u)
    {
        socketPackage.Write(u);
    }
    public void Write(uint u)
    {
        socketPackage.Write(u);
    }
    public void Write(int u)
    {
        socketPackage.Write(u);
    }
    public void Write(ulong u)
    {
        socketPackage.Write(u);
    }
    public void Write(long u)
    {
        socketPackage.Write(u);
    }
    public void Write(string u)
    {
        socketPackage.Write(u);
    }
    public void Write(byte[] u)
    {
        socketPackage.Write(u);
    }
    public void Write(Stream u)
    {
        socketPackage.Write(u);
    }
}

