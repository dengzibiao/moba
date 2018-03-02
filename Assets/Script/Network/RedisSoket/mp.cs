using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;
using System.Text;
using System.IO;

public class ChanelMp : Dictionary<string, object> {
    private static ChanelMp instance;
    public static ChanelMp Instance()
    {
        if(instance == null)
        {
            instance = new ChanelMp();
        }
        return instance;
    }
    //添加method 方法
    void set_method(string method)
    {
        Dictionary<string, object> arg = (Dictionary<string, object>)this;

        if (arg.ContainsKey(method))
            arg["method"] = method;
        else arg.Add("method", method);

    }
    //获取method
    string get_method(string v = "default")
    {
        Dictionary<string, object> arg = (Dictionary<string, object>)this;
        return arg.ContainsKey("method") ? (string)arg["method"] : v;
    }
    //设置参数
    void set_param(string k, string v)
    {
		Dictionary<string, object> param = (Dictionary<string, object>)get_param();
        if (param.ContainsKey(k)) param[k] = v;
        else param.Add(k, v);
    }
    void set_param(object param)
    { 
        Dictionary<string, object> arg = (Dictionary<string, object>)this;
        if (arg.ContainsKey("param"))
            arg["param"] = param;
        else arg.Add("param", param);
    }
    //获取参数表
	Dictionary<string, object> get_param()
	{
		Dictionary<string, object> arg = (Dictionary<string, object>)this;
		if (!arg.ContainsKey("param"))
            arg.Add("param", new Dictionary<string, object>());
		return (Dictionary<string, object>)arg ["param"];
	}
    string get_param(string k, string v = "")
    {
        //Dictionary<string, object> arg = (Dictionary<string, object>)this;
		Dictionary<string, object> param = get_param();
        return param.ContainsKey(k) ? (string)param[k] : v;
    }
    //生成json字符串
    public string to_json()
    {
        Dictionary<string, object> arg = (Dictionary<string, object>)this;
        StringBuilder sb = new StringBuilder();
        JsonWriter jw = new JsonWriter(sb);
        //jw.PrettyPrint = true;
        jw.Write((Dictionary<string, object>)arg);
        return sb.ToString();
    }
    //从json串取
    public void from_json(string json_s)
    {
		//Dictionary<string, object> arg=(Dictionary<string, object>)this;
		//Dictionary<string, object> mp=JsonUtility.FromJson<Dictionary<string, object>>(json_s);

        JsonReader jr = new JsonReader(json_s);
        Dictionary<string, object> mp = (Dictionary<string, object>)jr.Deserialize();
        set_method((string)mp["method"]);
        set_param(mp["param"]);
    }

	/*
	public static string redis_cmd(string[] cmd_set)
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
	}*/

//测试
    public static void test()
    {
		JsonReader jr = new JsonReader(tools.file_get_contents("config.json"));
		Dictionary<string, object> config = (Dictionary<string, object>)jr.Deserialize();

        ChanelMp mp1 =  Instance();

        mp1.set_method("C2S_玩家登陆");
		mp1.set_param( "gid", "1_3");
		tools.file_put_contents ("hello.txt", tools.to_json (mp1, true));

		RedisSocket c1 = new RedisSocket ();
		RedisSocket c2 = new RedisSocket ();
		c1.cfg ((string)config["c1_host"], (int)config["c1_port"]);
		c2.cfg ((string)config["c2_host"], (int)config["c2_port"]);
		string cmdline="hello word 消息信息!";
		c2.on_data (cmdline);
		c2.On_cmd_set_callback = new On_cmd_set_callback(mp1.On_cmd_set_callback2);
		c1.On_cmd_set_callback = new On_cmd_set_callback(mp1.On_cmd_set_callback1);



		c1.send (new string[]{"set","a","a===1"});
		c1.send (new string[]{"set","b","b===2"});
		c1.send (new string[]{"set","c","c===3"});
		c1.send (new string[]{"keys","*"});
		c1.send (new string[]{"get","c"});
        //c1.send(new string[] { "subscribe", "h", "g", "b" });
        c1.send("psubscribe h g b" );


        //file_put_contents ("hello.txt", redis_cmd (cmd_set));
        Debug.Log ("--------");
	//	c2.game_call (mp1);

    }

	public void On_cmd_set_callback1(RedisSocket redis_socket,string[] cmd_set)
	{
		StringBuilder sb = new StringBuilder();
		JsonWriter jw = new JsonWriter(sb);
		jw.Write (cmd_set);
		Debug.Log ("On_cmd_set_callback1 cmd_set=" + sb.ToString ());
	}
	public void On_cmd_set_callback2(RedisSocket redis_socket,string[] cmd_set)
	{
		StringBuilder sb = new StringBuilder();
		JsonWriter jw = new JsonWriter(sb);
		jw.Write (cmd_set);
		Debug.Log ("On_cmd_set_callback2 cmd_set=" + sb.ToString ());
		for (int i = 0; i < cmd_set.Length; i++) {
			Debug.Log(string.Format("mp.On_cmd_set_callback2:{0}/{1} {2}[{3}]",i,cmd_set.Length,cmd_set[i].Length,cmd_set[i]));
		}
	}

}
