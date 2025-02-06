using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentDialog :
    GlobalComponent,
    IInitializable,
    IHasDataTemplate<ContentDialogModel>
{
    [Header("General")]
    [SerializeField] private CanvasGroup _canvasGroup;

    [Space]
    [Header("Components")]
    [SerializeField] private GameObject _contentPanel;
    [SerializeField] private Button _leftBtn;
    [SerializeField] private TextMeshProUGUI _leftBtnText;
    [SerializeField] private Button _rightBtn;
    [SerializeField] private TextMeshProUGUI _rightBtnText;

    private Sequence _sequence;
    private ContentDialogModel _model;
    private ContentDialogState _state;

    public void Init() => gameObject.SetActive(false);

    public void LoadTemplate(ContentDialogModel model)
    {
        try
        {
            _model = model;
            _state = ContentDialogState.Idling;

            // 设置左按钮
            _leftBtnText.text = model.LeftButtonData.Text;
            _leftBtn.interactable = model.LeftButtonData.IsInteractable;
            _leftBtn.onClick.RemoveAllListeners();
            _leftBtn.onClick.AddListener(() =>
            {
                // 请求关闭
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
            });

            // 设置右按钮
            _rightBtnText.text = model.RightButtonData.Text;
            _rightBtn.interactable = model.RightButtonData.IsInteractable;
            _rightBtn.onClick.RemoveAllListeners();
            _rightBtn.onClick.AddListener(() =>
            {
                // 请求关闭
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
            });
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException)
                throw;
            throw new CustomErrorException($"[ContentDialog] Can't init Component. {ex.GetType()}, {ex.Message}",
                new CustomErrorItem(ErrorSeverity.Error, ErrorCode.ComponentInitFailed));
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
        _sequence = ShowSequence().Play();
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
        _sequence = HideSequence().Play();
        await _sequence.AsyncWaitForCompletion();

        _state = ContentDialogState.Idling;
    }

    /// <summary>
    /// Open 动画序列
    /// </summary>
    /// <returns></returns>
    private Sequence ShowSequence()
    {
        return DOTween.Sequence().OnKill(() =>
        {
            _canvasGroup.interactable = true;
            _leftBtn.interactable = _model.LeftButtonData.IsInteractable;
            _rightBtn.interactable = _model.RightButtonData.IsInteractable;
            _contentPanel.transform.localScale = Vector3.one;
        }).OnPlay(() =>
        {
            _canvasGroup.alpha = 0.0f;
            _canvasGroup.interactable = false;
            _leftBtn.interactable = false;
            _rightBtn.interactable = false;
            _contentPanel.transform.localScale = Vector3.one * 0.7f;
            gameObject.SetActive(true);
        }).Append(_canvasGroup.DOFade(1f, 0.3f))
            .Join(_contentPanel.transform.DOScale(1f, 0.35f).SetEase(Ease.OutBack));
    }

    /// <summary>
    /// Close 动画序列
    /// </summary>
    /// <returns></returns>
    private Sequence HideSequence()
    {
        return DOTween.Sequence().OnPlay(() =>
        {
            _canvasGroup.interactable = false;
            _leftBtn.interactable = false;
            _rightBtn.interactable = false;
        }).OnKill(() =>
        {
            _canvasGroup.alpha = 1f;
            gameObject.SetActive(false);
        }).Append(_canvasGroup.DOFade(0.0f, 0.5f));
    }

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
    public ButtonDataTemplate LeftButtonData;
    public ButtonDataTemplate RightButtonData;
}