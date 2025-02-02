using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("FrameMonster/UI/Utils/Global Canvas")]
public class GlobalCanvas :
    MonoBehaviourSingleton<GlobalCanvas>,
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
                $"GlobalCanvas must have the ReactiveComponent {reactiveComponent.gameObject.name} in its children.",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.CanvasValidateFailed));
        }
    }

    private void ValidateComponents()
    {
        var requiredComponents = new Component[]
        {
            GetComponent<Canvas>(),
            GetComponent<CanvasScaler>(),
            GetComponent<GraphicRaycaster>(),
            GetComponent<CanvasGroup>()
        };

        if (requiredComponents.Any(component => component == null))
        {
            throw new CustomErrorException("LoadingCanvas must have a Canvas, CanvasScaler, GraphicRaycaster, and CanvasGroup component.",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.CanvasValidateFailed));
        }
    }
}
