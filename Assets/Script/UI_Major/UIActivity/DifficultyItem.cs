using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Tianyu;


public class DifficultyItem : GUISingleItemList
{

    public delegate void OnDifficultyItemClick(int sceneID, bool isDisplay);
    public OnDifficultyItemClick OnItemClicks;

    //public UISprite DifficSprite;
    public UILabel DesLabel;
    public List<UISprite> starList = new List<UISprite>();
    public UILabel DifficLabel;
    public UILabel name;
    public GUISingleButton ItemBtn;
    public UISprite Icon;
    public UISprite titleBg;
    public int sceneID;
    public int mapID;
    List<int[]> star;
    int sceneStar;
    bool isDisplay = false;

    SceneNode sceneNode;

    UIActivity activity;

    protected override void InitItem()
    {
        activity = GetComponentInParent<UIActivity>();
        ItemBtn.onClick = OnItemClick;
    }
    public void SetState(bool isOpened, SceneNode scene, int star)
    {

    }
    public override void Info(object obj)
    {
        //Debug.Log(((SceneNode)obj).SceneName);
        sceneNode = (SceneNode)obj;
        if (((SceneNode)obj).SceneName == "简单")
        {
            DifficLabel.text = "[83FFC0FF]" + ((SceneNode)obj).SceneName;
        }
        else if (((SceneNode)obj).SceneName == "普通")
        {
            DifficLabel.text = "[51ABE8FF]" + ((SceneNode)obj).SceneName;
        }
        else if (((SceneNode)obj).SceneName == "困难")
        {
            DifficLabel.text = "[B256DFFF]" + ((SceneNode)obj).SceneName;
        }
        else if (((SceneNode)obj).SceneName == "大师")
        {
            DifficLabel.text = "[F65F5FFF]" + ((SceneNode)obj).SceneName;
        }
        //Icon.spriteName = UIActivity.instance.ModulIcon(((SceneNode)obj).bigmap_id);
        Icon.spriteName = TypeIcon(((SceneNode)obj).SceneName);
        titleBg.spriteName = TypeTitleBg(((SceneNode)obj).SceneName);
        mapID = ((SceneNode)obj).bigmap_id;
        star = GameLibrary.eventsList[(((SceneNode)obj).bigmap_id - 30000) / 100];
        sceneStar = Globe.GetStar(star[sceneNode.SceneId - sceneNode.bigmap_id - 1]);
        if (((SceneNode)obj).isOpened)
        {
            sceneID = ((SceneNode)obj).SceneId;

            DesLabel.gameObject.SetActive(false);
            Icon.color = new Color(1, 1, 1);

            for (int i = 0; i < starList.Count; i++)
            {
                starList[i].enabled = true;
                starList[i].spriteName = i + 1 <= sceneStar ? "xing" : "xing-hui";
            }
            isDisplay = true;
        }
        else
        {
            DesLabel.gameObject.SetActive(true);
            DesLabel.text = "战队等级" + ((SceneNode)obj).pass_lv + "开启";
            Icon.color = new Color(0, 0, 0);
            for (int i = 0; i < starList.Count; i++)
            {
                starList[i].enabled = false;
            }
            isDisplay = false;
        }

    }

    void OnItemClick()
    {
        if (!isDisplay)
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "战队等级未达到");
            return;
        }
        object[] openParams = new object[] { FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(sceneID), activity.type };
        Control.ShowGUI(UIPanleID.SceneEntry, EnumOpenUIType.DefaultUIOrSecond, false, openParams);
    }
    private string TypeIcon(string nameStr)
    {
        switch (nameStr)
        {
            case "简单":
                return "jiandan";
            case "普通":
                return "putong";
            case "困难":
                return "kunnan";
            case "大师":
                return "dashi";
            default:
                return null;
        }
    }
    private string TypeTitleBg(string nameStr)
    {
        switch (nameStr)
        {
            case "简单":
                return "jiandandi";
            case "普通":
                return "putongdi";
            case "困难":
                return "kunnandi";
            case "大师":
                return "dashidi";
            default:
                return null;
        }
    }
}