using UnityEngine;
using System.Collections;

public class ChangeState : MonoBehaviour {
  public GameObject mainCamera;
    public GameObject vrCamera;
    public GameObject uiCamera;
   
	// Use this for initialization
	void Start () {
        //if(Globe.IsvrState)
        {
            mainCamera.GetComponent<Camera>().enabled = !Globe.IsvrState;
            vrCamera.SetActive(Globe.IsvrState);
        }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnGUI()
    {
        if (true)
        {
            if (Globe.IsvrState)
            {
                if (GUI.Button(new Rect(Screen.width / 2 - 50, 10, 100, 50), "changeNormal"))
                {
                   // Debug.Log("vrStateChangef");
                    Globe.IsvrState = false;
                    mainCamera.GetComponent<Camera>().enabled = true;
                    vrCamera.SetActive(false);
                    uiCamera.SetActive(false);
                    uiCamera.SetActive(true);
                }
            }
            else
            {
                if (GUI.Button(new Rect(Screen.width / 2 - 50, 10, 100, 50), "changeVr"))
                {
                //    Debug.Log("vrStateChangef");
                    Globe.IsvrState = true;
                    mainCamera.GetComponent<Camera>().enabled = false;
                    vrCamera.SetActive(true);
                    uiCamera.SetActive(false);
                    uiCamera.SetActive(true);
                }
            }
           
        }
    }
}
