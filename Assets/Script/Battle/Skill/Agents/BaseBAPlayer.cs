using System;
using UnityEngine;

public class BaseBAPlayer : MonoBehaviour, BAPlayer
{
    public BaseBAPlayer ()
    {
        currentHp = 10000;
        maxHp = 10000;
    }
    public int currentHp
    {
        get; set;
    }

    //public BetterList<BattleEffect> Effects
    //{
    //    get
    //    {
    //        throw new NotImplementedException();
    //    }

    //    set
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public bool isDie
    {
        get
        {
            return false;
        }

        set
        {
            ;
        }
    }

    public int maxHp
    {
        get;
        set;
    }

    public bool moveable
    {
        get
        {
            throw new NotImplementedException();
        }

        set
        {
            throw new NotImplementedException();
        }
    }

    public virtual Vector2 Pos
    {
        get
        {
            throw new NotImplementedException();
        }

        set
        {
            throw new NotImplementedException();
        }
    }

    public float strength
    {
        get
        {
            return 0;
        }

        set
        {
            ;
        }
    }

    public bool CanBeTarget ()
    {
        throw new NotImplementedException();
    }
}