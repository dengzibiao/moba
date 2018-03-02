using System;
using System.Collections.Generic;
using Tianyu;
using UnityEngine;

public class UIMapContainer : GUIBase
{
    public GUISingleButton BtnBack;
    public GUISingleButton BtnPrev;
    public GUISingleButton BtnNext;

    public GUISingleButton BtnNormal;
    public GUISingleButton BtnElite;

    public UILabel LaMapName;
    public UILabel LaStarsGot;
    public GUISingleProgressBar PbStarsGot;
    public UILabel PbStarsGotLabel;
    public RewardButton[] RewardButtons;

    //public UISceneEntry SceneEntry;
    UIMap CurrentMap;
    Dictionary<int, int[]> dungeonStar;

    public TipsLevel tips;
    [HideInInspector]
    public bool isOpenElite = true;

    int totalStar = 0;
    int types = 1;//副本类型，1-普通，2-精英

    MapNode map;

    Dictionary<GameObject, RewardButton> rewards = new Dictionary<GameObject, RewardButton>();

    protected override void Init()
    {
        BtnBack.onClick = OnBack;
        BtnPrev.onClick = OnPrevMap;
        BtnNext.onClick = OnNextMap;
        BtnNormal.onClick = ChangeDifficultyToNormal;
        BtnElite.onClick = ChangeDifficultyToElite;
    }

    public void RefreshUI(MapNode map, int sceneType = 1)
    {
        this.map = map;

        if (CheckElite(map))
        {
            SetBtnState(BtnElite.gameObject, true);
            if (null == BtnElite.onClick)
                BtnElite.onClick = ChangeDifficultyToElite;
        }
        else
        {
            SetBtnState(BtnElite.gameObject, false);
            if (null != BtnElite.onClick)
                BtnElite.onClick = null;
        }
        //gameObject.SetActive(true);
        //Show();

        ShowDungeonType(sceneType);
    }

    //精英副本开放条件判断
    bool CheckElite(MapNode map)
    {
        Dictionary<int, int[]> dungeonStar = MapStar(map, 1);
        isOpenElite = true;
        List<int> dungeonStarKey = new List<int>(dungeonStar.Keys);
        for (int i = 0; i < dungeonStarKey.Count; i++)
        {
            if (Globe.GetStar(dungeonStar[dungeonStarKey[i]]) <= 0)
            {
                return false;
            }
        }
        return true;
    }

    Dictionary<int, int[]> MapStar(MapNode map, int index)
    {
        if (map != null)
        {
            Dictionary<int, int[]> star = new Dictionary<int, int[]>();
            if (GameLibrary.mapOrdinary.ContainsKey(map.MapId))
            {
                for (int i = map.MapId + index; i <= map.MapId + map.ordinary.Length * 2; i += 2)
                {
                    if (GameLibrary.mapOrdinary.ContainsKey(map.MapId) && GameLibrary.mapOrdinary[map.MapId].ContainsKey(i))
                        star.Add(i, GameLibrary.mapOrdinary[map.MapId][i]);
                }
            }
            return star;
        }
        return null;
    }

    void ShowDungeonType(int type)
    {
        types = type;
        totalStar = 0;
        dungeonStar = MapStar(map, type);
        List<int> dungStarKey = new List<int>(dungeonStar.Keys);
        for (int i = 0; i < dungStarKey.Count; i++)
        {
            if (Globe.GetStar(dungeonStar[dungStarKey[i]]) > 0)
                totalStar += Globe.GetStar(dungeonStar[dungStarKey[i]]);
        }
        SetNormalOrEliteBtnEffect(type == 1 ? BtnNormal.GetComponent<UISprite>() : BtnElite.GetComponent<UISprite>());
        RefreshMap(map, dungeonStar);
    }

    void RefreshMap(MapNode map, Dictionary<int, int[]> dungeonStar)
    {
        if (CurrentMap != null)
        {
            DestroyImmediate(CurrentMap.gameObject);
            CurrentMap = null;
        }
        if (null == CurrentMap)
        {
            GameObject mapPrefab = Resources.Load<GameObject>("Prefab/Maps/Map" + (types == 1 ? map.MapId : map.MapId + 1));
            CurrentMap = NGUITools.AddChild(gameObject, mapPrefab).GetComponent<UIMap>();
        }
        LaMapName.text = map.MapName;
        CurrentMap.MapContainer = this;
        SetPrevAndNextBtn();
        RefreshBox(map);
    }

    public void SetPrevAndNextBtn()
    {
        //副本修改 
        if (CurrentMap != null && map != null && dungeonStar != null)
        {
            bool isNext = CurrentMap.RefreshUI(map, dungeonStar, types);
            SetSwitchBtn(map, true);//临时测试4章全开，
        }
    }

