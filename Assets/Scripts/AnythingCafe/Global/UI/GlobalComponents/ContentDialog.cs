using Cysharp.Threading.Tasks;
using DG.Tweening;
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

    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        gameObject.SetActive(false);
    }

    public void LoadTemplate(ContentDialogModel model)
    {
        try
        {
            _model = model;

            // ������ť
            _leftBtnText.text = model.LeftButtonData.Text;
            _leftBtn.interactable = model.LeftButtonData.IsInteractable;
            _leftBtn.onClick.RemoveAllListeners();
            _leftBtn.onClick.AddListener(() => _ = UIManager.Instance.CloseReactive(this));

            // �����Ұ�ť
            _rightBtnText.text = model.RightButtonData.Text;
            _rightBtn.interactable = model.RightButtonData.IsInteractable;
            _rightBtn.onClick.RemoveAllListeners();
            _rightBtn.onClick.AddListener(() => _ = UIManager.Instance.CloseReactive(this));
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
    /// ��ContentDialog
    /// </summary>
    /// <returns></returns>
    public override async UniTask Open()
    {
        // ��ʾ����
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = ShowSequence().Play();
        await _sequence.AsyncWaitForCompletion();
    }

    /// <summary>
    ///  �ر�ContentDialog
    /// </summary>
    /// <returns></returns>
    public override async UniTask Close()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = HideSequence().Play();
        await _sequence.AsyncWaitForCompletion();
    }

    /// <summary>
    /// Open ��������
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
    /// Close ��������
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
            _canvasGroup.interactable = true;
            gameObject.SetActive(false);
        }).Append(_canvasGroup.DOFade(0.0f, 0.5f));
    }
}

/// <summary>
/// ContentDialog����ģ��
/// </summary>
public class ContentDialogModel : IDataTemplate
{
    public ButtonDataTemplate LeftButtonData;
    public ButtonDataTemplate RightButtonData;
}