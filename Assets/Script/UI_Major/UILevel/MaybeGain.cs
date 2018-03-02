using UnityEngine;
using System.Collections;
using System;
using Tianyu;

public class MaybeGain : MonoBehaviour
{

    public UISprite icon;
    public UISprite borderS;
    public UISprite soulStone;

    GameObject goodsTips;
    UISceneEntry sceneEnter;
    int[] infos;
    ItemNodeState item;

    public void Init(object info)
    {
        sceneEnter = transform.parent.parent.parent.GetComponent<UISceneEntry>();
        goodsTips = sceneEnter.goodsTips.gameObject;

        infos = new int[((int[])info).Length];
        for (int i = 0; i < infos.Length; i++)
        {
            infos[i] = ((int[])info)[i];
        }

        item = GameLibrary.Instance().ItemStateList[long.Parse(((int[])info)[0].ToString())];

        if (item.types == 6)
        {
            icon.atlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
        }

        ItemData.SetAngleMarking(soulStone, item.types);
        icon.spriteName = item.icon_name;
        borderS.spriteName = ItemData.GetFrameByGradeType((GradeType)item.grade);
    }

    void OnPress(bool isShow)
    {
        if (isShow)
            goodsTips.GetComponent<GoodsTips>().RefreshUI(item, infos[1]);
        else
            goodsTips.SetActive(false);
    }

}
