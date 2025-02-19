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
    /// �����Ҫ����һ�����������ֶ����ش˷���
    /// </summary>
    public virtual void Show() => gameObject.SetActive(true);

    /// <summary>
    /// �����Ҫ����һ�����������ֶ����ش˷���
    /// </summary>
    public virtual void Hide() => gameObject.SetActive(false);
}
