//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Allows dragging of the camera object and restricts camera's movement to be within bounds of the area created by the rootForBounds colliders.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Drag Camera")]
public class UIDragCamera : MonoBehaviour
{

    Transform player = null;

	/// <summary>
	/// Target object that will be dragged.
	/// </summary>

	public UIDraggableCamera draggableCamera;

	/// <summary>
	/// Automatically find the draggable camera if possible.
	/// </summary>

	void Awake ()
	{
		if (draggableCamera == null)
		{
			draggableCamera = NGUITools.FindInParents<UIDraggableCamera>(gameObject);
		}
	}

	/// <summary>
	/// Forward the press event to the draggable camera.
	/// </summary>

	void OnPress (bool isPressed)
	{
		if (enabled && NGUITools.GetActive(gameObject) && draggableCamera != null)
		{
            if (null != ThirdCamera.instance._MainPlayer)
            {
                player = ThirdCamera.instance._MainPlayer;
                ThirdCamera.instance._MainPlayer = null;
            }
            draggableCamera.Press(isPressed);
            if (!isPressed)
            {
                if (null != player && !GameLibrary.SceneType(SceneType.PVP3))
                {
                    ThirdCamera.instance._MainPlayer = player;
                }
            }
        }
        
	}

	/// <summary>
	/// Forward the drag event to the draggable camera.
	/// </summary>

	void OnDrag (Vector2 delta)
	{
		if (enabled && NGUITools.GetActive(gameObject) && draggableCamera != null)
		{
			draggableCamera.Drag(delta, player);
		}
	}

	/// <summary>
	/// Forward the scroll event to the draggable camera.
	/// </summary>

	void OnScroll (float delta)
	{
		if (enabled && NGUITools.GetActive(gameObject) && draggableCamera != null)
		{
			draggableCamera.Scroll(delta);
		}
	}
}
