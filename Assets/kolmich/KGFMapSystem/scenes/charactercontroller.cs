using UnityEngine;
using System.Collections;

public class charactercontroller : MonoBehaviour
{
	private float itsVelocity = 30.0f;
	
	// Update is called once per frame
	void Update ()
	{		
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{			
			transform.Rotate(new Vector3(0.0f,-Time.deltaTime*itsVelocity*4.0f,0.0f));
		}
		else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			transform.Rotate(new Vector3(0.0f,Time.deltaTime*itsVelocity*4.0f,0.0f));
		}
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
		{
			transform.position -= transform.forward*Time.deltaTime*itsVelocity;
		}
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
		{
			transform.position += transform.forward*Time.deltaTime*itsVelocity;
		}		
	}
}
