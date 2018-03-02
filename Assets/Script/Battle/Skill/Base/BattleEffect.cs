using System;

public enum BattleEffectType
{
	physics,
	magic
}

public interface BattleEffect 
{
	void Cast(BattleAgent target, BattleAgent caster);
	void Reverse ();
    int EffectTiming();
    SkillEffectNode GetNode();
}