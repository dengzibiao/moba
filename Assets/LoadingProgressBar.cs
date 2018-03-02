using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingProgressBar : MonoBehaviour {

    UISlider mProcess;
    float mNextTime = 0;
    static float mCurrentNum = 0f;
    float mMaxNum = 0.01f;
    float mCurrentDownSize;
    float mMaxSize;
    static string backImg;
    static string showText;
    UISprite m_backImg;
    UISprite m_roleRunImg;
    UISprite[] m_SpriteList;
    int m_RoleRunIndexer = 0;
    UILabel m_showText;
    UILabel m_proNumText0;
    UILabel m_proNumText1;
    UILabel m_proNumText2;
    public static bool m_LoginScene = true;
    public static bool m_DecompressRes = false;
    public static bool isFirst = true;
    //GameObject selfOb;

    public static LoadingProgressBar single;
    public static LoadingProgressBar GetSingle()
    {
        if (single)
            return single;
        else
            return null;
    }
    bool m_isLoaded = false;
    bool m_isGetNextScene = false;
    bool m_isWaiting = false;
    float startGetServerTime;
    public void SetLoaded()
    {
        m_isLoaded = true;
    }
    // public int baseTexture;
    void Awake()
    {

    }

    public void Start()
    {

        //if (m_LoginScene)
        //{
        //    mProcess = transform.FindChild("up").GetComponent<UISlider>();
        //}
        //else
        //{
            Transform mytra = transform;
            mProcess = mytra.FindChild("ProgressBar_liner/up").GetComponent<UISlider>();
            m_backImg = mytra.FindChild("BG").GetComponent<UISprite>();
            //m_roleRunImg = mytra.FindChild("UI_jiazaiObjAnchor/UI_jiazai/Control - Colored Slider/LoadRoleAnimations/LoadRoleAnimation").GetComponent<UISprite>();
            //m_roleRunImg.gameObject.SetActive(roleShow);
            //m_showText = mytra.FindChild("UI_jiazaiObjAnchor/UI_jiazai/Label").GetComponent<UILabel>();
            //m_SpriteList = new UISprite[4];
            //m_SpriteList[0] = m_backImg;
            //m_SpriteList[1] = m_roleRunImg;
            //m_SpriteList[2] = mytra.FindChild("UI_jiazaiObjAnchor/UI_jiazai/Control - Colored Slider").GetComponent<UISprite>();
            //m_SpriteList[3] = mytra.FindChild("UI_jiazaiObjAnchor/UI_jiazai/Control - Colored Slider/Foreground").GetComponent<UISprite>();
            //single = this;
            if (!m_isLoaded)
            {
                //selfOb = gameObject;
                //if (SwitchingScence.GetScence().dengBool)
                //{
                //    Application.LoadLevel("DengLu");
                //}
                //else
                {
                    //DontDestroyOnLoad(selfOb.transform.parent);
                    //Debug.Log(Time.time);

                    //StartCoroutine(LoadScence());
                    //mCurrentNum = 0f;
                }
                RandomBaseTexture();
                startGetServerTime = Time.time + 10f;

            }
        //backImg = FSDataNodeTable<LoadingUINode>.GetSingleton().DataNodeList[baseTexture].UI_type;
        //m_backImg.spriteName = backImg;

        // m_showText.text = showText;
        if (GameLibrary.LastScene == GameLibrary.UI_Major)
        {
            //Debug.Log("<color=#10DF11>scripId</color>" + playerData.GetInstance().guideData.scripId + "<color=#10DF11>typeId</color>" + playerData.GetInstance().guideData.typeId
            //    + "<color=#10DF11>stepId</color>" + playerData.GetInstance().guideData.stepId + "<color=#10DF11>uId</color>" + playerData.GetInstance().guideData.uId);
            playerData.GetInstance().guideData.scripId = 0;
            playerData.GetInstance().guideData.typeId = 0;
            playerData.GetInstance().guideData.stepId = 0;
            playerData.GetInstance().guideData.uId = 0;
            ClientSendDataMgr.GetSingle().GetGuideSend().SendGuidStep(99);
            //Debug.Log("<color=#10DF11>scripId</color>" + playerData.GetInstance().guideData.scripId + "<color=#10DF11>typeId</color>" + playerData.GetInstance().guideData.typeId
            //   + "<color=#10DF11>stepId</color>" + playerData.GetInstance().guideData.stepId + "<color=#10DF11>uId</color>" + playerData.GetInstance().guideData.uId);
        }

        UI_Loading.LoadScene(Globe.LoadScenceName, Globe.LoadTime, Globe.callBack, Globe.completed);
      
        //}
    }

    public void RandomBaseTexture()
    {
        if (isFirst)
        {
            int baseTexture = Random.Range(1, 7);
            //backImg = FSDataNodeTable<LoadingUINode>.GetSingleton().DataNodeList[baseTexture].UI_type;
            Debug.Log("randomCount: " + baseTexture);
            int showLable = Random.Range(1, 3);
            isFirst = false;
        }

        //showText = FSDataNodeTable<LanguageNode>.GetSingleton().DataNodeList[showLable].content;

    }
    // Update is called once per frame
    void Update()
    {

        if (m_roleRunImg && Time.frameCount % 3 == 0)
        {
            m_roleRunImg.spriteName = "UI_loading_pao_" + m_RoleRunIndexer.ToString();
            m_RoleRunIndexer++;
            if (m_RoleRunIndexer > 6)
            {
                m_RoleRunIndexer = 0;
            }
        }
        if (mIsDes)
        {
            if (Time.time - desTime > 0.5f)
            {
                DestroyObject(gameObject);

                //if (SwitchingScence.sceneType != 6)
                //{

                //    //if (SwitchingScence.GetScence().scenceNode.scenceType < 6)
                //    //    CameMoveByPlayer.GetCamera().IsCanMove = true;

                //    CameMoveByPlayer.GetCamera().SetShowRamera();
                //    if (FriendManager.Get().GetFriendCount() > 0 && SwitchingScence.sceneType == 2 || SwitchingScence.sceneType == 7)//zcs f
                //    {
                //        for (int i = 0; i < FriendManager.Get().GetFriendCount(); i++)
                //        {
                //            if (FriendManager.Get().GetIndexFightFriend(i))
                //            {
                //                FriendManager.Get().GetIndexFightFriend(i).SetShowActionNmae("Cchang");
                //            }
                //            // FightFriend.Get().SetShowActionNmae("Cchang");
                //        }
                //    }
                //}
                //else
                //{
                //    CameMoveByPlayer.GetCamera().selfSetPos = true;
                //}
            }
            else
            {
                foreach (UISprite child in m_SpriteList)
                {
                    child.color = new Color(1f, 1f, 1f, 1f - (Time.time - desTime) * 2f * 1f);
                }
                //m_backImg.color = new Color(1f, 1f, 1f, 1f - (Time.time - desTime)*2f * 1f);
                //m_roleRunImg.color = new Color(1f, 1f, 1f, 1f - (Time.time - desTime) * 2f * 1f);
            }
        }
        else
        {
            //Application.LoadLevelAsync(2);
            if (m_LoginScene)
            {
                if (!m_DecompressRes)
                {
                    mCurrentNum += Time.deltaTime * 0.2f;
                    if (mCurrentNum > mMaxNum)
                    {
                        mCurrentNum = mMaxNum;
                        if (!m_isWaiting)
                        {
                            m_isWaiting = true;
                            startGetServerTime = Time.time;
                        }
                    }
                    if (mProcess)
                    {
                        mProcess.value = mCurrentNum;
                    }
                }
            }
            else
            {
                if (!m_isGetNextScene)
                {
                    //if (SwitchingScence.GetScence().m_isSetNextScene)
                    //{
                    //    if (transform.FindChild("UI_Promort/UI_Prompt/0") && transform.FindChild("UI_Promort/UI_Prompt/0").gameObject.activeSelf)
                    //        transform.FindChild("UI_Promort/UI_Prompt/0").gameObject.SetActive(false);

                    //    StartCoroutine(LoadScence());
                    //    mCurrentNum = 0f;
                    //    m_isGetNextScene = true;
                    //    SwitchingScence.GetScence().m_isSetNextScene = false;
                    //}
                    //else
                    //{
                    //    if (Time.time - startGetServerTime > 7f)
                    //    {//超过5秒还是没有获取到服务器发送来的消息
                    //        //  if (transform.FindChild("UI_Promort/UI_Prompt/0"))
                    //        //  transform.FindChild("UI_Promort/UI_Prompt/0").gameObject.SetActive(true);
                    //        ClientNetMgr.GetSingle().Update();
                    //    }
                    //    else
                    //    {
                    //        ClientNetMgr.GetSingle().Update();
                    //    }
                    //}
                }
                //float tempAddNum = Mathf.Max(2f, (mMaxNum - mCurrentNum) * 0.2f);
                mCurrentNum += Time.deltaTime * 0.5f;
                if (mCurrentNum > mMaxNum)
                {
                    mCurrentNum = mMaxNum;
                    if (!m_isWaiting)
                    {
                        m_isWaiting = true;
                        startGetServerTime = Time.time;
                    }
                }
                if (mProcess)
                {
                    mProcess.value = mCurrentNum;
                }
            }
        }
    }

    public static int loadedRoleIndex = 0;

    //public void ReConnect()
    //{
    //    transform.FindChild("UI_Promort/UI_Prompt/0").gameObject.SetActive(false);
    //    //ClientSendDataMgr.GetSingle().GetLoginSend().SendEnterScen(AccountMng.getsingleton().mRoleDataArr[loadedRoleIndex].roleid);
    //    //startGetServerTime = Time.time;

    //    SwitchingScence.GetScence().dengBool = true;
    //    Application.LoadLevel("DengLu");
    //}


    public void SetMaxNum(float num)
    {
        //if (m_roleRunImg && !m_roleRunImg.gameObject.activeSelf)
        //    m_roleRunImg.gameObject.SetActive(true);
        mMaxNum = num;
        //Debug.Log(Time.time + "  " + num);
    }
    bool roleShow = false;
    public void SetRoleRun()
    {
        roleShow = true;
        if (m_roleRunImg)
            m_roleRunImg.gameObject.SetActive(true);
    }
    public void SetProNumText1Decompress(float size)
    {
        m_proNumText1.text = "正在解压资源:";
        mCurrentDownSize += size;
        m_proNumText0.text = mCurrentDownSize.ToString() + "mb/" + mMaxSize.ToString() + "mb";
    }

    public void SetStartProNumText(int maxNum, float maxSize)
    {
        SetTextInfo();
        m_proNumText1.text = "正在下载资源:";
        m_proNumText2.text = "(1/" + maxNum + "）";
        mMaxSize = maxSize;
        m_proNumText0.text = mCurrentDownSize.ToString() + "mb/" + mMaxSize.ToString() + "mb";
    }
    public void SetTextInfo()
    {
        //m_proNumText0 = transform.FindChild("Control - Colored Slider/Label").GetComponent<UILabel>();
        m_proNumText1 = transform.FindChild("ProgressBar_liner/Label1").GetComponent<UILabel>();
        //m_proNumText2 = transform.FindChild("Control - Colored Slider/Label2").GetComponent<UILabel>();
    }
    public void SetCurrentNum(int num, int maxNum)
    {
        mCurrentNum = 0;
        mProcess.value = 0;
        //mProcess.value = (float)num / maxNum;
        m_proNumText1.text = "正在下载资源:";
        m_proNumText2.text = "(" + (num + 1).ToString() + "/" + maxNum.ToString() + "）";
    }

    public void SetCurrentNumCompress(int index, int total)
    {
        if (mProcess)
        {
            mProcess.value = (float)index / total;
            m_proNumText1.text = "正在解压资源:";
            m_proNumText2.text = "(" + index.ToString() + "/" + total.ToString() + "）";
        }
    }

    public float GetCurrentNum()
    {
        return mProcess.value;
    }
    public void SetShowtext(string str)
    {
        //m_showText.text = str;
    }

    public void SetPos(Vector3 pos)
    {
        transform.parent.position = pos;
    }
    //public void ChangePar(Transform parent)
    //{
    //    Transform desTra = transform.parent;
    //    transform.parent = parent;
    //    transform.localPosition = new Vector3(0,0,0);
    //    DestroyObject(desTra.gameObject);
    //}
    bool mIsDes = false;
    float desTime;
    //public void DesPar()
    //{
    //    mIsDes = true;
    //    desTime = Time.time;


    //    if (SwitchingScence.sceneType == 4)
    //    {
    //        MainRoleScence.GetSingle().GetMainUIRootTra().FindChild("UI_Gongneng_A/UI_Gongneng_P/UI_Gongneng/UI_RoleGongNeng").gameObject.SetActive(false);
    //        MainRoleScence.GetSingle().GetMainUIRootTra().FindChild("UI_Gongneng_A/UI_Gongneng_P/UI_Gongneng/UI_RoleGongNeng_NQ").gameObject.SetActive(false);
    //        MainRoleScence.GetSingle().GetMainUIRootTra().FindChild("UI_Gongneng_A/UI_Gongneng_P/UI_Gongneng/UI_OtherGongNeng").gameObject.SetActive(false);
    //    }

    //    if (SwitchingScence.scenceID == 15)
    //    {
    //        MonsterModeNode monsterModel = FSDataNodeTable<MonsterModeNode>.GetSingleton().DataNodeList[510];
    //        GameObject NPCOb;
    //        ResLoad.LoadAndInstanResource("Role/NPC/", monsterModel.monsterModeName, monsterModel.monsterResourceName, out NPCOb,
    //            SelfPlayer.GetSelfPlayer().m_myTraPa.position + Vector3.one * 2.5f,
    //            SelfPlayer.GetSelfPlayer().m_myTraPa.rotation.eulerAngles, false);
    //        //}

    //        NPCOb.transform.parent = Scene_RoleManage.GetSingle().GetNPCCollection();

    //        NPCOb.name = "";

    //        //NPCOb.transform.position = new Vector3(tempNPC.NPCPositionX, tempNPC.NPCPositionY, tempNPC.NPCPositionZ);
    //        //NPCOb.transform.localScale = new Vector3(tempNPC.NPCSize, tempNPC.NPCSize, tempNPC.NPCSize);

    //        NPCOb.tag = "NPC";
    //        //NPCOb.transform.eulerAngles = new Vector3(tempNPC.NPCRositionX, tempNPC.NPCRositionY, tempNPC.NPCRositionZ);
    //        //}
    //        NPC npcInfo = null;
    //        //if (npcObj.GetGameOb() != null)
    //        {
    //            npcInfo = NPCOb.transform.GetChild(0).gameObject.AddComponent<NPC>();
    //            //npcInfo.modleID = npcObj.GetModeID();
    //            //npcInfo.m_scaling = tempNPC.NPCSize;
    //            npcInfo.MyInfo = new CMyObject(1);
    //            npcInfo.MyInfo.SetRoleID(1);
    //            //npcInfo.Animator.Play("Idle");
    //            //npcInfo.SetRoleName(tempNPC.NPCName);
    //            //npcInfo.SetRolePoint(tempNPC.NpcHead);
    //            //NPCList.AddFirst(npcInfo);

    //            npcInfo.mType = NPC.NPCType.FollowNpc;
    //        }
    //        NPCOb.GetComponent<NavMeshAgent>().enabled = true;
    //        NPCOb.GetComponent<NavMeshAgent>().avoidancePriority = 40;
    //        NPCOb.GetComponent<NavMeshAgent>().speed = 6f;



    //        //if (sceneBlockInfo.sceneBlock_type == 2)
    //        //    NPCOb.SetActive(false);
    //    }

    //}
    static AssetBundle bundle;
//    IEnumerator LoadScence()
//    {
//        //        MobileInput();
//#if UNITY_IPHONE || UNITY_ANDROID
//        // if(Handheld)
//        Handheld.ClearShaderCache();//清理显卡
//#endif
//        //Resources.UnloadUnusedAssets();
//        //System.GC.Collect();//清理内存

//        if (bundle)
//            bundle.Unload(false);
//        //bundle = AssetBundle.LoadFromFile(ResLoad.LOCAL_RES_PATH + SwitchingScence.GetScence().GetSceneInfo().scenceLevelResource + ".unity3d");
//        //Debug.Log(SwitchingScence.GetScence().GetSceneInfo().scenceLevelResource + "  " + SwitchingScence.GetScence().scenceName);
//        ////Object scene = bundle.mainAsset;
//        //AsyncOperation async = Application.LoadLevelAsync(SwitchingScence.GetScence().scenceName);

//        //yield return async;
//        //bundle.Unload(false); 
//    }
}
