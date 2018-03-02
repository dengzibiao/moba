

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMarquee : GUIBase {

    public UIPanel m_Panel;
    public UILabel m_Label;
    public UILabel m_CopyLabel;


    public float m_Speed;//播放速度或移动的距离
    public float m_SpaceGap = 250;
    public float m_TimeGap;//延迟

    private float m_fLabelSize;//初始化时存储Lable的长度用于计算
    private float m_fClipSize;
    private float m_fTargetPositionX;//Lable播放一次后消失的位置
    private Vector3 m_v3StartPosition;//存储初始化时Lable的位置
    private Vector3 m_v3SecondStartPosition;
    private Vector3 m_v3Offset;
    private bool m_bRolling=false;
    private bool m_labelopening;

    private int inndex = 0;//播放次数
    private string content;//本地内容储存
    public static UIMarquee instance;
    // Use this for initialization
    public UIMarquee()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIMarquee;
    }

    
    protected override void Init()
    {
        base.Init();
        this.gameObject.layer = this.transform.parent.gameObject.layer;
        m_v3StartPosition = m_Label.transform.localPosition;
        m_fClipSize = m_Panel.width;
        if (m_Speed <= 0) m_Speed = 1;
        if (m_SpaceGap <= 0) m_SpaceGap = m_Label.fontSize * 2;
        m_v3Offset = new Vector3(m_Speed, 0, 0);

    }

    bool isPlay = false;
    
    protected override void ShowHandler()
    {
        base.ShowHandler();
        if (!isPlay)
        {

            List<MarqueeData> listss = null;
            if (playerData.GetInstance().marqueeListDic.ContainsKey(1))
            {
                playerData.GetInstance().marqueeListDic.TryGetValue(1, out listss);
                if (listss.Count > 0)
                {
                    isPlay = true;
                    //Invoke("InitRolling", m_TimeGap);
                    //content = "[FFFF00FF]" + listss[0].user1 + "[-]" + "[2bb740]" + "【" + listss[0].user2 + "】"+"[-]"+"[FFFF00FF]"+listss[0].user3;
                    content = listss[0].user1;
                    InitRolling(content);
                    listss.RemoveAt(0);
                }
                if (listss.Count <= 0)
                {
                    playerData.GetInstance().marqueeListDic.Remove(1);
                }
            }
            else if (playerData.GetInstance().marqueeListDic.ContainsKey(2))
            {
                playerData.GetInstance().marqueeListDic.TryGetValue(2, out listss);
                if (listss.Count > 0)
                {
                    isPlay = true;
                    //content = "[FFFF00FF]" + listss[0].user1 + "[-]" + "[2bb740]" + "【" + listss[0].user2 + "】" + "[-]" + "[FFFF00FF]" + listss[0].user3;
                    content = listss[0].user1;
                    InitRolling(content);
                    listss.RemoveAt(0);
                }
                else { playerData.GetInstance().marqueeListDic.Remove(2); this.ShowHandler(); }
            }
            else if (playerData.GetInstance().marqueeListDic.ContainsKey(3))
            {
                playerData.GetInstance().marqueeListDic.TryGetValue(3, out listss);
                if (listss.Count > 0)
                {
                    isPlay = true;
                    //content = "[FFFF00FF]" + listss[0].user1 + "[-]" + "[2bb740]" + "【" + listss[0].user2 + "】" + "[-]" + "[FFFF00FF]" + listss[0].user3;
                    content = listss[0].user1;
                    InitRolling(content);
                    listss.RemoveAt(0);
                }
                else { playerData.GetInstance().marqueeListDic.Remove(3); this.ShowHandler(); }
            }
            else if (playerData.GetInstance().marqueeListDic.ContainsKey(4))
            {
                playerData.GetInstance().marqueeListDic.TryGetValue(4, out listss);
                if (listss.Count > 0)
                {
                    isPlay = true;
                    //content = "[FFFF00FF]" + listss[0].user1 + "[-]" + "[2bb740]" + "【" + listss[0].user2 + "】" + "[-]" + "[FFFF00FF]" + listss[0].user3;
                    content = listss[0].user1;
                    InitRolling(content);
                    listss.RemoveAt(0);
                }
                else { playerData.GetInstance().marqueeListDic.Remove(4); this.ShowHandler(); }
            }
            else if (playerData.GetInstance().marqueeListDic.ContainsKey(5))
            {
                playerData.GetInstance().marqueeListDic.TryGetValue(5, out listss);
                if (listss.Count > 0)
                {
                    isPlay = true;
                    //content = "[FFFF00FF]" + listss[0].user1 + "[-]" + "[2bb740]" + "【" + listss[0].user2 + "】" + "[-]" + "[FFFF00FF]" + listss[0].user3;
                    content = listss[0].user1;
                    InitRolling(content);
                    listss.RemoveAt(0);
                }
                else { playerData.GetInstance().marqueeListDic.Remove(5); this.ShowHandler(); }
            }
            else if (playerData.GetInstance().marqueeListDic.ContainsKey(6))
            {
                playerData.GetInstance().marqueeListDic.TryGetValue(6, out listss);
                if (listss.Count > 0)
                {
                    isPlay = true;
                    //content = "[FFFF00FF]" + listss[0].user1 + "[-]" + "[2bb740]" + "【" + listss[0].user2 + "】" + "[-]" + "[FFFF00FF]" + listss[0].user3;
                    content = listss[0].user1;
                    InitRolling(content);
                    listss.RemoveAt(0);
                }
                else { playerData.GetInstance().marqueeListDic.Remove(6); this.ShowHandler(); }
            }
            else if (playerData.GetInstance().marqueeListDic.ContainsKey(7))
            {
                playerData.GetInstance().marqueeListDic.TryGetValue(7, out listss);
                if (listss.Count > 0)
                {
                    isPlay = true;
                    //content = "[FFFF00FF]" + listss[0].user1 + "[-]" + "[2bb740]" + "【" + listss[0].user2 + "】" + "[-]" + "[FFFF00FF]" + listss[0].user3;
                    content = listss[0].user1;
                    InitRolling(content);
                    listss.RemoveAt(0);
                }
                else
                {
                    playerData.GetInstance().marqueeListDic.Remove(7);
                    //isPlay=false;
                    //this.Hide(); 
                }
            }
            else
            {
                isPlay = false;
                this.Hide();
            }
        }
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }
    int a = 0;
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
              
                if (inndex>0)
                {
                    inndex--;
                    Invoke("InitRolling", m_TimeGap);
                }
                else
                {
                    m_bRolling = false;
                    isPlay = false;
                    this.ShowHandler();
                }

            }

            m_CopyLabel.transform.localPosition -= m_v3Offset;
            if (m_CopyLabel.transform.localPosition.x < m_fTargetPositionX)
            {
                m_CopyLabel.transform.localPosition = m_v3SecondStartPosition;
                m_bRolling = false;//
                isPlay = false;//
                this.ShowHandler();
            }
        }

    }
    public void InitRolling(string name = null, string name2 = null)
    {
        Debug.Log(name+"name11111");
       
        //name2 = name;
        if (name != null || name2 != null)
        {
            m_Label.text = name;
            m_CopyLabel.text = name2;
        }
        else
        {
            m_Label.text = content;
        }
        m_fLabelSize = m_Label.localSize.x;
        m_fTargetPositionX = m_v3StartPosition.x - m_fLabelSize - m_SpaceGap;

        m_v3SecondStartPosition = new Vector3(m_v3StartPosition.x + m_fLabelSize + m_SpaceGap, m_v3StartPosition.y, m_v3StartPosition.z);
        m_Label.transform.localPosition = m_v3StartPosition;
        m_CopyLabel.transform.localPosition = m_v3SecondStartPosition;

        m_CopyLabel.enabled = m_fLabelSize > m_fClipSize;
        m_bRolling = true;

    }

    private void StartMove()
    {
        m_bRolling = true;
    }


}
