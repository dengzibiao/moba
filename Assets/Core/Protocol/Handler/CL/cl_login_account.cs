using System.Collections;
using System.IO;

public class CL_LoginAccountReq : ISerial
{
    public string user;

    public CL_LoginAccountReq()
    {
    }
    public override bool serial(BinaryWriter sscc_writer)
    {
        if(!base.serial(sscc_writer))
        {
            return false;
        }
        do
        {
            if(user == null)
            {
                return false;
            }
            byte[] sscc_bytes = System.Text.Encoding.UTF8.GetBytes(user);
            do
            {
                uint sscc_size = (uint)sscc_bytes.Length;
                if(sscc_size <= (0xff >> 1))
                {
                    sscc_writer.Write((byte)(sscc_size << 1));
                    break;
                }
                if(sscc_size <= (0xffff >> 1))
                {
                    sscc_writer.Write((ushort)((sscc_size << 1) | 1));
                    break;
                }
                return false;
            }
            while(false);
            sscc_writer.Write(sscc_bytes);
        }
        while(false);
        return true;
    }
    public override bool unserial(BinaryReader sscc_reader)
    {
        if(!base.unserial(sscc_reader))
        {
            return false;
        }
        do
        {
            uint sscc_size;
            do
            {
                uint sscc_read_size_n = (uint)sscc_reader.ReadByte();
                if((sscc_read_size_n & 1) != 0)
                {
                    sscc_read_size_n |= (((uint)sscc_reader.ReadByte()) << 8);
                }
                sscc_size = (sscc_read_size_n >> 1);
            }
            while(false);
            byte[] sscc_bytes = sscc_reader.ReadBytes((int)sscc_size);
            user = System.Text.Encoding.UTF8.GetString(sscc_bytes);
        }
        while(false);
        return true;
    }
};
public class CL_LoginAccountRsp : IResponse
{
    public uint uid;
    public ulong key;
    public string host;
    public ushort port;

    public CL_LoginAccountRsp()
    {
    }
    public override bool serial(BinaryWriter sscc_writer)
    {
        if(!base.serial(sscc_writer))
        {
            return false;
        }
        sscc_writer.Write((uint)uid);
        sscc_writer.Write((ulong)key);
        do
        {
            if(host == null)
            {
                return false;
            }
            byte[] sscc_bytes = System.Text.Encoding.UTF8.GetBytes(host);
            do
            {
                uint sscc_size = (uint)sscc_bytes.Length;
                if(sscc_size <= (0xff >> 1))
                {
                    sscc_writer.Write((byte)(sscc_size << 1));
                    break;
                }
                if(sscc_size <= (0xffff >> 1))
                {
                    sscc_writer.Write((ushort)((sscc_size << 1) | 1));
                    break;
                }
                return false;
            }
            while(false);
            sscc_writer.Write(sscc_bytes);
        }
        while(false);
        sscc_writer.Write((ushort)port);
        return true;
    }
    public override bool unserial(BinaryReader sscc_reader)
    {
        if(!base.unserial(sscc_reader))
        {
            return false;
        }
        uid = (uint)sscc_reader.ReadUInt32();
        key = (ulong)sscc_reader.ReadUInt64();
        do
        {
            uint sscc_size;
            do
            {
                uint sscc_read_size_n = (uint)sscc_reader.ReadByte();
                if((sscc_read_size_n & 1) != 0)
                {
                    sscc_read_size_n |= (((uint)sscc_reader.ReadByte()) << 8);
                }
                sscc_size = (sscc_read_size_n >> 1);
            }
            while(false);
            byte[] sscc_bytes = sscc_reader.ReadBytes((int)sscc_size);
            host = System.Text.Encoding.UTF8.GetString(sscc_bytes);
        }
        while(false);
        port = (ushort)sscc_reader.ReadUInt16();
        return true;
    }
};
public class CL_LoginAccount : CL_Base
{
    public const uint id = 1;
    public const string message_name = "CL_LoginAccount";
    public CL_LoginAccountReq req;
    public CL_LoginAccountRsp rsp;
    public CL_LoginAccount()
    {
        req = new CL_LoginAccountReq();
        rsp = new CL_LoginAccountRsp();
        HandlerManager.Instance.InQueue(id, this);
    }

    public void InSocketDate(string user)
    {
        //填需要传入的数据
        this.req.user = user;

        Push();
    }

    private void Push()
    {
        BinaryWriter bw = new BinaryWriter(new MemoryStream());
        req.serial(bw);
        uint size = (uint)bw.BaseStream.Length;

        Write((ushort)(size + 6));
        Write((uint)id);
        Write(bw.BaseStream);
        socketPackage.Flush();

        bw.BaseStream.Dispose();
        bw.BaseStream.Close();
        bw.Close();
    }
}
