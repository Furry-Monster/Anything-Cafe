using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

public class TitleUI :
    TitleSceneComponent,
    IInitializable
{
    [Header("General")]
    [SerializeField]
    private CanvasGroup _canvasGroup;

    private Sequence _sequence;

    public bool IsInitialized { get; set; }

    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

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
            // TODO:让模型可以在外部被编辑
            var noticeData = new NoticeDialogModel
            {
                CloseButtonData =
                {
                    IsInteractable = true,
                    Text = "I Know"
                }
            };
            await UIManager.Instance.OpenGlobal<NoticeDialog, NoticeDialogModel>(noticeData);
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
