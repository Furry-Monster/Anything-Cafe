using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class PlotObject : MonoBehaviour, IInteractable, IConditionalShow
{
    private bool _interactable = true;
    private bool _isInteracting;

    [SerializeField] private bool _checkConditionsOnStart = true;

    [SerializeField] private UnityEvent _onInteractionStart = new();
    [SerializeField] private UnityEvent _onInteractionEnd = new();
    [SerializeField] private UnityEvent _onConditionsFailed = new();

    private ConditionGroup _conditionGroup;

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
    public UnityEvent OnConditionsFailed => _onConditionsFailed;

    protected virtual void Awake()
    {
        _conditionGroup = GetComponent<ConditionGroup>();
        if (_conditionGroup == null)
        {
            _conditionGroup = gameObject.AddComponent<ConditionGroup>();
        }
    }

    protected virtual void Start()
    {
        ValidateComponents();

        if (_checkConditionsOnStart)
            CheckConditions();
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

        if (!CheckConditions())
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

    public void AddCondition(MonoBehaviour condition) => _conditionGroup.AddCondition(condition);

    public void RemoveCondition(MonoBehaviour condition) => _conditionGroup.RemoveCondition(condition);
}
