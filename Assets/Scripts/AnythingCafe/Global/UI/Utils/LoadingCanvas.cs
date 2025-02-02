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
        var canvas = GetComponent<Canvas>();
        var canvasScaler = GetComponent<CanvasScaler>();
        var graphicRaycaster = GetComponent<GraphicRaycaster>();
        var canvasGroup = GetComponent<CanvasGroup>();

        if (canvas == null || canvasScaler == null || graphicRaycaster == null || canvasGroup == null)
            throw new CustomErrorException("LoadingCanvas must have a Canvas, CanvasScaler, GraphicRaycaster, and CanvasGroup component.",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.CanvasValidateFailed));

    }
}
