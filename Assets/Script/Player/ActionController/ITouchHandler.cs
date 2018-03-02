using UnityEngine;
using System.Collections;

public interface ITouchHandler {
	/// <summary>
	/// Clear all key value.
	/// </summary>
	void Clear ();

	/// <summary>
	/// Determines whether this instance is touched the specified key.
	/// </summary>
	/// <returns><c>true</c> if this instance is touched the specified key; otherwise, <c>false</c>.</returns>
	/// <param name="key">Key.</param>
	bool IsTouched (TOUCH_KEY key);

	/// <summary>
	/// Touch the specified key.
	/// </summary>
	/// <param name="key">Key.</param>
	void Touch (TOUCH_KEY key);

	/// <summary>
	/// Release the specified key.
	/// </summary>
	/// <param name="key">Key.</param>
	void Release (TOUCH_KEY key);
}

public interface ITouchEventReceiver {
	void handleTouchEvent ();
}