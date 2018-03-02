using Tianyu;
using UnityEngine;

public class UIWorldMap : MonoBehaviour
{
    public GUISingleButton BtnBack;
    public ClickItem[] MapButtons;
    public UIMapContainer MapContainer;

    int mapId;

    void Start()
    {

        MapNode mp;

        BtnBack.onClick = OnBackBtnClick;
        for (int i = 0; i < MapButtons.Length; i++)
        {
            mp = null;

            MapButtons[i].Id = i + 1;

            if (i < playerData.GetInstance().worldMap.Count)
            {
                MapButtons[i].MapId = playerData.GetInstance().worldMap[i];
                mp = FSDataNodeTable<MapNode>.GetSingleton().FindDataByType(playerData.GetInstance().worldMap[i]);
                MapButtons[i].ClickWithId += (int id, int mapid) => OnMapBtnClick(mapid);
                UnityUtil.FindComponent<UISprite>(MapButtons[i].transform, "Sprite").color = new Color(1, 1, 1);
            }
            else
            {
                UnityUtil.FindComponent<UISprite>(MapButtons[i].transform, "Sprite").color = new Color(0, 0, 0);
            }

            if (null == mp)
            {
                UnityUtil.FindComponent<UILabel>(MapButtons[i].transform, "Label").text = "第" + MapButtons[i].Id + "章";
                UnityUtil.FindComponent<UILabel>(MapButtons[i].transform, "Label").color = new Color((float)173 / 255, (float)173 / 255, (float)173 / 255);
            }
            else
            {
                UnityUtil.FindComponent<UILabel>(MapButtons[i].transform, "Label").text = mp.MapName;
            }


        }
    }

    public void OnMapBtnClick(int mapId)
    {
        this.mapId = mapId;
        OnResult();
    }

    void OnBackBtnClick()
    {
        //Control.HideGUI(GameLibrary.UILevel);
        //Control.ShowGUI(GameLibrary.UI_Money);
    }

    public void OnResult()
    {
        MapContainer.RefreshUI(FSDataNodeTable<MapNode>.GetSingleton().FindDataByType(mapId));
        gameObject.SetActive(false);
    }





}