using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class WarningUI :
    TitleSceneComponent,
    IInitializable
{
    [Header("General")]
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [Header("Components")]
    [SerializeField]
    private Text _text;

    private Sequence _sequence;
    public Action OnTextClosed; // warningUI关闭回调

    public bool IsInitialized { get; set; }

    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Open the warning UI.
    /// </summary>
    /// <returns> UniTask </returns>
    public override async UniTask Open()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = TextEmerge();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    /// <summary>
    /// Close the warning UI.
    /// </summary>
    /// <returns> UniTask </returns>
    public override async UniTask Close()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = FadeOut();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    private Sequence TextEmerge() =>
        DOTween.Sequence()
            .OnPlay(() =>
            {
                gameObject.SetActive(true);
                _canvasGroup.alpha = 1;
                _canvasGroup.interactable = false;
                _text.color = Color.black;
            })
            .Append(_text.DOColor(Color.red, 1f));

    /// <summary>
    /// Start the fade out animation.
    /// </summary>
    /// <returns> Sequence </returns>
    private Sequence FadeOut() =>
        DOTween.Sequence()
            .OnKill(() => gameObject.SetActive(false))
            .Append(_text.DOColor(Color.black, 1.0f))
            .InsertCallback(1.0f, () => OnTextClosed?.Invoke())
            .InsertCallback(1.1f, () => _text.gameObject.SetActive(false)) // 关闭文字，避免错误显示
            .Append(_canvasGroup.DOFade(0, 0.5f));
}