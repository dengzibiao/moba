using UnityEngine;
using System.Collections;

public class HitReAction : BaseAction
{
    private float mMoveDuration = -1f;
    public override void Init(CharacterState cs, float duration, Vector3 dir, float mSpeed)
    {
        if (mMoveDuration != -1f)
        {
            SetTargetEnable(true);
        }
        base.Init(cs, duration, dir, mSpeed);
        speed = 4;
        mMoveDuration = duration;
    }

    public override void Update()
    {
        base.Update();
        if (mDeltaTime <= mMoveDuration)
        {
            target.pm.FastMove(dir.normalized * Time.deltaTime * speed, false);
        }
        if (mDeltaTime >= mMoveDuration)
        {
            SetTargetEnable(true);
            mMoveDuration = -1f;
        }
    }
}
