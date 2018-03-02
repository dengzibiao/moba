using UnityEngine;
using System.Collections;

public class flyMove : MonoBehaviour
{

    public bool returnpos = false;

    CharacterState player = null;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == Tag.player)
        {
            if (!returnpos)
            {
                player = collider.GetComponent<CharacterState>();
                if (player != null)
                {
                    player.transform.parent = transform;
                    player.pm.nav.enabled = false;
                    player.pm.SetControlSwitch(true);
                    transform.GetComponent<Animator>().enabled = true;
                    GetComponent<SphereCollider>().enabled = false;
                }
            }
        }
    }

    void onStopFly()
    {
        player.transform.parent = CharacterManager.instance.transform;
        player.pm.nav.enabled = true;
        player.pm.SetControlSwitch(false);
    }

}
