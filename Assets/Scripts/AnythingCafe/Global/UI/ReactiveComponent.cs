using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ReactiveComponent基于UniTask的实现
/// </summary>
public abstract class ReactiveComponent : MonoBehaviour, IReactiveComponent
{
    public abstract UniTask Open();

    public abstract UniTask Close();
}