    void RefreshBox(MapNode map)
    {
        int boxID = types + map.MapId;
        if (!GameLibrary.mapBox.ContainsKey(boxID)) return;
        int[][] box = new int[GameLibrary.mapBox[boxID].Length][];
        for (int i = 0; i < box.Length; i++)
        {
            box[i] = new int[GameLibrary.mapBox[boxID][i].Length];
            for (int j = 0; j < box[i].Length; j++)
            {
                box[i][j] = GameLibrary.mapBox[boxID][i][j];
            }
        }

        int starCount  = (types == 1 ? map.ordinary.Length : map.elite.Length) * 3;
        int barStarCount = totalStar - box[0][0] < 0 ? 0 : totalStar - box[0][0];

        PbStarsGotLabel.text = "[2dd740]" + totalStar + "[-]/" + starCount;
        PbStarsGot.InValue(barStarCount, starCount - box[0][0]);

        for (int i = 0; i < RewardButtons.Length; i++)
        {
            RewardButtons[i].boxIndex = i == 0 ? i + 1 : i + 2;
            RewardButtons[i].Sprite.spriteName = "baox" + RewardButtons[i].boxIndex + "_close";
            RewardButtons[i].Label.text = "" + box[i][0];
            RewardButtons[i].info = box[i];
            RewardButtons[i].index = i;
            RewardButtons[i].OnPressClick = null;
            if (null == RewardButtons[i].boxEffect)
                RewardButtons[i].boxEffect = RefreshBoxEffect(RewardButtons[i].Sprite.gameObject);
            RewardButtons[i].SetEffectActive(false);
            if (box[i][5] == 0 && RewardButtons[i].Sprite.spriteName != "baox" + RewardButtons[i].boxIndex + "_open")
            {
                RewardButtons[i].OnPressClick += OnBoxPress;
                if (!rewards.ContainsKey(RewardButtons[i].gameObject))
                    rewards.Add(RewardButtons[i].gameObject, RewardButtons[i]);
                if (totalStar >= box[i][0])
                    RewardButtons[i].SetEffectActive(true);
            }
            else
            {
                RewardButtons[i].Sprite.spriteName = "baox" + RewardButtons[i].boxIndex + "_open";
            }
        }
    }

    GameObject boxGO;
    int[] boxInfo;
    int boxIndex = 0;

    void OnBoxPress(GameObject go, bool isPress, int[] info, int index)
    {
        if (isPress)
        {
            if (totalStar >= info[0])
            {
                boxGO = go;
                boxInfo = info;
                boxIndex = index;
                ClientSendDataMgr.GetSingle().GetBattleSend().SendDrawDungeonBoxReward(map.MapId, 0, types, info[0]);
            }
            else
            {
                tips.RefreshUI(info);
            }
        }
        else
        {
            tips.CloseUI();
        }
    }

    public void DrawDungeonBoxReward()
    {
        if (rewards.ContainsKey(boxGO))
        {
            rewards[boxGO].OnPressClick -= OnBoxPress;
            rewards[boxGO].Sprite.spriteName = "baox" + rewards[boxGO].boxIndex + "_open";
            rewards[boxGO].SetEffectActive(false);
        }
        else
        {
            Debug.Log("rewards Do not contain " + boxGO);
        }
        if (boxInfo[3] != 0 && (int.Parse(StringUtil.SubString(boxInfo[3].ToString(), 3)) == 107))
        {
            int heroId = int.Parse(201 + StringUtil.SubString(boxInfo[3].ToString(), 6, 3));
            object[] ob = new object[4] { heroId, ShowHeroEffectType.StarEvaluationChest, playerData.GetInstance().AddHeroToList(heroId) ? HeroOrSoul.Hero : HeroOrSoul.Soul, 0 };
            Control.ShowGUI(UIPanleID.UILottryHeroEffect, EnumOpenUIType.DefaultUIOrSecond, false, ob);
        }
        else
        {
            OpenRewards();
        }
        (GameLibrary.mapBox[types + map.MapId])[boxIndex][5] = 1;
    }

    void OpenRewards()
    {
        ItemData tempData = new ItemData();
        TaskManager.Single().itemlist.Clear();
        if (boxInfo[1] != 0)
        {
            tempData.GradeTYPE = GradeType.Purple;
            tempData.Name = "金币";
            tempData.Describe = "金币";
            tempData.Count = boxInfo[1];
            tempData.IconName = "jinbi";
            tempData.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");

        }
        else if (boxInfo[2] != 0)
        {
            tempData.GradeTYPE = GradeType.Orange;
            tempData.Name = "钻石";
            tempData.Describe = "钻石";
            tempData.IconName = "zuanshi";
            tempData.Count = boxInfo[2];
            tempData.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
        }
        else if (boxInfo[3] != 0)
        {
            ItemNodeState item = GameLibrary.Instance().ItemStateList[boxInfo[3]];
            tempData.GradeTYPE = (GradeType)item.grade;
            tempData.Name = item.name;
            tempData.Types = item.types;
            tempData.Describe = item.describe;
            tempData.IconName = item.icon_name;
            tempData.Count = boxInfo[4];
            if (item.types == 6 || item.types == 7)
                tempData.UiAtlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
            else
                tempData.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");

        }
        TaskManager.Single().itemlist.Add(tempData);
        Control.ShowGUI(UIPanleID.UITaskRewardPanel, EnumOpenUIType.DefaultUIOrSecond);
    }

