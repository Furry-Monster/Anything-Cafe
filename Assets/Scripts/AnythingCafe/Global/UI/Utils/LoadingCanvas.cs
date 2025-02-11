using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("FrameMonster/UI/Utils/Loading Canvas")]
public class LoadingCanvas :
    MonoBehaviourSingleton<LoadingCanvas>,
    IInitializable
{
    public bool IsInitialized { get; set; }

    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        ValidateSelf();
    }

    /// <summary>
    /// ��鲢����ReactiveComponent�б�
    /// </summary>
    /// <param name="reactiveComponents"> ��ǰUIManager�е�ReactiveComponent�б� </param>
    /// <returns> ���º��ReactiveComponent�б� </returns>
    public void CheckComponents(List<ReactiveComponent> reactiveComponents)
    {
        var childrenComponent = transform.GetComponentsInChildren<ReactiveComponent>().ToList();

        if (childrenComponent.Count > reactiveComponents.Count)
        {
            // ȡ�
            var diff = childrenComponent.Except(reactiveComponents).ToList();
            foreach (var component in diff)
                throw new CustomErrorException(
                    $"[LoadingCanvas] ReactiveComponent {component.gameObject.name} is NOT in the LoadingComponent list.But exist in the scene",
                    new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.UICanvasInitFailed));
        }
        else if (childrenComponent.Count < reactiveComponents.Count)
        {
            // ȡ����
            var intersect = childrenComponent.Intersect(reactiveComponents).ToList();
            foreach (var component in intersect)
                throw new CustomErrorException(
                    $"[LoadingCanvas] ReactiveComponent {component.gameObject.name} is NOT in the scene list.But exist in the LoadingComponent",
                    new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.UICanvasInitFailed));
        }
    }

    private void ValidateSelf()
    {
        var canvas = GetComponent<Canvas>();
        var canvasScaler = GetComponent<CanvasScaler>();
        var graphicRaycaster = GetComponent<GraphicRaycaster>();
        var canvasGroup = GetComponent<CanvasGroup>();

        if (canvas == null || canvasScaler == null || graphicRaycaster == null || canvasGroup == null)
            throw new CustomErrorException("[LoadingCanvas] LoadingCanvas must have a Canvas, CanvasScaler, GraphicRaycaster, and CanvasGroup component.",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.CanvasValidateFailed));

    }
}
