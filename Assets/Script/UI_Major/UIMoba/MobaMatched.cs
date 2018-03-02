using System.Collections.Generic;
using UnityEngine;

public class MobaMatched : MonoBehaviour 
{
    public ItemHeroLineUp[] myHeros;
    public ItemHeroLineUp[] enemyHeros;

    public static MobaMatched instance;

	void Awake () 
	{
        instance = this;
    }

    public void Init ( HeroData[] myHeroDatas, HeroData[] enemyHeroDatas)
	{
        for(int i = 0; i< myHeroDatas.Length; i++)
        {
            myHeros[i].RefreshUI(myHeroDatas[i], false);
        }
        for(int j = 0; j < enemyHeroDatas.Length; j++)
        {
            enemyHeros[j].RefreshUI(enemyHeroDatas[j], false);
        }
    }
}