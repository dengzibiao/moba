using UnityEngine;
using System.Collections;

public class ArenaItem : MonoBehaviour
{

    public UISprite pIcon;
    public UILabel pLevel;
    public UILabel pName;
    public UILabel pRank;
    public UISprite arrow;

    public void RefreshUI(string icon, int lvl, string name, string rank, bool isShowArr = true)
    {
        gameObject.SetActive(true);
        pIcon.spriteName = icon + "_head";
        pLevel.text = lvl + "";
        pName.text = name;
        pRank.text = rank;
        arrow.enabled = isShowArr;
    }

}
