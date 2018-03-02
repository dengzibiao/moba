using UnityEngine;
using System.Text;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Examples/HUD Text")]
public class HUDText : MonoBehaviour
{
    protected class Entry
    {
        public float time;          // Timestamp of when this entry was added
        public float stay = 0f;     // How long the text will appear to stay stationary on the screen
        public float offset = 0f;   // How far the object has moved based on time
        public float xOffset = 0f;
        public float val = 0f;      // Optional value (used for damage)
        public UILabel label;       // Label on the game object
        public bool isLeft = false;
        public HUDType hudType;

        public float movementStart { get { return time + stay; } }
    }

    /// <summary>
    /// Sorting comparison function.
    /// </summary>
    static int Comparison(Entry a, Entry b)
    {
        if (a.movementStart < b.movementStart) return -1;
        if (a.movementStart > b.movementStart) return 1;
        return 0;
    }

    // Deprecated, use 'ambigiousFont' instead.
    [HideInInspector]
    [SerializeField]
    UIFont font;

    /// <summary>
    /// Font used by the labels.
    /// </summary>
    public UIFont bitmapFont;

    /// <summary>
    /// True type font used by the labels. Alternative to specifying a bitmap font ('font').
    /// </summary>
    public Font trueTypeFont;

    /// <summary>
    /// Size of the font to use for the popup list's labels.
    /// </summary>
    public int fontSize = 50;

    /// <summary>
    /// Font style used by the dynamic font.
    /// </summary>
    public FontStyle fontStyle = FontStyle.Normal;

    AnimationCurve YOffsetCurveUsed;
    AnimationCurve XOffsetCurveUsed;
    AnimationCurve AlphaCurveUsed;
    AnimationCurve ScaleCurveUsed;

