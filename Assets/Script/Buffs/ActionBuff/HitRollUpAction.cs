using UnityEngine;
public class HitRollUpAction : BaseAction
{
    private bool isChangeDir;
    private float mStayTime;
    private float mVerticalSpeed = 2f;
    private float mVerticalTime = 0.3f;
    private float mMoveSpeed;

    public override void Init(CharacterState cs, float duration, Vector3 dir, float mSpeed)
    {
        base.Init(cs, duration, dir, mSpeed);
        speed = mVerticalSpeed;
        isChangeDir = true;
        mStayTime = duration - mVerticalTime;
    }

    public override void Update()
    {
        base.Update();
        if (mDeltaTime <= duration)
        {
            if (mDeltaTime <= mVerticalTime)
            {
                mMoveSpeed = speed;
            }
            else if (mDeltaTime > mVerticalTime && mDeltaTime < mStayTime)
            {
                mMoveSpeed = 0;
            }
            else if (mDeltaTime >= mStayTime && isChangeDir)
            {
                mMoveSpeed = speed;
                dir = -dir;
                isChangeDir = false;
            }
            transform.Translate(dir * Time.deltaTime * mMoveSpeed, Space.World);
            transform.Rotate(new Vector3(0, 720 * Time.deltaTime, 0));
        }
        else
        {
            ResetPosition();
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
