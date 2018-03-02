using UnityEngine;

public class GameSetting : MonoBehaviour
{
    const string BG_MUSIC_MUTE = "BG_MUSIC_MUTE";
    const string EFFECT_SOUND_MUTE = "EFFECT_SOUND_MUTE";
    const string BG_MUSIC_VOLUME = "BG_MUSIC_VOLUME";
    const string EFFECT_SOUND_VOLUME = "EFFECT_SOUND_VOLUME";
    const string SHACKING = "SHACKING";
    const string POWER_CONSERVATION = "POWER_CONSERVATION";
    const string GAME_QUALITY = "GAME_QUALITY";

    public static bool isBgmMute = false;
    public static bool isSoundMute = false;
    public static float bgmVolume = 1f;
    public static float soundVolume = 1f;
    public static bool isPowerSave = true;
    public static bool isShacking = true;
    public static bool isHighQuality = true;
}
