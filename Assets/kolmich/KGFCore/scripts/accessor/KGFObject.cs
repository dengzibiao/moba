// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2012-06-22</date>
// <summary>minimalistic base class for all objects that use KGFAccessor</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// Minimalistic base class for all objects that should be accessed with KGFAccessor
/// Please do not add stuff but derive from this class, this should stay really minimal!
/// </summary>
public class KGFObject : MonoBehaviour
{
	/// <summary>
	/// Use this instead of Awake()
	/// </summary>
	public KGFDelegate EventOnAwake = new KGFDelegate();
	/// <summary>
	/// Use this instead of OnDestroy()
	/// </summary>
	public KGFDelegate EventOnDestroy = new KGFDelegate();
	
	/// <summary>
	/// Register itsself with KGFAccessor
	/// -> do not override, use KGFAwake() instead
	/// </summary>
	protected virtual void Awake()
	{
		KGFAccessor.AddKGFObject(this);
		EventOnAwake.Trigger(this);
		// call own KGFAwake, so user is not able to change our own basic Awake() behaviour
		KGFAwake();
	}
	
	/// <summary>
	/// Un-Register itsself with KGFAccessor
	/// </summary>
	void OnDestroy()
	{
		EventOnDestroy.Trigger(this);
		KGFAccessor.RemoveKGFObject(this);
		KGFDestroy();
	}
	
	/// <summary>
	/// Override this instead of Awake()
	/// </summary>
	protected virtual void KGFAwake()
	{
	}
	
	/// <summary>
	/// Override this instead of OnDestroy()
	/// </summary>
	protected virtual void KGFDestroy()
	{
	}
}
