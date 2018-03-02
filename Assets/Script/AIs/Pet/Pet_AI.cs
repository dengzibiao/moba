using UnityEngine;
using System.Collections;
using Tianyu;

public class Pet_AI : MonoBehaviour
{

    public long petID;

    Animator anim;
    UnityEngine.AI.NavMeshAgent nav;

    public CharacterState master;

    int layerIndex;
    int run_Hash;
    int idle01_Hash;

    float distance;
    float randomTime = 0f;

    GameObject petName;

    PetNode petnode;

    void Init()
    {
        anim = GetComponent<Animator>();
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        run_Hash = Animator.StringToHash("BaseCw.Run");
        idle01_Hash = Animator.StringToHash("BaseCw.Idle01");
        layerIndex = anim.GetLayerIndex("BaseCw");
        nav.stoppingDistance = 0.8f;
        nav.speed = master.moveSpeed - 0.1f;
        CreateNameLabel();
    }

    public void InitPet(CharacterState cs, long petID)
    {
        master = cs;
        this.petID = petID;
        Init();
    }

    void FixedUpdate()
    {
        SetPetNameInvisible(!GameLibrary.isBossChuChang);
        if (GameLibrary.isBossChuChang) return;
        if (null != petName)
            SetNamePos();
        //if (null == master)
        //    master = CharacterManager.playerCS;
        if (null != master)
            FollowPlayer();
    }

    void FollowPlayer()
    {
        distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(master.transform.position.x, master.transform.position.z));
        if (distance >= 5)
            SetPetPos();

        if (distance > nav.stoppingDistance)
        {
            if (anim.HasState(layerIndex, run_Hash))
                anim.SetBool("Run", true);
            nav.SetDestination(master.transform.position);
            randomTime = 0;
        }
        else
        {
            if (anim.HasState(layerIndex, run_Hash))
                anim.SetBool("Run", false);
            nav.enabled = false;
            nav.enabled = true;
            randomTime += Time.deltaTime;
            if (randomTime >= 10f)
            {
                RandomAnim();
                randomTime = 0;
            }
        }
    }

    void SetPetPos()
    {
        nav.enabled = false;
        BattleUtil.GetRadiusRandomPos(transform, master.transform, 1f, 2f);
        nav.enabled = true;
    }

    void RandomAnim()
    {
        if (anim.HasState(layerIndex, idle01_Hash))
            anim.SetTrigger("Idle");
    }

    void CreateNameLabel()
    {
        petnode = FSDataNodeTable<PetNode>.GetSingleton().FindDataByType(petID);
        if (petnode == null)
            return;
        petName = GameObject.Instantiate(Resources.Load("Prefab/OtherName")) as GameObject;
        petName.GetComponent<UIWidget>().depth = -2;
        petName.GetComponent<UILabel>().fontSize = 16;
        ChangePetName();
        petName.name = petnode.id + "_head";
        petName.transform.parent = null != CharacterManager.instance.UIControl ? CharacterManager.instance.UIControl : GameObject.FindObjectOfType<UIRoot>().transform;
        petName.transform.localScale = Vector3.one;
    }

    void SetNamePos()
    {
        petName.transform.position = BattleUtil.WorldToScreenPoint(transform.Find("Headbuff").position);
    }

    void SetPetNameInvisible(bool b)
    {
        if (petName != null)
        {
            if ((b && !petName.gameObject.activeSelf) || (!b && petName.gameObject.activeSelf))
            {
                petName.gameObject.SetActive(b);
            }
        }
    }

    public void DestroyName()
    {
        if(null != petName)
            Destroy(petName.gameObject);
    }

    public void ChangePetName()
    {
        if (null == petName || null == petnode) return;
        if (master == CharacterManager.playerCS)
        {
            petName.GetComponent<UILabel>().text = playerData.GetInstance().selfData.playeName + "的 " + petnode.name;
        }
        else
        {
            RoleInfo rInfo = playerData.GetInstance().NearRIarr[master.keyId];
            petName.GetComponent<UILabel>().text = rInfo.name + "的 " + petnode.name;
        }
    }

}