using UnityEngine;
using System.Collections;

public class PlayerInfo
{


    private int level ;             //英雄等级

    private int quality;            //英雄品质

    private int currentStone;       //当前魂石

    private int needStone = 10;     //需要升级魂石

    private int currentExp;         //当前经验



    public int Level
    {
        get
        {
            return level;
        }

        set
        {
            level = value;
        }
    }
    
    public int Quality
    {
        get
        {
            return quality;
        }

        set
        {
            quality = value;
        }
    }

    public int CurrentStone
    {
        get
        {
            return currentStone;
        }

        set
        {
            currentStone = value;
        }
    }

    public int NeedStone
    {
        get
        {
            return needStone;
        }

        set
        {
            needStone = value;
        }
    }

    public int CurrentExp
    {
        get
        {
            return currentExp;
        }

        set
        {
            currentExp = value;
        }
    }

}