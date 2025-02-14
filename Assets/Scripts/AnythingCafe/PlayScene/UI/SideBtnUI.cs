using System;
using UnityEngine;

public class SideBtnUI : PlaySceneComponent, IInitializable
{
    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        gameObject.SetActive(false);
    }

    public async void OnMenuClick()
    {
        try
        {
            await UIManager.Instance.OpenReactive<MenuUI>();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[SideBtnUI] {ex.Message}");
        }
    }

    public async void OnDiscoverClick()
    {
        try
        {
            await UIManager.Instance.OpenReactive<DiscoverUI>();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[SideBtnUI] {ex.Message}");
        }
    }

    public void OnHintClick()
    {

    }
}
