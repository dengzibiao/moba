using System;

public interface BABreakable : BattleAgent
{
    int currentHp { get; set; }
    //bool isDie { get; set; }
    //BetterList<BattleEffect> Effects { get; set; }
    bool CanBeTarget ();
}