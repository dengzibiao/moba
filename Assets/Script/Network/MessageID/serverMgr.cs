using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Tianyu
{
    /// <summary>
    /// 服务器数据
    /// </summary>
    public class ServeData
    {
        public string name;//服务器名字
        public string ip;//ip
        public int port;//端口
        public byte state;//服务器状态"-1 关闭 0维护 1火爆开放，2 新服
        public string Desc;//服务器描述
        public int areaId;
        public uint playerId;
        public long heroId;
        public string playerName;
    }
    public class roleData
    {
        public string name;
        public int icon;
        public int heroid;

    }
    public delegate void funcEntry(object data);
    public class serverMgr
    {

        public UserData myData = new UserData();
        public GameDataSave gamedata;
        private static serverMgr instance;
        public string lastAreaId = "";
        public string lastPlayerId = "";
        public Dictionary<int, roleData> playerList;



        public static serverMgr GetInstance()
        {
            if (instance == null)
            {
                instance = new serverMgr();
            }
            return instance;
        }
        public float GetGameAfficheStart()
        {
            if (gamedata.myData != null)
            {
                return gamedata.myData._iUser.versionNum;
            }
            return 0f;
        }
        public void SetGameAfficheStart(float versionNum)
        {
            if (gamedata.myData != null)
            {
                gamedata.myData._iUser.versionNum = versionNum;
            }
        }
        /// <summary>
        /// 设置背景音乐
        /// </summary>
        /// <param name="isMute"></param>
        public void SetGameMusic(bool isMute)
        {
            if (gamedata.myData != null)
            {
                gamedata.myData._iUser.isPlay = isMute;
            }
        }
        /// <summary>
        /// 音效
        /// </summary>
        /// <returns></returns>
        public bool GetGameSoundEffect()
        {
            if (gamedata.myData != null)
                return gamedata.myData._iUser.isPlayEffectSound;
            return false;
        }
        public void SetGameSoundEffect(bool isMute)
        {
            if (gamedata.myData != null)
            {
                gamedata.myData._iUser.isPlayEffectSound = isMute;
            }
        }
        public bool GetGameMusic()
        {
            if (gamedata.myData != null)
                return gamedata.myData._iUser.isPlay;
            return false;
        }
        public void SetAccount(string account)
        {
            if (gamedata.myData != null)
            {
                gamedata.myData._iUser.account = account;
            }
        }
        public long GetMobile()
        {
            if (gamedata != null && gamedata.myData != null)
            {
                return gamedata.myData._iUser.mobile;
            }
            return 0L;
        }
        public void SetMobile(long value)
        {
            if (gamedata.myData != null)
            {
                gamedata.myData._iUser.mobile = value;
            }
        }
        public string GetAccount()
        {
            if (gamedata != null && gamedata.myData != null)
                return gamedata.myData._iUser.account;
            return "";
        }
        public void SetPassword(string password)
        {
            if (gamedata.myData != null)
            {
                gamedata.myData._iUser.password = password;
            }
        }
        public string GetPassword()
        {
            if (gamedata.myData != null)
                return gamedata.myData._iUser.password;
            return "";
        }
        public void SetServer(string server)
        {
            if (gamedata.myData != null)
            {
                gamedata.myData._iUser.server = server;
            }
        }
        public string GetServer()
        {
            if (gamedata.myData != null)
                return gamedata.myData._iUser.server;
            return "";
        }

        public void SetName(string name)
        {
            if (gamedata.myData != null)
            {
                gamedata.myData._iUser.name = name;
            }
        }

        public string GetName()
        {
            if (gamedata.myData != null)
                return gamedata.myData._iUser.name;
            return "";
        }

        public void SetCreateAccountTime(string timer, bool isCurrentTime)
        {
            if (gamedata.myData != null)
            {
                string cTime = "";
                if (isCurrentTime)
                {
                    DateTime data = TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime());
                    cTime = data.Year + (data.Month > 9 ? "" + data.Month : "0" + data.Month) + (data.Day > 9 ? "" + data.Day : "0" + data.Day) + data.TimeOfDay;
                }
                else
                {
                    cTime = timer.Replace("-", "");
                    cTime = cTime.Replace(" ", "");
                }
                cTime = cTime.Replace(":", "");
                gamedata.myData._iUser.createTime = cTime.Substring(2, cTime.Length - 2);
            }
        }

        public string GetCreateAccountTime()
        {
            if (gamedata.myData != null)
                return gamedata.myData._iUser.createTime;
            return "";
        }

        //public void SetPlayerId(long playerId)
        //{
        //    if ( gamedata != null )
        //    {
        //        gamedata.myData._iUser.playerId = playerId;
        //    }
        //}

        //public long GetPlayerId ()
        //{
        //    if ( gamedata != null )
        //        return gamedata.myData._iUser.playerId;
        //    return 0;
        //}

        public void SetCurrentPoint(int pointCount)
        {
            if (gamedata.myData != null)
            {
                gamedata.myData._iUser.currentPoint = pointCount;
            }
        }

        public int GetCurrentPoint()
        {
            if (gamedata.myData != null)
                return gamedata.myData._iUser.currentPoint;
            return 0;
        }

        public void SetFullTime(long fullTime)
        {
            if (gamedata.myData != null)
            {
                gamedata.myData._iUser.fullTime = fullTime;
            }
        }

        public long GetFullTime()
        {
            if (gamedata.myData != null)
                return gamedata.myData._iUser.fullTime;
            return 0;
        }


        public void saveData()
        {
            if (gamedata != null)
            {
                gamedata.Save(/*GetPlayerId(),*/GetServer(), GetAccount(), GetPassword(), GetGameAfficheStart(), GetName(), GetMobile(), GetCurrentPoint(), GetFullTime(), GetGameMusic(), GetGameSoundEffect());
                // gamedata.SaveData();
            }
        }

        public Dictionary<int, string> serverkeymap = new Dictionary<int, string>();//服务器键值索引   通过areaId找区名
        public List<ServeData> serverlist = new List<ServeData>();//服务器的字典 
        public ServeData GetServerDataByName(string name)//名字返回Item
        {
            foreach (ServeData item in serverlist)
            {
                if (item.name == name)
                    return item;
            }
            return null;
        }
        public string GetPlayNameDataByName(string name)//名字返回Item
        {
            foreach (ServeData item in serverlist)
            {
                if (item.name == name)
                    return item.playerName;
            }
            return null;
        }
        public static void setloginstate(int state)
        {
            serverMgr.instance.loading_count += state;
        }
        public static Dictionary<string, object> config = new Dictionary<string, object>();
        public static RedisSocket chat_server_send = new RedisSocket();
        public static RedisSocket chat_server_recv = new RedisSocket();
        public static int chat_server_enabled = 0;
        public static List<string[]> cmd_set_list = new List<string[]>();
        public int loading_count = 0;
        //发送函数
        public static void On_redis_cmd_set_callback1(RedisSocket redis_server, string[] cmd_set)
        {
            string msg = tools.to_json(cmd_set);
            Debug.Log("On_redis_cmd_set_callback1:" + msg);
        }
        public static void UpdateFromMainThread()
        {
            while (cmd_set_list.Count > 0)
            {
                string[] cmd_set = cmd_set_list[0];

                string msg = tools.to_json(cmd_set);
                Debug.Log("On_redis_cmd_set_callback2:" + msg);
                if (cmd_set[0] == "pmessage" && cmd_set.Length >= 4)
                {
                    serverMgr.redis_message(cmd_set[1], cmd_set[2], cmd_set[3]);
                }
                else if (cmd_set[0] == "message" && cmd_set.Length >= 3)
                {
                    serverMgr.redis_message(cmd_set[1], cmd_set[1], cmd_set[2]);
                }
                cmd_set_list.RemoveAt(0);
            }
        }
        //接收函数
        public static void On_redis_cmd_set_callback2(RedisSocket redis_server, string[] cmd_set)
        {
            cmd_set_list.Add(cmd_set);
        }
        static void func_nearlist(object xxx)
        {
            object[] lst = (object[])xxx;
            playerData.GetInstance().CreateNearList(lst);

        }
        static Dictionary<string, funcEntry> funcTab = new Dictionary<string, funcEntry>(){
                { "nearlist",serverMgr.func_nearlist }
        };

        static void redis_message(string channel, string channelName, string message)
        {
            Debug.Log(string.Format("redis_message:{0} \"{1}\"", channelName, message));
            string[] args = message.Split(' ');
            string firstChar = channelName.Substring(0, 1);

            if (firstChar == "r")
            {
                object data = tools.from_json(message);
                Dictionary<string, object> d = (Dictionary<string, object>)data;
                string method = (string)d["method"];
                object param = d["param"];
                if (funcTab.ContainsKey(method))
                {
                    funcTab[method](param);
                }
                else
                {
                    Debug.Log(string.Format("ERROR: invalid method={0} {1}", method, message));
                }
                /*
                 * 
                 {
                    "method":"nearlist",
                    "param": [
                        { "playerId":1234,"heroId":1235,"mapX",.......},
                        { "playerId":1234,"heroId":1235,"mapX",.......}
                    ],
                 }
                 void methodName(Dirctionray
                 * */
                string playerId = channelName.Substring(1);
                if (args[0] == "nearlist")
                {
                    int mapId = 1;
                    for (int i = 1; i < args.Length; i++)
                    {
                        playerOnline(args[i], mapId);
                    }
                }

            }
            else
            if (firstChar == "w")
            {
                // string playerId = channelName.Substring(1);

                string heroId = channelName.Substring(1);
                if (playerData.GetInstance().selfData.playerId == int.Parse(heroId))
                {
                    Debug.Log("is self! do not");
                }
                if (args[0] == "moveto" && args.Length >= 8)
                {//recvice redis-cli publish w{playerId} moveto mapId mapX mapY mapZ dirX dirY dirZ
                    float mapX, mapY, mapZ, dirX, dirY, dirZ;
                    int mapId = int.Parse(args[1]);
                    mapX = float.Parse(args[2]);
                    mapY = float.Parse(args[3]);
                    mapZ = float.Parse(args[4]);
                    dirX = float.Parse(args[5]);
                    dirY = float.Parse(args[6]);
                    dirZ = float.Parse(args[7]);
                    hero_moveto(heroId, mapId, mapX, mapY, mapZ, dirX, dirY, dirZ);
                }

            }
            if (channelName == "broadcast")
            {//recvice redis-cli publish broadcast online mapId playerId
                Debug.Log(tools.to_json(args));
                if (args.Length > 1 && args[0] == "online")
                {
                    int mapId = int.Parse(args[1]);
                    string playerId = args[2];
                    playerOnline(playerId, mapId);
                }

            }
        }
        static void hero_moveto(string heroId, int mapId, float mapX, float mapY, float mapZ, float dirX, float dirY, float dirZ)
        {
            //Roledata data =  playerData.GetInstance().nearList[int.Parse(heroId)];
            // if (data == null)
            //     return;
            //  GameObject obj = data.model;
            //if(obj)
            {
                OtherPlayer other = GameObject.FindObjectOfType<OtherPlayer>();// (OtherPlayer)(CharacterManager.instance.transform.GetComponentsInChildren<OtherPlayer>()[0]);//obj.GetComponent<PlayerMotion>();
                if (other)
                {
                    other.GetComponent<PlayerMotion>().Move(new Vector3(mapX, mapY, mapZ));
                    other.GetComponent<PlayerMotion>().Rotation(new Vector3(dirX, dirY, dirZ));
                }
            }
            // data.model.
            Debug.Log(string.Format("hero_moveto:heroId={0},mapId={1},mapX={2},mapY={3},mapZ={4},dirX={5},dirY={6},dirZ={7}", heroId, mapId, mapX, mapY, mapZ, dirX, dirY, dirZ));
        }
        static void playerOnline(string playerId, int mapId)
        {
            Debug.Log(string.Format("redis-cli subscribe w{0}", playerId));
            //if(playerId==Globe.playerId) return;
            //if(mapId!=Globe.mapId)
            Debug.Log(string.Format("playerOnline:playerId={0},mapId={1}", playerId, mapId));
            chat_server_recv.send(string.Format("subscribe w{0}", playerId));//redis-cli subscribe w{playerId}
        }

    }



}