    public void ShowSceneEntry(SceneNode scene)
    {
        object[] openParams = new object[] { scene, OpenSourceType.Dungeons };
        Control.ShowGUI(UIPanleID.SceneEntry, EnumOpenUIType.DefaultUIOrSecond, false, openParams);

        //UISceneEntry sceneEntry = Control.ShowGUI(GameLibrary.SceneEntry) as UISceneEntry;
        //sceneEntry.type = OpenSourceType.Dungeons;
        //sceneEntry.RefreshUI(scene);
        //if (null == SceneEntry)
        //    SceneEntry = sceneEntry;
    }

    public void SetSwitchBtn(MapNode map, bool isNext)
    {
        SetBtnState(BtnPrev.gameObject, playerData.GetInstance().worldMap.Contains(map.MapId) && map.MapId == 100 ? false : true);
        if (types == 1)
            SetBtnState(BtnNext.gameObject, playerData.GetInstance().worldMap.Contains(map.MapId + 100) && isNext);
        else
            SetBtnState(BtnNext.gameObject, playerData.GetInstance().worldMap.Contains(map.MapId + 100) && CheckElite(FSDataNodeTable<MapNode>.GetSingleton().FindDataByType(map.MapId + 100)));
    }

    void SetBtnState(GameObject btn, bool isGrip)
    {
        ChangeColorGray.Instance.ChangeSpriteColor(btn.GetComponent<UISprite>(), isGrip);
        if (btn.GetComponent<BoxCollider>())
            btn.GetComponent<BoxCollider>().enabled = isGrip;
    }

    void OnPrevMap()
    {
        int prevID = CurrentMap.MapData.MapId - 100;
        RefreshUI(FSDataNodeTable<MapNode>.GetSingleton().FindDataByType(prevID), types);
    }

    void OnNextMap()
    {
        //章节开放
        int prevID = CurrentMap.MapData.MapId + 100;

        if (DataDefine.isSkipFunction || FunctionOpenMng.GetInstance().GetFunctionOpen(prevID / 100) && prevID / 100 <= 6)
        {
            RefreshUI(FSDataNodeTable<MapNode>.GetSingleton().FindDataByType(prevID), types);
        }
        else
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[prevID / 100].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }

    }
    /// <summary>
    /// 普通副本按钮
    /// </summary>
    void ChangeDifficultyToNormal()
    {
        if (types == 1)
            return;
        types = 1;
        ShowDungeonType(types);
    }
    //精英副本按钮
    public void ChangeDifficultyToElite()
    {
        if (types == 2)
            return;
        types = 2;
        ShowDungeonType(types);
    }

    void OnBack()
    {
        UILevel.instance.SaveMapID(map.MapId, types);
        //Control.HideGUI(GameLibrary.UILevel);
        UILevel.instance.HideUILevel();
        Control.ShowGUI(UIPanleID.UIMoney, EnumOpenUIType.DefaultUIOrSecond);
        Control.PlayBGmByClose(UIPanleID.UILevel);
    }


    #region 添加UI特效
    GameObject RefreshBoxEffect(GameObject parent)
    {
        GameObject _effect = null;
        _effect = Resource.CreatPrefabs("UI_FBBX_01", parent, Vector3.zero, GameLibrary.Effect_UI);
        if (null != _effect)
            SetItemEffect(_effect, parent.GetComponent<UISprite>());
        return _effect;
    }

    GameObject btnEffect;
    void SetNormalOrEliteBtnEffect(UISprite parent)
    {
        if (null == btnEffect)
            btnEffect = Resource.CreatPrefabs("UI_FBJP_01", parent.gameObject, Vector3.zero, GameLibrary.Effect_UI);
        SetEffectDepth(btnEffect, parent);
    }

    public void SetEffectDepth(GameObject effect, UISprite parent)
    {
        if (null != effect)
        {
            effect.transform.parent = parent.transform;
            effect.transform.localPosition = Vector3.zero;
            effect.GetComponent<RenderQueueModifier>().m_target = parent;
            effect.GetComponent<RenderQueueModifier>().GetRender();
        }
    }

    public void SetItemEffect(GameObject effect, UISprite parent)
    {
        if (null != effect)
        {
            RenderQueueModifier[] render = effect.GetComponentsInChildren<RenderQueueModifier>();
            for (int i = 0; i < render.Length; i++)
            {
                render[i].m_target = parent;
                render[i].GetRender();
            }
        }
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }


    #endregion



}