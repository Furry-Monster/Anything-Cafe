using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MachineOutputArea : ClickableArea
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        try
        {
            base.OnPointerClick(eventData);

            if (eventData.button == PointerEventData.InputButton.Left)
            {
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[MachineKeyboardArea] message: {ex.Message}\n stacktrace: {ex.StackTrace}");
        }
    }
}
