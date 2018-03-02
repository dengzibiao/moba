//#define PLAY_MUSIC
using UnityEngine;
using System.Collections.Generic;

public class AudioController : MonoBehaviour
{
	static AudioController _instance;
	AudioListener mAudioListener;
    Camera mainCamera;
    GameObject prefab;
    private string[] mPoolInitName = new string[] { "attack1", "attack2", "skill1", "skill2", "skill3", "skill4", "dlg1", "dlg2" };

    public static AudioController Instance {
		get {
			if (!_instance) {
				_instance = GameObject.FindObjectOfType (typeof(AudioController)) as AudioController;
				if (!_instance) {
					GameObject am = new GameObject ("AudioController");
					_instance = am.AddComponent (typeof(AudioController)) as AudioController;
					_instance.mAudioListener = am.AddMissingComponent<AudioListener> ();
				}
			}
			return _instance;
		}
	}
    
	void Awake ()
	{
        prefab = Resources.Load(GameLibrary.Resource_Sound + "EffectSound") as GameObject;
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (CharacterManager.player != null)
        {
            transform.position = CharacterManager.player.transform.position;
        }
        else 
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
            if (mainCamera != null)
                transform.position = mainCamera.transform.position;
        }
    }
	
	Dictionary<string,AudioClip> sounds = new Dictionary<string,AudioClip> ();
	AudioClip GetSound (string filename)
	{
		AudioClip audio = null;
		if (sounds.TryGetValue (filename, out audio)) {
			return audio;
		}
        audio = Resources.Load<AudioClip>(filename);
		sounds.Add (filename, audio);
		return audio;
	}

    public AudioSource PlayEffectSound (string soundname, CharacterState cs)
	{
		AudioClip audio = GetSound (GameLibrary.Resource_Sound + soundname); 
		if (audio != null)
			return PlayEffectSound (audio, cs);
		else
#if PLAY_MUSIC
            Debug.LogWarning ("can not find music name is::" + soundname);
#endif
        return null;
	}

    public AudioSource PlayEffectSound (AudioClip audio, CharacterState cs)
	{
		if (!GameSetting.isSoundMute)
			return PlaySound (audio, GameSetting.soundVolume, cs);
		return null;
	}

    //声音打断接口
    public void StopEffectSound(CharacterState cs)
    {
        if (mEffectSound != null && mEffectSound.ContainsKey(cs))
            mEffectSound [cs].Stop();
    }

    //UI声音打断接口
    public void StopUISound()
    {
        if (mUISound != null)
        {
            mUISound.Stop();
        }
    }

    public void ClearEffectSound()
    {
        mEffectSound.Clear();
    }

	Dictionary<CharacterState, AudioSource> mEffectSound = new Dictionary<CharacterState, AudioSource>();
	AudioSource PlaySound (AudioClip clip, float volume, CharacterState cs)
	{
		if (!mEffectSound.ContainsKey(cs)) {
            GameObject soundObj = Instantiate(prefab) as GameObject;
            soundObj.transform.parent = cs.transform;
            soundObj.transform.localPosition = Vector3.zero;
			mEffectSound.Add(cs, soundObj.GetComponent<AudioSource>());
        }
        mEffectSound[cs].pitch = 1f;
        mEffectSound [cs].PlayOneShot (clip, volume);
		return mEffectSound[cs];
	}

    public AudioSource PlayUISound(string soundname, bool absolutePath = false, bool Interrupt = true)
    {
        AudioClip audio = GetSound(absolutePath ? soundname : GameLibrary.Resource_UISound + soundname);
        if (audio != null)
        {
            if (Interrupt) StopUISound();
            return PlayUISound(audio);
        }
        else
#if PLAY_MUSIC
            Debug.LogWarning("can not find music name is::" + soundname);
#endif
        return null;
    }

    public AudioSource PlayUISound(AudioClip audio)
    {
        if (!GameSetting.isSoundMute)
            return PlaySound(audio, GameSetting.soundVolume);
        return null;
    }

    AudioSource mUISound;
    AudioSource PlaySound(AudioClip clip, float volume)
    {
        if (mUISound == null)
        {
            GameObject soundObj = new GameObject("UISound");
            soundObj.transform.parent = transform;
            mUISound = soundObj.AddMissingComponent<AudioSource>();
        }
        mUISound.pitch = 1f;
        mUISound.PlayOneShot(clip, volume);
        return mUISound;
    }

    public string bgmPlaying;
	AudioSource mBgMusic;
	public AudioSource PlayBackgroundMusic (string musicName, bool loop)
	{
		AudioClip bgMusic = GetSound (GameLibrary.Resource_Music + musicName);
		if (bgMusic != null)
		{
			if (mBgMusic == null) {
				GameObject musicObj = new GameObject ("BgMusic");
				musicObj.transform.parent = transform;
				mBgMusic = musicObj.AddMissingComponent <AudioSource> ();
			}
			if (mBgMusic != null && mBgMusic.enabled && NGUITools.GetActive (mBgMusic.gameObject)) {
				mBgMusic.clip = bgMusic;
				mBgMusic.loop = loop;
				mBgMusic.pitch = 1f;
				mBgMusic.volume = GameSetting.bgmVolume;
                mBgMusic.mute = GameSetting.isBgmMute;
				mBgMusic.Play ();
				bgmPlaying = musicName;
				return mBgMusic;
			}
		}
		else
		{
			Debug.Log("@@ can't load music " + musicName);
		}
		return null;
	}
	
	public void Volume (float volume)
	{
		if (mBgMusic != null) {
			mBgMusic.volume = volume;
		}
        GameSetting.bgmVolume = volume;
    }
	
	public void Mute (bool isMute)
	{
		if (mBgMusic != null) {
			mBgMusic.mute = isMute;
		}
        GameSetting.isBgmMute = isMute;
    }

    public void SoundVolume(float volume)
    {
        if (mUISound != null)
        {
            mUISound.volume = volume;
        }
        SetSoundVolume(volume);
        GameSetting.soundVolume = volume;
    }

    public void SoundMute(bool isMute)
    {
        if (mUISound != null)
        {
            mUISound.mute = isMute;
        }
        SetSoundMute(isMute);
        GameSetting.isSoundMute = isMute;
    }

    private void SetSoundVolume(float volume)
    {
        if (mEffectSound.Count > 0)
        {
            List<CharacterState> mTempCs = new List<CharacterState>(mEffectSound.Keys);
            for (int i = 0; i < mTempCs.Count; i++)
            {
                if (mTempCs[i] != null && mEffectSound[mTempCs[i]] != null)
                {
                    mEffectSound[mTempCs[i]].volume = volume;
                }
            }
        }
    }

    private void SetSoundMute(bool isMute)
    {
        if (mEffectSound.Count > 0)
        {
            List<CharacterState> mTempCs = new List<CharacterState>(mEffectSound.Keys);
            for (int i = 0; i < mTempCs.Count; i++)
            {
                if (mTempCs[i] != null && mEffectSound[mTempCs[i]] != null)
                {
                    mEffectSound[mTempCs[i]].mute = isMute;
                }
            }
        }
    }

    public void DisableOtherAudioListener ()
	{
		AudioListener[] listeners = GameObject.FindObjectsOfType (typeof(AudioListener)) as AudioListener[];
		if (listeners != null) {
			for (int i = 0; i < listeners.Length; ++i) {
				if (listeners [i] != mAudioListener) {
					listeners [i].enabled = false;
				}
			}
		}
	}

    public void CreateAudioClipPool(CharacterState cs)
    {
        string filename = string.Empty;
        for (int i = 0; i < mPoolInitName.Length; i++)
        {
            filename = GameLibrary.Resource_Sound + cs.CharData.attrNode.icon_name + "/" + mPoolInitName[i];
            if (!sounds.ContainsKey(filename))
            {
                AudioClip audio = Resources.Load<AudioClip>(filename);
                if (audio != null)
                {
                    sounds.Add(filename, audio);
                }
            }
        }
        if (!mEffectSound.ContainsKey(cs))
        {
            GameObject soundObj = Instantiate(prefab) as GameObject;
            soundObj.transform.parent = cs.transform;
            soundObj.transform.localPosition = Vector3.zero;
            mEffectSound.Add(cs, soundObj.GetComponent<AudioSource>());
        }
    }
}
