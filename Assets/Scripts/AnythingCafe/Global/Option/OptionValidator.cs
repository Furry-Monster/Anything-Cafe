public static class OptionValidator
{
    /// <summary>
    /// 验证音量值是否在0~1之间
    /// </summary>
    /// <param name="value"> 音量值 </param>
    /// <returns></returns>
    public static bool ValidateVolume(float value) => value is >= 0f and <= 1f;

    /// <summary>
    /// 验证分辨率是否在800x600~7680x4320之间
    /// </summary>
    /// <param name="width"> 宽度 </param>
    /// <param name="height"> 高度 </param>
    /// <returns></returns>
    public static bool ValidateResolution(int width, int height) =>
        width is >= 800 and <= 7680 &&  // 支持8K
        height is >= 600 and <= 4320;

    /// <summary>
    /// 验证语言是否为2位字符串
    /// </summary>
    /// <param name="language"> 语言 </param>
    /// <returns> 是否为2位字符串 </returns>
    public static bool ValidateLanguage(string language) => !string.IsNullOrEmpty(language) && language.Length == 2;
}