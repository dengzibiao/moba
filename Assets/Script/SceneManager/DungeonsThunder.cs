using UnityEngine;
using System.Collections;

public class DungeonsThunder : MonoBehaviour
{

    [HideInInspector]
    public CharacterState targetCs;

    GameObject thunder;
    GameObject warning;

    Vector3 endPos;

    bool isBeginThunder = false;

    void Start()
    {

        thunder = transform.Find("Thunder").gameObject;
        warning = transform.Find("Warning").gameObject;

        if (null != targetCs)
        {
            ThunderPos();
            Invoke("ThunderWarning", 1f);
        }

    }

    void FixedUpdate()
    {
        if (isBeginThunder)
        {
            transform.Translate(-Vector3.up * Time.deltaTime * 5);
            if (Vector3.Distance(transform.position, endPos) < 0.1f)
            {
                isBeginThunder = false;
                Destroy(gameObject);
            }
        }
    }

    void ThunderPos()
    {
        transform.position = targetCs.transform.position;
        //int randomRat = Random.Range(0, 360);
        //transform.Rotate(new Vector3(0, randomRat, 0));
        //int randomDis = Random.Range(1, 4);
        //transform.position = targetCs.transform.position + transform.forward;
    }

    void ThunderWarning()
    {
        warning.SetActive(false);
        endPos = transform.position;
        BeginThunder();
    }

    void BeginThunder()
    {
        transform.position = transform.position + Vector3.up * 3;
        GetComponent<SphereCollider>().enabled = true;
        thunder.SetActive(true);
        isBeginThunder = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<CharacterState>().Hp(10, HUDType.DamagePlayer, true);
            Destroy(gameObject);
        }
    }


}
