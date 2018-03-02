using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using Tianyu;

//约束条件  
public class TestData
{
    //public long playerId;
    public String server;
    public int state;
    public String account;
    public String password;
    public String name;
    public long mobile;
    public bool isPlay;//背景音乐开关
    public bool isPlayEffectSound;//音效开关
    public int currentPoint;
    public long fullTime;
    public float versionNum;
    public string createTime = "";
}

//数组约束  
public class UserData
{
    public TestData _iUser = new TestData();
    //function UserData() { }  
}

public class GameDataSave
{
    private String _FileLocation;
    private String _FileName = "Data.xml";
    private String _data;
    public UserData myData;
    public UserData tempData = new UserData();
    public bool State;


    private int i = 0;

    public void startXml()
    {
        _FileLocation = GetSavePath();  //文件位置
                                        //_FileLocation = Application.dataPath;
                                        //  Debug.Log(_FileLocation);

        FirstSave();

    }



    //实现一个 TextWriter，使其以一种特定的编码向流中写入字符。   
    StreamWriter streamWriter;

    public void FirstSave()
    {//初始化XML  

        //tempData._iUser.playerId = 0;
        tempData._iUser.server = "";
        tempData._iUser.versionNum = 0f;
        tempData._iUser.account = "";
        tempData._iUser.password = "";
        tempData._iUser.name = "";
        tempData._iUser.isPlay = false;
        tempData._iUser.isPlayEffectSound = false;
        //tempData._iUser.state = 9;
        tempData._iUser.mobile = 0;
        tempData._iUser.currentPoint = 0;
        tempData._iUser.fullTime = 0;
        //FileInfo 方法在您创建或打开文件时返回其他 I/O 类型  
        FileInfo fileInfo = new FileInfo(_FileLocation + _FileName);
        //    print(fileInfo.Exists);
        if (!fileInfo.Exists)
        {

            streamWriter = fileInfo.CreateText();

            //XML化  


            _data = SerializeObject(tempData);
            for (i = 0; i < 1; i++)
            {
                streamWriter.WriteLine(_data);
            }

            streamWriter.Close();
        }
        else
        {

        }
    }

    public static string GetSavePath()
    {
        string strTablePath = string.Empty;
#if (UNITY_EDITOR || UNITY_STANDALONE)
        strTablePath = Application.dataPath + "/";
#elif UNITY_ANDROID
        strTablePath = Application.persistentDataPath + "/";
#elif UNITY_IPHONE
        strTablePath = Application.dataPath.Substring( 0, Application.dataPath.Length - 5 );
        strTablePath = strTablePath.Substring(0,strTablePath.LastIndexOf('/')) + "/Documents/";
#endif
        return strTablePath;
    }

    public void Save(/*long playerId,*/String server, String account, String password, float versionNum, String name, long mobile, int pointCount, long fullTime,bool play,bool playEffectSound)
    {//保存数据到指定的XMl里  
        //tempData._iUser.playerId = playerId;
        tempData._iUser.server = server;
        tempData._iUser.account = account;
        tempData._iUser.password = password;
        tempData._iUser.versionNum = versionNum;
        tempData._iUser.name = name;
        tempData._iUser.isPlay = play;
        tempData._iUser.isPlayEffectSound = playEffectSound;
        //tempData._iUser.state = state;
        tempData._iUser.mobile = mobile;
        tempData._iUser.currentPoint = pointCount;
        tempData._iUser.fullTime = fullTime;


        FileInfo fileInfo = new FileInfo(_FileLocation + _FileName);
        StreamWriter streamWriter;
        //fileInfo.Delete();  
        streamWriter = fileInfo.CreateText();
        _data = SerializeObject(tempData);
        for (i = 0; i < 1; i++)
        {
            streamWriter.WriteLine(_data);
        }
        streamWriter.Close();
    }




    public void Load()
    {//读取保存在XML里的数据  
        StreamReader streamReader = File.OpenText(_FileLocation + "/" + _FileName);

        for (i = 0; i < 1; i++)
        {
            _data = streamReader.ReadLine();
            myData = (UserData)DeserializeObject(_data);
        }

        streamReader.Close();

    }



    public String UTF8ByteArrayToString(byte[] characters)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        String constructedString = encoding.GetString(characters);
        return (constructedString);
    }

    public byte[] StringToUTF8ByteArray(String pXmlString)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] byteArray = encoding.GetBytes(pXmlString);
        return byteArray;
    }

    // Here we serialize our UserData object of myData  
    public String SerializeObject(object pObject)
    {
        String XmlizedString = "";
        MemoryStream memoryStream = new MemoryStream();
        XmlSerializer xs = new XmlSerializer(typeof(UserData));
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        xs.Serialize(xmlTextWriter, pObject);
        memoryStream = (MemoryStream)xmlTextWriter.BaseStream; // (MemoryStream)  
        XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
        return XmlizedString;
    }

    // Here we deserialize it back into its original form  
    public object DeserializeObject(String pXmlizedString)
    {
        XmlSerializer xs = new XmlSerializer(typeof(UserData));
        MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
        return xs.Deserialize(memoryStream);
    }


}
