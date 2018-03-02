using UnityEngine;
using System.Collections;

public class NetworkMonster : MonoBehaviour {

    CharacterState cs;

    void Awake ()
    {
        cs = GetComponent<CharacterState>();
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AttackObject (GameObject targetObj)
    {
        cs.SetAttackTargetTo(targetObj.GetComponent<CharacterState>());
        if(transform!=null&& targetObj!=null)
        transform.LookAt(new Vector3(targetObj.transform.position.x, transform.position.y, targetObj.transform.position.z));
        if(cs!=null&&cs.pm!=null)
        cs.pm.ContinuousAttack();
        else
        {
            Debug.Log(transform.name + "没有cs.pm");
        }
    }
}
