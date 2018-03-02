using System.IO;
using System.Collections.Generic;
using System.Text;
using Pathfinding.Serialization.JsonFx;
using System;
using UnityEngine;

namespace Tianyu
{
    //文件的基类
    public abstract class FSDataNodeBase
    {
        public abstract void parseJson ( object jd );
    }
    //
    public class FSDataNodeTable<T> where T : FSDataNodeBase, new()
    {
        private string _data;
        public T tempData = new T();
        public Dictionary<long, T> DataNodeList { get { return mDataNodeList; } }
        
        //数据列表
        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="item">T类</param>
        /// <param name="typeid"> typeid</param>
        public void AddItem ( T item, int typeid )
        {
            if(DataNodeList == null)
                mDataNodeList = new Dictionary<long, T>();
            if(item != null)
            {
                mDataNodeList.Add(typeid, item);
                //  int n = 1;
            }
        }

        /// <summary>
        /// 根据typeid得到节点
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public T FindDataByType ( long typeId )
        {
            T retValue = default(T);
            if(mDataNodeList.TryGetValue(typeId, out retValue))
                return retValue;
            return null;
        }

        public void SaveJson ( string jsonString )
        {
            // string filePath = Application.dataPath + @"/myjson";
            FileInfo t = new FileInfo(jsonString);
            if(t.Exists)
            {
                File.Delete(jsonString);
                //t.Delete();
            }
            StreamWriter sw = t.CreateText();
            StringBuilder sb = new StringBuilder();
            JsonWriter writer = new JsonWriter(sb);

            // writer.WriteObjectStart();
            //foreach(T item in mDataNodeList.Values)
            //{
            //    string str = LitJson.JsonMapper.ToJson(item);
            //    sw.WriteLine(str);

            //}
            sw.Close();
            sw.Dispose();

        }
        public void LoadJson ( string jsonString )
        {
            Dictionary<string, object> json = (Dictionary<string, object>)Jsontext.ReadeData(jsonString);

            foreach(string jsonnode in json.Keys)
            {
                //  Dictionary<string, object> nodelist = (Dictionary<string, object>)json[jsonnode];
                long typeId = long.Parse(jsonnode);
                if(mDataNodeList == null)
                {
                    mDataNodeList = new Dictionary<long, T>();
                }
                if(!mDataNodeList.ContainsKey(typeId))
                {
                    T dataNode = new T();
                    //  LitJson.JsonMapper.ToObject<T>(strLine,false);
                    dataNode.parseJson(json[jsonnode]);
                    mDataNodeList.Add(typeId, dataNode);
                }
            }

        }
        public void LoadJsonArr(string jsonString)
        {
           object[] json = Jsontext.ReadeData(jsonString) as object[];

            for (int i = 0;i<json.Length;i++)
            {
                  Dictionary<string, object> node = (Dictionary<string, object>)json[i];
                long typeId = long.Parse(node["id"].ToString());
                if (mDataNodeList == null)
                {
                    mDataNodeList = new Dictionary<long, T>();
                }
                if (!mDataNodeList.ContainsKey(typeId))
                {
                    T dataNode = new T();
                    //  LitJson.JsonMapper.ToObject<T>(strLine,false);
                    dataNode.parseJson(node);
                    mDataNodeList.Add(typeId, dataNode);
                }
            }

        }

        public static FSDataNodeTable<T> GetSingleton ()
        {
            if(mSingleton == null)
                mSingleton = new FSDataNodeTable<T>();

            return mSingleton;
        }

        private Dictionary<String , Dictionary<long , T>> mDataNodelists;
        public Dictionary<String , Dictionary<long , T>> DataNodeLists { get { return mDataNodelists; } }

        public void LoadJsons ( string jsonString, string name )
        {
            Dictionary<string , object> json = ( Dictionary<string , object> ) Jsontext.ReadeData( jsonString );

            Dictionary<long , T> tempD = new Dictionary<long , T>();

            foreach ( string jsonnode in json.Keys )
            {
                //  Dictionary<string, object> nodelist = (Dictionary<string, object>)json[jsonnode];
                long typeId = long.Parse( jsonnode );
                if ( mDataNodelists == null )
                {
                    mDataNodelists = new Dictionary<String , Dictionary<long , T>>();
                }
                if ( !tempD.ContainsKey( typeId ) )
                {
                    T dataNode = new T();
                    //  LitJson.JsonMapper.ToObject<T>(strLine,false);
                    dataNode.parseJson( json [ jsonnode ] );
                    tempD.Add( typeId , dataNode );
                }
            }
            mDataNodelists.Add( name , tempD );
        }

        private Dictionary<long, T> mDataNodeList;
        private static FSDataNodeTable<T> mSingleton;


        public string UTF8ByteArrayToString ( byte[] characters )
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters);
            return ( constructedString );
        }

        public byte[] StringToUTF8ByteArray ( string pXmlString )
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        public long[] ParseToLongArray ( object config )
        {
            if(config is Array)
            {
                Array arr = (Array)config;
                long[] ret = new long[arr.Length];
                for(int i = 0; i < arr.Length; i++)
                {
                    ret[i] = long.Parse(arr.GetValue(i).ToString());
                }
                return ret;
            }
            //ebug.Log(config.ToString() + " is null");

            return null;
        }

        public float[] ParseToFloatArray( object config )
        {
            if(config is Array)
            {
                Array arr = (Array)config;
                float[] floatArr = new float[arr.Length];
                for(int i = 0; i< arr.Length; i++)
                {
                    floatArr[i] = float.Parse(arr.GetValue(i).ToString());
                }
                return floatArr;
            }
           //ebug.Log(config.ToString() + " is null");
           
            return null;
        }

        public double[] ParseToDoubleArray(object config)
        {
            if (config is Array)
            {
                Array arr = (Array)config;
                double[] floatArr = new double[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    floatArr[i] = double.Parse(arr.GetValue(i).ToString());
                }
                return floatArr;
            }
           //ebug.Log(config.ToString() + " is null");
            return null;
        }
    }
}

