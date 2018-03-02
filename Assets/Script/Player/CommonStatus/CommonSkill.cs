using UnityEngine;
using System.Collections;

public class CommonSkill : CommonSkillBase
{
    public CommonSkill(ANIM_INDEX[] animQueue, float[] animSpeed=null):base(animQueue, animSpeed)
    {
    }

    public override STATUS OnAnimationEnd ()
    {
        return base.OnAnimationEnd ();
    }
}
