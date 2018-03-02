using UnityEngine;
using System.Collections;
using Tianyu;

public enum AnimType
{
    None,
    Run,
    Appeared
}

public enum PosType
{
    Hero1,
    Hero2,
    Hero3,
    Hero4,
    Hero5,
    EmbattlePos,
    DetailPos,
    SelectPos,
    TitlePos,
    uisign,
    Lorry,
    heroEffect,
    NpcPos,
    All
}

public class HeroPosEmbattle : MonoBehaviour
{

    public static HeroPosEmbattle instance;

    public Transform HeroPos1;
    public Transform HeroPos2;
    public Transform HeroPos3;
    public Transform HeroPos4;
    public Transform HeroPos5;
    public Transform EmbattlePos;
    public Transform DetailPos;
    public Transform SelectPos;
    public Transform TitlePos;
    public Transform uisign;
    public Transform Chouxiangzi;
    public Transform UI_KaiBaoXiang_01;
    public Transform UI_KaiBaoXiang_Hero_01;
    public Transform npcPos;
    public PosType ty;
    public LotteryType typ;
    public int count;
    public CostType t;
    GameObject model = null;
    Transform parent = null;

    Transform embattleBG;

    public AnimType type;

    public GameObject[] arenaModel;

    void Awake()
    {
        instance = this;
        Chouxiangzi = transform.Find("Chouxiangzi");
        UI_KaiBaoXiang_01 = transform.Find("Chouxiangzi/UI_KaiBaoXiang_01");
        UI_KaiBaoXiang_Hero_01 = transform.Find("Chouxiangzi/UI_KaiBaoXiang_Hero_01");
        embattleBG = transform.Find("embattle_bg");
    }

