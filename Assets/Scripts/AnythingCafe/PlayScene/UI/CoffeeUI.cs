using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class CoffeeUI : PlaySceneComponent, IInitializable
{
    [Header("General")]
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [Header("Components")]
    [SerializeField]
    private GameObject _table;
    [SerializeField]
    private GameObject _trashBtn;
    [SerializeField]
    private GameObject _letHerDrinkBtn;
    [SerializeField]
    private GameObject _drinkYourselfBtn;
    [SerializeField]
    private GameObject _coffeeCup;

    private Sequence _sequence;

    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        gameObject.SetActive(false);
    }

    public override async UniTask Open()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = ShowCoffeeUIAnimation();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    public override async UniTask Close()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = HideCoffeeUIAnimation();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    private Sequence ShowCoffeeUIAnimation()
    {
        return DOTween.Sequence()
            .OnPlay(() =>
            {
                gameObject.SetActive(true);
                _canvasGroup.interactable = false;
            })
            .OnKill(() =>
            {
                _canvasGroup.interactable = true;
            })
            .Append(_table.transform.DOLocalMoveY(0, 0.5f))
            .Join(_trashBtn.transform.DOLocalMoveY(0, 0.5f))
            .Join(_letHerDrinkBtn.transform.DOLocalMoveY(0, 0.5f))
            .Join(_drinkYourselfBtn.transform.DOLocalMoveY(0, 0.5f))
            .Join(_coffeeCup.transform.DOLocalMoveY(0, 0.5f));
    }

    private Sequence HideCoffeeUIAnimation()
    {
        return DOTween.Sequence()
        .OnPlay(() =>
        {
            _canvasGroup.interactable = false;
        })
        .OnKill(() =>
        {
            _canvasGroup.interactable = true;
            gameObject.SetActive(false);
        })
        .Append(_table.transform.DOLocalMoveY(-100, 0.5f))
        .Join(_trashBtn.transform.DOLocalMoveY(-100, 0.5f))
        .Join(_letHerDrinkBtn.transform.DOLocalMoveY(-100, 0.5f))
        .Join(_drinkYourselfBtn.transform.DOLocalMoveY(-100, 0.5f))
        .Join(_coffeeCup.transform.DOLocalMoveY(-100, 0.5f));
    }

    public async UniTask OnTrashClick()
    {
        var data = new ContentDialogModel
        {
            LeftButtonData = new ButtonDataTemplate("Yes", null, true),
            RightButtonData = new ButtonDataTemplate("No", null, false)
        };
        await UIManager.Instance.OpenGlobal<ContentDialog, ContentDialogModel>(data);
    }

    public void OnLetHerDrinkClick()
    {

    }

    public void OnDrinkYourselfClick()
    {

    }
}
