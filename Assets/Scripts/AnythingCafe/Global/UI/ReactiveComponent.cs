using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ReactiveComponent基于UniTask的实现
/// </summary>
public abstract class ReactiveComponent : MonoBehaviour, IReactiveComponent
{
    public virtual UniTask Open()
    {
        this.gameObject.SetActive(true);
        return new UniTask();
    }

    public virtual UniTask Close()
    {
        this.gameObject.SetActive(false);
        return new UniTask();
    }
}
