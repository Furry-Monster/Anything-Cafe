using System;
using UnityEngine;

public class CustomErrorException : Exception
{
    public CustomErrorItem ErrorItem { get; set; }

    public CustomErrorException(string message, CustomErrorItem errorItem)
        : base(message)
    {
        this.ErrorItem = errorItem;
        switch (errorItem.Severity)
        {
            case ErrorSeverity.Info:
                Debug.Log($"[{errorItem.Code.ToString()}] {message}");
                break;
            case ErrorSeverity.Warning:
                Debug.LogWarning($"[{errorItem.Code.ToString()}] {message}");
                break;
            case ErrorSeverity.Error:
            case ErrorSeverity.ForceQuit:
                Debug.LogError($"[{errorItem.Code.ToString()}] {message}");
                break;
        }
    }
}
