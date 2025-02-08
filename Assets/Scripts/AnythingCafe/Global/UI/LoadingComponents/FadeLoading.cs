using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class FadeLoading :
    LoadingComponent,
    IInitializable,
    IHasDataTemplate<FadeLoadingModel>
{
    [Header("General")]
    [SerializeField] private CanvasGroup _canvasGroup;

    [Space]
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI _messageText;

    private Sequence _sequence;
    private FadeLoadingModel _model;

    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        gameObject.SetActive(false);
    }

    public void LoadTemplate(FadeLoadingModel model)
    {
        _model = model;

        _messageText.text = _model.Message;
    }

    public override async UniTask Open()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = ShowSequence();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    public override async UniTask Close()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = HideSequence();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    /// <summary>
    /// Fade打开Loading
    /// </summary>
    /// <returns> 动画序列 </returns>
    private Sequence ShowSequence()
    {
        return DOTween.Sequence().OnPlay(() =>
        {
            _canvasGroup.interactable = false;
        }).OnKill(() =>
        {
            _canvasGroup.interactable = true;
        }).Append(_canvasGroup.DOFade(1, _model.FadeInDuration));
    }

    /// <summary>
    /// Fade关闭Loading
    /// </summary>
    /// <returns> 动画序列 </returns>
    private Sequence HideSequence()
    {
        return DOTween.Sequence().OnPlay(() =>
        {
            _canvasGroup.interactable = false;
        }).Append(_canvasGroup.DOFade(0, _model.FadeOutDuration));
    }
}

public class FadeLoadingModel : IDataTemplate
{
    public string Message;
    public float FadeInDuration;
    public float FadeOutDuration;
}