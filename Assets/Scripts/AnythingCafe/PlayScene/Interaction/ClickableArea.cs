using UnityEngine.EventSystems;

public class ClickableArea :
    PlotObject,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Interactable) return;


    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Interactable) return;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Interactable) return;
    }
}
