using UnityEngine;
using System.Collections;

public class ToEscapeTrigger : MonoBehaviour
{

    public delegate void OnExit();
    public OnExit OnExitT;

    BoxCollider boxCol;
    GameObject effect;
    UnityEngine.AI.NavMeshObstacle navObs;


    void Start()
    {
        boxCol = GetComponent<BoxCollider>();
        effect = transform.FindChild("Particle System").gameObject;
        navObs = GetComponent<UnityEngine.AI.NavMeshObstacle>();
        effect.gameObject.SetActive(false);
        navObs.enabled = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (null != OnExitT)
                OnExitT();
            boxCol.isTrigger = false;
            FightTouch._instance.SetAllBtnLockStatus(false);
            effect.gameObject.SetActive(true);
            navObs.enabled = true;
        }
    }

}
