using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI :
    TitleSceneComponent,
    IInitializable
{
    [Header("General")]
    [SerializeField]
    private CanvasGroup _canvasGroup;

    private Sequence _sequence;

    public void Init()
    {
        gameObject.SetActive(true);
    }

    public override async UniTask Open()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = DOTween.Sequence()
            .OnPlay(() =>
            {
                _canvasGroup.interactable = true;
                gameObject.SetActive(true);
            })
            .Append(_canvasGroup.DOFade(1, 0.5f));
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    public async void OnThanksClick()
    {
        try
        {
            await UIManager.Instance.OpenReactive<ThanksUI>();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[TitleUI] {ex.Message}");
        }
    }

    public async void OnSupportClick()
    {
        try
        {
            await UIManager.Instance.OpenReactive<SupportUI>();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[TitleUI] {ex.Message}");
        }
    }

    public async void OnNoticeClick()
    {
        try
        {
            await UIManager.Instance.OpenGlobal<NoticeDialog>();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[TitleUI] {ex.Message}");
        }
    }

    public void OnAboutClick()
    {
        Application.OpenURL("https://www.google.com");
    }

    public async void OnStartClick()
    {
        try
        {
            await GameSceneManager.Instance.LoadScene(SceneID.PlayScene);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[TitleUI] {e.Message}");
        }
    }

    public void OnContinueClick()
    {

    }

    public async void OnGalleryClick()
    {
        try
        {
            await GameSceneManager.Instance.LoadScene(SceneID.GalleryScene);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[TitleUI] {ex.Message}");
        }
    }

    public async void OnSettingClick()
    {
        try
        {
            await UIManager.Instance.OpenGlobal<SettingPanel>();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[TitleUI] {ex.Message}");
        }
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
