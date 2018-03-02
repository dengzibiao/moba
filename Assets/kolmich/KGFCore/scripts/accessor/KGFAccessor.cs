// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2012-06-22</date>
// <summary>fast and easy accessor for all objects that are derrived from KGFObject</summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.IO;

/// <summary>
/// Easy accessor for objects of a given type.
/// Supports interfaces and live add/remove monitoring.
/// </summary>
public static class KGFAccessor
{
	private static Dictionary<Type,List<KGFObject>> itsListSorted = new Dictionary<Type, List<KGFObject>>();
	
	private static Dictionary<Type,KGFDelegate> itsListEventsAdd = new Dictionary<Type, KGFDelegate>();
	private static Dictionary<Type,KGFDelegate> itsListEventsAddOnce = new Dictionary<Type, KGFDelegate>();
	private static Dictionary<Type,KGFDelegate> itsListEventsRemove = new Dictionary<Type, KGFDelegate>();
	
	/// <summary>
	/// Event args class for accessor events
	/// </summary>
	public class KGFAccessorEventargs : System.EventArgs
	{
		KGFObject itsObject;
		
		public KGFAccessorEventargs(KGFObject theObject)
		{
			itsObject = theObject;
		}
		
		public KGFObject GetObject()
		{
			return itsObject;
		}
	}
	
	/// <summary>
	/// Add object to module_accessor
	/// </summary>
	/// <param name='theBaseScript'>
	/// All object_scripts will register using this method
	/// </param>
	public static void AddKGFObject(KGFObject theObjectScript)
	{
		// add to list of own type
		Type anObjectScriptType = theObjectScript.GetType();
		if (!itsListSorted.ContainsKey(anObjectScriptType))
			itsListSorted[anObjectScriptType] = new List<KGFObject>();
		itsListSorted[anObjectScriptType].Add(theObjectScript);
		
		// call register events
		foreach (Type aKeyType in itsListEventsAdd.Keys)
		{
			if (aKeyType.IsAssignableFrom(anObjectScriptType))
				itsListEventsAdd[aKeyType].Trigger(null,new KGFAccessorEventargs(theObjectScript));
		}
		
		if (itsListEventsAddOnce.Count > 0)
		{
			// call register events once
			List<Type> aTypeList = new List<Type>();
			foreach (Type aKeyType in itsListEventsAddOnce.Keys)
			{
				if (aKeyType.IsAssignableFrom(anObjectScriptType))
					aTypeList.Add(aKeyType);
			}
			foreach (Type aType in aTypeList)
			{
				itsListEventsAddOnce[aType].Trigger(null,new KGFAccessorEventargs(theObjectScript));
				itsListEventsAddOnce.Remove(aType);
			}
		}
	}
	
	/// <summary>
	/// Remove object from module_accessor
	/// </summary>
	/// <param name="theObjectScript"></param>
	public static void RemoveKGFObject(KGFObject theObjectScript)
	{
		// remove from list of own type
		Type anObjectScriptType = theObjectScript.GetType();
		try{
			itsListSorted[anObjectScriptType].Remove(theObjectScript);
		}
		catch
		{}
		
		// call registered events
		foreach (Type aKeyType in itsListEventsRemove.Keys)
		{
			if (aKeyType.IsAssignableFrom(anObjectScriptType))
				itsListEventsRemove[aKeyType].Trigger(null,new KGFAccessorEventargs(theObjectScript));
		}
	}
	
	/// <summary>
	/// Get module if it becomes available
	/// </summary>
	/// <param name="theCallback"></param>
	public static void GetExternal<T>(Action<object,EventArgs> theRegisterCallback) where T : KGFObject
	{
		T aT = GetObject<T>();
		if (aT != null)
			theRegisterCallback(aT,null);
		else
			RegisterAddOnceEvent<T>(theRegisterCallback);
	}
	
	/// <summary>
	/// Add delegate for callback if a object of the given type registers itsself
	/// </summary>
	/// <param name="theCallback"></param>
	public static void RegisterAddEvent<T>(Action<object,EventArgs> theCallback)
	{
		if (theCallback == null)
			return;
		Type theType = typeof(T);
		if (!itsListEventsAdd.ContainsKey(theType))
			itsListEventsAdd[theType] = new KGFDelegate();
		itsListEventsAdd[theType] += theCallback;
	}
	
	/// <summary>
	/// Add delegate for callback if a object of the given type registers itsself and unregisters it after the first found object
	/// </summary>
	/// <param name="theCallback"></param>
	public static void RegisterAddOnceEvent<T>(Action<object,EventArgs> theCallback)
	{
		if (theCallback == null)
			return;
		Type theType = typeof(T);
		if (!itsListEventsAddOnce.ContainsKey(theType))
			itsListEventsAddOnce[theType] = new KGFDelegate();
		itsListEventsAddOnce[theType] += theCallback;
	}
	
