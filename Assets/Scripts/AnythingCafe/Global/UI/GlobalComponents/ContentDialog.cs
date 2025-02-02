using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ContentDialog :
    GlobalComponent,
    IInitializable,
    IHasDataTemplate<ContentDialogModel>
{
    [SerializeField] private CanvasGroup _canvasGroup;

    private Text _text;
    private Button _leftBtn;
    private Text _leftBtnText;
    private Button _rightBtn;
    private Text _rightBtnText;

    private Sequence _sequence;
    private ContentDialogModel _model;
    private ContentDialogState _state;

    public void Init() => gameObject.SetActive(false);

    public void Init(ContentDialogModel model)
    {
        try
        {
            _model = model;
            _state = ContentDialogState.Idling;

            _text.text = model.Text;

            // 设置左按钮
            _leftBtnText.text = model.LeftButtonData.Text;
            _leftBtn.interactable = model.LeftButtonData.IsInteractable;
            _leftBtn.onClick.RemoveAllListeners();
            _leftBtn.onClick.AddListener(OnCloseRequest);

            // 设置右按钮
            _rightBtnText.text = model.RightButtonData.Text;
            _rightBtn.interactable = model.RightButtonData.IsInteractable;
            _rightBtn.onClick.RemoveAllListeners();
            _rightBtn.onClick.AddListener(OnCloseRequest);
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException)
                throw;
            throw new CustomErrorException($"[ContentDialog] Can't init Component. {ex.GetType()}, {ex.Message}",
                new CustomErrorItem(ErrorSeverity.Error, ErrorCode.ComponentInitFailed));
        }
    }

    private void OnCloseRequest()
    {
        _model.LeftButtonData.OnClick?.Invoke();
        switch (_state)
        {
            case ContentDialogState.Opening:
            case ContentDialogState.Closing:
                break;
            case ContentDialogState.Idling:
            default:
                _ = UIManager.Instance.CloseReactive(this);
                break;
        }
    }

    /// <summary>
    /// 打开ContentDialog
    /// </summary>
    /// <returns></returns>
    public override async UniTask Open()
    {
        _state = ContentDialogState.Opening;

        // 显示动画
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = (Sequence)OpenSequence().Play();
        await _sequence.AsyncWaitForCompletion();

        _state = ContentDialogState.Idling;
    }

    /// <summary>
    ///  关闭ContentDialog
    /// </summary>
    /// <returns></returns>
    public override async UniTask Close()
    {
        // 切换状态
        _state = ContentDialogState.Closing;

        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = (Sequence)CloseSequence().Play();
        await _sequence.AsyncWaitForCompletion();

        _state = ContentDialogState.Idling;
    }

    /// <summary>
    /// 打开动画序列
    /// </summary>
    /// <returns></returns>
    private Tween OpenSequence() =>
        DOTween.Sequence()
            .OnPlay(() =>
            {
                _canvasGroup.interactable = false;
                _leftBtn.interactable = false;
                _rightBtn.interactable = false;
            })
            .OnKill(() =>
            {
                _canvasGroup.alpha = 1f;
                gameObject.SetActive(false);
            })
            .Append(_canvasGroup.DOFade(0.0f, 0.5f));

    /// <summary>
    /// 关闭动画序列
    /// </summary>
    /// <returns></returns>
    private Tween CloseSequence() =>
        DOTween.Sequence()
            .OnPlay(() =>
            {
                _canvasGroup.interactable = false;
                _leftBtn.interactable = false;
                _rightBtn.interactable = false;
            })
            .OnKill(() =>
            {
                _canvasGroup.alpha = 1f;
                gameObject.SetActive(false);
            }).Append(_canvasGroup.DOFade(0.0f, 0.5f));

    /// <summary>
    /// ContentDialog状态枚举
    /// </summary>
    private enum ContentDialogState
    {
        Opening,
        Idling,
        Closing,
    }
}

/// <summary>
/// ContentDialog数据模型
/// </summary>
public class ContentDialogModel : IDataTemplate
{
    public string Text;
    public ButtonDataTemplate LeftButtonData;
    public ButtonDataTemplate RightButtonData;
}