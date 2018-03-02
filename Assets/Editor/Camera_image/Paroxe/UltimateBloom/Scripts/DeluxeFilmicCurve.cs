using UnityEngine;
using System.Collections;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class DeluxeFilmicCurve 
{
    [SerializeField]
    public float m_BlackPoint = 0.0f;

    [SerializeField]
    public float m_WhitePoint = 4.0f;

    [SerializeField]
    public float m_CrossOverPoint = 0.6f;

    [SerializeField]
    public float m_ToeStrength = 0.98f;

    [SerializeField]
    public float m_ShoulderStrength = 0.0f;

    public float m_k;
    public Vector4 m_ToeCoef;
    public Vector4 m_ShoulderCoef;

    public float ComputeK(float t, float c, float b, float s, float w)
    {
        float num = (1 - t) * (c - b);
        float denom = (1 - s) * (w - c) + (1 - t) * (c - b);

        return num / denom;
    }

    public float Toe(float x, float t, float c, float b, float s, float w, float k)
    {
        float xnum = m_ToeCoef.x * x;
        float xdenom = m_ToeCoef.y * x;

        return (xnum + m_ToeCoef.z) / (xdenom + m_ToeCoef.w);

        /*float num = k * (1 - t) * (x - b);
        float denom = c - (1 - t) * b - t * x;

        return num / denom;*/
    }

    public float Shoulder(float x, float t, float c, float b, float s, float w, float k)
    {
        float xnum = m_ShoulderCoef.x * x;
        float xdenom = m_ShoulderCoef.y * x;

        return (xnum + m_ShoulderCoef.z) / (xdenom + m_ShoulderCoef.w) + k;

        /*float num = (1 - k) * (x - c);
        float denom = s*x + (1 - s) * w - c;

        return num / denom + k;*/
    }

    public float Graph(float x, float t, float c, float b, float s, float w, float k)
    {
        if (x <= m_CrossOverPoint)
            return Toe(x, t, c, b, s, w, k);

        return Shoulder(x, t, c, b, s, w, k);
    }

    public void StoreK()
    {
        m_k = ComputeK(m_ToeStrength, m_CrossOverPoint, m_BlackPoint, m_ShoulderStrength, m_WhitePoint);
    }

    public void ComputeShaderCoefficients(float t, float c, float b, float s, float w, float k)
    {
        {
            float xNumMul = k * (1 - t);
            float numAdd = k * (1 - t) * -b;
            float xDenomMul = -t;
            float denomAdd = c - (1 - t) * b;
            m_ToeCoef = new Vector4(xNumMul, xDenomMul, numAdd, denomAdd);
        }

        {
            float xNumMul = (1 - k);
            float numAdd = (1 - k) * -c;
            float xDenomMul = s;
            float denomAdd = (1 - s) * w - c;
            m_ShoulderCoef = new Vector4(xNumMul, xDenomMul, numAdd, denomAdd);
        }
    }

    public void UpdateCoefficients()
    {
        StoreK();
        ComputeShaderCoefficients(m_ToeStrength, m_CrossOverPoint, m_BlackPoint, m_ShoulderStrength, m_WhitePoint, m_k);
    }


#if UNITY_EDITOR
    public void OnGUI()
    {
        SetupCurve();

        float denom = m_WhitePoint - m_BlackPoint;

        float co = (m_CrossOverPoint - m_BlackPoint) / denom;
        if (Mathf.Abs(denom) < 0.001f)
            co = 0.5f;

        m_BlackPoint = DoSlider("  Minimum Intensity", m_BlackPoint, 0.0f, 10.0f);
        m_WhitePoint = DoSlider("  Maximum Intensity", m_WhitePoint, 0.0f, 10.0f);
        co = DoSlider("  Mid-value range", co, 0.0f, 1.0f);
        m_ToeStrength = -1.0f * DoSlider("  Dark Colors Intensity", -1.0f * m_ToeStrength, -0.99f, 0.99f);
        m_ShoulderStrength = DoSlider("  Bright Colors Intensity", m_ShoulderStrength, -0.99f, 0.99f);

        Rect r = EditorGUILayout.BeginVertical(GUILayout.MinHeight(60));
        Rect rr = GUILayoutUtility.GetRect(Mathf.Min(r.width, 60), 60);

        EditorGUI.CurveField(rr, m_Curve);
        EditorGUILayout.EndVertical();

        m_CrossOverPoint = co * (m_WhitePoint - m_BlackPoint) + m_BlackPoint;
        UpdateCoefficients();
    }

    AnimationCurve m_Curve;

    private static float CalculateLinearTangent(AnimationCurve curve, int index, int toIndex)
    {
        return (float)(((double)curve[index].value - (double)curve[toIndex].value) / ((double)curve[index].time - (double)curve[toIndex].time));
    }

    void SetupCurve()
    {
        m_Curve = new AnimationCurve();

        DeluxeFilmicCurve dt = this;

        float min = dt.m_BlackPoint;
        float max = dt.m_WhitePoint;

        int nbFrame = 40;
        float step = (max - min) / nbFrame;

        float curr = min;
        float k = dt.ComputeK(dt.m_ToeStrength, dt.m_CrossOverPoint, dt.m_BlackPoint, dt.m_ShoulderStrength, dt.m_WhitePoint);

        dt.StoreK();
        dt.ComputeShaderCoefficients(dt.m_ToeStrength, dt.m_CrossOverPoint, dt.m_BlackPoint, dt.m_ShoulderStrength, dt.m_WhitePoint, k);

        for (int i = 0; i < nbFrame + 1; ++i)
        {
            float value = dt.Graph(curr, dt.m_ToeStrength, dt.m_CrossOverPoint, dt.m_BlackPoint, dt.m_ShoulderStrength, dt.m_WhitePoint, k);
            m_Curve.AddKey(new Keyframe(curr, value));

            curr += step;
        }

        for (int i = 0; i < m_Curve.keys.Length - 1; ++i)
        {
            float tangent = CalculateLinearTangent(m_Curve, i, i + 1);
            m_Curve.keys[i].inTangent = tangent;
            m_Curve.keys[i].outTangent = tangent;

            m_Curve.SmoothTangents(i, 0.0f);
        }
    }

    float DoSlider(string label, float value, float min, float max)
    {
        float v = value;
        EditorGUILayout.BeginHorizontal();
        v = Mathf.Clamp(EditorGUILayout.FloatField(label, v), min, max);
        v = GUILayout.HorizontalSlider(v, min, max);
        EditorGUILayout.EndHorizontal();

        return v;
    }

#endif
}