	/// <summary>
	/// Remove callback
	/// </summary>
	/// <param name="theCallback"></param>
	public static void UnregisterAddEvent<T>(Action<object,EventArgs> theCallback)
	{
		Type theType = typeof(T);
		if (itsListEventsAdd.ContainsKey(theType))
			itsListEventsAdd[theType] -= theCallback;
	}
	
	/// <summary>
	/// Add delegate for callback if a object of the given type unregisters itsself
	/// </summary>
	/// <param name="theCallback"></param>
	public static void RegisterRemoveEvent<T>(Action<object,EventArgs> theCallback)
	{
		if (theCallback == null)
			return;
		Type theType = typeof(T);
		if (!itsListEventsRemove.ContainsKey(theType))
			itsListEventsRemove[theType] = new KGFDelegate();
		itsListEventsRemove[theType] += theCallback;
	}
	
	/// <summary>
	/// Remove callback
	/// </summary>
	/// <param name="theCallback"></param>
	public static void UnregisterRemoveEvent<T>(Action<object,EventArgs> theCallback)
	{
		Type theType = typeof(T);
		if (itsListEventsRemove.ContainsKey(theType))
			itsListEventsRemove[theType] -= theCallback;
	}
	
	/// <summary>
	/// Get objects with specific type (include derived objects)
	/// </summary>
	/// <returns></returns>
	public static List<T> GetObjects<T>()
	{
		Type aWantedType = typeof(T);
		List<T> aList = new List<T>();	//TODO: unity doesnt delete lists correctly? this may cause some memory problems?
		foreach (Type aType in itsListSorted.Keys)
		{
			if (aWantedType.IsAssignableFrom(aType))
			{
				List<KGFObject> aListObjectScripts = itsListSorted[aType];
				
				for (int i=aListObjectScripts.Count-1;i>=0;i--)
				{
					object anObject = aListObjectScripts[i];
					MonoBehaviour aMonobehaviour = (MonoBehaviour) aListObjectScripts[i];
					if (aMonobehaviour == null)
					{
						aListObjectScripts.RemoveAt(i);
						continue;
					}
					if (aMonobehaviour.gameObject == null)
					{
						aListObjectScripts.RemoveAt(i);
						continue;
					}
					aList.Add((T)anObject);
				}
			}
		}
		return aList;
	}
	
//	public static void CleanListsFromDeadObjects()
//	{
//		foreach (Type aType in itsListSorted.Keys)
//		{
//			List<object_script> aListObjectScripts = itsListSorted[aType];
//			for (int i=aListObjectScripts.Count-1;i>=0;i--)
//			{
//				MonoBehaviour aMonobehaviour = (MonoBehaviour) aListObjectScripts[i];
//				if (aMonobehaviour == null)
//				{
//					aListObjectScripts.RemoveAt(i);
//					continue;
//				}
//				if (aMonobehaviour.gameObject == null)
//				{
//					aListObjectScripts.RemoveAt(i);
//					continue;
//				}
//			}
//		}
//	}
	
	/// <summary>
	/// Get object names of objects with specific type (include derived objects)
	/// </summary>
	/// <returns></returns>
	public static IEnumerable<string> GetObjectsNames<T>() where T : KGFObject
	{
		foreach (KGFObject anObject in GetObjects<T>())
			yield return anObject.name;
		yield break;
	}
	
	/// <summary>
	/// Get first object in list with specific type (include derived objects)
	/// </summary>
	/// <returns></returns>
	public static T GetObject<T>() where T : KGFObject
	{
		foreach (T aT in GetObjects<T>())
		{
			MonoBehaviour aMonoBehaviour = (MonoBehaviour)aT;
			if (aMonoBehaviour == null)
				continue;
			if (aMonoBehaviour.gameObject == null)
				continue;
			return aT;
		}
		return null;
	}
	
	/// <summary>
	/// Get count of currently registered add-handlers
	/// </summary>
	/// <returns></returns>
	public static int GetAddHandlerCount()
	{
		return itsListEventsAdd.Count;
	}
	
	/// <summary>
	/// Get count of currently registered add-once-handlers
	/// </summary>
	/// <returns></returns>
	public static int GetAddOnceHandlerCount()
	{
		return itsListEventsAddOnce.Count;
	}
	
	/// <summary>
	/// Get all cache list types
	/// </summary>
	/// <returns></returns>
	public static IEnumerable<Type> GetObjectCacheListTypes()
	{
		foreach (Type aType in itsListSorted.Keys)
			yield return aType;
		yield break;
	}
	
	/// <summary>
	/// If there is a fast cache list for this type, return the count of items in it
	/// </summary>
	/// <param name="theType"></param>
	/// <returns></returns>
	public static int GetObjectCacheListCountByType(Type theType)
	{
		if (itsListSorted.ContainsKey(theType))
			return itsListSorted[theType].Count;
		
		return 0;
	}
}
