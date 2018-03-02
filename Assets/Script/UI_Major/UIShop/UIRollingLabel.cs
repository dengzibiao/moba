/*
文件名（File Name）:   UI_ShopSell.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-7-1 16:46:51
*/
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIRollingLabel : MonoBehaviour
{

    public UIPanel m_Panel;
    public UILabel m_Label;
    public UILabel m_CopyLabel;


    public float m_Speed;//播放速度或移动的距离
    public float m_SpaceGap;//间隔
    public float m_TimeGap;

    private float m_fLabelSize;
    private float m_fClipSize;
    private float m_fTargetPositionX;
    private Vector3 m_v3StartPosition;
    private Vector3 m_v3SecondStartPosition;
    private Vector3 m_v3Offset;
    private bool m_bRolling;
    private bool m_labelopening;


    void Start()
    {
        this.gameObject.layer = this.transform.parent.gameObject.layer;
        m_v3StartPosition = m_Label.transform.localPosition;
        m_fClipSize = m_Panel.width;
        if (m_Speed <= 0) m_Speed = 1;
        if (m_SpaceGap <= 0) m_SpaceGap = m_Label.fontSize * 2;
        m_v3Offset = new Vector3(m_Speed, 0, 0);
        InitRolling("天娱在线获得英雄奖励");
    }

    public void InitRolling(string name = null)
    {
        if (name != null)
        {
            m_Label.text = name;
        }

        m_fLabelSize = m_Label.localSize.x;
        m_fTargetPositionX = m_v3StartPosition.x - m_fLabelSize - m_SpaceGap;

        m_v3SecondStartPosition = new Vector3(m_v3StartPosition.x + m_fLabelSize + m_SpaceGap, m_v3StartPosition.y, m_v3StartPosition.z);
        m_Label.transform.localPosition = m_v3StartPosition;
        m_CopyLabel.transform.localPosition = m_v3SecondStartPosition;
        m_CopyLabel.text = m_Label.text;

        m_CopyLabel.enabled = m_fLabelSize > m_fClipSize;
        m_bRolling = true;



    }

    private void m_Labelopen()
    {
        m_Label.enabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_CopyLabel.enabled)
        {
            m_Label.enabled = false;
            m_labelopening = false;
        }
        if (!m_Label.enabled && !m_labelopening)
        {
            m_Label.enabled = true;
            m_labelopening = true;
        }
        if (m_fLabelSize > m_fClipSize && m_bRolling)
        {
            m_Label.transform.localPosition -= m_v3Offset;
            if (m_Label.transform.localPosition.x < m_fTargetPositionX)
            {
                m_Label.transform.localPosition = m_v3SecondStartPosition;
                m_bRolling = false;
               // Invoke("StartMove", m_TimeGap);
            }

            m_CopyLabel.transform.localPosition -= m_v3Offset;
            if (m_CopyLabel.transform.localPosition.x < m_fTargetPositionX)
            {
                m_CopyLabel.transform.localPosition = m_v3SecondStartPosition;
                m_bRolling = false;
               // Invoke("StartMove", m_TimeGap);
            }
        }

    }

    private void StartMove()
    {
        m_bRolling = true;
    }

}
