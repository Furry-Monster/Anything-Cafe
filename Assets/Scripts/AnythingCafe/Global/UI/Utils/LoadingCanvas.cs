using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("FrameMonster/UI/Utils/Loading Canvas")]
public class LoadingCanvas :
    MonoBehaviourSingleton<LoadingCanvas>,
    IInitializable
{
    public void Init()
    {
        ValidateComponents();
    }

    public void CheckComponents(List<ReactiveComponent> reactiveComponents)
    {
        var childrenComponent = transform.GetComponentsInChildren<ReactiveComponent>().ToList();
        foreach (var reactiveComponent in reactiveComponents.Where(rc => !childrenComponent.Contains(rc)))
        {
            throw new CustomErrorException(
                $"LoadingCanvas must have the ReactiveComponent {reactiveComponent.gameObject.name} in its children.",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.CanvasValidateFailed));
        }
    }

    private void ValidateComponents()
    {
        var canvas = GetComponent<Canvas>();
        var canvasScaler = GetComponent<CanvasScaler>();
        var graphicRaycaster = GetComponent<GraphicRaycaster>();
        var canvasGroup = GetComponent<CanvasGroup>();

        if (canvas == null || canvasScaler == null || graphicRaycaster == null || canvasGroup == null)
            throw new CustomErrorException("LoadingCanvas must have a Canvas, CanvasScaler, GraphicRaycaster, and CanvasGroup component.",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.CanvasValidateFailed));

    }
}
