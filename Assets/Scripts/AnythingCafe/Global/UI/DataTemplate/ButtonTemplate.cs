using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ButtonDataTemplate
{
    public string Text;
    [CanBeNull] public string Tooltip;
    public bool IsInteractable;
    public Func<bool> OnClick;

    public ButtonDataTemplate(string text, [CanBeNull] string tooltip, bool isInteractable, Action onClick = null)
    {
        this.Text = text;
        this.Tooltip = tooltip;
        this.IsInteractable = isInteractable;
        this.OnClick = () =>
        {
            onClick?.Invoke();
            return true;
        };
    }

    public ButtonDataTemplate(string text, [CanBeNull] string tooltip, bool isInteractable, Func<bool> onClick)
    {
        this.Text = text;
        this.Tooltip = tooltip;
        this.IsInteractable = isInteractable;
        this.OnClick = onClick;
    }
}
