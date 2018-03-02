using System;
using UnityEngine;

public class CreateAreaEffect
{
    public AnimationCurve OffsetCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 150f) });
    public AnimationCurve ScaleCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 150f) });

    BattleRange EffectArea;


}