    public GameObject CreatModel(string goName, PosType pos, SpinWithMouse spin, AnimType type = AnimType.None, int rota = 180, float z = 0)
    {
        this.type = type;
        model = null;
        DestroyModel(pos);
        SetEmbattleBG(pos);
        model = Resource.CreateCharacter(goName, parent.gameObject);
        if (goName == "boss_009")
        {
            model.transform.localPosition = new Vector3(0, 0.2f, 0);
        }
        model.transform.localRotation = Quaternion.Euler(0, rota, 0);
        if (z != 0) model.transform.localPosition = new Vector3(0, 0, z);
        Transform redCircle = model.transform.FindChild("Effect_targetselected01");
        if (redCircle != null)
            redCircle.gameObject.SetActive(false);
        NGUITools.SetLayer(model, LayerMask.NameToLayer("UI"));
        model.tag = Tag.monster;
        //if (pos == PosType.SelectPos)
        //{
        //    model.transform.localScale = model.transform.localScale * 1.4f;
        //}
        GameObject taizi = transform.Find("TitlePos/fazhenxuanren_taizi").gameObject;
        if (taizi != null && pos == PosType.TitlePos)
        {
            model.transform.parent = taizi.transform;
            spin.target = taizi.transform;
        }
        else if (pos == PosType.SelectPos)
        {
            spin.target = SelectPos;
        }
        else if (spin != null)
        {
            spin.target = model.transform;
        }

        if (null != model)
            PlayAppearedAnim(model);
        return model;
    }
    public GameObject CreatModelByModelID(int modelId, PosType pos, SpinWithMouse spin, MountAndPet type, int rota = 180, AnimType animType = AnimType.None)
    {
        this.type = animType;
        model = null;
        DestroyModel(pos);
        SetEmbattleBG(pos);
        if (FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList.ContainsKey(modelId))
        {
            string path = FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList[modelId].respath;
            if (FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList[modelId].respath.Contains("yx_"))
                path = FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList[modelId].respath + "_show";
            model = Instantiate(Resources.Load(FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList[modelId].respath)) as GameObject;
            model.transform.parent = parent.transform;
            model.transform.localPosition = default(Vector3);
            if (Resource.GetNameByPath(modelId) == "boss_009")
            {
                model.transform.localPosition = new Vector3(0, 0.2f, 0);
            }
            model.transform.localRotation = Quaternion.identity;
            model.name = modelId + "";
            model.transform.localRotation = Quaternion.Euler(0, rota, 0);
            model.tag = Tag.monster;
            Transform redCircle = model.transform.FindChild("Effect_targetselected01");
            if (redCircle != null)
                redCircle.gameObject.SetActive(false);
            NGUITools.SetLayer(model, LayerMask.NameToLayer("UI"));

            if (pos == PosType.SelectPos)
            {
                model.transform.localScale = model.transform.localScale * 1.4f;
            }
            if (pos == PosType.NpcPos)
            {
                model.transform.localScale = Vector3.one * FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList[modelId].modelSize;
            }
            GameObject taizi = transform.Find("TitlePos/fazhenxuanren_taizi").gameObject;
            if (taizi != null && pos == PosType.TitlePos)
            {
                model.transform.parent = taizi.transform;
                if (null != spin)
                    spin.target = taizi.transform;
                if (type == MountAndPet.Mount)
                {
                    taizi.transform.localRotation = Quaternion.Euler(-90f, -60f, 0f);
                    model.transform.localScale = Vector3.one * 1.3f;
                }
                else
                {
                    model.transform.localScale = Vector3.one * 2f;
                }
                MountAndPetAnim(model);
            }
            else
            {
                if (null != spin)
                    spin.target = model.transform;
            }
        }


        return model;
    }
    /// <summary>
    /// 播放特效
    /// </summary>
    /// <param name="type"></param>
    /// <param name="tt"></param>
    /// <param name="count"></param>
    /// <param name="_type"></param>
    public void ShowLottryAnimaEffect(PosType type, LotteryType tt, int count, CostType _type)
    {
        switch (type)
        {
            case PosType.Lorry:
                ty = type;
                typ = tt;
                this.count = count;
                t = _type;
                if (!Chouxiangzi.gameObject.activeInHierarchy) Chouxiangzi.gameObject.SetActive(true);
                UI_KaiBaoXiang_01.gameObject.SetActive(true); break;
            case PosType.heroEffect:
                if (!Chouxiangzi.gameObject.activeInHierarchy) Chouxiangzi.gameObject.SetActive(true);
                UI_KaiBaoXiang_Hero_01.gameObject.SetActive(true);
                break;
        }
        this.type = AnimType.None;
    }
    /// <summary>
    /// 播放英雄特效完成
    /// </summary>
    public void LottryHeroEffectHandle()
    {
        UI_KaiBaoXiang_Hero_01.gameObject.SetActive(false);
    }
    /// <summary>
    /// 播放完成
    /// </summary>
    public void LottryAnimaEffectHandle()
    {

        if (typ != LotteryType.None && count != 0 && t != CostType.None)
        {
            ClientSendDataMgr.GetSingle().GetLotterySend().LotteryRequest(typ, this.count, t);
            typ = LotteryType.None;
            count = 0;
            t = CostType.None;
        }
        UI_KaiBaoXiang_01.gameObject.SetActive(false);
        //  HideLottryAnimaEffect();
        //   Control.HideGUI(GameLibrary.UILottryEffect);
        Control.ShowGUI(UIPanleID.UIMoney, EnumOpenUIType.DefaultUIOrSecond);
    }
    /// <summary>
    /// 关闭抽奖场景
    /// </summary>
    public void HideLottryAnimaEffect()
    {
        Chouxiangzi.gameObject.SetActive(false);
    }
    public void HideModel(PosType pos = PosType.All)
    {
        if (pos == PosType.All)
        {
            HeroPos1.gameObject.SetActive(false);
            HeroPos2.gameObject.SetActive(false);
            HeroPos3.gameObject.SetActive(false);
            HeroPos4.gameObject.SetActive(false);
            HeroPos5.gameObject.SetActive(false);
            EmbattlePos.gameObject.SetActive(false);
            DetailPos.gameObject.SetActive(false);
            SelectPos.gameObject.SetActive(false);
            TitlePos.gameObject.SetActive(false);
            uisign.gameObject.SetActive(false);
            npcPos.gameObject.SetActive(false);
        }
        else
        {
            GetPos(pos).gameObject.SetActive(false);
        }
        SetEmbattleBG(PosType.All, false);
        type = AnimType.None;
    }

    public void ShowModel(PosType pos = PosType.All)
    {
        if (pos == PosType.All) return;
        GetPos(pos).gameObject.SetActive(true);
    }

