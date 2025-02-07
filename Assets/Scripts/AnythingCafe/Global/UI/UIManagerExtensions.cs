using Cysharp.Threading.Tasks;

public static class UIManagerExtensions
{
    /// <summary>
    /// 显示Loading UI
    /// </summary>
    /// <param name="uiManager">UIManager实例</param>
    /// <param name="uiName">UI名称</param>
    /// <returns>UniTask</returns>
    public static async UniTask ShowLoading(this UIManager uiManager, string uiName)
    {
        await uiManager.OpenLoading(uiName);
    }

    /// <summary>
    /// 隐藏Loading UI
    /// </summary>
    /// <param name="uiManager">UIManager实例</param>
    /// <param name="uiName">UI名称</param>
    /// <returns>UniTask</returns>
    public static async UniTask HideLoading(this UIManager uiManager, string uiName)
    {
        await uiManager.CloseLoading(uiName);
    }
}