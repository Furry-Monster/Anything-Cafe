using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class PlotObject : MonoBehaviour, IInteractable, IConditionalShow
{
    private bool _interactable = true;
    private bool _isInteracting;

    [SerializeField] private UnityEvent _onInteractionStart = new();
    [SerializeField] private UnityEvent _onInteractionEnd = new();

    public virtual bool Interactable
    {
        get => _interactable;
        set
        {
            _interactable = value;
            OnInteractableChanged();
        }
    }

    public bool IsInteracting
    {
        get => _isInteracting;
        protected set => _isInteracting = value;
    }

    public UnityEvent OnInteractionStart => _onInteractionStart;
    public UnityEvent OnInteractionEnd => _onInteractionEnd;

    protected virtual void Start()
    {
        ValidateComponents();
    }

    protected virtual void ValidateComponents()
    {
        var colliderComponent = GetComponent<Collider2D>();
        if (colliderComponent != null && !colliderComponent.isTrigger)
        {
            colliderComponent.isTrigger = true;
        }
    }

    protected virtual void OnInteractableChanged()
    {
        // 子类可以重写此方法来响应可交互状态的变化
    }

    public virtual void OnInteract()
    {
        if (!Interactable) return;
        OnInteractionBegin();
    }

    public virtual void OnInteractionBegin()
    {
        if (!Interactable || IsInteracting) return;
        IsInteracting = true;
        OnInteractionStart?.Invoke();
    }

    public virtual void OnInteractionComplete()
    {
        if (!IsInteracting) return;
        IsInteracting = false;
        OnInteractionEnd?.Invoke();
    }

    public virtual void OnInteractionCancel()
    {
        if (!IsInteracting) return;
        IsInteracting = false;
        OnInteractionEnd?.Invoke();
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        if (IsInteracting)
        {
            OnInteractionCancel();
        }
    }
}
