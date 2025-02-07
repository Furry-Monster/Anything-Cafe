using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
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
    [SerializeField] private Button _closeButton;
    [SerializeField] private TextMeshProUGUI _closeButtonText;

    private Sequence _sequence;
    private NoticeDialogModel _model;
    private NoticeDialogState _state;

    public void Init() => gameObject.SetActive(false);

    public void LoadTemplate(NoticeDialogModel model)
    {
        try
        {
            Debug.Log($"[LoadTemplate] {model.ToString()}");
            _model = model;
            _state = NoticeDialogState.Idling;

            // ���ð�ť 
            _closeButtonText.text = model.CloseButtonData.Text;
            _closeButton.interactable = model.CloseButtonData.IsInteractable;
            _closeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.AddListener(() =>
            {
                // ����ر�
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
    /// �򿪶Ի���
    /// </summary>
    /// <returns></returns>
    public override async UniTask Open()
    {
        _state = NoticeDialogState.Opening;

        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = ShowSequence();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();

        _state = NoticeDialogState.Idling;
    }

    /// <summary>
    /// �رնԻ���
    /// </summary>
    /// <returns></returns>
    public override async UniTask Close()
    {
        _state = NoticeDialogState.Closing;

        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = HideSequence();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();

        _state = NoticeDialogState.Idling;
    }

    /// <summary>
    ///     �򿪶�������
    /// </summary>
    /// <returns></returns>
    private Sequence ShowSequence()
    {
        return DOTween.Sequence()
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
            .Append(_canvasGroup.DOFade(1f, 0.3f))
            .Join(_contentPanel.transform.DOScale(1f, 0.35f).SetEase(Ease.OutBack));
    }

    /// <summary>
    ///     �رն�������
    /// </summary>
    /// <returns></returns>
    private Sequence HideSequence()
    {
        return DOTween.Sequence().OnPlay(() =>
        {
            _canvasGroup.interactable = false;
            _closeButton.interactable = false;
        }).OnKill(() =>
        {
            _canvasGroup.alpha = 1f;
            gameObject.SetActive(false);
        }).Append(_canvasGroup.DOFade(0.0f, 0.5f));
    }

    /// <summary>
    ///     ״̬ö��
    /// </summary>
    private enum NoticeDialogState
    {
        Opening,
        Idling,
        Closing,
    }

}

/// <summary>
///     ��ť����ģ��
/// </summary>
public class NoticeDialogModel : IDataTemplate
{
    public ButtonDataTemplate CloseButtonData;
}
