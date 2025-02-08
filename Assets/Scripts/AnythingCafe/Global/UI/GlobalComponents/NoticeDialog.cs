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

    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        gameObject.SetActive(false);
    }

    public void LoadTemplate(NoticeDialogModel model)
    {
        try
        {
            Debug.Log($"[LoadTemplate] {model.ToString()}");
            _model = model;

            // 设置按钮 
            _closeButtonText.text = model.CloseButtonData.Text;
            _closeButton.interactable = model.CloseButtonData.IsInteractable;
            _closeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.AddListener(() => _ = UIManager.Instance.CloseReactive(this));
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
    /// 打开对话框
    /// </summary>
    /// <returns></returns>
    public override async UniTask Open()
    {

        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = ShowSequence();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    /// <summary>
    /// 关闭对话框
    /// </summary>
    /// <returns></returns>
    public override async UniTask Close()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = HideSequence();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    /// <summary>
    ///     打开动画序列
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
    ///     关闭动画序列
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
}

/// <summary>
///     按钮数据模板
/// </summary>
public class NoticeDialogModel : IDataTemplate
{
    public ButtonDataTemplate CloseButtonData;
}
