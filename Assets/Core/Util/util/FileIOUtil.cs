using UnityEngine;
using System.IO;

public class FileIOUtil
{

    /// <summary>
    /// 从文件读取 Stream
    /// </summary>
    public static Stream FileToStream(string fileName)
    {
        // 打开文件
        FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        // 读取文件的 byte[]
        byte[] bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, bytes.Length);
        fileStream.Close();
        // 把 byte[] 转换成 Stream
        Stream stream = new MemoryStream(bytes);
        return stream;
    }

    /// <summary>
    /// 将 Stream 写入文件
    /// </summary>
    public static void StreamToFile(Stream stream, string fileName)
    {
        // 把 Stream 转换成 byte[]
        byte[] bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);
        // 设置当前流的位置为流的开始
        stream.Seek(0, SeekOrigin.Begin);
        // 把 byte[] 写入文件
        FileStream fs = new FileStream(fileName, FileMode.Create);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(bytes);
        bw.Close();
        fs.Close();
    }

    /// 将 byte[] 转成 Stream
    public static Stream BytesToStream(byte[] bytes)
    {
        Stream stream = new MemoryStream(bytes);
        return stream;
    }

    /// 将 Stream 转成 byte[]
    public static byte[] StreamToBytes(Stream stream)
    {
        byte[] bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);
        // 设置当前流的位置为流的开始
        stream.Seek(0, SeekOrigin.Begin);
        return bytes;
    }

    /// 将 String 转成 byte[]
    public static byte[] StringToBytes(string fileName, string input)
    {
        System.Text.UnicodeEncoding converter = new System.Text.UnicodeEncoding();
        byte[] inputBytes = converter.GetBytes(input);
        string inputString = converter.GetString(inputBytes);

        return inputBytes;
    }


    /// 将 String 转成 byte[]
    public static string BytesToString(string fileName, byte[] input)
    {
        System.Text.UnicodeEncoding converter = new System.Text.UnicodeEncoding();
        string inputString = converter.GetString(input);

        return inputString;
    }

    /// 二进制转换成图片
    public void ByteToImage(byte[] bytes)
    {
        //MemoryStream ms = new MemoryStream(bytes);
        //ms.Position = 0;
        //Image img = Image.FromStream(ms);
        //ms.Close();
    }


}
