using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ReactiveComponent����UniTask��ʵ��
/// </summary>
public abstract class ReactiveComponent : MonoBehaviour, IReactiveComponent
{
    public abstract UniTask Open();

    public abstract UniTask Close();
}
