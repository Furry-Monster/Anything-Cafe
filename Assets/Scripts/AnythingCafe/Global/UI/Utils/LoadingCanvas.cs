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
    /// ��鲢����ReactiveComponent�б�
    /// </summary>
    /// <param name="reactiveComponents"> ��ǰUIManager�е�ReactiveComponent�б� </param>
    /// <returns> ���º��ReactiveComponent�б� </returns>
    public List<ReactiveComponent> CheckComponents(List<ReactiveComponent> reactiveComponents)
    {
        var childrenComponent = transform.GetComponentsInChildren<ReactiveComponent>().ToList();
        if (childrenComponent.Count > reactiveComponents.Count)
        {
            // ȡ�
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
