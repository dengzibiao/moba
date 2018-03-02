using UnityEngine;

public class FlopAnimEvent : MonoBehaviour 
{
    FlopCardItem flopCardItem;
    void Start ()
    {
        flopCardItem = GetComponentInParent<FlopCardItem>();
    }

    public void OnAnimOver ()
    {
        if(flopCardItem != null)
            flopCardItem.FlopOver();
    }
}