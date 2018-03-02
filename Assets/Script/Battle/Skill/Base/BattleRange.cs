using System;
using System.Reflection;
using UnityEngine;


public enum BattleRangeType
{
    rect = 0,
    sector = 1,
}

public abstract class BattleRange
{
    public AnimationCurve OffsetCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 150f) });
    public AnimationCurve ScaleCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 150f) });
    public AnimationCurve RotateCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 150f) });

    public BattleRangeType RangeType;
    public Vector2 OffsetCenter = new Vector2(0f, 0f);
    public float Size = 0f;

    public abstract bool InRange ( Vector2 targetPos, Vector2 basePos);

    public BattleRange ( float size)
    {
        Size = size;
    }

    public BattleRange ( float size, Vector2 offCenter )
    {
        Size = size;
        OffsetCenter = offCenter;
    }

    public BattleRange (string[] config)
    {
        Size = GetParam(config, 0);
        OffsetCenter = new Vector2(GetParam(config, 1), GetParam(config, 2));
    }

    protected float GetParam ( string[] config, int index)
    {
        if(config.Length > index)
        {
            return float.Parse(config[index]);
        }
        return 0f;
    }
}

public class RectRange : BattleRange
{
    public float Ysize = 0f;
    public float Xstart = 0f;
    public float Ystart = 0f;
    
    public RectRange ( float size ) : base(size)
    {
        Ysize = size;
    }

    public RectRange ( float size, Vector2 offCenter ) : base(size, offCenter)
    {
        Ysize = size;
    }

    public RectRange ( string[] config ) : base(config)
    {
        float ySize = GetParam(config, 3);
        Ysize = ySize > 0f ? ySize : Size;
        Xstart = GetParam(config, 4);
        Ystart = GetParam(config, 5);
     //   if(Xstart >= Size || Ystart > Ysize)
           // Debug.LogError("Wrong Range Size Start Config");
    }

    public override bool InRange ( Vector2 targetPos , Vector2 basePos)
    {
        Vector2 pos = basePos + OffsetCenter;
        return (( targetPos.x < pos.x + 0.5f * Size && targetPos.x > pos.x + 0.5f * ( Size - Xstart )) || ( targetPos.x > pos.x - 0.5f * Size && targetPos.x < pos.x + 0.5f * ( Xstart - Size ))) && (( targetPos.y < pos.y + 0.5f * Ysize && targetPos.y > pos.y + 0.5f * ( Ysize - Ystart ) ) || ( targetPos.y > pos.y - 0.5f * Ysize && targetPos.y < pos.y + 0.5f * ( Ystart - Ysize ) ));
    }
}

public class SectorRange : BattleRange
{
    public float Angle = 360f;
    public float SizeStart = 0f;
    public Vector2 Direction = new Vector2(0f, 0f);

    public SectorRange ( float size) : base(size)
    {
    }

    public SectorRange ( float size ,Vector2 offCenter ) : base(size, offCenter)
    {
    }

    public SectorRange ( float size, Vector2 offCenter, float angle) : base(size, offCenter)
    {
        Angle = angle;
    }

    public SectorRange ( float size, Vector2 offCenter, float angle, Vector2 direction ) : base(size, offCenter)
    {
        Angle = angle;
        Direction = direction;
    }

    public SectorRange ( string[] config ) : base(config)
    {
        Angle = GetParam(config, 3);
        SizeStart = GetParam(config, 4);
        Direction = new Vector2(GetParam(config, 5), GetParam(config, 6));
    }

    public override bool InRange ( Vector2 targetPos, Vector2 basePos )
    {
        if(Vector2.Distance(targetPos, basePos + OffsetCenter) > Size)
            return false;
        return true;
    }
}