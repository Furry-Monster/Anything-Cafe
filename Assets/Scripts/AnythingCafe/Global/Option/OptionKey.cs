using System;
using System.Collections.Generic;
using System.Reflection;

public static class Option
{
    public enum Key
    {
        GlobalVolume,
        AmbientVolume,
        EroVolume,
        MusicVolume,
        SFXVolume,
        UIVolume,
        ScreenMode,
        ResolutionWidth,
        ResolutionHeight,
        Language
    }

    public static Dictionary<Key, string> KeyToName = new()
    {
        { Key.GlobalVolume, "GLOBAL_VOLUME" },
        { Key.AmbientVolume, "AMBIENT_VOLUME" },
        { Key.EroVolume, "ERO_VOLUME" },
        { Key.MusicVolume, "MUSIC_VOLUME" },
        { Key.SFXVolume, "SFX_VOLUME" },
        { Key.UIVolume, "UI_VOLUME" },
        { Key.ScreenMode, "SCREEN_MODE" },
        { Key.ResolutionWidth, "RESOLUTION_WIDTH" },
        { Key.ResolutionHeight, "RESOLUTION_HEIGHT" },
        { Key.Language, "LANGUAGE" },
    };


}
