using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    public delegate void TriggerEvent();
    public TriggerEvent OnTrigger;
    public bool once = true;

    void OnTriggerEnter(Collider other)
    {
        OnTrigger();
        if(once)
            gameObject.SetActive(false);
    }
}