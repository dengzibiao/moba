using UnityEngine;
using System.Collections;

public class HithangAction : BaseAction
{
    private float mVerticalSpeed = 4f;
    private float mVerticalTime = 0.3f;
    public float mMoveSpeed;

    public override void Init(CharacterState cs, float duration, Vector3 dir, float mSpeed)
    {
        base.Init(cs, duration, dir, mSpeed);
        speed = mVerticalSpeed;
    }

    public override void Update()
    {
        base.Update();
        if (mDeltaTime <= mVerticalTime * 2)
        {
            mMoveSpeed = speed * (mVerticalTime - mDeltaTime) / mVerticalTime;
            transform.Translate(dir * Time.deltaTime * mMoveSpeed, Space.World);
        }
        else
        {
            if (nav != null)
            {
                nav.enabled = true;
            }
        }
    }

    public override void SetTargetEnable(bool b)
    {
        base.SetTargetEnable(b);
        if (nav != null)
        {
            nav.enabled = b;
        }
        if (b)
        {
            ResetPosition();
        }
    }
}
