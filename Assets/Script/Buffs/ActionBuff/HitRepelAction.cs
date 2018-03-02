using UnityEngine;
using System.Collections;

public class HitRepelAction : BaseAction
{
    private float mRepelSpeed = 2f;
    public float mMoveDuration = 0f;
    public override void Init(CharacterState cs, float distance, Vector3 dir, float mSpeed)
    {
        base.Init(cs, distance, dir, mSpeed);
        speed = mSpeed != 0 ? mSpeed : mRepelSpeed;
        mMoveDuration = duration <= 0.3f ? 0 : duration  - 0.3f;
    }

    public override void Update()
    {
        base.Update();
        if (mDeltaTime <= mMoveDuration)
        {
            target.pm.FastMove(dir.normalized * Time.deltaTime * speed);
        }
    }
}
