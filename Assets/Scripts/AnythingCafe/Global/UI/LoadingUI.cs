using Cysharp.Threading.Tasks;
using UnityEngine;

public enum LoadingUIType
{
    BlackFade,
    LoadingBar,
}

public abstract class LoadingUI : MonoBehaviour,ICommonUI
{
    public abstract UniTask Show(float duration);

    public abstract UniTask Hide(float duration);
}
