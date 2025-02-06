using System.Collections;
using System.Collections.Generic;
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
    private FadeLoadingState _state;

    public void Init() => gameObject.SetActive(false);

    public void LoadTemplate(FadeLoadingModel model)
    {
        _model = model;
        _state = FadeLoadingState.Idling;

        _messageText.text = _model.Message;
    }

    public async override UniTask Open()
    {
        
    }

    public async override UniTask Close()
    {

    }

    

    private enum FadeLoadingState
    {
        Opening,
        Idling,
        Closing,
    }
}

public class FadeLoadingModel : IDataTemplate
{
    public string Message;
}