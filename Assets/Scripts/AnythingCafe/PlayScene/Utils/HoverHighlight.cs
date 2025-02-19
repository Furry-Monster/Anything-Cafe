using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[AddComponentMenu("FrameMonster/Effects/HoverHighlight")]
public class HoverHighlight : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public Material glowMaterial; // 拖入发光材质
    private Material originalMaterial;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = _spriteRenderer.material;
    }

    private void OnMouseEnter() => _spriteRenderer.material = glowMaterial;

    private void OnMouseExit() => _spriteRenderer.material = originalMaterial;
}
