using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MenuUI : PlaySceneComponent, IInitializable
{
    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        gameObject.SetActive(false);
    }

    public override UniTask Open()
    {
        return base.Open();
    }

    public override UniTask Close()
    {
        return base.Close();
    }

    public void OnContinueClick()
    {
        _ = UIManager.Instance.CloseReactive(this);
    }

    public async void OnMainMenuClick()
    {
        try
        {
            await GameSceneManager.Instance.LoadScene(SceneID.TitleScene);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[TitleUI] {e.Message}");
        }
    }

    public async void OnSettingClick()
    {
        try
        {
            await UIManager.Instance.CloseReactive(this);
            await UIManager.Instance.OpenGlobal<SettingPanel>();
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException)
                throw;
            throw new CustomErrorException($"[MenuUI] Cant open Setting Panel bcz:{ex}",
                new CustomErrorItem(ErrorSeverity.Error, ErrorCode.UICantOpen));
        }
    }
}
