using UnityEngine;
using System.Collections;

public class EffectFollowTarget : MonoBehaviour
{
    private CharacterState mFollowTarget;

    public void init(CharacterState target, float destoryTime)
    {
        mFollowTarget = target;
        Destroy(gameObject, destoryTime);
    }

    void Update()
    {
        if (mFollowTarget == null)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position = mFollowTarget.mHitPoint.position;
        }
    }
}
