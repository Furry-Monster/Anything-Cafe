using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class MenuUI : PlaySceneComponent, IInitializable
{
    [Header("General")]
    private CanvasGroup _canvasGroup;

    private Sequence _sequence;

    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        gameObject.SetActive(false);
    }

    public override async UniTask Open()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = DropDownAnimation();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    public override async UniTask Close()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = RiseUpAnimation();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    private Sequence DropDownAnimation()
    {
        return DOTween.Sequence().OnPlay(() =>
        {
            gameObject.SetActive(true);
            _canvasGroup.interactable = false;
        }).OnKill(() =>
        {
            _canvasGroup.interactable = true;
        }).Append(transform.DOLocalMoveY(0, 0.5f, false).SetEase(Ease.OutCubic));
    }

    private Sequence RiseUpAnimation()
    {
        return DOTween.Sequence().OnPlay(() =>
            {
                _canvasGroup.interactable = false;
            }).OnKill(() =>
            {
                gameObject.SetActive(false);
            }).Append(transform.DOLocalMoveY(1700, 0.5f, false).SetEase(Ease.OutCubic));
    }

    public void OnContinueClick()
    {
        _ = UIManager.Instance.CloseReactive(this);
    }

    public async void OnMainMenuClick()
    {
        try
        {
            await UIManager.Instance.CloseReactive(this);
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
