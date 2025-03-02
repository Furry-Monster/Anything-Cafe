using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class DynamicObject : MonoBehaviour, IInteractable, ICanBeConditional
{
    private bool _interactable = true;
    private bool _isInteracting;

    [SerializeField] private UnityEvent _onInteractionStart = new();
    [SerializeField] private UnityEvent _onInteractionEnd = new();
    [SerializeField] private UnityEvent _onConditionsFailed = new();

    private bool _isConditional;
    private ConditionGroup _conditionGroup;

    public bool Interactable
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
    public UnityEvent OnConditionsFailed => _onConditionsFailed;

    protected virtual void Awake()
    {
        ValidateComponents();
    }

    /// <summary>
    /// 组件验证
    /// </summary>
    protected virtual void ValidateComponents()
    {
        _conditionGroup = GetComponent<ConditionGroup>();

        _isConditional = _conditionGroup != null;

        var colliderComponent = GetComponent<Collider2D>();

        if (colliderComponent != null && !colliderComponent.isTrigger)
        {
            colliderComponent.isTrigger = true;
        }
    }

    /// <summary>
    /// 子类可以重写此方法来响应可交互状态的变化
    /// </summary>
    protected virtual void OnInteractableChanged()
    {
        if (Interactable) Show();
        else Hide();
    }

    public virtual void OnInteract()
    {
        if (!Interactable) return;

        if (_isConditional && !CheckConditions())
        {
            OnConditionsFailed?.Invoke();
            return;
        }

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

    protected bool CheckConditions() => _conditionGroup == null || _conditionGroup.IsMet();

    public string GetFailureReason() => _conditionGroup.GetFailureReason() ?? string.Empty;

    public void AddCondition(ScriptableObject condition) => _conditionGroup.AddCondition(condition);

    public void RemoveCondition(ScriptableObject condition) => _conditionGroup.RemoveCondition(condition);
}
