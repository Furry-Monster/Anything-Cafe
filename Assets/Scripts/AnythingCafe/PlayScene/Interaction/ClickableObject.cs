using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
[AddComponentMenu("Anything Cafe/GamePlay/Interaction/Clickable Object")]
public class ClickableObject : DynamicObject,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _hoverColor = new(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color _disabledColor = new(0.5f, 0.5f, 0.5f, 1f);

    private SpriteRenderer _spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisualState();
    }

    protected override void OnInteractableChanged()
    {
        base.OnInteractableChanged();
        UpdateVisualState();
    }

    /// <summary>
    /// Note:这个方法仅仅用于拦截鼠标点击事件，具体的交互逻辑请重载OnInteractBegin()方法
    /// </summary>
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (!Interactable) return;
        OnInteract();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!Interactable) return;
        UpdateVisualState(true);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!Interactable) return;
        UpdateVisualState();
    }

    protected virtual void UpdateVisualState(bool isHovered = false)
    {
        if (_spriteRenderer == null) return;

        if (!Interactable)
        {
            _spriteRenderer.color = _disabledColor;
        }
        else if (isHovered)
        {
            _spriteRenderer.color = _hoverColor;
        }
        else
        {
            _spriteRenderer.color = _normalColor;
        }
    }
}
