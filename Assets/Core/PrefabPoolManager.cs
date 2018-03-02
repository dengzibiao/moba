using UnityEngine;
using System.Collections;
using PathologicalGames;
using System.Collections.Generic;

public class PrefabPoolManager
{
    private SpawnPool spawnPool;
    private BetterList<string> mPoolList = new BetterList<string>();

    public PrefabPoolManager ()
    {
        spawnPool = PoolManager.Pools.Create ("PoolManager");
        spawnPool.dontReparent = false;
    }

    public PrefabPoolManager(string poolName)
    {
        spawnPool = PoolManager.Pools.Create(poolName);
        spawnPool.dontReparent = false;
    }

    public PrefabPoolManager (string[] strings) : this()
    {
        AddPrefabPools (strings);
    }

    public void AddPrefabPools (string[] strings)
    {
        for (int i = 0; i < strings.Length; i++)
        {
            AddPrefabPool(strings[i]);
        }
    }

    public void AddPrefabPool (string s, int amount = 5)
    {
        if (mPoolList == null || !mPoolList.Contains(s)) {
            PrefabPool prePool = GetPrefabPool (s, amount);
            if (prePool != null)
            {
                spawnPool._perPrefabPoolOptions.Add(prePool);
                spawnPool.CreatePrefabPool(prePool);
            }
        }
    }
    
    PrefabPool GetPrefabPool (string s, int amount)
    {
        PrefabPool ret = null;
        Transform t = null;
        t = Resources.Load<Transform>(s);
        if (t != null)
        {
            ret = new PrefabPool(t);
            ret.preloadAmount = amount;
            ret.limitAmount = amount;
            ret.cullDespawned = true;
            ret.cullAbove = amount;
            ret.cullDelay = 2;
            ret.cullMaxPerPass = 2;
            ret._logMessages = false;
            mPoolList.Add(s);
            if (!mPoolList.Contains(s))
                mPoolList.Add(s);
        }
        return ret;
    }

    public GameObject Spawn (string s)
    {
        if (!spawnPool.prefabs.ContainsKey(s))
            return null;
        return spawnPool.Spawn(s).gameObject;
    }

    public void Despawn (Transform t)
    {
        if (!spawnPool.IsSpawned (t)) {
            GameObject.Destroy(t.gameObject);
            return ;
        }
        if (t != null)
            t.parent = spawnPool.transform;
        spawnPool.Despawn (t);
    }

    public void DespawnAll ()
    {
        spawnPool.DespawnAll ();
    }
}

