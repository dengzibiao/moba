using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[ExecuteInEditMode]
public class SceneCameraMovement : MonoBehaviour 
{
	public float sensitivityX = 4F;
	public float sensitivityY = 4F;

	private float origMoveSpeed;
	public float moveSpeed = 1f;
	
	private float mHdg = 0F;
	private float mPitch = 0F;

	public ScionEngine.ScionPostProcess postProcessComponent;

	#if UNITY_EDITOR
    private void OnEnable()
    {
        EditorApplication.update += UpdateInEditor;
	}

	private void OnDisable()
	{
		EditorApplication.update -= UpdateInEditor;
	}
 
    private void UpdateInEditor ()
    {
        if (EditorApplication.isPlaying == false)
        {
            Update ();
        }
	}
	#endif

	void Start()
	{
		origMoveSpeed = moveSpeed;
		mPitch = transform.localEulerAngles.x;
		mHdg = transform.localEulerAngles.y;
	}

	private bool limitMovement = false;


	void Update()
	{
		if (Input.GetKeyDown("c"))
		{
			if (postProcessComponent.enabled == true) postProcessComponent.enabled = false;
			else postProcessComponent.enabled = true;
		}
		if (limitMovement == true)
		{
			Vector3 pos = transform.position;
			pos = Vector3.Min(pos, new Vector3(40.0f, 40.0f, 40.0f));
			pos = Vector3.Max(pos, new Vector3(-30.0f, 0.0f, -30.0f));
			transform.position = pos;
		}

        if (Application.isPlaying)
        {
            if (Input.GetKey (KeyCode.LeftShift))
            {
                moveSpeed = 5.0f * origMoveSpeed;
            }
            else moveSpeed = origMoveSpeed;

            if (Input.GetMouseButton (1))
            {
                float deltaX = Input.GetAxisRaw ("Mouse X") * sensitivityX;
                float deltaY = Input.GetAxisRaw ("Mouse Y") * sensitivityY;

                ChangeHeading (deltaX);
                ChangePitch (-deltaY);
            }

            if (Input.GetKey (KeyCode.D))
            {
                Strafe (moveSpeed * Time.smoothDeltaTime);
            }

            if (Input.GetKey (KeyCode.A))
            {
                Strafe (-moveSpeed * Time.smoothDeltaTime);
            }

            if (Input.GetKey (KeyCode.W))
            {
                MoveForwards (moveSpeed * Time.smoothDeltaTime);
            }

            if (Input.GetKey (KeyCode.S))
            {
                MoveForwards (-moveSpeed * Time.smoothDeltaTime);
            }
        }
	}
	
	void MoveForwards(float aVal)
	{
		transform.position += aVal * transform.forward;
	}
	
	void Strafe(float aVal)
	{
		transform.position += aVal * transform.right;
	}
	
	void ChangeHeight(float aVal)
	{
		transform.position += aVal * Vector3.up;
	}
	
	void ChangeHeading(float aVal)
	{
		mHdg += aVal;
		WrapAngle(ref mHdg);
		transform.localEulerAngles = new Vector3(mPitch, mHdg, 0);
	}
	
	void ChangePitch(float aVal)
	{
		mPitch += aVal;
		WrapAngle(ref mPitch);
		transform.localEulerAngles = new Vector3(mPitch, mHdg, 0);
	}
	
	public static void WrapAngle(ref float angle)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
	}
}
