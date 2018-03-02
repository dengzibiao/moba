/*
文件名（File Name）:   SoundManager.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager {
    /// <summary>
    /// 摄像机
    /// </summary>
    public static GameObject mMainCamObj;
    /// <summary>
    /// 音乐剪辑片断集合
    /// </summary>
    public static AudioClip[] mAudioClips;
    /// <summary>
    /// 背景音乐
    /// </summary>
    private static AudioSource mBGM;
    /// <summary>
    /// 
    /// </summary>
    private static bool mbInitAssetBundle = false;
    /// <summary>
    /// 背景音乐最大音量
    /// </summary>
    private static float mCurBGMVolumeMax;

    private static Dictionary<string, Vector3> m_ReserveSndInfo = new Dictionary<string, Vector3>();
    private static Dictionary<string, AudioClip> clipDic = new Dictionary<string, AudioClip>();
    /// <summary>
    /// 初始化音乐
    /// </summary>
    public static void Init()
    {
        mAudioClips = new AudioClip[SoundDefine.SoundSrcPaths.Length];
        for (int i = 0; i < SoundDefine.SoundSrcPaths.Length; i++)
        {
            mAudioClips[i] = Resources.Load(SoundDefine.SoundSrcPaths[i], typeof(AudioClip)) as AudioClip;

            if (mAudioClips[i] != null)
            {
                clipDic.Add(SoundDefine.SoundSrcPaths[i], mAudioClips[i]);
            }
        }
        mBGM = null;
        mCurBGMVolumeMax = 0.7f;
    }
    public static AudioSource GetBGM()
    {
        return mBGM;
    }
    /// <summary>
    /// 初始化背景音乐
    /// </summary>
    public static void InitBGM()
    {
        GameObject bgm = GameObject.Find("BGM");
        if (bgm != null)
        {
            mBGM = bgm.GetComponent<AudioSource>();
        }
        else
        {
         //   Debug.LogError("Not Found BGM Object");
        }
        mCurBGMVolumeMax = mBGM.volume;

        mBGM.mute = false;

    }

    /// <summary>
    /// 设置静音
    /// </summary>
    /// <param name="bMute"></param>
    public static void SetMuteBGM(bool bMute)
    {
        mBGM.mute = bMute;
    }

    /// <summary>
    /// 播放背景音乐 
    /// </summary>
    /// <param name="sndId"></param>
    public static bool PlayBGM(string name)
    {
        //if ((sndId < 0) || (sndId >= mAudioClips.Length))
        //    return false;

        if (name == null) return false;
        if (GeneralDefine.mBGMOff)
            return false;

        if (mBGM.isPlaying)
            mBGM.Stop();    
        AudioClip clip = null;
        clipDic.TryGetValue(name, out clip);
        if (clip!=null)
        {
            mBGM.clip = clip;
        }
        // mAudioClips[sndId];

        mBGM.loop = true;

        //if (sndId == SoundDefine.mWinBgMId)
        //{
        //    mBGM.loop = false;
        //}
        mBGM.Play();
        RefreshBGMVolume();
        return true;

    }


    /// <summary>
    /// 刷新背景音乐音量
    /// </summary>
    public static void RefreshBGMVolume(float change = 1f)
    {
        SetBGMVolume(mCurBGMVolumeMax);// * change);
    }

    /// <summary>
    /// 设置背景音乐音量
    /// </summary>
    /// <param name="vol"></param>
    public static void SetBGMVolume(float vol)
    {
        mBGM.volume = vol;
    }
    /// <summary>
    /// 停止播放
    /// </summary>
    public static void StopBGM()
    {
        mBGM.Stop();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sndId"></param>
    /// <param name="srcObj"></param>
    /// <returns></returns>
    public static bool PlaySnd(string  name, GameObject srcObj)
    {
        if (GeneralDefine.mSndOff)
            return false;
       // Debug.Log("Play Snd : " + name);

        //if (sndId < 0 || sndId >= mAudioClips.Length)
        //    return false;
        if (name == null) return false;
        if (!m_ReserveSndInfo.ContainsKey(name))
        {
            m_ReserveSndInfo.Add(name, srcObj.transform.position);
            //如果背景音乐组件已创建,调节背景音乐音量
            if (mBGM != null)
            {
                RefreshBGMVolume(0.1f);
            }
        }
        return true;
    }
    /// <summary>
    /// 播放音乐剪辑
    /// </summary>
    /// <param name="sndId"></param>
    /// <returns></returns>
    public static bool PlaySnd(string name)
    {
        Debug.Log("Play Snd:" + name);
        //if ((sndId < 0) || (sndId >= mAudioClips.Length))
        //    return false;
        if (name == null) return false;
        Vector3 zero = Vector3.zero;

        if (mMainCamObj != null)
            zero = mMainCamObj.transform.position;

        if (!m_ReserveSndInfo.ContainsKey(name))
        {
            m_ReserveSndInfo.Add(name, zero);
            //如果背景音乐组件已创建,调节背景音乐音量
            if (mBGM != null)
            {
                RefreshBGMVolume(0.1f);
            }
        }
        return true;
    }



    /// <summary>
    /// 重置储存音乐信息
    /// </summary>
    public static void Reset()
    {
        m_ReserveSndInfo.Clear();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="delta"></param>
    public static void Update(float delta)
    {
        foreach (KeyValuePair<string, Vector3> pair in m_ReserveSndInfo)
        {

            if (clipDic[pair.Key] != null)
            {
                AudioSource.PlayClipAtPoint(clipDic[pair.Key], pair.Value);
            }
            else
            {
              //  Debug.Log("Not Found Snd key:" + pair.Key);
            }
        }

        m_ReserveSndInfo.Clear();
    }
}