    AnimationCurve offsetCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(0.2f,15f), new Keyframe(1.2f, 0f)});
    AnimationCurve xOffsetCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(0.2f, 10f), new Keyframe(1.2f, 20f)});
    AnimationCurve alphaCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(0.1f, 1f), new Keyframe(0.8f, 1f), new Keyframe(1.2f, 0f)});
    AnimationCurve scaleCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(0.1f, 1f), new Keyframe(1.2f, 0.8f)});

    AnimationCurve addOffsetCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1.2f, 30f) });
    AnimationCurve addxOffsetCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1.2f, 0f) });

    AnimationCurve buffOffsetCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(0.3f, 5f), new Keyframe(1.2f, 5f)});
    AnimationCurve buffXOffsetCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1.2f, 0f) });
    AnimationCurve buffAlphaCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(0.1f, 1f), new Keyframe(0.6f, 1f), new Keyframe(1.2f, 0f)});
    AnimationCurve buffScaleCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 1f), new Keyframe(1.2f, 1f) });

    List <Entry> mList = new List<Entry>();
    List<Entry> mUnused = new List<Entry>();

    int counter = 0;
    private StringBuilder mBuilder;
    float totalEnd;
    //HUDType type;

    /// <summary>
    /// Whether some HUD text is visible.
    /// </summary>
    public bool isVisible { get { return mList.Count != 0; } }

    /// <summary>
    /// Font used by the HUD text. Conveniently wraps both dynamic and bitmap fonts into one property.
    /// </summary>
    public Object ambigiousFont
    {
        get
        {
            if (trueTypeFont != null) return trueTypeFont;
            if (bitmapFont != null) return bitmapFont;
            return font;
        }
        set
        {
            if (value is Font)
            {
                trueTypeFont = value as Font;
                bitmapFont = null;
                font = null;
            }
            else if (value is UIFont)
            {
                bitmapFont = value as UIFont;
                trueTypeFont = null;
                font = null;
            }
        }
    }

    /// <summary>
    /// Create a new entry, reusing an old entry if necessary.
    /// </summary>
    Entry Create()
    {
        // See if an unused entry can be reused
        //可以重复使用的
        if (mUnused.Count > 0)
        {
            Entry ent = mUnused[mUnused.Count - 1];
            mUnused.RemoveAt(mUnused.Count - 1);
            //ent.offset = 0f;
            //ent.xOffset = 0f;
            //ent.label.alpha = 0f;
            //ent.label.cachedTransform.localPosition = new Vector3(0.001f, 0.001f, 0.001f);
            ent.label.depth = NGUITools.CalculateNextDepth(gameObject);
            NGUITools.SetActive(ent.label.gameObject, true);
            mList.Add(ent);
            return ent;
        }

        // New entry
        //创建一个新的
        Entry ne = new Entry();
        ne.label = NGUITools.AddWidget<UILabel>(gameObject);
        ne.label.name = counter.ToString();
        ne.label.ambigiousFont = ambigiousFont;
        ne.label.fontSize = fontSize;
        ne.label.fontStyle = fontStyle;
        ne.label.overflowMethod = UILabel.Overflow.ResizeFreely;

        // Make it small so that it's invisible to start with
        ne.label.cachedTransform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        mList.Add(ne);
        ++counter;
        return ne;
    }

    /// <summary>
    /// Delete the specified entry, adding it to the unused list.
    /// </summary>
    void Delete(Entry ent)
    {
        mList.Remove(ent);
        mUnused.Add(ent);
        NGUITools.SetActive(ent.label.gameObject, false);
    }

    /// <summary>
    /// Add a new scrolling text entry.
    /// </summary>
    public void Add(string str, float stayDuration, HUDType hType)
    {
        if (!enabled) return;

        // Create a new entry
        Entry ne = Create();
        ne.stay = stayDuration;
        ne.label.applyGradient = false;
        ne.label.alpha = 0f;
        ne.label.effectStyle = UILabel.Effect.Outline;
        ne.label.effectDistance = new Vector2(1, 1);
        ne.label.color = Color.white;
        ne.label.fontSize = GameLibrary.isMoba ? 20 : 30;
        ne.label.depth = 999;
        ne.time = Time.realtimeSinceStartup;
        ne.isLeft = Random.Range(0, 2) > 0;
        ne.hudType = hType;

        switch (hType)
        {
            case HUDType.DamagePlayer:
                ne.label.text = str;
                ne.label.effectColor = new Color((float)229 / 255, 0, 0);
                break;
            case HUDType.DamageEnemy:
                ne.label.text = str;
                ne.label.color = new Color((float)255 / 255, (float)59 / 255, (float)44 / 255);
                ne.label.effectColor = new Color((float)101 / 255, 0, 0);
                break;
            case HUDType.Crit:
                mBuilder = new StringBuilder(Localization.Get("HUDCrit"));
                ne.label.text = mBuilder.Append(str).ToString();
                ne.label.applyGradient = true;
                ne.label.gradientTop = new Color((float)255/255, (float)250/255, (float)126/255);
                ne.label.gradientBottom = new Color((float)255/255, (float)235/255, (float)101/255);
                ne.label.effectColor = new Color((float)33/255, (float)12/255, 0);
                ne.label.effectDistance = new Vector2(1, 1);
                ne.label.fontSize = GameLibrary.isMoba ? 30 : 40;
                break;
            case HUDType.Cure:
                mBuilder = new StringBuilder(Localization.Get("HUDCure"));
                ne.label.text = mBuilder.Append("+").Append(str).ToString();
                ne.label.effectColor = new Color(0, (float)40 / 255, (float)0 / 255);
                ne.label.color = new Color((float)142 / 255, (float)245 / 255, (float)106 / 255);
                break;
            case HUDType.Bleeding:
                mBuilder = new StringBuilder(Localization.Get("HUDBleeding"));
                ne.label.text = mBuilder.Append(str).ToString();
                ne.label.color = new Color((float)255 / 255, (float)59 / 255, (float)44 / 255);
                ne.label.effectColor = new Color((float)101 / 255, 0, 0);
                break;
            case HUDType.SuckBlood:
                mBuilder = new StringBuilder(Localization.Get("HUDSuckBlood"));
                ne.label.text = mBuilder.Append(str).ToString();
                ne.label.effectColor = new Color(0, (float)195 / 255, (float)4 / 255);
                break;
            case HUDType.Miss:
                mBuilder = new StringBuilder(Localization.Get("HUDMiss"));
                ne.label.text = mBuilder.ToString();
                //ne.label.color = new Color((float)255 / 255, (float)59 / 255, (float)44 / 255);
                ne.label.effectColor = new Color((float)101 / 255, 0, 0);
                break;
            case HUDType.Immune:
                mBuilder = new StringBuilder(Localization.Get("HUDImmune"));
                ne.label.text = mBuilder.ToString();
                //ne.label.color = new Color((float)255 / 255, (float)59 / 255, (float)44 / 255);
                ne.label.effectColor = new Color((float)101 / 255, 0, 0);
                break;
            case HUDType.Absorb:
                mBuilder = new StringBuilder(Localization.Get("HUDAbsorb"));
                //ne.label.text = mBuilder.Append("(").Append(str).Append(")").ToString();
                ne.label.text = mBuilder.ToString();
                ne.label.effectColor = new Color(0, (float)40 / 255, (float)0 / 255);
                break;
            default:
                break;
        }

        YOffsetCurveUsed = offsetCurve;
        XOffsetCurveUsed = xOffsetCurve;
        AlphaCurveUsed = alphaCurve;
        ScaleCurveUsed = scaleCurve;

        if(isAddType(ne))
        {
            YOffsetCurveUsed = addOffsetCurve;
            XOffsetCurveUsed = addxOffsetCurve;
        }

        if(isBuffType(ne))
        {
            YOffsetCurveUsed = buffOffsetCurve;
            XOffsetCurveUsed = buffXOffsetCurve;
            AlphaCurveUsed = buffAlphaCurve;
            ScaleCurveUsed = buffScaleCurve;
        }

        float offsetEnd = YOffsetCurveUsed.keys[YOffsetCurveUsed.keys.Length - 1].time;
        float xOffsetEnd = XOffsetCurveUsed.keys[XOffsetCurveUsed.keys.Length - 1].time;
        float alphaEnd = AlphaCurveUsed.keys[AlphaCurveUsed.keys.Length - 1].time;
        float scalesEnd = ScaleCurveUsed.keys[ScaleCurveUsed.keys.Length - 1].time;
        totalEnd = Mathf.Max(scalesEnd, Mathf.Max(Mathf.Max(offsetEnd, xOffsetEnd), alphaEnd));

        // Sort the list
        mList.Sort(Comparison);
    }

    /// <summary>
    /// Auto-upgrade legacy font references.
    /// </summary>
    void OnEnable()
    {
        if (font != null)
        {
            if (font.isDynamic)
            {
                trueTypeFont = font.dynamicFont;
                fontStyle = font.dynamicFontStyle;
                mUseDynamicFont = true;
            }
            else if (bitmapFont == null)
            {
                bitmapFont = font;
                mUseDynamicFont = false;
            }
            font = null;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

    bool mUseDynamicFont = false;

    void OnValidate()
    {
        Font ttf = trueTypeFont;
        UIFont fnt = bitmapFont;

        bitmapFont = null;
        trueTypeFont = null;

        if (ttf != null && (fnt == null || !mUseDynamicFont))
        {
            bitmapFont = null;
            trueTypeFont = ttf;
            mUseDynamicFont = true;
        }
        else if (fnt != null)
        {
            // Auto-upgrade from 3.0.2 and earlier
            if (fnt.isDynamic)
            {
                trueTypeFont = fnt.dynamicFont;
                fontStyle = fnt.dynamicFontStyle;
                fontSize = fnt.defaultSize;
                mUseDynamicFont = true;
            }
            else
            {
                bitmapFont = fnt;
                mUseDynamicFont = false;
            }
        }
        else
        {
            trueTypeFont = ttf;
            mUseDynamicFont = true;
        }
    }

    /// <summary>
    /// Disable all labels when this script gets disabled.
    /// </summary>
    void OnDisable()
    {
        for (int i = mList.Count; i > 0;)
        {
            Entry ent = mList[--i];
            if (ent.label != null) ent.label.enabled = false;
            else mList.RemoveAt(i);
        }
    }

    bool isAddType (Entry ent)
    {
        return ent.hudType == HUDType.Cure || ent.hudType == HUDType.SuckBlood || ent.hudType == HUDType.Miss;
    }

    bool isBuffType ( Entry ent )
    {
        return ent.hudType == HUDType.Immune || ent.hudType == HUDType.Absorb;
    }

    /// <summary>
    /// Update the position of all labels, as well as update their size and alpha.
    /// </summary>
    void FixedUpdate()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        float time = RealTime.time;
        // Adjust alpha and delete old entries
        for (int i = mList.Count; i > 0;)
        {
            Entry ent = mList[--i];
            float currentTime = time - ent.movementStart;

            ent.offset = YOffsetCurveUsed.Evaluate(currentTime);
            ent.xOffset = XOffsetCurveUsed.Evaluate(currentTime);
            ent.label.alpha = AlphaCurveUsed.Evaluate(currentTime);
            float s = ScaleCurveUsed.Evaluate(time - ent.time);
            if (s < 0.001f) s = 0.001f;
            ent.label.cachedTransform.localScale = s * Vector3.one;

            // Delete the entry when needed
            if (currentTime > (isAddType(ent) ? 1f : totalEnd)) Delete(ent);
            else ent.label.enabled = true;
        }

        float offset = 0f;
        float xOffset = 0f;
        //Move the entries
        for (int i = mList.Count; i > 0;)
        {
            offset = 0f;
            xOffset = 0f;
            Entry ent = mList[--i];
            offset = Mathf.Max(offset, ent.offset * 5);
            xOffset = Mathf.Max(xOffset, ent.xOffset * 5);
            float offsetRate = GameLibrary.isMoba ? 0.6f : 1f;
            ent.label.cachedTransform.localPosition = new Vector3(ent.isLeft ? xOffset * offsetRate : -xOffset * offsetRate, offset * offsetRate, 0f);
            offset += Mathf.Round(ent.label.cachedTransform.localScale.y * ent.label.fontSize);
            xOffset += Mathf.Round(ent.label.cachedTransform.localScale.y * ent.label.fontSize);
        }
    }
}
