using UnityEngine;
using System.Collections;

public class MountAndPetMotion : MonoBehaviour
{
    public int nodeId;
    public MountAndPet mCwType;
    private Animator ani;
    private int mDefaultLayer;
    private int run_Hash, idle_Hash, idle01_Hash, idle02_Hash;
    private UIMountNode mountNode;
    private PetNode petNode;
    private string mSoundPath;
    private Pet_AI mPetAI;
    private CharacterState cs;
    private GUIManager mUImanager;

    void Awake()
    {
        ani = GetComponent<Animator>();
        mDefaultLayer = ani.GetLayerIndex("BaseCw");
        run_Hash = Animator.StringToHash("BaseCw.Run");
        idle_Hash = Animator.StringToHash("BaseCw.Idle");
        idle01_Hash = Animator.StringToHash("BaseCw.Idle01");
        idle02_Hash = Animator.StringToHash("BaseCw.Idle02");
        Init(nodeId, mCwType);
        mUImanager = Singleton<GUIManager>.Instance;
    }

    public void Init(int nodeId, MountAndPet type)
    {
        this.nodeId = nodeId;
        mCwType = type;
        mSoundPath = string.Empty;
        switch (mCwType)
        {
            case MountAndPet.Null:
                break;
            case MountAndPet.Mount:
                GameLibrary.mountNodeList.TryGetValue(nodeId, out mountNode);
                break;
            case MountAndPet.Pet:
                GameLibrary.PetNodeList.TryGetValue(nodeId, out petNode);
                break;
            default:
                break;
        }
    }

    public void PlayMusicByAnim(AnimatorStateInfo stateInfo)
    {
        int mCurHash = stateInfo.fullPathHash;
        if (mCurHash == run_Hash)
        {

        }
        else if (mCurHash == idle_Hash)
        {

        }
        else if (mCurHash == idle01_Hash)
        {
            mSoundPath = GetSoundPathByType("show1");
        }
        else if (mCurHash == idle02_Hash)
        {

        }
        if (!string.IsNullOrEmpty(mSoundPath))
        {
            AudioController.Instance.PlayUISound(mSoundPath, true);
        }
    }

    private void GetMountAndPetMaster()
    {
        switch (mCwType)
        {
            case MountAndPet.Null:
                break;
            case MountAndPet.Mount:
                if (cs == null)
                {
                    cs = transform.GetComponentInParent<CharacterState>();
                }
                break;
            case MountAndPet.Pet:
                if (mPetAI == null)
                {
                    mPetAI = GetComponent<Pet_AI>();
                    if (mPetAI != null)
                    {
                        cs = mPetAI.master;
                    }
                }
                break;
            default:
                break;
        }
    }

    private bool CheckCanPlay()
    {
        return cs != null && CharacterManager.playerCS == cs && (mUImanager == null || (mUImanager != null && !mUImanager.HasFullScreenUIActive()));
    }

    private string GetSoundPathByType(string soundName)
    {
        string result = string.Empty;
        GetMountAndPetMaster();
        switch (mCwType)
        {
            case MountAndPet.Null:
                break;
            case MountAndPet.Mount:
                if (mountNode != null && (cs == null || CheckCanPlay()))
                {
                    result += GameLibrary.Resource_Sound + mountNode.icon_name + "/" + soundName;
                }
                break;
            case MountAndPet.Pet:
                if (petNode != null && (mPetAI == null || CheckCanPlay()))
                {
                    result += GameLibrary.Resource_Sound + petNode.icon_name + "/" + soundName;
                }
                break;
            default:
                break;
        }
        return result;
    }

    /// <summary>
    /// 播放音乐
    /// </summary>
    /// <param name="s"></param>
    public virtual void PlayMusic(string s)
    {
        if (s != null && s.Length != 0)
        {
            mSoundPath = GetSoundPathByType(s);
            if (!string.IsNullOrEmpty(mSoundPath))
            {
                AudioController.Instance.PlayUISound(mSoundPath, true);
            }
        }
    }

    public virtual void StopMusic()
    {
        mSoundPath = GetSoundPathByType(string.Empty);
        if (!string.IsNullOrEmpty(mSoundPath))
        {
            AudioController.Instance.StopUISound();
        }
    }
}
