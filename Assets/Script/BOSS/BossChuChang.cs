using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class BossChuChang : MonoBehaviour
{

    public bool isWarning = false;
    float warningTime = 1.5f;
    public string queue;
    public GameObject bossobj;
    public List<SpawnMonster> enemyList = new List<SpawnMonster>();
    private SpawnMonster bossSpawnMonster;
    private CharacterState mCurBoss;
    private SceneNode mCurSceneNode;
    private Boss_AI mBossAI;
    private SkinnedMeshRenderer[] skinned;
    private int spawnQueue;
    private int mDefaultUIDepth = 0;
    private Animator mCurBossAni;
    private bool mShouldTimer;
    private UICamera mUICameras;
    private Camera mUICamera;
    private Mesh mBossMaterial;
    private float mBossLength;

    bool isKV;
    GameObject cart;

    private UIRoot _uiRoot;
    private UIRoot mUIRoot
    {
        get
        {
            if (_uiRoot == null)
            {
                _uiRoot = FindObjectOfType<UIRoot>();
            }
            return _uiRoot;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (GameLibrary.SceneType(SceneType.KV) && other.CompareTag(Tag.cart))
        {
            isKV = true;
            cart = other.gameObject;
            TriggerBoss();
        }
        else if (CheckMainPlayerTrigger(other.gameObject))
        {
            TriggerBoss();
        }
    }

    public void TriggerBoss()
    {
        if (CharacterManager.playerCS == null || CharacterManager.playerCS.isDie) return;
        mShouldTimer = false;
        skinned = CharacterManager.playerCS.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (bossobj != null)
        {
            bossSpawnMonster = bossobj.GetComponent<SpawnMonster>();
            spawnQueue = bossSpawnMonster.spawnQueue;
        }
        mCurSceneNode = FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(GameLibrary.dungeonId);
        if (isKV && cart.GetComponent<BullockCarts>())
            cart.GetComponent<BullockCarts>().isMove = false;
        GetComponent<BoxCollider>().enabled = false;
        if (isWarning && null != bossobj)
        {
            SceneUIManager.instance.bossWarning.ShowWarning(warningTime);
        }

        if (mCurSceneNode.needAnimation == 1)
        {
            //摄像机直推的动作
            PlayEntranceAnimation();
            //CD停止
            CDTimer.GetInstance().CDRunOrStop(false);
        }
        else if (mCurSceneNode.needAnimation == 2)
        {
            //模型出场动画加上摄像机动画
            PlaySpecialAnimation();
            //CD停止
            CDTimer.GetInstance().CDRunOrStop(false);
        }
        else if (mCurSceneNode.needAnimation == 3)
        {
            MonsterData mCurMonsterData = new MonsterData(spawnQueue);
            ShowBoos(mCurMonsterData);
            //CD停止
            CDTimer.GetInstance().CDRunOrStop(false);
        }
        else
        {
            Invoke("ShowBossSpawn", warningTime);
        }
    }

    void Update()
    {
        if (mCurBossAni != null)
        {
            AnimatorStateInfo mStateInfo = mCurBossAni.GetCurrentAnimatorStateInfo(0);
            if (!mShouldTimer && mStateInfo.IsName("Enter"))
            {
                mShouldTimer = true;
                Invoke("AddBossName", mStateInfo.length - 3.5f);
            }
        }
    }

    void PlaySpecialAnimation()
    {
        GameLibrary.isBossChuChang = true;
        CharacterManager.instance.PlayerStop();
        if (bossSpawnMonster != null)
        {
            bossSpawnMonster.OnCreatMonster += (GameObject mBossObj, CharacterData cd) =>
            {
                mCurBoss = mBossObj.GetComponent<CharacterState>();
                if (mCurBoss.mBossShowCamera != null)
                {
                    AudioController.Instance.PlayEffectSound("BOSS/Boss_" + cd.attrNode.modelNode.modelPath, CharacterManager.playerCS);
                    mBossAI = mCurBoss.GetComponent<Boss_AI>();
                    if (mBossAI != null)
                    {
                        mBossAI.enabled = false;
                    }
                    GameLibrary.Instance().SetCsInvisible(CharacterManager.playerCS, true, skinned);
                    SetUICameraDepthAndCullingMask();
                    mCurBossAni = mCurBoss.pm.ani;
                    mCurBoss.pm.Enter();
                    mCurBoss.OnEnterOver += (mCs) =>
                    {
                        if (mBossAI != null)
                        {
                            mBossAI.enabled = true;
                        }
                        GameLibrary.isBossChuChang = false;
                        mCs.mBossShowCamera.gameObject.SetActive(false);
                        GameLibrary.Instance().SetCsInvisible(CharacterManager.playerCS, false, skinned);
                    };
                    mCurBoss.mBossShowCamera.gameObject.SetActive(true);
                }
                else
                {
                    GameLibrary.isBossChuChang = false;
                }
            };
            bossSpawnMonster.StartSpawn();
        }
    }

    void PlayEntranceAnimation()
    {
        GameLibrary.isBossChuChang = true;
        CharacterManager.instance.PlayerStop();
        if (bossSpawnMonster != null)
        {
            bossSpawnMonster.OnCreatMonster += (GameObject mBossObj, CharacterData cd) =>
            {
                GameLibrary.Instance().SetCsInvisible(CharacterManager.playerCS, true, skinned);
                SetUICameraDepthAndCullingMask();
                GameObject mEntranceCamera = new GameObject("mEntranceCamera");
                mEntranceCamera.transform.parent = mBossObj.transform.parent;
                Camera mCamera = mEntranceCamera.AddComponent<Camera>();
                mCamera.depth = Camera.main.depth + 1;
                mCamera.cullingMask = Camera.main.cullingMask;
                mCurBoss = mBossObj.GetComponent<CharacterState>();
                mBossAI = mCurBoss.GetComponent<Boss_AI>();
                if (mBossAI != null)
                {
                    mBossAI.enabled = false;
                }
                AddBossName();
                float mExtendSize = GetMeshSize(mCurBoss);
                mCurBoss.transform.forward = CharacterManager.playerCS.transform.position - mCurBoss.transform.position;
                mEntranceCamera.transform.forward = mCurBoss.transform.position - CharacterManager.playerCS.transform.position;
                mEntranceCamera.transform.position = CharacterManager.playerCS.transform.position + mEntranceCamera.transform.forward * 0.1f;
                mEntranceCamera.transform.localPosition = new Vector3(mEntranceCamera.transform.localPosition.x, mCurBoss.transform.localPosition.y + mExtendSize + 0.05f, mEntranceCamera.transform.localPosition.z);
                float mExtendDis = Vector3.Distance(CharacterManager.playerCS.transform.position, mCurBoss.transform.position) - mExtendSize - mBossLength - mCamera.nearClipPlane - 0.1f;
                //if (cd.attrNode.icon_name.Equals("gw_082") || cd.attrNode.icon_name.Equals("gw_113")) mExtendDis -= mExtendSize * 2.0f;
                TweenPosition td = mEntranceCamera.AddComponent<TweenPosition>();
                SetTweenPositon(td, mEntranceCamera.transform.localPosition, mEntranceCamera.transform.localPosition + mEntranceCamera.transform.forward * mExtendDis,
                    UITweener.Style.Once, UITweener.Method.Linear, 0.5f);
                td.SetOnFinished(() =>
                {
                    SwitchSlowAnimation(mEntranceCamera, mCurBoss);
                });
            };
            bossSpawnMonster.StartSpawn();
        }
    }

    private void SetUICameraDepthAndCullingMask()
    {
        mUICameras = FindObjectOfType<UICamera>();
        mUICamera = mUICameras.GetComponent<Camera>();
        mUICamera.depth = mDefaultUIDepth + 1;
        mUICamera.cullingMask = 1 << (int)GameLayer.BossShow;
        AudioController.Instance.PlayBackgroundMusic("BossIn", false);
    }

    private void AddBossName()
    {
        //播放名字显示的声音
        AudioController.Instance.PlayUISound(GameLibrary.Resource_Sound + "BOSS/BossName", true);
        SetBossNamePos(mUICameras, mUICamera);
    }

    private float GetMeshSize(CharacterState mCurCs)
    {
        float result = 0.3f;
        SetBossMaterial(mCurCs);
        if (mBossMaterial != null)
        {
            result = mBossMaterial.bounds.size.z * 0.5f * mCurCs.transform.localScale.z;
        }
        return result;
    }

    private void SetBossMaterial(CharacterState mCurCs)
    {
        if (mBossMaterial == null)
        {
            mBossMaterial = mCurCs.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
            mBossLength = (mBossMaterial.bounds.size.y - mBossMaterial.bounds.center.y) * 0.5f * mCurCs.transform.localScale.y;
        }
    }

    private void SwitchSlowAnimation(GameObject mEntranceCamera, CharacterState mCurCs)
    {
        mCurCs.pm.Enter();
        TweenPosition td = mEntranceCamera.AddComponent<TweenPosition>();
        SetTweenPositon(td, mEntranceCamera.transform.localPosition, mEntranceCamera.transform.localPosition + mEntranceCamera.transform.forward * 0.05f / mCurCs.transform.localScale.z,
            UITweener.Style.Once, UITweener.Method.EaseIn, 3.0f);
        td.SetOnFinished(() =>
        {
            td.onFinished.Clear();
            if (mBossAI != null)
            {
                mBossAI.enabled = true;
            }
            GameLibrary.isBossChuChang = false;
            GameLibrary.Instance().SetCsInvisible(CharacterManager.playerCS, false, skinned);
            Destroy(mEntranceCamera);
        });
    }

    private void SetBossNamePos(UICamera mUICameras, Camera mUICamera)
    {
        GameObject prefab = Resources.Load("Prefab/BossShowUI") as GameObject;
        if (prefab != null)
        {
            mUIRoot.gameObject.layer = (int)GameLayer.BossShow;
            GameObject go = NGUITools.AddChild(mUIRoot.gameObject, prefab);
            go.GetComponent<UILabel>().text = mCurSceneNode.animationName;
            NGUITools.SetLayer(go, (int)GameLayer.BossShow);
            TweenPosition td = go.AddComponent<TweenPosition>();
            float mWidth = mUIRoot.manualWidth;
            float mHeight = mUIRoot.manualHeight;
            SetTweenPositon(td, new Vector3(-mWidth / 2, -mHeight * (0.5f - 0.22f), 0), new Vector3(-mWidth * (0.5f - 0.618f), -mHeight * (0.5f - 0.22f), 0),
                UITweener.Style.Once, UITweener.Method.Linear, 0.5f);
            td.SetOnFinished(() =>
            {
                TweenPosition mTd = go.AddComponent<TweenPosition>();
                SetTweenPositon(mTd, mTd.transform.localPosition, mTd.transform.localPosition + Vector3.right * 0.3f,
                UITweener.Style.Once, UITweener.Method.EaseIn, 3.0f);
                mTd.SetOnFinished(() =>
                {
                    Destroy(go);
                    mUIRoot.gameObject.layer = LayerMask.NameToLayer("UI");
                    mUICamera.cullingMask = 1 << LayerMask.NameToLayer("UI");
                    mUICamera.depth = mDefaultUIDepth;
                    StartLandingShuJu.GetInstance().PlayBgMusic();
                    //打开CD
                    CDTimer.GetInstance().CDRunOrStop(true);
                });
            });
        }
    }

    private void SetTweenPositon(TweenPosition td, Vector3 from, Vector3 to, UITweener.Style style, UITweener.Method method, float duration)
    {
        td.from = from;
        td.to = to;
        td.style = style;
        td.method = method;
        td.duration = duration;
        td.ignoreTimeScale = false;
    }

    void ShowBoos(MonsterData mCurMonsterData)
    {
        if (mCurMonsterData.attrNode == null)
        {
            Invoke("ShowBossSpawn", warningTime);
            Invoke("ShowMonster", warningTime + 0.8f);
        }
        else
        {
            GameLibrary.isBossChuChang = true;
            GameLibrary.Instance().SetCsInvisible(CharacterManager.playerCS, true, skinned);
            SetUICameraDepthAndCullingMask();
            CharacterManager.instance.PlayerStop();
            this.gameObject.SetActive(false);
            string modelPath = mCurMonsterData.attrNode.modelNode.modelPath;
            GameObject go = Resources.Load(GameLibrary.Effect_Boss + modelPath) as GameObject;
            if (go != null)
            {
                AudioController.Instance.PlayEffectSound("BOSS/Boss_" + modelPath, CharacterManager.playerCS); 
                GameObject mCurBossEffect = Instantiate(go) as GameObject;
                mCurBossEffect.transform.position = bossobj.transform.position;
                Effect_LifeCycle lifeCycle = mCurBossEffect.GetComponent<Effect_LifeCycle>();
                Invoke("AddBossName", lifeCycle.cycle - 3.5f);
                lifeCycle.OnDesEff += () =>
                {
                    GameLibrary.isBossChuChang = false;
                    GameLibrary.Instance().SetCsInvisible(CharacterManager.playerCS, false, skinned);
                    Invoke("ShowBossSpawn", 0);
                    Invoke("ShowMonster", 0.8f);
                };
            }
            else
            {
                GameLibrary.isBossChuChang = false;
                Invoke("ShowBossSpawn", warningTime);
                Invoke("ShowMonster", warningTime + 0.8f);
            }
        }
    }

    void ShowBossSpawn()
    {
        if (null != bossobj)
        {
            bossobj.GetComponent<SpawnMonster>().StartSpawn();
        }
    }

    void ShowMonster()
    {
        if (null != enemyList && enemyList.Count > 0)
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (null == enemyList[i]) continue;
                enemyList[i].enabled = true;
            }
        }
    }

    bool CheckMainPlayerTrigger(GameObject go)
    {
        return !GameLibrary.SceneType(SceneType.KV) && go.CompareTag(Tag.player) && go.GetComponent<CharacterState>() && go.GetComponent<CharacterState>().state == Modestatus.Player;
    }

}