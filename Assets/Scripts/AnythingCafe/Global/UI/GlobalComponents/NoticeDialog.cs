using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NoticeDialog :
    GlobalComponent,
    IInitializable,
    IHasDataTemplate<NoticeDialogModel>
{
    [Header("General")]
    [SerializeField] private CanvasGroup _canvasGroup;

    [Space]
    [Header("Components")]
    [SerializeField] private GameObject _contentPanel;
    [SerializeField] private Text _text;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Text _closeButtonText;

    private Sequence _sequence;
    private NoticeDialogModel _model;
    private NoticeDialogState _state;

    public void Init() => gameObject.SetActive(false);

    public void Init(NoticeDialogModel model)
    {
        try
        {
            _model = model;
            _state = NoticeDialogState.Idling;

            _text.text = model.Text;
            _closeButtonText.text = model.CloseButtonData.Text;
            _closeButton.interactable = model.CloseButtonData.IsInteractable;
            _closeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.AddListener(() =>
            {
                // 请求关闭
                switch (_state)
                {
                    case NoticeDialogState.Opening:
                    case NoticeDialogState.Closing:
                        break;
                    case NoticeDialogState.Idling:
                    default:
                        _ = UIManager.Instance.CloseReactive(this);
                        break;
                }
            });
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException)
                throw;
            throw new CustomErrorException($"[NoticeDialog] Can't init Component. {ex.GetType()}, {ex.Message}",
                new CustomErrorItem(ErrorSeverity.Error, ErrorCode.ComponentInitFailed));
        }
    }


    /// <summary>
    ///     打开对话框
    /// </summary>
    /// <returns></returns>
    public override async UniTask Open()
    {
        _state = NoticeDialogState.Opening;

        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = (Sequence)OpenSequence();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();

        _state = NoticeDialogState.Idling;
    }

    /// <summary>
    ///     关闭对话框
    /// </summary>
    /// <returns></returns>
    public override async UniTask Close()
    {
        _state = NoticeDialogState.Closing;

        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = (Sequence)CloseSequence();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();

        _state = NoticeDialogState.Idling;
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
    private enum NoticeDialogState
    {
        Opening,
        Idling,
        Closing,
    }

}

/// <summary>
///     按钮数据模板
/// </summary>
public class NoticeDialogModel : IDataTemplate
{
    public string Text;
    public ButtonDataTemplate CloseButtonData;
}
