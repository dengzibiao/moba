using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Tianyu;



public class DifficSelectData
{

    private static DifficSelectData mSingleton;
    public static DifficSelectData Instance()
    {
        if (mSingleton == null)
            mSingleton = new DifficSelectData();
        return mSingleton;
    }
    public Dictionary<int, bool> mapID = new Dictionary<int, bool>();

}
public class UIActDifficSelect : GUIBase
{

    public UIActivity activity;
    public GUISingleButton BackBtn;
    public UILabel PromptLabel;
    public UISceneEntry sceneEnter;
    public DifficultyItem difficultyItem;
    public GUISingleMultList DifficMultList;
    public static List<SceneNode> itemRankList = new List<SceneNode>();
    public int[] star;
    private static UIActDifficSelect mSingleton;
    public Transform view;
    public static UIActDifficSelect Instance()
    {
        if (mSingleton == null)
            mSingleton = new UIActDifficSelect();
        return mSingleton;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    protected override void Init()
    {
        BackBtn.onClick = OnBackBtnClick;
        view = transform.Find("Scroll View").transform;
    }

    public void RefreshUI(MapNode map)
    {
        itemRankList.Clear();
        SceneNode scene = null;
        if (map != null)
        {
            List<int[]> star = GameLibrary.eventsList[(map.MapId - 30000) / 100];
            for (int i = 0; i < map.ordinary.Length; i++)
            {
                scene = FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(map.ordinary[i]);

                int a = 0;
                if (i > 0)
                {
                    a = i - 1;
                }
                if (Globe.GetStar(star[a]) >= 0 && scene.pass_lv <= playerData.GetInstance().selfData.level)
                {
                    //DifficSelectData.Instance().mapID.Add(map.MapId + i + 1, false);
                    scene.isOpened = true;
                }
                else
                {
                    scene.isOpened = false;
                    //DifficSelectData.Instance().mapID.Add(map.MapId + i + 1, false);
                }
                itemRankList.Add(scene);
            }
        }
        BackBtn.transform.Find("Label").GetComponent<UILabel>().text = map.MapName;
        switch ((map.MapId - 30000) / 100)
        {
            case 1: PromptLabel.text = "此模式下只能使用男性英雄"; break;
            case 2: PromptLabel.text = "此模式下只能使用女性英雄"; break;
            case 3: PromptLabel.text = "此模式下只能使用力量英雄"; break;
            case 4: PromptLabel.text = "此模式下只能使用敏捷英雄"; break;
            case 5: PromptLabel.text = "此模式下只能使用智力英雄"; break;
        }

        DifficMultList.InSize(map.ordinary.Length, map.ordinary.Length);
        DifficMultList.Info(itemRankList.ToArray());
    }
    void OnBackBtnClick()
    {
        activity.ActModeSelect.gameObject.SetActive(true);
        Hide();

    }

    protected override void OnRelease()
    {
        base.OnRelease();
        view.GetComponent<UIScrollView>().ResetPosition();
    }

}
