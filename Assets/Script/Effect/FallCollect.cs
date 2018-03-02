using UnityEngine;

public class FallCollect : MonoBehaviour
{
    private float timer = 2f;
    public Transform player;
    private GameObject hit;
    private GameObject ball;
    public int type = 0;//0：gold 1:hp 2:anger


    public delegate void Trigger(GameObject prop, GameObject hero);
    public Trigger OnTrigger;

    public bool isFollowTarget = true;

    void Start()
    {
        player = CharacterManager.player.transform;
        hit = transform.Find("hit").gameObject;
        ball = transform.Find("ball").gameObject;
    }

    void Update()
    {
        if (isFollowTarget)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                LookTarget();
            }
        }
    }

    private void LookTarget()
    {
        Vector3 v = player.position - transform.position;
        if (v.magnitude >= 0.1f)
        {
            transform.Translate(v.normalized * Time.deltaTime * 3f);
        }
        else
        {
            hit.SetActive(true);
            ball.SetActive(false);
            Invoke("DestroyBall", 0.2f);
        }
    }

    private void DestroyBall()
    {
        if (type == 2)
        {
            // AngerPoint._instance.ChangePoint();
        }
        else if (type == 1)
        {
            player.GetComponent<CharacterState>().Hp(-100);
        }

        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            OnTrigger(gameObject, other.gameObject);
        }
    }

}
