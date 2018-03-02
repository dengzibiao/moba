using UnityEngine;
using System.Collections;

public class Auxiliary{
	//public static string ShiftTime(int NeedTime,int CreateTime){
	//    int SurpusTime = NeedTime-((int)(System.DateTime.Now.ToFileTime()/10000000)-CreateTime);
	//    return ShiftTime(SurpusTime);
		
	//}
	
	public static bool isKey(int nKey,int nMax)
	{
		if(nKey>=0 && nKey < nMax)
		{
			return true;
		}
		return false;
	}
	public static int Year(long TimeSecond)
	{
		System.DateTime dattime = new System.DateTime(TimeSecond);
	   return dattime.Year;
	}
	public static int Month(long TimeSecond)
	{
		System.DateTime dattime = new System.DateTime(TimeSecond);
		return dattime.Month;
	}
	public static int Day(long TimeSecond)
	{
		System.DateTime dattime = new System.DateTime(TimeSecond);
		return dattime.Day;
	}
	public static int Hour(long TimeSecond)
	{
		System.DateTime dattime = new System.DateTime(TimeSecond);
		return dattime.Hour;
	}
	//public static int ShiftTimeMonth
	public static int ShiftTimeDate(long TimeSecond)//获得天数
	{   
		if (TimeSecond >= 86400) //说明大于一天了
		{
			return System.Convert.ToInt16(TimeSecond / 86400);
		}
		else
		{
			return 0;
		}
	}
	public static int ShiftTimehour(long TimeSecond)//获得小时数
	{
		if (TimeSecond >= 86400||TimeSecond<=-86400) //说明大于一天了
		{
			return System.Convert.ToInt16((TimeSecond % 86400) / 3600);
		}
		else if (TimeSecond > 3600 || TimeSecond < -3600)
		{
			return System.Convert.ToInt16(TimeSecond / 3600);
		}
		else
		{
			return 0;
		}
	}
	public static int ShiftTimeMinute(long TimeSecond)//获得分钟数
	{
		if (TimeSecond >= 86400) //说明大于一天了
		{
			return System.Convert.ToInt16((TimeSecond % 86400 % 3600) / 60);
		}
		else if (TimeSecond > 3600)
		{
			return System.Convert.ToInt16((TimeSecond % 3600) / 60);
		}
		else if (TimeSecond >= 60)
		{
			return System.Convert.ToInt16(TimeSecond / 60);
		}
		else
		{
			return 0;
		}
	}
	public static int ShiftTimeSecond(long TimeSecond)//获得秒数
	{
		if (TimeSecond >= 86400) //说明大于一天了
		{
			return System.Convert.ToInt16(TimeSecond % 86400 % 3600 % 60);
		}
		
		else if (TimeSecond >= 60)
		{
			return System.Convert.ToInt16(TimeSecond % 60);
		}
		else
		{
			return (int)TimeSecond;
		}
	}
	public static int GetMiniteToTime(int nhour)
	{
		return System.DateTime.Now.Hour - nhour; //ShiftTimehour(TimeSecond);
	}
	public static void ShiftTime(long TimeSecond)
	{
		//string r = "";
		int day = 0, hour, minute, second;

		if (TimeSecond >= 86400) //说明大于一天了
		{
			day = System.Convert.ToInt16(TimeSecond / 86400);
			hour = System.Convert.ToInt16((TimeSecond % 86400) / 3600);
			minute = System.Convert.ToInt16((TimeSecond % 86400 % 3600) / 60);
			second = System.Convert.ToInt16(TimeSecond % 86400 % 3600 % 60);

		}
		else if (TimeSecond >= 3600)
		{
			day = 0;
			hour = System.Convert.ToInt16(TimeSecond / 3600);
			minute = System.Convert.ToInt16((TimeSecond % 3600) / 60);
			second = System.Convert.ToInt16(TimeSecond % 3600 % 60);
		}
		else if (TimeSecond >= 60)
		{
			day = 0;
			hour = 0;
			minute = System.Convert.ToInt16(TimeSecond / 60);
			second = System.Convert.ToInt16(TimeSecond % 60);
		}
		else if (TimeSecond >=0)
		{
			day = 0;
			hour = 0;
			minute = 0;
			second = System.Convert.ToInt16(TimeSecond);
		  //  r =  string.Format(StringTable.GetSingleton().FindString(DataDefine.SECOND), second);
		}
	   // return 0;
	}
	
	
	static public uint GetNowTime()
	{
	   // Debug.Log(ShiftTimeDate((long)(System.DateTime.UtcNow - new System.DateTime(2014, 9, 11)).TotalSeconds) + " tian");
	   // Debug.Log(ShiftTimehour((long)(System.DateTime.UtcNow - new System.DateTime(2014, 9, 12, 12, 0, 0)).TotalSeconds) + " xiaoshi");
	  //  Debug.Log(ShiftTimehour(TotalSeconds(System.DateTime.UtcNow - new System.DateTime(2014, 9, 12, 13, 0, 0)).TotalSeconds) + " xiaoshi");
		return (uint)((System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds + mDeltaTime);
	}
	
	public static string GetResStringValue(int nOldValue)
	{
		string strNowValue ;
		if(nOldValue >10000)
		{
			string strUintW = "";//StringTable.GetSingleton().FindString(DataDefine.UNIT_WAN);
			strNowValue = nOldValue/10000+strUintW; 
		}else
		{
			strNowValue = nOldValue+""; 
		}
		return strNowValue;
	}
	
	public static void SetServerTime(double serverTime)
	{
	   // Debug.Log(System.DateTime.UtcNow.ToString() + "xi tong shi jian =======");
	  //  Debug.Log((System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds.ToString());
		mDeltaTime = serverTime - (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds;
	}
	
	public static uint GetServerDeltaTime()
	{
		return (uint)mDeltaTime;
	}
	
	private static double mDeltaTime;
}
