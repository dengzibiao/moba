using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class NPCMotion : MonoBehaviour
{
    int layerIndex;
    Animator anim;
    UnityEngine.AI.NavMeshAgent nav;

    GameObject player = null;
    Transform AutoPoint = null;

    bool isDungeons = true;
    float distance;

    int run_Hash;
    int talk_Hash;
    int idle01_Hash;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == GameLibrary.UI_Major || SceneManager.GetActiveScene().name == GameLibrary.LGhuangyuan || Globe.isFightGuide)
            isDungeons = false;
        anim = GetComponent<Animator>();
        if (null != anim.runtimeAnimatorController)
        {
            layerIndex = anim.GetLayerIndex("BaseNpc");
            if (layerIndex != -1)
            {
                run_Hash = Animator.StringToHash("BaseNpc.Run");
                talk_Hash = Animator.StringToHash("BaseNpc.Talk");
                idle01_Hash = Animator.StringToHash("BaseNpc.Idle01");
                anim.SetLayerWeight(0, 0);
                anim.SetLayerWeight(layerIndex, 1);
            }
        }
        if (isDungeons)
        {
            nav = UnityUtil.AddComponetIfNull<UnityEngine.AI.NavMeshAgent>(gameObject);
            nav.radius = 0.1f;
            nav.height = 0.6f;
            player = CharacterManager.player;
            nav.stoppingDistance = 0.5f;
            nav.speed = 1;
        }
        else
        {
            InvokeRepeating("RandomState", 0f, 5f);
        }
    }
    void FixedUpdate()
    {
        if (isDungeons)
        {
            if (null == AutoPoint)
                Run();
            else
                RunAirWall();
        }
    }

    void RandomState()
    {
        if (null == anim.runtimeAnimatorController) return;

        float random = Globe.isFightGuide ? 0f : Random.value;
        if (random >= 0.5f)
        {
            if (layerIndex != -1 && anim.HasState(layerIndex, idle01_Hash))
                anim.SetTrigger("Idle");
        }
        else
        {
            if (layerIndex != -1 && anim.HasState(layerIndex, talk_Hash))
                anim.SetTrigger("Talk");
        }
    }

    public void Move(Vector3 v)
    {
        nav.SetDestination(v);
    }

    void Run()
    {
        if (null == player)
            player = CharacterManager.player;
        if (null == player) return;
        distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance > nav.stoppingDistance)
        {
            if (anim.HasState(layerIndex, run_Hash))
                anim.SetInteger("Speed", 1);
            Move(player.transform.position);
        }
        if (distance <= nav.stoppingDistance)
        {
            if (anim.HasState(layerIndex, run_Hash))
                anim.SetInteger("Speed", 0);
        }
    }

    void RunAirWall()
    {
        if (null == AutoPoint) return;
        distance = BattleUtil.V3ToV2Dis(AutoPoint.position, transform.position);
        if (distance >= nav.stoppingDistance)
        {
            anim.SetInteger("Speed", 1);
            nav.SetDestination(AutoPoint.position);
        }
        if (distance <= 1)
        {
            enabled = false;
            gameObject.SetActive(false);
        }
    }

    public void SetAutoPoint(Transform pos)
    {
        if (null != pos)
            AutoPoint = pos;
    }

}
