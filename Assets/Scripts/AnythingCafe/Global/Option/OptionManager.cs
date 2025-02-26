using System;
using System.Collections.Generic;

public class OptionManager : Singleton<OptionManager>, IInitializable
{
    public float GlobalVolume { get; private set; }
    public float AmbientVolume { get; private set; }
    public float EroVolume { get; private set; }
    public float MusicVolume { get; private set; }
    public float SFXVolume { get; private set; }
    public float UIVolume { get; private set; }

    public ScreenMode ScreenMode { get; private set; }
    public int ResolutionWidth { get; private set; }
    public int ResolutionHeight { get; private set; }

    // public IEnumerable Language { get; private set; }

    public event Action OnOptionChanged;

    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;

        LoadAll();

        IsInitialized = true;
    }

    public void Load(Option.Key key)
    {
        var option = Option.KeyToName[key];

        var optionValue = ES3.Load(option);

        switch (key)
        {
            case Option.Key.GlobalVolume:
                GlobalVolume = (float)optionValue;
                break;
            case Option.Key.AmbientVolume:
                AmbientVolume = (float)optionValue;
                break;
            case Option.Key.EroVolume:
                EroVolume = (float)optionValue;
                break;
            case Option.Key.MusicVolume:
                MusicVolume = (float)optionValue;
                break;
            case Option.Key.SFXVolume:
                SFXVolume = (float)optionValue;
                break;
            case Option.Key.UIVolume:
                UIVolume = (float)optionValue;
                break;
            case Option.Key.ScreenMode:
                ScreenMode = (ScreenMode)optionValue;
                break;
            case Option.Key.ResolutionWidth:
                break;
            case Option.Key.ResolutionHeight:
                break;
            case Option.Key.Language:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(key), key, null);
        }

        OnOptionChanged?.Invoke();
    }

    public void LoadAll()
    {


        OnOptionChanged?.Invoke();
    }

    public void Modify(Option.Key key, object value)
    {

    }

    public void Save()
    {

    }
}
