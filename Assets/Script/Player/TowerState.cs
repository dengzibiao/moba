using UnityEngine;
using System.Collections;

public class TowerState : MonoBehaviour
{
    public GameObject patrolPoint;   

    void Awake()
    {       
        patrolPoint = new GameObject();
        patrolPoint.name = "PatrolPoints";
        patrolPoint.transform.parent = this.transform;
      //var collider=  patrolPoint.AddComponent<BoxCollider>();
      //  collider.isTrigger = true;
      //  collider.size = new Vector3(1f,1f,4f);
    }

   public void SetPatrolPoint(Vector3 pos)
    {
        if (patrolPoint!=null)
        {
            patrolPoint.transform.localPosition=pos;
        }
    }

    void OnTriggerEnter(Collider obj)
    {
       
    }

}
