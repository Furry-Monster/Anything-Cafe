public static class OptionValidator
{
    public static bool ValidateVolume(float value) => value >= 0f && value <= 1f;

    public static bool ValidateResolution(int width, int height)
    {
        return width >= 800 && width <= 7680 &&  // 支持8K
               height >= 600 && height <= 4320;
    }

    public static bool ValidateLanguage(string language)
    {
        return !string.IsNullOrEmpty(language) && language.Length == 2;
    }
}