using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class ClosableComponent : MonoBehaviour, IReactiveComponent
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
