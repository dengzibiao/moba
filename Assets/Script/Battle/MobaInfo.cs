using System.Collections.Generic;
using UnityEngine;

public class MobaInfo : MonoBehaviour 
{
    public GUISingleButton CloseBtn;
    public MobaKoInfoItem[] RedInfoItems;
    public MobaKoInfoItem[] BlueInfoItems;

    void Awake () 
	{
        CloseBtn.onClick = () => gameObject.SetActive(false);
        for(int i = 0; i < RedInfoItems.Length; i++)
        {
            RedInfoItems[i].gameObject.SetActive(false);
        }
        for(int j = 0; j < BlueInfoItems.Length; j++)
        {
            BlueInfoItems[j].gameObject.SetActive(false);
        }
    }

    void OnEnable ()
    {
        if(SceneMoba3.instance != null)
        {
            RefreshInfo(SceneMoba3.instance.herosDataBlue, SceneMoba3.instance.herosDataRed);
        }
        if(SceneMoba1.instance != null)
        {
            RedInfoItems[0].gameObject.SetActive(true);
            RedInfoItems[0].Refresh(SceneMoba1.instance.HeroDataRed);
            BlueInfoItems[0].gameObject.SetActive(true);
            BlueInfoItems[0].Refresh(SceneMoba1.instance.HeroDataBlue);
        }
    }

    public void RefreshInfo (List<HeroData> blueDatas, List<HeroData> redDatas)
    {
        for(int i = 0; i < redDatas.Count; i++)
        {
            RedInfoItems[i].gameObject.SetActive(true);
            RedInfoItems[i].Refresh(redDatas[i]);
        }
        for(int j = 0; j < blueDatas.Count; j++)
        {
            BlueInfoItems[j].gameObject.SetActive(true);
            BlueInfoItems[j].Refresh(blueDatas[j], j == 0 ? true : false);
        }
    }
}