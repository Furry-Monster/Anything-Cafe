using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FadeLoading :
    LoadingComponent,
    IInitializable,
    IHasDataTemplate<FadeLoadingModel>
{
    [SerializeField] private CanvasGroup _canvasGroup;

    public void Init()
    {

    }

    public void Init(FadeLoadingModel param)
    {

    }

    public override UniTask Open()
    {
        return base.Open();
    }

    public override UniTask Close()
    {
        return base.Close();
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
    private string _message;
}