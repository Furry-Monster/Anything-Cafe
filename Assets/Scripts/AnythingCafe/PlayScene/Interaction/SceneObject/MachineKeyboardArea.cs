using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MachineKeyboardArea : ClickableArea
{
    public override async void OnPointerClick(PointerEventData eventData)
    {
        try
        {
            base.OnPointerClick(eventData);

            if (eventData.button == PointerEventData.InputButton.Left)
                await UIManager.Instance.OpenReactive<MachineUI>();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[MachineKeyboardArea] message: {ex.Message}\n stacktrace: {ex.StackTrace}");
        }
    }
}
