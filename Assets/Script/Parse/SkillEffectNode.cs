using System;
using Tianyu;
using System.Collections.Generic;

public enum SkillEffectType
{
    ChangeValue,
    CreateArea,
    CreateObject
}

public class SkillEffectNode : FSDataNodeBase
{
    public long id;
    public int effectTiming;
    public SkillEffectType effectType;
    public string config;

    public override void parseJson ( object jd )
    {
        Dictionary<string, object> items = (Dictionary<string, object>)jd;

        config = ( items["effects"] ).ToString();
    }

}