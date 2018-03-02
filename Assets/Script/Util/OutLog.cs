using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class OutLog : MonoBehaviour 
{
	static List<string> mLines = new List<string>();
	string outpath;
	static OutLog m_singleton;
	public static OutLog GetSingleton()
	{
		return m_singleton;
	}
	void Start () {
	//	string str = SystemInfo.deviceUniqueIdentifier;
		if (m_singleton != null) {
			Destroy (m_singleton.gameObject);
		}
		m_singleton = this;
		DontDestroyOnLoad(gameObject);
		outpath = Application.persistentDataPath + "/outLog.txt";
		if (System.IO.File.Exists (outpath)) {
			File.Delete (outpath);
		}
		Application.logMessageReceived += LogToGUI;
		Application.logMessageReceivedThreaded += WriteLogToFile;
        LogErr("Test log");
    }

	void WriteLogToFile(string logString, string stackTrace, LogType type)
	{
		using(StreamWriter writer = new StreamWriter(outpath, true, Encoding.UTF8)) {
			if (type == LogType.Error
           || type == LogType.Exception) {
				writer.WriteLine(PlatformConfig.wordwrap + logString + PlatformConfig.wordwrap + stackTrace.Replace("\n", PlatformConfig.wordwrap));
			} else {
				writer.WriteLine(logString);
			}
		}
	}

	void LogToGUI(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Exception ||type == LogType.Log) 
		{
			LogErr(logString);
			LogErr(stackTrace);
		}
	}

	public static void LogErr (string str)
	{
		if (Application.isPlaying) {
			if (mLines.Count > 20) {
				mLines.RemoveAt(0);
			}
			mLines.Add(str);
		}
	}

	/*void OnGUI()
	{
		GUI.color = Color.red;
		for (int i = 0, imax = mLines.Count; i < imax; ++i) {
			GUILayout.Label(mLines[i]);
		}
       //GUI.Label(new Rect(0f, 0f, 900f, 30f), Application.persistentDataPath);
	}*/
}