    public void DestroyModel(PosType pos)
    {
        parent = GetPos(pos);

        if (pos == PosType.SelectPos)
        {
            parent.localRotation = Quaternion.Euler(0, 0, 0);
        }

        foreach (Transform tran in parent)
        {
            if (!tran.name.Contains("fazhen") && !tran.name.Contains("Enter"))
            {
                Destroy(tran.gameObject);
            }
            if (tran.name.Contains("Enter"))
            {
                tran.gameObject.SetActive(false);
            }
        }
        //称号和人物详情 要删除台子下的任务模型
        if (pos == PosType.TitlePos)
        {
            Transform tempT = parent.Find("fazhenxuanren_taizi");
            tempT.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            foreach (Transform tran in tempT)
            {
                if (!tran.name.Contains("Enter"))
                {
                    Destroy(tran.gameObject);
                }
                else
                {
                    tran.gameObject.SetActive(false);
                }
            }
        }
        parent.gameObject.SetActive(true);
    }

    Transform GetPos(PosType pos)
    {
        switch (pos)
        {
            case PosType.Hero1: return HeroPos1;
            case PosType.Hero2: return HeroPos2;
            case PosType.Hero3: return HeroPos3;
            case PosType.Hero4: return HeroPos4;
            case PosType.Hero5: return HeroPos5;
            case PosType.EmbattlePos: return EmbattlePos;
            case PosType.SelectPos: return SelectPos;
            case PosType.TitlePos: return TitlePos;
            case PosType.uisign: return uisign;
            case PosType.NpcPos: return npcPos;
            default: return DetailPos;
        }
    }

    Animator ani;
    CharacterState cs;
    int Appeared;
    int idel02;
    void PlayAppearedAnim(GameObject model, HeroAttrNode heroAttr = null)
    {
        ani = model.GetComponent<Animator>();
        cs = UnityUtil.AddComponetIfNull<CharacterState>(model);
        if (type == AnimType.Run)
        {
            ani.SetInteger("Speed", 1);
        }
        else if (type == AnimType.Appeared)
        {
            Appeared = Animator.StringToHash("Base.Chuchang");
            if (ani.HasState(ani.GetLayerIndex("Base"), Appeared))
                ani.SetTrigger("Appeared");
        }
        type = AnimType.None;
        if (heroAttr != null && heroAttr.heroNode != null)
        {
            cs.mAmountDlg = heroAttr.heroNode.dlgAmount;
        }
    }

    void MountAndPetAnim(GameObject go)
    {
        if (null == go) return;
        ani = model.GetComponent<Animator>();
        Appeared = Animator.StringToHash("BaseCw.Idle01");
        idel02 = Animator.StringToHash("BaseCw.Idle02");
        if (ani.HasState(ani.GetLayerIndex("BaseCw"), Appeared) && ani.HasState(ani.GetLayerIndex("BaseCw"), idel02))
        {
            float random = Random.value;
            if (random > 0.5f)
                ani.SetTrigger("Idle");
            else
                ani.SetTrigger("Idle02");
        }
        else if (ani.HasState(ani.GetLayerIndex("BaseCw"), Appeared))
            ani.SetTrigger("Idle");
    }

    public void SetArenaModelPos(bool isTwoToThree)
    {
        arenaModel[0].transform.localPosition = new Vector3(0, isTwoToThree ? 0.287f : -0.348f, -0.41f);
        arenaModel[1].transform.localPosition = new Vector3(0, isTwoToThree ? 0.038f : 0.683f, -0.41f);
    }

    void SetEmbattleBG(PosType pos, bool isEnable = true)
    {
        if (isEnable)
        {
            if (null != embattleBG)
                embattleBG.gameObject.SetActive(TypeIsEmbattle(pos));
        }
        else
        {
            if (null != embattleBG)
                embattleBG.gameObject.SetActive(false);
        }
    }

    bool TypeIsEmbattle(PosType type)
    {
        return type == PosType.EmbattlePos || type == PosType.Hero1 || type == PosType.Hero2 || type == PosType.Hero3 || type == PosType.Hero4 || type == PosType.Hero5;
    }

}
