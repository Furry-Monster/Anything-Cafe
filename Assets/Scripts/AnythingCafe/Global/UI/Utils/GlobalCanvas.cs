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
        ValidateSelf();
    }

    /// <summary>
    /// 检查并更新ReactiveComponent列表
    /// </summary>
    /// <param name="reactiveComponents"> 当前UIManager中的ReactiveComponent列表 </param>
    /// <returns> 更新后的ReactiveComponent列表 </returns>
    public List<ReactiveComponent> CheckComponents(List<ReactiveComponent> reactiveComponents)
    {
        var childrenComponent = transform.GetComponentsInChildren<ReactiveComponent>().ToList();
        if (childrenComponent.Count > reactiveComponents.Count)
        {
            // 取差集
            var diff = childrenComponent.Except(reactiveComponents).ToList();
            reactiveComponents.AddRange(diff);
            return reactiveComponents;
        }
    }

    private void ValidateSelf()
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
