using UnityEngine;

[CreateAssetMenu(fileName = "OptionDefaults", menuName = "AnythingCafe/Options/OptionDefaults")]
public class OptionDefaults : ScriptableObject
{
    [Header("音频设置")]
    [Range(0.0f, 1.0f)] public float DefaultGlobalVolume = 1.0f;
    [Range(0.0f, 1.0f)] public float DefaultAmbientVolume = 1.0f;
    [Range(0.0f, 1.0f)] public float DefaultEroVolume = 1.0f;
    [Range(0.0f, 1.0f)] public float DefaultMusicVolume = 1.0f;
    [Range(0.0f, 1.0f)] public float DefaultSFXVolume = 1.0f;
    [Range(0.0f, 1.0f)] public float DefaultUIVolume = 1.0f;

    [Header("显示设置")]
    public ScreenMode DefaultScreenMode = ScreenMode.Windowed;
    public int DefaultResolutionWidth = 1920;
    public int DefaultResolutionHeight = 1080;

    [Header("其他设置")]
    public string DefaultLanguage = "en";

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