using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NotifyDialog :
    ReactiveComponent,
    IInitializable,
    IHasDataTemplate<NotifyDialogModel>
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _contentPanel;

    private Text _text;
    private Button _closeButton;
    private Text _closeButtonText;

    private Sequence _sequence;
    private NotifyDialogModel _model;
    private NotifyDialogState _state;

    public void Init()
    {
        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(() => _ = UIManager.Instance.CloseReactiveComponent(this));
        gameObject.SetActive(false);
    }

    public void Init(NotifyDialogModel model)
    {
        _model = model;
        _state = NotifyDialogState.Idling;

        _text.text = model.Text;
        _closeButtonText.text = model.CloseButtonData.Text;
        _closeButton.interactable = model.CloseButtonData.IsInteractable;
        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(() =>
        {
            // 请求关闭
            switch (_state)
            {
                case NotifyDialogState.Opening:
                case NotifyDialogState.Closing:
                    break;
                case NotifyDialogState.Idling:
                default:
                    _ = UIManager.Instance.CloseReactiveComponent(this);
                    break;
            }
        });
    }

    /// <summary>
    ///     打开对话框
    /// </summary>
    /// <returns></returns>
    public override async UniTask Open()
    {
        _state = NotifyDialogState.Opening;

        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = (Sequence)OpenSequence();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();

        _state = NotifyDialogState.Idling;
    }

    /// <summary>
    ///     关闭对话框
    /// </summary>
    /// <returns></returns>
    public override async UniTask Close()
    {
        _state = NotifyDialogState.Closing;

        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = (Sequence)CloseSequence();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();

        _state = NotifyDialogState.Idling;
    }

    /// <summary>
    ///     打开动画序列
    /// </summary>
    /// <returns></returns>
    private Tween OpenSequence() =>
        DOTween.Sequence()
            .OnKill(() =>
            {
                _canvasGroup.interactable = true;
                _closeButton.interactable = _model.CloseButtonData.IsInteractable;
                _contentPanel.transform.localScale = Vector3.one;
            })
            .OnPlay(() =>
            {
                _canvasGroup.alpha = 0.0f;
                _canvasGroup.interactable = false;
                _closeButton.interactable = false;
                _contentPanel.transform.localScale = Vector3.one * 0.7f;
                gameObject.SetActive(true);
            })
            .Append(_canvasGroup
                    .DOFade(1f, 0.3f))
                    .Join(_contentPanel.transform.DOScale(1f, 0.35f)
                    .SetEase(Ease.OutBack));

    /// <summary>
    ///     关闭动画序列
    /// </summary>
    /// <returns></returns>
    private Tween CloseSequence() =>
        DOTween.Sequence().OnPlay(() =>
        {
            _canvasGroup.interactable = false;
            _closeButton.interactable = false;
        }).OnKill(() =>
        {
            _canvasGroup.alpha = 1f;
            gameObject.SetActive(false);
        }).Append(_canvasGroup.DOFade(0.0f, 0.5f));

    /// <summary>
    ///     状态枚举
    /// </summary>
    private enum NotifyDialogState
    {
        Opening,
        Idling,
        Closing,
    }

}

/// <summary>
///     按钮数据模板
/// </summary>
public class NotifyDialogModel
{
    public string Text;
    public ButtonDataTemplate CloseButtonData;
}
