using UnityEngine;
using Tianyu;
using System.Collections.Generic;

public class UIMap : GUIBase
{
    public SceneButton[] SceneButtons;
    public UIMapContainer MapContainer;
    public MapNode MapData;
    GameObject itemEffect;
    bool isSetEffect;

    void InitHide()
    {
        for (int i = 0; i < SceneButtons.Length; i++)
        {
            SceneButtons[i].gameObject.SetActive(false);
        }
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    public bool RefreshUI(MapNode map, Dictionary<int, int[]> dungeonStar, int types = 1)
    {
        InitHide();
        MapData = map;
        gameObject.SetActive(true);
        isSetEffect = true;
        if (null != itemEffect)
            itemEffect.SetActive(false);
        int star = 0;
        int sceneIndex = 0;
        int lastIndex = 0;

        for (int i = (MapData.MapId + types); i <= (MapData.MapId + map.ordinary.Length * 2); i += 2)
        {
            if (!dungeonStar.ContainsKey(i)) continue;
            star = i;
            SceneButtons[sceneIndex].gameObject.SetActive(true);
            if (null != dungeonStar && dungeonStar.Count > 0)
            {
                if (Globe.GetStar(dungeonStar[star]) >= 0)
                {
                    if (null == SceneButtons[sceneIndex].GetComponent<ClickItem>().ClickGo)
                        SceneButtons[sceneIndex].GetComponent<ClickItem>().ClickGo += OnClickSceneBtn;
                }
                SceneButtons[sceneIndex].RefreshUI(FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(star), Globe.GetStar(dungeonStar[star]));

                if (Globe.GetStar(dungeonStar[star]) == 0)
                {
                    SetItemEffect(SceneButtons[sceneIndex].Icon.enabled ? SceneButtons[sceneIndex].Icon : SceneButtons[sceneIndex].BgIcon);
                }
                
            }
            else
            {
                SceneButtons[sceneIndex].RefreshUI(FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(star));
            }
            lastIndex = star;
            sceneIndex++;
        }
        return star != 0 && null != dungeonStar && Globe.GetStar(dungeonStar[star]) > 0;
    }

    void OnClickSceneBtn(GameObject go)
    {
        MapContainer.ShowSceneEntry(go.GetComponent<SceneButton>().SceneData);
    }

    public static string SceneIcon(int type)
    {
        switch (type)
        {
            case 1: return "KM";
            case 2: return "MOBA";
            case 3: return "TP";
            case 4: return "KV";
            case 5: return "TD";
            case 6: return "ES";
            default:
                return "BOSS";
        }
    }
    protected override void RegisterComponent()
    {
        base.RegisterComponent();

        RegisterComponentID(10, 92, SceneButtons[0].gameObject);

        if (playerData.GetInstance().guideData.uId == 1092)
        {
            if (UIGuidePanel.Single() != null)
                UIGuidePanel.Single().InitGuide();
        }


    }
    protected override void RegisterIsOver()
    {
        base.RegisterIsOver();
    }

    void SetItemEffect(UISprite parent)
    {
        isSetEffect = false;
        if (null == itemEffect)
            itemEffect = Resource.CreatPrefabs("UI_FBTX_02", parent.gameObject, Vector3.zero, GameLibrary.Effect_UI);
        MapContainer.SetItemEffect(itemEffect, parent);
        if (null != itemEffect)
            itemEffect.SetActive(true);
    }

}