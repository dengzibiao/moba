//  <author>Alexander Murauer</author>
//  <email>alexander.murauer@kolmich.at</email>
//  <date>2010-05-28</date>
//  <summary>short summary</summary>

using System.Collections.Generic;

public class KGFMessageList
{
	public abstract class Message
	{
		string itsMessage;
		
		public Message(string theMessage)
		{
			itsMessage = theMessage;
		}
		
		public string Description
		{
			get
			{
				return itsMessage;
			}
		}
	}
	
	public bool itsHasErrors = false;
	public bool itsHasWarnings = false;
	public bool itsHasInfos = false;
	
	public class Error : Message
	{
		public Error(string theMessage):base(theMessage){}
	}
	
	public class Info : Message
	{
		public Info(string theMessage):base(theMessage){}
	}
	
	public class Warning : Message
	{
		public Warning(string theMessage):base(theMessage){}
	}
	
	List<Message> itsMessageList = new List<Message>();
	
	public void AddError(string theMessage)
	{
		itsHasErrors = true;
		itsMessageList.Add(new Error(theMessage));
	}
	
	public void AddInfo(string theMessage)
	{
		itsHasInfos = true;
		itsMessageList.Add(new Info(theMessage));
	}
	
	public void AddWarning(string theMessage)
	{
		itsHasWarnings = true;
		itsMessageList.Add(new Warning(theMessage));
	}
	
	public string[] GetErrorArray()
	{
		List<string> anErrorList = new List<string>();
		foreach (Message aMessage in itsMessageList)
		{
			if (aMessage is Error)
				anErrorList.Add(aMessage.Description);
		}
		return anErrorList.ToArray();
	}
	
	public string[] GetInfoArray()
	{
		List<string> anErrorList = new List<string>();
		foreach (Message aMessage in itsMessageList)
		{
			if (aMessage is Info)
				anErrorList.Add(aMessage.Description);
		}
		return anErrorList.ToArray();
	}
	
	public string[] GetWarningArray()
	{
		List<string> anErrorList = new List<string>();
		foreach (Message aMessage in itsMessageList)
		{
			if (aMessage is Warning)
				anErrorList.Add(aMessage.Description);
		}
		return anErrorList.ToArray();
	}
	
	public Message[] GetAllMessagesArray()
	{
		return itsMessageList.ToArray();
	}
}
