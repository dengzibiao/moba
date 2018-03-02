using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIArenaPanel : MonoBehaviour
{
    public UILabel mName;
    public UILabel dName;
    public List<TeamIcon> mIcon = new List<TeamIcon>();
    public List<TeamIcon> dIcon = new List<TeamIcon>();
    public BattleCDandScore ArenaCD;


    public void RefreshName(int lvl, string name)
    {
        mName.text = playerData.GetInstance().selfData.level + "级 " + playerData.GetInstance().selfData.playeName;
        dName.text = lvl + "级 " + name;
    }

    public void SetArenaCD(int time = 300)
    {
        ArenaCD.StartCD(time);
    }
}
