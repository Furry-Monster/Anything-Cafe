using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class WarningUI :
    TitleComponent,
    IInitializable
{
    [Header("General")]
    [SerializeField]
    private CanvasGroup _canvasGroup;

    private Sequence _sequence;

    public void Init() => gameObject.SetActive(false);

    /// <summary>
    /// Open the warning UI.
    /// </summary>
    /// <returns></returns>
    public override async UniTask Open()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = FadeIn();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    /// <summary>
    /// Close the warning UI.
    /// </summary>
    /// <returns></returns>
    public override async UniTask Close()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = FadeOut();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    /// <summary>
    /// Start the fade in animation.
    /// </summary>
    /// <returns></returns>
    private Sequence FadeIn() =>
        DOTween.Sequence()
            .OnPlay(() => _canvasGroup.interactable = false)
            .Append(_canvasGroup.DOFade(1, 0.5f));

    /// <summary>
    /// Start the fade out animation.
    /// </summary>
    /// <returns></returns>
    private Sequence FadeOut() =>
        DOTween.Sequence()
            .OnKill(() => _canvasGroup.interactable = true)
            .Append(_canvasGroup.DOFade(0, 0.5f));
}