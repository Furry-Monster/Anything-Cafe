using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WarningUI :
    TitleComponent,
    IInitializable
{
    [Header("General")]
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [Header("Components")]
    [SerializeField]
    private Text _text;

    private Sequence _sequence;
    public Action OnCloseEnd; // warningUI¹Ø±Õ»Øµ÷

    public void Init() => gameObject.SetActive(false);

    /// <summary>
    /// Open the warning UI.
    /// </summary>
    /// <returns> UniTask </returns>
    public override async UniTask Open()
    {
        gameObject.SetActive(true);

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
        OnCloseEnd?.Invoke();

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
            .Append(_text.DOColor(Color.red, 2f));

    /// <summary>
    /// Start the fade out animation.
    /// </summary>
    /// <returns> Sequence </returns>
    private Sequence FadeOut() =>
        DOTween.Sequence()
            .OnKill(() =>
            {
                _canvasGroup.interactable = true;
                _canvasGroup.alpha = 1;
                gameObject.SetActive(false);
            })
            .Append(_text.DOColor(Color.black, 2f))
            .Append(_canvasGroup.DOFade(0, 0.5f))
            .InsertCallback(2f, () => OnCloseEnd?.Invoke());
}