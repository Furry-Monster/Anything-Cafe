using UnityEngine;

[CreateAssetMenu(fileName = "OptionDefaults", menuName = "AnythingCafe/Options/OptionDefaults")]
public class OptionDefaults : ScriptableObject
{
    [Header("音频设置")]
    public float defaultGlobalVolume = 1.0f;
    public float defaultAmbientVolume = 1.0f;
    public float defaultEroVolume = 1.0f;
    public float defaultMusicVolume = 1.0f;
    public float defaultSFXVolume = 1.0f;
    public float defaultUIVolume = 1.0f;

    [Header("显示设置")]
    public ScreenMode defaultScreenMode = ScreenMode.Windowed;
    public int defaultResolutionWidth = 1920;
    public int defaultResolutionHeight = 1080;

    [Header("其他设置")]
    public string defaultLanguage = "en";

    private static OptionDefaults _instance;
    public static OptionDefaults Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<OptionDefaults>("OptionDefaults");
            return _instance;
        }
    }
}