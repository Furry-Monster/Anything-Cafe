using UnityEngine;

public class PlotObject : MonoBehaviour, IInteractable, IConditionalShow
{
    private bool _interactable;

    public virtual bool Interactable
    {
        get => _interactable;
        set => _interactable = value;
    }

    /// <summary>
    /// 如果需要满足一定条件，则手动重载此方法
    /// </summary>
    public virtual void Show() => gameObject.SetActive(true);

    /// <summary>
    /// 如果需要满足一定条件，则手动重载此方法
    /// </summary>
    public virtual void Hide() => gameObject.SetActive(false);
}
