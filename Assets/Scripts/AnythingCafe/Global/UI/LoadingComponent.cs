using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class LoadingComponent : MonoBehaviour,ICommonComponent
{
    public abstract UniTask Show(float duration);

    public abstract UniTask Hide(float duration);
}
