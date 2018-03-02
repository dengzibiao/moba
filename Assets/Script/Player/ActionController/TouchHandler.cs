using UnityEngine;
using System.Collections;

public enum TouchEvent
{
	Down,
	Up,
	Press
}

public class TouchHandler : ITouchHandler
{
	private static TouchHandler instance = null;
	private static readonly object syncRoot = new object ();
	public delegate void KeyChanged (TouchEvent e,TOUCH_KEY key);

	public static KeyChanged OnKeyChanged;
    public Vector3 mOffset;

	public static TouchHandler GetInstance ()
	{
		if (instance == null) {
			lock (syncRoot) {
				if (instance == null) {
					instance = new TouchHandler ();
				}
			}
		}
		return instance;
	}

	private int mKeyValue;

	private TouchHandler ()
	{
		Start ();
	}

	void Start ()
	{
		Clear ();
	}

	void OnDisable ()
	{
	}

	public void Clear ()
	{
		mKeyValue = 0;
	}
		
	public bool IsTouched (TOUCH_KEY key)
	{
		return IsTouched (key, mKeyValue);
	}

	public bool IsTouched (TOUCH_KEY key, int compKey)
	{
		return (compKey & (int)key) != 0;
	}

	public void Touch (TOUCH_KEY key)
	{
		Touch (key, ref mKeyValue);
	}

	public void Touch (TOUCH_KEY key, ref int compKey)
	{
        //Clear();
        if (OnKeyChanged != null && !IsTouched(key))
            OnKeyChanged(TouchEvent.Down, key);
        compKey |= (int)key;
    }

	public void Release (TOUCH_KEY key)
	{
		Release (key, ref mKeyValue);
	}

	public void Release (TOUCH_KEY key, ref int compKey)
	{
		if (OnKeyChanged != null && IsTouched (key))
			OnKeyChanged (TouchEvent.Up, key);
		compKey &= ~(int)key;
	}
}
