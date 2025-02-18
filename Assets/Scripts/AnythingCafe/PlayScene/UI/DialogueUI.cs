using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI :
    PlaySceneComponent,
    IInitializable,
    IHasDataTemplate<DialogueUIModel>
{
    [Header("General")]
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [Header("Components")]
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private Button _nextBtn;

    private Sequence _sequence;
    private DialogueUIModel _model;

    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        gameObject.SetActive(false);
    }

    public void LoadTemplate(DialogueUIModel model)
    {
        try
        {
            _model = model;

            _text.text = model.Text;

            _nextBtn.interactable = model.NextBtnData.IsInteractable;
            _nextBtn.onClick.RemoveAllListeners();
            _nextBtn.onClick.AddListener(() =>
            {
                _model.NextBtnData.OnClick?.Invoke();
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

    public override async UniTask Open()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = DropDown();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    public override async UniTask Close()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = RiseUp();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    private Sequence DropDown()
    {
        return DOTween.Sequence().OnPlay(() =>
        {
            gameObject.SetActive(true);
            _canvasGroup.interactable = false;
        }).OnKill(() =>
        {
            _canvasGroup.interactable = true;
        }).Append(transform.DOLocalMoveY(165, 0.5f).SetEase(Ease.OutBack));
    }

    private Sequence RiseUp()
    {
        return DOTween.Sequence().OnPlay(() =>
        {
            _canvasGroup.interactable = false;
        }).OnKill(() =>
        {
            _canvasGroup.interactable = true;
            gameObject.SetActive(false);
        }).Append(transform.DOLocalMoveY(375, 0.5f).SetEase(Ease.OutBack));
    }

}

/// <summary>
/// DialogueUI的数据模型
/// </summary>
public class DialogueUIModel : IDataTemplate
{
    public string Text { get; set; }
    public ButtonDataTemplate NextBtnData { get; set; }
}