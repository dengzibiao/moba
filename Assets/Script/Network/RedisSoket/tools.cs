using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;
using System.Text;
using System.IO;

public class tools
{
	public tools ()
	{
	}
	public static string to_json(object o,bool pretty=false)
	{
		StringBuilder sb = new StringBuilder();
		JsonWriter jw = new JsonWriter(sb);
		jw.PrettyPrint = pretty; 
		jw.Write(o);
		return sb.ToString();
	}
	public static object from_json(string json_s)
	{
		JsonReader jr = new JsonReader(json_s);
		return jr.Deserialize();
	}

	public static bool file_put_contents(string file,string contents)
	{
		try {
			FileStream fs = new FileStream(file , FileMode.Create, FileAccess.Write);         
			StreamWriter sw = new StreamWriter(fs);
			sw.Write(contents);
			sw.Flush();
			fs.Close();
			return true;
		} catch(IOException e)
		{
			Debug.Log(string.Format("file_put_contents({0},{1}):{2}", file,contents,e));
			return false;
		}
	}
    //获取配置文件内容
	public static string file_get_contents(string file)
	{
		try {
            // File.ReadAllText()
            FileStream fs = new FileStream(file , FileMode.Open, FileAccess.Read);         
            StreamReader sw = new StreamReader(fs);
            string text = sw.ReadToEnd(); // File.ReadAllText(GameDataSave.GetSavePath()+file);//sw.ReadToEnd();

            fs.Close();
            Debug.Log(text);
            return text;
		} catch(IOException e)
		{
			Debug.Log(string.Format("file_get_contents({0}): {1}", file,e));
			return "";
		}
	}


}